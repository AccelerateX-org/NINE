using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyStik.TimeTable.Web.Controllers
{
    public class ScriptDocumentsController : BaseController
    {
        // GET: Scripts
        public ActionResult Index()
        {
            var model = Db.ScriptPublishings.ToList();


            return View(model);
        }
    }
}