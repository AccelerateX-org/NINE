using System;
using System.Linq;
using ActionMailer.Net.Mvc;
using log4net;
using MyStik.TimeTable.Web.Models;
using System.Net.Mail;

namespace MyStik.TimeTable.Web.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public class MailController : MailerBase
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="mail"></param>
        protected override void OnMailSent(MailMessage mail)
        {
            var logger = LogManager.GetLogger("OnMailSent");

            if (mail.To.Count == 1)
            {
                logger.DebugFormat("Mail sent: Sender[{0}] Subject[{1}] Receiver[{2}]", mail.From, mail.Subject, mail.To.First());
            }
            else
            {
                logger.DebugFormat("Mail sent: Sender[{0}] Subject[{1}] Receivers[{2}-{3}-{4}]", mail.From, mail.Subject, mail.To.Count, mail.CC.Count, mail.Bcc.Count);
            }
        }


        /// <summary>
        /// Bestätigung einer Eintragung
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public EmailResult Subscription(SubscriptionMailModel model)
        {
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
            To.Add(model.User.Email);
            Subject = "Ihre Eintragung in " + model.Summary.Name;

            return Email("Discharge", model);
        }

        /// <summary>
        /// Validierung einer E-Mail Adresse
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        internal EmailResult VerfiyEMail(ConfirmEmailMailModel model)
        {
            To.Add(model.User.Email);
            Subject = "Ihre Registrierung bei NINE";

            return Email("VerifyEMail", model);
        }

        internal EmailResult VerfiyHmEMail(ConfirmEmailMailModel model)
        {
            To.Add(model.User.Email);
            Subject = "Bestätigung Studierender der HM";

            return Email("VerifyHmEMail", model);
        }

        internal EmailResult ForgotPasswordMail(ForgotPasswordMailModel model)
        {
            To.Add(model.User.Email);
            Subject = "Passwort zurücksetzen";

            return Email("ForgotPassword", model);
        }

        internal EmailResult ChangeEMail(ConfirmEmailMailModel model)
        {
            // Die geänderte E-Mail Adresse steht in den Bemerkungen
            // und wird erst bei Bestätigung übertragen
            To.Add(model.User.Remark);
            Subject = "Änderung der E-Mail Adresse bei NINE";

            return Email("ChangeEMail", model);
        }


        internal EmailResult InvitationMail(ForgotPasswordMailModel model, ApplicationUser sender, string language)
        {
            From = new MailAddress(sender.Email,
                               sender.FirstName + " " + sender.LastName + " (via NINE)").ToString();
            
            To.Add(model.User.Email);
            Subject = model.CustomSubject;

            foreach (var attachment in model.Attachments)
            {
                Attachments.Add(attachment.FileName, attachment.Bytes);
            }


            return Email(GetTemplate("Invitation", language), model);
        }

        internal EmailResult ResetPasswordMail(ResetPasswordMailModel model)
        {
            To.Add(model.User.Email);
            Subject = "Passwort zurücksetzen";

            return Email("ResetPassword", model);
        }

        internal EmailResult DeleteUserMail(DeleteUserMailModel model)
        {
            To.Add(model.User.Email);
            Subject = "Ihr Benutzerkonto bei NINE";

            return Email("DeleteUser", model);
        }


        /// <summary>
        /// Austragen durch anderen Benutzer
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public EmailResult RemoveSubscription(SubscriptionMailModel model)
        {
            To.Add(model.User.Email);
            From = new MailAddress(model.SenderUser.Email,
                               model.SenderUser.FirstName + " " + model.SenderUser.LastName + " (via NINE)").ToString();

            Subject = "Ihre Eintragung in " + model.Summary.Name;

            return Email("RemoveSubscription", model);
        }

        #region MailDummies
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
        #endregion

        #region TextMails (CustomMails)
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
        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public EmailResult GenericMail(GenericMailDeliveryModel model)
        {
            if (model.Sender != null && !string.IsNullOrEmpty(model.Sender.Email))
            {
                From = new MailAddress(model.Sender.Email,
                    model.Sender.FirstName + " " + model.Sender.LastName + " (via NINE)").ToString();
            }

            To.Add(model.Receiver.Email);

            foreach (var attachment in model.Attachments)
            {
                Attachments.Add(attachment.FileName, attachment.Bytes);
            }

            Subject = model.Subject;

            if (string.IsNullOrEmpty(model.TemplateName))
            {
                // Kein Template gegeben => Komplett Custom
                return Email("CustomTextMail2", model.TemplateContent);
            }

            // Es ist ein Template gegeben => das auch verwenden
            return Email(model.TemplateName, model.TemplateContent);
        }

        private string GetTemplate(string templateName, string language)
        {
            return String.Format("{0}_{1}", language, templateName);
        }

    }
}