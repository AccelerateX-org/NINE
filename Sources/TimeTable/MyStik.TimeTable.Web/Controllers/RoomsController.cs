using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MyStik.TimeTable.Web.Models;

namespace MyStik.TimeTable.Web.Controllers
{
    public class RoomsController : BaseController
    {
        // GET: Rooms
        public ActionResult Index()
        {
            var model = Db.Organisers.Where(o => o.RoomAssignments.Any()).OrderBy(s => s.Name).ToList();

            return View(model);
        }

        public ActionResult Organiser(Guid id)
        {
            var model = new OrganiserViewModel
            {
                Organiser = GetOrganiser(id)
            };

             model.Rooms = Db.Rooms.Where(x => x.Assignments.Any(a => a.Organiser.Id == id)).ToList();

            return View(model);
        }
    }
}