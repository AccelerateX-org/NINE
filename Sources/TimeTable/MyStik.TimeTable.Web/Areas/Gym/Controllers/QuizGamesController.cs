using MyStik.Gym.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyStik.TimeTable.Web.Areas.Gym.Controllers
{
    public class QuizGamesController : GymBaseController
    {
        private GymDbContext db = new GymDbContext();

        // GET: Gym/QuizGames
        public ActionResult Index()
        {
            var model = db.QuizGames.ToList();

            return View(model);
        }
    }
}