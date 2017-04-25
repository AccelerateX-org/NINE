using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyStik.TimeTable.Web.Controllers
{
    public class ErrorController : Controller
    {
        // GET: Error
        public ActionResult MissingCourse()
        {
            return View();
        }
    }
}