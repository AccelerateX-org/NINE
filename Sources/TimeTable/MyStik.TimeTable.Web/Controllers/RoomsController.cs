using System;
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

            ViewBag.UserRights = GetUserRight(org);

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

            ViewBag.UserRights = GetUserRight(org);

            return View(model);
        }

        public ActionResult Statistics()
        {
            return View();
        }
        public ActionResult Schedule(Guid? id)
        {
            var org = id == null ? GetMyOrganisation() : GetOrganiser(id.Value);
            return View();
        }

        public ActionResult Allocation()
        {
            var from = DateTime.Today;
            var to = from.AddDays(7);

            var beginn = DateTime.Parse("08:00");
            var end = DateTime.Parse("21:00");

            var rooms = Db.Rooms.Where(x => x.Number.StartsWith("R")).Take(10).ToList();

            var roomDict = new Dictionary<Room, List<ActivityDate>>();

            foreach (var ro in rooms)
            {
                // Alle Termine in diesem Bereich
                var dates = ro.Dates.Where(x => x.Beginn >= from && x.End <= to).ToList();
                roomDict[ro] = dates;
            }
            
            var t = beginn;

            while (t <= end)
            {
                var t1 = t;
                var t2 = t1.AddMinutes(60);

                foreach (var r in rooms)
                {
                    /*
                    var alloc = roomDict[r].Where(x => 
                        !x.Occurrence.IsCancelled && 
                        ((x.Begin.TimeOdDay <= t1.TimeOdDay && x.End.TimeOdDay >= t2.TimeOdDay) || 
                        (x.Begin.TimeOdDay >= t1.TimeOdDay && x.End.TimeOdDay <= t2.TimeOdDay) || 
                        (x.Begin.TimeOdDay <= t1.TimeOdDay && x.End.TimeOdDay <= t2.TimeOdDay) || 
                        (x.Begin.TimeOdDay <= t2.TimeOdDay && x.End.TimeOdDay >= t2.TimeOdDay))

                                                (d.End > @from && d.End <= until) || // Veranstaltung endet im Zeitraum
                        (d.Begin >= @from && d.Begin < until) || // Veranstaltung beginnt im Zeitraum
                        (d.Begin <= @from && d.End >= until) // Veranstaltung zieht sich über gesamten Zeitraum


                    ).ToList();
                    if (alloc.Any())
                    {
                        // belegt
                    }
                    else
                    {
                        // frei
                    }
                    */
                }

                t = t.AddMinutes(60);
            }
        }
    }
}