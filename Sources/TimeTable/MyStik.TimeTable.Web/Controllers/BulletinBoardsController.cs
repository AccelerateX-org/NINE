using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyStik.TimeTable.Web.Controllers
{
    public class BulletinBoardsController : BaseController
    {
        /// <summary>
        /// All boards of the current user
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            var user = GetCurrentUser();

            var model = Db.BulletinBoards.ToList();

            return View(model);
        }

        public ActionResult Details(Guid id)
        {
            var board = Db.BulletinBoards.SingleOrDefault(x => x.Id == id);

            return View(board);
        }

        /// <summary>
        /// Board eines Studiengangs
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Curriculum(Guid id) 
        {
            var curr = Db.Curricula.SingleOrDefault(x => x.Id == id);

            return RedirectToAction("Details", new {id=curr.BulletinBoard.Id});
        }

        /// <summary>
        /// Board eines Members
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Member(Guid id)
        {
            return View();
        }


        /// <summary>
        /// Board eines Gremiums
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Autonomy(Guid id)
        {
            return View();
        }

    }
}