using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Web.Mvc;
//using System.Web.Security;
using log4net;
using Microsoft.AspNet.Identity;
using MyStik.TimeTable.Data;
using MyStik.TimeTable.DataServices;
using MyStik.TimeTable.Web.Models;
using MyStik.TimeTable.Web.Services;

namespace MyStik.TimeTable.Web.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public class StudentsController : BaseController
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            ViewBag.UserRight = GetUserRight();

            // Liste aller Fachschaften
            var org = GetMyOrganisation();

            ViewBag.MyOrganisation = org;
            ViewBag.Unions = Db.Organisers.Where(o => o.IsStudent).ToList();

            var model = new List<StudentStatisticsModel>();
            var list = Db.Students.GroupBy(x => new {x.Curriculum, x.FirstSemester});

            foreach (var ding in list)
            {
                model.Add(new StudentStatisticsModel
                {
                    Curriculum = ding.Key.Curriculum,
                    Semester = ding.Key.FirstSemester,
                    Count = ding.Count()
                });
            }

            return View(model);
        }


        public ActionResult StartGroups()
        {
            ViewBag.UserRight = GetUserRight();

            // Liste aller Fachschaften
            var org = GetMyOrganisation();

            ViewBag.MyOrganisation = org;

            var model = new List<StudentStatisticsModel>();
            var list = Db.Students.GroupBy(x => new { x.Curriculum, x.FirstSemester });

            foreach (var ding in list)
            {
                model.Add(new StudentStatisticsModel
                {
                    Curriculum = ding.Key.Curriculum,
                    Semester = ding.Key.FirstSemester,
                    Count = ding.Count()
                });
            }

            return View(model);
        }

        public ActionResult SemesterGroups()
        {
            return View();
        }


        [HttpPost]
        public PartialViewResult Search(string searchString)
        {
            var semester = SemesterService.GetSemester(DateTime.Today);
            var vorSemester = new SemesterService().GetSemester(semester, 1);

            var model = new List<StudentViewModel>();

            if (!string.IsNullOrEmpty(searchString))
            {
                var userDb = new ApplicationDbContext();
                var users = from s in userDb.Users select s;
                if (!string.IsNullOrEmpty(searchString))
                {
                    users = users.Where(u =>
                        u.MemberState == MemberState.Student &&
                        (u.FirstName.ToUpper().Contains(searchString.ToUpper()) ||
                         u.LastName.ToUpper().Contains(searchString.ToUpper()))
                    );
                }


                var semSubService = new SemesterSubscriptionService();

                foreach (var user in users)
                {
                    var studModel = new StudentViewModel
                    {
                        User = user,
                        Student = Db.Students.Where(x => x.UserId.Equals(user.Id)).OrderByDescending(x => x.Created).FirstOrDefault()
                    };


                    // alle Kurse des Benutzers
                    var courses =
                        Db.Activities.OfType<Course>()
                            .Where(c => c.Occurrence.Subscriptions.Any(s => s.UserId.Equals(user.Id)) &&
                                        c.SemesterGroups.Any((g => g.Semester.Id == semester.Id)))
                            .OrderBy(c => c.Name)
                            .ToList();

                    studModel.CurrentCourses = courses;

                    var lastCourses =
                        Db.Activities.OfType<Course>()
                            .Where(c => c.Occurrence.Subscriptions.Any(s => s.UserId.Equals(user.Id)) &&
                                        c.SemesterGroups.Any((g => g.Semester.Id == vorSemester.Id)))
                            .OrderBy(c => c.Name)
                            .ToList();

                    studModel.LastCourses = lastCourses;


                    model.Add(studModel);
                }
            }

            model = model.OrderBy(u => u.User.LastName).ToList();

            ViewBag.CurrentSemester = semester;
            ViewBag.LastSemester = vorSemester;

            return PartialView("_Profile", model);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Invitation()
        {
            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Invitation(InvitationFileModel model)
        {
            InvitationCheckModel invitationList = new InvitationCheckModel();
            //var sem = SemesterService.GetSemester(DateTime.Today);

            try
            {

            foreach (var attachment in model.Attachments)
            {

                if (attachment != null)
                {
                    var bytes = new byte[attachment.ContentLength];
                    attachment.InputStream.Read(bytes, 0, attachment.ContentLength);

                    var stream = new System.IO.MemoryStream(bytes);
                    var reader = new System.IO.StreamReader(stream, Encoding.Default);
                    var text = reader.ReadToEnd();

                    string[] lines = text.Split('\n');


                    var i = 0;
                    foreach (var line in lines)
                    {
                        if (i > 0)
                        {
                            string newline = line.Trim();

                            if (!string.IsNullOrEmpty(newline))
                            {
                                string[] words = newline.Split(';');

                                var invitation = new StudentInvitationModel
                                {
                                    LastName = words[0].Trim(),
                                    FirstName = words[1].Trim(),
                                    Email = words[2].Trim(),
                                    Organiser = words[3].Trim(),
                                    Curriculum = words[4].Trim(),
                                    Semester = words[5].Trim(),
                                    Invite = true
                                };


                                var user = UserManager.FindByEmail(invitation.Email);
                                if (user != null)
                                {
                                    invitation.Invite = false;
                                    invitation.Remark = "Hat bereits ein Benutzerkonto";
                                }

                                var sem = SemesterService.GetSemester(invitation.Semester);
                                if (sem == null)
                                {
                                    invitation.Invite = false;
                                    invitation.Remark += "Semester unbekannt";
                                }

                                var org = Db.Organisers.SingleOrDefault(x =>
                                    x.ShortName.Equals(invitation.Organiser));

                                if (org == null)
                                {
                                    invitation.Invite = false;
                                    invitation.Remark += "Veranstalter unbekannt";
                                }
                                else
                                {
                                    var curr = org.Curricula.SingleOrDefault(c => c.ShortName.Equals(invitation.Curriculum));
                                    if (curr == null)
                                    {
                                        invitation.Invite = false;
                                        invitation.Remark += "Studiengang unbekannt";
                                    }
                                }

                                invitationList.Invitations.Add(invitation);
                                }
                            }
                        i++;
                    }
                }
            }
                Session["InvitationList"] = invitationList;
            }
            catch (Exception ex)
            {
                invitationList.Error = ex.Message;
            }


            return View("InvitationList", invitationList);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult SendInvitations()
        {
            ViewBag.Languages = new []
            {
                new SelectListItem() {Text = "deutsch", Value = "de"},
                new SelectListItem() {Text = "englisch", Value = "en"}
            };
            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SendInvitations(OccurrenceMailingModel model)
        {
            var host = AppUser;

            var invitationList = Session["InvitationList"] as InvitationCheckModel;

            var semSubService = new SemesterSubscriptionService();

            // Keine Liste
            // Vermutung, die Session ist abgelaufen
            if (invitationList == null)
                return View("SendInvitationsError");

            var attachmentList = new List<CustomMailAttachtmentModel>();
            foreach (var attachment in model.Attachments)
            {
                if (attachment != null)
                {
                    var bytes = new byte[attachment.ContentLength];
                    attachment.InputStream.Read(bytes, 0, attachment.ContentLength);

                    attachmentList.Add(new CustomMailAttachtmentModel
                    {
                        FileName = Path.GetFileName(attachment.FileName),
                        Bytes = bytes,
                    });
                }
            }




            foreach (var invitation in invitationList.Invitations.Where(i => i.Invite))
            {
                invitation.Invited = false;

                var now = DateTime.Now;
                var user = new ApplicationUser
                {
                    UserName = invitation.Email, 
                    Email = invitation.Email,
                    FirstName = invitation.FirstName,
                    LastName = invitation.LastName,
                    Registered = now,
                    MemberState = MemberState.Student,
                    Remark = "Einladung von " + host.FullName,
                    ExpiryDate = null,              // Einladung bleibt dauerhaft bestehen - Deprovisionierung automatisch
                    Submitted = now,
                    EmailConfirmed = true, // damit ist auch ein "ForgotPassword" möglich, auch wenn er die Einladung nicht angenommen hat.
                    IsApproved = true, // Damit bekommt der Nutzer von Anfang an E-Mails
                    Faculty = host.Id               // Benutzer der eingeladen hat
                };

                // Benutzer anlegen, mit Dummy Passwort

                
                //string pswd = Membership.GeneratePassword(10, 2);

                var result = UserManager.Create(user, "Pas1234?");
                if (result.Succeeded)
                {

                    // analog Forget E-Mail Versand
                    string code = UserManager.GeneratePasswordResetToken(user.Id);

                    var mailModel = new ForgotPasswordMailModel
                    {
                        User = user,
                        Token = code,
                        CustomSubject = model.Subject,
                        CustomBody = model.Body,
                        Attachments = attachmentList,
                        IsNewAccount = true,
                    };
                        

                    try
                    {
                        new MailController().InvitationMail(mailModel, host, model.TemplateLanguage).Deliver();

                        // Student anlegen
                        var student = Db.Students.FirstOrDefault(x => x.UserId.Equals(user.Id));

                        if (student == null)
                        {
                            var sem = SemesterService.GetSemester(invitation.Semester);

                            var org = Db.Organisers.SingleOrDefault(x =>
                                x.ShortName.Equals(invitation.Organiser));

                            var curr = org.Curricula.SingleOrDefault(c => c.ShortName.Equals(invitation.Curriculum));


                            student = new Student
                            {
                                Created = DateTime.Now,
                                Curriculum = curr,
                                FirstSemester = sem,
                                UserId = user.Id
                            };

                            Db.Students.Add(student);
                            Db.SaveChanges();
                        }

                        //semSubService.Subscribe(user.Id, invitation.SemGroup.Id);

                        invitation.Invited = true;
                    }
                    catch (SmtpFailedRecipientException ex)
                    {
                        invitation.Remark = ex.Message;
                    }
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        invitation.Remark += error;
                    }
                }


            }


            var deliveryMailModel = new GenericMailDeliveryModel
            {
                Subject = model.Subject,
                Receiver = host,
                TemplateContent = new UserMailModel
                {
                    CustomBody = model.Body,
                }
            };


            // Mail an Einladenden versenden
            var ms = new MemoryStream();
            var writer = new StreamWriter(ms, Encoding.Default);


            writer.Write(
                "Name;Vorname;E-Mail;Versand;Bemerkung");

            writer.Write(Environment.NewLine);

            foreach (var delivery in invitationList.Invitations)
            {
                writer.Write("{0};{1};{2};{3};{4}",
                    delivery.LastName, delivery.FirstName, delivery.Email,
                    (delivery.Invite && delivery.Invited) ? "OK" : "FEHLER",
                    delivery.Remark);
                writer.Write(Environment.NewLine);
            }

            writer.Flush();
            writer.Dispose();

            var sb = new StringBuilder();
            sb.Append("Versandbericht");
            sb.Append(".csv");

            deliveryMailModel.Attachments.Add(new CustomMailAttachtmentModel
            {
                FileName = sb.ToString(),
                Bytes = ms.GetBuffer()
            });

            try
            {
                new MailController().GenericMail(deliveryMailModel).Deliver();
            }
            catch (Exception ex)
            {
                var logger = LogManager.GetLogger("SendMail");
                logger.ErrorFormat("Mailsent failed: {0}", ex.Message);
            }

            return View("SendInvitationsSuccess", invitationList);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult CoursePlan(string id)
        {
            var model = new UserCoursePlanViewModel();
            var user = UserManager.FindById(id);

            model.User = user;
            model.Student = Db.Students.Where(x => x.UserId.Equals(user.Id)).OrderByDescending(x => x.Created)
                .FirstOrDefault();

            var semester = SemesterService.GetSemester(DateTime.Today);
            var org = GetMyOrganisation();

            var nextSemester = SemesterService.GetNextSemester(semester);
            var hasGroups = Db.SemesterGroups.Any(x =>
                x.Semester.Id == nextSemester.Id && x.CapacityGroup.CurriculumGroup.Curriculum.Organiser.Id == org.Id);
            if (hasGroups)
                semester = nextSemester;


            model.Semester = semester;

            var courses =
                Db.Activities.OfType<Course>()
                .Where(c => c.Occurrence.Subscriptions.Any(s => s.UserId.Equals(id)) && c.SemesterGroups.Any((g => g.Semester.Id == semester.Id)))
                .OrderBy(c => c.Name)
                .ToList();

            foreach (var course in courses)
            {
                var summary = new CourseSummaryModel();

                summary.Course = course;

                var lectures =
                    Db.Members.Where(l => l.Dates.Any(occ => occ.Activity.Id == course.Id)).ToList();
                summary.Lecturers.AddRange(lectures);

                var rooms =
                    Db.Rooms.Where(l => l.Dates.Any(occ => occ.Activity.Id == course.Id)).ToList();
                summary.Rooms.AddRange(rooms);


                var days = (from occ in course.Dates
                            select
                                new
                                {
                                    Day = occ.Begin.DayOfWeek,
                                    Begin = occ.Begin.TimeOfDay,
                                    End = occ.End.TimeOfDay,
                                }).Distinct();

                foreach (var day in days)
                {
                    var defaultDay = course.Dates.FirstOrDefault(d => d.Begin.DayOfWeek == day.Day);

                    var courseDate = new CourseDateModel
                    {
                        DayOfWeek = day.Day,
                        StartTime = day.Begin,
                        EndTime = day.End,
                        DefaultDate = defaultDay.Begin
                    };
                    summary.Dates.Add(courseDate);
                }

                summary.Lottery =
                    Db.Lotteries.FirstOrDefault(x => x.Occurrences.Any(y => y.Id == course.Occurrence.Id));


                var subscriptions = course.Occurrence.Subscriptions.Where(s => s.UserId.Equals(id));
                foreach (var subscription in subscriptions)
                {
                    model.CourseSubscriptions.Add(new UserCourseSubscriptionViewModel
                    {
                        CourseSummary = summary,
                        Subscription = subscription
                    });
                }

                // jetzt die Tage rausholen und anfügen
                // nur für die mit Platz
                var sub = course.Occurrence.Subscriptions.FirstOrDefault(s => s.UserId.Equals(id));
                if (sub.OnWaitingList == false)
                {

                    foreach (var courseDate in course.Dates)
                    {
                        var dayPlan = model.CourseDates.SingleOrDefault(x => x.Day == courseDate.Begin.Date);
                        if (dayPlan == null)
                        {
                            dayPlan = new UserCourseDatePlanModel {Day = courseDate.Begin.Date};
                            model.CourseDates.Add(dayPlan);
                        }

                        dayPlan.Dates.Add(courseDate);
                    }
                }
            }

            ViewBag.UserRight = GetUserRight();


            return View(model);
        }

        public ActionResult Remove(Guid id)
        {
            var member = Db.Members.SingleOrDefault(x => x.Id == id);

            Db.Members.Remove(member);
            Db.SaveChanges();

            return RedirectToAction("Index");
        }

        public ActionResult ChangeCurriculum(Guid id)
        {
            var student = Db.Students.SingleOrDefault(x => x.Id == id);
            var user = UserManager.FindById(student.UserId);
            var org = GetMyOrganisation();

            var model = new CurriculumSubscriptionViewModel();
            model.User = user;
            model.Student = student;
            model.CurrId = student.Curriculum.Id;
            model.SemId = student.FirstSemester.Id;
            model.IsDual = student.IsDual;
            model.IsPartTime = student.IsPartTime;

            ViewBag.Curricula = org.Curricula.OrderBy(f => f.ShortName).Select(f => new SelectListItem
            {
                Text = f.Name,
                Value = f.Id.ToString(),
            });

            ViewBag.Semesters = Db.Semesters.Where(x => x.StartCourses <= DateTime.Today).OrderBy(x => x.StartCourses)
                .Select(f => new SelectListItem
                    {
                        Text = f.Name,
                        Value = f.Id.ToString(),
                    }
                );

            return View(model);
        }

        [HttpPost]
        public ActionResult ChangeCurriculum(CurriculumSubscriptionViewModel model)
        {
            var student = Db.Students.SingleOrDefault(x => x.Id == model.Student.Id);
            var user = UserManager.FindById(student.UserId);

            var curr = Db.Curricula.SingleOrDefault(x => x.Id == model.CurrId);
            var semester = Db.Semesters.SingleOrDefault(x => x.Id == model.SemId);

            student.Curriculum = curr;
            student.FirstSemester = semester;
            student.IsDual = model.IsDual;
            student.IsPartTime = model.IsPartTime;

            Db.SaveChanges();

            return RedirectToAction("CoursePlan", new {id = user.Id});
        }

        public ActionResult ChangeNumber(Guid id)
        {
            var student = Db.Students.SingleOrDefault(x => x.Id == id);
            var user = UserManager.FindById(student.UserId);

            var model = new CurriculumSubscriptionViewModel();
            model.User = user;
            model.Student = student;
            model.Number = student.Number;

            return View(model);
        }

        [HttpPost]
        public ActionResult ChangeNumber(CurriculumSubscriptionViewModel model)
        {
            var student = Db.Students.SingleOrDefault(x => x.Id == model.Student.Id);
            var user = UserManager.FindById(student.UserId);

            student.Number = model.Number;

            Db.SaveChanges();

            return RedirectToAction("CoursePlan", new { id = user.Id });
        }

        public ActionResult DeleteStudent(Guid id)
        {
            var student = Db.Students.SingleOrDefault(x => x.Id == id);

            if (student != null)
            {
                Db.Students.Remove(student);

                /*
                Db.Entry(student).State = EntityState.Modified;

                student.HasCompleted = true;
                */


                Db.SaveChanges();
            }


            var user = UserManager.FindById(student.UserId);

            return RedirectToAction("CoursePlan", new {id=user.Id});
        }


        /*

        /// <summary>
        /// Generates a Random Password
        /// respecting the given strength requirements.
        /// </summary>
        /// <param name="opts">A valid PasswordOptions object
        /// containing the password strength requirements.</param>
        /// <returns>A random password</returns>
        public static string GenerateRandomPassword(PasswordOptions opts = null)
        {
            if (opts == null) opts = new PasswordOptions()
            {
                RequiredLength = 8,
                RequiredUniqueChars = 4,
                RequireDigit = true,
                RequireLowercase = true,
                RequireNonAlphanumeric = true,
                RequireUppercase = true
            };

            string[] randomChars = new[] {
            "ABCDEFGHJKLMNOPQRSTUVWXYZ",    // uppercase 
            "abcdefghijkmnopqrstuvwxyz",    // lowercase
            "0123456789",                   // digits
            "!@$?_-"                        // non-alphanumeric
        };

            Random rand = new Random(Environment.TickCount);
            List<char> chars = new List<char>();

            if (opts.RequireUppercase)
                chars.Insert(rand.Next(0, chars.Count),
                    randomChars[0][rand.Next(0, randomChars[0].Length)]);

            if (opts.RequireLowercase)
                chars.Insert(rand.Next(0, chars.Count),
                    randomChars[1][rand.Next(0, randomChars[1].Length)]);

            if (opts.RequireDigit)
                chars.Insert(rand.Next(0, chars.Count),
                    randomChars[2][rand.Next(0, randomChars[2].Length)]);

            if (opts.RequireNonAlphanumeric)
                chars.Insert(rand.Next(0, chars.Count),
                    randomChars[3][rand.Next(0, randomChars[3].Length)]);

            for (int i = chars.Count; i < opts.RequiredLength
                || chars.Distinct().Count() < opts.RequiredUniqueChars; i++)
            {
                string rcs = randomChars[rand.Next(0, randomChars.Length)];
                chars.Insert(rand.Next(0, chars.Count),
                    rcs[rand.Next(0, rcs.Length)]);
            }

            return new string(chars.ToArray());
        }
        */
    }
}