﻿using System.Web.Http;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using MyStik.TimeTable.Web.Api.Responses;
using MyStik.TimeTable.Web.Models;

namespace MyStik.TimeTable.Web.Api.Controller
{
    /// <summary>
    /// 
    /// </summary>
    public class AccountController : ApiController
    {
        /// <summary>
        /// 
        /// </summary>
        protected IdentifyConfig.ApplicationUserManager _userManager;


        /// <summary>
        /// 
        /// </summary>
        protected AccountController()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public IdentifyConfig.ApplicationUserManager UserManager
        {
            get => _userManager ?? new IdentifyConfig.ApplicationUserManager(new UserStore<ApplicationUser>(new ApplicationDbContext()));
            protected set => _userManager = value;
        }





        /// <summary>
        /// Abfrage der UserId
        /// </summary>
        /// <param name="UserName">Username oder Email-Adresse</param>
        /// <param name="Password">Das zum UserName dazugehörige Passwort</param>
        /// <returns>Die zum Account dazugehörige UserId</returns>
        public UserIdResponse GetUserId(string UserName, string Password)
        {
            // Hypothese: Login schlägt fehl - es kann keine UserId ermittelt werden
            var userId = string.Empty;

            var db = new ApplicationDbContext();

            ApplicationUser user = null;
            //Überprüfen ob Mail
            if (UserName.Contains("@"))
            {
                //Suche ob Mail vorhanden
                var tempUser = UserManager.FindByEmail(UserName);

                if (tempUser != null)
                {
                    //wenn was gefunden wurde, Überprüfen ob PW stimmt
                    user = UserManager.Find(tempUser.UserName, Password);
                    //wenn pw vorhanden userID abfragen
                    if (user != null)
                        userId = user.Id;
                }
            }
            //Übergebener string ist evtl Loginname
            else
            {
                //Überprüfen ob vorhanden und PW stimmt
                user = UserManager.Find(UserName, Password);

                //wenn user vorhanden stimmt userID abfragen
                if (user != null)
                    userId = user.Id;
            }

            // jetzt steht in jedem Fall etwas sinnvolles in der userId drin!
            // ein guter Zeitpunkt, um die "Response" zu erstellen
            var response = new UserIdResponse
            {
                UserId = userId,
            };

            return response;
        }


    }
}
