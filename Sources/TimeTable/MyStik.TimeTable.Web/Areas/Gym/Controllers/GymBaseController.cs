using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using MyStik.TimeTable.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyStik.TimeTable.Web.Areas.Gym.Controllers
{
    public class GymBaseController : Controller
    {
        /// <summary>
        /// 
        /// </summary>
        private IdentifyConfig.ApplicationUserManager _userManager;


        /// <summary>
        /// 
        /// </summary>
        protected IdentifyConfig.ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ??
                    (_userManager = HttpContext.GetOwinContext().GetUserManager<IdentifyConfig.ApplicationUserManager>());
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected ApplicationUser GetCurrentUser()
        {
            return UserManager.FindByName(User.Identity.Name);
        }


        protected ApplicationUser GetUser(string userId)
        {
            return UserManager.FindById(userId);
        }

    }
}