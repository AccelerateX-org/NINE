using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using MyStik.TimeTable.Data;
using MyStik.TimeTable.Web.Models;

namespace MyStik.TimeTable.Web.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public class NewsletterController : BaseController
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Index(Guid? id)
        {
            ViewBag.FacultyList = Db.Organisers.Where(x => x.Activities.OfType<Newsletter>().Any()).OrderBy(s => s.Name).Select(f => new SelectListItem
            {
                Text = f.Name,
                Value = f.Id.ToString(),
            });

            if (id.HasValue)
                return View(GetOrganiser(id.Value));

            return View(GetMyOrganisation());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="facultyId">Kurzname der Fakultät</param>
        /// <returns></returns>
        [HttpPost]
        public PartialViewResult Faculty(Guid facultyId)
        {
            var newsletters = Db.Activities.OfType<Newsletter>().Where(x => x.Organiser.Id == facultyId).ToList();

            var model = new List<NewsletterViewModel>();

            var userRight = new UserRight();
            userRight.User = UserManager.FindByName(User.Identity.Name);
            userRight.Member = GetMyMembership();

            var semester = SemesterService.GetSemester(DateTime.Today);

            foreach (var newsletter in newsletters)
            {
                bool isMember = IsUserMemberOf(newsletter.Organiser.ShortName) || User.IsInRole("SysAdmin");
                bool isAdmin = IsUserAdminOf(newsletter.Organiser.ShortName) || User.IsInRole("SysAdmin");

                model.Add(new NewsletterViewModel
                {
                    Newsletter = newsletter,
                    State = ActivityService.GetActivityState(newsletter.Occurrence, AppUser),
                    IsMember = isMember,
                    IsAdmin = isAdmin,
                });

            }

            ViewBag.UserRight = userRight;
            ViewBag.Curricula = Db.Curricula.ToList();
            ViewBag.Semester = semester;

            return PartialView("_NewsletterList", model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
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

                ViewBag.UserRight = GetUserRight();

                return View(model);
            }


            return View("Missing", "Home");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Create()
        {
            var org = GetMyOrganisation();

            var model = new Newsletter {Organiser = org};

            return View(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Create(Newsletter model)
        {

            var org = GetMyOrganisation();

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


            return RedirectToAction("Newsletter", "Organiser", new {id=org.Id});
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Edit(Guid id)
        {
            var model = Db.Activities.OfType<Newsletter>().SingleOrDefault(a => a.Id == id);
            return View(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Edit(Newsletter model)
        {
            if (ModelState.IsValid)
            {
                Db.Entry(model).State = EntityState.Modified;
                Db.SaveChanges();
            }

            return RedirectToAction("Newsletter", "Organiser", new {id = model.Organiser.Id});
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Delete(Guid id)
        {
            var model = Db.Activities.OfType<Newsletter>().SingleOrDefault(a => a.Id == id);



            return View(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Delete(Newsletter model)
        {
            var newsletter = Db.Activities.OfType<Newsletter>().SingleOrDefault(x => x.Id == model.Id);

            var allSubscriptions = newsletter.Occurrence.Subscriptions.ToList();

            foreach (var subscription in allSubscriptions)
            {
                Db.Subscriptions.Remove(subscription);
            }
            Db.Activities.Remove(newsletter);
            Db.SaveChanges();

            return RedirectToAction("Newsletter", "Organiser", new { id = model.Organiser.Id });
        }

    }
}