using MyStik.TimeTable.Web.Api.Responses;
using MyStik.TimeTable.Web.Api.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace MyStik.TimeTable.Web.Api.Controller
{
    public class NotificationController : ApiController
    {
        // GET: Notification+
        [HttpGet]
        public TokenRegistryResponse TokenRegistration(string userId, string token, string deviceName)
        {
            //Initialisierung des NotificationService
            var notificationService = new NotificationService();
            
            //Erstellen des Response mit Hilfe der notificationList
            var response = new TokenRegistryResponse();

            response = notificationService.SaveToken(userId, token, deviceName);
            
            return response;
        }

        [HttpGet]
        public PersonalNotificationResponse GetPersonalNotifications (string UserId)
        {
            //Initialisierung des NotificationService
            var notificationService = new NotificationService();

            //Abfrage der persönlichen Notification mit Hilfe des NotificationService
            var notificationList = notificationService.GetPersonalNotifications(UserId);

            //Erstellen des Response mit Hilfe der notificationList
            var response = new PersonalNotificationResponse
            {
                Notifications = notificationList,
            };

            //Rückgabe der Response
            return response;
        }


    }
}