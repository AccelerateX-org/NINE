﻿using MyStik.TimeTable.Web.Api.DTOs;
using MyStik.TimeTable.Web.Api.Responses;
using MyStik.TimeTable.Web.Api.Services;
using System.Linq;
using System.Web.Http;

namespace MyStik.TimeTable.Web.Api.Controller
{
    public class RegisterResponse
    {

    }

    /// <summary>
    /// 
    /// </summary>
    [AllowAnonymous]
    [RoutePrefix("api/v2/notification")]
    public class NotificationController : ApiController
    {

        [Route("register")]
        [HttpPost]
        public RegisterResponse Register([FromBody] string request)
        {

            return null;
        }


        /// <summary>
            /// 
            /// </summary>
            /// <param name="userId"></param>
            /// <param name="token"></param>
            /// <param name="deviceName"></param>
            /// <returns></returns>
            [System.Web.Http.Route("register")]
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
        [System.Web.Http.Route("")]
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