using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
                model.Subject = string.Format("[{0}]", summary.Name);
                model.ReceiverCount = subscribers.Count;
            }

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

                    foreach (var subscription in subscribers)
                    {
                        var user = UserManager.FindById(subscription.UserId);
                        if (user != null)
                        {
                            users.Add(user);
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
                return View();
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
        public ActionResult AllStudents()
        {
            ViewBag.UserRight = GetUserRight();
            return View();
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
                var org = GetMyOrganisation();

                /*
                var backgroundMailModel = GetMailModel(model);
                backgroundMailModel.OrgId = org.Id;
                backgroundMailModel.SemesterId = semester.Id;
                backgroundMailModel.SenderId = UserManager.FindByName(User.Identity.Name).Id;
                backgroundMailModel.IsImportant = model.IsImportant;
                backgroundMailModel.ListName = "Alle Studierende";
                backgroundMailModel.IsDistributionList = true;

                BackgroundJob.Enqueue<MailService>(x => x.SendAll(backgroundMailModel));
                */


                // Alle Studierenden und alle Staff, die E-Mails erhalten wollen
                ICollection<ApplicationUser> userList;
                userList = _userDb.Users.Where(u => 
                    u.MemberState == MemberState.Student || 
                    (u.MemberState == MemberState.Staff && u.LikeEMails)).ToList();

                // jetzt reduzieren, um nicht FK Mitglieder!
                var deleteList = new List<ApplicationUser>();

                var subService = new SemesterSubscriptionService(Db);

                foreach (var user in userList)
                {
                    if (user.MemberState == MemberState.Student)
                    {
                        var isInSem = subService.IsSubscribed(user.Id, semester, org);
                        if (!isInSem)
                            deleteList.Add(user);
                    }
                    else
                    {
                        var isInFK = Db.Members.Any(x =>
                            x.Organiser.Id == org.Id &&
                            string.IsNullOrEmpty(x.UserId) && x.UserId.Equals(user.Id));
                        if (!isInFK)
                            deleteList.Add(user);
                    }
                }

                foreach (var user in deleteList)
                {
                    userList.Remove(user);
                }


                model.ListName = "Alle Studierende";
                model.IsDistributionList = true;

                logger.InfoFormat("UserList with {0} entires", userList.Count);

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

                var orgName = new MemberService(Db, UserManager).GetOrganisationName(semester, User.Identity.Name);
                var organiser = Db.Organisers.SingleOrDefault(org => org.ShortName.Equals(orgName));

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
        public ActionResult StudentGroup()
        {
            var org = GetMyOrganisation();

            var semesterList = SemesterService.GetActiveSemester(org);

            ViewBag.SemesterList = semesterList.OrderByDescending(s => s.StartCourses).Select(f => new SelectListItem
            {
                Text = f.Name,
                Value = f.Name,
            });


            ViewBag.UserRight = GetUserRight();

            var model = new SemesterViewModel
            {
                Semester = SemesterService.GetSemester(DateTime.Today)
            };

            return View(model);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="GroupIds"></param>
        /// <returns></returns>
        [HttpPost]
        public PartialViewResult StudentGroupSelect(ICollection<Guid> GroupIds)
        {
            if (GroupIds != null)
            {
                var groupList = new List<SemesterGroup>();
                foreach (var groupId in GroupIds)
                {
                    groupList.Add(Db.SemesterGroups.SingleOrDefault(g => g.Id == groupId));
                }

                Session["GroupList"] = groupList;

                return PartialView("_GroupSelectionConfirm", groupList);
            }

            
            return PartialView("_GroupSelectionError");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult StudentGroupMail()
        {
            if (Session["GroupList"] == null)
            {
                return RedirectToAction("StudentGroup");
            }

            var model = new OccurrenceMailingModel();

            ViewBag.GroupList = Session["GroupList"] as ICollection<SemesterGroup>;

            return View(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="semId"></param>
        /// <returns></returns>
        [HttpPost]
        public PartialViewResult GroupList(string semId)
        {
            var semester = SemesterService.GetSemester(semId);
            var org = GetMyOrganisation();

            var model = Db.SemesterGroups
                .Where(
                    x =>
                        x.Semester.Id == semester.Id &&
                        x.CapacityGroup.CurriculumGroup.Curriculum.Organiser.Id == org.Id)
                .OrderBy(g => g.CapacityGroup.CurriculumGroup.Curriculum.Name)
                .ThenBy(g => g.CapacityGroup.CurriculumGroup.Name)
                .ThenBy(g => g.Name)
                .ToList();

            ViewBag.Semester = semester;

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

            var groupList = Session["GroupList"] as ICollection<SemesterGroup>;

            if (ModelState.IsValid && groupList != null)
            {
                // Basis: alle Staff, die E-Mails erhalten wollen => nein, das ist zu spezifisch
                var userList =  new List<ApplicationUser>();
                    //_userDb.Users.Where(u => (u.MemberState == MemberState.Staff && u.LikeEMails)).ToList();

                var sb = new StringBuilder();
                foreach (var group in groupList)
                {
                    foreach (var subscription in group.Subscriptions)
                    {
                        ApplicationUser user = UserManager.FindById(subscription.UserId);
                        if (user != null && user.EmailConfirmed)
                        {
                            userList.Add(user);
                        }
                    }
                    sb.Append(group.FullName);
                    if (group != groupList.Last())
                    {
                        sb.Append(", ");
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
            else
            {
                return View();
            }
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
            var mailModel = new GenericMailDeliveryModel
            {
                Subject = model.Subject,
                Sender = UserManager.FindByName(User.Identity.Name),
                TemplateContent = new UserMailModel
                {
                    CustomBody = model.Body,
                    IsImportant = model.IsImportant,
                    ListName = model.ListName,
                    IsDistributionList = model.IsDistributionList
                }
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
                        mailModel.Receiver = user;

                        // Versand versuchen
                        try
                        {
                            new MailController().GenericMail(mailModel).Deliver();

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
            mailModel.Receiver= mailModel.Sender;

            // Versandbericht nur an Staff
            if (mailModel.Sender.MemberState == MemberState.Staff)
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
                new MailController().GenericMail(mailModel).Deliver();
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
                    ViewBag.ActionName = summary.Action;
                    ViewBag.ControllerName = summary.Controller;
                    ViewBag.Id = summary.Id;
                }
                else
                {
                    ViewBag.ActionName = "Index";
                    ViewBag.ControllerName = "Activity";
                    ViewBag.Id = "";
                }
            }
            else
            {
                ViewBag.ActionName = "Index";
                ViewBag.ControllerName = "Mailing";
                ViewBag.Id = "";
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

    }
}