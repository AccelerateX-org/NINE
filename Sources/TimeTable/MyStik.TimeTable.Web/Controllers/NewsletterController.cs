using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using MyStik.TimeTable.Data;
using MyStik.TimeTable.Web.Models;
using MyStik.TimeTable.Web.Services;

namespace MyStik.TimeTable.Web.Controllers
{
    public class NewsletterController : BaseController
    {
        //
        // GET: /Activity/
        public ActionResult Index()
        {


            var newsletters = Db.Activities.OfType<Newsletter>().ToList();

            var model = new List<NewsletterViewModel>();

            var userRight = new UserRight(User.IsInRole("SysAdmin"));
            userRight.User = UserManager.FindByName(User.Identity.Name);

            var semester = GetSemester();

            foreach (var newsletter in newsletters)
            {
                bool isMember = IsUserMemberOf(newsletter.Organiser.ShortName) || User.IsInRole("SysAdmin");
                bool isAdmin = IsUserAdminOf(newsletter.Organiser.ShortName) || User.IsInRole("SysAdmin");

                model.Add(new NewsletterViewModel
                {
                    Newsletter = newsletter,
                    State = ActivityService.GetActivityState(newsletter.Occurrence, AppUser, semester),
                    IsMember = isMember,
                    IsAdmin = isAdmin,
                });

                if (!userRight.IsOrgMember)
                    userRight.IsOrgMember = isMember;

                if (!userRight.IsOrgAdmin)
                    userRight.IsOrgAdmin = isAdmin;
            }

            ViewBag.UserRight = userRight;
            ViewBag.Curricula = Db.Curricula.ToList();
            ViewBag.Semester = semester;

            return View(model);
        }

        public ActionResult SendNews(Guid id)
        {
            var newsletter = Db.Activities.OfType<Newsletter>().SingleOrDefault(n => n.Id == id);

            if (newsletter != null)
            {
                var summary = new ActivitySummary {Activity = newsletter};

                var model = new OccurrenceMailingModel
                {
                    OccurrenceId = newsletter.Occurrence.Id,
                    Summary = summary,
                    Body = "",
                    Subject = "",
                };

                return View(model);
            }


            return View("Missing", "Home");
        }

        [HttpPost]
        public ActionResult SendNews(OccurrenceMailingModel model)
        {
            var occ = Db.Occurrences.SingleOrDefault(o => o.Id == model.OccurrenceId);

            if (occ != null)
            {
                // Die Subscription stecken in der Activity
                var summary = ActivityService.GetSummary(model.OccurrenceId);

                if (!summary.Subscriptions.Any())
                {
                    return RedirectToAction(summary.Action, summary.Controller, new { id = summary.Id });
                }
            }

            var m = new MailingController();
            m.ControllerContext = ControllerContext;
            return m.CustomOccurrenceMail(model);
        }


        public ActionResult Create(string id)
        {
            var org = Db.Organisers.SingleOrDefault(o => o.ShortName.ToUpper().Equals(id.ToUpper()));

            var model = new Newsletter {Organiser = org};

            return View(model);
        }

        [HttpPost]
        public ActionResult Create(Newsletter model)
        {

            var org = Db.Organisers.SingleOrDefault(o => o.ShortName.ToUpper().Equals(model.Organiser.ShortName.ToUpper()));

            var newsletter = new Newsletter
            {
                Name = model.Name,
                Description = model.Description,
                Organiser = org,
                Occurrence = new Occurrence
                {
                    IsAvailable = true,
                    Capacity = -1,
                },
            };

            Db.Activities.Add(newsletter);
            Db.SaveChanges();


            return RedirectToAction("Newsletter", "Organiser", new {id = model.Organiser.ShortName});
        }

        public ActionResult Edit(Guid id)
        {
            var model = Db.Activities.OfType<Newsletter>().SingleOrDefault(a => a.Id == id);
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(Newsletter model)
        {
            if (ModelState.IsValid)
            {
                Db.Entry(model).State = EntityState.Modified;
                Db.SaveChanges();
            }

            return RedirectToAction("Newsletter", "Organiser", new { id = model.Organiser.ShortName });
        }

        public ActionResult Members(Guid id)
        {
            var letter = Db.Activities.OfType<Newsletter>().SingleOrDefault(a => a.Id == id);

            var model = new NewsletterCharacteristicModel();

            model.Newsletter = letter;

            var iNumber = 0;
            foreach (var subscription in letter.Occurrence.Subscriptions.OrderBy(s => s.TimeStamp))
            {
                var subscriber = UserManager.FindById(subscription.UserId);
                if (subscriber != null)
                {
                    iNumber++;
                    model.Member.Add(
                        new CourseMemberModel()
                        {
                            Number = iNumber,
                            Subscription = subscription,
                            User = subscriber,
                        }
                        );
                }
            }

            return View(model);
        }
    }
}