using System.Web.Mvc;

namespace MyStik.TimeTable.Web.Areas.Lab.Controllers
{
    [AllowAnonymous]
    public class HomeController : Controller
    {
        // GET: Lab/Home
        public ActionResult MyDay()
        {
            return View();
        }
        public ActionResult WhatsUp()
        {
            return View();
        }
    }
}