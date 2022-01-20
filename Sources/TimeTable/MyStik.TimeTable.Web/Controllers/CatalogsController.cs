using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyStik.TimeTable.Web.Controllers
{
    public class CatalogsController : BaseController
    {
        // GET: Catalogs
        public ActionResult Index()
        {
            return View();
        }
    }
}