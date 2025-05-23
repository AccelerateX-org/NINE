﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using MyStik.TimeTable.Data;
using MyStik.TimeTable.Web.Helpers;
using MyStik.TimeTable.Web.Models;
using MyStik.TimeTable.Web.Services;
using MyStik.TimeTable.Web.Utils;

namespace MyStik.TimeTable.Web.Controllers
{
    public class ThesisSupervisionController : BaseController
    {
        // GET: Supervision
        public ActionResult Index()
        {
            var user = GetCurrentUser();
            var org = GetMyOrganisation();
            var member = GetMyMembership(org.Id);
            if (member == null)
                return View("_NoAccess");

            var userService = new UserInfoService();

            
            var theses = Db.Theses.Where(x => 
                x.Supervisors.Any(m => m.Member.Id == member.Id) && // Alle Abschlussarbeiten für den Betreuer
                x.IsCleared == null || x.IsCleared.Value == false // noch nicht abgerechnet
            ).ToList();


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

        /*
        public ActionResult Requests()
        {
            var user = GetCurrentUser();
            var org = GetMyOrganisation();
            var member = GetMyMembership();

            var userService = new UserInfoService();


            var theses = Db.Theses.Where(x =>
                    x.Supervisors.Any(m => m.Member.Id == member.Id) && // Alle Abschlussarbeiten für den Betreuer
                    x.DeliveryDate == null   // noch nich abgegeben
            ).ToList();


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
        */

        public ActionResult Cleared()
        {
            var user = GetCurrentUser();
            var org = GetMyOrganisation();
            var member = GetMyMembership(org.Id);
            if (member == null)
                return View("_NoAccess");

            var userService = new UserInfoService();


            var theses = Db.Theses.Where(x =>
                    x.Supervisors.Any(m => m.Member.Id == member.Id) && // Alle Abschlussarbeiten für den Betreuer
                    x.DeliveryDate != null &&  // abgegeben
                    x.IsCleared != null && x.IsCleared == true  // archiviert / abgerechnet
            ).ToList();


            var model = new SupervisionOverviewModel {Organiser = org, Member = member};


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




        public ActionResult Details(Guid id)
        {
            var itsMe = GetCurrentUser();

            var thesis = Db.Theses.SingleOrDefault(x => x.Id == id);

            var org = thesis.Student.Curriculum.Organiser;
            var userRight = GetUserRight(org);

            var user = GetUser(thesis.Student.UserId);
            var student = thesis.Student;
            var member = GetMyMembership(thesis.Student.Curriculum.Organiser.Id);

            if (member == null)
                return View("_NoAccess");

            var model = new ThesisStateModel
            {
                User = user,
                Student = student,
                Thesis = thesis,
                Supervisor = thesis.Supervisors.SingleOrDefault(x => x.Member.UserId.Equals(itsMe.Id))
            };

            ViewBag.UserRight = userRight;
            ViewBag.Organiser = org;


            return View(model);
        }

        public ActionResult ChangeTitle(Guid id)
        {
            var thesis = Db.Theses.SingleOrDefault(x => x.Id == id);

            return View(thesis);
        }


        [HttpPost]
        public ActionResult ChangeTitle(Thesis model)
        {
            var thesis = Db.Theses.SingleOrDefault(x => x.Id == model.Id);

            var userService = new UserInfoService();
            var member = GetMyMembership(thesis.Student.Curriculum.Organiser.Id);
            var user = GetCurrentUser();


            if (thesis != null)
            {
                if (!string.IsNullOrEmpty(model.TitleDe))
                    thesis.TitleDe = model.TitleDe;

                if (!string.IsNullOrEmpty(model.TitleEn))
                    thesis.TitleEn = model.TitleEn;

                Db.SaveChanges();
            }

            // Mailversand bei Änderung des Thema
            if (thesis.IssueDate != null)
            {
                // Mail an Studierenden, dass der Lehrende den Thema geändert hat
                var tm = InitMailModel(thesis, user);

                new MailController().ThesisSupervisorTitleChangedEMail(tm).Deliver();
            }


            return RedirectToAction("Details", new { id = thesis.Id });
        }

        public ActionResult Accept(Guid id)
        {
            var userService = new UserInfoService();

            var supervisor = Db.Supervisors.SingleOrDefault(x => x.Id == id);
            var thesis = supervisor.Thesis;

            var user = GetCurrentUser();
            var member = GetMyMembership(thesis.Student.Curriculum.Organiser.Id);
            if (member == null)
                return View("_NoAccess");



            supervisor.AcceptanceDate = DateTime.Now;
            Db.SaveChanges();

            // Mail an Studierenden
            var model = InitMailModel(thesis, user);
            model.IsAccepted = true;

            new MailController().ThesisSupervisionResponseEMail(model).Deliver();



            return RedirectToAction("Details", new { id = thesis.Id });
        }

        public ActionResult Reject(Guid id)
        {
            var userService = new UserInfoService();

            var supervisor = Db.Supervisors.SingleOrDefault(x => x.Id == id);
            var thesis = supervisor.Thesis;

            var user = GetCurrentUser();
            var member = GetMyMembership(thesis.Student.Curriculum.Organiser.Id);
            if (member == null)
                return View("_NoAccess");


            thesis.Supervisors.Remove(supervisor);
            Db.Supervisors.Remove(supervisor);
            Db.SaveChanges();

            // Mail an Studierenden
            var model = InitMailModel(thesis, user);
            model.IsAccepted = false;

            new MailController().ThesisSupervisionResponseEMail(model).Deliver();



            return RedirectToAction("Details", new { id = thesis.Id });
        }


        public ActionResult Remove(Guid id)
        {
            var userService = new UserInfoService();

            var supervisor = Db.Supervisors.SingleOrDefault(x => x.Id == id);
            var thesis = supervisor.Thesis;

            var user = GetCurrentUser();
            var member = GetMyMembership(thesis.Student.Curriculum.Organiser.Id);
            if (member == null)
                return View("_NoAccess");


            var supervisorUser = GetUser(supervisor.Member.UserId);

            thesis.Supervisors.Remove(supervisor);
            Db.Supervisors.Remove(supervisor);
            Db.SaveChanges();



            // Mail an Betreuer mit Kopie an Studierenden
            var model = InitMailModel(thesis, user);

            new MailController().ThesisSupervisorRemoveEMail(model, supervisorUser).Deliver();



            return RedirectToAction("Details", new { id = thesis.Id });
        }


        /// <summary>
        /// Anmeldung durch den Betreuer
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Issue(Guid id)
        {
            var userService = new UserInfoService();

            var user = GetCurrentUser();

            var thesis = Db.Theses.SingleOrDefault(x => x.Id == id);

            var period = thesis.Student.Curriculum.ThesisDuration;
            if (period == 0)
            {
                period = 3;
            }

            thesis.IssueDate = DateTime.Now;
            thesis.ExpirationDate = thesis.IssueDate.Value.AddMonths(period);

            Db.SaveChanges();

            // Mail an Studierenden
            var model = InitMailModel(thesis, user);

            new MailController().ThesisSupervisorIssuedEMail(model).Deliver();



            return RedirectToAction("Details", new { id = thesis.Id });
        }


        /// <summary>
        /// Anmeldung durch den Betreuer
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Delivered(Guid id)
        {
            var userService = new UserInfoService();

            var user = GetCurrentUser();
            var thesis = Db.Theses.SingleOrDefault(x => x.Id == id);

            thesis.DeliveryDate = DateTime.Now;
            Db.SaveChanges();

            // Mail an Studierenden
            var model = InitMailModel(thesis, user);

            new MailController().ThesisSupervisorDeliveredEMail(model).Deliver();



            return RedirectToAction("Details", new { id = thesis.Id });
        }


        /// <summary>
        /// Abgabe storniertz
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Storno(Guid id)
        {
            var userService = new UserInfoService();

            var user = GetCurrentUser();
            var thesis = Db.Theses.SingleOrDefault(x => x.Id == id);

            thesis.DeliveryDate = null;
            Db.SaveChanges();

            // Mail an Studierenden
            var model = InitMailModel(thesis, user);

            new MailController().ThesisSupervisorDeliveryStornoEMail(model).Deliver();



            return RedirectToAction("Details", new { id = thesis.Id });
        }



        private ThesisMailModel InitMailModel(Thesis t, ApplicationUser senderUser)
        {
            var model = new ThesisMailModel();

            var userService = new UserInfoService();

            model.Thesis = t;
            model.StudentUser = userService.GetUser(t.Student.UserId);

            foreach (var supervisor in t.Supervisors)
            {
                var user = userService.GetUser(supervisor.Member.UserId);

                if (user != null)
                {
                    model.SupervisorUsers.Add(user);
                }
            }

            model.ActionUser = senderUser;

            var pk = t.Student.Curriculum.Organiser.Autonomy?.Committees.FirstOrDefault(x =>
                    x.Name.Equals("PK") &&
                    x.Curriculum != null &&
                    x.Curriculum.Id == t.Student.Curriculum.Id);


            var pkv = pk?.Members.FirstOrDefault(x => x.HasChair);

            if (pkv?.Member != null)
            {
                model.BoardUser = userService.GetUser(pkv.Member.UserId);
            }


            return model;
        }

        public ActionResult AddSupervisors(Guid id)
        {
            var thesis = Db.Theses.SingleOrDefault(x => x.Id == id);


            var model = new ThesisSupervisionModel
            {
                Thesis = thesis,
                OrganiserId = thesis.Student.Curriculum.Organiser.Id
            };


            // Liste aller Fakultäten
            ViewBag.Organiser = Db.Organisers.Where(x => x.Id == thesis.Student.Curriculum.Organiser.Id).OrderBy(x => x.ShortName).Select(c => new SelectListItem
            {
                Text = c.ShortName,
                Value = c.Id.ToString(),
            });



            return View(model);
        }

        [HttpPost]
        public ActionResult AddSupervisors(Guid id, Guid[] DozIds)
        {
            var thesis = Db.Theses.SingleOrDefault(x => x.Id == id);

            if (thesis == null)
            {
                return Json(new { result = "Redirect", url = Url.Action("RequestIncomplete") });
            }


            var supervisors = new List<Supervisor>();

            foreach (var dozId in DozIds)
            {
                var member = Db.Members.SingleOrDefault(x => x.Id == dozId);

                if (member != null && thesis.Supervisors.All(x => x.Member.Id != member.Id))
                {
                    var supervisor = new Supervisor
                    {
                        Thesis = thesis,
                        Member = member
                    };
                    thesis.Supervisors.Add(supervisor);
                    Db.Supervisors.Add(supervisor);

                    supervisors.Add(supervisor);
                }
            }

            Db.SaveChanges();

            var userService = new UserInfoService();
            var user = GetCurrentUser();

            foreach (var supervisor in supervisors)
            {
                // der user des angefragten Lehrenden
                var supervisorUser = userService.GetUser(supervisor.Member.UserId);

                if (supervisorUser != null)
                {
                    var tm = InitMailModel(thesis, user);
                    tm.AsSubstitute = true;

                    new MailController().ThesisSupervisionRequestEMail(tm, supervisorUser).Deliver();
                }

            }


            return Json(new { result = "Redirect", url = Url.Action("Details", new { id = thesis.Id }) });
        }


        public ActionResult AcceptProlongRequest(Guid id)
        {
            var userService = new UserInfoService();
            var user = GetCurrentUser();

            var supervisor = Db.Supervisors.SingleOrDefault(x => x.Id == id);
            var thesis = supervisor.Thesis;

            var member = GetMyMembership(thesis.Student.Curriculum.Organiser.Id);
            if (member == null)
                return View("_NoAccess");



            thesis.ProlongSupervisorAccepted = true;
            Db.SaveChanges();

            // Mail an PK-Vorsitzenden
            var model = InitMailModel(thesis, user);

            if (model.BoardUser != null)
            {
                new MailController().ThesisProlongRequestBoardEMail(model).Deliver();
            }

            return RedirectToAction("Details", new { id = thesis.Id });
        }

        public ActionResult RejectProlongRequest(Guid id)
        {
            var userService = new UserInfoService();

            var supervisor = Db.Supervisors.SingleOrDefault(x => x.Id == id);
            var thesis = supervisor.Thesis;

            var user = GetCurrentUser();
            var member = GetMyMembership(thesis.Student.Curriculum.Organiser.Id);
            if (member == null)
                return View("_NoAccess");


            thesis.ProlongSupervisorAccepted = false;
            Db.SaveChanges();

            // Mail an Studierenden
            /*
            var model = InitMailModel(thesis, user);
            model.IsAccepted = false;

            new MailController().ThesisSupervisionResponseEMail(model).Deliver();
            */


            return RedirectToAction("Details", new { id = thesis.Id });
        }


    }
}
