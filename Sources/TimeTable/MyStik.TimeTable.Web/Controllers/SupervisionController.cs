using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MyStik.TimeTable.Data;
using MyStik.TimeTable.Web.Models;
using MyStik.TimeTable.Web.Services;

namespace MyStik.TimeTable.Web.Controllers
{
    public class SupervisionController : BaseController
    {
        // GET: Supervision
        public ActionResult Index()
        {
            var user = GetCurrentUser();
            var org = GetMyOrganisation();

            var supervisions = Db.Activities.OfType<Supervision>().Where(x =>
                x.Owners.Any(m => m.Member.Organiser.Id == org.Id)).ToList();


            var model = new SupervisionOverviewModel();
            model.Organiser = org;

            foreach (var supervision in supervisions)
            {
                foreach (var owner in supervision.Owners)
                {
                    if (!model.Supervisions.ContainsKey(owner.Member))
                    {
                        model.Supervisions[owner.Member] = new List<Supervision>();
                    }

                    model.Supervisions[owner.Member].Add(supervision);
                }
            }

            return View(model);
        }

        public ActionResult Create()
        {
            var member = GetMyMembership();
            if (member == null)
                return RedirectToAction("Index", "Dashboard");

            return View();
        }

        [HttpPost]
        public ActionResult Create(SupervisionCreateModel model)
        {
            var member = GetMyMembership();

            if (member != null)
            {
                var supervision = new Supervision();

                supervision.Name = model.Title;
                supervision.Description = model.Description;
                supervision.Owners.Add(new ActivityOwner {Member = member});
                supervision.Occurrence = new Occurrence
                {
                    Capacity = -1,
                };

                Db.Activities.Add(supervision);
                Db.SaveChanges();
            }

            return RedirectToAction("Thesis", "Lecturer");
        }


        public ActionResult Details(Guid id)
        {
            var supervision = Db.Activities.OfType<Supervision>().SingleOrDefault(x => x.Id == id);

            var model = new SupervisionCreateModel();
            model.Supervision = supervision;

            return View(model);
        }

