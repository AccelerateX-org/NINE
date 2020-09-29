using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using MyStik.TimeTable.Data;
using MyStik.TimeTable.Web.Models;
using MyStik.TimeTable.Web.Services;
using MyStik.TimeTable.Web.Utils;
using PdfSharp;
using PdfSharp.Pdf;
using TheArtOfDev.HtmlRenderer.PdfSharp;

namespace MyStik.TimeTable.Web.Controllers
{
    public class ThesisSupervisionController : BaseController
    {
        // GET: Supervision
        public ActionResult Index()
        {
            var user = GetCurrentUser();
            var org = GetMyOrganisation();
            var member = GetMyMembership();
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
            var member = GetMyMembership();
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
            var org = GetMyOrganisation();
            var userRight = GetUserRight(org);

            var thesis = Db.Theses.SingleOrDefault(x => x.Id == id);

            var user = GetUser(thesis.Student.UserId);
            var student = thesis.Student;
            var member = GetMyMembership();

            if (member == null)
                return View("_NoAccess");

            var model = new ThesisStateModel
            {
                User = user,
                Student = student,
                Thesis = thesis,
                Supervisor = thesis.Supervisors.SingleOrDefault(x => x.Member.Id == member.Id)
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
            var userService = new UserInfoService();
            var member = GetMyMembership();
            var user = GetCurrentUser();

            var thesis = Db.Theses.SingleOrDefault(x => x.Id == model.Id);

            if (thesis != null)
            {
                if (!string.IsNullOrEmpty(model.TitleDe))
                    thesis.TitleDe = model.TitleDe;

                if (!string.IsNullOrEmpty(model.TitleEn))
                    thesis.TitleEn = model.TitleEn;

                Db.SaveChanges();
            }

            // Mailversand bei Änderung des Titels
            if (thesis.IssueDate != null)
            {
                // Mail an Studierenden, dass der Lehrende den Titel geändert hat
                var tm = InitMailModel(thesis, user);

                new MailController().ThesisSupervisorTitleChangedEMail(tm).Deliver();

            }


            return RedirectToAction("Details", new { id = thesis.Id });
        }

        public ActionResult Accept(Guid id)
        {
            var userService = new UserInfoService();

            var user = GetCurrentUser();
            var member = GetMyMembership();
            if (member == null)
                return View("_NoAccess");

            var supervisor = Db.Supervisors.SingleOrDefault(x => x.Id == id);
            var thesis = supervisor.Thesis;


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

            var user = GetCurrentUser();
            var member = GetMyMembership();
            if (member == null)
                return View("_NoAccess");

            var supervisor = Db.Supervisors.SingleOrDefault(x => x.Id == id);
            var thesis = supervisor.Thesis;

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

            var user = GetCurrentUser();
            var member = GetMyMembership();
            if (member == null)
                return View("_NoAccess");

            var supervisor = Db.Supervisors.SingleOrDefault(x => x.Id == id);
            var thesis = supervisor.Thesis;

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

            int period = 0;
            bool success = int.TryParse(thesis.Student.Curriculum.Version, out period);
            if (!success || period == 0)
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




        public ActionResult MarkingEmpty(Guid id)
        {
            var userService = new UserInfoService();
            var user = GetCurrentUser();

            var thesis = Db.Theses.SingleOrDefault(x => x.Id == id);

            // Mail mit Notenbeleg zum Ausdrucken an sich selbst senden
            var tm = new ThesisStateModel()
            {
                Thesis = thesis,
                Student = thesis.Student,
                User = userService.GetUser(thesis.Student.UserId),
                Mark = ""
            };


            // hier zunächst mit Postal - weil es so geht
            var stream = new MemoryStream();

            var email = new ThesisEmail("ThesisMarked");
            email.To = user.Email;
            email.From = MailController.InitSystemFrom();
            email.Subject = "Notenmeldung Abschlussarbeit";
            email.Thesis = tm;
            email.Receiver = user;

            var html = this.RenderViewToString("_ThesisPrintOut", email);
            PdfDocument pdf = PdfGenerator.GeneratePdf(html, PageSize.A4);
            //pdf.Save("document.pdf");
            pdf.Save(stream, false);

            // Stream zurücksetzen
            stream.Position = 0;
            email.Attach(new Attachment(stream, "Notenmeldung.pdf", System.Net.Mime.MediaTypeNames.Application.Pdf));
            email.Send();

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

    }
}
