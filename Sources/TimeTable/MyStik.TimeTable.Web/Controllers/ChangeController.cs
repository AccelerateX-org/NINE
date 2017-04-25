using MyStik.TimeTable.Data;
using MyStik.TimeTable.Web.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.ServiceModel.Web;
using MyStik.TimeTable.Web.Services;

namespace MyStik.TimeTable.Web.Controllers
{
    public class ChangeController : BaseController
    {
        // GET: Change
        public ActionResult Index()
        {
            var model = Db.DateChanges.OrderBy(c => c.TimeStamp).ToList();
            return View(model);
        }

        public ActionResult DeleteAll()
        {
            var changes = Db.DateChanges.ToList();

            foreach (var change in changes)
            {
                change.Date.Changes.Remove(change);
                Db.DateChanges.Remove(change);
            }

            Db.SaveChanges();

            return RedirectToAction("Index");
        }

        [HttpPost]
        public string CreateAllNotifications()
        {
            var notificationService = new NotificationService();
            int number = notificationService.CreateAllNotifications();
            string returnText;

            if (number == 0)
            {
                returnText = "Es sind keine zu bearbeitenden Notifications im System vorhanden.";
            }
            else if (number == 1)
            {
                returnText = "Eine Notification wurde erzeugt.";
            }
            else
            {
                returnText = number + " Notifications wurden erzeugt.";
            }

            return returnText;
        }

        [HttpPost]
        public string CreateNotification(string changeId)
        {
            ActivityDateChange change = Db.DateChanges.SingleOrDefault(c => c.Id.ToString().Equals(changeId));

            string returnString = "<p style='margin-bottom: 20px;'>Beim Abrufen des ActivityChangeDates ist ein Fehler aufgetreten.</p>";

            if (change != null) { 

            DateTime untilDate = DateTime.Now.AddDays(7);

            var notificationService = new NotificationService();
            bool notificationGenerated = notificationService.CreateSingleNotification(changeId);

            string[] helpString = new string[2];

            if (notificationGenerated == true)
            {
                helpString [0] = "Notification wurde erfolgreich erstellt und versendet.";
            }
            else 
            {
                if (DateTime.Compare(DateTime.Now, change.Date.Begin) > 0)
                {
                    helpString[0] = "Die Notification wurde nicht erstellt, da die Veranstaltung in der Vergangenheit liegt.";
                }
                else if (DateTime.Compare(change.Date.Begin, untilDate) > 0)
                {
                    helpString[0] = "Die Notification wurde nicht erstellt, da die Veranstaltung mehr als 7 Tage in der Zukunft liegt.";
                }                
                else if (change.IsNotificationGenerated)
                {
                    helpString [0] = "Die Notification wurde nicht erstellt, da sie bereits erstellt worden ist.";
                }
                else
                {
                    helpString[0] = "Beim erstellen der Notification ist ein Fehler aufgetreten.";
                }
            }

            helpString[1] = notificationService.GenerateNotificationText(changeId);

            returnString = "<span style='font-weight: bold;'>Details:</span></br><p  style='margin-bottom: 20px;'>" + helpString[0]
                    + "</p><span style='font-weight: bold;'>NotificationText:</span></br><p  style='margin-bottom: 20px;'>"
                    + helpString[1] + "</p>";
            }

            return returnString;
        }
    }
}