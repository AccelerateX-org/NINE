using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using MyStik.TimeTable.Data;
using MyStik.TimeTable.Web.Models;

namespace MyStik.TimeTable.Web.Controllers
{
    public class StudyRoomController : BaseController
    {
        // GET: StudyRoom
        public ActionResult Index()
        {
            var org = GetMyOrganisation();

            var culture = Thread.CurrentThread.CurrentUICulture;
            ViewBag.Culture = culture;
            ViewBag.UserRight = GetUserRight(User.Identity.Name, org.ShortName);

            var model = new StudyRoomViewModel
            {
                Organiser = org,
                Date = DateTime.Today.ToShortDateString(),
            };


            return View(model);
        }

        [HttpPost]
        public PartialViewResult AvailableRooms(string date, string time)
        {

            var requesteDate = DateTime.Parse(date);

            var requestedTime = DateTime.Parse(time);

            var requestedDateTime = requesteDate.Add(requestedTime.TimeOfDay);

            var org = GetMyOrganisation();


            var roomService = new MyStik.TimeTable.Web.Services.RoomService();

            var model = new FreeRoomSummaryModel();

            model.AvailableRooms = roomService.GetAvailableLearningRooms(org.Id, requestedDateTime);


            return PartialView("_RoomList", model);
        }
    }
}