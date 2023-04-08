using System.Web.Mvc;

namespace MyStik.TimeTable.Web.Areas.Gym.Controllers
{
    public class HomeController : Controller
    {
        // GET: Gym/Home
        public ActionResult Index()
        {
            return View();
        }
    }
}