using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MyStik.TimeTable.Data;
using MyStik.TimeTable.Web.Models;

namespace MyStik.TimeTable.Web.Controllers
{
    public class InternshipController : BaseController
    {
        // GET: Internship
        public ActionResult Index()
        {
            var user = GetCurrentUser();
            var student = StudentService.GetCurrentStudent(user);
            var internship = Db.Internships.FirstOrDefault(x => x.Student.Id == student.Id);

            var model = new InternshipStateModel
            {
                User = user,
                Student = student,
                Internship = internship
            };


            return View(model);
        }

        public ActionResult Check()
        {
            var user = GetCurrentUser();
            var student = StudentService.GetCurrentStudent(user);
            var thesis = Db.Internships.FirstOrDefault(x => x.Student.Id == student.Id);

            // Eine evtl. vorhandene alte Anfrage löschen
            if (thesis?.RequestAuthority != null)
            {
                thesis.ResponseDate = null;
                thesis.IsPassed = null;
                thesis.RequestAuthority = null;

                Db.SaveChanges();
            }


            var model = new InternshipStateModel
            {
                User = user,
                Student = student,
                Internship = thesis
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult Check(ThesisDetailModel model)
        {
            var user = GetCurrentUser();
            var student = StudentService.GetCurrentStudent(user);
            var thesis = Db.Internships.FirstOrDefault(x => x.Student.Id == student.Id);


            if (thesis == null)
            {
                thesis = new Internship
                {
                    Student = student,
                    RequestDate = DateTime.Now,
                };

                Db.Internships.Add(thesis);
            }

            // die eigene Bestätigung
            thesis.ResponseDate = DateTime.Now;
            thesis.IsPassed = true;
            thesis.RequestAuthority = null;

            Db.SaveChanges();

            return RedirectToAction("Index");
        }

        public ActionResult Plan()
        {
            var user = GetCurrentUser();
            var student = StudentService.GetCurrentStudent(user);
            var thesis = Db.Internships.FirstOrDefault(x => x.Student.Id == student.Id);

            var model = new InternshipStateModel()
            {
                User = user,
                Student = student,
                Internship = thesis,
                PlanBeginn = DateTime.Today.ToShortDateString(),
                PlanEnd = DateTime.Today.AddDays(60).ToShortDateString()
            };



            return View(model);
        }

        [HttpPost]
        public ActionResult Plan(InternshipStateModel model)
        {
            var beginn = DateTime.Parse(model.PlanBeginn);
            var end = DateTime.Parse(model.PlanEnd);

            if (beginn < DateTime.Today)
            {
                var user = GetCurrentUser();
                var student = StudentService.GetCurrentStudent(user);
                var thesis2 = Db.Internships.FirstOrDefault(x => x.Student.Id == student.Id);

                var model2 = new InternshipStateModel()
                {
                    User = user,
                    Student = student,
                    Internship = thesis2,
                    PlanBeginn = DateTime.Today.ToShortDateString(),
                    PlanEnd = DateTime.Today.AddDays(60).ToShortDateString(),
                };

                ModelState.AddModelError("", "Der Beginn muss in der Zukunft liegen");

                return View(model2);
            }


            if (end <= beginn)
            {
                var user = GetCurrentUser();
                var student = StudentService.GetCurrentStudent(user);
                var thesis2 = Db.Internships.FirstOrDefault(x => x.Student.Id == student.Id);

                var model2 = new InternshipStateModel()
                {
                    User = user,
                    Student = student,
                    Internship = thesis2,
                    PlanBeginn = DateTime.Today.ToShortDateString(),
                    PlanEnd = DateTime.Today.AddDays(60).ToShortDateString(),
                };

                ModelState.AddModelError("", "Das Emde muss nach dem Beginn liegen");

                return View(model2);
            }


            var thesis = Db.Internships.SingleOrDefault(x => x.Id == model.Internship.Id);

            thesis.PlannedBegin = beginn;
            thesis.PlannedEnd = end;
            thesis.LastPlanChange = DateTime.Now;
            ;

            Db.SaveChanges();



            return RedirectToAction("Index");
        }


        public ActionResult Advisor(Guid id)
        {
            var thesis = Db.Internships.SingleOrDefault(x => x.Id == id);

            var model = new InternshipAdvisorViewModel
            {
                Internship = thesis
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
        public ActionResult Advisor(InternshipAdvisorViewModel model)
        {
            var thesis = Db.Internships.SingleOrDefault(x => x.Id == model.Internship.Id);

            var advisor = thesis.Advisors.FirstOrDefault();

            if (advisor == null)
            {
                advisor = new Advisor();
                advisor.Internship = thesis;
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