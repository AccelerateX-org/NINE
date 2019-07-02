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


        public ActionResult Details(Guid id)
        {
            var org = GetMyOrganisation();
            var userRight = GetUserRight(org);

            var thesis = Db.Theses.SingleOrDefault(x => x.Id == id);

            var user = GetUser(thesis.Student.UserId);
            var student = thesis.Student;
            var member = GetMyMembership();

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
                var tm = new ThesisStateModel()
                {
                    Thesis = thesis,
                    Student = thesis.Student,
                    User = userService.GetUser(thesis.Student.UserId)
                };

                new MailController().ThesisSupervisorTitleChangedEMail(tm, user).Deliver();

            }


            return RedirectToAction("Details", new { id = thesis.Id });
        }

        public ActionResult Accept(Guid id)
        {
            var userService = new UserInfoService();

            var user = GetCurrentUser();
            var member = GetMyMembership();

            var supervisor = Db.Supervisors.SingleOrDefault(x => x.Id == id);
            var thesis = supervisor.Thesis;


            supervisor.AcceptanceDate = DateTime.Now;
            Db.SaveChanges();

            // Mail an Studierenden
            var model = new ThesisStateModel()
            {
                Thesis = thesis,
                Student = thesis.Student,
                User = userService.GetUser(thesis.Student.UserId)
            };

            new MailController().ThesisSupervisionResponseEMail(model, supervisor, user).Deliver();



            return RedirectToAction("Details", new { id = thesis.Id });
        }

        public ActionResult Reject(Guid id)
        {
            var userService = new UserInfoService();

            var user = GetCurrentUser();
            var member = GetMyMembership();

            var supervisor = Db.Supervisors.SingleOrDefault(x => x.Id == id);
            var thesis = supervisor.Thesis;

            thesis.Supervisors.Remove(supervisor);
            Db.Supervisors.Remove(supervisor);
            Db.SaveChanges();

            // Mail an Studierenden
            var model = new ThesisStateModel()
            {
                Thesis = thesis,
                Student = thesis.Student,
                User = userService.GetUser(thesis.Student.UserId)
            };

            new MailController().ThesisSupervisionResponseEMail(model, null, user).Deliver();



            return RedirectToAction("Details", new { id = thesis.Id });
        }


        public ActionResult Remove(Guid id)
        {
            var userService = new UserInfoService();

            var user = GetCurrentUser();
            var member = GetMyMembership();

            var supervisor = Db.Supervisors.SingleOrDefault(x => x.Id == id);
            var thesis = supervisor.Thesis;

            var supervisorUser = GetUser(supervisor.Member.UserId);

            thesis.Supervisors.Remove(supervisor);
            Db.Supervisors.Remove(supervisor);
            Db.SaveChanges();



            // Mail an Betreuer mit Kopie an Studierenden
            var model = new ThesisStateModel()
            {
                Thesis = thesis,
                Student = thesis.Student,
                User = userService.GetUser(thesis.Student.UserId)
            };

            new MailController().ThesisSupervisorRemoveEMail(model, supervisorUser, user).Deliver();



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
            var model = new ThesisStateModel()
            {
                Thesis = thesis,
                Student = thesis.Student,
                User = userService.GetUser(thesis.Student.UserId)
            };

            new MailController().ThesisSupervisorIssuedEMail(model, user).Deliver();



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
            var model = new ThesisStateModel()
            {
                Thesis = thesis,
                Student = thesis.Student,
                User = userService.GetUser(thesis.Student.UserId)
            };

            new MailController().ThesisSupervisorDeliveredEMail(model, user).Deliver();



            return RedirectToAction("Details", new { id = thesis.Id });
        }


        public ActionResult Marking(Guid id)
        {
            var thesis = Db.Theses.SingleOrDefault(x => x.Id == id);

            var model = new ThesisMarkingModel
            {
                Thesis = thesis,
                Mark = ""
            };


            return View(model);
        }


        [HttpPost]
        public ActionResult Marking(ThesisMarkingModel model)
        {
            var userService = new UserInfoService();
            var user = GetCurrentUser();

            var thesis = Db.Theses.SingleOrDefault(x => x.Id == model.Thesis.Id);

            thesis.GradeDate = DateTime.Now;
            Db.SaveChanges();

            // Mail mit Notenbeleg zum Ausdrucken an sich selbst senden
            var tm = new ThesisStateModel()
            {
                Thesis = thesis,
                Student = thesis.Student,
                User = userService.GetUser(thesis.Student.UserId),
                Mark = model.Mark
            };


            // hier zunächst mit Postal - weil es so geht
            var stream = new MemoryStream();

            var email = new ThesisEmail("ThesisMarked");
            email.To = user.Email;
            email.From = MailController.InitSystemFrom();
            email.Subject = "Notenmeldung Abschlussarbeit";
            email.Thesis = tm;
            email.Receiver = user;
            
            var html = this.RenderViewToString("_PrintOut", email);
            PdfDocument pdf = PdfGenerator.GeneratePdf(html, PageSize.A4);
            //pdf.Save("document.pdf");
            pdf.Save(stream, false);

            // Stream zurücksetzen
            stream.Position = 0;
            email.Attach(new Attachment(stream, "Notenmeldung.pdf", System.Net.Mime.MediaTypeNames.Application.Pdf));
            email.Send();
            
            return RedirectToAction("Details", new { id = thesis.Id });
        }

        public ActionResult MarkingEmpty(Guid id)
        {
            var userService = new UserInfoService();
            var user = GetCurrentUser();

            var thesis = Db.Theses.SingleOrDefault(x => x.Id == id);

            thesis.GradeDate = DateTime.Now;
            Db.SaveChanges();

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

            var html = this.RenderViewToString("_PrintOut", email);
            PdfDocument pdf = PdfGenerator.GeneratePdf(html, PageSize.A4);
            //pdf.Save("document.pdf");
            pdf.Save(stream, false);

            // Stream zurücksetzen
            stream.Position = 0;
            email.Attach(new Attachment(stream, "Notenmeldung.pdf", System.Net.Mime.MediaTypeNames.Application.Pdf));
            email.Send();

            return RedirectToAction("Details", new { id = thesis.Id });
        }

    }
}
