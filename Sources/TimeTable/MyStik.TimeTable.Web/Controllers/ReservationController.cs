using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web.Mvc;
using log4net;
using Microsoft.AspNet.Identity;
using MyStik.TimeTable.Data;
using MyStik.TimeTable.DataServices;
using MyStik.TimeTable.Web.Models;
using MyStik.TimeTable.Web.Services;

namespace MyStik.TimeTable.Web.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public class ReservationController : BaseController
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            var org = GetMyOrganisation();

            var reservationList = Db.Activities.OfType<Reservation>().Where(r => r.Organiser.Id == org.Id).ToList();

            var courseService = new CourseService(Db);

            var model = new ReservationListViewModel();
            model.Organiser = org;

            foreach (var res in reservationList)
            {
                var rm =
                    new ReservationViewModel
                    {
                        Reservation = res,
                        Owner = UserManager.FindById(res.UserId)
                    };

                rm.FirstDate = res.Dates.OrderBy(x => x.Begin).FirstOrDefault();
                rm.LastDate = res.Dates.OrderByDescending(x => x.Begin).FirstOrDefault();

                var lectures =
                    Db.Members.Where(l => l.Dates.Any(occ => occ.Activity.Id == res.Id)).ToList();

                var rooms =
                    Db.Rooms.Where(l => l.Dates.Any(occ => occ.Activity.Id == res.Id)).ToList();

                rm.Hosts = lectures;
                rm.Rooms = rooms;


                model.Reservations.Add(rm);
            }

            ViewBag.UserRight = GetUserRight();

            return View(model);
        }


        public ActionResult Details(Guid id)
        {
            var model = Db.Activities.OfType<Reservation>().SingleOrDefault(x => x.Id == id);
            ViewBag.UserRight = GetUserRight();

            return View(model);
        }



        public ActionResult CreateReservation()
        {
            var semester = SemesterService.GetSemester(DateTime.Today);

            var roomService = new MyStik.TimeTable.Web.Services.RoomService();

            var org = GetMyOrganisation();

            var userRight = GetUserRight(User.Identity.Name, org.ShortName);

            // Alle Räume, auf die der Veranstalter Zugriff hat
            var rooms = roomService.GetRooms(org.Id, userRight.IsRoomAdmin);

            var now = DateTime.Now;
            var minute = DateTime.Now.Minute;
            var quarter = minute / 15;
            var time = now.AddMinutes(-minute + quarter * 15);

            var model = new ReservationCreateModel
            {
                NewDate = DateTime.Today.ToShortDateString(),
                NewBegin = time.TimeOfDay.ToString(),
                NewEnd = time.TimeOfDay.ToString(),
                DailyEnd = DateTime.Today.ToShortDateString(),
                WeeklyEnd = DateTime.Today.ToShortDateString(),
                IsDaily = false,
                IsWeekly = false,
                Rooms = rooms,
                OrganiserId = org.Id
            };

            ViewBag.Organiser = Db.Organisers.OrderBy(x => x.ShortName).Select(c => new SelectListItem
            {
                Text = c.ShortName,
                Value = c.Id.ToString(),
            });

            var culture = Thread.CurrentThread.CurrentUICulture;
            ViewBag.Culture = culture;


            return View(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult CreateReservation(ReservationCreateModel model)
        {
            // Doppelungen von Namen pro Organiser vermeiden
            var org = GetMyOrganisation();

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


            var semester = SemesterService.GetSemester(DateTime.Today);

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

                if (model.DozIds != null)
                {
                    foreach (var dozId in model.DozIds )
                    {
                        date.Hosts.Add(Db.Members.SingleOrDefault(x => x.Id == dozId));
                    }
                }


                reservation.Dates.Add(date);
            }

            Db.SaveChanges();

            return Json(new { result = "Redirect", url = Url.Action("Details", new { id = reservation.Id }) });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public PartialViewResult DateList(Guid id)
        {
            var model = Db.Activities.OfType<Reservation>().SingleOrDefault(r => r.Id == id);

            ViewBag.UserRight = GetUserRight();

            return PartialView("_DateList", model);
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult DeleteReservation(Guid id)
        {
            var reservation = Db.Activities.OfType<Reservation>().SingleOrDefault(r => r.Id == id);

            return View(reservation);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult DeleteReservationConfirmed(Guid id)
        {
            var reservation = Db.Activities.OfType<Reservation>().SingleOrDefault(r => r.Id == id);

            var dateList = reservation.Dates.ToList();

            foreach (var date in dateList)
            {
                DeleteService.DeleteActivityDate(date.Id);
            }

            Db.Activities.Remove(reservation);
            Db.SaveChanges();

            return RedirectToAction("Index");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult DeleteReservationDate(Guid id)
        {
            var date = Db.ActivityDates.SingleOrDefault(d => d.Id == id);

            var activity = date.Activity;
            
            DeleteService.DeleteActivityDate(id);

            return RedirectToAction("Details", new {id=activity.Id});
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult MoveDate(Guid id)
        {
            var date = Db.ActivityDates.SingleOrDefault(x => x.Id == id);
            var course = date.Activity;

            var model = new CourseMoveDateModel
            {
                Course = course,
                ActivityDateId = date.Id,
                Date = date,
                NewDate = date.Begin.ToShortDateString(),
                NewBegin = date.Begin.TimeOfDay.ToString(),
                NewEnd = date.End.TimeOfDay.ToString(),
                OrganiserId2 = course.Organiser.Id,
                OrganiserId3 = course.Organiser.Id,
                Description = date.Description
            };

            ViewBag.Organiser = Db.Organisers.OrderBy(x => x.ShortName).Select(c => new SelectListItem
            {
                Text = c.ShortName,
                Value = c.Id.ToString(),
            });


            var culture = Thread.CurrentThread.CurrentUICulture;
            ViewBag.Culture = culture;


            return View(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult MoveDate(CourseMoveDateModel model)
        {
            var logger = LogManager.GetLogger("Course");

            var activityDate = Db.ActivityDates.SingleOrDefault(d => d.Id == model.ActivityDateId);

            activityDate.Description = model.Description;

            // Berechnung der neuen Zeiten
            var day = DateTime.Parse(model.NewDate);
            var from = DateTime.Parse(model.NewBegin);
            var to = DateTime.Parse(model.NewEnd);

            var start = day.Add(from.TimeOfDay);
            var end = day.Add(to.TimeOfDay);

            // Das Änderungsobjekt anlegen
            var changeService = new ChangeService(Db);
            var changeObject = changeService.CreateActivityDateChange(activityDate, start, end, model.RoomIds);

            // Es hat keine Änderungen gegeben, die eine Notification ergeben!
            // es können sicht aber immer noch die Dozenten verändert haben
            // Das Umsetzen was geändert wurde

            activityDate.Hosts.Clear();
            if (model.DozIds != null)
            {
                foreach (var dozId in model.DozIds)
                {
                    activityDate.Hosts.Add(Db.Members.SingleOrDefault(m => m.Id == dozId));
                }
            }

            if (changeObject != null)
            {
                // Es liegt eine Änderung vor => Änderung in DB speichern
                // Änderung umsetzen
                changeObject.UserId = AppUser.Id;
                Db.DateChanges.Add(changeObject);

                // Änderungen am DateObjekt vornehmen
                activityDate.Begin = start;
                activityDate.End = end;

                // es kommen die neuen Räume und Dozenten
                // => zuerst alle löschen!
                activityDate.Rooms.Clear();

                if (model.RoomIds != null)
                {
                    foreach (var roomId in model.RoomIds)
                    {
                        activityDate.Rooms.Add(Db.Rooms.SingleOrDefault(r => r.Id == roomId));
                    }
                }
            }

            // Um die Anzahl der Transaktionen klein zu halten werden erst
            // an dieser Stelle alle Änderungen am Datum und 
            // dem ChangeObjekt gespeichert
            Db.SaveChanges();

            // Jetzt erst die Notification auslösen
            /*
            if (changeObject != null)
            {
                NotificationService nservice = new NotificationService();
                nservice.CreateSingleNotification(changeObject.Id.ToString());
            }
            */

            return Json(new { result = "Redirect", url = Url.Action("Details", new { id = activityDate.Activity.Id }) });

        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult CreateDate(Guid id)
        {
            var course = Db.Activities.SingleOrDefault(c => c.Id == id);
            var org = GetMyOrganisation();


            var model = new CourseDateCreatenModel
            {
                Course = course,
                CourseId = course.Id,
                OrganiserId2 = org.Id,
                OrganiserId3 = org.Id,
                NewDate = DateTime.Today.ToShortDateString(),
                NewBegin = DateTime.Now.TimeOfDay.ToString(),
                NewEnd = DateTime.Now.TimeOfDay.ToString(),
            };


            ViewBag.Organiser = Db.Organisers.OrderBy(x => x.ShortName).Select(c => new SelectListItem
            {
                Text = c.ShortName,
                Value = c.Id.ToString(),
            });

            // Liste aller Fakultäten, auf die Zugriff auf Räume bestehen
            // aktuell nur meine
            ViewBag.RoomOrganiser = Db.Organisers.Where(x => x.Id == org.Id).OrderBy(x => x.ShortName).Select(c => new SelectListItem
            {
                Text = c.ShortName,
                Value = c.Id.ToString(),
            });

            var culture = Thread.CurrentThread.CurrentUICulture;
            ViewBag.Culture = culture;

            return View(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult CreateDate(CourseDateCreateModelExtended model)
        {
            var course = Db.Activities.SingleOrDefault(c => c.Id == model.CourseId);

            // Jetzt die Termine - falls vorhanden
            var dozList = new List<OrganiserMember>();
            if (model.DozIds != null)
            {
                dozList.AddRange(model.DozIds.Select(dozId => Db.Members.SingleOrDefault(g => g.Id == dozId))
                    .Where(doz => doz != null));
            }

            var roomList = new List<Room>();
            if (model.RoomIds != null)
            {
                roomList.AddRange(model.RoomIds.Select(roomId => Db.Rooms.SingleOrDefault(g => g.Id == roomId))
                    .Where(doz => doz != null));
            }

            // Termine anelegen
            var semesterService = new SemesterService();

            if (model.Dates != null)
            {
                foreach (var date in model.Dates)
                {
                    string[] elems = date.Split('#');
                    var day = DateTime.Parse(elems[0]);
                    var begin = TimeSpan.Parse(elems[1]);
                    var end = TimeSpan.Parse(elems[2]);
                    var frequency = int.Parse(elems[4]);


                    ICollection<DateTime> dayList;
                    // wenn Wiederholung, dann muss auch ein Enddatum angegeben sein
                    // sonst nimm nur den Einzeltag
                    if (frequency > 0 && !string.IsNullOrEmpty(elems[3]))
                    {
                        var lastDay = DateTime.Parse(elems[3]);
                        dayList = semesterService.GetDays(day, lastDay, frequency);
                    }
                    else
                    {
                        dayList = new List<DateTime> { day };
                    }


                    foreach (var dateDay in dayList)
                    {
                        var activityDate = new ActivityDate
                        {
                            Activity = course,
                            Begin = dateDay.Add(begin),
                            End = dateDay.Add(end),
                            Occurrence = new Occurrence
                            {
                                Capacity = -1,
                                IsAvailable = true,
                                FromIsRestricted = false,
                                UntilIsRestricted = false,
                                IsCanceled = false,
                                IsMoved = false,
                                UseGroups = false,
                            },

                        };

                        foreach (var room in roomList)
                        {
                            activityDate.Rooms.Add(room);
                        }

                        foreach (var doz in dozList)
                        {
                            activityDate.Hosts.Add(doz);
                        }

                        Db.ActivityDates.Add(activityDate);

                    }
                }
            }

            Db.SaveChanges();

            return Json(new { result = "Redirect", url = Url.Action("Details", new { id = course.Id }) });

        }





    }
}