using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Web.Mvc;
using log4net;
using Microsoft.AspNet.Identity;
using MyStik.TimeTable.Data;
using MyStik.TimeTable.Web.Models;
using MyStik.TimeTable.Web.Services;
using RazorEngine.Compilation.ImpromptuInterface;

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
            var model = Db.Organisers.Where(o => o.IsStudent).ToList();

            ViewBag.MyOrganisation = GetMyOrganisation();

            return View(model);
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
            var sem = GetSemester();

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
                                    LastName = words[0],
                                    FirstName = words[1],
                                    Email = words[2],
                                    Curriculum = words[3],
                                    Group = words[4],
                                    Invite = true
                                };


                                var user = UserManager.FindByEmail(invitation.Email);
                                if (user != null)
                                {
                                    invitation.Invite = false;
                                    invitation.Remark = "Hat bereits ein Benutzerkonto";
                                }

                                var curr = Db.Curricula.SingleOrDefault(c => c.ShortName.Equals(invitation.Curriculum.Trim()));
                                if (curr == null)
                                {
                                    invitation.Remark += "Studiengang unbekannt";
                                }
                                else
                                {
                                    
                                    var groupList = Db.SemesterGroups.Where(g =>
                                            g.Semester.Id == sem.Id && g.CapacityGroup.CurriculumGroup.Curriculum.Id == curr.Id).ToList();

                                    foreach (var group in groupList)
                                    {
                                        if (group.GroupName.Trim().Equals(invitation.Group.Trim()))
                                        {
                                            invitation.SemGroup = group;
                                        }
                                    }

                                    if (invitation.SemGroup == null)
                                    {
                                        invitation.Invite = false;
                                        invitation.Remark += "Studiengruppe unbekannt";
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

                var now = GlobalSettings.Now;
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

                        // zur Semestergruppe zuordnen
                        semSubService.Subscribe(user.Id, invitation.SemGroup.Id);

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

            model.User = UserManager.FindById(id);

            var semester = GetSemester();
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

                summary.HasLottery = Db.Lotteries.Any(x => x.Occurrences.Any(y => y.Id == course.Occurrence.Id));


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

            return View(model);
        }


    }
}