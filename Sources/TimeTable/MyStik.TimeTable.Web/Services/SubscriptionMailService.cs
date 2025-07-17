using System;
using MyStik.TimeTable.Data;
using MyStik.TimeTable.Web.Models;

namespace MyStik.TimeTable.Web.Services
{
    public class SubscriptionMailService : BaseMailService
    {
        public void SendSubscriptionEMail(Course course, OccurrenceSubscription subscription, ApplicationUser host)
        {
            var email = new SubscriptionEmail("Subscription")
            {
                Subject = "[nine] Eintragung in " + course.Name,

                Course = course,
                Subscription = subscription,
                Actor = host,
                Student = UserService.GetUser(subscription.UserId)
            };

            try
            {
                EmailService.Send(email);
                Logger.InfoFormat("E-Mail an {0} erfolgreich versendet", email.Student.Email);
            }
            catch (Exception exMail)
            {
                Logger.ErrorFormat("Fehler bei E-Mail Versand an: {0} - Ursache {1}", email.Student.Email,
                    exMail.Message);
            }

        }

        public void SendSubscriptionEMail(Course course, string userId, ApplicationUser host, OrganiserMember member = null)
        {
            var user = UserService.GetUser(userId);

            if (user != null)
            {
                var email = new SubscriptionEmail("Subscription")
                {
                    Subject = "[nine] Eintragung in " + course.Name,

                    Course = course,
                    Subscription = null,
                    Actor = host,
                    Student = UserService.GetUser(userId),
                    Member = member
                };

                try
                {
                    EmailService.Send(email);
                    Logger.InfoFormat("E-Mail an {0} erfolgreich versendet", email.Student.Email);
                }
                catch (Exception exMail)
                {
                    Logger.ErrorFormat("Fehler bei E-Mail Versand an: {0} - Ursache {1}", email.Student.Email,
                        exMail.Message);
                }
            }
        }

        public void SendSucceedingEMail(Course course, OccurrenceSubscription subscription)
        {
            var email = new SubscriptionEmail("Succeeding")
            {
                Subject = "[nine] Eintragung in " + course.Name,

                Course = course,
                Subscription = subscription,
                Student = UserService.GetUser(subscription.UserId)
            };

            try
            {
                EmailService.Send(email);
                Logger.InfoFormat("E-Mail an {0} erfolgreich versendet", email.Student.Email);
            }
            catch (Exception exMail)
            {
                Logger.ErrorFormat("Fehler bei E-Mail Versand an: {0} - Ursache {1}", email.Student.Email,
                    exMail.Message);
            }

        }

    }
}