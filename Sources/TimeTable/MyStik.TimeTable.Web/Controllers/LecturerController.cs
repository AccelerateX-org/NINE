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
        // GET: Rooms
        public ActionResult Index()
        {
            var org = GetMyOrganisation();

            var model = Db.Organisers.Where(o => o.Members.Any()).OrderBy(s => s.Name).ToList();

            ViewBag.UserRights = GetUserRight(org);

            return View(model);
        }

        public ActionResult Organiser(Guid id)
        {
            var org = GetOrganiser(id);

            var model = new OrganiserViewModel
            {
                Organiser = org
            };

            ViewBag.UserRights = GetUserRight(org);

            return View(model);
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

            if (officeHour.ByAgreement)
            {
                return View("DateListAgreement", model);
            }


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