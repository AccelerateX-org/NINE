using System.Web.Mvc;

namespace MyStik.TimeTable.Web.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [AllowAnonymous]
    public class PersonController : BaseController
    {
        /// <summary>
        /// Alle Personen, deren Prfoli öffentlich ist
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="orgName"></param>
        /// <param name="shortName"></param>
        /// <returns></returns>
        public ActionResult MyProfile(string orgName, string shortName)
        {
            var model = MemberService.GetMemberFromShortName(orgName, shortName);
            return View("Profile", model);
        }
    }
}