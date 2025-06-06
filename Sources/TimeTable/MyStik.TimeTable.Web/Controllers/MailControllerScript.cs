﻿using ActionMailer.Net.Mvc;
using MyStik.TimeTable.Web.Models;

namespace MyStik.TimeTable.Web.Controllers
{

    /// <summary>
    /// Hier werden Mails direkt versendet 
    /// </summary>
    public partial class MailController : MailerBase
    {
        /// <summary>
        /// Entfernung durch Dritten
        /// </summary>
        /// <param name="thesisState"></param>
        /// <param name="supervisorUser"></param>
        /// <returns></returns>
        internal EmailResult ScriptOrderTicket(ScriptOrderMailModel mailModel)
        {
            InitSenderTopic(MAIL_SECTION_SCRIPTSHOP);

            // geht an jeweiligen Besteller
            To.Add(mailModel.User.Email);

            Subject = $"Bestellbestätigung / Abholschein";

            return Email("ScriptOrderTicketEMail", mailModel);
        }



        internal EmailResult ScriptOrderNotification(ScriptOrderMailModel mailModel)
        {
            InitSenderTopic(MAIL_SECTION_SCRIPTSHOP);

            // geht an jeweiligen Besteller
            To.Add(mailModel.User.Email);

            Subject = $"Benachrichtigung Skriptbestellung";

            return Email("ScriptOrderNotificationEMail", mailModel);
        }



    }
}