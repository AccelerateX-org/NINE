using System.Drawing;
using System.IO;
using System.Text;
using System.Web.Routing;
using MyStik.TimeTable.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MyStik.TimeTable.DataServices;
using MyStik.TimeTable.Web.Models;
using MyStik.TimeTable.Web.Services;
using System.Data.Entity;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using Pitchfork.QRGenerator;

namespace MyStik.TimeTable.Web.Controllers
{ 
    public class RoomController : BaseController
    {
        public ActionResult Admin()
        {
            var model = Db.Rooms.OrderBy(r => r.Number).ToList();

            ViewBag.UserRight = GetUserRight(User.Identity.Name, "FK 09");

            return View(model);
        }

        /// <summary>
        /// Startseite Raumverwaltung
        /// </summary>
        /// <returns></returns>
        private ActionResult Index()
        {
            // Liste aller Fakultäten, die Räume haben



            ViewBag.FacultyList = Db.Organisers.Where(o => o.RoomAssignments.Any()).OrderBy(s => s.Name).Select(f => new SelectListItem
            {
                Text = f.Name,
                Value = f.Id.ToString(),
            });

            return View(GetMyOrganisation());
        }

        public ActionResult Calendar(Guid id)
        {
            var model = Db.Rooms.SingleOrDefault(r => r.Id == id);

            return View(model);
        }


        public ActionResult Index(Guid? id)
        {
            if (!id.HasValue)
            {
                return Index();
            }


            var model = new RoomCharacteristicModel();

            var room = Db.Rooms.SingleOrDefault(r => r.Id == id);

            if (room != null)
            {
                model.Room = room;

                var now = GlobalSettings.Now;

                model.CurrentOccurrences = room.Dates.Where(oc => oc.Begin <= now && oc.End >= now).ToList();
                model.NextOccurrences =
                    room.Dates.Where(oc => oc.Begin > now).OrderBy(oc => oc.Begin).Distinct().Take(1).ToList();
            }

            return View("Characteristics", model);
        }

        public ActionResult IndexMobil()
        {
            var model = new RoomMobileViewModel();

            model.Faculty = "FK 09 (WI)";
            model.SeminarRooms = new List<RoomMobileStateViewModel>();
            model.EDVRooms = new List<RoomMobileStateViewModel2>();
            model.AllRooms = new List<RoomMobileStateViewModel3>();
            var from = DateTime.Now;
            var until = DateTime.Now.AddMinutes(90);
             
            //var rooms = Db.Rooms.Where(r => r.Number.StartsWith("R ")).OrderBy(r => r.Number).ToList();
            var rooms = Db.Rooms.Where(room => !room.Dates.Any(d =>(d.End>@from&&d.End<=until))); //Die freien Seminarräume werden angezeigt
            var rooms1 = Db.Rooms.Where(r => r.Number.StartsWith("R BG")).OrderBy(r => r.Number).ToList(); //Die EDV-Räume im Keller werden angezeigt (jedoch nicht dynamsich)
            var rooms2 = Db.Rooms.Where(r => r.Number.StartsWith("R")).OrderBy(r => r.Number).ToList(); // Alle Räume mit "R" werden angezeigt
            
            
            //var rooms = new List<Room>();

            //var r1 = new Room
            //{
            //    Number = "abcf",
            //    Capacity = 88
            //};

            //rooms.Add(r1); 

            foreach (var room in rooms)
            {
                var rvm = new RoomMobileStateViewModel
                {
                    Room = room,
                    AnzahlTische = 7,
                    Status = "frei bis.."
                };

                model.SeminarRooms.Add(rvm);

            }
            

            foreach (var room in rooms1)
            {
                var efg = new RoomMobileStateViewModel2
                {
                    Room = room,
                    AnzahlTische = 7,
                    Status = "frei bis.."
                };

                model.EDVRooms.Add(efg);

            }

            foreach (var room in rooms2)
            {
                var abc = new RoomMobileStateViewModel3
                {
                    Room = room,
                    AnzahlTische = 7,
                    Beschreibung = "Labor"
                };

                model.AllRooms.Add(abc);
            }

            


            return View(model);
        }


