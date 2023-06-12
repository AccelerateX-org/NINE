using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Web.Mvc;
using MyStik.TimeTable.Data;
using MyStik.TimeTable.Web.Models;
using MyStik.TimeTable.Web.Utils;
using Newtonsoft.Json.Converters;
using Org.BouncyCastle.Asn1.Crmf;
using PdfSharp;
using TheArtOfDev.HtmlRenderer.PdfSharp;

namespace MyStik.TimeTable.Web.Controllers
{
    public class ModuleDescriptionController : BaseController
    {
        public ActionResult Details(Guid id)
        {

            var module = Db.CurriculumModules.SingleOrDefault(x => x.Id == id);
            var semester = SemesterService.GetSemester(DateTime.Today);
            var nextSemester = SemesterService.GetNextSemester(semester);
            var prevSemester = SemesterService.GetPreviousSemester(semester);


            ViewBag.UserRight = GetUserRight(module.Catalog.Organiser);

            var semesterList = new List<SelectListItem>();

            SelectListItem semItem = null;
            if (nextSemester != null)
            {
                semItem = new SelectListItem
                {
                    Text = nextSemester.Name,
                    Value = nextSemester.Id.ToString(),
                    Selected = false
                };
                semesterList.Add(semItem);
            }

            semItem = new SelectListItem
            {
                Text = semester.Name,
                Value = semester.Id.ToString(),
                Selected = true
            };
            semesterList.Add(semItem);

            semItem = new SelectListItem
            {
                Text = prevSemester.Name,
                Value = prevSemester.Id.ToString(),
                Selected = false
            };
            semesterList.Add(semItem);

            ViewBag.SemesterList = semesterList;


            return View(module);
        }

        [HttpPost]
        public PartialViewResult Description(Guid moduleId, Guid semId)
        {
            var module = Db.CurriculumModules.SingleOrDefault(x => x.Id == moduleId);
            var semester = SemesterService.GetSemester(semId);


            // die aktuell veröffentlichte Fassung
            var lastPublished = module.Descriptions
                .Where(x =>
                    x.Semester.Id == semester.Id && x.ChangeLog != null && x.ChangeLog.Approved != null)
                .OrderByDescending(x => x.ChangeLog.Approved)
                .FirstOrDefault();

            // die aktuell veröffentlichten Prüfungsangebote
            var exams = new List<ExaminationDescription>();

            foreach (var accr in module.Accreditations)
            {
                var subExams = accr.ExaminationDescriptions
                    .Where(x => x.Semester.Id == semester.Id && x.ChangeLog != null && x.ChangeLog.Approved != null)
                    .ToList();
                exams.AddRange(subExams);
            }

            ViewBag.UserRight = GetUserRight(module.Catalog.Organiser);
            ViewBag.CurrentSemester = SemesterService.GetSemester(DateTime.Today);

            var model = new ModuleSemesterView
            {
                CurriculumModule = module,
                Semester = semester,
                ModuleDescription = lastPublished,
                Exams = exams
            };


            return PartialView("_Semester", model);
        }



        public ActionResult ContentChange(Guid id)
        {
            var desc = Db.ModuleDescriptions.SingleOrDefault(x => x.Id == id);

            ViewBag.UserRight = GetUserRight(desc.Module.Catalog.Organiser);
            ViewBag.CurrentSemester = SemesterService.GetSemester(DateTime.Today);

            return View(desc);
        }


