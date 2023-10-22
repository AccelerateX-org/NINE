using System.IO;
using System.Text;
using MyStik.TimeTable.Data;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Drawing.Imaging;
using System.Linq;
using System.Web.Mvc;
using MyStik.TimeTable.DataServices;
using MyStik.TimeTable.Web.Models;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using Pitchfork.QRGenerator;
using RoomService = MyStik.TimeTable.Web.Services.RoomService;

namespace MyStik.TimeTable.Web.Controllers
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
        public ActionResult Admin()
        {
            var model = Db.Rooms.OrderBy(r => r.Number).ToList();

            var org = GetMyOrganisation();

            ViewBag.UserRight = GetUserRight(User.Identity.Name, org.ShortName);

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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Calendar(Guid id)
        {
            var model = Db.Rooms.SingleOrDefault(r => r.Id == id);

            return View(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Details(Guid id)
        {
            var model = Db.Rooms.SingleOrDefault(r => r.Id == id);

            var org = GetMyOrganisation();
            ViewBag.UserRight = GetUserRight(User.Identity.Name, org.ShortName);
            ViewBag.Organiser = org;

            return View(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Rules(Guid id)
        {
            var model = Db.Rooms.SingleOrDefault(r => r.Id == id);

            var org = GetMyOrganisation();
            ViewBag.UserRight = GetUserRight(User.Identity.Name, org.ShortName);
            ViewBag.Organiser = org;

            ViewBag.Admins = org.Members.Where(x => x.IsRoomAdmin);

            return View(model);
        }




        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
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

                var now = DateTime.Now;

                model.CurrentOccurrences = room.Dates.Where(oc => oc.Begin <= now && oc.End >= now).ToList();
                model.NextOccurrences =
                    room.Dates.Where(oc => oc.Begin > now).OrderBy(oc => oc.Begin).Distinct().Take(1).ToList();
            }

            return View("Characteristics", model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="facultyId"></param>
        /// <returns></returns>
        [HttpPost]
        public PartialViewResult Faculty(Guid facultyId)
        {
            var model = new List<RoomInfoModel>();
            var org = Db.Organisers.SingleOrDefault(o => o.Id == facultyId);

            if (org != null)
            {
                // Alle Räume, auf die der Veranstalter Zugriff hat
                var roomService = new MyStik.TimeTable.Web.Services.RoomService();
                var rooms = roomService.GetRooms(org.Id, true);


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


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Free()
        {
            var from = DateTime.Now;
            var to = from;

            var model = new FreeRoomSummaryModel();
            var org = GetMyOrganisation();

            // Ermittle alle Räume im Zeitraum [von, bis]

            // jeden Raum durchgehen
            // aktuelle Veranstaltung ermitteln
            // letzte Veranstaltung ermitteln, die beendet ist
            // nächste Veranstaltung ermitteln, die in Zukunft beginnen wird


            var roomService = new MyStik.TimeTable.Web.Services.RoomService();
            var fk09 = Db.Organisers.SingleOrDefault(o => o.Id == org.Id);
            model.AvailableRooms = roomService.GetAvaliableRoomsNow(fk09.Id);
            model.FutureRooms = roomService.GetNextAvaliableRoomsNow(fk09.Id);

            model.AvailableRooms = model.AvailableRooms.OrderBy(r => r.Room.Number).ToList();

            ViewBag.Organiser = fk09.ShortName;

            return View("Free", model);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult LookupDate()
        {
            var model = new RoomLookUpModel
            {
                NewDate = DateTime.Today.ToShortDateString(),
                NewBegin = DateTime.Now.TimeOfDay.ToString(),
                NewEnd = DateTime.Now.AddMinutes(90).TimeOfDay.ToString(),
                NewDate2 = DateTime.Today.ToShortDateString(),
            };

            return View(model);
        }

        public PartialViewResult Search(RoomLookUpModel model)
        {
            var sDateStart = DateTime.Parse(model.NewDate);
            var sDateEnd = DateTime.Parse(model.NewDate2);
            var sBegin = TimeSpan.Parse(model.NewBegin);
            var sEnd = TimeSpan.Parse(model.NewEnd);

            var roomService = new MyStik.TimeTable.Web.Services.RoomService();
            var org = GetMyOrganisation();
            var userRight = GetUserRight(User.Identity.Name, org.ShortName);


            if (sDateEnd < sDateStart)
            {
                return PartialView("_ErrorMsg", "Enddatum ist kleiner als Startdatum");
            }

            if (sEnd < sBegin)
            {
                return PartialView("_ErrorMsg", "Ende ist kleiner als Beginn");
            }

            var result = new RoomSearchResultModel();
            result.Begin = sBegin;
            result.End = sEnd;


            // Fallunterscheidungen
            // gleicher Tag
            if (sDateStart == sDateEnd)
            {
                var from = sDateStart.Add(sBegin);
                var until = sDateStart.Add(sEnd);
                var rooms = roomService.GetFreeRooms(org.Id, userRight.IsRoomAdmin, from, until);
                result.Rooms = rooms.ToList();
                result.DayList = new List<DateTime>();
                result.DayList.Add(sDateStart);
                return PartialView("_SearchResultList", result);
            }

            // Zeitraum, ggf. Wochentage berücksichtigen
            // ermittle alle Tage zwischen Beginn und Ende
            var dateList = new List<DateTime>();
            var date = sDateStart;
            if (!model.IsMonday && !model.IsTuesday && !model.IsWednesday &&
                !model.IsThursday && !model.IsFriday && !model.IsSaturday &&
                !model.IsSunday)
            {
                while (date <= sDateEnd)
                {
                    dateList.Add(date);
                    date = date.AddDays(1);
                }
            }
            else
            {
                while (date <= sDateEnd)
                {
                    if (model.IsMonday && date.DayOfWeek == DayOfWeek.Monday)
                        dateList.Add(date);
                    if (model.IsTuesday && date.DayOfWeek == DayOfWeek.Tuesday)
                        dateList.Add(date);
                    if (model.IsWednesday && date.DayOfWeek == DayOfWeek.Wednesday)
                        dateList.Add(date);
                    if (model.IsThursday && date.DayOfWeek == DayOfWeek.Thursday)
                        dateList.Add(date);
                    if (model.IsFriday && date.DayOfWeek == DayOfWeek.Friday)
                        dateList.Add(date);
                    if (model.IsSaturday && date.DayOfWeek == DayOfWeek.Saturday)
                        dateList.Add(date);
                    if (model.IsSunday && date.DayOfWeek == DayOfWeek.Sunday)
                        dateList.Add(date);

                    date = date.AddDays(1);
                }
            }

            var rooms2 = roomService.GetFreeRooms(org.Id, userRight.IsRoomAdmin, dateList, sBegin, sEnd);

            result.DayList = dateList;
            result.Rooms = rooms2.ToList();

            return PartialView("_SearchResultList", result);

        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="date"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        [HttpPost]
        public PartialViewResult RoomListByDate(DateTime date, TimeSpan from, TimeSpan to)
        {
            DateTime start = date.AddHours(from.Hours).AddMinutes(from.Minutes);
            DateTime end = date.AddHours(to.Hours).AddMinutes(to.Minutes);

            var org = GetMyOrganisation();
            var member = MemberService.GetMember(org.Id);
            var isOrgAdmin = member?.IsAdmin ?? false;

            var rooms = new MyStik.TimeTable.Web.Services.RoomService().GetFreeRooms(org.Id, isOrgAdmin, start, end);

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
            var room = Db.Rooms.SingleOrDefault(x => x.Id == model.Id);

            if (ModelState.IsValid)
            {
                if (room != null)
                {
                    room.Name = model.Name;
                    room.Description = model.Description;
                    room.Capacity = model.Capacity;
                    room.HasAccessControl = model.HasAccessControl;
                    room.IsForLearning = model.IsForLearning;
                }

                Db.SaveChanges();

                var org = GetMyOrganisation();
                ViewBag.UserRight = GetUserRight(User.Identity.Name, org.ShortName);
                ViewBag.Organiser = org;

                return RedirectToAction("Details", new { id = room.Id });
            }

            return View(room);
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

            if (room != null)
            {
                var dates = room.Dates.ToList();

                foreach (var date in dates)
                {
                    date.Rooms.Remove(room);
                }

                foreach (var assign in room.Assignments.ToList())
                {
                    Db.RoomAssignments.Remove(assign);

                }


                Db.Rooms.Remove(room);
                Db.SaveChanges();
            }
            
            return RedirectToAction("Index");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult OpenAll()
        {
            var rooms = Db.Rooms.OrderBy(r => r.Number).ToList();

            foreach (var room in rooms)
            {
                foreach (var roomAssignment in room.Assignments)
                {
                    roomAssignment.InternalNeedConfirmation = false;
                }
                
            }

            Db.SaveChanges();

            return RedirectToAction("Index");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Transfer()
        {
            var model = new RoomTransferModel();

            model.StartDate = DateTime.Today.ToShortDateString();
            model.StartTime = "08:00";
            model.EndDate = DateTime.Today.ToShortDateString();
            model.EndTime = "22:00";


            var org = GetMyOrganisation();

            var roomService = new MyStik.TimeTable.Web.Services.RoomService();
            var rooms = roomService.GetRooms(org.Id, true).OrderBy(x => x.Number).ToList();

            ViewBag.SourceRoomId = new SelectList(rooms, "Id", "FullName");
            ViewBag.TargetRoomId = new SelectList(rooms, "Id", "FullName");
            ViewBag.Organiser = org;

            return View(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public PartialViewResult TransferPreview(RoomTransferModel model)
        {
            var sourceRoom = Db.Rooms.SingleOrDefault(r => r.Id == model.SourceRoomId);

            var startDate = DateTime.Parse(model.StartDate);
            var startTime = TimeSpan.Parse(model.StartTime);
            var endDate = DateTime.Parse(model.EndDate);
            var endTime = TimeSpan.Parse(model.EndTime);

            var from = startDate.Add(startTime);
            var until = endDate.Add(endTime);

            var dates = sourceRoom.Dates.Where(x => (x.End > from && x.End <= until) ||
                                                    (x.Begin >= from && x.Begin < until)).OrderBy(x => x.Begin).ToList();


            return PartialView("_TransferPreview", dates);
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
                return RedirectToAction("Rooms", "Organiser");

            var startDate = DateTime.Parse(model.StartDate);
            var startTime = TimeSpan.Parse(model.StartTime);
            var endDate = DateTime.Parse(model.EndDate);
            var endTime = TimeSpan.Parse(model.EndTime);

            var from = startDate.Add(startTime);
            var until = endDate.Add(endTime);

            var dates = sourceRoom.Dates.Where(x => (x.End > from && x.End <= until) ||
                                                    (x.Begin >= from && x.Begin < until)).OrderBy(x => x.Begin).ToList();


            foreach (var date in dates)
            {
                date.Rooms.Remove(sourceRoom);
                date.Rooms.Add(targetRoom);
            }

            Db.SaveChanges();


            return RedirectToAction("Rooms", "Organiser");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
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

                var streamImg = new System.IO.MemoryStream();
                img.Save(streamImg, ImageFormat.Png);
                streamImg.Position = 0;

                var ximg = XImage.FromStream(streamImg);
               

                gfx.DrawImage(ximg, new XPoint(
                    200F,
                    250F));


                float marginLeft = 30F;
                float footerY = 700;
                float lineSpace = 12;

                gfx.DrawString("Powered by", fontFooter, XBrushes.Black, new XPoint(marginLeft, footerY));
                gfx.DrawString("NINE http://nine.wi.hm.edu", fontFooter, XBrushes.Black, new XPoint(marginLeft, footerY+=lineSpace));
                gfx.DrawString("PDFsharp http://www.pdfsharp.net/", fontFooter, XBrushes.Black, new XPoint(marginLeft, footerY += lineSpace));
                gfx.DrawString("Pitchfork.QRGenerator https://github.com/GrabYourPitchforks/Pitchfork.QRGenerator", fontFooter, XBrushes.Black, new XPoint(marginLeft, footerY += lineSpace));

            }

        
            var stream = new MemoryStream();

            doc.Save(stream, false);

            return File(stream, "text/pdf", "Test.pdf");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult List(Guid id)
        {
            var room = Db.Rooms.SingleOrDefault(r => r.Id == id);

            var ac = new Services.ActivityService();

            var model = new List<ActivityDate>();
            if (room != null)
            {
                var futureOccurrenceList =
                    room.Dates.Where(o => o.Begin >= DateTime.Today).OrderBy(o => o.Begin).ToList();

                foreach (var date in futureOccurrenceList)
                {
                    model.Add(date);
                }
            }

            return View(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Assignments()
        {
            var roomService = new MyStik.TimeTable.Web.Services.RoomService();

            // Raum - Organisation - Genehmigung für interne - Genehmigung für Externe
            roomService.AddAssignment("R BG.089", "FK 09", false, true);    // Raum steht FK 09 exklusiv zur Verfügung
            roomService.AddAssignment("T 2.017", "FK 09", true, false);     // Raum steht FK 09 nur teilweise zur Verfügung
            roomService.AddAssignment("R 3.099", "FK 09", true, true);     // Raum soll freigehalten werden


            return View("Admin");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="roomIds"></param>
        /// <param name="orgName"></param>
        /// <param name="needExternal"></param>
        /// <param name="needInternal"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="roomIds"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dateId"></param>
        /// <param name="roomId"></param>
        /// <param name="date"></param>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dateId"></param>
        /// <param name="roomId"></param>
        /// <param name="date"></param>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetStateWeekly(Guid? dateId, Guid roomId, string date, string begin, string end)
        {
            var sDate = DateTime.Parse(date);
            var sBegin = TimeSpan.Parse(begin);
            var sEnd = TimeSpan.Parse(end);


            var room = Db.Rooms.SingleOrDefault(r => r.Id == roomId);

            var semester = SemesterService.GetSemester(DateTime.Today);

            var dayOfWeek = sDate.DayOfWeek;

            var dayList = new SemesterService().GetDays(semester.Id, dayOfWeek);

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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dateId"></param>
        /// <param name="roomId"></param>
        /// <param name="dates"></param>
        /// <returns></returns>
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
                        dayList = semesterService.GetDays(semester.Id, day);
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


        /// <summary>
        /// 
        /// </summary>
        /// <param name="roomId"></param>
        /// <param name="Dates"></param>
        /// <returns></returns>
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
                        dayList = semesterService.GetDays(semester.Id, day);
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="date"></param>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        /// <returns></returns>
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

            var userRight = GetUserRight(org);

            // Alle Räume, auf die der Veranstalter Zugriff hat
            var rooms = roomService.GetAvaliableRooms(org.Id, from, until, userRight.IsRoomAdmin);

            return PartialView("_FreeRoomList", rooms);
        }

        public ActionResult DateList(Guid id)
        {
            var semester = SemesterService.GetSemester(DateTime.Today);

            var roomService = new RoomService();

            var model = roomService.GetRoomSchedule(id, semester);

            return View(model);
        }


        public ActionResult DateListAll(Guid id)
        {
            var model = Db.ActivityDates.Where(x => x.Rooms.Any(r => r.Id == id)).OrderBy(d => d.Begin).ToList();

            return View(model);
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public PartialViewResult UnLockRoom(Guid id)
        {
            var assignment = Db.RoomAssignments.SingleOrDefault(x => x.Id == id);

            var room = assignment.Room;

            if (assignment != null)
            {
                assignment.InternalNeedConfirmation = false;
                Db.SaveChanges();
            }


            return PartialView("_EmptyRow");

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public PartialViewResult LockRoom(Guid id)
        {
            var assignment = Db.RoomAssignments.SingleOrDefault(x => x.Id == id);

            if (assignment != null)
            {
                assignment.InternalNeedConfirmation = true;
                Db.SaveChanges();
            }

            return PartialView("_EmptyRow");
        }

        [HttpPost]
        public PartialViewResult LinkOrganiser(Guid roomId, Guid orgId)
        {
            var room = Db.Rooms.SingleOrDefault(x => x.Id == roomId);
            var org = Db.Organisers.SingleOrDefault(x => x.Id == orgId);

            if (room != null && org != null)
            {
                var assign = room.Assignments.FirstOrDefault(x => x.Organiser.Id == org.Id);
                if (assign == null)
                {
                    assign = new RoomAssignment
                    {
                        ExternalNeedConfirmation = true,
                        InternalNeedConfirmation = true,
                        Room = room,
                        Organiser = org
                    };
                    Db.RoomAssignments.Add(assign);
                    Db.SaveChanges();
                }


                return PartialView("_LinkRow", assign);

            }



            return PartialView("_EmptyRow");
        }



        [HttpPost]
        public PartialViewResult DeleteLink(Guid id)
        {
            var assign = Db.RoomAssignments.SingleOrDefault(x => x.Id == id);

            if (assign != null)
            {
                var room = assign.Room;
                Db.RoomAssignments.Remove(assign);
                Db.SaveChanges();
                return PartialView("_EmptyRow");
            }


            return PartialView("_EmptyRow");
        }


        public ActionResult Labels(Guid? semId, Guid? orgId)
        {
            var semester = SemesterService.GetSemester(semId);
            var nextSemester =  SemesterService.GetNextSemester(semester);
            var org = orgId == null ? GetMyOrganisation() : GetOrganiser(orgId.Value);

            var roomService = new MyStik.TimeTable.Web.Services.RoomService();
            var rooms = roomService.GetRooms(org.Id, true);

            ViewBag.UserRight = GetUserRight(User.Identity.Name, org.ShortName);
            ViewBag.Organiser = org;
            ViewBag.Semester = semester;
            ViewBag.NextSemester = nextSemester;


            return View(rooms);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Create()
        {
            var model = new Room();

            return View(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Create(Room model)
        {
            if (ModelState.IsValid)
            {
                Db.Entry(model).State = EntityState.Added;

                // Verknüpfung anlegen
                var assign = new RoomAssignment
                {
                    ExternalNeedConfirmation = true,
                    InternalNeedConfirmation = true,
                    Room = model,
                    Organiser = GetMyOrganisation()
                };
                Db.RoomAssignments.Add(assign);
                Db.SaveChanges();
            }

            return RedirectToAction("Rooms", "Organiser");
        }

    }

}