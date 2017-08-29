using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using MyStik.TimeTable.Data;
using MyStik.TimeTable.Web.Controllers;
using MyStik.TimeTable.Web.Models;
using Postal;

namespace MyStik.TimeTable.Web.Services
{
    /// <summary>
    /// 
    /// </summary>
    public class MailService
    {
        /// <summary>
        /// 
        /// </summary>
        public void CheckInactive()
        {
            var userDb = new ApplicationDbContext();

            var lastDate = DateTime.Today.AddDays(-180);
            var approvedDate = DateTime.Today.AddDays(14);

            // alle die sich 180 Tage nicht angemeldet haben
            // und die noch noch nicht benachrichtigt wurden
            // d.h. kein Approved-Datum haben 
            // (das ist das Datum, an dem sie abgeschaltet werden)
            // nur Studenten
            var inactiveUsers = userDb.Users.Where(x =>
                x.LastLogin.HasValue && x.LastLogin.Value < lastDate &&
                !x.Approved.HasValue && x.MemberState == MemberState.Student).ToList();

            var email = new InactivityReportEmail()
            {
                To = "hinz@hm.edu",
                Count = inactiveUsers.Count
            };


            var viewsPath = Path.GetFullPath(HostingEnvironment.MapPath(@"~/Views/Emails"));
            var engines = new ViewEngineCollection();
            var engine = new FileSystemRazorViewEngine(viewsPath);
            engines.Add(engine);
            var emailService = new Postal.EmailService(engines);

            // Rendern und senden
            emailService.Send(email);

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mail"></param>
        public void SendAll(MailJobModel mail)
        {
            var userDb = new ApplicationDbContext();
            var Db = new TimeTableDbContext();

            ICollection<ApplicationUser> userList;
            userList = userDb.Users.Where(u =>
                u.MemberState == MemberState.Student ||
                (u.MemberState == MemberState.Staff && u.LikeEMails)).ToList();

            var sender = userDb.Users.SingleOrDefault(x => x.Id.Equals(mail.SenderId));

            // jetzt reduzieren, um nicht FK Mitglieder!
            var deleteList = new List<ApplicationUser>();

            var subService = new SemesterSubscriptionService(Db);

            foreach (var user in userList)
            {
                if (user.MemberState == MemberState.Student)
                {
                    var isInSem = subService.IsSubscribed(user.Id, mail.SemesterId, mail.OrgId);
                    if (!isInSem)
                        deleteList.Add(user);
                }
                else
                {
                    var isInFK = Db.Members.Any(x =>
                        x.Organiser.Id == mail.OrgId &&
                        string.IsNullOrEmpty(x.UserId) && x.UserId.Equals(user.Id));
                    if (!isInFK)
                        deleteList.Add(user);
                }
            }

            foreach (var user in deleteList)
            {
                userList.Remove(user);
            }

            SendMail(userList, sender, mail);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="receiverList"></param>
        /// <param name="sender"></param>
        /// <param name="model"></param>
        public void SendMail(ICollection<ApplicationUser> receiverList, ApplicationUser sender, MailJobModel model)
            {

                var viewsPath = Path.GetFullPath(HostingEnvironment.MapPath(@"~/Views/Emails"));
                var engines = new ViewEngineCollection();
                var engine = new FileSystemRazorViewEngine(viewsPath);
                engines.Add(engine);
                var emailService = new Postal.EmailService(engines);


            // Das Mail-Model aufbauen
            var mailModel = new CustomBodyEmail()
                {
                    From = sender.Email,
                    Subject = model.Subject,
                    Body = model.Body,
                    IsImportant = model.IsImportant,
                    IsDistributionList = model.IsDistributionList,
                    ListName = model.ListName
            };

                foreach (var attachment in model.Files)
                {
                    MemoryStream ms = new MemoryStream(attachment.Bytes);

                    var a = new System.Net.Mail.Attachment(ms, attachment.FileName);

                    mailModel.Attachments.Add(a);
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
                            mailModel.To = user.Email;

                            // Versand versuchen
                            try
                            {
                                emailService.Send(mailModel);

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
                mailModel.To = sender.Email;

                // Versandbericht nur an Staff
                if (sender.MemberState == MemberState.Staff)
                {
                    var ms = new MemoryStream();
                    var writer = new StreamWriter(ms, Encoding.Default);

                    writer.Write(
                        "Name;Vorname;E-Mail;Versand;Bemerkung");

                    writer.Write(Environment.NewLine);

                    foreach (var delivery in deliveryModel.Deliveries)
                    {
                        writer.Write("{0};{1};{2};{3};{4}",
                            delivery.User.LastName, delivery.User.FirstName, delivery.User.Email,
                            delivery.DeliverySuccessful ? "OK" : "FEHLER",
                            delivery.ErrorMessage);
                        writer.Write(Environment.NewLine);
                    }

                    writer.Flush();
                    writer.Dispose();

                    var sb = new StringBuilder();
                    sb.Append("Versandbericht");
                    sb.Append(".csv");

                    var bytes = ms.GetBuffer();
                    var ms2 = new MemoryStream(bytes);

                    var a = new System.Net.Mail.Attachment(ms2, sb.ToString());

                    mailModel.Attachments.Add(a);
                }

                try
                {
                    emailService.Send(mailModel);
                }
                finally { }


            }

    }
}
