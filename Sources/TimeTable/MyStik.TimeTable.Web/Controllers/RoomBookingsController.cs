using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyStik.TimeTable.Web.Controllers
{
    public class RoomBookingsController : BaseController
    {
        // GET: RoomBookings
        public ActionResult Index(Guid id)
        {
            var org = GetOrganiser(id);

            ViewBag.UserRight = GetUserRight(org);

            return View(org);
        }
    }
}