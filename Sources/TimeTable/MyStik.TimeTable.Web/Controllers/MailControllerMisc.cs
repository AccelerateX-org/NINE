using System;
using System.Configuration;
using System.Linq;
using System.Net.Configuration;
using ActionMailer.Net.Mvc;
using log4net;
using MyStik.TimeTable.Web.Models;
using System.Net.Mail;
using MyStik.TimeTable.Data;

namespace MyStik.TimeTable.Web.Controllers
{

    /// <summary>
    /// Hier werden Mails direkt versendet 
    /// </summary>
    public partial class MailController : MailerBase
    {
        /// <summary>
        /// Bestätigung einer Eintragung
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public EmailResult Subscription(SubscriptionMailModel model)
        {
            InitSenderTopic(MAIL_SECTION_SUBSCRIPTIONS);
            To.Add(model.User.Email);
            Subject = "Ihre Eintragung in " + model.Summary.Name;

            return Email("Subscription", model);
        }

        /// <summary>
        /// Bestätigung einer Austragung
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public EmailResult Discharge(SubscriptionMailModel model)
        {
            InitSenderTopic(MAIL_SECTION_SUBSCRIPTIONS);
            To.Add(model.User.Email);
            Subject = "Ihre Eintragung in " + model.Summary.Name;

            return Email("Discharge", model);
        }



        /// <summary>
        /// Austragen durch anderen Benutzer
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public EmailResult RemoveSubscription(SubscriptionMailModel model)
        {
            InitSenderTopic(MAIL_SECTION_SUBSCRIPTIONS);
            To.Add(model.User.Email);
            CC.Add(model.SenderUser.Email);

            Subject = "Ihre Eintragung in " + model.Summary.Name;

            return Email("RemoveSubscription", model);
        }

        /// <summary>
        /// Wird nur als Dummy zum Erzeugen des Standardtexts verwendet
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public EmailResult CancelOccurrence(SubscriptionMailModel model)
        {
            From = new MailAddress(model.SenderUser.Email,
                               model.SenderUser.FirstName + " " + model.SenderUser.LastName + " (via NINE)").ToString();

            Subject = "Absage " + model.Summary.Name;


            return Email("CancelOccurrence", model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public EmailResult CancelAllOccurrences(SubscriptionMailModel model)
        {
            From = new MailAddress(model.SenderUser.Email,
                               model.SenderUser.FirstName + " " + model.SenderUser.LastName + " (via NINE)").ToString();

            Subject = "Absage " + model.Summary.Name;


            return Email("CancelAllOccurrences", model);
        }

        /// <summary>
        /// Wird nur als Dummy zum Erzeugen des Standardtexts verwendet
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public EmailResult ReactivateOccurrence(SubscriptionMailModel model)
        {
            From = new MailAddress(model.SenderUser.Email,
                               model.SenderUser.FirstName + " " + model.SenderUser.LastName + " (via NINE)").ToString();

            Subject = "Reaktivierung " + model.Summary.Name;

            return Email("ReactivateOccurrence", model);
        }

        /*
        /// <summary>
        /// E-Mail durch Benutzereingabe
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public EmailResult CustomTextEmail(CustomMailModel model)
        {
            if (!string.IsNullOrEmpty(model.SenderUser.Email))
            {

                From = new MailAddress(model.SenderUser.Email,
                    model.SenderUser.FirstName + " " + model.SenderUser.LastName + " (via NINE)").ToString();
            }

            if (model.ReceiverUsers.Count == 1)
            {
                To.Add(model.ReceiverUsers.First().Email);
            }
            else
            {
                // Mehr als einer => BCC
                foreach (var receiver in model.ReceiverUsers)
                {
                    BCC.Add(receiver.Email);
                }
            }

            foreach (var attachtment in model.Attachments)
            {
                Attachments.Add(attachtment.FileName, attachtment.Bytes);
            }


            Subject = model.Subject;

            return Email("CustomTextMail", model);
        }
        */


        /// <summary>
        /// Fachshcaft
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        internal EmailResult RegisterUnionEMail(OrgMemberMailModel model)
        {
            To.Add(model.User.Email);
            Subject = $"Mitmachen bei {model.Organiser.ShortName}";

            return Email("RegisterUnionEMail", model);
        }







    }
}