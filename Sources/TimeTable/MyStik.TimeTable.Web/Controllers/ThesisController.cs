using System;
using System.Collections.Generic;
using MyStik.TimeTable.Data;
using MyStik.TimeTable.Web.Models;
using System.Linq;
using System.Web.Mvc;
using MyStik.TimeTable.Web.Services;

namespace MyStik.TimeTable.Web.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public class ThesisController : BaseController
    {
        public ActionResult MyWork()
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

            return RedirectToAction("MyWork");
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

            return RedirectToAction("MyWork");
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
            ViewBag.Organiser = Db.Organisers.OrderBy(x => x.ShortName).Select(c => new SelectListItem
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

                var tm = new ThesisStateModel
                {
                    Thesis = thesis,
                    Student = thesis.Student,
                    User = userService.GetUser(thesis.Student.UserId)
                };

                // der user des angefragten Lehrenden
                var user = userService.GetUser(supervisor.Member.UserId);

                if (user != null)
                {
                    new MailController().ThesisSupervisionRequestEMail(tm, user).Deliver();
                }
            }


            return Json(new { result = "Redirect", url = Url.Action("MyWork") });
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


            return RedirectToAction("MyWork");
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
            var model = new ThesisStateModel()
            {
                Thesis = thesis,
                Student = thesis.Student,
                User = userService.GetUser(thesis.Student.UserId)
            };

            new MailController().ThesisSupervisorRemoveEMail(model, supervisorUser, user).Deliver();



            return RedirectToAction("MyWork");
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

            int period = 0;
            bool success = int.TryParse(thesis.Student.Curriculum.Version, out period);
            if (!success || period == 0)
            {
                period = 3;
            }

            thesis.IssueDate = DateTime.Parse(model.IssueDate);
            thesis.ExpirationDate = thesis.IssueDate.Value.AddMonths(period);

            Db.SaveChanges();



            return RedirectToAction("MyWork");
        }


    }
}