        public ActionResult Edit(Guid id)
        {
            var supervision = Db.Activities.OfType<Supervision>().SingleOrDefault(x => x.Id == id);

            var model = new SupervisionCreateModel();
            model.Supervision = supervision;
            model.Title = supervision.Name;
            model.Description = supervision.Description;

            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(SupervisionCreateModel model)
        {
            var member = GetMyMembership();

            if (member != null)
            {
                var supervision = Db.Activities.OfType<Supervision>().SingleOrDefault(x => x.Id == model.Supervision.Id);

                supervision.Name = model.Title;
                supervision.Description = model.Description;

                Db.SaveChanges();
            }

            return RedirectToAction("Details", new{id = model.Supervision.Id});
        }


        public ActionResult Request(Guid id)
        {
            var supervision = Db.Activities.OfType<Supervision>().SingleOrDefault(x => x.Id == id);

            var model = new SupervisionRequestModel();
            model.Supervision = supervision;

            return View(model);
        }


        [HttpPost]
        public ActionResult Request(SupervisionRequestModel model)
        {
            var user = GetCurrentUser();
            var supervision = Db.Activities.OfType<Supervision>().SingleOrDefault(x => x.Id == model.Supervision.Id);

            // hat schon eine Anfrage bei diesem Angebot
            if (supervision.Occurrence.Subscriptions.Any(x => x.UserId.Equals(user.Id)))
            {
                // Schon eingetragen
                // Fehlermeldung: da sind sie schon drin
                return RedirectToAction("Index", "Dashboard");
            }

            // Lehrende sollten sehen können, wo jemand eingetragen ist
            // Die Ablehnungs Mails sind dann Nachweis für ein "Nichtfinden"

            var subscription = new OccurrenceSubscription
            {
                UserId = user.Id,
                SubscriberRemark = model.Description,
                TimeStamp = DateTime.Now,
                OnWaitingList = true,
                IsConfirmed = false,
                Occurrence = supervision.Occurrence
            };

            Db.Subscriptions.Add(subscription);
            Db.SaveChanges();

            // EMail versenden
            // Mail an Betreuer
            if (supervision.Owners.Any())
            {
                var member = supervision.Owners.First().Member;
                var hostUser = GetUser(member.UserId);

                var mailModel = new ThesisRequestMailModel
                {
                    Supervision = supervision,
                    Request = subscription,
                    Requester = user, // der anfragende Student
                    User = hostUser, // der Betreuuer
                };

                var mail = new MailController();
                mail.ThesisRequestEMail(mailModel).Deliver();
            }



            return RedirectToAction("Index", "Dashboard");
        }

        public ActionResult RequestDetails(Guid id)
        {
            var subscription = Db.Subscriptions.OfType<OccurrenceSubscription>().SingleOrDefault(x => x.Id == id);

            var supervision = Db.Activities.OfType<Supervision>()
                .SingleOrDefault(x => x.Occurrence.Id == subscription.Occurrence.Id);


            var model = new SupervisionRequestModel();

            model.Subscription = subscription;
            model.Supervision = supervision;

            return View(model);
        }


        public ActionResult EditRequest(Guid id)
        {
            var subscription = Db.Subscriptions.OfType<OccurrenceSubscription>().SingleOrDefault(x => x.Id == id);

            var supervision = Db.Activities.OfType<Supervision>()
                .SingleOrDefault(x => x.Occurrence.Id == subscription.Occurrence.Id);


            var model = new SupervisionRequestModel();

            model.Subscription = subscription;
            model.Supervision = supervision;
            model.Description = subscription.SubscriberRemark;

            return View(model);
        }

        [HttpPost]
        public ActionResult EditRequest(SupervisionRequestModel model)
        {
            var subscription = Db.Subscriptions.OfType<OccurrenceSubscription>()
                .SingleOrDefault(x => x.Id == model.Subscription.Id);

            subscription.SubscriberRemark = model.Description;
            Db.SaveChanges();

            return RedirectToAction("RequestDetails", new {id = model.Subscription.Id});
        }

        public ActionResult WithdrawRequest(Guid id)
        {
            var subscription = Db.Subscriptions.OfType<OccurrenceSubscription>()
                .SingleOrDefault(x => x.Id == id);

            Db.Subscriptions.Remove(subscription);
            Db.SaveChanges();

            return RedirectToAction("Index", "Dashboard");
        }


        public ActionResult AcceptRequest(Guid id)
        {
            var subscription = Db.Subscriptions.OfType<OccurrenceSubscription>()
                .SingleOrDefault(x => x.Id == id);

            var supervision = Db.Activities.OfType<Supervision>()
                .SingleOrDefault(x => x.Occurrence.Id == subscription.Occurrence.Id);

            var subInfoService = new SemesterSubscriptionService();
            var semester = SemesterService.GetSemester(DateTime.Today);

            var model = new SupervisionAcceptModel();
            model.User = GetUser(subscription.UserId);
            model.Curriculum = subInfoService.GetBestCurriculum(subscription.UserId, semester);
            model.Request.Subscription = subscription;
            model.Request.Supervision = supervision;
            model.Request.Lecturer = supervision.Owners.First().Member;

            // alle anderen
            var list = Db.Activities.OfType<Supervision>().Where(x =>
                x.Occurrence.Id != subscription.Occurrence.Id &&
                x.Occurrence.Subscriptions.Any(u => u.UserId.Equals(subscription.UserId))).ToList();

            foreach (var item in list)
            {
                var request = new SupervisionRequestModel();

                request.Lecturer = item.Owners.First().Member;
                request.Supervision = item;
                request.Subscription = item.Occurrence.Subscriptions.FirstOrDefault(x => x.UserId.Equals(subscription.UserId));

                model.AlternativeRequests.Add(request);
            }

            return View(model);
        }
    }
}
