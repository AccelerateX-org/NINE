using System;
using System.Linq;
using System.Web.Mvc;
using MyStik.TimeTable.Data;
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
            var user = GetCurrentUser();
            var infoService = new OfficeHourInfoService(UserManager);

            if (id == null)
            {
                var summaryModel = new LecturerSummaryModel()
                {
                    Memberships = MemberService.GetFacultyMemberships(user.Id)
                };

                var officeHours = 
                    Db.Activities.OfType<OfficeHour>().Where(x =>
                        x.Owners.Any(o => !string.IsNullOrEmpty(o.Member.UserId) && o.Member.UserId.Equals(user.Id)))
                    .ToList();

                foreach (var oh in officeHours)
                {
                    var ohModel = new LecturerOfficehourSummaryModel()
                    {
                        OfficeHour = oh
                    };


                    summaryModel.OfficeHours.Add(ohModel);
                }

                var semester = SemesterService.GetSemester(DateTime.Today);

                ViewBag.ThisSemester = semester;
                ViewBag.NextSemester = SemesterService.GetNextSemester(semester);


                return View(summaryModel);
            }

            var officeHour = Db.Activities.OfType<OfficeHour>().SingleOrDefault(x => x.Id == id.Value);

            var model = new OfficeHourSubscriptionViewModel
            {
                OfficeHour = officeHour,
                Semester = officeHour.Semester,
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


        public ActionResult Courses()
        {
            var user = GetCurrentUser();

            var courses = Db.Activities.OfType<Course>().Where(x => 
                x.Owners.Any(o => o.Member.UserId.Equals(user.Id)) ||
                x.Dates.Any(d => d.Hosts.Any(h => !string.IsNullOrEmpty(h.UserId) && h.UserId.Equals(user.Id)))).ToList();

            var model = new LecturerSummaryModel();

            foreach (var c in courses)
            {
                var orderedDates = c.Dates.OrderBy(x => x.Begin).ToList();
                var firstDate = orderedDates.FirstOrDefault();
                var lastDate = orderedDates.LastOrDefault();
                var owner = c.Owners.FirstOrDefault(o => !string.IsNullOrEmpty(o.Member.UserId) && o.Member.UserId.Equals(user.Id));
                var dates = c.Dates.Where(x => x.Hosts.Any(h => !string.IsNullOrEmpty(h.UserId) && h.UserId.Equals(user.Id))).ToList();

                var courseModel = new LecturerCourseSummaryModel
                {
                    Course = c,
                    FirstDate = firstDate,
                    LastDate = lastDate,
                    Owner = owner,
                    HostingDates = dates
                };


                model.Courses.Add(courseModel);
            }


            return View(model);
        }

    }
}