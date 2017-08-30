using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using MyStik.TimeTable.Data;
using MyStik.TimeTable.Web.Controllers;
using MyStik.TimeTable.Web.Models;

namespace MyStik.TimeTable.Web.Areas.Admin.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public class RoomController : BaseController
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            var model = Db.Rooms.ToList();
            return View(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult UnAssigned()
        {
            var model = Db.Rooms.Where(x => !x.Assignments.Any() && (!x.Number.StartsWith("T") && !x.Number.StartsWith("R"))).ToList();
            return View(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult DeleteUnAssigned()
        {
            var rooms = Db.Rooms.Where(x => !x.Assignments.Any() && (!x.Number.StartsWith("T") && !x.Number.StartsWith("R"))).ToList();

            foreach (var room in rooms)
            {
                // Termine und Reservierungen nicht löschen
                // die könnten theoretisch in mehreren Räumen vorkommen
                Db.Rooms.Remove(room);
            }

            Db.SaveChanges();

            return RedirectToAction("UnAssigned");
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Details(Guid id)
        {
            var model = Db.Rooms.SingleOrDefault(x => x.Id == id);

            return View(model);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Edit(Guid id)
        {
            var model = Db.Rooms.SingleOrDefault(r => r.Id == id);

            return View(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Edit(Room model)
        {
            if (ModelState.IsValid)
            {
                var room = Db.Rooms.SingleOrDefault(x => x.Id == model.Id);

                if (room != null)
                {
                    room.Number = model.Number;
                    room.Name = model.Name;
                    room.Description = model.Description;
                    room.Capacity = model.Capacity;
                }

                Db.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Delete(Guid id)
        {
            var room = Db.Rooms.SingleOrDefault(r => r.Id == id);

            var model = new RoomDeleteModel { Room = room };

            return View(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Delete(RoomDeleteModel model)
        {
            var room = Db.Rooms.SingleOrDefault(r => r.Id == model.Room.Id);

            if (room != null && room.Dates.Count == 0)
            {
                Db.Rooms.Remove(room);
                Db.SaveChanges();
            }

            return RedirectToAction("Index");
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Links(Guid id)
        {
            var model = Db.Rooms.SingleOrDefault(r => r.Id == id);

            return View(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="roomId"></param>
        /// <param name="orgName"></param>
        /// <returns></returns>
        public ActionResult AddLink(Guid roomId, string orgName)
        {
            var room = Db.Rooms.SingleOrDefault(x => x.Id == roomId);
            var org = Db.Organisers.SingleOrDefault(x => x.ShortName.Equals(orgName));

            if (room != null && org != null)
            {
                var assign = room.Assignments.FirstOrDefault(x => x.Organiser.Id == org.Id);
                if (assign == null)
                {
                    assign = new RoomAssignment
                    {
                        ExternalNeedConfirmation = false,
                        InternalNeedConfirmation = false,
                        Room = room,
                        Organiser = org
                    };
                    Db.RoomAssignments.Add(assign);
                    Db.SaveChanges();
                }

            }

            return RedirectToAction("Links", new {id = roomId});
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult DeleteLink(Guid id)
        {
            var assign = Db.RoomAssignments.SingleOrDefault(x => x.Id == id);

            if (assign != null)
            {
                var room = assign.Room;
                Db.RoomAssignments.Remove(assign);
                Db.SaveChanges();
                return RedirectToAction("Links", new { id = room.Id });
            }


            return RedirectToAction("Index");
        }



        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Transfer()
        {
            var model = new RoomTransferModel();


            var roomList = Db.Rooms.OrderBy(r => r.Number).ToList();

            ViewBag.SourceRoomId = new SelectList(roomList, "Id", "Number");

            ViewBag.TargetRoomId = new SelectList(roomList, "Id", "Number");


            return View(model);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Transfer(RoomTransferModel model)
        {
            var sourceRoom = Db.Rooms.SingleOrDefault(r => r.Id == model.SourceRoomId);
            var targetRoom = Db.Rooms.SingleOrDefault(r => r.Id == model.TargetRoomId);

            if (sourceRoom == null || targetRoom == null)
                return RedirectToAction("Index");

            foreach (var date in sourceRoom.Dates.ToList())
            {
                date.Rooms.Remove(sourceRoom);
                date.Rooms.Add(targetRoom);
            }

            Db.SaveChanges();


            return RedirectToAction("Index");
        }


    }
}