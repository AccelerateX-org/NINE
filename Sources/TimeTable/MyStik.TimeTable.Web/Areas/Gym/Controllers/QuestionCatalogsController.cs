using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyStik.TimeTable.Web.Areas.Gym.Controllers
{
    public class QuestionCatalogsController : Controller
    {
        /// <summary>
        /// Alle zugänglichen Kataloge
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {


            return View();
        }
    }
}