        public ActionResult Edit(Guid id)
        {
            var desc = Db.ModuleDescriptions.SingleOrDefault(x => x.Id == id);

            var model = new ModuleDescriptionEditModel()
            {
                ModuleDescription = desc,
                DescriptionText = desc.Description,
                DescriptionTextEn = desc.DescriptionEn
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(ModuleDescriptionEditModel model)
        {
            var user = GetCurrentUser();
            var desc = Db.ModuleDescriptions.SingleOrDefault(x => x.Id == model.ModuleDescription.Id);

            desc.Description = model.DescriptionText;
            desc.DescriptionEn = model.DescriptionTextEn;

            var changeLog = desc.ChangeLog;

            if (changeLog == null)
            {
                changeLog = new ChangeLog
                {
                    Created = DateTime.Now
                };
                desc.ChangeLog = changeLog;
                Db.ChangeLogs.Add(changeLog);
            }

            changeLog.LastEdited = DateTime.Now;
            changeLog.UserIdAmendment = user.Id;

            Db.SaveChanges();

            return RedirectToAction("Descriptions", new { moduleId = desc.Module.Id, semId = desc.Semester.Id });
        }

        public ActionResult Copy(Guid id)
        {
            var user = GetCurrentUser();

            var desc = Db.ModuleDescriptions.SingleOrDefault(x => x.Id == id);

            var sem = desc.Semester;
            var prevSem = SemesterService.GetPreviousSemester(sem);

            var prevDesc = desc.Module.Descriptions.FirstOrDefault(x => x.Semester.Id == prevSem.Id);

            if (prevDesc != null)
            {
                desc.Description = prevDesc.Description;

                var changeLog = desc.ChangeLog;

                if (changeLog == null)
                {
                    changeLog = new ChangeLog
                    {
                        Created = DateTime.Now
                    };
                    desc.ChangeLog = changeLog;
                    Db.ChangeLogs.Add(changeLog);
                }

                changeLog.LastEdited = DateTime.Now;
                changeLog.UserIdAmendment = user.Id;

                Db.SaveChanges();
            }

            return RedirectToAction("ContentChange", new { id = desc.Id });
        }


        public PartialViewResult ChangeSemester(Guid semId)
        {
            var semester = SemesterService.GetSemester(semId);
            Session["SemesterId"] = semId.ToString();

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="opportunityId"></param>
        /// <param name="targetSubjectId"></param>
        /// <returns></returns>
        public ActionResult MoveCourse2Subject(Guid opportunityId, Guid targetSubjectId)
        {
            var opportunity = Db.SubjectOpportunities.SingleOrDefault(x => x.Id == opportunityId);
            var targetSubject = Db.ModuleCourses.SingleOrDefault(x => x.Id == targetSubjectId);

            var module = opportunity.Subject.Module;

            opportunity.Subject = targetSubject;
            Db.SaveChanges();


            return RedirectToAction("Details", new { id = module.Id });
        }

        public ActionResult CreateExamination(Guid moduleId, Guid semId)
        {
            var module = Db.CurriculumModules.SingleOrDefault(x => x.Id == moduleId);
            var semester = SemesterService.GetSemester(semId);


            var model = new ExaminationEditModel();

            model.moduleId = module.Id;
            model.semesterId = semester.Id;
            if (module.Accreditations.Any())
            {
                var accr = module.Accreditations.First();
                model.accredidationId = accr.Id;

                if (accr.Slot.CurriculumSection != null)
                {
                    model.orgId = accr.Slot.CurriculumSection.Curriculum.Organiser.Id;
                }
                else
                {
                    model.orgId = accr.Slot.AreaOption.Area.Curriculum.Organiser.Id;
                }

                model.Accreditation = accr;
            }

            model.Module = module;
            model.Semester = semester;

            ViewBag.ExamOptions = module.ExaminationOptions
                .Select(x => new SelectListItem
                {
                    Text = x.FullName,
                    Value = x.Id.ToString()

                });


            return View(model);
        }

        [HttpPost]
        public ActionResult CreateExamination(ExaminationEditModel model)
        {
            var user = GetCurrentUser();
            var module = Db.CurriculumModules.SingleOrDefault(x => x.Id == model.moduleId);
            var accr = Db.Accreditations.SingleOrDefault(x => x.Id == model.accredidationId);
            var semester = Db.Semesters.SingleOrDefault(x => x.Id == model.semesterId);
            var examOption = Db.ExaminationOptions.SingleOrDefault(x => x.Id == model.examOptId);
            var firstMember = Db.Members.SingleOrDefault(x => x.Id == model.firstMemberId);
            var secondMember = Db.Members.SingleOrDefault(x => x.Id == model.secondMemberId);


            var changeLog = new ChangeLog
            {
                Created = DateTime.Now
            };
            changeLog.LastEdited = DateTime.Now;
            changeLog.UserIdAmendment = user.Id;
            Db.ChangeLogs.Add(changeLog);

            var examDesc = new ExaminationDescription
            {
                Semester = semester,
                Accreditation = accr,
                ChangeLog = changeLog,
                ExaminationOption = examOption,
                FirstExminer = firstMember,
                SecondExaminer = secondMember,
                Conditions = model.Conditions,
                Utilities = model.Utilities
            };

            accr.ExaminationDescriptions.Add(examDesc);

            Db.ExaminationDescriptions.Add(examDesc);
            Db.SaveChanges();


            return RedirectToAction("Details", new { id = model.moduleId, });
        }

        public ActionResult EditExamination(Guid id)
        {
            var examDesc = Db.ExaminationDescriptions.SingleOrDefault(x => x.Id == id);
            var semester = examDesc.Semester;
            var module = examDesc.Accreditation.Module;
            var accr = examDesc.Accreditation;

            var model = new ExaminationEditModel();

            model.examinationId = examDesc.Id;
            model.moduleId = module.Id;
            model.semesterId = examDesc.Semester.Id;
            model.accredidationId = examDesc.Accreditation.Id;

            if (examDesc.Accreditation.Slot.CurriculumSection != null)
            {
                model.orgId = examDesc.Accreditation.Slot.CurriculumSection.Curriculum.Organiser.Id;
            }
            else
            {
                model.orgId = examDesc.Accreditation.Slot.AreaOption.Area.Curriculum.Organiser.Id;
            }

            model.firstMemberId = examDesc.FirstExminer?.Id ?? Guid.Empty;
            model.secondMemberId = examDesc.SecondExaminer?.Id ?? Guid.Empty;
            model.Conditions = examDesc.Conditions;
            model.Utilities = examDesc.Utilities;

            model.Accreditation = accr;
            model.Module = module;
            model.Semester = semester;
            model.FirstMember = examDesc.FirstExminer;
            model.SecondMember = examDesc.SecondExaminer;


            ViewBag.ExamOptions = module.ExaminationOptions
                .Select(x => new SelectListItem
                {
                    Text = x.FullName,
                    Value = x.Id.ToString()

                });


            return View(model);
        }

        [HttpPost]
        public ActionResult EditExamination(ExaminationEditModel model)
        {
            var user = GetCurrentUser();
            var examDesc = Db.ExaminationDescriptions.SingleOrDefault(x => x.Id == model.examinationId);

            var module = Db.CurriculumModules.SingleOrDefault(x => x.Id == model.moduleId);
            var accr = Db.Accreditations.SingleOrDefault(x => x.Id == model.accredidationId);
            var semester = Db.Semesters.SingleOrDefault(x => x.Id == model.semesterId);

            var examOption = Db.ExaminationOptions.SingleOrDefault(x => x.Id == model.examOptId);
            var firstMember = Db.Members.SingleOrDefault(x => x.Id == model.firstMemberId);
            var secondMember = Db.Members.SingleOrDefault(x => x.Id == model.secondMemberId);


            var changeLog = examDesc.ChangeLog;

            if (changeLog == null)
            {
                changeLog = new ChangeLog
                {
                    Created = DateTime.Now
                };
                Db.ChangeLogs.Add(changeLog);
                examDesc.ChangeLog = changeLog;
            }

            changeLog.LastEdited = DateTime.Now;
            changeLog.UserIdAmendment = user.Id;

            examDesc.ExaminationOption = examOption;
            examDesc.FirstExminer = firstMember;
            examDesc.SecondExaminer = secondMember;
            examDesc.Conditions = model.Conditions;
            examDesc.Utilities = model.Utilities;

            Db.Entry(examDesc).State = EntityState.Modified;

            Db.SaveChanges();

            return RedirectToAction("Details", new { id = model.moduleId, });
        }

        public ActionResult DeleteExamination(Guid id)
        {
            var examDesc = Db.ExaminationDescriptions.SingleOrDefault(x => x.Id == id);

            var module = examDesc.Accreditation.Module;

            if (examDesc.ChangeLog != null)
            {
                Db.ChangeLogs.Remove(examDesc.ChangeLog);
            }

            Db.ExaminationDescriptions.Remove(examDesc);
            Db.SaveChanges();

            return RedirectToAction("Details", new { id = module.Id, });
        }

        public ActionResult Descriptions(Guid moduleId, Guid semId)
        {
            var module = Db.CurriculumModules.SingleOrDefault(x => x.Id == moduleId);
            var semester = SemesterService.GetSemester(semId);

            var allDesc = module.Descriptions
                .Where(x => x.Semester.Id == semId && x.ChangeLog != null)
                .OrderByDescending(x => x.ChangeLog.LastEdited).ToList();

            // check die erste
            // publiziert => Button neue Fassung
            // nicht publiziert => Buttons ändern | publizieren

            var badDesc = module.Descriptions
                .Where(x => x.Semester.Id == semId && x.ChangeLog == null).ToList();

            var model = new ModuleDescriptionsViewModel
            {
                Module = module,
                Semester = semester,
                ModuleDescriptions = allDesc,
                BadModuleDescriptions = badDesc
            };

            return View(model);
        }

        public ActionResult Publish(Guid id)
        {
            var user = GetCurrentUser();

            var desc = Db.ModuleDescriptions.SingleOrDefault(x => x.Id == id);

            if (desc.ChangeLog != null)
            {
                desc.ChangeLog.Approved = DateTime.Now;
                desc.ChangeLog.UserIdApproval = user.Id;
                Db.SaveChanges();
            }

            return RedirectToAction("Descriptions", new { moduleId = desc.Module.Id, semId = desc.Semester.Id });
        }

        public ActionResult PublishExamination(Guid id)
        {
            var user = GetCurrentUser();

            var desc = Db.ExaminationDescriptions.SingleOrDefault(x => x.Id == id);

            if (desc.ChangeLog != null)
            {
                desc.ChangeLog.Approved = DateTime.Now;
                desc.ChangeLog.UserIdApproval = user.Id;
                Db.SaveChanges();
            }

            return RedirectToAction("Exams", new { moduleId = desc.Accreditation.Module.Id, semId = desc.Semester.Id });
        }


        public ActionResult FollowUp(Guid id)
        {
            var user = GetCurrentUser();

            var desc = Db.ModuleDescriptions.SingleOrDefault(x => x.Id == id);

            var followUpDesc = new ModuleDescription
            {
                Description = desc.Description,
                Module = desc.Module,
                Semester = desc.Semester
            };

            var changeLog = new ChangeLog
            {
                Created = DateTime.Now,
                LastEdited = DateTime.Now,
                UserIdAmendment = user.Id,
            };

            followUpDesc.ChangeLog = changeLog;

            Db.ChangeLogs.Add(changeLog);
            Db.ModuleDescriptions.Add(followUpDesc);

            Db.SaveChanges();

            return RedirectToAction("Descriptions", new { moduleId = desc.Module.Id, semId = desc.Semester.Id });
        }

        public ActionResult Init(Guid moduleId, Guid semId)
        {
            var user = GetCurrentUser();

            var module = Db.CurriculumModules.SingleOrDefault(x => x.Id == moduleId);
            var semester = SemesterService.GetSemester(semId);

            var desc = new ModuleDescription
            {
                Description = string.Empty,
                Module = module,
                Semester = semester
            };

            var changeLog = new ChangeLog
            {
                Created = DateTime.Now,
                LastEdited = DateTime.Now,
                UserIdAmendment = user.Id,
            };

            desc.ChangeLog = changeLog;

            Db.ChangeLogs.Add(changeLog);
            Db.ModuleDescriptions.Add(desc);
            Db.SaveChanges();

            return RedirectToAction("Descriptions", new { moduleId = desc.Module.Id, semId = desc.Semester.Id });
        }


        public FileResult DownloadPdf(Guid moduleId, Guid semId)
        {
            var module = Db.CurriculumModules.SingleOrDefault(x => x.Id == moduleId);
            var semester = SemesterService.GetSemester(semId);

            var desc = module.Descriptions.FirstOrDefault(x => x.Semester.Id == semester.Id);

            ViewBag.UserRight = GetUserRight(module.Catalog.Organiser);
            ViewBag.CurrentSemester = SemesterService.GetSemester(DateTime.Today);

            var model = new ModuleSemesterView
            {
                CurriculumModule = module,
                Semester = semester,
                ModuleDescription = desc
            };



            var stream = new MemoryStream();
            var html = this.RenderViewToString("_ModuleDescriptionPrintOut", model);
            var pdf = PdfGenerator.GeneratePdf(html, PageSize.A4);
            pdf.Save(stream, false);

            // Stream zurücksetzen
            stream.Position = 0;

            return File(stream.GetBuffer(), "application/pdf", "Modulbeschreibung.pdf");
        }

        public ActionResult Exams(Guid moduleId, Guid semId)
        {
            var module = Db.CurriculumModules.SingleOrDefault(x => x.Id == moduleId);
            var semester = SemesterService.GetSemester(semId);

            // die aktuell veröffentlichten Prüfungsangebote
            var exams = new List<ExaminationDescription>();

            foreach (var accr in module.Accreditations)
            {
                var subExams = accr.ExaminationDescriptions
                    .Where(x => x.Semester.Id == semester.Id && x.ChangeLog != null)
                    .ToList();
                exams.AddRange(subExams);
            }


            var model = new ModuleDescriptionsViewModel
            {
                Module = module,
                Semester = semester,
                Exams = exams
            };

            return View(model);
        }


        public ActionResult ExaminationForms(Guid id)
        {
            var module = Db.CurriculumModules.SingleOrDefault(x => x.Id == id);

            var model = new ModuleDescriptionsViewModel
            {
                Module = module,
            };

            return View(model);
        }

        public ActionResult CreateExaminationOption(Guid id)
        {
            var module = Db.CurriculumModules.SingleOrDefault(x => x.Id == id);

            var exam = new ExaminationOption
            {
                Module = module
            };

            return View(exam);
        }

        [HttpPost]
        public ActionResult CreateExaminationOption(ExaminationOption model)
        {
            var module = Db.CurriculumModules.SingleOrDefault(x => x.Id == model.Module.Id);

            var exam = new ExaminationOption
            {
                Module = module,
                Name = model.Name,
            };

            Db.ExaminationOptions.Add(exam);
            Db.SaveChanges();

            return RedirectToAction("ExaminationForms", new { id = exam.Module.Id });
        }


        public ActionResult EditExaminationOption(Guid id)
        {
            var exam = Db.ExaminationOptions.SingleOrDefault(x => x.Id == id);

            return View(exam);
        }

        [HttpPost]
        public ActionResult EditExaminationOption(ExaminationOption model)
        {
            var exam = Db.ExaminationOptions.SingleOrDefault(x => x.Id == model.Id);

            exam.Name = model.Name;
            Db.SaveChanges();

            return RedirectToAction("ExaminationForms", new { id = exam.Module.Id });
        }

        public ActionResult CreateExaminationFraction(Guid id)
        {
            var exam = Db.ExaminationOptions.SingleOrDefault(x => x.Id == id);

            var model = new ExaminationFractionViewModel
            {
                Option = exam
            };

            var examinationForms =
                Db.ExaminationForms.Select(x => new SelectListItem
                {
                    Text = x.ShortName,
                    Value = x.Id.ToString()
                });

            ViewBag.ExamOptions = examinationForms;

            if (examinationForms.Any())
            {
                model.ExaminationTypeId = Guid.Parse(examinationForms.First().Value);
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult CreateExaminationFraction(ExaminationFractionViewModel model)
        {
            var exam = Db.ExaminationOptions.SingleOrDefault(x => x.Id == model.Option.Id);
            var type = Db.ExaminationForms.SingleOrDefault(x => x.Id == model.ExaminationTypeId);

            var fraction = new ExaminationFraction
            {
                ExaminationOption = exam,
                Form = type,
                Weight = model.Weight / (double)100,
                MinDuration = model.MinDuration,
                MaxDuration = model.MaxDuration
            };

            Db.ExaminationFractions.Add(fraction);
            Db.SaveChanges();

            return RedirectToAction("ExaminationForms", new { id = exam.Module.Id });
        }

        public ActionResult EditExaminationFraction(Guid id)
        {
            var fraction = Db.ExaminationFractions.SingleOrDefault(x => x.Id == id);

            var model = new ExaminationFractionViewModel
            {
                Option = fraction.ExaminationOption,
                FractionId = fraction.Id,
                ExaminationTypeId = fraction.Form.Id,
                Weight = (int)(fraction.Weight * 100 + 0.0001),
                MinDuration = (int)fraction.MinDuration,
                MaxDuration = (int)fraction.MaxDuration
            };

            var examinationForms =
                Db.ExaminationForms.Select(x => new SelectListItem
                {
                    Text = x.ShortName,
                    Value = x.Id.ToString()
                });

            ViewBag.ExamOptions = examinationForms;

            return View(model);
        }

        [HttpPost]
        public ActionResult EditExaminationFraction(ExaminationFractionViewModel model)
        {
            var fraction = Db.ExaminationFractions.SingleOrDefault(x => x.Id == model.FractionId);
            var type = Db.ExaminationForms.SingleOrDefault(x => x.Id == model.ExaminationTypeId);

            fraction.Form = type;
            fraction.Weight = model.Weight / (double)100;
            fraction.MinDuration = model.MinDuration;
            fraction.MaxDuration = model.MaxDuration;

            Db.SaveChanges();

            return RedirectToAction("ExaminationForms", new { id = fraction.ExaminationOption.Module.Id });
        }

        public ActionResult Subjects(Guid id)
        {
            var module = Db.CurriculumModules.SingleOrDefault(x => x.Id == id);

            var model = new ModuleDescriptionsViewModel
            {
                Module = module,
            };

            return View(model);
        }



        public ActionResult CreateSubject(Guid id)
        {
            var module = Db.CurriculumModules.SingleOrDefault(x => x.Id == id);

            var model = new SubjectViewModel
            {
                Module = module
            };

            var teachingForms =
                Db.TeachingFormats.Select(x => new SelectListItem
                {
                    Text = x.Tag,
                    Value = x.Id.ToString()
                });

            ViewBag.TeachingOptions = teachingForms;

            if (teachingForms.Any())
            {
                model.TeachingTypeId = Guid.Parse(teachingForms.First().Value);
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult CreateSubject(SubjectViewModel model)
        {
            var module = Db.CurriculumModules.SingleOrDefault(x => x.Id == model.Module.Id);
            var type = Db.TeachingFormats.SingleOrDefault(x => x.Id == model.TeachingTypeId);

            var subject = new ModuleSubject
            {
                Module = module,
                SWS = model.SWS,
                Tag = model.Tag,
                Name = model.Name,
                TeachingFormat = type
            };

            Db.ModuleCourses.Add(subject);
            Db.SaveChanges();

            return RedirectToAction("Subjects", new { id = module.Id });
        }

        public ActionResult EditSubject(Guid id)
        {
            var subject = Db.ModuleCourses.SingleOrDefault(x => x.Id == id);

            var model = new SubjectViewModel
            {
                Module = subject.Module,
                SubjectId = subject.Id,
                Tag = subject.Tag,
                Name = subject.Name,
                SWS = subject.SWS,
                TeachingTypeId = subject.TeachingFormat.Id,
            };

            var teachingForms =
                Db.TeachingFormats.Select(x => new SelectListItem
                {
                    Text = x.Tag,
                    Value = x.Id.ToString()
                });

            ViewBag.TeachingOptions = teachingForms;

            return View(model);
        }

        public ActionResult DeleteSubject(Guid id)
        {
            var subject = Db.ModuleCourses.SingleOrDefault(x => x.Id == id);
            var module = subject.Module;

            foreach (var opp in subject.Opportunities.ToList())
            {
                Db.SubjectOpportunities.Remove(opp);
            }

            foreach (var accr in subject.Module.Accreditations.ToList())
            {
                foreach (var teaching in accr.TeachingDescriptions.ToList())
                {
                    Db.TeachingDescriptions.Remove(teaching);
                }
            }

            Db.ModuleCourses.Remove(subject);

            Db.SaveChanges();

            return RedirectToAction("Subjects", new { id = module.Id });
        }

        [HttpPost]
        public ActionResult EditSubject(SubjectViewModel model)
        {
            var subject = Db.ModuleCourses.SingleOrDefault(x => x.Id == model.SubjectId);
            var type = Db.TeachingFormats.SingleOrDefault(x => x.Id == model.TeachingTypeId);

            subject.TeachingFormat = type;
            subject.Tag = model.Tag;
            subject.Name = model.Name;
            subject.SWS = model.SWS;

            Db.SaveChanges();

            return RedirectToAction("Subjects", new { id = subject.Module.Id });
        }

        public ActionResult Teachings(Guid moduleId, Guid semId)
        {
            var module = Db.CurriculumModules.SingleOrDefault(x => x.Id == moduleId);
            var semester = SemesterService.GetSemester(semId);

            var model = new ModuleDescriptionsViewModel
            {
                Module = module,
                Semester = semester
            };

            return View(model);
        }

        public ActionResult CreateTeaching(Guid moduleId, Guid semId)
        {
            var module = Db.CurriculumModules.SingleOrDefault(x => x.Id == moduleId);
            var semester = SemesterService.GetSemester(semId);

            var model = new ModuleDescriptionsViewModel
            {
                Module = module,
                Semester = semester,
                Organiser = module.Catalog.Organiser,
                Organisers = Db.Organisers.Where(x => x.ModuleCatalogs.Any()).ToList()
            };

            return View(model);
        }

        [HttpPost]
        public PartialViewResult LoadCourses(Guid orgId, Guid semId, string text)
        {
            var org = Db.Organisers.SingleOrDefault(x => x.Id == orgId);
            var semester = SemesterService.GetSemester(semId);

            var courses =
                Db.Activities.OfType<Course>().Where(c =>
                    c.SemesterGroups.Any(g =>
                        g.Semester.Id == semester.Id &&
                        g.CapacityGroup.CurriculumGroup.Curriculum.Organiser.Id == org.Id) &&
                    ((!string.IsNullOrEmpty(c.ShortName) && (c.ShortName.Contains(text)) ||
                      (!string.IsNullOrEmpty(c.Name) && c.Name.Contains(text))))
                ).OrderBy(c => c.ShortName).ToList();

            ViewBag.ListName = "sourceModuleList";
            return PartialView("_CourseListGroup", courses);
        }


        [HttpPost]
        public PartialViewResult LoadTeachings(Guid accrId, Guid subjectId, Guid semId)
        {
            var semester = SemesterService.GetSemester(semId);
            var accr = Db.Accreditations.SingleOrDefault(x => x.Id == accrId);
            var subject = Db.ModuleCourses.SingleOrDefault(x => x.Id == subjectId);

            var teachings = Db.TeachingDescriptions.Where(x =>
                x.Accreditation.Id == accr.Id &&
                x.Semester.Id == semester.Id &&
                x.Subject.Id == subject.Id).ToList();

            ViewBag.ListName = "targetModuleList";
            return PartialView("_TeachingListGroup", teachings);
        }

        [HttpPost]
        public PartialViewResult CreateTeachings(Guid accrId, Guid subjectId, Guid semId, Guid[] courseIds)
        {
            var semester = SemesterService.GetSemester(semId);
            var accr = Db.Accreditations.SingleOrDefault(x => x.Id == accrId);
            var subject = Db.ModuleCourses.SingleOrDefault(x => x.Id == subjectId);

            foreach (var courseId in courseIds)
            {
                var course = Db.Activities.OfType<Course>().SingleOrDefault(x => x.Id == courseId);

                if (course == null) continue;

                var teaching = Db.TeachingDescriptions.FirstOrDefault(x =>
                    x.Accreditation.Id == accr.Id &&
                    x.Semester.Id == semester.Id &&
                    x.Subject.Id == subject.Id &&
                    x.Course.Id == course.Id);

                if (teaching != null) continue;
                
                teaching = new TeachingDescription
                {
                    Accreditation = accr,
                    Semester = semester,
                    Subject = subject,
                    Course = course
                };

                Db.TeachingDescriptions.Add(teaching);
            }

            Db.SaveChanges();

            return null;
        }

        public ActionResult DeleteTeaching(Guid id)
        {
            var teaching = Db.TeachingDescriptions.SingleOrDefault(x => x.Id == id);

            var module = teaching.Accreditation.Module;
            var semester = teaching.Semester;

            Db.TeachingDescriptions.Remove((teaching));
            Db.SaveChanges();

            return RedirectToAction("Teachings", new { moduleId = module.Id, semId = semester.Id });
        }

        public ActionResult DeleteOpportunity(Guid id)
        {
            var teaching = Db.SubjectOpportunities.SingleOrDefault(x => x.Id == id);

            var module = teaching.Subject.Module;
            var semester = teaching.Semester;

            Db.SubjectOpportunities.Remove((teaching));
            Db.SaveChanges();

            return RedirectToAction("Teachings", new { moduleId = module.Id, semId = semester.Id });
        }

    }
}