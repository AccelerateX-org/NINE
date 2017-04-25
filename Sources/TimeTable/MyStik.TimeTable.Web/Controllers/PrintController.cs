using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MyStik.TimeTable.Data;
using MyStik.TimeTable.Web.Models;
using Pitchfork.QRGenerator;

namespace MyStik.TimeTable.Web.Controllers
{
    public class PrintController : BaseController
    {
        // GET: Print
        public ActionResult OfficeHourDate(Guid id)
        {
            var summary = ActivityService.GetSummary(id) as ActivityDateSummary;
            var date = summary.Date;
            var oh = date.Activity as OfficeHour;
            var member = date.Hosts.FirstOrDefault();

            var model = new OfficeHourCharacteristicModel
            {
                OfficeHour = oh,
                Semester = oh.Semester,
                Host = member,
                Date = date
            };


            return View(model);
        }

        public ActionResult PersonalSchedule()
        {
            var member = GetMyMembership();
            ViewBag.Semester = GetSemester();
            return View(member);
        }

        public ActionResult Schedule(Guid id)
        {
            var member = Db.Members.SingleOrDefault(m => m.Id == id);
            ViewBag.Semester = GetSemester();
            return View("PersonalSchedule", member);
        }

        public ActionResult RoomSchedule(Guid id)
        {
            var room = Db.Rooms.SingleOrDefault(r => r.Id == id);
            return View(room);
        }

        public FileResult GetQrCode(Guid id)
        {
            var room = Db.Rooms.SingleOrDefault(r => r.Id == id);

            var url = Url.Action("Reservation", "Public", new { id = room.Id }, Request.Url.Scheme);

            var img = Pitchfork.QRGenerator.QRCodeImageGenerator.GetQRCode(url, ErrorCorrectionLevel.Q);

            MemoryStream ms = new MemoryStream();
            
            img.Save(ms, ImageFormat.Png);

            return File(ms.GetBuffer(), @"image/png");

        }
    }
}