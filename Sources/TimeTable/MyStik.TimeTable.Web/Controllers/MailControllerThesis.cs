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
        /// Entfernung durch Dritten
        /// </summary>
        /// <param name="thesisState"></param>
        /// <param name="supervisorUser"></param>
        /// <returns></returns>
        internal EmailResult ThesisSupervisorRemoveEMail(ThesisStateModel thesisState, ApplicationUser supervisorUser, ApplicationUser actionUser)
        {
            InitSenderTopic(MAIL_SECTION_THESIS);

            // geht an Lehrenden
            To.Add(supervisorUser.Email);


            Subject = $"Betreuung Abschlussarbeit";

            var mailModel = new ThesisMailModel()
            {
                Thesis = thesisState.Thesis,
                User = supervisorUser,
                ActionUser = actionUser,
                StudentUser = thesisState.User
            };


            return Email("ThesisSupervisorRemoveEMail", mailModel);
        }

        /// <summary>
        /// Annahme durch Dritten
        /// </summary>
        /// <param name="thesisState"></param>
        /// <param name="supervisorUser"></param>
        /// <returns></returns>
        internal EmailResult ThesisSupervisorAssignEMail(ThesisStateModel thesisState, ApplicationUser supervisorUser, ApplicationUser actionUser)
        {
            InitSenderTopic(MAIL_SECTION_THESIS);

            // geht an Lehrenden
            To.Add(supervisorUser.Email);


            Subject = $"Betreuung Abschlussarbeit";

            var mailModel = new ThesisMailModel()
            {
                Thesis = thesisState.Thesis,
                User = supervisorUser,
                ActionUser = actionUser,
                StudentUser = thesisState.User
            };


            return Email("ThesisSupervisorAssignEMail", mailModel);
        }


        /// <summary>
        /// Anfrage an Betreuer
        /// </summary>
        /// <param name="thesisState"></param>
        /// <param name="supervisorUser"></param>
        /// <returns></returns>
        internal EmailResult ThesisSupervisionRequestEMail(ThesisStateModel thesisState, ApplicationUser supervisorUser)
        {
            InitSenderTopic(MAIL_SECTION_THESIS);

            // geht an Lehrenden
            To.Add(supervisorUser.Email);

            // Kopie an Studierenden
            CC.Add(thesisState.User.Email);


            Subject = $"Neue Anmeldung für eine Abschlussarbeit";

            var mailModel = new ThesisMailModel()
            {
                Thesis = thesisState.Thesis,
                User = supervisorUser,
                StudentUser = thesisState.User
            };


            return Email("ThesisSupervisionRequestEMail", mailModel);
        }

        /// <summary>
        /// Anfrage an Betreuer
        /// </summary>
        /// <param name="thesisState"></param>
        /// <param name="supervisorUser"></param>
        /// <returns></returns>
        internal EmailResult ThesisSupervisionRequestEMail(ThesisStateModel thesisState, ApplicationUser supervisorUser, ApplicationUser actionUser)
        {
            InitSenderTopic(MAIL_SECTION_THESIS);

            // geht an Lehrenden
            To.Add(supervisorUser.Email);

            // Kopie an Studierenden
            CC.Add(thesisState.User.Email);


            Subject = $"Neue Anmeldung für eine Abschlussarbeit";

            var mailModel = new ThesisMailModel()
            {
                Thesis = thesisState.Thesis,
                User = supervisorUser,
                StudentUser = thesisState.User,
                ActionUser = actionUser
            };


            return Email("ThesisSupervisionRequestEMail", mailModel);
        }



        /// <summary>
        /// Antwort auf die Prüfung der Voraussetzungen
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        internal EmailResult ThesisConditionCheckResponseEMail(ThesisStateModel thesisState)
        {
            InitSenderTopic(MAIL_SECTION_THESIS);

            // geht nur an den Studierenden
            To.Add(thesisState.User.Email);

            Subject = $"Prüfung der Voraussetzungen";

            var mailModel = new ThesisMailModel()
            {
                Thesis = thesisState.Thesis,
                User = thesisState.User
            };


            return Email("ThesisConditionCheckResponseEMail", mailModel);
        }


        internal EmailResult MemberMoveDateEMail(MemberMoveDateMailModel model)
        {
            if (model.SourceUser != null)
            {
                To.Add(model.SourceUser.Email);
            }
            if (model.TargetUser != null)
            {
                To.Add(model.TargetUser.Email);
            }
            CC.Add(model.User.Email);
            Subject = $"Übertragung von Terminen";

            return Email("MemberMoveDateEMail", model);
        }

        internal EmailResult LotterySelectionEMail(LotterySelectionMailModel model)
        {
            To.Add(model.User.Email);
            Subject = $"Wahlverfahren {model.Lottery.Name}: Ihre Auswahl";

            return Email("LotterySelectionEMail", model);
        }



        private string GetTemplate(string templateName, string language)
        {
            return String.Format("{0}_{1}", language, templateName);
        }


        public EmailResult ThesisSupervisorDeliveredEMail(ThesisStateModel thesisState, ApplicationUser user)
        {
            InitSenderTopic(MAIL_SECTION_THESIS);

            // geht nur an den Studierenden
            To.Add(thesisState.User.Email);

            // cc an Betreuer bzw.Admin, der erfasst hat
            CC.Add(user.Email);

            Subject = $"Abgabe der Abschlussarbeit";

            var mailModel = new ThesisMailModel()
            {
                Thesis = thesisState.Thesis,
                User = thesisState.User,
                ActionUser = user,
            };


            return Email("ThesisDeliveredEMail", mailModel);
        }

        public EmailResult ThesisSupervisorDeliveryStornoEMail(ThesisStateModel thesisState, ApplicationUser user)
        {
            InitSenderTopic(MAIL_SECTION_THESIS);

            // geht nur an den Studierenden
            To.Add(thesisState.User.Email);

            // cc an Betreuer bzw.Admin, der erfasst hat
            CC.Add(user.Email);

            Subject = $"Abgabe der Abschlussarbeit";

            var mailModel = new ThesisMailModel()
            {
                Thesis = thesisState.Thesis,
                User = thesisState.User,
                ActionUser = user,
            };


            return Email("ThesisDeliveryStornoEMail", mailModel);
        }



        public EmailResult ThesisSupervisorIssuedEMail(ThesisStateModel thesisState, ApplicationUser user)
        {
            InitSenderTopic(MAIL_SECTION_THESIS);

            // geht nur an den Studierenden
            To.Add(thesisState.User.Email);

            // cc an Betreuer
            CC.Add(user.Email);

            Subject = $"Anmeldung der Abschlussarbeit";

            var mailModel = new ThesisMailModel()
            {
                Thesis = thesisState.Thesis,
                User = thesisState.User,
                ActionUser = user,
            };


            return Email("ThesisSupervisorIssuedEMail", mailModel);
        }

        public EmailResult ThesisSupervisionResponseEMail(ThesisStateModel thesisState, Supervisor supervisor, ApplicationUser user)
        {
            InitSenderTopic(MAIL_SECTION_THESIS);

            // geht nur an den Studierenden
            To.Add(thesisState.User.Email);

            // cc an Betreuer
            CC.Add(user.Email);

            Subject = $"Betreuung der Abschlussarbeit";

            var mailModel = new ThesisMailModel()
            {
                Thesis = thesisState.Thesis,
                User = thesisState.User,
                ActionUser = user,
                Supervisor = supervisor
            };


            return Email("ThesisSupervisionResponseEMail", mailModel);
        }

        public EmailResult ThesisSupervisorTitleChangedEMail(ThesisStateModel thesisState, ApplicationUser user)
        {
            InitSenderTopic(MAIL_SECTION_THESIS);

            // geht nur an den Studierenden
            To.Add(thesisState.User.Email);

            // cc an Betreuer
            CC.Add(user.Email);

            Subject = $"Änderung Titel der Abschlussarbeit";

            var mailModel = new ThesisMailModel()
            {
                Thesis = thesisState.Thesis,
                User = thesisState.User,
                ActionUser = user,
            };


            return Email("ThesisSupervisorTitleChangedEMail", mailModel);
        }
    }
}