        [HttpPost]
        public PartialViewResult Faculty(Guid facultyId)
        {
            var model = new List<RoomInfoModel>();
            var org = Db.Organisers.SingleOrDefault(o => o.Id == facultyId);

            if (org != null)
            {
                // Alle Räume, auf die der Veranstalter Zugriff hat
                var roomService = new MyStik.TimeTable.Web.Services.RoomService();
                var rooms = roomService.GetRooms(org.ShortName, true);


                // Für jeden Raum den Status besorgen
                foreach (var room in rooms)
                {
                    var info = new RoomInfoModel
                    {
                        Room = room,
                        CurrentDate = roomService.GetCurrentDate(room),
                        NeedInternalConfirmation = roomService.NeedInternalConfirmation(room, org.ShortName)
                    };

                    // Nächste Belegung nur dann notwendig, wenn Raum aktuell frei ist
                    if (info.CurrentDate == null)
                    {
                        info.NextDate = roomService.GetNextDate(room);
                    }

                    model.Add(info);
                }

                model = model.OrderBy(r => r.Room.Number).ToList();

                ViewBag.Organiser = org;
                ViewBag.UserRight = GetUserRight(User.Identity.Name, org.ShortName);
            }

            return PartialView("_FacultyRoomList", model);
        }


        
        public ActionResult Free()
        {
            var from = GlobalSettings.Now;
            var to = from;

            var model = new FreeRoomSummaryModel();

            // Ermittle alle Räume im Zeitraum [von, bis]

            // jeden Raum durchgehen
            // aktuelle Veranstaltung ermitteln
            // letzte Veranstaltung ermitteln, die beendet ist
            // nächste Veranstaltung ermitteln, die in Zukunft beginnen wird


            var roomService = new MyStik.TimeTable.Web.Services.RoomService();
            var fk09 = Db.Organisers.SingleOrDefault(o => o.ShortName.Equals("FK 09"));
            model.AvailableRooms = roomService.GetAvaliableRoomsNow(fk09.Id);
            model.FutureRooms = roomService.GetNextAvaliableRoomsNow(fk09.Id);

            model.AvailableRooms = model.AvailableRooms.OrderBy(r => r.Room.Number).ToList();

            ViewBag.Organiser = fk09.ShortName;

            return View("Free", model);
        }


        public ActionResult LookupDate()
        {
            var semester = GetSemester();
            var semStart = semester.StartCourses;

            var memberService = new MemberService(Db, UserManager);
            var roomService = new MyStik.TimeTable.Web.Services.RoomService();

            var orgName = User.IsInRole("SysAdmin") ?
                "FK 09" :
                memberService.GetOrganisationName(semester, User.Identity.Name);

            var userRight = GetUserRight(User.Identity.Name, "FK 09");

            // Alle Räume, auf die der Veranstalter Zugriff hat
            var rooms = roomService.GetRooms(orgName, userRight.IsOrgAdmin);



            var model = new RoomLookUpModel
            {
                NewDate = DateTime.Today.ToShortDateString(),
                NewBegin = DateTime.Now.TimeOfDay.ToString(),
                NewEnd = DateTime.Now.AddMinutes(90).TimeOfDay.ToString(),
                Rooms = rooms,
            };

            SetTimeSelections();

            return View(model);
        }

        public ActionResult LookupPeriod()
        {
            var semStart = GetSemester().StartCourses;

            var model = new RoomLookUpModel
            {
                Date1 = semStart > GlobalSettings.Today ? semStart : GlobalSettings.Today,
                Date2 = semStart > GlobalSettings.Today ? semStart : GlobalSettings.Today,
                BeginHour = GlobalSettings.Now.TimeOfDay.Hours,
                BeginMinute = (GlobalSettings.Now.TimeOfDay.Minutes / 15) * 15,
                EndHour = GlobalSettings.Now.TimeOfDay.Hours + 1,
                EndMinute = (GlobalSettings.Now.TimeOfDay.Minutes / 15) * 15,
            };

            SetTimeSelections();

            return View(model);
        }

