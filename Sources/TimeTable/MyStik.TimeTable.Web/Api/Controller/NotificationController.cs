using MyStik.TimeTable.Web.Api.Responses;
using MyStik.TimeTable.Web.Api.Services;
using System.Web.Http;
using System.Web.Http.Results;
using System.Web.Mvc;

namespace MyStik.TimeTable.Web.Api.Controller
{
    /// <summary>
    /// 
    /// </summary>
    public class NotificationController : ApiController
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="token"></param>
        /// <param name="deviceName"></param>
        /// <returns></returns>
        [System.Web.Http.HttpGet]
        public TokenRegistryResponse TokenRegistration(string userId, string token, string deviceName)
        {
            //Initialisierung des NotificationService
            var notificationService = new NotificationService();
            
            //Erstellen des Response mit Hilfe der notificationList
            var response = new TokenRegistryResponse();

            response = notificationService.SaveToken(userId, token, deviceName);
            
            return response;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="UserId"></param>
        /// <returns></returns>
        [System.Web.Http.HttpGet]
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