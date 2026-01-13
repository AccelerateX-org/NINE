using Hangfire;
using MyStik.TimeTable.Data;
using MyStik.TimeTable.Web.Helpers;
using MyStik.TimeTable.Web.Jobs;
using MyStik.TimeTable.Web.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using MyStik.TimeTable.DataServices;
using Microsoft.Ajax.Utilities;
using System.Runtime.InteropServices;

namespace MyStik.TimeTable.Web.Controllers
{
    public class StudyPlanPublishingModel
    {
        public Curriculum Curriculum { get; set; } 
        public Semester Semester { get; set; }
        public string Remark { get; set; }

        public bool RollForward { get; set; }
    }

    public class StudyPlanController : BaseController
    {
        [AllowAnonymous]
        public ActionResult Doc(Guid id, Guid? semId)
        {
            var sem = semId.HasValue? SemesterService.GetSemester(semId.Value):SemesterService.GetSemester(DateTime.Today);
            var curr = Db.Curricula.SingleOrDefault(x => x.Id == id);

            var subjects = Db.ModuleCourses.Where(x =>
                x.SubjectAccreditations.Any(c =>
                    c.Slot != null &&
                    c.Slot.AreaOption != null &&
                    c.Slot.AreaOption.Area.Curriculum.Id == curr.Id)).Include(moduleSubject => moduleSubject.Module).ToList();

            var modules = subjects.Select(x => x.Module).Distinct().ToList();


            var printModel = new StudyPlanViewModel
            {
                TimeStamp = DateTime.Now,
                Remark = "",
                Curriculum = curr,
                Semester = sem,
                Modules = modules,
            };

            var currentSemester = sem;
            var nextSemester = SemesterService.GetNextSemester(sem);
            var prevSemester = SemesterService.GetPreviousSemester(sem);

            ViewBag.CurrentSemester = currentSemester;
            ViewBag.NextSemester = nextSemester;
            ViewBag.PrevSemester = prevSemester;


            return View(printModel);
        }


        // GET: StudyPlan
        public ActionResult Details(Guid currId, Guid semId)
        {
            var semester = Db.Semesters.SingleOrDefault(x => x.Id == semId);
            var curriculum = Db.Curricula.SingleOrDefault(x => x.Id == currId);

            var model = new StudyPlanViewModel
            {
                Curriculum = curriculum,
                Semester = semester
                //    Modules = modules
            };

            // hier muss überprüft werden, ob der aktuelle Benutzer
            // der Fakultät des Studiengangs angehört oder nicht
            ViewBag.UserRight = GetUserRight(model.Curriculum.Organiser);


            return View(model);
        }

        public ActionResult Admin(Guid currId, Guid semId)
        {
            var semester = Db.Semesters.SingleOrDefault(x => x.Id == semId);
            var curriculum = Db.Curricula.SingleOrDefault(x => x.Id == currId);

            var model = new StudyPlanViewModel
            {
                Curriculum = curriculum,
                Semester = semester
                //    Modules = modules
            };

            // hier muss überprüft werden, ob der aktuelle Benutzer
            // der Fakultät des Studiengangs angehört oder nicht
            ViewBag.UserRight = GetUserRight(model.Curriculum.Organiser);


            return View(model);
        }