        public ActionResult LookupDayOfWeek()
        {
            var semStart = GetSemester().StartCourses;

            var model = new RoomLookUpModel
            {
                DayOfWeek = (int)GlobalSettings.Today.DayOfWeek,
                BeginHour = GlobalSettings.Now.TimeOfDay.Hours,
                BeginMinute = (GlobalSettings.Now.TimeOfDay.Minutes / 15) * 15,
                EndHour = GlobalSettings.Now.TimeOfDay.Hours + 1,
                EndMinute = (GlobalSettings.Now.TimeOfDay.Minutes / 15) * 15,
            };

            SetTimeSelections();

            return View(model);
        }


        [HttpPost]
        public PartialViewResult RoomListByDate(DateTime date, TimeSpan from, TimeSpan to)
        {
            DateTime start = date.AddHours(from.Hours).AddMinutes(from.Minutes);
            DateTime end = date.AddHours(to.Hours).AddMinutes(to.Minutes);

            var rooms = new MyStik.TimeTable.Web.Services.RoomService().GetFreeRooms(start, end, IsOrgAdmin());

            var model = new List<FreeRoomModel>();

            foreach (var room in rooms)
            {
                var freeRoom = new FreeRoomModel
                {
                    Room = room,
                    CurrentDates = null,
                    LastDate = room.Dates.Where(d => d.End <= end).OrderBy(d => d.End).LastOrDefault(),
                    NextDate = room.Dates.Where(d => d.Begin >= end).OrderBy(d => d.Begin).FirstOrDefault(),
                    From = start,
                    To = end,
                };

                model.Add(freeRoom);
            }

            model = model.OrderByDescending(r => r.Room.Number).ToList();

            return PartialView("_RoomList", model);
        }

        [HttpPost]
        public PartialViewResult RoomListByPeriod(DateTime date1, TimeSpan from, DateTime date2, TimeSpan to)
        {
            DateTime start = date1.AddHours(from.Hours).AddMinutes(from.Minutes);
            DateTime end = date2.AddHours(to.Hours).AddMinutes(to.Minutes);

            var rooms = new MyStik.TimeTable.Web.Services.RoomService().GetFreeRooms(start, end, IsOrgAdmin());

            var model = new List<FreeRoomModel>();

            foreach (var room in rooms)
            {
                var freeRoom = new FreeRoomModel
                {
                    Room = room,
                    CurrentDates = null,
                    LastDate = room.Dates.Where(d => d.End <= end).OrderBy(d => d.End).LastOrDefault(),
                    NextDate = room.Dates.Where(d => d.Begin >= end).OrderBy(d => d.Begin).FirstOrDefault(),
                    From = start,
                    To = end,
                };

                model.Add(freeRoom);
            }

            model = model.OrderByDescending(r => r.Duration).ThenBy(r => r.Room.Number).ToList();

            return PartialView("_RoomList", model);
        }

        [HttpPost]
        public PartialViewResult RoomListByDayOfWeek(int day, TimeSpan from, TimeSpan to)
        {
            var semester = GetSemester();

            //var fk09 = Db.Organisers.SingleOrDefault(o => o.ShortName.Equals("FK 09"));

            var rooms = new MyStik.TimeTable.Web.Services.RoomService().GetFreeRooms((DayOfWeek)day, from, to, semester, IsOrgAdmin());

            var model = new List<FreeRoomModel>();

            foreach (var room in rooms)
            {
                var freeRoom = new FreeRoomModel
                {
                    Room = room,
                    CurrentDates = null,
                    LastDate = null,
                    NextDate = null,
                    From = semester.StartCourses,
                    To = semester.EndCourses,
                };

                model.Add(freeRoom);
            }

            model = model.OrderByDescending(r => r.Duration).ThenBy(r => r.Room.Number).ToList();

            return PartialView("_RoomList", model);
        }

        public ActionResult Edit(Guid id)
        {
            var model = Db.Rooms.SingleOrDefault(r => r.Id == id);

            return View(model);
        }


