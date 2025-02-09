﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Ical.Net.Collections;
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
    public class MailingController : BaseController
    {
        private ApplicationDbContext _userDb = new ApplicationDbContext();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            var org = GetMyOrganisation();
            ViewBag.Organiser = org;
            ViewBag.Semester = SemesterService.GetSemester(DateTime.Today);
            ViewBag.UserRight = GetUserRight();
            ViewBag.Curricula = Db.Curricula.Where(x => x.Organiser.Id == org.Id).ToList();

            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult CustomOccurrenceMail(Guid id)
        {
            var db = new TimeTableDbContext();

            var occ = db.Occurrences.SingleOrDefault(c => c.Id == id);

            var model = new OccurrenceMailingModel();

            if (occ != null)
            {
                var summary = new ActivityService().GetSummary(id);
                var subscribers = summary.Subscriptions;

                model.OccurrenceId = occ.Id;
                model.Name = summary.Name;
                model.Subject = $"[{summary.Name}]";
                model.ReceiverCount = subscribers.Count;
                model.Occurrence = occ;
                model.UseParticipients = true;
                model.UseWaitingList = false;
            }

            ViewBag.UserRight = GetUserRight();
            ViewBag.SystemMail = MailController.InitSystemFrom();

            return View(model);
        }

        /// <summary>
        /// Versand einer E-Mail an eine Teilnehmerliste
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult CustomOccurrenceMail(OccurrenceMailingModel model)
        {
            var logger = LogManager.GetLogger("OccurrenceMail");

            if (ModelState.IsValid)
            {
                var db = new TimeTableDbContext();

                var occ = db.Occurrences.SingleOrDefault(c => c.Id == model.OccurrenceId);

                if (occ != null)
                {

                    // Liste der Empfänger ermitteln
                    var ac = new ActivityService();
                    var summary = ac.GetSummary(model.OccurrenceId);

                    var subscribers = summary.Subscriptions;
                    var users = new List<ApplicationUser>();

                    if (model.UseParticipients)
                    {
                        foreach (var subscription in subscribers.Where(x => x.OnWaitingList == false))
                        {
                            var user = UserManager.FindById(subscription.UserId);
                            if (user != null)
                            {
                                users.Add(user);
                            }
                        }
                    }

                    if (model.UseWaitingList)
                    {
                        foreach (var subscription in subscribers.Where(x => x.OnWaitingList == true))
                        {
                            var user = UserManager.FindById(subscription.UserId);
                            if (user != null)
                            {
                                users.Add(user);
                            }
                        }
                    }


                    // Mail an Teilnehmer einer Veranstaltung
                    // durch selbstständiges Eintragen automatisch Erhalt von Mails gewährleisten
                    model.IsImportant = true;
                    model.IsDistributionList = false;
                    model.ListName = summary.Name;

                    SendMail(users, model);

                    return RedirectToAction("MailSentSuccess", "Mailing", new { id = model.OccurrenceId });

                }
                else
                {
                    logger.ErrorFormat("Occurrence {0} does not exist", model.OccurrenceId);
                    return RedirectToAction("MailSentError", "Mailing");
                }

            }
            else
            {

                ViewBag.ErrorMessage = "Fehler im Formular: " + ModelState.ToString();
                return View("InvalidModel");
            }

        }

        private MailJobModel GetMailModel(OccurrenceMailingModel model)
        {
            var m = new MailJobModel
            {
                SenderId = "",
                Subject = model.Subject,
                Body = model.Body // HttpUtility.HtmlEncode(model.Body)
            };

            var size = 0;
            var count = 0;
            foreach (var attachment in model.Attachments)
            {
                if (attachment != null)
                {
                    size += attachment.ContentLength;
                    count++;

                    var bytes = new byte[attachment.ContentLength];
                    attachment.InputStream.Read(bytes, 0, attachment.ContentLength);

                    m.Files.Add(new CustomMailAttachtmentModel
                    {
                        FileName = Path.GetFileName(attachment.FileName),
                        Bytes = bytes,
                    });
                }
            }

            return m;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult AllStudents(Guid id)
        {
            var org = GetOrganisation(id);

            ViewBag.UserRight = GetUserRight(org);
            ViewBag.Organiser = org;
            var model = new OccurrenceMailingModel
            {
                OrgId = org.Id,
            };
            return View(model);
        }

        /// <summary>
        /// E-Mail an alle Studierenden
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AllStudents(OccurrenceMailingModel model)
        {
            var logger = LogManager.GetLogger("AllStudentsMail");


            if (ModelState.IsValid)
            {
                var semester = SemesterService.GetSemester(DateTime.Today);
                var org = GetOrganisation(model.OrgId);


                // Alle Studierenden in Studiengängen des Veranstalters
                ICollection<ApplicationUser> userList = new List<ApplicationUser>();

                // nur die aktiven
                var allStudents = Db.Students.Where(x => 
                    x.Curriculum != null && 
                    x.Curriculum.Organiser != null && 
                    x.Curriculum.Organiser.Id == org.Id && 
                    x.LastSemester == null).ToList();

                // ggf. doppelt vorhandene rauswerfen
                var allUserIds = allStudents.Select(s => s.UserId).Distinct().ToList();

                // als Empfänger kommen nur existierende User in Frage
                foreach (var userId in allUserIds)
                {
                    ApplicationUser user = UserManager.FindById(userId);
                    if (user != null)
                    {
                        userList.Add(user);
                    }
                }


                model.ListName = "Alle Studierende";
                model.IsDistributionList = true;

                logger.InfoFormat("Active Students [{0}] - UserIds [{1}] - Available Users [{2}]", 
                    allStudents, allUserIds.Count, userList.Count);

                // Der SysAdmin kann nicht versenden - es wird nur geloggt!
                if (!User.IsInRole("SysAdmin"))
                {
                    SendMail(userList, model);
                }

                //ICollection<ApplicationUser> userList = new List<ApplicationUser>();
                return View("ReceiverList", userList);
            }
            else
            {
                return View();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult AllMembers()
        {
            var model = new OccurrenceMailingModel();
            return View(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AllMembers(OccurrenceMailingModel model)
        {
            var logger = LogManager.GetLogger("AllMember");
            var semester = SemesterService.GetSemester(DateTime.Today);

            if (ModelState.IsValid)
            {
                // Liste der Empfänger ermitteln
                // Alle Member rausfiltern
                var organiser = GetMyOrganisation();

                if (organiser != null)
                {

                    var memberList = new List<ApplicationUser>();

                    foreach (var member in organiser.Members)
                    {
                        if (!string.IsNullOrEmpty(member.UserId))
                        {
                            var user = _userDb.Users.SingleOrDefault(u => u.Id.Equals(member.UserId));
                            if (user != null)
                            {
                                memberList.Add(user);
                            }
                        }
                    }


                    logger.InfoFormat("Memberlist with {0} entires", memberList.Count);

                    if (!User.IsInRole("SysAdmin"))
                    {
                        SendMail(memberList, model);
                    }
                    return View("ReceiverList", memberList);
                }
            }

            return View();
        }



        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult StudentGroup(Guid id)
        {
            var org = GetOrganisation(id);

            var semesterList = SemesterService.GetActiveSemester(org);

            ViewBag.SemesterList = semesterList.OrderByDescending(s => s.StartCourses).Select(f => new SelectListItem
            {
                Text = f.Name,
                Value = f.Id.ToString()
            });


            ViewBag.UserRight = GetUserRight(org);

            var model = new SemesterViewModel
            {
                Semester = SemesterService.GetSemester(DateTime.Today),
                Organiser = org
            };

            return View(model);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="GroupIds"></param>
        /// <returns></returns>
        [HttpPost]
        public PartialViewResult StudentGroupSelect(Guid semId, Guid orgId, ICollection<Guid> GroupIds)
        {
            if (GroupIds != null)
            {
                var model = new OccurrenceMailingModel()
                {
                    SemId = semId,
                    OrgId = orgId
                };

                model.GroupIdList = new List<string>();
                foreach (var groupId in GroupIds)
                {
                    model.GroupIdList.Add(groupId.ToString());
                }

                ViewBag.UserRight = GetUserRight();

                return PartialView("_StudentGroupMail", model);
            }


            return PartialView("_GroupSelectionError");
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="semId"></param>
        /// <returns></returns>
        [HttpPost]
        public PartialViewResult GroupList(Guid semId, Guid orgId)
        {
            var semester = SemesterService.GetSemester(semId);
            var org = GetOrganisation(orgId);

            var model = new SemesterOverviewModel();
            model.Semester = semester;
            model.Organiser = org;
            model.Labels = new List<ItemLabel>();

            
            return PartialView("_StudentGroupSelect", model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult StudentGroup(OccurrenceMailingModel model)
        {
            var logger = LogManager.GetLogger("StudentGroup");

            var ids = model.GroupIdList.First().Split(',');



            var userList =  new List<ApplicationUser>();

            // nur noch alle, die in LVs der Gruppen eingetrangen sind
            var studentService = new StudentService(Db);
            
            // für den Namen der Liste
            var sb = new StringBuilder();

            List<string> userIds = new List<string>();
            foreach (var semGroupId in ids)
            {
                var id = Guid.Parse(semGroupId);

                var label = Db.ItemLabels.SingleOrDefault(x => x.Id == id);

                var courses = Db.Activities.OfType<Course>().Where(x => 
                    x.Organiser != null && x.Semester != null && x.LabelSet != null &&
                    x.Organiser.Id == model.OrgId && x.Semester.Id == model.SemId && x.LabelSet.ItemLabels.Any(l => l.Id == id)
                ).Include(activity => activity.Occurrence.Subscriptions).ToList();

                foreach (var course in courses)
                {
                    var subIds = course.Occurrence.Subscriptions.Select(x => x.UserId).ToList();
                    userIds.AddRange(subIds);
                }

                sb.Append(label.Name);
                sb.Append(", ");

                logger.InfoFormat("Students in group {0}: {1}", label.Name, userIds.Count);
            }
            
            
            logger.InfoFormat("Total entries {0}", userIds.Count);
            var userIdsNoDuplicates = userIds.Distinct().ToList();
            logger.InfoFormat("Total entries after reduction {0}", userIdsNoDuplicates.Count);

            foreach (var userId in userIdsNoDuplicates)
            {
                ApplicationUser user = UserManager.FindById(userId);
                if (user != null)
                {
                    userList.Add(user);
                }
            }

            model.ListName = sb.ToString();
            model.IsDistributionList = true;

            logger.InfoFormat("UserList with {0} entires", userList.Count);

            if (!User.IsInRole("SysAdmin"))
            {
                SendMail(userList, model);
            }

            return View("ReceiverList", userList);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult HostByRoom()
        {
            var model = new OccurrenceMailingModel();
            return View(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult HostByDate()
        {
            var model = new OccurrenceMailingModel();
            return View(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Guests()
        {
            var model = new OccurrenceMailingModel();
            return View(model);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="receiverList"></param>
        /// <param name="model"></param>
        /// <param name="pckSize"></param>
        public void SendMail(ICollection<ApplicationUser> receiverList, OccurrenceMailingModel model, int pckSize = 0)
        {
            var logger = LogManager.GetLogger("SendMail");
            logger.InfoFormat("Subject [{0}], Receiverlist has [{1}] entries", model.Subject, receiverList.Count);
            

            // Das Mail-Model aufbauen
            var mailModel = new UserMailModel
            {
                CustomSubject = model.Subject,
                SenderUser = UserManager.FindByName(User.Identity.Name),
                CustomBody = model.Body,
                IsImportant = model.IsImportant,
                ListName = model.ListName,
                IsDistributionList = model.IsDistributionList
            };

            var size = 0;
            var count = 0;
            foreach (var attachment in model.Attachments)
            {
                if (attachment != null)
                {
                    size += attachment.ContentLength;
                    count++;

                    var bytes = new byte[attachment.ContentLength];
                    attachment.InputStream.Read(bytes, 0, attachment.ContentLength);

                    mailModel.Attachments.Add(new CustomMailAttachtmentModel
                    {
                        FileName = Path.GetFileName(attachment.FileName),
                        Bytes = bytes,
                    });
                }
            }

            if (count > 0 && size > 0)
            {
                logger.InfoFormat("# Attachments [{0}], Size [{1:0.00}] MBytes", count, size / (double)(1024 * 1024));
            }


            // 1 Mail für jeden
            var deliveryModel = new MailDeliverSummaryReportModel();
            var errorCount = 0;

            foreach (var user in receiverList)
            {
                // Mails werden nur versendet, wenn die Mail Adresse bestätigt ist
                if (!user.EmailConfirmed)
                {
                    // In den Bericht aufnehmen
                    errorCount++;
                    deliveryModel.Deliveries.Add(new MailDeliveryReportModel
                    {
                        User = user,
                        DeliverySuccessful = false,
                        ErrorMessage = "Mailadresse nicht bestätigt. Grund: " + user.AccountErrorMessage
                    });
                }
                else
                {
                    // hier erst auf Like E-Mail überprüfen
                    // wenn die E-Mail unwichtig ist und der benutzer keine E-Mail erhalten möchte
                    // dann bekommt er auch keine - es wird aber im Versandprotokoll vermerkt
                    if (!model.IsImportant && !user.LikeEMails)
                    {
                        errorCount++;
                        deliveryModel.Deliveries.Add(new MailDeliveryReportModel
                        {
                            User = user,
                            DeliverySuccessful = false,
                            ErrorMessage = "Benutzer möchte keine E-Mails erhalten"
                        });
                        
                    }
                    else
                    {
                        mailModel.User = user;

                        // Versand versuchen
                        try
                        {
                            new MailController().GenericMessageMail(mailModel).Deliver();

                            deliveryModel.Deliveries.Add(new MailDeliveryReportModel
                            {
                                User = user,
                                DeliverySuccessful = true,
                            });
                        }
                        catch (Exception ex)
                        {
                            errorCount++;
                            var strError = string.Format("Fehler bei Versand. Grund: {0}. Mailadresse wird auf ungültig gesetzt.", ex.Message);

                            deliveryModel.Deliveries.Add(new MailDeliveryReportModel
                            {
                                User = user,
                                DeliverySuccessful = false,
                                ErrorMessage = strError
                            });

                            /*
                            user.EmailConfirmed = false;
                            // Ein Expiry ist nicht sinnvoll / möglich, da E-Mail Adresse ja ohnehin nicht erreichbar
                            user.Remark = strError;
                            user.Submitted = DateTime.Now;
                            UserManager.Update(user);
                             */
                        }
                    }
                }
            }

            // Kopie an Absender
            mailModel.User= mailModel.SenderUser;

            // Versandbericht nur an Staff
            if (mailModel.SenderUser.MemberState == MemberState.Staff)
            {
                var ms = new MemoryStream();
                var writer = new StreamWriter(ms, Encoding.Default);

                writer.Write(
                    "Name;Vorname;E-Mail;Versand;Bemerkung");

                writer.Write(Environment.NewLine);

                foreach (var delivery in deliveryModel.Deliveries)
                {
                    if (delivery.DeliverySuccessful)
                    {
                        writer.Write("{0};{1};;{2};",
                            delivery.User.LastName, delivery.User.FirstName,
                            delivery.DeliverySuccessful ? "OK" : "FEHLER");
                    }
                    else
                    {
                        writer.Write("{0};{1};{2};{3};{4}",
                            delivery.User.LastName, delivery.User.FirstName, delivery.User.Email,
                            delivery.DeliverySuccessful ? "OK" : "FEHLER",
                            delivery.ErrorMessage);

                    }
                    writer.Write(Environment.NewLine);
                }

                writer.Flush();
                writer.Dispose();

                var sb = new StringBuilder();
                sb.Append("Versandbericht");
                sb.Append(".csv");

                mailModel.Attachments.Add(new CustomMailAttachtmentModel
                {
                    FileName = sb.ToString(),
                    Bytes = ms.GetBuffer()
                });
            }

            try
            {
                new MailController().GenericMessageMail(mailModel).Deliver();
            }
            catch (Exception ex)
            {
                logger.ErrorFormat("Mailsent failed: {0}", ex.Message);
            }


            logger.InfoFormat("Mail sent to {0} receivers, # errors [{1}]", receiverList.Count, errorCount);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult MailSentSuccess(Guid? id)
        {
            if (id.HasValue)
            {
                var activityService = new ActivityService();
                var summary = activityService.GetSummary(id.Value);

                if (summary != null)
                {
                    if (summary.Activity is Newsletter)
                    {
                        ViewBag.ActionName = "Index";
                        ViewBag.ControllerName = "Messaging";
                        ViewBag.Id = "";
                        ViewBag.Text = "zur Startseite Mailverteiler";
                    }
                    else
                    {
                        ViewBag.ActionName = summary.Action;
                        ViewBag.ControllerName = summary.Controller;
                        ViewBag.Id = summary.Id;
                        ViewBag.Text = "zur Startseite von " + summary.Activity.Name;
                    }
                }
                else
                {
                    ViewBag.ActionName = "Index";
                    ViewBag.ControllerName = "Activity";
                    ViewBag.Id = "";
                    ViewBag.Text = "zur Startseite Mailverteiler";
                }
            }
            else
            {
                ViewBag.ActionName = "Index";
                ViewBag.ControllerName = "Messaging";
                ViewBag.Id = "";
                ViewBag.Text = "zur Startseite Mailverteiler";
            }

            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult MailSentError()
        {
            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public FileResult AdressList(Guid id)
        {
            var curr = Db.Curricula.SingleOrDefault(c => c.Id == id);

            var sem = SemesterService.GetSemester(DateTime.Today);

            var subscriber = Db.Subscriptions.OfType<SemesterSubscription>().Where(s => s.SemesterGroup.Semester.Id == sem.Id &&
                                                                       s.SemesterGroup.CapacityGroup.CurriculumGroup.Curriculum.Id ==
                                                                       curr.Id).ToList();

            var ms = new MemoryStream();
            var writer = new StreamWriter(ms, Encoding.Default);


            writer.Write(
                "Name;Vorname;E-Mail;Studiengang;Studiengruppe;Teilgruppe");

            writer.Write(Environment.NewLine);

            foreach (var subscription in subscriber)
            {
                var user = UserManager.FindById(subscription.UserId);
                if (user != null)
                {
                    writer.Write("{0};{1};{2};{3};{4};{5}",
                        user.LastName, user.FirstName, user.Email,
                        subscription.SemesterGroup.CapacityGroup.CurriculumGroup.Curriculum.ShortName,
                        subscription.SemesterGroup.CapacityGroup.CurriculumGroup.Name,
                        subscription.SemesterGroup.CapacityGroup.Name);
                    writer.Write(Environment.NewLine);
                }
            }

            writer.Flush();
            writer.Dispose();

            var sb = new StringBuilder();
            sb.Append("Adressliste_");
            sb.Append(curr.ShortName);
            sb.Append("_");
            sb.Append(DateTime.Today.ToString("yyMMdd"));
            sb.Append(".csv");

            return File(ms.GetBuffer(), "text/csv", sb.ToString());
        }

        public ActionResult Curriculum(Guid id)
        {
            var org = GetOrganisation(id);

            ViewBag.UserRight = GetUserRight();

            var model = new OccurrenceMailingModel();

            ViewBag.Curricula = org.Curricula.OrderBy(f => f.ShortName).Select(f => new SelectListItem
            {
                Text = f.Name,
                Value = f.Id.ToString(),
            });

            var nextDate = DateTime.Today.AddDays(70);

            ViewBag.Semesters = Db.Semesters.Where(x => x.StartCourses <= nextDate).OrderByDescending(x => x.EndCourses)
                .Select(f => new SelectListItem
                    {
                        Text = f.Name,
                        Value = f.Id.ToString(),
                    }
                );


            return View(model);
        }


        [HttpPost]
        public ActionResult Curriculum(OccurrenceMailingModel model)
        {
            var logger = LogManager.GetLogger("StudentGroup");

            var sem = Db.Semesters.SingleOrDefault(x => x.Id == model.SemId);
            var curr = Db.Curricula.SingleOrDefault(x => x.Id == model.CurrId);

            var students = Db.Students.Where(x => x.Curriculum.Id == curr.Id && x.FirstSemester.Id == sem.Id &&
                                   x.LastSemester == null).ToList();

            var userList = new List<ApplicationUser>();

            foreach (var student in students)
            {
                ApplicationUser user = UserManager.FindById(student.UserId);
                if (user != null)
                {
                    userList.Add(user);
                }
            }

            var sb = new StringBuilder();
            sb.AppendFormat("Studiengang {0}, Studienbeginn {1}", curr.ShortName, sem.Name);

            model.ListName = sb.ToString();
            model.IsDistributionList = true;

            logger.InfoFormat("UserList with {0} entires", userList.Count);

            if (!User.IsInRole("SysAdmin"))
            {
                SendMail(userList, model);
            }
            return View("ReceiverList", userList);
        }




        public ActionResult Curricula(Guid id)
        {
            var org = GetOrganisation(id);

            ViewBag.UserRight = GetUserRight();

            var model = new OccurrenceMailingModel();

            var list = new List<SelectListItem>();
            foreach (var curr in org.Curricula.OrderBy(f => f.ShortName))
            {
                var students = Db.Students.Count(x => x.Curriculum.Id == curr.Id &&
                                                      x.LastSemester == null);

                list.Add(new SelectListItem
                {
                    Text = curr.Name + "(" + students + " Empfänger:innen)",
                    Value = curr.Id.ToString(),
                });

            }

            ViewBag.Curricula = list;

            /*
            ViewBag.Curricula = org.Curricula.OrderBy(f => f.ShortName).Select(f => new SelectListItem
            {
                Text = f.Name,
                Value = f.Id.ToString(),
            });
            */


            return View(model);
        }


        [HttpPost]
        public ActionResult Curricula(OccurrenceMailingModel model)
        {
            var logger = LogManager.GetLogger("StudentGroup");

            var curr = Db.Curricula.SingleOrDefault(x => x.Id == model.CurrId);

            var students = Db.Students.Where(x => x.Curriculum.Id == curr.Id &&
            x.LastSemester == null).ToList();

            var userList = new List<ApplicationUser>();

            foreach (var student in students)
            {
                ApplicationUser user = UserManager.FindById(student.UserId);
                if (user != null)
                {
                    userList.Add(user);
                }
            }

            var sb = new StringBuilder();
            sb.AppendFormat("Studiengang {0}", curr.ShortName);

            model.ListName = sb.ToString();
            model.IsDistributionList = true;

            logger.InfoFormat("UserList with {0} entires", userList.Count);

            if (!User.IsInRole("SysAdmin"))
            {
                SendMail(userList, model);
            }
            return View("ReceiverList", userList);
        }


        public ActionResult CurriculumModule(Guid moduleId, Guid semId)
        {
            ViewBag.UserRight = GetUserRight();

            var model = new OccurrenceMailingModel();

            model.ModuleId = moduleId;
            model.SemId = semId;

            var module = Db.CurriculumModules.SingleOrDefault(x => x.Id == moduleId);

            ViewBag.Module = module;

            return View(model);
        }


        [HttpPost]
        public ActionResult CurriculumModule(OccurrenceMailingModel model)
        {
            var logger = LogManager.GetLogger("StudentGroup");

            var sem = Db.Semesters.SingleOrDefault(x => x.Id == model.SemId);
            var module = Db.CurriculumModules.SingleOrDefault(x => x.Id == model.ModuleId);


            var list = new List<ModuleParticipantModel>();

            /*
            foreach (var moduleCourse in module.ModuleCourses)
            {
                var courses =
                    moduleCourse.Nexus.Where(x => x.Course.SemesterGroups.Any(g => g.Semester.Id == sem.Id))
                        .Select(x => x.Course).Distinct().ToList();


                foreach (var course in courses)
                {
                    foreach (var subscription in course.Occurrence.Subscriptions)
                    {
                        var participant = list.SingleOrDefault(x => x.UserId.Equals(subscription.UserId));

                        if (participant == null)
                        {
                            participant = new ModuleParticipantModel
                            {
                                UserId = subscription.UserId
                            };
                            list.Add(participant);
                        }
                    }
                }
            }
            */

            var userList = new List<ApplicationUser>();

            foreach (var student in list)
            {
                ApplicationUser user = UserManager.FindById(student.UserId);
                if (user != null)
                {
                    userList.Add(user);
                }
            }

            var sb = new StringBuilder();
            sb.AppendFormat("Modul {0} ({1})", module.Name, module.ShortName);

            model.ListName = sb.ToString();
            model.IsImportant = true;
            model.IsDistributionList = false;

            logger.InfoFormat("UserList with {0} entires", userList.Count);

            if (!User.IsInRole("SysAdmin"))
            {
                SendMail(userList, model);
            }
            return View("ReceiverList", userList);
        }



    }
}