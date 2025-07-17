using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using MyStik.TimeTable.Data;
using MyStik.TimeTable.Web.Models;
using MyStik.TimeTable.Web.Services;

namespace MyStik.TimeTable.Web.Controllers
{
    public class CieController : BaseController
    {
        public ActionResult Index(Guid id)
        {
            var semester = SemesterService.GetSemester(id);

            var ciebGroups = Db.SemesterGroups.Where(x => x.Semester.Id == semester.Id && x.CapacityGroup.CurriculumGroup.Curriculum.ShortName.Equals("CIE-B")).ToList();
            var ciemGroups = Db.SemesterGroups.Where(x => x.Semester.Id == semester.Id && x.CapacityGroup.CurriculumGroup.Curriculum.ShortName.Equals("CIE-M")).ToList();

            var allCieGroups = new List<SemesterGroup>();
            allCieGroups.AddRange(ciebGroups);
            allCieGroups.AddRange(ciemGroups);


            var model = new CieSemesterModel
            {
                Semester = semester,
            };

            var courseSummaryService = new CourseService(Db);

            foreach (var cieGroup in allCieGroups)
            {
                foreach (var course in cieGroup.Activities.OfType<Course>())
                {
                    var cie = model.Courses.SingleOrDefault(x => x.Course.Course.Id == course.Id);

                    if (cie == null)
                    {
                        cie = new CieCourseModel
                        {
                            Course = courseSummaryService.GetCourseSummary(course),
                        };
                        model.Courses.Add(cie);
                    }

                    cie.CieGroups.Add(cieGroup);
                }
            }

            return View(model);
        }


        // GET: Cie
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
            var invitationList = CreateCheckModel(model);

            return View("InvitationList", invitationList);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Import(InvitationFileModel model)
        {
            var invitationList = CreateCheckModel(model);
            var host = GetCurrentUser();

            var studentService = new StudentService(Db);

            foreach (var invitation in invitationList.Invitations.Where(x => x.Semester != null && x.Curriculum != null && x.Course != null))
            {
                var user = UserManager.FindByEmail(invitation.Email);

                if (user == null)
                {
                    var now = DateTime.Now;
                    user = new ApplicationUser
                    {
                        UserName = invitation.Email,
                        Email = invitation.Email,
                        FirstName = invitation.FirstName,
                        LastName = invitation.LastName,
                        Registered = now,
                        MemberState = MemberState.Student,
                        Remark = "CIE",
                        ExpiryDate = null, // Einladung bleibt dauerhaft bestehen - Deprovisionierung automatisch
                        Submitted = now,
                        EmailConfirmed =
                            true, // damit ist auch ein "ForgotPassword" möglich, auch wenn er die Einladung nicht angenommen hat.
                        IsApproved = true, // Damit bekommt der Nutzer von Anfang an E-Mails
                        Faculty = host.Id // Benutzer der eingeladen hat
                    };

                    // Benutzer anlegen, mit Dummy Passwort
                    var result = UserManager.Create(user, "Cie98#lcl?");

                    if (result.Succeeded)
                    {

                        // analog Forget E-Mail Versand
                        string code = UserManager.GeneratePasswordResetToken(user.Id);

                        var mailModel = new ForgotPasswordMailModel
                        {
                            User = user,
                            Token = code,
                            CustomSubject = "Your NINE Account",
                            CustomBody = "",
                            Attachments = null,
                            IsNewAccount = true,
                        };


                        try
                        {
                            new MailController().InvitationMail(mailModel, host, "en").Deliver();
                        }
                        catch (SmtpFailedRecipientException ex)
                        {
                            invitation.Remark = ex.Message;
                        }
                    }
                }

                var students = studentService.GetCurrentStudent(user);
                if (!students.Any())
                {
                    var student = new Student
                    {
                        Created = DateTime.Now,
                        Curriculum = invitation.Curriculum,
                        FirstSemester = invitation.Semester,
                        UserId = user.Id
                    };

                    Db.Students.Add(student);
                }


                if (invitation.Course != null)
                {
                    var subscription =
                        invitation.Course.Occurrence.Subscriptions.FirstOrDefault(x => x.UserId.Equals(user.Id));

                    if (subscription == null)
                    {
                        subscription = new OccurrenceSubscription
                        {
                            TimeStamp = DateTime.Now,
                            UserId = user.Id,
                            OnWaitingList = invitation.OnWaitinglist,
                            Occurrence = invitation.Course.Occurrence,
                            HostRemark = invitation.Remark
                        };
                        invitation.Course.Occurrence.Subscriptions.Add(subscription);
                    }
                }
            }

            Db.SaveChanges();

            return View("InvitationList", invitationList);
        }

