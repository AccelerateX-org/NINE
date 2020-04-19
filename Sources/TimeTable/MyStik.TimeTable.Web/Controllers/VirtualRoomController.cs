using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MyStik.TimeTable.Data;

namespace MyStik.TimeTable.Web.Controllers
{
    public class VirtualRoomController : BaseController
    {
        // GET: VirtualRoom
        public ActionResult Index()
        {
            var member = GetMyMembership();

            var rooms = Db.VirtualRooms.Where(x => x.Owner.Id == member.Id).ToList();

            return View(rooms);
        }

        public ActionResult Create()
        {

            return View();
        }

        [HttpPost]
        public ActionResult Create(VirtualRoom model)
        {
            var member = GetMyMembership();

            var room = new VirtualRoom
            {
                Owner = member,
                Name = model.Name,
                Description = model.Description,
                AccessUrl = model.AccessUrl,
                TokenName = model.TokenName,
                Token = model.Token,
                ParticipientsOnly = true
            };

            Db.VirtualRooms.Add(room);
            Db.SaveChanges();


            return RedirectToAction("Index");
        }


        public ActionResult Edit(Guid id)
        {
            var model = Db.VirtualRooms.SingleOrDefault(x => x.Id == id);

            return View(model);
        }



        [HttpPost]
        public ActionResult Edit(VirtualRoom model)
        {
            var room = Db.VirtualRooms.SingleOrDefault(x => x.Id == model.Id);

            room.Name = model.Name;
            room.Description = model.Description;
            room.AccessUrl = model.AccessUrl;
            room.TokenName = model.TokenName;
            room.Token = model.Token;

            Db.SaveChanges();

            return RedirectToAction("Index");
        }


        public ActionResult Delete(Guid id)
        {
            var room = Db.VirtualRooms.SingleOrDefault(x => x.Id == id);

            Db.VirtualRooms.Remove(room);
            Db.SaveChanges();
            

            return RedirectToAction("Index");
        }


        [HttpPost]
        public JsonResult PersonalRoomList(Guid orgId, string number)
        {
            var memmber = GetMyMembership();

            var roomList = Db.VirtualRooms.Where(x => x.Owner.Id == memmber.Id);

            var list = roomList.Where(l => l.Name.ToUpper().Contains(number.ToUpper()))
                .OrderBy(l => l.Name)
                .Select(l => new
                {
                    name = l.Name,
                    capacity = "",
                    id = l.Id
                })
                .Take(10);
            return Json(list);
        }


        public ActionResult Details(Guid id)
        {
            var model = Db.VirtualRooms.SingleOrDefault(x => x.Id == id);

            return View(model);
        }

    }
}