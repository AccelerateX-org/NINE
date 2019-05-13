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
        /// Validierung einer E-Mail Adresse
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        internal EmailResult VerfiyEMail(ConfirmEmailMailModel model)
        {
            InitSenderTopic(MAIL_SECTION_ACCOUNT);

            To.Add(model.User.Email);
            Subject = "Ihre Registrierung bei NINE";

            return Email("VerifyEMail", model);
        }

        internal EmailResult VerfiyHmEMail(ConfirmEmailMailModel model)
        {
            InitSenderTopic(MAIL_SECTION_ACCOUNT);

            To.Add(model.User.Email);
            Subject = "Bestätigung Studierender der HM";

            return Email("VerifyHmEMail", model);
        }

        internal EmailResult ForgotPasswordMail(ForgotPasswordMailModel model)
        {
            InitSenderTopic(MAIL_SECTION_ACCOUNT);

            To.Add(model.User.Email);
            Subject = "Passwort zurücksetzen";

            return Email("ForgotPassword", model);
        }

        internal EmailResult ChangeEMail(ConfirmEmailMailModel model)
        {
            InitSenderTopic(MAIL_SECTION_ACCOUNT);

            // Die geänderte E-Mail Adresse steht in den Bemerkungen
            // und wird erst bei Bestätigung übertragen
            To.Add(model.User.Remark);
            Subject = "Änderung der E-Mail Adresse bei NINE";

            return Email("ChangeEMail", model);
        }

        internal EmailResult ResetPasswordMail(ResetPasswordMailModel model)
        {
            InitSenderTopic(MAIL_SECTION_ACCOUNT);

            To.Add(model.User.Email);
            Subject = "Passwort zurücksetzen";

            return Email("ResetPassword", model);
        }

        /// <summary>
        /// Der Benutzer löscht sein eigenes Konto
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        internal EmailResult DeleteUserMail(DeleteUserMailModel model)
        {
            InitSenderTopic(MAIL_SECTION_ACCOUNT);

            To.Add(model.User.Email);
            Subject = "Ihr Benutzerkonto bei NINE";

            return Email("DeleteUser", model);
        }


        /// <summary>
        /// Versand von Einladungen als Benutzer
        /// </summary>
        /// <param name="model"></param>
        /// <param name="sender"></param>
        /// <param name="language"></param>
        /// <returns></returns>
        internal EmailResult InvitationMail(ForgotPasswordMailModel model, ApplicationUser sender, string language)
        {
            InitSenderTopic(MAIL_SECTION_ACCOUNT);

            
            To.Add(model.User.Email);
            CC.Add(sender.Email);
            Subject = model.CustomSubject;

            foreach (var attachment in model.Attachments)
            {
                Attachments.Add(attachment.FileName, attachment.Bytes);
            }


            return Email(GetTemplate("Invitation", language), model);
        }


    }
}