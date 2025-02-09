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
        internal EmailResult ThesisSupervisorRemoveEMail(ThesisMailModel mailModel, ApplicationUser supervisorUser)
        {
            InitSenderTopic(MAIL_SECTION_THESIS);

            // geht an Lehrenden
            To.Add(supervisorUser.Email);
            mailModel.User = supervisorUser;

            Subject = $"Betreuung Abschlussarbeit";

            return Email("ThesisSupervisorRemoveEMail", mailModel);
        }

        /// <summary>
        /// Annahme durch Dritten
        /// </summary>
        /// <param name="thesisState"></param>
        /// <param name="supervisorUser"></param>
        /// <returns></returns>
        internal EmailResult ThesisSupervisorAssignEMail(ThesisMailModel mailModel, ApplicationUser supervisorUser)
        {
            InitSenderTopic(MAIL_SECTION_THESIS);

            // geht an Lehrenden
            To.Add(supervisorUser.Email);
            mailModel.User = supervisorUser;

            Subject = $"Betreuung Abschlussarbeit";


            return Email("ThesisSupervisorAssignEMail", mailModel);
        }


        /// <summary>
        /// Anfrage an Betreuer
        /// </summary>
        /// <param name="thesisState"></param>
        /// <param name="supervisorUser"></param>
        /// <returns></returns>
        internal EmailResult ThesisSupervisionRequestEMail(ThesisMailModel mailModel, ApplicationUser supervisorUser)
        {
            InitSenderTopic(MAIL_SECTION_THESIS);

            // geht an Lehrenden
            To.Add(supervisorUser.Email);
            mailModel.User = supervisorUser;

            // Kopie an Studierenden
            CC.Add(mailModel.StudentUser.Email);

            Subject = $"Neue Betreuungsanfrage für eine Abschlussarbeit";


            return Email("ThesisSupervisionRequestEMail", mailModel);
        }


        internal EmailResult ThesisSupervisionProlongRequestEMail(ThesisMailModel mailModel, ApplicationUser supervisorUser)
        {
            InitSenderTopic(MAIL_SECTION_THESIS);

            // geht an Lehrenden
            To.Add(supervisorUser.Email);
            mailModel.User = supervisorUser;

            // Kopie an Studierenden
            CC.Add(mailModel.StudentUser.Email);

            Subject = $"Anfrage auf Verlängerung einer Abschlussarbeit";


            return Email("ThesisSupervisionProlongRequestEMail", mailModel);
        }



        /// <summary>
        /// Antwort auf die Prüfung der Voraussetzungen
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        internal EmailResult ThesisConditionCheckResponseEMail(ThesisMailModel mailModel)
        {
            InitSenderTopic(MAIL_SECTION_THESIS);

            // geht nur an den Studierenden
            To.Add(mailModel.StudentUser.Email);
            mailModel.User = mailModel.StudentUser;

            Subject = $"Prüfung der Voraussetzungen";


            return Email("ThesisConditionCheckResponseEMail", mailModel);
        }



        public EmailResult ThesisSupervisorDeliveredEMail(ThesisMailModel mailModel)
        {
            InitSenderTopic(MAIL_SECTION_THESIS);

            // geht nur an den Studierenden
            To.Add(mailModel.StudentUser.Email);
            mailModel.User = mailModel.StudentUser;

            // cc an Betreuer
            foreach (var user in mailModel.SupervisorUsers)
            {
                CC.Add(user.Email);
            }

            // cc an Betreuer bzw.Admin, der erfasst hat
            CC.Add(mailModel.ActionUser.Email);

            Subject = $"Abgabe der Abschlussarbeit";


            return Email("ThesisDeliveredEMail", mailModel);
        }

        public EmailResult ThesisSupervisorDeliveryStornoEMail(ThesisMailModel mailModel)
        {
            InitSenderTopic(MAIL_SECTION_THESIS);

            // geht nur an den Studierenden
            To.Add(mailModel.StudentUser.Email);
            mailModel.User = mailModel.StudentUser;

            // cc an Betreuer bzw.Admin, der erfasst hat
            CC.Add(mailModel.ActionUser.Email);

            Subject = $"Abgabe der Abschlussarbeit";

            return Email("ThesisDeliveryStornoEMail", mailModel);
        }



        public EmailResult ThesisSupervisorIssuedEMail(ThesisMailModel mailModel)
        {
            InitSenderTopic(MAIL_SECTION_THESIS);

            // geht nur an den Studierenden
            To.Add(mailModel.StudentUser.Email);
            mailModel.User = mailModel.StudentUser;

            // cc an Betreuer
            foreach (var user in mailModel.SupervisorUsers)
            {
                CC.Add(user.Email);
            }

            Subject = $"Anmeldung der Abschlussarbeit";

            return Email("ThesisSupervisorIssuedEMail", mailModel);
        }

        public EmailResult ThesisSupervisionResponseEMail(ThesisMailModel mailModel)
        {
            InitSenderTopic(MAIL_SECTION_THESIS);

            // geht nur an den Studierenden
            To.Add(mailModel.StudentUser.Email);
            mailModel.User = mailModel.StudentUser;

            // cc an Betreuer
            foreach (var user in mailModel.SupervisorUsers)
            {
                CC.Add(user.Email);
            }

            Subject = $"Betreuung der Abschlussarbeit";


            return Email("ThesisSupervisionResponseEMail", mailModel);
        }

        public EmailResult ThesisSupervisorTitleChangedEMail(ThesisMailModel mailModel)
        {
            InitSenderTopic(MAIL_SECTION_THESIS);

            // geht nur an den Studierenden
            To.Add(mailModel.StudentUser.Email);
            mailModel.User = mailModel.StudentUser;

            // cc an Betreuer
            foreach (var user in mailModel.SupervisorUsers)
            {
                CC.Add(user.Email);
            }

            Subject = $"Änderung Thema der Abschlussarbeit";


            return Email("ThesisSupervisorTitleChangedEMail", mailModel);
        }

        public EmailResult ThesisMarkedEMail(ThesisMailModel mailModel)
        {
            InitSenderTopic(MAIL_SECTION_THESIS);

            // geht nur an den Studierenden
            To.Add(mailModel.StudentUser.Email);
            mailModel.User = mailModel.StudentUser;

            // cc an Betreuer
            foreach (var user in mailModel.SupervisorUsers)
            {
                CC.Add(user.Email);
            }

            // cc an Betreuer bzw.Admin, der erfasst hat
            CC.Add(mailModel.ActionUser.Email);

            Subject = $"Notenmeldung Ihrer Abschlussarbeit";


            return Email("ThesisMarkedEMail", mailModel);
        }


        public EmailResult ThesisSupervisorAcceptProlongEMail(ThesisMailModel mailModel)
        {
            InitSenderTopic(MAIL_SECTION_THESIS);

            // geht nur an den Studierenden
            To.Add(mailModel.StudentUser.Email);
            mailModel.User = mailModel.StudentUser;

            // cc an Betreuer
            foreach (var user in mailModel.SupervisorUsers)
            {
                CC.Add(user.Email);
            }

            // cc an Betreuer bzw.Admin, der erfasst hat
            CC.Add(mailModel.ActionUser.Email);

            Subject = $"Annahme Verlängerungsantrag der Abschlussarbeit";


            return Email("ThesisProlongAcceptedEMail", mailModel);
        }


        public EmailResult ThesisSupervisorRejectProlongEMail(ThesisMailModel mailModel)
        {
            InitSenderTopic(MAIL_SECTION_THESIS);

            // geht nur an den Studierenden
            To.Add(mailModel.StudentUser.Email);
            mailModel.User = mailModel.StudentUser;

            // cc an Betreuer
            foreach (var user in mailModel.SupervisorUsers)
            {
                CC.Add(user.Email);
            }

            // cc an Betreuer bzw.Admin, der erfasst hat
            CC.Add(mailModel.ActionUser.Email);

            Subject = $"Annahme Verlängerungsantrag der Abschlussarbeit";


            return Email("ThesisProlongRejectedEMail", mailModel);
        }

        /// <summary>
        /// Betreuer hat Verlängerung zugestimmt
        /// Mail an Studierenden mit cc an PKV
        /// </summary>
        /// <param name="mailModel"></param>
        /// <returns></returns>
        public EmailResult ThesisProlongRequestBoardEMail(ThesisMailModel mailModel)
        {
            InitSenderTopic(MAIL_SECTION_THESIS);

            // geht nur an den Studierenden
            To.Add(mailModel.StudentUser.Email);
            mailModel.User = mailModel.StudentUser;


            // geht in cc an PK.Vorsitz
            CC.Add(mailModel.BoardUser.Email);
            // cc an Betreuer bzw.Admin, der erfasst hat
            CC.Add(mailModel.ActionUser.Email);

            Subject = $"Verlängerungsantrag der Abschlussarbeit";


            return Email("ThesisProlongRequestBoardEMail", mailModel);
        }


    }
}