        [HttpPost]
        public ActionResult Edit(Room model)
        {
            if (ModelState.IsValid)
            {
                Db.Entry(model).State = EntityState.Modified;
                Db.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        public ActionResult Create()
        {
            var model = new Room();

            return View(model);
        }


        [HttpPost]
        public ActionResult Create(Room model)
        {
            if (ModelState.IsValid)
            {
                Db.Entry(model).State = EntityState.Added;
                Db.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        public ActionResult Delete(Guid id)
        {
            var room = Db.Rooms.SingleOrDefault(r => r.Id == id);

            var model = new RoomDeleteModel { Room = room };

            return View(model);
        }

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

        public ActionResult Transfer()
        {
            var model = new RoomTransferModel();


            var roomList = Db.Rooms.OrderBy(r => r.Number).ToList();

            ViewBag.SourceRoomId = new SelectList(roomList, "Id", "Number");

            ViewBag.TargetRoomId = new SelectList(roomList, "Id", "Number");


            return View(model);
        }

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

        public FileResult RoomBook()
        {
            var doc = new PdfDocument();

            foreach (var room in Db.Rooms.OrderBy(r => r.Number))
            {
                var page = doc.AddPage();
                XGraphics gfx = XGraphics.FromPdfPage(page);

                XFont fontTitle = new XFont("Verdana", 20, XFontStyle.Bold);
                XFont fontFooter = new XFont("Verdana", 10, XFontStyle.Regular); 

                var url = Url.Action("Reservation", "Public", new {id = room.Id}, Request.Url.Scheme );

                var img = Pitchfork.QRGenerator.QRCodeImageGenerator.GetQRCode(url, ErrorCorrectionLevel.Q);

                var sb = new StringBuilder();
                sb.AppendFormat("Aktuelle Belegung von Raum {0}", room.Number);

                gfx.DrawString(sb.ToString(), fontTitle, XBrushes.Black, 
                            new XRect(0, 50, page.Width, page.Height), 
                            XStringFormats.TopCenter); 

                gfx.DrawImage(img, new PointF(
                    200F,
                    250F));


                float marginLeft = 30F;
                float footerY = 700;
                float lineSpace = 12;

                gfx.DrawString("Powered by", fontFooter, XBrushes.Black, new PointF(marginLeft, footerY));
                gfx.DrawString("NINE http://nine.wi.hm.edu", fontFooter, XBrushes.Black, new PointF(marginLeft, footerY+=lineSpace));
                gfx.DrawString("PDFsharp http://www.pdfsharp.net/", fontFooter, XBrushes.Black, new PointF(marginLeft, footerY += lineSpace));
                gfx.DrawString("Pitchfork.QRGenerator https://github.com/GrabYourPitchforks/Pitchfork.QRGenerator", fontFooter, XBrushes.Black, new PointF(marginLeft, footerY += lineSpace));

            }


            var stream = new MemoryStream();

            doc.Save(stream, false);

            return File(stream, "text/pdf", "Test.pdf");
        }

        public ActionResult List(Guid id)
        {
            var room = Db.Rooms.SingleOrDefault(r => r.Id == id);

            var ac = new Services.ActivityService();

            var model = new List<ActivityDate>();
            if (room != null)
            {
                var futureOccurrenceList =
                    room.Dates.Where(o => o.Begin >= GlobalSettings.Today).OrderBy(o => o.Begin).ToList();

                foreach (var date in futureOccurrenceList)
                {
                    model.Add(date);
                }
            }

            return View(model);
        }


        public ActionResult Assignments()
        {
            var roomService = new MyStik.TimeTable.Web.Services.RoomService();

            // Raum - Organisation - Genehmigung für interne - Genehmigung für Externe
            roomService.AddAssignment("R BG.089", "FK 09", false, true);    // Raum steht FK 09 exklusiv zur Verfügung
            roomService.AddAssignment("T 2.017", "FK 09", true, false);     // Raum steht FK 09 nur teilweise zur Verfügung
            roomService.AddAssignment("R 3.099", "FK 09", true, true);     // Raum soll freigehalten werden


            return View("Admin");
        }

        [HttpPost]
        public PartialViewResult AddAssignment(ICollection<Guid> roomIds, string orgName, bool needExternal, bool needInternal)
        {
            var org = Db.Organisers.SingleOrDefault(o => o.ShortName.Equals(orgName));

            if (org != null && roomIds != null)
            {
                foreach (var roomId in roomIds)
                {
                    var room = Db.Rooms.SingleOrDefault(r => r.Id == roomId);
                    if (room != null)
                    {
                        var assignment = room.Assignments.SingleOrDefault(a => a.Organiser.Id == org.Id);
                        if (assignment == null)
                        {
                            assignment = new RoomAssignment();
                            assignment.Organiser = org;
                            room.Assignments.Add(assignment);
                        }
                        assignment.ExternalNeedConfirmation = needExternal;
                        assignment.InternalNeedConfirmation = needInternal;

                    }
                }
                Db.SaveChanges();
            }


            var model = Db.Rooms.OrderBy(r => r.Number).ToList();

            ViewBag.UserRight = GetUserRight(User.Identity.Name, "FK 09");


            return PartialView("_RoomInfoList", model);
        }


        [HttpPost]
        public PartialViewResult RemoveAssignments(ICollection<Guid> roomIds)
        {
            if (roomIds != null)
            {
                foreach (var roomId in roomIds)
                {
                    var room = Db.Rooms.SingleOrDefault(r => r.Id == roomId);
                    if (room != null)
                    {
                        var assignList = room.Assignments.ToList();
                        foreach (var assignment in assignList)
                        {
                            room.Assignments.Remove(assignment);
                            Db.RoomAssignments.Remove(assignment);
                        }
                    }
                }
                Db.SaveChanges();
            }

            var model = Db.Rooms.OrderBy(r => r.Number).ToList();

            ViewBag.UserRight = GetUserRight(User.Identity.Name, "FK 09");

            return PartialView("_RoomInfoList", model);
            
        }

        [HttpPost]
        public JsonResult GetState(Guid? dateId, Guid roomId, string date, string begin, string end)
        {
            var sDate = DateTime.Parse(date);
            var sBegin = TimeSpan.Parse(begin);
            var sEnd = TimeSpan.Parse(end);

            var from = sDate.Add(sBegin);
            var until = sDate.Add(sEnd);

            var room = Db.Rooms.SingleOrDefault(r => r.Id == roomId);


            var dateList = room.Dates.Where(d =>
                (d.End > @from && d.End <= until) || // Veranstaltung endet im Zeitraum
                (d.Begin >= @from && d.Begin < until) || // Veranstaltung beginnt im Zeitraum
                (d.Begin <= @from && d.End >= until) // Veranstaltung zieht sich über gesamten Zeitraum
                ).ToList();

            var model = new RoomOccModel();

            model.RoomId = roomId;

            if (dateList.Count == 0)
            {
                model.State = "frei";
            }
            else if (dateList.Count == 1)
            {
                if (dateId.HasValue && dateList.Any(d => d.Id == dateId.Value))
                {
                    model.State = "selbst";
                }
                else
                {
                    model.State = "belegt";
                }
            }
            else
            {
                model.State = "mehrfach belegt";
            }
            
            return Json(model);
        }


        [HttpPost]
        public JsonResult GetStateWeekly(Guid? dateId, Guid roomId, string date, string begin, string end)
        {
            var sDate = DateTime.Parse(date);
            var sBegin = TimeSpan.Parse(begin);
            var sEnd = TimeSpan.Parse(end);


            var room = Db.Rooms.SingleOrDefault(r => r.Id == roomId);

            var semester = GetSemester();

            var dayOfWeek = sDate.DayOfWeek;

            var dayList = new SemesterService().GetDays(semester.Name, dayOfWeek);

            var nCollisions = 0;

            foreach (var day in dayList)
            {
                var from = day.Add(sBegin);
                var until = day.Add(sEnd);

                nCollisions += room.Dates.Count(d =>
                    (d.End > @from && d.End <= until) || // Veranstaltung endet im Zeitraum
                    (d.Begin >= @from && d.Begin < until) || // Veranstaltung beginnt im Zeitraum
                    (d.Begin <= @from && d.End >= until) // Veranstaltung zieht sich über gesamten Zeitraum
                    );
            }

            var model = new RoomOccModel();

            model.RoomId = roomId;

            if (nCollisions == 0)
            {
                model.State = "frei";
            }
            else
            {
                model.State = String.Format("belegt (Anzahl Kollisionen {0})", nCollisions);
            }

            return Json(model);
        }

        [HttpPost]
        public JsonResult GetStates(Guid? dateId, Guid roomId, ICollection<string> dates)
        {
            var room = Db.Rooms.SingleOrDefault(r => r.Id == roomId);
            var semesterService = new SemesterService();

            var conflicts = new List<ActivityDate>();

            if (dates != null)
            {
                foreach (var date in dates)
                {
                    string[] elems = date.Split('#');
                    var day = DateTime.Parse(elems[0]);
                    var begin = TimeSpan.Parse(elems[1]);
                    var end = TimeSpan.Parse(elems[2]);
                    var isWdh = bool.Parse(elems[3]);

                    ICollection<DateTime> dayList;
                    var semester = semesterService.GetSemester(day);

                    if (isWdh && semester != null)
                    {
                        dayList = semesterService.GetDays(semester.Name, day);
                    }
                    else
                    {
                        dayList = new List<DateTime> {day};
                    }

                    foreach (var dateDay in dayList)
                    {
                        var from = dateDay.Add(begin);
                        var until = dateDay.Add(end);

                        var cl = room.Dates.Where(d =>
                            (d.End > @from && d.End <= until) || // Veranstaltung endet im Zeitraum
                            (d.Begin >= @from && d.Begin < until) || // Veranstaltung beginnt im Zeitraum
                            (d.Begin <= @from && d.End >= until) // Veranstaltung zieht sich über gesamten Zeitraum
                            ).ToList();

                        conflicts.AddRange(cl);
                    }
                }
            }

            // wenn nur ein Konflikt, dann prüfen, ob man es selbst ist
            if (conflicts.Count == 1 && dateId != null)
            {
                if (conflicts.First().Id == dateId)
                {
                    return Json(0);
                }
                else
                {
                    return Json(1);
                }

            }

            return Json(conflicts.Count);
        }



        [HttpPost]
        public PartialViewResult GetConflicts(Guid roomId, ICollection<string> Dates)
        {
            var room = Db.Rooms.SingleOrDefault(r => r.Id == roomId);
            var semesterService = new SemesterService();

            var model = new RoomConflictViewModel
            {
                Room = room
            };

            if (Dates != null)
            {
                foreach (var date in Dates)
                {
                    string[] elems = date.Split('#');
                    var day = DateTime.Parse(elems[0]);
                    var begin = TimeSpan.Parse(elems[1]);
                    var end = TimeSpan.Parse(elems[2]);
                    var isWdh = bool.Parse(elems[3]);

                    ICollection<DateTime> dayList;
                    var semester = semesterService.GetSemester(day);

                    if (isWdh && semester != null)
                    {
                        dayList = semesterService.GetDays(semester.Name, day);
                    }
                    else
                    {
                        dayList = new List<DateTime> { day };
                    }

                    foreach (var dateDay in dayList)
                    {
                        var from = dateDay.Add(begin);
                        var until = dateDay.Add(end);

                        var list = room.Dates.Where(d =>
                            (d.End > @from && d.End <= until) || // Veranstaltung endet im Zeitraum
                            (d.Begin >= @from && d.Begin < until) || // Veranstaltung beginnt im Zeitraum
                            (d.Begin <= @from && d.End >= until) // Veranstaltung zieht sich über gesamten Zeitraum
                            ).ToList();

                        if (list.Any())
                        {
                            var conflict = new RoomDateConflictViewModel
                            {
                                Begin = from,
                                End = until,
                            };

                            conflict.Conflicts.AddRange(list);

                            model.ConflictDates.Add(conflict);
                        }
                    }
                }
            }

            return PartialView("_ConflictTable", model);
        }


        [HttpPost]
        public PartialViewResult GetAvailableRooms(string date, string begin, string end)
        {
            var sDate = DateTime.Parse(date);
            var sBegin = TimeSpan.Parse(begin);
            var sEnd = TimeSpan.Parse(end);

            var from = sDate.Add(sBegin);
            var until = sDate.Add(sEnd);

            var roomService = new MyStik.TimeTable.Web.Services.RoomService();

            var org = GetMyOrganisation();

            // Alle Räume, auf die der Veranstalter Zugriff hat
            var rooms = roomService.GetAvaliableRooms(org.Id, from, until);

            return PartialView("_FreeRoomList", rooms);
        }

    }
}