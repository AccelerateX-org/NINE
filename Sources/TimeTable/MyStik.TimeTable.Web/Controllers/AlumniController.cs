using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using MyStik.TimeTable.Data;
using MyStik.TimeTable.Web.Models;
using System.ComponentModel.DataAnnotations;

namespace MyStik.TimeTable.Web.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [AllowAnonymous]
    public class AlumniController : BaseController
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult RequestKey(string email)
        {
            var isValidAddress = new EmailAddressAttribute().IsValid(email);
            if (!isValidAddress) 
                return RedirectToAction("KeySent", new { email = email });

            var alumni = Db.Alumnae.FirstOrDefault(x =>
                !string.IsNullOrEmpty(x.Email) && x.Email.ToLower().Equals(email.ToLower()));

            if (alumni == null)
            {
                var seed = DateTime.Now.TimeOfDay.Milliseconds;
                var rand = new Random(seed);
                var code = rand.Next(100000, 999999);

                alumni = new Alumnus
                {
                    Code = code,
                    CodeExpiryDateTime = DateTime.Now.AddHours(24),
                    Email = email,
                    IsValid = false,
                };
                Db.Alumnae.Add(alumni);
                Db.SaveChanges();

                // jetzt E-mail versenden
                try
                {
                    new MailController().AlumniInvitationEMail(alumni).Deliver();
                }
                catch (Exception e)
                {
                    // ignored
                }
            }
            else
            {
                // Trotzdem so tun als ob E-Mail versendet, aber anderer Text
            }

            return RedirectToAction("KeySent", new {email = email});
        }

        public ActionResult KeySent(string email)
        {
            ViewBag.Email = email;

            return View();
        }



        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult ContactData(Guid id)
        {
            var alumni = Db.Alumnae.SingleOrDefault(x => x.Id == id);
            return View(alumni);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ContactData(Alumnus model)
        {



            return RedirectToAction("ThankYou", new {id = model.Id});
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "SysAdmin")]
        public ActionResult Delete()
        {
            // Alle Alumni löschen
            foreach (var alumni in Db.Alumnae.ToList())
            {
                var user = UserManager.FindById(alumni.UserId);
                UserManager.Delete(user);

                Db.Alumnae.Remove(alumni);
            }
            Db.SaveChanges();
            
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Bestätigung als Almuni eines Studiengangs
        /// </summary>
        /// <param name="id">Student</param>
        /// <returns></returns>
        public ActionResult Accept(Guid id)
        {
            // Das kann nur der aktuelle user für sich
            var user = GetCurrentUser();

            var student = Db.Students.SingleOrDefault(x => x.Id == id);

            if (!user.Id.Equals(student.UserId))
            {
                return View("_NoAccess");
            }



            // Check gibt es dazu schon einen Alumni
            // Link über die user id
            var alumni = Db.Alumnae.SingleOrDefault(x =>
                x.UserId.Equals(student.UserId) && x.Curriculum.Id == student.Curriculum.Id);

            // Standard: es gibt den alumni noch nicht
            if (alumni == null)
            {
                // anlegen, aber nicht speichern
                alumni = new Alumnus
                {
                    Curriculum = student.Curriculum,
                    Semester = student.LastSemester
                };
            }


            var model = new AlumniViewModel
            {
                Student = student,
                Alumni = alumni
            };


            return View(model);
        }

        [HttpPost]
        public ActionResult Accept(AlumniViewModel model)
        {
            // den Alumni anlegen, falls es ihn noch nicht gibt
            var student = Db.Students.SingleOrDefault(x => x.Id == model.Student.Id);

            var alumni = Db.Alumnae.SingleOrDefault(x =>
                x.UserId.Equals(student.UserId) && x.Curriculum.Id == student.Curriculum.Id);

            // Standard: es gibt den alumni noch nicht
            if (alumni == null)
            {
                // anlegen, aber nicht speichern
                alumni = new Alumnus
                {
                    Curriculum = student.Curriculum,
                    Semester = student.LastSemester,
                    UserId = student.UserId
                };

                Db.Alumnae.Add(alumni);
                Db.SaveChanges();

                return RedirectToAction("ThankYou", new { id = alumni.Id });
            }

            return RedirectToAction("Curricula", "Subscription");
        }

        public ActionResult ThankYou(Guid id)
        {
            var alumni = Db.Alumnae.SingleOrDefault(x => x.Id == id);
            var user = UserManager.FindById(alumni.UserId);

            var model = new AlumniViewModel
            {
                Alumni = alumni,
                User = user
            };

            return View(model);
        }

        public ActionResult Deny(Guid id)
        {
            var alumni = Db.Alumnae.SingleOrDefault(x => x.Id == id);

            if (alumni != null)
            {
                Db.Alumnae.Remove(alumni);
                Db.SaveChanges();
            }

            return RedirectToAction("Curricula", "Subscription");
        }

        public FileResult Download()
        {
            var member = GetMyMembership();

            var alumni = Db.Alumnae.Where(x => x.Curriculum.Organiser.Id == member.Organiser.Id).ToList();

            var ms = new MemoryStream();
            var writer = new StreamWriter(ms, Encoding.Default);

            writer.Write(
                "Name;Vorname;Studiengang;Semester;E-Mail");

            writer.Write(Environment.NewLine);


            foreach (var alumnus in alumni)
            {
                var user = UserManager.FindById(alumnus.UserId);

                if (user != null)
                {
                    writer.Write("{0};{1};{2};{3};{4}",
                        user.LastName, user.FirstName,
                        alumnus.Curriculum.ShortName, alumnus.Semester.Name,
                        user.Email);
                    writer.Write(Environment.NewLine);
                }
            }

            writer.Flush();
            writer.Dispose();

            var sb = new StringBuilder();
            sb.Append("Alumni");
            sb.Append(member.Organiser.ShortName);
            sb.Append("_");
            sb.Append(DateTime.Today.ToString("yyyyMMdd"));
            sb.Append(".csv");

            return File(ms.GetBuffer(), "text/csv", sb.ToString());
        }

    }
}