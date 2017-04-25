using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using MyStik.TimeTable.Web.Api.Contracts;
using MyStik.TimeTable.Web.Api.Responses;
using MyStik.TimeTable.Web.Services;
using MyStik.TimeTable.Web.Models;
using MyStik.TimeTable.Data;

namespace MyStik.TimeTable.Web.Api.Services
{
    public class AccountInfoService
    {
        public UserIdResponse  GetUserId (string UserName, string Password)
        {
            // Hypothese: Login schlägt fehl - es kann keine UserId ermittelt werden
            var userId = string.Empty;

            var userService = new UserService();
            var db = new ApplicationDbContext();
            var UserIdRes = new UserIdResponse();

            ApplicationUser user = null;
            //Überprüfen ob Mail
            if (UserName.Contains("@"))
            {
                //Suche ob Mail vorhanden
                var tempUser = userService.FindByEMail(UserName);

                if (tempUser != null)
                {
                    //wenn was gefunden wurde, Überprüfen ob PW stimmt
                    user = userService.UserManager.Find(tempUser.UserName, Password);
                    //wenn pw vorhanden userID abfragen
                    if (user != null)
                    {
                        //UserID Abfragen
                        userId = user.Id;
                    }
                }
            }
            //Übergebener string ist evtl Loginname
            else
            {
                //Überprüfen ob vorhanden und PW stimmt
                user = userService.UserManager.Find(UserName, Password);

                //wenn user vorhanden stimmt userID abfragen
                if (user != null)
                {
                    //UserID Abfragen
                    userId = user.Id;
                }
            }

            UserIdRes.UserId = userId;
            //Rückgabe des UserId-Response
            return UserIdRes;
        }
    }
}