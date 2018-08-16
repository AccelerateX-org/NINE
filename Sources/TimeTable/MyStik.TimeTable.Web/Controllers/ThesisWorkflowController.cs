using System;
using System.Linq;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using MyStik.TimeTable.Data;
using MyStik.TimeTable.Web.Models;

namespace MyStik.TimeTable.Web.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public class ThesisWorkflowController : BaseController
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public ActionResult Accept()
        {
            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Accepted()
        {
            ViewBag.Vorname = "Max";
            ViewBag.Nachname = "Mustermann";
            ViewBag.Matrikelnr = "123456789";
            ViewBag.E_Mail = "mustermann@hm.edu";
            ViewBag.Thema = "Ich bin ein Thema";
            ViewBag.Firma = "Ich bin eine Firma";
            ViewBag.Expose = "Ich  bin ein Expose";

            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Denied()
        {
            ViewBag.Vorname = "Max";
            ViewBag.Nachname = "Mustermann";
            ViewBag.Matrikelnr = "123456789";
            ViewBag.E_Mail = "mustermann@hm.edu";
            ViewBag.Thema = "Ich bin ein Thema";
            ViewBag.Firma = "Ich bin eine Firma";
            ViewBag.Expose = "Ich  bin ein Expose";

            return View();
        }

        public ActionResult Issue()
        {


            return View();
        }


        [HttpPost]
        public ActionResult Issue(ThesisIssueModel model)
        {
            // was muss angelegt werden

            // aus dem Benutzernamen den User suchen
            var user = UserManager.FindByName(model.UserName);
            if (user == null)
            {
                ModelState.AddModelError("UserName", "Benutzer existiert nicht");
                return View(model);
            }

            var issueDate = DateTime.Parse(model.IssueDate);
            if (issueDate < DateTime.Today)
            {
                ModelState.AddModelError("IssueDate", "Ausgabedatum darf nicht in der Vergangenheit liegen");
                return View(model);
            }

            // jetzt alles anlegen
            var member = GetMyMembership();

            // den Studenten suchen
            var student = Db.Students.SingleOrDefault(x => x.UserId.Equals(user.Id));
            if (student == null)
            {
                student = new Student();
                student.UserId = user.Id;
            }

            // Die Prüfung finden
            var semester = SemesterService.GetSemester(DateTime.Today);

            // das Exam finden => das ist die Aktivität
            // gibt es bei mir schon ein Exam mit diesem Studierenden als Prüfling?
            var exam = Db.Activities.OfType<Exam>().SingleOrDefault(x => x.Owners.Any(y => y.Member.Id == member.Id) && x.StudentExams.Any(y => y.Examinee.Id == student.Id));
            if (exam == null)
            {
                var owner = new ActivityOwner
                {
                    Member = member,
                    IsLocked = false
                };

                exam = new Exam
                {
                    Name = "Abschlussarbeit",
                    ShortName = "AA",
                    Semester = semester
                };
                exam.Owners.Add(owner);

                Db.ActivityOwners.Add(owner);
                Db.Activities.Add(exam);

                // ein StudentExam bauen
                var studentExam = new StudentExam();
                studentExam.Examinee = student;
                
                exam.StudentExams.Add(studentExam);

                Db.SaveChanges();
            }




            return RedirectToAction("Index");
        }

    }
}