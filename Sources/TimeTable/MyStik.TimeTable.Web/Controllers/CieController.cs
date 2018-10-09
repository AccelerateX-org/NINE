using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using MyStik.TimeTable.Data;
using MyStik.TimeTable.Web.Models;
using MyStik.TimeTable.Web.Services;
using PdfSharp.Pdf;

namespace MyStik.TimeTable.Web.Controllers
{
    public class CieController : BaseController
    {
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

            /*
            if (!string.IsNullOrEmpty(invitationList.Error))
                return View("InvitationList", invitationList);
            */


            foreach (var invitation in invitationList.Invitations)
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
                        EmailConfirmed = true, // damit ist auch ein "ForgotPassword" möglich, auch wenn er die Einladung nicht angenommen hat.
                        IsApproved = true, // Damit bekommt der Nutzer von Anfang an E-Mails
                        Faculty = host.Id // Benutzer der eingeladen hat
                    };

                    // Benutzer anlegen, mit Dummy Passwort
                    var result = UserManager.Create(user, "Cie98#lcl?");
                }

                var student = studentService.GetCurrentStudent(user);
                if (student == null)
                {
                    student = new Student();
                    student.Created = DateTime.Now;
                    student.Curriculum = invitation.Curriculum;
                    student.FirstSemester = invitation.Semester;
                    student.UserId = user.Id;

                    Db.Students.Add(student);
                }


                if (invitation.Course != null)
                {
                    var subscription =
                        invitation.Course.Occurrence.Subscriptions.FirstOrDefault(x => x.UserId.Equals(user.Id));

                    if (subscription == null)
                    {
                        subscription = new OccurrenceSubscription();
                        subscription.TimeStamp = DateTime.Now;
                        subscription.UserId = user.Id;
                        subscription.OnWaitingList = invitation.OnWaitinglist;
                        subscription.Priority = 0;
                        subscription.Occurrence = invitation.Course.Occurrence;
                        subscription.HostRemark = invitation.Remark;
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
                    if (i > 0)
                    {
                        string newline = line.Trim();

                        if (!string.IsNullOrEmpty(newline))
                        {
                            var words = newline.Split(';');

                            var invitation = new CieInvitationModel
                            {
                                LastName = words[0].Trim(),
                                FirstName = words[1].Trim(),
                                Email = words[2].Trim(),
                                Invite = true
                            };

                            invitation.Semester = SemesterService.GetSemester(words[6].Trim());
                            invitation.Curriculum = GetCieCurriculum(words[3].Trim());

                            var org = GetOrganiser(words[5].Trim());

                            invitation.Course = GetCieCourse(org, invitation.Semester,
                                words[4].Trim());

                            if (invitation.Course != null)
                            {
                                // TODO: Ermittlung des Status
                                var code = words[7].Trim();
                                var state = words[8].Trim();

                                // zuerst die roten
                                if (code.Equals("red"))
                                {
                                    // gehört der Incomer zu einem der Studiengänge der CIE-LV
                                    var hasFit = invitation.Course.SemesterGroups.Any(x =>
                                        x.CapacityGroup.CurriculumGroup.Curriculum.Id == invitation.Curriculum.Id);

                                    if (hasFit)
                                    {
                                        if (state.Equals("TN"))
                                        {
                                            invitation.OnWaitinglist = false;
                                            invitation.Remark = "Studiengang passt. Ist Teilnehmer";
                                        }
                                        else
                                        {
                                            invitation.OnWaitinglist = true;
                                            invitation.Remark =
                                                "Studiengang passt. Wurde in der Platzverlosung von primuss / FK 13 auf Warteliste gesetzt. Offenbar alle Plätze belegt.";
                                        }
                                    }
                                    else
                                    {
                                        if (state.Equals("TN"))
                                        {
                                            invitation.OnWaitinglist = true;
                                            invitation.Remark =
                                                "Studiengang passt nicht. Wurde in der Platzverlosung von primuss / FK 13 auf Teilnehmerliste gesetzt. Muss manuel! überprüft werden";
                                        }
                                        else
                                        {
                                            invitation.OnWaitinglist = true;
                                            invitation.Remark =
                                                "Studiengang passt nicht. Wurde in der Platzverlosung von primuss / FK 13 auf Warteliste gesetzt. OK!";
                                        }
                                    }
                                }
                                else if (code.Equals("yellow"))
                                {
                                    // yellow
                                    // gehört der Studi zur Fakultät
                                    var hasFit = invitation.Curriculum.Organiser.Id == org.Id;

                                    if (hasFit)
                                    {
                                        if (state.Equals("TN"))
                                        {
                                            invitation.OnWaitinglist = false;
                                            invitation.Remark = "Student gehört zur Fakultät. Ist Teilnehmer";
                                        }
                                        else
                                        {
                                            invitation.OnWaitinglist = true;
                                            invitation.Remark =
                                                "Student gehört zur Fakultät. Wurde von in der Platzverlosung von primuss / FK 13 auf Warteliste gesetzt. Offenbar alle Plätze belegt.";
                                        }
                                    }
                                    else
                                    {
                                        if (state.Equals("TN"))
                                        {
                                            invitation.OnWaitinglist = true;
                                            invitation.Remark =
                                                "Student gehört zu einer anderen Fakultät. Wurde in der Platzverlosung von primuss / FK 13 auf Teilnehmerliste gesetzt. Muss manuell überprüft werden!";
                                        }
                                        else
                                        {
                                            invitation.OnWaitinglist = true;
                                            invitation.Remark =
                                                "Student gehört zu einer anderen Fakultät. Wurde in der Platzverlosung von primuss / FK 13 auf Warteliste gesetzt. Offenbar alle Plätze belegt.";
                                        }
                                    }

                                }
                                else
                                {
                                    if (state.Equals("TN"))
                                    {
                                        invitation.OnWaitinglist = false;
                                        invitation.Remark = "Wurde in der Platzverlosung von primuss / FK 13 auf Teilnehmerliste gesetzt.";
                                    }
                                    else
                                    {
                                        invitation.OnWaitinglist = true;
                                        invitation.Remark = "Wurde in der Platzverlosung von primuss / FK 13 auf Warteliste gesetzt.";
                                    }

                                }
                            }
                            else
                            {
                                invitation.Remark = "LV nicht gefunden";
                                invitationList.Error = "Fehlehafte Daten";
                            }

                            invitationList.Invitations.Add(invitation);
                        }
                    }
                    i++;
                }
            }
            catch (Exception ex)
            {
                invitationList.Error = ex.Message;
            }

            return invitationList;
        }



        private Course GetCieCourse(ActivityOrganiser org, Semester semester, string code)
        {
            return 
            Db.Activities.OfType<Course>().FirstOrDefault(x =>
                x.SemesterGroups.Any(g =>
                    g.CapacityGroup.CurriculumGroup.Curriculum.Organiser.Id == org.Id && g.Semester.Id == semester.Id) &&
                x.ShortName.Contains(code));
        }

        private Curriculum GetCieCurriculum(string code)
        {
            var org = GetCieOrganiser(code);

            var curr = code.EndsWith("B") ? "CIE-B" : "CIE-M";

            return org.Curricula.FirstOrDefault(x => x.ShortName.Equals(curr));
        }


        private ActivityOrganiser GetCieOrganiser(string code)
        {
            if (code.StartsWith("AR"))
                return GetOrganiser("FK 01");

            if (code.StartsWith("BI"))
                return GetOrganiser("FK 02");

            if (code.StartsWith("FA") || 
                code.StartsWith("MB") || 
                code.StartsWith("LR") || 
                code.StartsWith("RS") ||
                code.StartsWith("FE") ||
                code.StartsWith("TB"))
                return GetOrganiser("FK 03");

            if (code.StartsWith("EI") ||
                code.StartsWith("RE") ||
                code.StartsWith("EM") ||
                code.StartsWith("SM") ||
                code.StartsWith("EL") ||
                code.StartsWith("EE"))
                return GetOrganiser("FK 04");

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

            if (code.StartsWith("IF") ||
                code.StartsWith("IC") ||
                code.StartsWith("IB") ||
                code.StartsWith("IS") ||
                code.StartsWith("IG") ||
                code.StartsWith("IN"))
                return GetOrganiser("FK 07");

            if (code.StartsWith("GN") ||
                code.StartsWith("GD") ||
                code.StartsWith("KG") ||
                code.StartsWith("GO"))
                return GetOrganiser("FK 08");

            if (code.StartsWith("LM") ||
                code.StartsWith("AU") ||
                code.StartsWith("WI") ||
                code.StartsWith("WW"))
                return GetOrganiser("FK 09");

            if (code.StartsWith("BW") ||
                code.StartsWith("UB") ||
                code.StartsWith("BB") ||
                code.StartsWith("BE") ||
                code.StartsWith("BS"))
                return GetOrganiser("FK 10");

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

            if (code.StartsWith("DS"))
                return GetOrganiser("FK 12");

            if (code.StartsWith("IK") ||
                code.StartsWith("PI") ||
                code.StartsWith("IK") ||
                code.StartsWith("PZ"))
                return GetOrganiser("FK 13");


            if (code.StartsWith("TR") ||
                code.StartsWith("TH"))
                return GetOrganiser("FK 14");


            return GetOrganiser("FK 13");
        }

    }
}