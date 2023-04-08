using System.Linq;
using System.Web.Mvc;
using MyStik.TimeTable.Web.Models;

namespace MyStik.TimeTable.Web.Controllers
{
    public class NoticeBoardController : BaseController
    {
        // GET: NoticeBoard
        public ActionResult Index()
        {

            // was gibt es neues
            var advs = Db.Advertisements.ToList();


            var model = new NoticeBoardModel
            {
                Advertisements = advs
            };


            return View(model);
        }
    }
}