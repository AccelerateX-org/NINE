using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using MyStik.TimeTable.Data;
using MyStik.TimeTable.DataServices;
using MyStik.TimeTable.Web.Models;
using MyStik.TimeTable.Web.Services;

namespace MyStik.TimeTable.Web.Controllers
{
    public class ReservationController : BaseController
    {
        // GET: Reservation
        public ActionResult Index()
        {
            var reservationList = Db.Activities.OfType<Reservation>().Where(r => r.Organiser.ShortName.Equals("FK 09")).ToList();

            var model = new List<ReservationViewModel>();

            foreach (var res in reservationList)
            {
                model.Add(new ReservationViewModel
                {
                    Reservation = res,
                    Owner = UserManager.FindById(res.UserId)
                });
            }

            ViewBag.UserRight = GetUserRight();

            return View(model);
        }

        public ActionResult CreateReservation()
        {
            var semester = GetSemester();

            var memberService = new MemberService(Db, UserManager);
            var roomService = new MyStik.TimeTable.Web.Services.RoomService();

            var orgName = User.IsInRole("SysAdmin") ?
                "FK 09" :
                memberService.GetOrganisationName(semester, User.Identity.Name);

            var userRight = GetUserRight(User.Identity.Name, "FK 09");

            // Alle Räume, auf die der Veranstalter Zugriff hat
            var rooms = roomService.GetRooms(orgName, userRight.IsOrgAdmin);

            var now = DateTime.Now;
            var minute = DateTime.Now.Minute;
            var quarter = minute / 15;
            var time = now.AddMinutes(-minute + quarter * 15);

            var model = new ReservationCreateModel
            {
                NewDate = GlobalSettings.Today.ToShortDateString(),
                NewBegin = time.TimeOfDay.ToString(),
                NewEnd = time.TimeOfDay.ToString(),
                DailyEnd = GlobalSettings.Today.ToShortDateString(),
                WeeklyEnd = GlobalSettings.Today.ToShortDateString(),
                IsDaily = false,
                IsWeekly = false,
                Rooms = rooms
            };


            return View(model);
        }

        [HttpPost]
        public PartialViewResult CreateReservation(ReservationCreateModel model)
        {
            // Doppelungen von Namen pro Organiser vermeiden
            var org = Db.Organisers.SingleOrDefault(o => o.ShortName.Equals("FK 09"));

            var reservation =
                Db.Activities.OfType<Reservation>().SingleOrDefault(r => r.Organiser.Id == org.Id &&
                                                                         r.Name.ToLower().Equals(model.Name.ToLower()));

            if (reservation == null)
            {
                var user = AppUser;

                reservation = new Reservation
                {
                    Name = model.Name,
                    ShortName = model.Name,
                    Description = model.Description,
                    UserId = user.Id,
                    Organiser = org
                };

                Db.Activities.Add(reservation);
            }

            // Termine hinzufügen
            var sDate = DateTime.Parse(model.NewDate);
            var sBegin = TimeSpan.Parse(model.NewBegin);
            var sEnd = TimeSpan.Parse(model.NewEnd);

            var sDateEndDaily = DateTime.Parse(model.DailyEnd);
            var sDateEndWeekly = DateTime.Parse(model.WeeklyEnd);


            var semester = GetSemester();

            var dayOfWeek = sDate.DayOfWeek;

            var dayListDaily = new List<DateTime>();
            var dayListWeekly = new List<DateTime>();

            // Erstellung der Datumsliste
            if (model.IsDaily)
            {
                // alle Tage von NewDate bis DailyDate
                // egal ob Wochenende / Feiertag
                var day = sDate;
                while (day <= sDateEndDaily)
                {
                    dayListDaily.Add(day);
                    day = day.AddDays(1);
                }
            }
            else
            {
                dayListDaily.Add(sDate);
            }
            
            
            if (model.IsWeekly)
            {
                foreach (var dailyDate in dayListDaily)
                {
                    var day = dailyDate;
                    while (day <= sDateEndWeekly)
                    {
                        dayListWeekly.Add(day);
                        day = day.AddDays(7);
                    }
                }
            }
            else
            {
                dayListWeekly.AddRange(dayListDaily);
            }


            foreach (var day in dayListWeekly)
            {
                var start = day.Add(sBegin);
                var end = day.Add(sEnd);

                var date = new ActivityDate
                {
                    Begin = start,
                    End = end,
                    //Occurrence = new Occurrence { Capacity = -1, IsAvailable = true, FromIsRestricted = false, UntilIsRestricted = false },
                };

                if (model.RoomIds != null)
                {
                    foreach (var roomId in model.RoomIds)
                    {
                        date.Rooms.Add(Db.Rooms.SingleOrDefault(r => r.Id == roomId));
                    }
                }

                reservation.Dates.Add(date);
            }

            Db.SaveChanges();


            return PartialView("_CreateReservationSuccess");
        }

        [HttpPost]
        public PartialViewResult DateList(Guid id)
        {
            var model = Db.Activities.OfType<Reservation>().SingleOrDefault(r => r.Id == id);

            ViewBag.UserRight = GetUserRight();

            return PartialView("_DateList", model);
        }

        public ActionResult DeleteReservation(Guid id)
        {
            var reservation = Db.Activities.OfType<Reservation>().SingleOrDefault(r => r.Id == id);

            var dateList = reservation.Dates.ToList();

            foreach (var date in dateList)
            {
                Db.ActivityDates.Remove(date);
            }

            reservation.Dates.Clear();
            Db.Activities.Remove(reservation);

            Db.SaveChanges();

            return RedirectToAction("Index");
        }

        [HttpPost]
        public PartialViewResult DeleteReservationDate(Guid id)
        {
            var date = Db.ActivityDates.SingleOrDefault(d => d.Id == id);

            var activity = date.Activity;
            
            activity.Dates.Remove(date);

            Db.ActivityDates.Remove(date);

            Db.SaveChanges();

            return PartialView("_EmptyRow");
        }
    }
}