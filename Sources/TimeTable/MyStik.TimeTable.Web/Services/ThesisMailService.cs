using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using MyStik.TimeTable.Data;
using MyStik.TimeTable.Web.Models;

namespace MyStik.TimeTable.Web.Services
{
    public class ThesisMailService : BaseMailService
    {
        public void SendConditionRequestAccept(ThesisStateModel model, OrganiserMember member, ApplicationUser user)
        {
            var email = new ThesisEmail("ThesisMailTemplate");

            email.Subject = "Ihr Antrag auf Anmeldung einer Abschlussarbeit";
            email.Thesis = model;
            email.Member = member;
            email.User = user;
            email.Body = "<p><strong>Antrag wurde angenommen. Die Voraussetzungen für die Abschlussarbeit sind erfüllt.</strong></p>";

            SendThesisMail(email);
        }

        public void SendConditionRequestDeny(ThesisStateModel model, OrganiserMember member, ApplicationUser user)
        {
            var email = new ThesisEmail("ThesisMailTemplate");

            email.Subject = "Ihr Antrag auf Anmeldung einer Abschlussarbeit";
            email.Thesis = model;
            email.Member = member;
            email.User = user;

            var sb = new StringBuilder();
            sb.Append(
                "<p><strong>Antrag wurde abgelehnt. Die Voraussetzungen für die Abschlussarbeit sind NICHT erfüllt.</strong></p>");
            sb.AppendFormat("<p>Begründung:</p><p>{0}</p>", model.Thesis.RequestMessage);

            email.Body = sb.ToString();

            SendThesisMail(email);

        }

        public void SendSupervisionRequest(ThesisStateModel model, OrganiserMember member, ApplicationUser user)
        {
            var email = new ThesisEmail("ThesisSupervisionRequest");

            email.Subject = "Anfrage zur Betreuung einer Abschlussarbeit";
            email.Thesis = model;
            email.Member = member;
            email.User = user;

            SendThesisMail(email);
        }

        public void SendSupervisionRequestAccept(ThesisStateModel model, OrganiserMember member, ApplicationUser user)
        {
            var email = new ThesisEmail("ThesisMailTemplate");

            email.Subject = "Ihre Anfrage zur Betreuung einer Abschlussarbeit";
            email.Thesis = model;
            email.Member = member;
            email.User = user;
            email.Body = "<p><strong>Die Betreuung Ihrer Abschlussarbeit wurde angenommen.</strong></p>";

            SendThesisMail(email);

        }

        public void SendSupervisionRequestDeny(ThesisStateModel model, OrganiserMember member, ApplicationUser user)
        {
            var email = new ThesisEmail("ThesisMailTemplate");

            email.Subject = "Ihre Anfrage zur Betreuung einer Abschlussarbeit";
            email.Thesis = model;
            email.Member = member;
            email.User = user;
            email.Body = "<p><strong>Die Betreuung Ihrer Abschlussarbeit wurde abgelehnt.</strong></p>";

            SendThesisMail(email);

        }


        private void SendThesisMail(ThesisEmail email)
        {
            try
            {
                EmailService.Send(email);
                Logger.InfoFormat("E-Mail an {0} erfolgreich versendet", email.Thesis.User.Email);
            }
            catch (Exception exMail)
            {
                Logger.ErrorFormat("Fehler bei E-Mail Versand an: {0} - Ursache {1}", email.Thesis.User.Email,
                    exMail.Message);
            }

        }

    }
}