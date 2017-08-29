using System.Web.Mvc;

namespace MyStik.TimeTable.Web.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public class ThesisWorkflowController : Controller
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public ActionResult Accept(Models.Thesis a)
        {
            return View(a);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Accepted()
        {
            ViewBag.Vorname = "Max";
            ViewBag.Nachname = "Mustermann";
            ViewBag.Matrikelnr = "123456789";
            ViewBag.E_Mail = "mustermann@hm.edu";
            ViewBag.Thema = "Ich bin ein Thema";
            ViewBag.Firma = "Ich bin eine Firma";
            ViewBag.Expose = "Ich  bin ein Expose";

            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Denied()
        {
            ViewBag.Vorname = "Max";
            ViewBag.Nachname = "Mustermann";
            ViewBag.Matrikelnr = "123456789";
            ViewBag.E_Mail = "mustermann@hm.edu";
            ViewBag.Thema = "Ich bin ein Thema";
            ViewBag.Firma = "Ich bin eine Firma";
            ViewBag.Expose = "Ich  bin ein Expose";

            return View();
        }

    }
}