using System.Web.Mvc;
using MyStik.TimeTable.Web.Helpers;

namespace MyStik.TimeTable.Web.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [AllowAnonymous]
    public class SiteController : BaseController
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ReturnUrl"></param>
        /// <returns></returns>
        public ActionResult AllowCookies(string ReturnUrl)
        {
            CookieConsent.SetCookieConsent(Response, true);
            return RedirectToLocal(ReturnUrl);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ReturnUrl"></param>
        /// <returns></returns>
        public ActionResult NoCookies(string ReturnUrl)
        {
            CookieConsent.SetCookieConsent(Response, false);
            // if we got an ajax submit, just return 200 OK, else redirect back
            if (Request.IsAjaxRequest())
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.OK);

            
            return RedirectToLocal(ReturnUrl);
        }
    }
}