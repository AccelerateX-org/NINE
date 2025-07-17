using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Web.Mvc;
using log4net;
using Microsoft.Ajax.Utilities;


//using System.Web.Security;
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
            var user = GetCurrentUser();

            var members = GetMyMemberships();

            if (id != null)
            {
                var member = members.FirstOrDefault(x => x.Organiser.Id == id.Value);
                if (member == null)
                {
                    return View("_NoAccess");
                }

                ViewBag.Organiser = member.Organiser;
                ViewBag.UserRight = GetUserRight(member.Organiser);
                return View(new List<StudentStatisticsModel>());
            }

            var adminMember = members.FirstOrDefault(x => x.IsStudentAdmin);
            var isAdmin = adminMember != null;

            ViewBag.IsStudAdmin = isAdmin;

            if (!isAdmin)
                return View("_NoAccess");

            ViewBag.Organiser = adminMember.Organiser;
            ViewBag.UserRight = GetUserRight(adminMember.Organiser);

            var model = new List<StudentStatisticsModel>();
            return View(model);
        }


        public ActionResult Labels(Guid? orgId, Guid? currId, Guid? labelId)
        {
            var user = GetCurrentUser();

            var members = GetMyMemberships();
            ActivityOrganiser org = null;

            if (orgId != null)
            {
                var member = members.FirstOrDefault(x => x.Organiser.Id == orgId.Value);
                if (member == null)
                {
                    return View("_NoAccess");
                }
                org = member.Organiser;
            }
            else
            {
                var adminMember = members.FirstOrDefault(x => x.IsStudentAdmin);
                var isAdmin = adminMember != null;

                ViewBag.IsStudAdmin = isAdmin;

                if (!isAdmin)
                    return View("_NoAccess");

                org = adminMember.Organiser;
            }


            var semester = SemesterService.GetSemester(DateTime.Today);

            var curr = currId.HasValue ?
                Db.Curricula.SingleOrDefault(x => x.Id == currId.Value) :
                Db.Curricula.Where(x => x.Organiser.Id == org.Id && !x.IsDeprecated).OrderBy(x => x.ShortName).FirstOrDefault();


            var label = labelId.HasValue ?
                Db.ItemLabels.SingleOrDefault(x => x.Id == labelId.Value) :
                curr.LabelSet.ItemLabels.FirstOrDefault();


            var model = new SemesterActiveViewModel
            {
                Curriculum = curr,
                Semester = semester,
                Organiser = org,
                Label = label,
                Courses = new List<CourseSummaryModel>()
            };

            ViewBag.UserRight = GetUserRight(org);
            ViewBag.Organisers = Db.Organisers.Where(x => x.Id == org.Id).Select(c => new SelectListItem
            {
                Text = c.ShortName,
                Value = c.Id.ToString(),
            });
            ViewBag.Semesters = Db.Semesters.Where(x => x.Id == semester.Id).Select(c => new SelectListItem
            {
                Text = c.Name,
                Value = c.Id.ToString(),
            });
            ViewBag.Curricula = Db.Curricula.Where(x => x.Organiser.Id == org.Id && !x.IsDeprecated).OrderBy(x => x.ShortName).Select(c => new SelectListItem
            {
                Text = c.ShortName,
                Value = c.Id.ToString(),
            });
            ViewBag.Labels = curr.LabelSet.ItemLabels.OrderBy(x => x.Name).Select(c => new SelectListItem
            {
                Text = c.Name,
                Value = c.Id.ToString(),
            });


            return View(model);
        }


        public ActionResult StartGroups(Guid? id)
        {

            var org = id == null ? GetMyOrganisation(): GetOrganiser(id.Value);

            ViewBag.UserRight = GetUserRight(org);


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
            var curr = Db.Curricula.Include(curriculum => curriculum.Organiser).SingleOrDefault(x => x.Id == id);
            var userRight = GetUserRight(curr.Organiser);

            if (!userRight.Member.IsStudentAdmin)
                return null;

            // nur aktive, d.h. noch kein Abschlussemester
            var students = Db.Students.Where(x => x.Curriculum.Id == curr.Id).Include(student => student.FirstSemester).ToList();


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
                var studentService = new StudentService(Db);

                foreach (var user in users)
                {
                    var student = studentService.GetCurrentStudent(user.Id).FirstOrDefault();
                    /*
                    var student = Db.Students.Where(x => x.UserId.Equals(user.Id)).OrderByDescending(x => x.Created)
                        .FirstOrDefault();
                    */

                    if (student == null) continue;

                    var studModel = new StudentViewModel
                    {
                        User = user,
                        Student = student
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



        public ActionResult UploadLabels()
        {
            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult UploadLabels(InvitationFileModel model)
        {
            InvitationCheckModel invitationList = new InvitationCheckModel();
            //var sem = SemesterService.GetSemester(DateTime.Today);
            var studentService = new StudentService(Db);

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
                            i++;
                            if (i == 1)  continue;

                            var newline = line.Trim();
                            if (string.IsNullOrEmpty(newline)) continue;

                            var words = newline.Split(';');

                            var invitation = new StudentInvitationModel
                            {
                                Email = words[0].Trim(),
                                LabelLevel = words[1].Trim(), // Ebene
                                LabelName = words[2].Trim(), // Label
                                Invite = true
                            };
                            invitationList.Invitations.Add(invitation);

                            // nur bekannte Benutzer
                            var user = UserManager.FindByEmail(invitation.Email);
                            if (user == null)
                            {
                                invitation.Invite = false;
                                invitation.Remark = "Kein Account gefunden";
                                continue;
                            }

                            var student = studentService.GetCurrentStudent(user.Id).FirstOrDefault();
                            if (student == null)
                            {
                                invitation.Invite = false;
                                invitation.Remark = "Kein Studium gefunden";
                                continue;
                            }

                            invitation.Student = student;

                            if (student.LabelSet == null)
                            {
                                var labelSet2 = new ItemLabelSet();
                                Db.ItemLabelSets.Add(labelSet2);
                                student.LabelSet = labelSet2;
                            }

                            if (string.IsNullOrEmpty(invitation.LabelLevel) ||
                                string.IsNullOrEmpty(invitation.LabelName))
                            {
                                invitation.Invite = false;
                                invitation.Remark = "Fehlende Angabe zu Ebene oder Kohorte";
                                continue;
                            }

                            ItemLabelSet labelSet = null;
                            var organiser = student.Curriculum.Organiser;
                            var levelId = invitation.LabelLevel;
                            if (string.IsNullOrEmpty(levelId))
                            {
                                invitation.Invite = false;
                                invitation.Remark = "Fehlende Angabe zur Kohorte";
                                continue;
                            }

                            if (organiser.Institution != null && levelId.ToUpper().Equals(organiser.Institution.Tag.ToUpper()))
                            {
                                labelSet = organiser.Institution.LabelSet;
                            }
                            else if (levelId.ToUpper().Equals(organiser.ShortName.ToUpper()))
                            {
                                labelSet = organiser.LabelSet;
                            }
                            else
                            {
                                var curr = organiser.Curricula.FirstOrDefault(x => x.ShortName.ToUpper().Equals(levelId));
                                if (curr != null)
                                {
                                    labelSet = curr.LabelSet;
                                }
                            }


                            // wenn kein Labelset identifiziert werden kann, dann wird das Label auch nicht importiert
                            // Label suchen, aber nicht anlegen
                            if (labelSet == null)
                            {
                                invitation.Invite = false;
                                invitation.Remark = "Zuordnung der Kohorte nicht gefunden";
                                continue;
                            }

                            var label = labelSet.ItemLabels.FirstOrDefault(x => x.Name.Equals(invitation.Label));
                            if (label == null)
                            {
                                invitation.Invite = false;
                                invitation.Remark = "Kohorte nicht vorhanden";
                                continue;
                            }

                            // erst wenn alles passt, dann hinzufügen
                            student.LabelSet.ItemLabels.Add(label);
                        }
                    }
                }

                Db.SaveChanges();
            }
            catch (Exception ex)
            {
                invitationList.Error = ex.Message;
            }

            return View("UploadList", invitationList);
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
        public ActionResult Details(Guid id)
        {
            var model = new StudentDetailViewModel();

            var student = Db.Students.Include(student1 => student1.LabelSet).Include(student2 =>
                student2.Curriculum.Organiser).SingleOrDefault(x => x.Id == id);

            if (student == null)
                return RedirectToAction("Index");

            if (student.LabelSet == null)
            {
                var labelSet = new ItemLabelSet();
                Db.ItemLabelSets.Add(labelSet);
                student.LabelSet = labelSet;
                Db.SaveChanges();
            }

            var user = UserManager.FindById(student.UserId);

            model.User = user;
            model.Students = Db.Students.Where(x => x.UserId.Equals(user.Id)).OrderByDescending(x => x.Created).Include(x =>
                x.Curriculum.Organiser).ToList();
            model.Student = model.Students.FirstOrDefault();

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

            var org = student.Curriculum.Organiser;
            ViewBag.UserRight = GetUserRight(org);

            var members = GetMyMemberships();
            var adminMember = members.FirstOrDefault(x => x.IsStudentAdmin);
            ViewBag.IsStudAdmin = adminMember != null;

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
            var org = student.Curriculum.Organiser;

            var model = new CurriculumSubscriptionViewModel();
            model.User = user;
            model.Student = student;
            model.CurrId = student.Curriculum.Id;
            model.SemId = student.FirstSemester.Id;
            model.IsDual = student.IsDual;
            model.IsPartTime = student.IsPartTime;

            ViewBag.Curricula = org.Curricula.Where(x => x.IsPublished).OrderBy(f => f.ShortName).Select(f => new SelectListItem
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
            var students = Db.Students.Where(x => x.Curriculum.Id == curr.Id).ToList();

            var model = new StudentsByCurriculumViewModel();

            model.Curriculum = curr;
            model.Students = students;


            return View(model);
        }


        public ActionResult SwitchToFullTime(Guid id)
        {
            var student = Db.Students.Include(student1 => student1.Curriculum).SingleOrDefault(x => x.Id == id);

            if (student != null)
            {
                student.IsPartTime = false;
                Db.SaveChanges();

                return RedirectToAction("Admin", new { id = student.Curriculum.Id });
            }

            return RedirectToAction("Index");
        }

        public ActionResult SwitchToPartTime(Guid id)
        {
            var student = Db.Students.Include(student1 => student1.Curriculum).SingleOrDefault(x => x.Id == id);

            if (student != null)
            {
                student.IsPartTime = true;
                Db.SaveChanges();

                return RedirectToAction("Admin", new { id = student.Curriculum.Id });
            }

            return RedirectToAction("Index");
        }


        public ActionResult ProlongCurriculum(Guid id)
        {
            var student = Db.Students.Include(student1 => student1.LastSemester).SingleOrDefault(x => x.Id == id);

            if (student != null)
            {
                var sem = student.LastSemester;
                student.LastSemester = null;
                student.HasCompleted = false;
                Db.SaveChanges();
                return RedirectToAction("Details", new { id = student.Id });
            }

            return RedirectToAction("Index");
        }


        public ActionResult ChangeLabel(Guid id)
        {
            var student = Db.Students.Include(student1 => student1.LastSemester).SingleOrDefault(x => x.Id == id);

            if (student == null)
                return RedirectToAction("Index");

            var model = new StudentDetailViewModel();

            var user = UserManager.FindById(student.UserId);

            model.User = user;
            model.Students = Db.Students.Where(x => x.UserId.Equals(user.Id)).OrderByDescending(x => x.Created).Include(x =>
                x.Curriculum.Organiser).ToList();
            model.Student = model.Students.FirstOrDefault();

            var org = student.Curriculum.Organiser;
            ViewBag.UserRight = GetUserRight(org);

            var members = GetMyMemberships();
            var adminMember = members.FirstOrDefault(x => x.IsStudentAdmin);
            ViewBag.IsStudAdmin = adminMember != null;


            return View(model);
        }


        public ActionResult EditLabel(Guid studentId, Guid labelId)
        {
            var student = Db.Students.Include(student1 => student1.LabelSet.ItemLabels).SingleOrDefault(c => c.Id == studentId);
            var label = student.LabelSet.ItemLabels.FirstOrDefault(x => x.Id == labelId);

            ViewBag.Student = student;

            return View(label);
        }


        public ActionResult ChangeLabels(Guid id)
        {
            var student = Db.Students.SingleOrDefault(c => c.Id == id);
            var user = UserManager.FindById(student.UserId);

            var model = new CourseLabelViewModel()
            {
                Student = student,
                User = user,
                Organisers = Db.Organisers.Where(x => x.LabelSet != null && !x.IsStudent && x.Curricula.Any()).OrderBy(x => x.ShortName).ToList()
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult AssignCourseLabels(Guid studentId, Guid[] labelIds)
        {
            var student = Db.Students.Include(x => x.LabelSet.ItemLabels).SingleOrDefault(c => c.Id == studentId);

            foreach (var labelId in labelIds)
            {
                var label = Db.ItemLabels.SingleOrDefault(x => x.Id == labelId);

                if (label != null && !student.LabelSet.ItemLabels.Contains(label))
                {
                    student.LabelSet.ItemLabels.Add(label);
                }
            }

            Db.SaveChanges();

            return null;
        }

        public ActionResult RemoveLabel(Guid studentId, Guid labelId)
        {
            var student = Db.Students.SingleOrDefault(c => c.Id == studentId);
            var label = student.LabelSet.ItemLabels.FirstOrDefault(x => x.Id == labelId);

            if (label != null)
            {
                student.LabelSet.ItemLabels.Remove(label);
                Db.SaveChanges();
            }

            return RedirectToAction("Details", new { id = student.Id });
        }

        public PartialViewResult GetStudentList(Guid semId, Guid currId, Guid labelId)
        {
            var semester = SemesterService.GetSemester(semId);
            var curr = Db.Curricula.SingleOrDefault(x => x.Id == currId);
            var label = Db.ItemLabels.SingleOrDefault(x => x.Id == labelId);

            if (semester == null || curr == null || label == null)
            {
                return PartialView("_StudentList", new List<StudentViewModel>());
            }

            var userInfoService = new UserInfoService();

            // Alle aktiven Students mit diesem Curriculum
            var students = Db.Students
                .Where(x => x.Curriculum.Id == currId && x.LastSemester == null)
                .ToList();

            // Alle Kurse aus dem Semester, die das Label haben
            var courses = Db.Activities.OfType<Course>()
                .Where(c => c.Semester.Id == semId && c.LabelSet != null && c.LabelSet.ItemLabels.Any(l => l.Id == label.Id)).Include(activity =>
                    activity.Occurrence.Subscriptions)
                .ToList();

            var model = new List<StudentViewModel>();

            foreach (var student in students)
            {
                var user = userInfoService.GetUser(student.UserId);
                if (user == null)
                    continue;

                // Prüfen, in welchen Kursen des Semesters der Student eingeschrieben ist
                var courseList = courses.Where(c => c.Occurrence.Subscriptions.Any(s => s.UserId == user.Id)).ToList();

                if (!courseList.Any()) continue;

                var hasLabel = student.LabelSet != null && student.LabelSet.ItemLabels.Any(l => l.Id == labelId);

                var studModel = new StudentViewModel
                {
                    User = user,
                    Student = student,
                    CurrentCourses = courseList,
                    HasLabel = hasLabel,
                };

                model.Add(studModel);
            }



            return PartialView("_StudentsList", model);
        }
    }
}