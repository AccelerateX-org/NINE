using System.Web.Http;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using MyStik.TimeTable.Data;
using MyStik.TimeTable.Web.Models;

namespace MyStik.TimeTable.Web.Api.Controller
{
    /// <summary>
    /// 
    /// </summary>
    public class ApiBaseController : ApiController
    {
        /// <summary>
        /// 
        /// </summary>
        protected readonly TimeTableDbContext Db = new TimeTableDbContext();

        /// <summary>
        /// 
        /// </summary>
        protected IdentifyConfig.ApplicationUserManager _userManager;

        /// <summary>
        /// 
        /// </summary>
        public IdentifyConfig.ApplicationUserManager UserManager
        {
            get => _userManager ?? new IdentifyConfig.ApplicationUserManager(new UserStore<ApplicationUser>(new ApplicationDbContext()));
            protected set => _userManager = value;
        }

        protected ApplicationUser GetUser(string id)
        {
            return UserManager.FindById(id);
        }
    }
}
