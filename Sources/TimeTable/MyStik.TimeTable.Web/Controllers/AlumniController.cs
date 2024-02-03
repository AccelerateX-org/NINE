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

namespace MyStik.TimeTable.Web.Controllers
{
    public class AlumniContactDataModel
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public int Code { get; set; }

        public string Gender { get; set; }
        public string Title { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string Degree { get; set; }
        public Guid FacultyId { get; set; }
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

        public ActionResult RequestKeyInternal(string email, AlumniViewModel model)
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

                var student = Db.Students.SingleOrDefault(x => x.Id == model.Student.Id);
                if (student != null)
                {
                    alumni.Curriculum = student.Curriculum;
                    alumni.Organiser = student.Curriculum.Organiser;
                    alumni.UserId = student.UserId;
                    alumni.Degree = student.Curriculum.Degree.ShortName;
                    alumni.Faculty = student.Curriculum.Organiser.ShortName;
                    alumni.FinishingSemester = SemesterService.GetSemester(DateTime.Today).Name;
                }

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
        public ActionResult ContactData(Guid? id)
        {
            Alumnus alumni = null;
            if (id == null)
            {
                alumni = new Alumnus
                {
                    Code = 0000,
                    Id = Guid.NewGuid(),
                    Email = "dummy@acceleratex.org"
                };
            }
            else
            {
                alumni = Db.Alumnae.SingleOrDefault(x => x.Id == id) ?? new Alumnus
                {
                    Code = 0000,
                    Id = Guid.NewGuid(),
                    Email = "dummy@acceleratex.org"
                };
            }

            AlumniContactDataModel model = new AlumniContactDataModel
            {
                Id = alumni.Id,
                Code = alumni.Code,
                Email = alumni.Email,
            };

            // das ist dann eindeutig ein interner
            // soll nur noch Name und Titel angeben können
            if (alumni.Curriculum != null)
            {
                ViewBag.Curriculum = alumni.Curriculum;

                model.FirstName = alumni.FirstName;
                model.LastName = alumni.LastName;
                model.Title = alumni.Title;
                model.Gender = alumni.Gender;
            }


            ViewBag.Organisers = Db.Organisers.Where(x => x.IsFaculty && !x.IsStudent).OrderBy(x => x.ShortName).ToList();

            int today = DateTime.Today.Year;
            List<int> years = new List<int>();
            for (int y = today; y >= 1971; y--)
            {
                years.Add(y);
            }
            ViewBag.Years = years;

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

            if (alumni != null && model.Code == alumni.Code &&
                alumni.CodeExpiryDateTime >= DateTime.Now)
            {
                var org = Db.Organisers.SingleOrDefault(x => x.Id == model.FacultyId);

                if (org != null)
                {
                    var sem = $"{model.Semester} {model.Year}";
                    
                    alumni.Gender = model.Gender;
                    alumni.Title = model.Title;
                    alumni.FirstName = model.FirstName;
                    alumni.LastName = model.LastName;

                    if (alumni.Curriculum == null)
                    {
                        alumni.Degree = model.Degree;
                        alumni.Faculty = org.ShortName;
                        alumni.FinishingSemester = sem;
                        alumni.Organiser = org;
                    }

                    alumni.Created = DateTime.Now;
                    alumni.IsValid = true;
                  

                    Db.SaveChanges();
                }

                return RedirectToAction("ThankYou", new { id = alumni.Id });
            }

            return RedirectToAction("ThankYou", new {id = Guid.Empty});
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
            var user = GetCurrentUser();
            var student = Db.Students.SingleOrDefault(x => x.Id == id);
            var sem = SemesterService.GetSemester(DateTime.Today);

            if (student != null && student.UserId.Equals(user.Id))
            {
                var alumni =
                    Db.Alumnae.FirstOrDefault(x => !string.IsNullOrEmpty(x.UserId) && x.UserId.Equals(user.Id));

                if (alumni != null)
                {
                    // Zeig an, was angegeben wurde => aber auch zum Ändern, bis auf E-Mail Adresse
                }
                else
                {
                    if (user.Email.EndsWith("@hm.edu"))
                    {
                        // Code anfordern, erst dann Alumni auch anlegen
                        var model = new AlumniViewModel
                        {
                            Student = student,
                            User = user
                        };

                        return View("StayConnected", model);
                    }
                    else
                    {
                        alumni = new Alumnus
                        {
                            Email = user.Email,
                            Code = 0,
                            IsValid = false,
                            CodeExpiryDateTime = DateTime.Now.AddHours(24),
                            Curriculum = student.Curriculum,
                            Organiser = student.Curriculum.Organiser,
                            Semester = sem,
                            Degree = student.Curriculum.Degree.ShortName,
                            Faculty = student.Curriculum.Organiser.ShortName,
                            FirstName = user.FirstName,
                            LastName = user.LastName,
                            UserId = user.Id,
                            FinishingSemester = sem.Name,
                        };

                        Db.Alumnae.Add(alumni);
                        Db.SaveChanges();

                        return RedirectToAction("ContactData", new {id = alumni.Id} );
                    }

                }
            }

            return RedirectToAction("ThankYou");
        }

    }
}