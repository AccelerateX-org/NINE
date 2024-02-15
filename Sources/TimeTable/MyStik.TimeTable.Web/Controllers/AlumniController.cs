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
using Org.BouncyCastle.Asn1.Utilities;
using Postal;

namespace MyStik.TimeTable.Web.Controllers
{
    public class AlumniContactDataModel
    {
        public Alumnus Alumni { get; set; }
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string Code { get; set; }

        public string Gender { get; set; }
        public string Title { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public Guid OrgId { get; set; }
        public Guid CurrId { get; set; }

        public string CurrName { get; set; }
        public string Degree { get; set; }


        public string Semester { get; set; }
        public string Year { get; set; }
    }


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
        [AllowAnonymous]
        public ActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
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
                var code = rand.Next(100000, 999999).ToString();

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

            return RedirectToAction("KeySent", new { email = email });
        }


        [AllowAnonymous]
        public ActionResult KeySent(string email)
        {
            ViewBag.Email = email;

            return View();
        }




        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        public ActionResult ContactData(Guid id)
        {
            var alumni = Db.Alumnae.SingleOrDefault(x => x.Id == id);

            AlumniContactDataModel model = new AlumniContactDataModel
            {
                Alumni = alumni,
                Id = alumni.Id,
                Code = alumni.Code,
                Email = alumni.Email,
                FirstName = alumni.FirstName,
                LastName = alumni.LastName,
                Title = alumni.Title,
                Gender = alumni.Gender,
                CurrName = alumni.Program,
                Degree = alumni.Degree,
            };

            ViewBag.Organisers = Db.Organisers.Where(x => x.IsFaculty && !x.IsStudent).OrderBy(x => x.ShortName)
                .ToList();

            int today = DateTime.Today.Year;
            List<int> years = new List<int>();
            for (int y = today; y >= 1971; y--)
            {
                years.Add(y);
            }

            ViewBag.Years = years;

            ViewBag.Gender = new List<SelectListItem>();
            ViewBag.Gender.Add(new SelectListItem(){ Value = "(ohne)", Text = "(ohne)" });
            ViewBag.Gender.Add(new SelectListItem() { Value = "Frau", Text = "Frau" });
            ViewBag.Gender.Add(new SelectListItem() { Value = "Herr", Text = "Herr" });


            return View(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        public ActionResult ContactData(AlumniContactDataModel model)
        {
            var alumni = Db.Alumnae.SingleOrDefault(x => x.Id == model.Id);

            if (alumni != null &&
                alumni.CodeExpiryDateTime >= DateTime.Now)
            {
                var org = Db.Organisers.SingleOrDefault(x => x.Id == model.OrgId);
                var curr = Db.Curricula.SingleOrDefault(x => x.Id == model.CurrId);

                alumni.Gender = model.Gender;
                alumni.Title = model.Title;
                alumni.FirstName = model.FirstName;
                alumni.LastName = model.LastName;

                if (alumni.Semester == null)
                {
                    var sem = $"{model.Semester} {model.Year}";
                    alumni.FinishingSemester = sem;
                }

                if (alumni.Organiser == null && alumni.Curriculum == null)
                {
                    if (org != null)
                    {
                        alumni.Faculty = org.ShortName;
                        alumni.Organiser = org;
                    }
                    else
                    {
                        alumni.Faculty = model.CurrName;
                    }

                    if (curr != null)
                    {
                        alumni.Program = curr.Tag;
                        alumni.Degree = curr.Degree.ShortName;
                        alumni.Curriculum = curr;
                    }
                    else
                    {
                        alumni.Program = model.CurrName;
                        alumni.Degree = model.Degree;
                    }
                }
                else
                {
                    alumni.Faculty = alumni.Curriculum.Organiser.ShortName;
                    alumni.Program = alumni.Curriculum.Tag;
                }

                alumni.Created = DateTime.Now;
                alumni.IsValid = true;

                Db.SaveChanges();

                return RedirectToAction("ThankYou", new { id = alumni.Id });
            }

            return RedirectToAction("ThankYou", new { id = Guid.Empty });
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

        [AllowAnonymous]
        public ActionResult ThankYou(Guid? id)
        {
            if (id.HasValue && id != Guid.Empty)
            {
                var alumni = Db.Alumnae.SingleOrDefault(x => x.Id == id.Value);

                return View(alumni);
            }
            else
            {
                return View("ThankYouAnonymous");
            }
        }

        public ActionResult List(Guid id)
        {
            var org = GetOrganiser(id);
            var user = GetCurrentUser();
            var userRight = GetUserRight(org);

            if (!userRight.IsStudentAdmin)
            {
                return View("_NoAccess");
            }

            var model = Db.Alumnae.Where(x => x.Organiser != null && x.Organiser.Id == org.Id).ToList();

            ViewBag.Organiser = org;
            ViewBag.UserRight = userRight;

            return View(model);
        }

        public ActionResult Admin(Guid id)
        {
            var org = GetOrganiser(id);
            var user = GetCurrentUser();
            var userRight = GetUserRight(org);

            if (!userRight.IsStudentAdmin)
            {
                return View("_NoAccess");
            }

            var model = Db.Alumnae.ToList();

            ViewBag.Organiser = org;
            ViewBag.UserRight = userRight;

            return View(model);
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

        public ActionResult StayConnected(Guid id)
        {
            var sem = SemesterService.GetSemester(DateTime.Today);
            var student = Db.Students.SingleOrDefault(x => x.Id == id);
            var alumni = Db.Alumnae.SingleOrDefault(x => x.Student.Id == student.Id);

            if (alumni == null)
            {
                var user = GetUser(student.UserId);

                alumni = new Alumnus
                {
                    Code = string.Empty,
                    CodeExpiryDateTime = DateTime.Now.AddHours(24),
                    Email = user.Email,
                    IsValid = false,
                    Student = student,
                    Curriculum = student.Curriculum,
                    Organiser = student.Curriculum.Organiser,
                    UserId = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Semester = sem,
                    Degree = student.Curriculum.Degree.ShortName,
                    Faculty = student.Curriculum.Organiser.ShortName,
                    FinishingSemester = sem.Name,
                };

                Db.Alumnae.Add(alumni);
                Db.SaveChanges();
            }

            return RedirectToAction("ContactData", new { id = alumni.Id });
        }

        public ActionResult Repair(Guid id)
        {
            var org = GetOrganiser(id);

            var alumni = Db.Alumnae.Where(x => string.IsNullOrEmpty(x.Email)).ToList();

            foreach (var alumnus in alumni)
            {
                var user = GetUser(alumnus.UserId);
                if (user == null)
                {
                    Db.Alumnae.Remove(alumnus);
                }
                else
                {
                    alumnus.Email = user.Email;
                    alumnus.FirstName = user.FirstName;
                    alumnus.LastName = user.LastName;

                    if (alumnus.Curriculum != null)
                    {
                        alumnus.Organiser = alumnus.Curriculum.Organiser;
                        alumnus.Student = Db.Students.Where(x =>
                            x.Curriculum.Id == alumnus.Curriculum.Id && x.UserId.Equals(user.Id)).OrderByDescending(x => x.FirstSemester.StartCourses).FirstOrDefault();

                        alumnus.Faculty = alumnus.Curriculum.Organiser.ShortName;
                        alumnus.Program = alumnus.Curriculum.Tag;
                        if (alumnus.Curriculum.Degree != null)
                        {
                            alumnus.Degree = alumnus.Curriculum.Degree.ShortName;
                        }
                    }

                    if (alumnus.Semester != null)
                    {
                        alumnus.FinishingSemester = alumnus.Semester.Name;
                    }
                }
            }

            Db.SaveChanges();

            return RedirectToAction("Admin", new { id = id });
        }

        public ActionResult Remove(Guid id)
        {
            var org = GetOrganiser(id);

            var alumni = Db.Alumnae.Where(x => 
                (x.Organiser != null && x.Organiser.Id == org.Id) ||
                (x.Curriculum != null && x.Curriculum.Organiser.Id == org.Id)
                ).ToList();

            foreach (var alumnus in alumni)
            {
                Db.Alumnae.Remove(alumnus);
            }

            Db.SaveChanges();

            return RedirectToAction("Admin", new { id = id });
        }

    }

}