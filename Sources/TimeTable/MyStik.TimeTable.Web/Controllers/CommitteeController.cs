using System;
using System.Linq;
using System.Web.Mvc;

namespace MyStik.TimeTable.Web.Controllers
{
    public class CommitteeController : BaseController
    {
        /// <summary>
        /// Liste aller Gremien
        /// </summary>
        /// <param name="id">Autonomie</param>
        /// <returns></returns>
        public ActionResult Index(Guid? id)
        {
            if (id == null)
            {
                var model = Db.Committees.ToList();
                return View(model);
            }

            var model2 = Db.Committees.ToList();
            return View(model2);

            //            var model2 = Db.Committees.Where(x => x.Autonomy.Id == id).ToList();
            //            return View(model2);
        }

        public ActionResult Details(Guid id)
        {
            var model = Db.Committees.SingleOrDefault(x => x.Id == id);

            return View(model);
        }

        public ActionResult AddMember()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddMember(Guid comid, Guid memberId)
        {
            return View();
        }

        public ActionResult DeleteMember(Guid id)
        {
            return View();
        }

        public ActionResult Delete(Guid id)
        {
            return View();
        }

        public ActionResult Create()
        {
            return View();
        }


    }
}