﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using MyStik.TimeTable.Data;
using MyStik.TimeTable.Web.Models;
using System.Linq;
using System.Threading;
using System.Web.Mvc;
using MyStik.TimeTable.Web.Services;
using MyStik.TimeTable.Web.Utils;

namespace MyStik.TimeTable.Web.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public class ThesisController : BaseController
    {
        public ActionResult Index(Guid? id)
        {
            var user = GetCurrentUser();

            if (id != null)
            {
                // eine bestimmte Arbeit anzeigen
                var thesis = Db.Theses.Include(thesis1 => thesis1.Student).FirstOrDefault(x => x.Id == id);
                if (thesis == null)
                {
                    return HttpNotFound();
                }
                var model = new ThesisStateModel
                {
                    User = user,
                    Student = thesis.Student,
                    Thesis = thesis
                };
                return View(model);
            }

            var student = StudentService.GetCurrentStudent(user);
            var t = Db.Theses.FirstOrDefault(x => x.Student.Id == student.Id);
            if (t == null)
            {
                return HttpNotFound();
            }

            var m = new ThesisStateModel
            {
                User = user,
                Student = student,
                Thesis = t
            };

            return View(m);
        }
        public ActionResult Request()
        {
            var user = GetCurrentUser();
            var student = StudentService.GetCurrentStudent(user);
            var thesis = Db.Theses.FirstOrDefault(x => x.Student.Id == student.Id);

            // Eine evtl. vorhandene alte Anfrage löschen
            if (thesis?.RequestAuthority != null)
            {
                thesis.ResponseDate = null;
                thesis.IsPassed = null;
                thesis.RequestAuthority = null;

                Db.SaveChanges();
            }


            var model = new ThesisDetailModel
            {
                User = user,
                Student = student,
                Thesis = thesis
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult Request(ThesisDetailModel model)
        {
            var user = GetCurrentUser();
            var student = StudentService.GetCurrentStudent(user);
            var thesis = Db.Theses.FirstOrDefault(x => x.Student.Id == student.Id);


            if (student != null && !string.IsNullOrEmpty(model.Student.Number))
            {
                student.Number = model.Student.Number;
            }

            if (thesis == null)
            {
                thesis = new Thesis
                {
                    Student = student,
                    RequestDate = DateTime.Now,
                };

                Db.Theses.Add(thesis);
            }

            Db.SaveChanges();

            return RedirectToAction("Index");
        }


        public ActionResult Check()
        {
            var user = GetCurrentUser();
            var student = StudentService.GetCurrentStudent(user);
            var thesis = Db.Theses.FirstOrDefault(x => x.Student.Id == student.Id);

            // Eine evtl. vorhandene alte Anfrage löschen
            if (thesis?.RequestAuthority != null)
            {
                thesis.ResponseDate = null;
                thesis.IsPassed = null;
                thesis.RequestAuthority = null;

                Db.SaveChanges();
            }


            var model = new ThesisDetailModel
            {
                User = user,
                Student = student,
                Thesis = thesis
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult Check(ThesisDetailModel model)
        {
            var user = GetCurrentUser();
            var student = StudentService.GetCurrentStudent(user);
            var thesis = Db.Theses.FirstOrDefault(x => x.Student.Id == student.Id);


            if (thesis == null)
            {
                thesis = new Thesis
                {
                    Student = student,
                    RequestDate = DateTime.Now,
                };

                Db.Theses.Add(thesis);
            }

            // die eigene Bestätigung
            thesis.ResponseDate = DateTime.Now;
            thesis.IsPassed = true;
            thesis.RequestAuthority = null;

            Db.SaveChanges();

            return RedirectToAction("Index");
        }



        public ActionResult RequestSupervision(Guid id)
        {
            var thesis = Db.Theses.SingleOrDefault(x => x.Id == id);


            var model = new ThesisSupervisionModel
            {
                Thesis = thesis,
                OrganiserId = thesis.Student.Curriculum.Organiser.Id
            };


            // Liste aller Fakultäten
            // nur die eigene
            /*
            ViewBag.Organiser = Db.Organisers.Where(x => x.Id == thesis.Student.Curriculum.Organiser.Id).OrderBy(x => x.ShortName).Select(c => new SelectListItem
            {
                Text = c.ShortName,
                Value = c.Id.ToString(),
            });
            */

            ViewBag.Organiser = Db.Organisers.Where(x => x.IsFaculty && !x.IsStudent).OrderBy(x => x.ShortName).Select(c => new SelectListItem
            {
                Text = c.ShortName,
                Value = c.Id.ToString(),
            });


            return View(model);
        }

        [HttpPost]
        public ActionResult RequestSupervision(Guid id, Guid[] DozIds)
        {
            var thesis = Db.Theses.SingleOrDefault(x => x.Id == id);
            var user = GetCurrentUser();

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

            foreach (var supervisor in supervisors)
            {

                // der user des angefragten Lehrenden
                var supervisorUser = userService.GetUser(supervisor.Member.UserId);

                if (supervisorUser != null)
                {
                    var tm = InitMailModel(thesis, user);

                    new MailController().ThesisSupervisionRequestEMail(tm, supervisorUser).Deliver();
                }
            }


            return Json(new { result = "Redirect", url = Url.Action("Index") });
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


                // Mailversand bei Änderung des Titels
                /*
                if (thesis.IssueDate != null)
                {
                    // Mail an Studierenden, dass der Lehrende den Thema geändert hat
                    var tm = InitMailModel(thesis, user);

                    new MailController().ThesisSupervisorTitleChangedEMail(tm).Deliver();
                }
                */

            }


            return RedirectToAction("Index");
        }

        public ActionResult RemoveSupervisor(Guid id)
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
            var model = InitMailModel(thesis, user);

            new MailController().ThesisSupervisorRemoveEMail(model, supervisorUser).Deliver();



            return RedirectToAction("Index");
        }

        public ActionResult Issue()
        {
            var user = GetCurrentUser();
            var student = StudentService.GetCurrentStudent(user);
            var thesis = Db.Theses.FirstOrDefault(x => x.Student.Id == student.Id);

            var model = new ThesisStateModel
            {
                User = user,
                Student = student,
                Thesis = thesis,
                IssueDate = DateTime.Today.ToShortDateString()
            };

            if (thesis.PlannedBegin != null && thesis.PlannedBegin.Value > DateTime.Today)
            {
                model.IssueDate = thesis.PlannedBegin.Value.ToShortDateString();
            }


            var culture = Thread.CurrentThread.CurrentUICulture;
            ViewBag.Culture = culture;

            return View(model);
        }

        [HttpPost]
        public ActionResult Issue(ThesisStateModel model)
        {
            var date = DateTime.Parse(model.IssueDate);

            if (date < DateTime.Today)
            {
                var user = GetCurrentUser();
                var student = StudentService.GetCurrentStudent(user);
                var thesis2 = Db.Theses.FirstOrDefault(x => x.Student.Id == student.Id);

                var model2 = new ThesisStateModel
                {
                    User = user,
                    Student = student,
                    Thesis = thesis2,
                    IssueDate = DateTime.Today.ToShortDateString()
                };

                ModelState.AddModelError("", "Das Datum muss in der Zukunft liegen");

                return View(model2);
            }


            var thesis = Db.Theses.SingleOrDefault(x => x.Id == model.Thesis.Id);

            var period = thesis.Student.Curriculum.ThesisDuration;
            if (period == 0)
            {
                period = 3;
            }

            thesis.IssueDate = DateTime.Parse(model.IssueDate);
            thesis.ExpirationDate = thesis.IssueDate.Value.AddMonths(period).AddDays(-1);

            Db.SaveChanges();



            return RedirectToAction("Index");
        }

        public ActionResult MoveIssue()
        {
            var user = GetCurrentUser();
            var student = StudentService.GetCurrentStudent(user);
            var thesis = Db.Theses.FirstOrDefault(x => x.Student.Id == student.Id);

            if (thesis.PlannedBegin.HasValue && thesis.IssueDate.HasValue &&
                thesis.IssueDate.Value < thesis.PlannedBegin.Value &&
                thesis.PlannedBegin.Value > DateTime.Today)
            {
                thesis.IssueDate = thesis.PlannedBegin;
                Db.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        public ActionResult Prolong()
        {
            var user = GetCurrentUser();
            var student = StudentService.GetCurrentStudent(user);
            var thesis = Db.Theses.FirstOrDefault(x => x.Student.Id == student.Id);

            var model = new ThesisStateModel
            {
                User = user,
                Student = student,
                Thesis = thesis,
                ProlongDate = DateTime.Today.ToShortDateString(),
                ProlongReason = ""
            };

            var culture = Thread.CurrentThread.CurrentUICulture;
            ViewBag.Culture = culture;


            return View(model);
        }

        [HttpPost]
        public ActionResult Prolong(ThesisStateModel model)
        {
            var date = DateTime.Parse(model.ProlongDate);
            var user = GetCurrentUser();

            if (date < DateTime.Today)
            {
                var student = StudentService.GetCurrentStudent(user);
                var thesis2 = Db.Theses.FirstOrDefault(x => x.Student.Id == student.Id);

                var model2 = new ThesisStateModel
                {
                    User = user,
                    Student = student,
                    Thesis = thesis2,
                    ProlongDate = DateTime.Today.ToShortDateString(),
                    ProlongReason = model.ProlongReason
                };

                ModelState.AddModelError("", "Das Datum muss in der Zukunft liegen");

                return View(model2);
            }




            var thesis = Db.Theses.SingleOrDefault(x => x.Id == model.Thesis.Id);


            thesis.ProlongRequestDate = DateTime.Now;
            thesis.ProlongExtensionDate = DateTime.Parse(model.ProlongDate);
            thesis.ProlongReason = model.ProlongReason;

            Db.SaveChanges();


            var userService = new UserInfoService();

            foreach (var supervisor in thesis.Supervisors)
            {

                // der user des angefragten Lehrenden
                var supervisorUser = userService.GetUser(supervisor.Member.UserId);

                if (supervisorUser != null)
                {
                    var tm = InitMailModel(thesis, user);

                    new MailController().ThesisSupervisionProlongRequestEMail(tm, supervisorUser).Deliver();
                }
            }



            return RedirectToAction("Index");
        }


        public ActionResult ProlongAgain()
        {
            var user = GetCurrentUser();
            var student = StudentService.GetCurrentStudent(user);
            var thesis = Db.Theses.FirstOrDefault(x => x.Student.Id == student.Id);

            var model = new ThesisStateModel
            {
                User = user,
                Student = student,
                Thesis = thesis,
                ProlongDate = DateTime.Today.ToShortDateString(),
                ProlongReason = ""
            };

            var culture = Thread.CurrentThread.CurrentUICulture;
            ViewBag.Culture = culture;


            return View("Prolong", model);
        }

        [HttpPost]
        public ActionResult ProlongAgain(ThesisStateModel model)
        {
            var date = DateTime.Parse(model.ProlongDate);
            var user = GetCurrentUser();

            if (date < DateTime.Today)
            {
                var student = StudentService.GetCurrentStudent(user);
                var thesis2 = Db.Theses.FirstOrDefault(x => x.Student.Id == student.Id);

                var model2 = new ThesisStateModel
                {
                    User = user,
                    Student = student,
                    Thesis = thesis2,
                    ProlongDate = DateTime.Today.ToShortDateString(),
                    ProlongReason = model.ProlongReason
                };

                ModelState.AddModelError("", "Das Datum muss in der Zukunft liegen");

                return View("Prolong", model2);
            }




            var thesis = Db.Theses.SingleOrDefault(x => x.Id == model.Thesis.Id);

            thesis.RenewalDate = null;      // das bisherige Verlängerungsdatum löschen
            thesis.ProlongRequestDate = DateTime.Now;
            thesis.ProlongExtensionDate = DateTime.Parse(model.ProlongDate);
            thesis.ProlongReason = model.ProlongReason;
            thesis.ProlongSupervisorAccepted = null;
            thesis.ProlongExaminationBoardAccepted = null;
            thesis.ProlongRejection = string.Empty;

            Db.SaveChanges();


            var userService = new UserInfoService();

            foreach (var supervisor in thesis.Supervisors)
            {

                // der user des angefragten Lehrenden
                var supervisorUser = userService.GetUser(supervisor.Member.UserId);

                if (supervisorUser != null)
                {
                    var tm = InitMailModel(thesis, user);

                    new MailController().ThesisSupervisionProlongRequestEMail(tm, supervisorUser).Deliver();
                }
            }



            return RedirectToAction("Index");
        }




        public ActionResult Plan()
        {
            var user = GetCurrentUser();
            var student = StudentService.GetCurrentStudent(user);
            var thesis = Db.Theses.FirstOrDefault(x => x.Student.Id == student.Id);

            var model = new ThesisStateModel
            {
                User = user,
                Student = student,
                Thesis = thesis,
                IssueDate = DateTime.Today.AddDays(30).ToShortDateString()
            };

            var culture = Thread.CurrentThread.CurrentUICulture;
            ViewBag.Culture = culture;


            return View(model);
        }

        [HttpPost]
        public ActionResult Plan(ThesisStateModel model)
        {
            var date = DateTime.Parse(model.IssueDate);

            if (date < DateTime.Today)
            {
                var user = GetCurrentUser();
                var student = StudentService.GetCurrentStudent(user);
                var thesis2 = Db.Theses.FirstOrDefault(x => x.Student.Id == student.Id);

                var model2 = new ThesisStateModel
                {
                    User = user,
                    Student = student,
                    Thesis = thesis2,
                    IssueDate = DateTime.Today.ToShortDateString()
                };

                ModelState.AddModelError("", "Das Datum muss in der Zukunft liegen");

                return View(model2);
            }


            var thesis = Db.Theses.SingleOrDefault(x => x.Id == model.Thesis.Id);

            var period = thesis.Student.Curriculum.ThesisDuration;
            if (period == 0)
            {
                period = 3;
            }

            thesis.PlannedBegin = DateTime.Parse(model.IssueDate);
            thesis.PlannedEnd = thesis.PlannedBegin.Value.AddMonths(period);
            thesis.LastPlanChange = DateTime.Now;
            ;

            Db.SaveChanges();



            return RedirectToAction("Index");
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


        public ActionResult Advisor(Guid id)
        {
            var thesis = Db.Theses.SingleOrDefault(x => x.Id == id);

            var model = new ThesisAdvisorViewModel
            {
                Thesis = thesis
            };

            var advisor = thesis.Advisors.FirstOrDefault();
            if (advisor != null)
            {
                model.CorporateName = advisor.CorporateName;
                model.PersonFirstName = advisor.PersonFirstName;
                model.PersonLastName = advisor.PersonLastName;
                model.PersonAction = advisor.PersonAction;
                model.PersonEMail = advisor.PersonEMail;
                model.PersonPhone = advisor.PersonPhone;
            }


            return View(model);
        }


        [HttpPost]
        public ActionResult Advisor(ThesisAdvisorViewModel model)
        {
            var thesis = Db.Theses.SingleOrDefault(x => x.Id == model.Thesis.Id);

            var advisor = thesis.Advisors.FirstOrDefault();

            if (advisor == null)
            {
                advisor = new Advisor();
                advisor.Thesis = thesis;
                thesis.Advisors.Add(advisor);
            }

            advisor.CorporateName = model.CorporateName;
            advisor.PersonFirstName = model.PersonFirstName;
            advisor.PersonLastName = model.PersonLastName;
            advisor.PersonAction = model.PersonAction;
            advisor.PersonEMail = model.PersonEMail;
            advisor.PersonPhone = model.PersonPhone;

            Db.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}