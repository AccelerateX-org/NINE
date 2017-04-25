using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using MyStik.TimeTable.Data;
using MyStik.TimeTable.Web.Models;
using MyStik.TimeTable.Web.Services;

namespace MyStik.TimeTable.Web.Controllers
{
    public class StudyBookController : BaseController
    {
        // GET: StudyBook
        public ActionResult Index()
        {
            ViewBag.SemesterList = Db.Semesters.OrderByDescending(s => s.StartCourses).Select(f => new SelectListItem
            {
                Text = f.Name,
                Value = f.Name,
            });


            var user = AppUser;
            var userRight = GetUserRight();

            userRight.IsHost = Db.ActivityDates.Any(d => d.Hosts.Any(h => h.UserId.Equals(user.Id)));

            
            ViewBag.UserRight = userRight;

            var model = new SemesterViewModel
            {
                Semester = GetSemester()
            };

            return View(model);
        }

        [HttpPost]
        public PartialViewResult SemesterProfile(string semGroupId)
        {
            var user = AppUser;
            var semester = Db.Semesters.SingleOrDefault(s => s.Name.Equals(semGroupId));

            if (user == null || semester == null)
                return null;

            var model = new StudyBookViewModel();

            model.Courses =
                Db.Activities.OfType<Course>().Where(a =>
                    a.SemesterGroups.Any(s => s.Semester.Id == semester.Id) &&
                    a.Dates.Any(d => d.Hosts.Any(l => !string.IsNullOrEmpty(l.UserId) && l.UserId.Equals(user.Id)))).ToList();


            model.Events =
                Db.Activities.OfType<Event>().Where(a =>
                    a.SemesterGroups.Any(s => s.Semester.Id == semester.Id) &&
                    a.Dates.Any(d => d.Hosts.Any(l => !string.IsNullOrEmpty(l.UserId) && l.UserId.Equals(user.Id)))).ToList();


            model.OfficeHour =
                Db.Activities.OfType<OfficeHour>().SingleOrDefault(a =>
                    a.Semester.Id == semester.Id &&
                    a.Dates.Any(oc => oc.Hosts.Any(l => l.UserId.Equals(user.Id))));

            model.Semester = semester;

            // wo war ich eingetragen
            // Alle Termine als Teilnehmer

            var activities = Db.Activities.Where(a => 
                a.SemesterGroups.Any(s => s.Semester.Id == semester.Id) &&
                a.Occurrence.Subscriptions.Any(u => u.UserId.Equals(user.Id))).ToList();
            foreach (var activity in activities)
            {
                    model.MySubscriptions.Add(new ActivitySubscriptionModel
                    {
                        Activity = new ActivitySummary { Activity = activity },
                        State = ActivityService.GetActivityState(activity.Occurrence, user, semester)
                    });
            }

            var dates = Db.ActivityDates.Where(d =>
                d.Activity.SemesterGroups.Any(s => s.Semester.Id == semester.Id) &&
                d.Occurrence.Subscriptions.Any(u => u.UserId.Equals(user.Id))).ToList();
            foreach (var activityDate in dates)
            {
                model.MySubscriptions.Add(new ActivitySubscriptionModel
                {
                    Activity = new ActivityDateSummary { Date = activityDate },
                    State = ActivityService.GetActivityState(activityDate.Occurrence, user, semester)
                });
            }

            var slots = Db.ActivitySlots.Where(s =>
                s.ActivityDate.Activity.SemesterGroups.Any(g => g.Semester.Id == semester.Id) &&
                s.Occurrence.Subscriptions.Any(u => u.UserId.Equals(user.Id))).ToList();
            foreach (var activitySlot in slots)
            {
                model.MySubscriptions.Add(new ActivitySubscriptionModel
                {
                    Activity = new ActivitySlotSummary { Slot = activitySlot },
                    State = ActivityService.GetActivityState(activitySlot.Occurrence, user, semester)
                });
            }


            return PartialView("_SemesterProfile", model);
        }
    }
}