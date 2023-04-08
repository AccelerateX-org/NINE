using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MyStik.TimeTable.Web.Models;
using MyStik.TimeTable.Web.Services;

namespace MyStik.TimeTable.Web.Controllers
{
    public class InternshipAdminController : BaseController
    {
        // GET: InternshipAdmin
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

            return View();
        }

        /// <summary>
        /// Angekündigt
        /// Alle Arbeiten, bei das Prüfungsamt noch nicht draufgesehen hat
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

            var theses = Db.Internships.Where(x =>
                    x.Student.Curriculum.Organiser.Id == org.Id && // Student zur Fakultät gehörend
                    (x.RequestAuthority == null) ||       // PA hat das noch nicht gesehen
                    (x.RequestAuthority != null && x.IsPassed.HasValue && x.IsPassed.Value == false) // Voraussetzungen nicht erfüllt
            ).ToList();

            var model = new List<InternshipStateModel>();

            foreach (var thesis in theses)
            {
                var tm = new InternshipStateModel
                {
                    Internship = thesis,
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

            var theses = Db.Internships.Where(x =>
                    x.Student.Curriculum.Organiser.Id == org.Id && // Student zur Fakultät gehörend
                    (x.IsPassed.HasValue && x.IsPassed.Value == true) && // Voraussetzungen erfüllt
                    (x.AcceptedDate == null) // noch nicht angemeldet
            ).ToList();

            var model = new List<InternshipStateModel>();

            foreach (var thesis in theses)
            {
                var tm = new InternshipStateModel
                {
                    Internship = thesis,
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

            var thesis = Db.Internships.SingleOrDefault(x => x.Id == id);

            var user = GetUser(thesis.Student.UserId);
            var student = thesis.Student;

            if (user == null)
            {
                user = new ApplicationUser
                {
                    FirstName = "Kein Benutzerkonto"
                };
            }

            var model = new InternshipStateModel
            {
                User = user,
                Student = student,
                Internship = thesis
            };

            ViewBag.UserRight = userRight;
            ViewBag.Organiser = org;


            return View(model);
        }


        /// <summary>
        /// Anzeige des Annahme / Ablehnungsdialog
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Approval(Guid id)
        {
            var userService = new UserInfoService();
            var thesis = Db.Internships.SingleOrDefault(x => x.Id == id);

            var model = new InternshipStateModel()
            {
                Internship = thesis,
                Student = thesis.Student,
                User = userService.GetUser(thesis.Student.UserId)
            };

            ViewBag.UserRight = GetUserRight();

            return View(model);
        }




        public ActionResult Deny(Guid id)
        {
            var userService = new UserInfoService();
            var thesis = Db.Internships.SingleOrDefault(x => x.Id == id);

            var model = new InternshipDetailModel()
            {
                Internship = thesis,
                Student = thesis.Student,
                User = userService.GetUser(thesis.Student.UserId)
            };

            ViewBag.UserRight = GetUserRight();

            return View(model);
        }

        [HttpPost]
        public ActionResult Deny(InternshipDetailModel model)
        {
            var member = GetMyMembership();
            var user = GetCurrentUser();

            var thesis = Db.Internships.SingleOrDefault(x => x.Id == model.Internship.Id);

            thesis.RequestMessage = model.Internship.RequestMessage;
            thesis.ResponseDate = DateTime.Now;
            thesis.IsPassed = false;
            thesis.RequestAuthority = member;

            Db.SaveChanges();

            /*
            var tm = InitMailModel(thesis, user);

            new MailController().ThesisConditionCheckResponseEMail(tm).Deliver();
            */


            return RedirectToAction("Announced");
        }


        [HttpPost]
        public ActionResult Approve(InternshipDetailModel model)
        {
            var member = GetMyMembership();
            var user = GetCurrentUser();

            var thesis = Db.Internships.SingleOrDefault(x => x.Id == model.Internship.Id);

            thesis.ResponseDate = DateTime.Now;
            thesis.IsPassed = true;
            thesis.RequestAuthority = member;

            Db.SaveChanges();

            /*
            var tm = InitMailModel(thesis, user);

            new MailController().ThesisConditionCheckResponseEMail(tm).Deliver();
            */


            return RedirectToAction("Announced");
        }

    }
}