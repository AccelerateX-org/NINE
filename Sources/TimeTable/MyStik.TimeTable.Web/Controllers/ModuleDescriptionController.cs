using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using MyStik.TimeTable.Data;
using MyStik.TimeTable.Web.Models;

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
                    Text = $"Nächstes: {nextSemester.Name}",
                    Value = nextSemester.Id.ToString(),
                    Selected = false
                };
                semesterList.Add(semItem);
            }

            semItem = new SelectListItem
            {
                Text = $"Aktuell: {semester.Name}",
                Value = semester.Id.ToString(),
                Selected = true
            };
            semesterList.Add(semItem);

            semItem = new SelectListItem
            {
                Text = $"Letztes: {prevSemester.Name}",
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

            var desc = module.Descriptions.FirstOrDefault(x => x.Semester.Id == semester.Id);

            // Default => lege eine Beschreibung an
            // TODO: automatisch auf dem Vorsemester, falls vorhanden

            if (desc == null)
            {
                desc = new ModuleDescription
                {
                    Description = "",
                    Module = module,
                    Semester = semester
                };

                Db.ModuleDescriptions.Add(desc);
                Db.SaveChanges();
            }

            ViewBag.UserRight = GetUserRight(module.Catalog.Organiser);
            ViewBag.CurrentSemester = SemesterService.GetSemester(DateTime.Today);

            return PartialView("_Semester", desc);
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
                DescriptionText = desc.Description
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(ModuleDescriptionEditModel model)
        {
            var user = GetCurrentUser();
            var desc = Db.ModuleDescriptions.SingleOrDefault(x => x.Id == model.ModuleDescription.Id);

            desc.Description = model.DescriptionText;

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

            return RedirectToAction("Details", new { id = desc.Module.Id });
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
            model.semesteId = semester.Id;

            model.Module = module;
            model.Semester = semester;

            return View(model);
        }

        [HttpPost]
        public ActionResult CreateExamination(ExaminationEditModel model)
        {
            return RedirectToAction("Details", new { id = model.moduleId, });
        }
    }

    public class ModuleDescriptionEditModel
    {
        public ModuleDescription ModuleDescription { get; set; }

        [AllowHtml]
        public string DescriptionText { get; set; }
    }


}
