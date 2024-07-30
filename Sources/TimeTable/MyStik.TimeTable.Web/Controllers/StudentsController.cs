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
using MyStik.TimeTable.Web.Areas.Admin.Models;
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
        public ActionResult Index(Guid? id)
        {
            ViewBag.UserRight = GetUserRight();

            // Liste aller Fachschaften
            var org = id == null ?  GetMyOrganisation() : GetOrganiser(id.Value);

            ViewBag.MyOrganisation = org;


            var model = new List<StudentStatisticsModel>();

            return View(model);
        }


        public ActionResult StartGroups(Guid? id)
        {
            ViewBag.UserRight = GetUserRight();

            var org = id == null ? GetMyOrganisation(): GetOrganiser(id.Value);


            // Liste aller Studiengänge
            var currs = Db.Curricula.Where(x => x.Organiser.Id == org.Id).ToList();

            var semesterToday = SemesterService.GetSemester(DateTime.Today);
            var nextSemester = SemesterService.GetNextSemester(semesterToday);

            var isActive = SemesterService.IsActive(nextSemester);

            var startSemester = semesterToday;
            if (isActive)
            {
                startSemester = nextSemester;
            }



            var model = new StudentsOrgViewModel
            {
                Organiser = org
            };


            model.Semesters.Add(startSemester);
            for (var i = 0; i <= 6; i++)
            {
                var prevSemester = SemesterService.GetPreviousSemester(startSemester);
                model.Semesters.Add(prevSemester);
                startSemester = prevSemester;
            }



            foreach (var curr in currs)
            {
                var cModel = new StudentsByCurriculumViewModel()
                {
                    Curriculum = curr,
                    Students = Db.Students.Where(x => x.Curriculum.Id == curr.Id).ToList(),
                    Alumnae = Db.Alumnae.Where(x => x.Curriculum.Id == curr.Id).ToList()
                };

                model.StudentsByCurriculum.Add(cModel);
            }

            return View(model);
        }

        public FileResult List(Guid id)
        {
            var curr = Db.Curricula.SingleOrDefault(x => x.Id == id);
            var userRight = GetUserRight(curr.Organiser);

            if (!userRight.Member.IsStudentAdmin)
                return null;

            // nur aktive, d.h. noch kein Abschlussemester
            var students = Db.Students.Where(x => x.Curriculum.Id == curr.Id && x.LastSemester == null).ToList();


            var ms = new MemoryStream();
            var writer = new StreamWriter(ms, Encoding.Default);

            writer.Write(
                "Name;Vorname;E-Mail;Studienbeginn;Letztes Login");

            writer.Write(Environment.NewLine);

            foreach (var student in students)
            {
                var user = UserManager.FindById(student.UserId);

                if (user != null)
                {
                    // Altfälle oder sonstige Ausnahme
                    // Studiengang für gleichen user
                    // mit neuerem Beginn
                    // anders als bisheriges
                    var nextStudiesCount = Db.Students.Count(x =>
                        x.UserId.Equals(user.Id) &&
                        x.Curriculum.Id != student.Curriculum.Id &&
                        x.FirstSemester.StartCourses > student.FirstSemester.StartCourses);

                    if (nextStudiesCount == 0)
                    {
                        if (user.LastLogin.HasValue)
                        {
                            writer.Write("{0};{1};{2};{3};{4}",
                                user.LastName, user.FirstName, user.Email,
                                student.FirstSemester.Name,
                                user.LastLogin.Value.ToShortDateString());
                        }
                        else
                        {
                            writer.Write("{0};{1};{2};{3};n.a.",
                                user.LastName, user.FirstName, user.Email,
                                student.FirstSemester.Name
                                );
                        }

                        writer.Write(Environment.NewLine);
                    }
                }
            }

            writer.Flush();
            writer.Dispose();

            var sb = new StringBuilder();
            sb.Append("Studierende_");
            sb.Append(curr.ShortName);
            sb.Append("_");
            sb.Append(DateTime.Today.ToString("yyyyMMdd"));
            sb.Append(".csv");

            return File(ms.GetBuffer(), "text/csv", sb.ToString());
        }

        public ActionResult CourseList(Guid currId, Guid semId)
        {
            var curr = Db.Curricula.SingleOrDefault(x => x.Id == currId);
            var sem = SemesterService.GetSemester(semId);

            var students = Db.Students.Where(x => x.Curriculum.Id == currId && x.FirstSemester.Id == semId).ToList();


            ViewBag.Curriculum = curr;
            ViewBag.Semester = sem;

            return View(students);
        }


        public ActionResult SemesterGroups()
        {






            return View();
        }


        [HttpPost]
        public PartialViewResult Search(string searchString)
        {
            var semester = SemesterService.GetSemester(DateTime.Today);

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

                    model.Add(studModel);
                }
            }

            model = model.OrderBy(u => u.User.LastName).ToList();

            ViewBag.CurrentSemester = semester;

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


            var deliveryMailModel = new UserMailModel
            {
                CustomSubject = model.Subject,
                User = host,
                CustomBody = model.Body,
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
                new MailController().GenericMessageMail(deliveryMailModel).Deliver();
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
        public ActionResult Details(string id)
        {
            var model = new StudentDetailViewModel();
            var user = UserManager.FindById(id);

            model.User = user;
            model.Students = Db.Students.Where(x => x.UserId.Equals(user.Id)).OrderByDescending(x => x.Created).Include(student =>
                student.Curriculum.Organiser).ToList();
            model.Student = model.Students.FirstOrDefault();

            var org = model.Student.Curriculum.Organiser;

            var allCourses = Db.Activities.OfType<Course>()
                .Where(x => x.Occurrence.Subscriptions.Any(s => s.UserId.Equals(user.Id)))
                .Include(activity => activity.Semester).ToList();

            foreach (var course in allCourses.Where(x => x.Semester != null).ToList())
            {
                var semModel = model.Semester.FirstOrDefault(x => x.Semester.Id == course.Semester.Id);
                if (semModel == null)
                {
                    semModel = new StudentSemesterViewModel
                    {
                        Semester = course.Semester
                    };

                    model.Semester.Add(semModel);
                }

                if (!semModel.Courses.Contains(course))
                {
                    semModel.Courses.Add(course);
                }
            }

            ViewBag.UserRight = GetUserRight(org);

            return View(model);
        }


        /// <summary>
            /// 
            /// </summary>
            /// <param name="id"></param>
            /// <returns></returns>
        public ActionResult CoursePlan(string id, Guid semId)
        {
            var model = new UserCoursePlanViewModel();
            var user = UserManager.FindById(id);

            model.User = user;
            model.Student = Db.Students.Where(x => x.UserId.Equals(user.Id)).OrderByDescending(x => x.Created)
                .FirstOrDefault();

            var semester = SemesterService.GetSemester(semId);
            var org = model.Student.Curriculum.Organiser;

            model.Semester = semester;

            var courses =
                Db.Activities.OfType<Course>()
                .Where(c => c.Occurrence.Subscriptions.Any(s => s.UserId.Equals(id)) && c.Semester.Id == semester.Id)
                .OrderBy(c => c.Name)
                .ToList();

            var courseService = new CourseService(Db);

            foreach (var course in courses)
            {
                var summary = courseService.GetCourseSummary(course);

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

            ViewBag.UserRight = GetUserRight(org);


            return View(model);
        }


        public ActionResult Curriculum(Guid id)
        {
            var model = new StudentDetailViewModel();

            var student = Db.Students.SingleOrDefault(x => x.Id == id);
            var user = UserManager.FindById(student.UserId);

            model.Student = student;
            model.User = user;
            model.Theses =  Db.Theses.Where(x => x.Student.Id == student.Id).ToList();

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

            var futureDate = DateTime.Today.AddDays(180);

            ViewBag.Semesters = Db.Semesters.Where(x => x.StartCourses <= futureDate).OrderBy(x => x.StartCourses)
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

            return RedirectToAction("Details", new {id = user.Id});
        }


        public ActionResult SwitchCurriculum(Guid id)
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

            var futureDate = DateTime.Today.AddDays(180);

            ViewBag.Semesters = Db.Semesters.Where(x => x.StartCourses <= futureDate).OrderBy(x => x.StartCourses)
                .Select(f => new SelectListItem
                    {
                        Text = f.Name,
                        Value = f.Id.ToString(),
                    }
                );

            return View(model);
        }

        [HttpPost]
        public ActionResult SwitchCurriculum(CurriculumSubscriptionViewModel model)
        {
            var student = Db.Students.SingleOrDefault(x => x.Id == model.Student.Id);
            var user = UserManager.FindById(student.UserId);

            var curr = Db.Curricula.SingleOrDefault(x => x.Id == model.CurrId);
            var semester = Db.Semesters.SingleOrDefault(x => x.Id == model.SemId);

            // Das ist jetzt ein neuer Studiengang

            // Zuerst den alten Studiengang beenden
            var lastSemester = SemesterService.GetPreviousSemester(semester);
            student.LastSemester = lastSemester;


            var studentNew = new Student();

            studentNew.UserId = user.Id;
            studentNew.Created = DateTime.Now;
            studentNew.Curriculum = curr;
            studentNew.FirstSemester = semester;
            studentNew.IsDual = model.IsDual;
            studentNew.IsPartTime = model.IsPartTime;

            Db.Students.Add(studentNew);

            Db.SaveChanges();

            return RedirectToAction("Details", new { id = user.Id });
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

            return RedirectToAction("Index");
        }

        public ActionResult Logs(Guid id)
        {
            var student = Db.Students.SingleOrDefault(x => x.Id == id);
            
            var db = new LogDbContext();

            List<Log> logs = null;

            if (student != null)
            {
                var user = GetUser(student.UserId);
                logs = db.Log.Where(l => l.Thread.Equals(user.Email)).ToList();
            }

            return View(logs);
        }

        public ActionResult Subscribe(Guid id)
        {
            var student = Db.Students.SingleOrDefault(x => x.Id == id);
            var user = GetUser(student.UserId);
            var sem = SemesterService.GetSemester(DateTime.Today);

            var model = new StudentSubscriptionModel
            {
                OrgName = student.Curriculum.Organiser.ShortName,
                SemesterName = sem.Name
            };

            model.Student = student;
            model.User = user;


            return View(model);
        }


        [HttpPost]
        public ActionResult Subscribe(StudentSubscriptionModel model)
        {
            var org = Db.Organisers.SingleOrDefault(x => x.ShortName.Equals(model.OrgName.Trim()));
            if (org == null)
            {
                ModelState.AddModelError("OrgName", "Es existiert keine Einrichtung / Fakultät mit dieser Bezeichnung");
                return View(model);
            }


            var semester = SemesterService.GetSemester(model.SemesterName.Trim());
            if (semester == null)
            {
                ModelState.AddModelError("SemesterName", "Es existiert kein Semester mit dieser Bezeichnung");
                return View(model);
            }


            var courses = Db.Activities.OfType<Course>().Where(x =>
                x.ShortName.Equals(model.CourseShortName.Trim()) &&
                x.Organiser.Id == org.Id &&
                x.Semester.Id == semester.Id).ToList();

            if (!courses.Any())
            {
                ModelState.AddModelError("CourseShortName", "Es existiert keine Lehrveranstaltung mit dieser Bezeichnung");
                return View(model);
            }

            if (courses.Count > 1)
            {
                ModelState.AddModelError("CourseShortName", $"Bezeichnung nicht eindeutig. Es existieren {courses.Count} Lehrveranstaltungen mit dieser Bezeichnung");
                return View(model);
            }

            var host = GetCurrentUser();
            var course = courses.First();

            var subscription = course.Occurrence.Subscriptions.FirstOrDefault(x => x.UserId.Equals(model.User.Id));

            if (subscription != null)
            {
                ModelState.AddModelError("CourseShortName", "Ist bereits in dieser Lehrveranstaltung eingetragen.");
                return View(model);
            }

            subscription = new OccurrenceSubscription();
            subscription.TimeStamp = DateTime.Now;
            subscription.UserId = model.User.Id;
            subscription.OnWaitingList = false;
            subscription.Occurrence = course.Occurrence;
            course.Occurrence.Subscriptions.Add(subscription);

            // wenn es ein Wahlverfahren gibt, dann als Prio 1
            var lottery =
                Db.Lotteries.FirstOrDefault(x => x.Occurrences.Any(y => y.Id == course.Occurrence.Id));

            if (lottery != null)
            {
                subscription.Priority = 1;

                var game = lottery.Games.FirstOrDefault(x => x.UserId.Equals(model.User.Id));
                if (game == null)
                {
                    game = new LotteryGame
                    {
                        Lottery = lottery,
                        UserId = subscription.UserId,
                        AcceptDefault = false,
                        CoursesWanted = lottery.MaxConfirm,
                        Created = DateTime.Now,
                        LastChange = DateTime.Now
                    };
                    lottery.Games.Add(game);
                }
            }

            Db.SaveChanges();

            // Bei Erfolg Mail versenden
            var mailService = new SubscriptionMailService();
            mailService.SendSubscriptionEMail(course, subscription, host);

           


            return RedirectToAction("CoursePlan", new {id = model.User.Id, semId = semester.Id });
        }



        [HttpPost]
        public ActionResult Unsubscribe(StudentSubscriptionModel model)
        {
            var org = GetMyOrganisation();
            var semester = SemesterService.GetSemester(model.SemesterName);

            if (semester == null)
            {
                ModelState.AddModelError("SemesterName", "Es existiert kein Semester mit dieser Bezeichnung");
                return View(model);
            }


            var courses = Db.Activities.OfType<Course>().Where(x =>
                x.ShortName.Equals(model.CourseShortName.Trim()) &&
                x.Organiser.Id == org.Id &&
                x.Semester.Id == semester.Id).ToList();

            if (!courses.Any())
            {
                ModelState.AddModelError("CourseShortName", "Es existiert keine Lehrveranstaltung mit dieser Bezeichnung");
                return View(model);
            }

            if (courses.Count > 1)
            {
                ModelState.AddModelError("CourseShortName", $"Bezeichnung nicht eindeutig. Es existieren {courses.Count} Lehrveranstaltungen mit dieser Bezeichnung");
                return View(model);
            }

            var host = GetCurrentUser();
            var course = courses.First();

            var subscription = course.Occurrence.Subscriptions.FirstOrDefault(x => x.UserId.Equals(model.User.Id));

            if (subscription == null)
            {
                ModelState.AddModelError("CourseShortName", "Ist in dieser Lehrveranstaltung nicht eingetragen.");
                return View(model);
            }

            var subService = new SubscriptionService(Db);
            subService.DeleteSubscription(subscription);

            var mailService = new SubscriptionMailService();
            mailService.SendSubscriptionEMail(course, model.User.Id, host);

            return RedirectToAction("CoursePlan", new { id = model.User.Id, semId=semester.Id });
        }





        public ActionResult Unsubscribe(Guid id)
        {
            var student = Db.Students.SingleOrDefault(x => x.Id == id);
            var user = GetUser(student.UserId);

            var model = new StudentSubscriptionModel();

            model.Student = student;
            model.User = user;


            return View(model);
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


        public ActionResult Admin(Guid id)
        {
            var curr = Db.Curricula.SingleOrDefault(x => x.Id == id);
            
            // nur aktive, d.h. noch kein Abschlussemester
            var students = Db.Students.Where(x => x.Curriculum.Id == curr.Id && x.LastSemester == null).ToList();

            var model = new StudentsByCurriculumViewModel();

            model.Curriculum = curr;
            model.Students = students;


            return View(model);
        }


        public ActionResult SwitchToFullTime(Guid id)
        {
            var student = Db.Students.SingleOrDefault(x => x.Id == id);


            student.IsPartTime = false;
            Db.SaveChanges();

            return RedirectToAction("Admin", new {id = student.Curriculum.Id});
        }

        public ActionResult SwitchToPartTime(Guid id)
        {
            var student = Db.Students.SingleOrDefault(x => x.Id == id);


            student.IsPartTime = true;
            Db.SaveChanges();

            return RedirectToAction("Admin", new { id = student.Curriculum.Id });
        }

    }
}