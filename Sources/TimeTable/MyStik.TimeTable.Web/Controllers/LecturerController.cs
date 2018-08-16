using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MyStik.TimeTable.Data;
using MyStik.TimeTable.DataServices;
using MyStik.TimeTable.Web.Models;
using MyStik.TimeTable.Web.Services;

namespace MyStik.TimeTable.Web.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public class LecturerController : BaseController
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            var model = new OrganiserViewModel();

            ViewBag.FacultyList = Db.Organisers.Where(o => !o.IsStudent && o.Members.Any())
                .OrderBy(s => s.Name).Select(f => new SelectListItem
                {
                    Text = f.Name,
                    Value = f.Id.ToString(),
                });

            ViewBag.UserRight = GetUserRight();

            model.Organiser = GetMyOrganisation();

            ViewBag.MenuId = "menu-lecturers";

            return View(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="facultyId"></param>
        /// <returns></returns>
        [HttpPost]
        public PartialViewResult Faculty(Guid facultyId)
        {
            var sem = SemesterService.GetSemester(DateTime.Today);

            var model = new List<LecturerViewModel>();

            var orgService = new OrganizerService(Db);
            var faculty = orgService.GetOrganiser(facultyId);

            // alle die einen termin haben, der zu einer aktuellen Semestergruppe gehört
            var activeLecturers = orgService.GetLecturers(faculty, sem);

            foreach (var lecturer in activeLecturers)
            {
                var viewModel = new LecturerViewModel
                {
                    Lecturer = lecturer,
                    OfficeHour = null, //myOfficeHour,
                    IsActive = true //myOfficeHour != null || hasDates
                };

                model.Add(viewModel);
            }

            ViewBag.UserRight = GetUserRight();
            ViewBag.Semester = sem;


            return PartialView("_ProfileList", model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Calendar(Guid id)
        {
            var model = Db.Members.SingleOrDefault(r => r.Id == id);

            return View(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult OfficeHour(Guid? id)
        {
            var semester = SemesterService.GetSemester(id);
            var member = GetMyMembership();

            var officeHour = Db.Activities.OfType<OfficeHour>().FirstOrDefault(x =>
                x.Semester.Id == semester.Id && x.Owners.Any(y => y.Member.Id == member.Id));
            var infoService = new OfficeHourInfoService(UserManager);

            if (officeHour == null)
                return RedirectToAction("Create", "OfficeHour", new { id = semester.Id });

            var model = new OfficeHourSubscriptionViewModel
            {
                OfficeHour = officeHour,
                Semester = semester,
                Host = infoService.GetHost(officeHour),
            };

            model.Dates.AddRange(infoService.GetDates(officeHour));


            return View("DateList", model);
        }

        public ActionResult Thesis(Guid? id)
        {
            var semester = SemesterService.GetSemester(DateTime.Today);
            var member = GetMyMembership();

            var thesisActivities = Db.Activities.OfType<Supervision>().Where(x =>
               x.Owners.Any(y => y.Member.Id == member.Id)).ToList();

            var theses = Db.Theses.Where(x => x.Supervision.Owners.Any(y => y.Member.Id == member.Id)).ToList();

            var model = new ThesisSemesterSummaryModel();
            model.Semester = semester;
            model.Supervisions = thesisActivities;
            model.Theses = theses;

            return View(model);
        }

        public ActionResult EditDate(Guid id)
        {
            var date = Db.ActivityDates.SingleOrDefault(x => x.Id == id);

            return View(date);
        }

        [HttpPost]
        public ActionResult EditDate(ActivityDate model)
        {
            var date = Db.ActivityDates.SingleOrDefault(x => x.Id == model.Id);

            date.Title = model.Title;
            Db.SaveChanges();

            var officeHour = date.Activity as OfficeHour;

            return RedirectToAction("DateDetails", new{id=date.Id});
        }

        public ActionResult DateDetails(Guid id)
        {
            var date = Db.ActivityDates.SingleOrDefault(x => x.Id == id);
            var officeHour = date.Activity as OfficeHour;
            var infoService = new OfficeHourInfoService(UserManager);

            var model = new OfficeHourDateViewModel();

            model.OfficeHour = officeHour;
            model.Date = date;
            model.Subscriptions.AddRange(infoService.GetSubscriptions(date));

            return View(model);
        }

        public ActionResult Responsibilities(Guid id)
        {
            var model = Db.Members.SingleOrDefault(x => x.Id == id);
            return View(model);
        }


    }
}