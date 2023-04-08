using System.Linq;
using System.Web.Mvc;
using MyStik.TimeTable.Data;
using MyStik.TimeTable.Web.Models;

namespace MyStik.TimeTable.Web.Controllers
{
    public class MessagingController : BaseController
    {
        // GET: Messaging
        public ActionResult Index()
        {
            var org = GetMyOrganisation();

            var model = new MessagingOverviewModel();

            model.Organiser = org;
            model.Newsletter = Db.Activities.OfType<Newsletter>().Where(x => x.Organiser.Id == org.Id).ToList();


            return View(model);
        }
    }
}