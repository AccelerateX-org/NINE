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
            var member = GetMyMembership();

            var userService = new UserInfoService();

            var theses = Db.Theses.Where(x => x.Supervisors.Any(m => m.Member.Id == member.Id)).ToList();

            var model = new SupervisionOverviewModel();
            model.Organiser = org;
            model.Member = member;


            foreach (var thesis in theses)
            {
                var tm = new ThesisStateModel
                {
                    Thesis = thesis,
                    Student = thesis.Student,
                    User = userService.GetUser(thesis.Student.UserId)
                };

                model.Thesis.Add(tm);
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


        public ActionResult CreateRequest(Guid id)
        {
            var supervision = Db.Activities.OfType<Supervision>().SingleOrDefault(x => x.Id == id);

            var model = new SupervisionRequestModel();
            model.Supervision = supervision;

            return View(model);
        }


        [HttpPost]
        public ActionResult CreateRequest(SupervisionRequestModel model)
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
            var thesis = Db.Theses.SingleOrDefault(x => x.Id == id);
            var member = GetMyMembership();
            var user = GetCurrentUser();

            var supervision = thesis.Supervisors.FirstOrDefault(x => x.Member.Id == member.Id);
            supervision.AcceptanceDate = DateTime.Now;
            Db.SaveChanges();

            // TODO: E-Mail versenden
            var userService = new UserInfoService();

            var tm = new ThesisStateModel
            {
                Thesis = thesis,
                Student = thesis.Student,
                User = userService.GetUser(thesis.Student.UserId)
            };

            var mailService = new ThesisMailService();
            mailService.SendSupervisionRequestAccept(tm, member, user);

            return RedirectToAction("Index");
        }


        public ActionResult RejectRequest(Guid id)
        {
            var thesis = Db.Theses.SingleOrDefault(x => x.Id == id);
            var member = GetMyMembership();
            var user = GetCurrentUser();

            var supervision = thesis.Supervisors.FirstOrDefault(x => x.Member.Id == member.Id);

            if (supervision != null)
            {
                thesis.Supervisors.Remove(supervision);
                Db.Supervisors.Remove(supervision);

                Db.SaveChanges();
            }

            // TODO: E-Mail versenden
            var userService = new UserInfoService();

            var tm = new ThesisStateModel
            {
                Thesis = thesis,
                Student = thesis.Student,
                User = userService.GetUser(thesis.Student.UserId)
            };

            var mailService = new ThesisMailService();
            mailService.SendSupervisionRequestDeny(tm, member, user);

            return RedirectToAction("Index");
        }



        [HttpPost]
        public PartialViewResult SupervisionState(Guid id)
        {
            var userService = new UserInfoService();

            var thesis = Db.Theses.SingleOrDefault(x => x.Id == id);

            var tm = new ThesisStateModel
            {
                Thesis = thesis,
                Student = thesis.Student,
                User = userService.GetUser(thesis.Student.UserId)
            };

            var member = GetMyMembership();

            var model = new SupervisionDetailModel
            {
                Thesis = tm,
                Supervisor = member
            };

            var didIAccepted = thesis.Supervisors.Any(x => x.Member.Id == member.Id && x.AcceptanceDate.HasValue);

            // Habe noch nicht reagiert
            if (!didIAccepted)
                return PartialView("_StateRequest", model);

            // ich habe angenommen (sonst taucht es eh nicht auf)
            // aktuell noch unklar
            return PartialView("_StateUnknown", model);
        }
    }
}
