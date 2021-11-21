using System;
using System.Collections.Generic;
using System.IO;
using MyStik.TimeTable.Data;
using MyStik.TimeTable.Web.Models;
using System.Linq;
using System.Net.Mail;
using System.Threading;
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


            var theses = Db.Theses.Where(x =>
                    x.Student.Curriculum.Organiser.Id == org.Id && // Student zur Fakultät gehörend
                    (x.IssueDate != null && x.ProlongRequestDate != null && x.RenewalDate == null) && // angemeldet, Verlängerung angefragt, aber noch kein Datum gesetzt
                    x.DeliveryDate == null && // noch nich abgegeben
                    x.IsCleared == null // noch nicht archiviert
            ).ToList();

            ViewBag.RequestCount = theses.Count;



            return View("Index");
        }


        /// <summary>
        /// Angekündigt
        /// Alle Arbeiten, bei denen das Prüfungsamt noch nicht draufgesehen hat
        /// oder draufgesehen hat, aber die Voraussetzungen nicht erfüllt sind
        /// </summary>
        /// <returns></returns>
        public ActionResult Announced()
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
                    x.IssueDate == null &&                // noch nicht angemeldet
                    ((x.RequestAuthority == null) ||       // PA hat das noch nicht gesehen
                    (x.RequestAuthority != null && x.IsPassed.HasValue && x.IsPassed.Value == false)) // Voraussetzungen nicht erfüllt
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
        /// Angekündigt
        /// Alle Arbeiten, bei denen das Prüfungsamt noch nicht draufgesehen hat
        /// oder draufgesehen hat, aber die Voraussetzungen nicht erfüllt sind
        /// </summary>
        /// <returns></returns>
        public ActionResult Approved()
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
                    x.IssueDate == null &&                // noch nicht angemeldet
                    (x.RequestAuthority != null && x.IsPassed.HasValue && x.IsPassed.Value == true) // Voraussetzungen sind erfüllt
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






        public ActionResult Plannend()
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
                    (x.IsPassed.HasValue && x.IsPassed.Value == true) && // Voraussetzungen erfüllt
                    (x.IssueDate == null) // noch nicht angemeldet
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


        public ActionResult Issued()
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
                    (x.IssueDate != null) && // angemeldet
                    x.DeliveryDate == null      // noch nicht abgegeben
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


        public ActionResult Running()
        {
            var org = GetMyOrganisation();
            var userRight = GetUserRight(org);

            if (!userRight.IsExamAdmin)
            {
                return View("_NoAccess");
            }

            var userService = new UserInfoService();

            /*
            var theses = Db.Theses.Where(x =>
                    x.Student.Curriculum.Organiser.Id == org.Id && // Student zur Fakultät gehörend
                    (x.IssueDate != null && x.IssueDate.Value <= DateTime.Today) && // angemeldet in der Vergangenheit
                    x.DeliveryDate == null && // noch nich abgegeben
                    x.IsCleared == null // noch nicht archiviert
            ).ToList();
            */

            var theses = Db.Theses.Where(x =>
                    x.Student.Curriculum.Organiser.Id == org.Id && // Student zur Fakultät gehörend
                    (x.IssueDate != null && x.ProlongRequestDate !=null && x.RenewalDate == null) && // angemeldet, Verlängerung angefragt, aber noch kein Datum gesetzt
                    x.DeliveryDate == null && // noch nich abgegeben
                    x.IsCleared == null // noch nicht archiviert
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
                    x.IsCleared == null && // noch nicht archiviert
                    x.GradeDate == null && // noch keine Note gemeldet
                    x.DeliveryDate != null // abgegeben haben
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
                    x.IsCleared == null && // noch nicht archiviert
                    x.GradeDate != null // abgegeben haben
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

        public ActionResult Statistics()
        {
            var semesterList = new List<Semester>();

            // das aktuelle Semester
            var pSem = SemesterService.GetSemester(DateTime.Today);
            semesterList.Add(pSem);

            // Vorsemester
            pSem = SemesterService.GetPreviousSemester(pSem);
            semesterList.Add(pSem);

            // Vorvorsemester
            pSem = SemesterService.GetPreviousSemester(pSem);
            semesterList.Add(pSem);

            var org = GetMyOrganisation();

            var matrix = new Dictionary<Semester, Dictionary<Curriculum, int>>();

            foreach (var semester in semesterList)
            {
                matrix[semester] = new Dictionary<Curriculum, int>();
                var nextStartDay = semester.EndCourses.AddDays(1);
                foreach (var curriculum in org.Curricula)
                {

                    // benotete Arbeiten po Studiengang und Semester
                    var nThesis = Db.Theses.Count(x => x.GradeDate != null &&
                                                       semester.StartCourses <= x.GradeDate &&
                                                       x.GradeDate < nextStartDay &&
                                                       x.Student.Curriculum.Id == curriculum.Id);

                    matrix[semester][curriculum] = nThesis;
                }
            }

            var model = new ThesisStatisticsModel
            {
                Organiser = org,
                Semester = semesterList,
                Curricula = org.Curricula,
                Matrix = matrix
            };


            return View(model);
        }




        public ActionResult Details(Guid id)
        {
            var org = GetMyOrganisation();
            var userRight = GetUserRight(org);

            if (!userRight.IsExamAdmin)
            {
                return View("_NoAccess");
            }

            var thesis = Db.Theses.SingleOrDefault(x => x.Id == id);

            var user = GetUser(thesis.Student.UserId);
            var student = thesis.Student;

            if (user == null)
            {
                user = new ApplicationUser
                {
                    FirstName = "Kein Benutzerkonto"
                };
            }

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

            return RedirectToAction("Details", new { id = thesis.Id });
            //return RedirectToAction("Graduates");
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


            return RedirectToAction("Details", new {id = thesis.Id});
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


            return RedirectToAction("Details", new { id = thesis.Id });
            //return RedirectToAction("Announced");
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


            return RedirectToAction("Details", new { id = thesis.Id });
            //return RedirectToAction("Announced");
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



            return RedirectToAction("Details", new {id = thesis.Id});
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


            return RedirectToAction("Details", new {id = thesis.Id});
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
            ViewBag.Organiser = Db.Organisers.Where(x => x.Id == thesis.Student.Curriculum.Organiser.Id)
                .OrderBy(x => x.ShortName).Select(c => new SelectListItem
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
                return Json(new {result = "Redirect", url = Url.Action("RequestIncomplete")});
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


            return Json(new {result = "Redirect", url = Url.Action("Details", new {id = thesis.Id})});
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

            return RedirectToAction("Details", new {id = thesis.Id});
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
            //return RedirectToAction("Running");
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
            thesis.GradeDate = null;
            thesis.IsCleared = null;

            Db.SaveChanges();

            // Mail an Studierenden
            var model = InitMailModel(thesis, user);

            new MailController().ThesisSupervisorDeliveryStornoEMail(model).Deliver();



            return RedirectToAction("Details", new {id = thesis.Id});
        }

        public ActionResult StornoMarked(Guid id)
        {
            var userService = new UserInfoService();

            var user = GetCurrentUser();
            var thesis = Db.Theses.SingleOrDefault(x => x.Id == id);

            thesis.DeliveryDate = null;
            thesis.GradeDate = null;
            thesis.IsCleared = null;

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

        public ActionResult Extend(Guid id)
        {
            var org = GetMyOrganisation();
            var userRight = GetUserRight(org);

            if (!userRight.IsExamAdmin)
            {
                return View("_NoAccess");
            }

            var userService = new UserInfoService();

            var user = GetCurrentUser();
            var thesis = Db.Theses.SingleOrDefault(x => x.Id == id);

            var model = new ThesisExtendViewModel();
            model.Thesis = thesis;
            model.StudentUser = userService.GetUser(thesis.Student.UserId);
            model.NewDateEnd = thesis.ExpirationDate.Value.ToShortDateString();

            var culture = Thread.CurrentThread.CurrentUICulture;
            ViewBag.Culture = culture;

            return View(model);
        }

        [HttpPost]
        public ActionResult Extend(ThesisExtendViewModel model)
        {
            var thesis = Db.Theses.SingleOrDefault(x => x.Id == model.Thesis.Id);

            var date = DateTime.Parse(model.NewDateEnd);

            if (date <= thesis.ExpirationDate.Value)
            {
                ModelState.AddModelError("NewDateEnd", "Das neue Datum muss nach dem bisherigem Datum liegen.");

                var userService = new UserInfoService();
                var m = new ThesisExtendViewModel();
                m.Thesis = thesis;
                m.StudentUser = userService.GetUser(thesis.Student.UserId);
                m.NewDateEnd = thesis.ExpirationDate.Value.ToShortDateString();


                var culture = Thread.CurrentThread.CurrentUICulture;
                ViewBag.Culture = culture;

                return View(m);
            }


            thesis.RenewalDate = date;
            Db.SaveChanges();

            return RedirectToAction("Details", new { id = thesis.Id });
        }

        public ActionResult RepairExpiry()
        {
            var org = GetMyOrganisation();

            var model = org.Curricula.ToList();

            return View(model);
        }

        public ActionResult RepairExpiryGo()
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
                    (x.IssueDate != null) && // angemeldet
                    x.DeliveryDate == null      // noch nicht abgegeben
            ).ToList();

            var model = new List<ThesisStateModel>();

            foreach (var thesis in theses)
            {
                int period = 0;
                bool success = int.TryParse(thesis.Student.Curriculum.Version, out period);
                if (!success || period == 0)
                {
                    period = 3;
                }

                thesis.ExpirationDate = thesis.IssueDate.Value.AddMonths(period).AddDays(-1);
            }

            Db.SaveChanges();

            return RedirectToAction("Issued");
        }


        public ActionResult AcceptProlongRequest(Guid id)
        {
            var org = GetMyOrganisation();
            var userRight = GetUserRight(org);

            if (!userRight.IsExamAdmin)
            {
                return View("_NoAccess");
            }

            var userService = new UserInfoService();

            var thesis = Db.Theses.SingleOrDefault(x => x.Id == id);

            thesis.ProlongExaminationBoardAccepted = true;
            Db.SaveChanges();

            // Mail an Studierenden
            var user = GetCurrentUser();
            var model = InitMailModel(thesis, user);

            new MailController().ThesisSupervisorAcceptProlongEMail(model).Deliver();

            return RedirectToAction("Details", new { id = thesis.Id });
        }

        public ActionResult RejectProlongRequest(Guid id)
        {
            var org = GetMyOrganisation();
            var userRight = GetUserRight(org);

            if (!userRight.IsExamAdmin)
            {
                return View("_NoAccess");
            }

            var userService = new UserInfoService();

            var thesis = Db.Theses.SingleOrDefault(x => x.Id == id);

            thesis.ProlongExaminationBoardAccepted = false;
            Db.SaveChanges();


            // Mail an Studierenden
            var user = GetCurrentUser();
            var model = InitMailModel(thesis, user);

            new MailController().ThesisSupervisorRejectProlongEMail(model).Deliver();


            return RedirectToAction("Details", new { id = thesis.Id });
        }

    }
}