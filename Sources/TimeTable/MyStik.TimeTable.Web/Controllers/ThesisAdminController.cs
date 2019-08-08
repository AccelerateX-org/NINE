using System;
using System.Collections.Generic;
using System.IO;
using MyStik.TimeTable.Data;
using MyStik.TimeTable.Web.Models;
using System.Linq;
using System.Net.Mail;
using System.Web.Mvc;
using MyStik.TimeTable.Web.Services;
using MyStik.TimeTable.Web.Utils;
using PdfSharp;
using PdfSharp.Pdf;
using TheArtOfDev.HtmlRenderer.PdfSharp;

namespace MyStik.TimeTable.Web.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public class ThesisAdminController : BaseController
    {
        /// <summary>
        /// Der Status meiner Abschlussarbeit
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            // der aktuelle Benutzer
            var org = GetMyOrganisation();
            var userRight = GetUserRight(org);

            if (!userRight.IsExamAdmin)
            {
                return View("_NoAccess");
            }

            ViewBag.UserRight = userRight;
            ViewBag.Organiser = org;

            return View("Index");
        }

        public ActionResult Running()
        {
            var org = GetMyOrganisation();
            var userRight = GetUserRight(org);

            if (!userRight.IsExamAdmin)
            {
                return View("_NoAccess");
            }

            var userService = new UserInfoService();

            var theses = Db.Theses.Where(x =>
                    x.Student.Curriculum.Organiser.Id == org.Id && // Student zur Fakultät gehörend
                    x.DeliveryDate == null &&   // noch nich abgegeben
                    x.IsCleared == null         // noch nicht archiviert
            ).ToList();

            var model = new List<ThesisStateModel>();

            foreach (var thesis in theses)
            {
                var tm = new ThesisStateModel
                {
                    Thesis = thesis,
                    Student = thesis.Student,
                    User = userService.GetUser(thesis.Student.UserId)
                };

                model.Add(tm);
            }

            ViewBag.UserRight = userRight;
            ViewBag.Organiser = org;


            return View(model);
        }




        /// <summary>
        /// Absolventen, sind alle mit abegebener Arbeit
        /// </summary>
        /// <returns></returns>
        public ActionResult Done()
        {
            var org = GetMyOrganisation();
            var userRight = GetUserRight(org);

            if (!userRight.IsExamAdmin)
            {
                return View("_NoAccess");
            }

            var userService = new UserInfoService();

            var theses = Db.Theses.Where(x =>
                    x.Student.Curriculum.Organiser.Id == org.Id && // Student zur Fakultät gehörend
                    x.IsCleared == null  &&     // noch nicht archiviert
                    x.GradeDate == null &&      // noch keine Note gemeldet
                    x.DeliveryDate != null      // abgegeben haben
            ).ToList();

            var model = new List<ThesisStateModel>();

            foreach (var thesis in theses)
            {
                var tm = new ThesisStateModel
                {
                    Thesis = thesis,
                    Student = thesis.Student,
                    User = userService.GetUser(thesis.Student.UserId)
                };

                model.Add(tm);
            }

            ViewBag.UserRight = userRight;
            ViewBag.Organiser = org;


            return View(model);
        }


        /// <summary>
        /// Absolventen, sind alle mit gemeldeter Note
        /// </summary>
        /// <returns></returns>
        public ActionResult Graduates()
        {
            var org = GetMyOrganisation();
            var userRight = GetUserRight(org);

            if (!userRight.IsExamAdmin)
            {
                return View("_NoAccess");
            }

            var userService = new UserInfoService();

            var theses = Db.Theses.Where(x =>
                    x.Student.Curriculum.Organiser.Id == org.Id && // Student zur Fakultät gehörend
                    x.IsCleared == null &&      // noch nicht archiviert
                    x.GradeDate != null         // abgegeben haben
            ).ToList();

            var model = new List<ThesisStateModel>();

            foreach (var thesis in theses)
            {
                var tm = new ThesisStateModel
                {
                    Thesis = thesis,
                    Student = thesis.Student,
                    User = userService.GetUser(thesis.Student.UserId)
                };

                model.Add(tm);
            }

            ViewBag.UserRight = userRight;
            ViewBag.Organiser = org;


            return View(model);
        }



        public ActionResult Archived()
        {
            var org = GetMyOrganisation();
            var userRight = GetUserRight(org);

            if (!userRight.IsExamAdmin)
            {
                return View("NoAccess");
            }

            var userService = new UserInfoService();

            var theses = Db.Theses.Where(x =>
                    x.Student.Curriculum.Organiser.Id == org.Id && // Student zur Fakultät gehörend
                    x.IsCleared != null && x.IsCleared == true // archiviert
            ).ToList();

            var model = new List<ThesisStateModel>();

            foreach (var thesis in theses)
            {
                var tm = new ThesisStateModel
                {
                    Thesis = thesis,
                    Student = thesis.Student,
                    User = userService.GetUser(thesis.Student.UserId)
                };

                model.Add(tm);
            }

            ViewBag.UserRight = userRight;
            ViewBag.Organiser = org;


            return View(model);
        }




        public ActionResult Details(Guid id)
        {
            var org = GetMyOrganisation();
            var userRight = GetUserRight(org);

            var thesis = Db.Theses.SingleOrDefault(x => x.Id == id);

            var user = GetUser(thesis.Student.UserId);
            var student = thesis.Student;

            var model = new ThesisStateModel
            {
                User = user,
                Student = student,
                Thesis = thesis
            };

            ViewBag.UserRight = userRight;
            ViewBag.Organiser = org;


            return View(model);
        }

        public ActionResult Delete(Guid id)
        {
            var thesis = Db.Theses.SingleOrDefault(x => x.Id == id);

            return View(thesis);
        }


        public ActionResult DeleteConfirmed(Guid id)
        {
            var thesis = Db.Theses.SingleOrDefault(x => x.Id == id);

            foreach (var advisor in thesis.Advisors.ToList())
            {
                Db.Advisors.Remove(advisor);
            }

            foreach (var supervisor in thesis.Supervisors.ToList())
            {
                Db.Supervisors.Remove(supervisor);
            }

            Db.Theses.Remove(thesis);
            Db.SaveChanges();

            return RedirectToAction("Index");
        }

        public ActionResult Archive(Guid id)
        {
            var thesis = Db.Theses.SingleOrDefault(x => x.Id == id);

            thesis.IsCleared = true;

            Db.SaveChanges();

            return RedirectToAction("Graduates");
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

            if (thesis != null)
            {
                if (!string.IsNullOrEmpty(model.TitleDe))
                    thesis.TitleDe = model.TitleDe;

                if (!string.IsNullOrEmpty(model.TitleEn))
                    thesis.TitleEn = model.TitleEn;

                Db.SaveChanges();
            }


            return RedirectToAction("Details", new {id=thesis.Id});
        }


        /// <summary>
        /// Anzeige des Annahme / Ablehnungsdialog
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Approval(Guid id)
        {
            var userService = new UserInfoService();
            var thesis = Db.Theses.SingleOrDefault(x => x.Id == id);

            var model = new ThesisStateModel
            {
                Thesis = thesis,
                Student = thesis.Student,
                User = userService.GetUser(thesis.Student.UserId)
            };

            ViewBag.UserRight = GetUserRight();

            return View(model);
        }




        public ActionResult Deny(Guid id)
        {
            var userService = new UserInfoService();
            var thesis = Db.Theses.SingleOrDefault(x => x.Id == id);

            var model = new ThesisDetailModel
            {
                Thesis = thesis,
                Student = thesis.Student,
                User = userService.GetUser(thesis.Student.UserId)
            };

            ViewBag.UserRight = GetUserRight();

            return View(model);
        }

        [HttpPost]
        public ActionResult Deny(ThesisDetailModel model)
        {
            var member = GetMyMembership();
            var user = GetCurrentUser();

            var thesis = Db.Theses.SingleOrDefault(x => x.Id == model.Thesis.Id);

            thesis.RequestMessage = model.Thesis.RequestMessage;
            thesis.ResponseDate = DateTime.Now;
            thesis.IsPassed = false;
            thesis.RequestAuthority = member;

            Db.SaveChanges();

            var tm = InitMailModel(thesis, user);

            new MailController().ThesisConditionCheckResponseEMail(tm).Deliver();


            return RedirectToAction("Running");
        }


        [HttpPost]
        public ActionResult Approve(ThesisDetailModel model)
        {
            var member = GetMyMembership();
            var user = GetCurrentUser();

            var thesis = Db.Theses.SingleOrDefault(x => x.Id == model.Thesis.Id);

            thesis.ResponseDate = DateTime.Now;
            thesis.IsPassed = true;
            thesis.RequestAuthority = member;

            Db.SaveChanges();

            var tm = InitMailModel(thesis, user);

            new MailController().ThesisConditionCheckResponseEMail(tm).Deliver();


            return RedirectToAction("Running");
        }

        /// <summary>
        /// Annahme der Betreuung durch den Admin
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult AssignSupervisor(Guid id)
        {
            var userService = new UserInfoService();
            var user = GetCurrentUser();

            var supervisor = Db.Supervisors.SingleOrDefault(x => x.Id == id);
            var supervisorUser = userService.GetUser(supervisor.Member.UserId);

            var thesis = supervisor.Thesis;

            supervisor.AcceptanceDate = DateTime.Now;
            supervisor.Remark = $"Angenommen durch {user.FullName}";

            Db.SaveChanges();


            // Mail an Lehrenden
            var model = InitMailModel(thesis, user);
            model.AsSubstitute = true;

            new MailController().ThesisSupervisorAssignEMail(model, supervisorUser).Deliver();



            return RedirectToAction("Details", new { id = thesis.Id });
        }

        /// <summary>
        /// Betreuer entfernen
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult RemoveSupervisor(Guid id)
        {
            var userService = new UserInfoService();
            var user = GetCurrentUser();

            var supervisor = Db.Supervisors.SingleOrDefault(x => x.Id == id);
            var supervisorUser = userService.GetUser(supervisor.Member.UserId);

            var thesis = supervisor.Thesis;
            thesis.Supervisors.Remove(supervisor);

            Db.Supervisors.Remove(supervisor);
            Db.SaveChanges();

            // Mail an Lehrenden
            var model = InitMailModel(thesis, user);

            new MailController().ThesisSupervisorRemoveEMail(model, supervisorUser).Deliver();


            return RedirectToAction("Details", new { id = thesis.Id });
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
            ViewBag.Organiser = Db.Organisers.OrderBy(x => x.ShortName).Select(c => new SelectListItem
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

        public ActionResult RequestIncomplete()
        {
            var user = GetCurrentUser();
            var student = StudentService.GetCurrentStudent(user);
            var thesis = Db.Theses.FirstOrDefault(x => x.Student.Id == student.Id);


            var model = new ThesisStateModel
            {
                User = user,
                Student = student,
                Thesis = thesis
            };

            return View(model);
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
            var tm = InitMailModel(thesis, user);
            tm.AsSubstitute = true;

            new MailController().ThesisSupervisorIssuedEMail(tm).Deliver();

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

            return RedirectToAction("Running");
        }

        /// <summary>
        /// Stornierung der Abgabe
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


        /// <summary>
        /// Notenmeldung erfolgt
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Marked(Guid id)
        {
            // an den Stduierenden eine E-Mail mit Kopien an
            // Betreuer
            // Aktuellen Benutzer
            var userService = new UserInfoService();
            var user = GetCurrentUser();

            var thesis = Db.Theses.SingleOrDefault(x => x.Id == id);


            // Mail an Studierenden vorbereiten
            var model = InitMailModel(thesis, user);

            return View(model);
        }


        /// <summary>
        /// Notenmeldung erfolgt
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult SendMark(Guid id)
        {
            // an den Stduierenden eine E-Mail mit Kopien an
            // Betreuer
            // Aktuellen Benutzer
            var userService = new UserInfoService();
            var user = GetCurrentUser();

            var thesis = Db.Theses.SingleOrDefault(x => x.Id == id);
            thesis.GradeDate = DateTime.Today;


            // Annahme: Damit ist auch das Studium beendet
            var sem = SemesterService.GetSemester(DateTime.Today);
            thesis.Student.LastSemester = sem;

            Db.SaveChanges();


            // Mail an Studierenden
            var model = InitMailModel(thesis, user);

            new MailController().ThesisMarkedEMail(model).Deliver();

            return RedirectToAction("Done");
        }



        public ActionResult Marking(Guid id)
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
    }

}