        public ActionResult Publish(Guid currId, Guid semId)
        {
            var model = new StudyPlanPublishingModel
            {
                Curriculum = Db.Curricula.SingleOrDefault(x => x.Id == currId),
                Semester = Db.Semesters.SingleOrDefault(y => y.Id == semId)
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult Publish(StudyPlanPublishingModel model)
        {
            var user = GetCurrentUser();

            var curr = Db.Curricula.SingleOrDefault(x => x.Id == model.Curriculum.Id);
            var semester = Db.Semesters.SingleOrDefault(x => x.Id == model.Semester.Id);
            var member =
                curr.Organiser.Members.SingleOrDefault(m =>
                    !string.IsNullOrEmpty(m.UserId) && m.UserId.Equals(user.Id));

            var now = DateTime.Now;

            // alle Module veröffentlichen
            var nextSemester = SemesterService.GetNextSemester(semester);

            var subjects = Db.ModuleCourses.Where(x =>
                x.SubjectAccreditations.Any(c =>
                    c.Slot != null &&
                    c.Slot.AreaOption != null &&
                    c.Slot.AreaOption.Area.Curriculum.Id == curr.Id)).ToList();

            var modules = subjects.Select(x => x.Module).Distinct().ToList();

            foreach (var module in modules)
            {
                // aktuelle Beschreibung des Semesters
                var currentDesc = module.Descriptions.Where(x => x.Semester.Id == semester.Id && x.ChangeLog != null)
                    .OrderByDescending(x => x.ChangeLog.LastEdited).FirstOrDefault();

                if (currentDesc != null)
                {
                    // Modulbeschreibung veröffentlichen
                    currentDesc.ChangeLog.Approved = now;
                    currentDesc.ChangeLog.UserIdApproval = user.Id;

                    // Fortschreibung
                    // die aktuelle Beschreibung des Folgesemesters
                    if (model.RollForward)
                    {
                        var nextDesc = module.Descriptions
                            .Where(x => x.Semester.Id == nextSemester.Id && x.ChangeLog != null)
                            .OrderByDescending(x => x.ChangeLog.LastEdited).FirstOrDefault();

                        // neue Beschreibung anlegen
                        if (nextDesc == null)
                        {
                            nextDesc = new ModuleDescription
                            {
                                Description = string.Empty,
                                Module = module,
                                Semester = nextSemester
                            };

                            var descChangeLog = new ChangeLog
                            {
                                Created = now,
                                LastEdited = now,
                                UserIdAmendment = currentDesc.ChangeLog.UserIdAmendment
                            };

                            nextDesc.ChangeLog = descChangeLog;

                            Db.ModuleDescriptions.Add(nextDesc);
                            Db.ChangeLogs.Add(descChangeLog);
                        }

                        // Modulbeschreibung fortschreiben nur wenn Text leer und nicht veröffentlicht
                        if (nextDesc.ChangeLog.Approved == null)
                        {
                            if (string.IsNullOrEmpty(nextDesc.Description))
                            {
                                nextDesc.Description = currentDesc.Description;
                            }
                            if (string.IsNullOrEmpty(nextDesc.DescriptionEn))
                            {
                                nextDesc.DescriptionEn = currentDesc.DescriptionEn;
                            }

                            nextDesc.ChangeLog.LastEdited = now;
                        }
                    }
                }

                foreach (var accr in module.ExaminationOptions.ToList())
                {
                    foreach (var exam in accr.ExaminationDescriptions.Where(x => x.Semester.Id == semester.Id).ToList())
                    {
                        if (exam.ChangeLog == null)
                        {
                            var examChangeLog = new ChangeLog
                            {
                                Created = now,
                                LastEdited = now,
                                UserIdAmendment = currentDesc.ChangeLog.UserIdAmendment
                            };

                            exam.ChangeLog = examChangeLog;
                            Db.ChangeLogs.Add(examChangeLog);
                        }

                        exam.ChangeLog.Approved = now;
                        exam.ChangeLog.UserIdApproval = user.Id;

                        // Fortschreibung der Prüfungsangebote
                        // wenn schon welche vorhanden, dann nicht fortschreiben
                        if (model.RollForward)
                        {
                            var hasNextExams = accr.ExaminationDescriptions.Any(x => x.Semester.Id == nextSemester.Id);

                            if (!hasNextExams)
                            {
                                var nextExam = new ExaminationDescription
                                {
                                    Semester = nextSemester,
                                    FirstExminer = exam.FirstExminer,
                                    SecondExaminer = exam.SecondExaminer,
                                    Conditions = exam.Conditions,
                                    Description = exam.Description,
                                    Duration = exam.Duration,
                                    Utilities = exam.Utilities,
                                    ExaminationOption = exam.ExaminationOption,
                                };

                                var examChangeLog = new ChangeLog
                                {
                                    Created = now,
                                    LastEdited = now,
                                    UserIdAmendment = exam.ChangeLog.UserIdAmendment
                                };

                                nextExam.ChangeLog = examChangeLog;
                                Db.ChangeLogs.Add(examChangeLog);
                                Db.ExaminationDescriptions.Add(nextExam);
                            }
                        }
                    }
                }
            }

            Db.SaveChanges();


            return RedirectToAction("Doc", "StudyPlan", new { id = curr.Id, semId = semester.Id });
        }

        public ActionResult Unpublish(Guid currId, Guid semId)
        {
            var curr = Db.Curricula.SingleOrDefault(x => x.Id == currId);
            var semester = Db.Semesters.SingleOrDefault(x => x.Id == semId);

            var subjects = Db.ModuleCourses.Where(x =>
                x.SubjectAccreditations.Any(c =>
                    c.Slot != null &&
                    c.Slot.AreaOption != null &&
                    c.Slot.AreaOption.Area.Curriculum.Id == curr.Id)).ToList();

            var modules = subjects.Select(x => x.Module).Distinct().ToList();

            foreach (var module in modules)
            {
                // aktuelle Beschreibung des Semesters
                var descs = module.Descriptions.Where(x => x.Semester.Id == semester.Id && x.ChangeLog != null)
                    .ToList();

                foreach (var d in descs)
                {
                    d.ChangeLog.Approved = null;
                    d.ChangeLog.IsVisible = true;
                }

                foreach (var accr in module.ExaminationOptions)
                {
                    var exams = accr.ExaminationDescriptions
                        .Where(x => x.Semester.Id == semester.Id && x.ChangeLog != null).ToList();
                    foreach (var exam in exams)
                    {
                        exam.ChangeLog.Approved = null;
                        exam.ChangeLog.IsVisible = true;
                    }
                }
            }

            Db.SaveChanges();

            return RedirectToAction("Details", new { currId = currId, semId = semId });
        }

        public ActionResult SlotPlan(Guid currId, Guid semId)
        {
            var curr = Db.Curricula.Include(curriculum => curriculum.Organiser).Include(curriculum1 => curriculum1.Areas.Select(curriculumArea => curriculumArea.Options.Select(areaOption => areaOption.Slots.Select(curriculumSlot =>
                curriculumSlot.LabelSet.ItemLabels)))).SingleOrDefault(x => x.Id == currId);

            var model = new CurriculumViewModel();

            model.Curriculum = curr;
            model.Semester = SemesterService.GetSemester(semId);
            model.Organiser = curr.Organiser;

            // die Labels sammlen
            var labels = new List<ItemLabel>();
            foreach (var area in curr.Areas)
            {
                foreach (var areaOption in area.Options)
                {
                    foreach (var slot in areaOption.Slots)
                    {
                        if (slot.LabelSet == null || !slot.LabelSet.ItemLabels.Any()) continue;
                        foreach (var label in slot.LabelSet.ItemLabels)
                        {
                            if (!labels.Contains(label))
                            {
                                labels.Add(label);
                            }
                        }

                    }
                }
            }

            ViewBag.FilterLabels = labels.OrderBy(x => x.Name);

            model.Areas = new List<AreaSelectViewModel>();

            var allAreasWithOptions = curr.Areas.Where(x => x.Options.Count > 1).ToList();
            foreach (var area in allAreasWithOptions)
            {
                var optList = area.Options.ToList();
                optList.Shuffle();
                var option = optList.First();

                var selectOption = new AreaSelectViewModel
                {
                    Area = area,
                    Option = option,
                };

                model.Areas.Add(selectOption);
            }

            // hier muss überprüft werden, ob der aktuelle Benutzer
            // der Fakultät des Studiengangs angehört oder nicht
            ViewBag.UserRight = GetUserRight(model.Curriculum.Organiser);


            return View(model);
        }


        [HttpPost]
        public PartialViewResult LoadModulePlan(Guid currId, string label)
        {
            var curr = Db.Curricula.SingleOrDefault(x => x.Id == currId);

            var model = new CurriculumViewModel();

            model.Curriculum = curr;

            model.FilterLabel = curr.LabelSet.ItemLabels.FirstOrDefault(x => x.Name.Equals(label));


            return PartialView("_ModulePlan", model);
        }


        [HttpPost]
        public PartialViewResult LoadModulePlanAreas(Guid currId, Guid semId, Guid[] optIds)
        {
            var curr = Db.Curricula.SingleOrDefault(x => x.Id == currId);
            var sem = SemesterService.GetSemester(semId);

            var model = new CurriculumViewModel();

            model.Curriculum = curr;
            model.Semester = sem;

            model.Areas = new List<AreaSelectViewModel>();

            foreach (var optId in optIds)
            {
                var option = Db.AreaOptions.SingleOrDefault(x => x.Id == optId);

                var selectOption = new AreaSelectViewModel
                {
                    Area = option.Area,
                    Option = option,
                };

                model.Areas.Add(selectOption);
            }

            return PartialView("_ModuleAreaPlan", model);
        }


        public ActionResult AssignmentPlan(Guid currId, Guid semId)
        {
            var curr = Db.Curricula.SingleOrDefault(x => x.Id == currId);
            var sem = SemesterService.GetSemester(semId);

            var model = new CurriculumViewModel();

            model.Curriculum = curr;
            model.Semester = sem;

            return View(model);
        }

        public ActionResult Slot(Guid slotId, Guid semId)
        {
            var slot = Db.CurriculumSlots.SingleOrDefault(x => x.Id == slotId);

            if (slot.CurriculumSection != null)
            {
                ViewBag.UserRight = GetUserRight(slot.CurriculumSection.Curriculum.Organiser);
            }

            if (slot.AreaOption != null)
            {
                ViewBag.UserRight = GetUserRight(slot.AreaOption.Area.Curriculum.Organiser);
            }

            ViewBag.Semester = SemesterService.GetSemester(semId);

            return View(slot);
        }
    }
}