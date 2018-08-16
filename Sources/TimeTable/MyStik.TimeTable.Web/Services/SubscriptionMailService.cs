using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
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

        public void SendSubscriptionEMail(Course course, string userId, ApplicationUser host)
        {
            var email = new SubscriptionEmail("Subscription")
            {
                Subject = "[nine] Eintragung in " + course.Name,

                Course = course,
                Subscription = null,
                Actor = host,
                Student = UserService.GetUser(userId)
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