using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MyStik.TimeTable.Web.Models;
using MyStik.TimeTable.Data;

namespace MyStik.TimeTable.Web.Controllers
{
    public class RoomsController : BaseController
    {
        // GET: Rooms
        public ActionResult Index()
        {
            var org = GetMyOrganisation();

            var model = Db.Organisers.Where(o => o.RoomAssignments.Any()).OrderBy(s => s.Name).ToList();

            ViewBag.UserRight = GetUserRight(org);

            return View(model);
        }

        public ActionResult Organiser(Guid id)
        {
            var org = GetOrganiser(id);

            var model = new OrganiserViewModel
            {
                Organiser = org
            };

             model.Rooms = Db.Rooms.Where(x => x.Assignments.Any(a => a.Organiser.Id == id)).ToList();

            ViewBag.UserRight = GetUserRight(org);

            return View(model);
        }

        public ActionResult Statistics()
        {
            return View();
        }
        public ActionResult Schedule(Guid? id)
        {
            var org = id == null ? GetMyOrganisation() : GetOrganiser(id.Value);
            ViewBag.UserRight = GetUserRight(org);
            return View();
        }

        public ActionResult Reports(Guid id)
        {
            var org = GetOrganiser(id);

            var model = new OrganiserViewModel
            {
                Organiser = org
            };


            ViewBag.UserRight = GetUserRight(org);
            return View(model);
        }


        public ActionResult Allocation()
        {

            return View();
        }

        public ActionResult Groups(Guid id)
        {
            var org = GetOrganiser(id);

            var model = new OrganiserViewModel
            {
                Organiser = org
            };

            model.Rooms = Db.Rooms.Where(x => x.Assignments.Any(a => a.Organiser.Id == id)).ToList();

            ViewBag.UserRight = GetUserRight(org);

            return View(model);
        }

    }
}