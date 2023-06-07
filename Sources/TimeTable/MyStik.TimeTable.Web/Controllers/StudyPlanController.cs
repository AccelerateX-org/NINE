using Hangfire;
using MyStik.TimeTable.Data;
using MyStik.TimeTable.Web.Helpers;
using MyStik.TimeTable.Web.Jobs;
using MyStik.TimeTable.Web.Models;
using PdfSharp;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using TheArtOfDev.HtmlRenderer.PdfSharp;

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
        // GET: StudyPlan
        public ActionResult Details(Guid currId, Guid semId)
        {
            /*
            var modules = Db.CurriculumModules.Where(x =>
                x.Accreditations.Any(c =>
                    c.Slot != null &&
                    c.Slot.AreaOption != null &&
                    c.Slot.AreaOption.Area.Curriculum.Id == currId)).ToList();
            */

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
                Semester =Db.Semesters.SingleOrDefault(y => y.Id == semId)
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult Publish(StudyPlanPublishingModel model)
        {
            var user = GetCurrentUser();

            var curr = Db.Curricula.SingleOrDefault(x => x.Id == model.Curriculum.Id);
            var semester = Db.Semesters.SingleOrDefault(x => x.Id == model.Semester.Id);
            var member = GetMyMembership();

            var now = DateTime.Now;

            // alle Module veröffentlichen
            var nextSemester = SemesterService.GetNextSemester(semester);

            var modules = Db.CurriculumModules.Where(x =>
                x.Accreditations.Any(c =>
                    c.Slot != null &&
                    c.Slot.AreaOption != null &&
                    c.Slot.AreaOption.Area.Curriculum.Id == curr.Id)).ToList();

            foreach (var module in modules)
            {
                // aktuelle Beschreibung des Semesters
                var currentDesc = module.Descriptions.Where(x => x.Semester.Id == semester.Id && x.ChangeLog != null).OrderByDescending(x => x.ChangeLog.LastEdited).FirstOrDefault();

                if (currentDesc != null)
                {
                    // Modulbeschreibung veröffentlichen
                    currentDesc.ChangeLog.Approved = now;
                    currentDesc.ChangeLog.UserIdApproval = user.Id;

                    // Fortschreibung
                    // die aktuelle Beschreibung des Folgesemesters
                    if (model.RollForward)
                    {
                        var nextDesc = module.Descriptions.Where(x => x.Semester.Id == nextSemester.Id && x.ChangeLog != null).OrderByDescending(x => x.ChangeLog.LastEdited).FirstOrDefault();

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
                        if (nextDesc.ChangeLog.Approved == null && string.IsNullOrEmpty(nextDesc.Description))
                        {
                            nextDesc.Description = currentDesc.Description;
                            nextDesc.ChangeLog.LastEdited = now;
                        }
                    }
                }
                foreach (var accr in module.Accreditations.Where(x => x.Slot.AreaOption.Area.Curriculum.Id == curr.Id).ToList())
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
                                    Accreditation = accr,
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




            bool useBackground = false;

            if (useBackground)
            {
                var job = new StudyPlanPrintJobDescription
                {
                    CurriculumId = model.Curriculum.Id,
                    SemesterId = model.Semester.Id,
                    MemberId = member.Id,
                    Remark = model.Remark
                };

                BackgroundJob.Enqueue<StudyPlanPrintJob>(x => x.Print(job));
            }
            else
            {
                var printModel = new StudyPlanViewModel
                {
                    TimeStamp = DateTime.Now,
                    Remark = model.Remark,
                    Curriculum = curr,
                    Semester = semester,
                    Modules = modules,
                };


                var viewName = "_StudyPlanPrintOutForeground";

                var html = this.RenderViewToString(viewName, printModel);

                var pdf = PdfGenerator.GeneratePdf(html, PageSize.A4);

                var storage = new BinaryStorage
                {
                    Category = "Studienplan",
                    FileType = "application/pdf",
                    Name = $"{semester.Name}_{curr.ShortName}_Studienplan_{printModel.TimeStamp.ToString("yyyyMMdd")}.pdf",
                    Created = DateTime.Now,
                    Description = "Automatisch erzeugt",
                };

                using (var stream = new MemoryStream())
                {
                    pdf.Save(stream, false);
                    stream.Position = 0;
                    storage.BinaryData = stream.GetBuffer();
                }

                Db.Storages.Add(storage);

                var adv = new Advertisement
                {
                    Title = $"Studienplan {curr.ShortName} für Semester {semester.Name}",
                    Description = model.Remark,
                    Owner = member,
                    Created = printModel.TimeStamp,
                    VisibleUntil = semester.EndCourses,
                    Attachment = storage,
                };

                Db.Advertisements.Add(adv);

                var positing = new BoardPosting
                {
                    Advertisement = adv,
                    BulletinBoard = curr.BulletinBoard,
                    Published = printModel.TimeStamp
                };

                Db.BoardPosts.Add(positing);

                Db.SaveChanges();
            }


            return RedirectToAction("Curriculum", "BulletinBoards", new { id = model.Curriculum.Id });
        }
    }
}