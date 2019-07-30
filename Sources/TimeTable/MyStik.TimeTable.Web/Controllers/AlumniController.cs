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

namespace MyStik.TimeTable.Web.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public class AlumniController : BaseController
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            var member = GetMyMembership();
            if (member == null)
                return View("_NoAccess");

            var alumni = Db.Alumnae.Where(x => x.Curriculum.Organiser.Id == member.Organiser.Id);

            var model = new List<AlumniViewModel>();

            foreach (var alumnus in alumni)
            {
                var user = UserManager.FindById(alumnus.UserId);

                var m = new AlumniViewModel
                {
                    Alumni = alumnus,
                    Student = null,
                    User = user
                };

                model.Add(m);
            }


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
                                        Semester = words[4],
                                        Invite = true,
                                    };


                                    var user = UserManager.FindByEmail(invitation.Email);
                                    if (user != null)
                                    {
                                        invitation.User = user;
                                        invitation.Remark = "Hat bereits ein Benutzerkonto";
                                    }

                                    var curr = Db.Curricula.SingleOrDefault(c => c.ShortName.Equals(invitation.Curriculum.Trim()));
                                    if (curr == null)
                                    {
                                        invitation.Invite = false;
                                        invitation.Remark += "Studiengang unbekannt";
                                    }

                                    var sem = Db.Semesters.SingleOrDefault(s => s.Name.Equals(invitation.Semester));
                                    if (sem == null)
                                    {
                                        invitation.Invite = false;
                                        invitation.Remark += "Semester unbekannt";
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

        /*
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult SendInvitations()
        {
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
                var newUser = false;
                if (invitation.User == null)
                {
                    var user = new ApplicationUser
                    {
                        UserName = invitation.Email,
                        Email = invitation.Email,
                        FirstName = invitation.FirstName,
                        LastName = invitation.LastName,
                        Registered = now,
                        MemberState = MemberState.Guest,
                        Remark = "Einladung von " + host.FullName,
                        ExpiryDate = null, // Einladung bleibt dauerhaft bestehen - Deprovisionierung automatisch
                        Submitted = now,
                        EmailConfirmed = true, // damit ist auch ein "ForgotPassword" möglich, auch wenn er die Einladung nicht angenommen hat.
                        IsApproved = true, // Damit bekommt der Nutzer von Anfang an E-Mails
                        Faculty = host.Id // Benutzer der eingeladen hat
                    };

                    // Benutzer anlegen, mit Dummy Passwort
                    var result = UserManager.Create(user, "Pas1234?");
                    if (result.Succeeded)
                    {
                        invitation.User = user;
                        newUser = true;
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            invitation.Remark += error;
                        }
                    }
                }

                if (invitation.User != null)
                {
                    // Eintrag in Alumni Tabelle - nur falls es ihn noch nicht gibt
                    var curr = Db.Curricula.SingleOrDefault(c => c.ShortName.Equals(invitation.Curriculum.Trim()));
                    var sem = Db.Semesters.SingleOrDefault(s => s.Name.Equals(invitation.Semester));
                    if (!Db.Alumnae.Any(a => a.Curriculum.Id == curr.Id &&
                        a.Semester.Id == sem.Id &&
                        a.UserId.Equals(invitation.User.Id)))
                    {
                        var alumnus = new Alumnus
                        {
                            UserId = invitation.User.Id,
                            Curriculum = curr,
                            Semester = sem
                        };
                        Db.Alumnae.Add(alumnus);
                        Db.SaveChanges();
                    }

                    // nur neue Benutzer bekommen eine E-Mail
                    // analog Forget E-Mail Versand
                    string code = UserManager.GeneratePasswordResetToken(invitation.User.Id);

                    var mailModel = new ForgotPasswordMailModel
                    {
                        User = invitation.User,
                        Token = code,
                        CustomSubject = model.Subject,
                        CustomBody = model.Body,
                        Attachments = attachmentList,
                        IsNewAccount = newUser
                    };


                    try
                    {
                        new MailController().InvitationMail(mailModel, host, "de").Deliver();
                        invitation.Invited = true;
                    }
                    catch (SmtpFailedRecipientException ex)
                    {
                        invitation.Remark = ex.Message;
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
        */


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

    }
}