using System.Linq;
using System.Web.Mvc;

namespace MyStik.TimeTable.Web.Controllers
{
    public class AdministrationController : BaseController
    {
        // GET: Administration
        public ActionResult Index()
        {
            var user = GetCurrentUser();
            var meberships = MemberService.GetFacultyMemberships(user.Id);

            if (!meberships.Any())
                return RedirectToAction("Apply");

            return View();
        }

        public ActionResult Apply()
        {
            return View();
        }
    }
}