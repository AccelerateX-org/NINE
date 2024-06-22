using System;
using System.Data.Entity;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using MyStik.TimeTable.Data;
using MyStik.TimeTable.Web.Models;
using MyStik.TimeTable.Web.Services;
using Pitchfork.QRGenerator;

namespace MyStik.TimeTable.Web.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [OutputCache(VaryByParam = "*", Duration = 0, NoStore = true)]
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
        public ActionResult PersonalSchedule(Guid? id)
        {
            ViewBag.Semester = SemesterService.GetSemester(id);
            ViewBag.Name = GetCurrentUser().FullName;
            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="semId"></param>
        /// <returns></returns>
        public ActionResult Schedule(Guid id, Guid? semId)
        {
            var member = Db.Members.SingleOrDefault(m => m.Id == id);
            ViewBag.Semester = SemesterService.GetSemester(semId);
            return View("PersonalSchedule", member);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="semId"></param>
        /// <param name="showCalendar"></param>
        /// <param name="showDateList"></param>
        /// <param name="isMoSa"></param>
        /// <returns></returns>
        public ActionResult RoomSchedule(Guid id, Guid? semId, bool showCalendar, bool showDateList, bool isMoSa)
        {
            var semester = SemesterService.GetSemester(semId);

            var roomService = new RoomService();

            var model = roomService.GetRoomSchedule(id, semester);

            ViewBag.ShowCalendar = showCalendar;
            ViewBag.ShowDateList = showDateList;
            ViewBag.IsMoSa = isMoSa;
            ViewBag.UseDates = false;

            return View(model);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult RoomScheduleCurrentWeek(Guid id)
        {
            var semester = SemesterService.GetSemester(DateTime.Today);

            var roomService = new RoomService();

            var today = DateTime.Today;
            var monday = today.AddDays(-(int)today.DayOfWeek + 1);
            var sunday = monday.AddDays(6);

            var model = roomService.GetRoomSchedule(id, monday, sunday);

            ViewBag.ShowCalendar = true;
            ViewBag.ShowDateList = false;
            ViewBag.IsMoSa = true;
            ViewBag.DefaultDate = monday.ToString("yyyy-MM-dd");
            ViewBag.UseDates = true;
            ViewBag.Monday = monday;
            ViewBag.Sunday = sunday;

            return View("RoomScheduleWeek", model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult RoomScheduleNextWeek(Guid id)
        {
            var semester = SemesterService.GetSemester(DateTime.Today);

            var roomService = new RoomService();

            var today = DateTime.Today;
            var monday = today.AddDays(-(int)today.DayOfWeek + 8);
            var sunday = monday.AddDays(6);

            var model = roomService.GetRoomSchedule(id, monday, sunday);

            ViewBag.ShowCalendar = true;
            ViewBag.ShowDateList = false;
            ViewBag.IsMoSa = true;
            ViewBag.DefaultDate = monday.ToString("yyyy-MM-dd");
            ViewBag.UseDates = true;
            ViewBag.Monday = monday;
            ViewBag.Sunday = sunday;

            return View("RoomScheduleWeek", model);
        }



        public ActionResult RoomLabel(Guid id)
        {
            var semester = SemesterService.GetSemester(DateTime.Today);

            var roomService = new RoomService();

            var model = roomService.GetRoomSchedule(id, semester);

            ViewBag.ShowCalendar = true;
            ViewBag.ShowDateList = false;
            ViewBag.IsMoSa = true;
            ViewBag.DefaultDate = DateTime.Today.ToString("yyyy-MM-dd");
            ViewBag.UseDates = true;

            return View("RoomLabel", model);
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

        public ActionResult ThesisMark(Guid id)
        {
            var userService = new UserInfoService();
            var user = GetCurrentUser();

            var thesis = Db.Theses.SingleOrDefault(x => x.Id == id);

            // Mail mit Notenbeleg zum Ausdrucken an sich selbst senden
            var tm = new ThesisStateModel()
            {
                Thesis = thesis,
                Student = thesis.Student,
                User = userService.GetUser(thesis.Student.UserId),
                Mark = ""
            };

            return View("ThesisPrintOut", tm);
        }

        [AllowAnonymous]
        public ActionResult Module(Guid moduleId, Guid? semId)
        {
            var sem = SemesterService.GetSemester(semId);
            var module = Db.CurriculumModules.Include(curriculumModule => curriculumModule.Descriptions.Select(moduleDescription => moduleDescription.Semester)).Include(curriculumModule1 =>
                curriculumModule1.Descriptions.Select(moduleDescription1 => moduleDescription1.ChangeLog)).SingleOrDefault(x => x.Id == moduleId);

            var desc = module.Descriptions
                .Where(x =>
                    x.Semester.Id == sem.Id && x.ChangeLog != null)
                .OrderByDescending(x => x.ChangeLog.Created)
                .FirstOrDefault();

            return View(desc);
        }

        [AllowAnonymous]
        public ActionResult StudyPlan(Guid currId, Guid? semId)
        {
            var sem = semId.HasValue ? SemesterService.GetSemester(semId.Value) : SemesterService.GetSemester(DateTime.Today);
            var curr = Db.Curricula.SingleOrDefault(x => x.Id == currId);

            var subjects = Db.ModuleCourses.Where(x =>
                x.SubjectAccreditations.Any(c =>
                    c.Slot != null &&
                    c.Slot.AreaOption != null &&
                    c.Slot.AreaOption.Area.Curriculum.Id == curr.Id)).Include(moduleSubject => moduleSubject.Module).ToList();

            var modules = subjects.Select(x => x.Module).Distinct().ToList();


            var printModel = new StudyPlanViewModel
            {
                TimeStamp = DateTime.Now,
                Remark = "",
                Curriculum = curr,
                Semester = sem,
                Modules = modules,
            };

            return View(printModel);
        }

    }
}