        private CieInvitationCheckModel CreateCheckModel(InvitationFileModel model)
        {
            var invitationList = new CieInvitationCheckModel();

            var attachment = model.Attachments.FirstOrDefault();

            if (attachment == null)
            {
                invitationList.Error = "Keine Datei";
                return invitationList;
            }


            try
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
                    if (i <= 1) continue;
                    var newline = line.Trim();

                    if (!string.IsNullOrEmpty(newline))
                    {
                        var words = newline.Split(';');

                        if (!string.IsNullOrEmpty(words[0]))
                        {

                            var invitation = new CieInvitationModel
                            {
                                LastName = words[0].Trim(),
                                FirstName = words[1].Trim(),
                                Email = words[2].Trim(),
                                CurrName = words[3].Trim(),
                                CourseName = words[4].Trim(),
                                SemesterName = words[5].Trim(),
                                StateName = words[6].Trim(),
                                Invite = true,
                            };

                            invitation.Curriculum = GetCieCurriculum(invitation.CurrName);
                            invitation.Semester = SemesterService.GetSemester(invitation.SemesterName);
                            if (invitation.Curriculum != null && invitation.Semester != null)
                            {
                                invitation.Course = GetCieCourse(invitation.Semester, invitation.Curriculum.Organiser, invitation.CourseName);
                            }

                            if (invitation.StateName.Equals("TN"))
                            {
                                invitation.OnWaitinglist = false;
                            }
                            else
                            {
                                invitation.OnWaitinglist = true;
                            }

                            invitationList.Invitations.Add(invitation);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                invitationList.Error = ex.Message;
            }

            return invitationList;
        }



        private Course GetCieCourse(Semester semester, ActivityOrganiser org, string code)
        {
            if (semester == null || org == null)
                return null;

            return 
            Db.Activities.OfType<Course>().FirstOrDefault(x =>
                    x.Semester.Id == semester.Id &&
                    x.Organiser.Id == org.Id &&
                    x.ShortName.Equals(code));
        }

        private Curriculum GetCieCurriculum(string code)
        {
            // FK01
            if (code.StartsWith("AR-B")) return GetCurriculum("BAAR");
            if (code.StartsWith("AR-M")) return GetCurriculum("MAAR");

            // FK01
            if (code.StartsWith("BI-B")) return GetCurriculum("BIB");
            if (code.StartsWith("BI-M")) return GetCurriculum("BIM");

            // FK03
            if (code.StartsWith("FA-B")) return GetCurriculum("FAB");
            if (code.StartsWith("MB-B")) return GetCurriculum("MBB");
            if (code.StartsWith("LR-B")) return GetCurriculum("LRB");
            if (code.StartsWith("MB-M")) return GetCurriculum("MBM");

            /*
            code.StartsWith("RS") ||
                code.StartsWith("FE") ||
                code.StartsWith("TB"))
            */

            // FK04
            if (code.StartsWith("EI-B")) return GetCurriculum("BAET");
            if (code.StartsWith("EI-M")) return GetCurriculum("MAET");

            /*
            if (code.StartsWith("EI") ||
                code.StartsWith("RE") ||
                code.StartsWith("EM") ||
                code.StartsWith("SM") ||
                code.StartsWith("EL") ||
                code.StartsWith("EE"))
                return GetOrganiser("FK 04");
            */

            // FK05

            /*
            if (code.StartsWith("VS") ||
                code.StartsWith("VV") ||
                code.StartsWith("DR") ||
                code.StartsWith("TN") ||
                code.StartsWith("PK") ||
                code.StartsWith("PW") ||
                code.StartsWith("GT") ||
                code.StartsWith("PR") ||
                code.StartsWith("VF") ||
                code.StartsWith("DP") ||
                code.StartsWith("TK"))
                return GetOrganiser("FK 05");
            */

            // FK06

            /*
            if (code.StartsWith("AO") ||
                code.StartsWith("CT") ||
                code.StartsWith("PH") ||
                code.StartsWith("MF") ||
                code.StartsWith("MT") ||
                code.StartsWith("BO") ||
                code.StartsWith("PN") ||
                code.StartsWith("PA") ||
                code.StartsWith("MN") ||
                code.StartsWith("PO") ||
                code.StartsWith("BT"))
                return GetOrganiser("FK 06");
            */

            // FK07
            if (code.StartsWith("IF-B")) return GetCurriculum("BAINF");
            if (code.StartsWith("IF-M")) return GetCurriculum("MAINF");

            /*
            if (code.StartsWith("IF") ||
                code.StartsWith("IC") ||
                code.StartsWith("IB") ||
                code.StartsWith("IS") ||
                code.StartsWith("IG") ||
                code.StartsWith("IN"))
                return GetOrganiser("FK 07");
            */

            // FK08

            /*
            if (code.StartsWith("GN") ||
                code.StartsWith("GD") ||
                code.StartsWith("KG") ||
                code.StartsWith("GO"))
                return GetOrganiser("FK 08");
            */

            // FK09
            if (code.StartsWith("AU-B")) return GetCurriculum("AU");
            if (code.StartsWith("LM-B")) return GetCurriculum("LM");
            if (code.StartsWith("WI-B")) return GetCurriculum("WI");
            if (code.StartsWith("WI-M")) return GetCurriculum("WIM");

            /*
            if (code.StartsWith("LM") ||
                code.StartsWith("AU") ||
                code.StartsWith("WI") ||
                code.StartsWith("WW"))
                return GetOrganiser("FK 09");
            */

            // FK10
            if (code.StartsWith("BW-B")) return GetCurriculum("BABW");
            if (code.StartsWith("BW-M")) return GetCurriculum("MABW");

            /*
            if (code.StartsWith("BW") ||
                code.StartsWith("UB") ||
                code.StartsWith("BB") ||
                code.StartsWith("BE") ||
                code.StartsWith("BS"))
                return GetOrganiser("FK 10");
            */

            // FK11
            if (code.StartsWith("SA-M")) return GetCurriculum("SAFD");

            /*
            if (code.StartsWith("SW") ||
                code.StartsWith("SR") ||
                code.StartsWith("SI") ||
                code.StartsWith("SK") ||
                code.StartsWith("PF") ||
                code.StartsWith("SB") ||
                code.StartsWith("SO") ||
                code.StartsWith("SY") ||
                code.StartsWith("SF") ||
                code.StartsWith("GW") ||
                code.StartsWith("SD") ||
                code.StartsWith("GE") ||
                code.StartsWith("PY") ||
                code.StartsWith("SE"))
                return GetOrganiser("FK 11");
            */

            // FK12
            if (code.StartsWith("DS-B")) return GetCurriculum("DES-B");
            if (code.StartsWith("DS-M")) return GetCurriculum("DES-M");

            /*
            if (code.StartsWith("DS"))
                return GetOrganiser("FK 12");
            */

            // FK13

            /*
            if (code.StartsWith("IK") ||
                code.StartsWith("PI") ||
                code.StartsWith("IK") ||
                code.StartsWith("PZ"))
                return GetOrganiser("FK 13");
            */

            // FK14
            if (code.StartsWith("TR-B")) return GetCurriculum("BATM");
            if (code.StartsWith("TS-M")) return GetCurriculum("MATM");
            if (code.StartsWith("TH-M")) return GetCurriculum("MAHM");

            /*
            if (code.StartsWith("TR") ||
                code.StartsWith("TH"))
                return GetOrganiser("FK 14");
            */

            return null;
        }

        private Curriculum GetCurriculum(string code)
        {
            return Db.Curricula.FirstOrDefault(x => x.ShortName.StartsWith(code));
        }

    }
}