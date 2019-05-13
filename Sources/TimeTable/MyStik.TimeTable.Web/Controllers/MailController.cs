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
        public static readonly string MAIL_SECTION_ACCOUNT = "Benutzerverwaltung";
        public static readonly string MAIL_SECTION_MESSAGES = "Benarchitigung";
        public static readonly string MAIL_SECTION_THESIS = "Abschlussarbeiten";
        public static readonly string MAIL_SECTION_SUBSCRIPTIONS = "Platzvergabe";

        public static string InitFrom(string section)
        {
            var topic = $"NINE {section}";
            var smtpSection = (SmtpSection)ConfigurationManager.GetSection("system.net/mailSettings/smtp");

            return new MailAddress(smtpSection.From, topic).ToString();
        }


        public static string InitSystemFrom()
        {
            var smtpSection = (SmtpSection)ConfigurationManager.GetSection("system.net/mailSettings/smtp");

            return smtpSection.From;
        }



        protected void InitSenderTopic(string section)
        {
            From = InitFrom(section);
        }
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
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public EmailResult GenericMessageMail(UserMailModel model)
        {
            InitSenderTopic(MAIL_SECTION_MESSAGES);
            To.Add(model.User.Email);

            foreach (var attachment in model.Attachments)
            {
                Attachments.Add(attachment.FileName, attachment.Bytes);
            }

            Subject = $"Eine Nachricht von {model.SenderUser.FirstName} {model.SenderUser.LastName}";

            return Email("GenericMessageMail", model);
        }

    }
}