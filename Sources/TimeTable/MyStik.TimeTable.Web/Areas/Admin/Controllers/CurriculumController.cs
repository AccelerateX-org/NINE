using System.Web.Mvc;
using MyStik.TimeTable.Web.Controllers;

namespace MyStik.TimeTable.Web.Areas.Admin.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public class CurriculumController : BaseController
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="org"></param>
        /// <param name="curr"></param>
        /// <returns></returns>
        public ActionResult ImportModuleCatalog(string org, string curr)
        {
            // Zurück zur Startseite
            return RedirectToAction("Index", "Home");
        }
    }
}