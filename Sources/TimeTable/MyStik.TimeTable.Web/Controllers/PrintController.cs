using System;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using MyStik.TimeTable.Data;
using MyStik.TimeTable.Web.Models;
using Pitchfork.QRGenerator;

namespace MyStik.TimeTable.Web.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public class PrintController : BaseController
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult PersonalSchedule()
        {
            ViewBag.Semester = GetSemester();
            ViewBag.Name = GetCurrentUser().FullName;
            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Schedule(Guid id)
        {
            var member = Db.Members.SingleOrDefault(m => m.Id == id);
            ViewBag.Semester = GetSemester();
            return View("PersonalSchedule", member);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult RoomSchedule(Guid id)
        {
            var room = Db.Rooms.SingleOrDefault(r => r.Id == id);
            return View(room);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
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