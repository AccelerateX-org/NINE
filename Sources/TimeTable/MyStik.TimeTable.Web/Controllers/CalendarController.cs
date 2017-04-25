using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web;
using System.Web.Mvc;
using DDay.iCal;
using DDay.iCal.Serialization;
using log4net;
using Microsoft.AspNet.Identity;
using MyStik.TimeTable.Data;
using MyStik.TimeTable.DataServices;
using MyStik.TimeTable.Web.Helpers;
using MyStik.TimeTable.Web.Models;
using MyStik.TimeTable.Web.Services;

namespace MyStik.TimeTable.Web.Controllers
{
    public class CalendarController : BaseController
    {
        public ActionResult Index()
        {
            var user = UserManager.FindByName(User.Identity.Name);

            ViewBag.CalendarToken = user.Id;
            ViewBag.CalendarPeriod = GetSemester().Name;

            return View();
        }

        private IEnumerable<CalendarEventModel> GetCalendarEvents(IEnumerable<ActivityDateSummary> dates, bool printHosts, bool printRooms, bool useStates=true)
        {
            var events = new List<CalendarEventModel>();

            var datesToDelete = new List<ActivityDate>();

            var semester = GetSemester();

            foreach (var date in dates)
            {
                if (date.Activity != null)
                {
                    // State
                    // wenn Course => Activity
                    // wenn Sprechstunde => Date oder Slot

                    var eventViewModel = new CalenderEventViewModel
                    {
                        Summary = date,
                        State =
                            date.Date.Activity is Course && useStates
                                ? ActivityService.GetActivityState(date.Date.Activity.Occurrence, AppUser, semester)
                                : null,
                    };

                    // Workaround für fullcalendar
                    // wenn der Kalendereintrag ein zu geringe höhe hat,
                    // dann wird statt des Endes der Titel angezeigt
                    // Den Titel rendern wir selber, d.h. i.d.R. geben wir ihn nicht an!
                    // file: fullcalendar.js line 3945
                    var duration = date.Date.End - date.Date.Begin;
                    string title = null;
                    /*
                if (duration.TotalMinutes <= 60)
                    title = date.Date.End.TimeOfDay.ToString(@"hh\:mm");
                 */
                    var sb = new StringBuilder();
                    //sb.Append(eventViewModel.Summary.ShortName);

                    /*
                    if (printRooms)
                    {
                        sb.Append(" (");

                        if (eventViewModel.Summary.Date.Rooms.Any())
                        {
                            foreach (var r in eventViewModel.Summary.Date.Rooms)
                            {
                                sb.Append(r.Number);
                                if (r != eventViewModel.Summary.Date.Rooms.Last())
                                {
                                    sb.Append(", ");
                                }
                            }
                        }
                        else
                        {
                            sb.Append("keine Raumangabe");
                        }
                        sb.Append(")");
                    }
                     */

                    events.Add(new CalendarEventModel
                    {
                        title = sb.ToString(),
                        allDay = false,
                        start = date.Date.Begin.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                        end = date.Date.End.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                        textColor = date.TextColor,
                        backgroundColor = date.BackgroundColor,
                        borderColor = "#000",
                        htmlToolbarInfo = this.RenderViewToString("_CalendarEventToolbarInfo", eventViewModel),
                        htmlToolbar = this.RenderViewToString("_CalendarEventToolbar", eventViewModel),
                        htmlContent = this.RenderViewToString("_CalendarEventContent", eventViewModel),
                    });
                }
                else
                {
                    datesToDelete.Add(date.Date);
                }
            }

            if (datesToDelete.Any())
            {
                // so ein Date darf es nicht geben => löschen
                var db = new TimeTableDbContext();
                foreach (var date in datesToDelete)
                {
                    var dd = db.ActivityDates.SingleOrDefault(d => d.Id == date.Id);
                    dd.Hosts.Clear();
                    dd.Rooms.Clear();
                    db.ActivityDates.Remove(dd);
                }
                db.SaveChanges();
            }

            return events;
        }

        private IEnumerable<CalendarEventModel> GetCalendarEventsMobile(IEnumerable<ActivityDateSummary> dates, bool printHosts, bool printRooms, bool useStates = true)
        {
            var events = new List<CalendarEventModel>();

            var user = AppUser;
            var semester = GetSemester();

            foreach (var date in dates)
            {
                if (date.Activity != null)
                {
                    // State
                    // wenn Course => Activity
                    // wenn Sprechstunde => Date oder Slot

                    var eventViewModel = new CalenderEventViewModel
                    {
                        Summary = date,
                        State =
                            date.Date.Activity is Course && useStates
                                ? ActivityService.GetActivityState(date.Date.Activity.Occurrence, user, semester)
                                : null,
                    };

                    // Workaround für fullcalendar
                    // wenn der Kalendereintrag ein zu geringe höhe hat,
                    // dann wird statt des Endes der Titel angezeigt
                    // Den Titel rendern wir selber, d.h. i.d.R. geben wir ihn nicht an!
                    // file: fullcalendar.js line 3945
                    var duration = date.Date.End - date.Date.Begin;
                    string title = null;
                    var sb = new StringBuilder();
                    //HIER ÄNDERN
                    events.Add(new CalendarEventModel
                    {
                        
                        allDay = false,
                        start = date.Date.Begin.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                        end = date.Date.End.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                        textColor = date.TextColor,
                        backgroundColor = date.BackgroundColor,
                        borderColor = "#000000",
                        htmlToolbarInfo = this.RenderViewToString("_CalendarEventToolbarInfo", eventViewModel),
                        htmlToolbar = this.RenderViewToString("_CalendarEventToolbar", eventViewModel),
                        htmlContent = this.RenderViewToString("_CalendarEventContentMobile", eventViewModel),
                    });
                }
            }

            return events;
        }


        /// <summary>
        /// Umwandlung eines UNIX-Dates in ein DateTimeObjekt
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        /*
        private DateTime GetDateTime(int time)
        {
            return new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(time);
        }
         */

        private DateTime GetDateTime(string time)
        {
            var dt = DateTime.Parse(time);
            return dt;
        }

        
        //
        // GET: /Calendar/
        [HttpPost]
        [AllowAnonymous]
        public JsonResult CourseEventsByProgram(Guid semGroupId, bool showPersonalDates, string start, string end)
        {
            var startDate = GetDateTime(start);
            var endDate = GetDateTime(end);

            var cal = new List<CalendarEventModel>();

            var db = new TimeTableDbContext();

            var dateMap = new Dictionary<Guid, ActivityDateSummary>();

            var user = UserManager.FindByName(User.Identity.Name);

            if (user != null && showPersonalDates)
            {
                // 1. Angebot des angemeldeten Dozentens
                var allDates = db.ActivityDates.Where(c =>
                    c.Begin >= startDate && c.End <= endDate &&
                    c.Hosts.Any(l => !string.IsNullOrEmpty(l.UserId) && l.UserId.Equals(user.Id))).ToList();

                foreach (var date in allDates)
                {
                    dateMap[date.Id] = new ActivityDateSummary(date, ActivityDateType.Offer);
                }


                // 2. die gebuchten
                var myOcs = db.Occurrences.Where(o => o.Subscriptions.Any(s => s.UserId.Equals(user.Id))).ToList();

                var ac = new ActivityService();

                foreach (var occ in myOcs)
                {
                    var summary = ac.GetSummary(occ.Id);

                    var dates = summary.GetDates(startDate, endDate);

                    foreach (var date in dates)
                    {
                        if (!dateMap.ContainsKey(date.Id))
                        {
                            dateMap[date.Id] = new ActivityDateSummary(date, ActivityDateType.Subscription);

                        }
                    }
                }
            }

            // 3. das Suchergebnis
            // das Semester suchen, dass zum Datum passt
            // Grundannahme:  Vorlesungszeiten überlappen sich nicht
            var semester = new SemesterService().GetSemester(startDate, endDate);

            if (semester != null)
            {
                // Daten anzeigen, wenn Booking Enabled oder
                // wenn Benutzer Member der FK09 ist (=Prof, LB, Sekretariat)
                var lookUp = semester.BookingEnabled;
                if (!lookUp)
                {
                    lookUp = new MemberService(Db, UserManager).IsUserMemberOf(User.Identity.Name, "FK 09") ||
                        User.IsInRole("SysAdmin");
                }

                if (lookUp)
                {
                    var courses = db.Activities.Where(c => c.SemesterGroups.Any(g =>
                        g.Id == semGroupId)).ToList();

                    foreach (var course in courses)
                    {
                        var dates = course.Dates.Where(c => c.Begin >= startDate && c.End <= endDate);

                        foreach (var date in dates)
                        {
                            if (!dateMap.ContainsKey(date.Id))
                            {
                                dateMap[date.Id] = new ActivityDateSummary(date, ActivityDateType.SearchResult);
                            }
                        }
                    }
                }
            }

            cal.AddRange(GetCalendarEvents(dateMap.Values.ToList(), true, true));

            return Json(cal);
        }


        [HttpPost]
        public JsonResult CourseEventsByLecturer(Guid dozId, bool showPersonalDates, string start, string end)
        {
            var startDate = GetDateTime(start);
            var endDate = GetDateTime(end);

            var cal = new List<CalendarEventModel>();

            var db = new TimeTableDbContext();

            var user = UserManager.FindByName(User.Identity.Name);

            var dateMap = new Dictionary<Guid, ActivityDateSummary>();

            if (showPersonalDates)
            {
                // 1. Angebot des angemeldeten Dozentens
                var allMyDates = db.ActivityDates.Where(c =>
                    c.Begin >= startDate && c.End <= endDate &&
                    c.Hosts.Any(l => !string.IsNullOrEmpty(l.UserId) && l.UserId.Equals(user.Id))).ToList();

                foreach (var date in allMyDates)
                {
                    dateMap[date.Id] = new ActivityDateSummary(date, ActivityDateType.Offer);
                }

                // 2. die gebuchten
                var myOcs = db.Occurrences.Where(o => o.Subscriptions.Any(s => s.UserId.Equals(user.Id))).ToList();

                var ac = new ActivityService();

                foreach (var occ in myOcs)
                {
                    var summary = ac.GetSummary(occ.Id);

                    var dates = summary.GetDates(startDate, endDate);

                    foreach (var date in dates)
                    {
                        if (!dateMap.ContainsKey(date.Id))
                        {
                            dateMap[date.Id] = new ActivityDateSummary(date, ActivityDateType.Subscription);

                        }
                    }
                }
            }


            // 3. das Suchergebnis
            // das Semester suchen, dass zum Datum passt
            // Grundannahme:  Vorlesungszeiten überlappen sich nicht
            var semester = new SemesterService().GetSemester(startDate, endDate);

            if (semester != null)
            {
                // Daten anzeigen, wenn Booking Enabled oder
                // wenn Benutzer Member der FK09 ist (=Prof, LB, Sekretariat)
                var lookUp = semester.BookingEnabled;
                if (!lookUp)
                {
                    lookUp = new MemberService(Db, UserManager).IsUserMemberOf(User.Identity.Name, "FK 09") ||
                        User.IsInRole("SysAdmin");
                }

                var fk09 = db.Organisers.SingleOrDefault(org => org.ShortName.Equals("FK 09"));

                if (fk09 != null)
                {
                    var lecturer = fk09.Members.SingleOrDefault(l => l.Id == dozId);

                    if (lecturer != null)
                    {
                        var allDates = db.ActivityDates.Where(c =>
                            c.Begin >= startDate && c.End <= endDate &&
                            c.Hosts.Any(oc => oc.Id == lecturer.Id)).ToList();

                        foreach (var date in allDates)
                        {
                            // Sprechstunden immer anzeigen
                            if (lookUp || date.Activity is OfficeHour)
                            {
                                if (!dateMap.ContainsKey(date.Id))
                                {
                                    dateMap[date.Id] = new ActivityDateSummary(date, ActivityDateType.SearchResult);
                                }
                            }
                        }
                    }
                }
            }

            cal.AddRange(GetCalendarEvents(dateMap.Values.ToList(), false, true));

            return Json(cal);
        }

        [HttpPost]
        public JsonResult CourseEventsByRoom(Guid roomId, bool showPersonalDates, string start, string end)
        {
            var startDate = GetDateTime(start);
            var endDate = GetDateTime(end);

            var cal = new List<CalendarEventModel>();

            var db = new TimeTableDbContext();

            var user = UserManager.FindByName(User.Identity.Name);

            var dateMap = new Dictionary<Guid, ActivityDateSummary>();

            if (showPersonalDates)
            {
                // 1. Angebot des angemeldeten Dozentens
                var allMyDates = db.ActivityDates.Where(c =>
                    c.Begin >= startDate && c.End <= endDate &&
                    c.Hosts.Any(l => !string.IsNullOrEmpty(l.UserId) && l.UserId.Equals(user.Id))).ToList();

                foreach (var date in allMyDates)
                {
                    dateMap[date.Id] = new ActivityDateSummary(date, ActivityDateType.Offer);
                }


                // 2. die gebuchten
                var myOcs = db.Occurrences.Where(o => o.Subscriptions.Any(s => s.UserId.Equals(user.Id))).ToList();

                var ac = new ActivityService();

                foreach (var occ in myOcs)
                {
                    var summary = ac.GetSummary(occ.Id);

                    var dates = summary.GetDates(startDate, endDate);

                    foreach (var date in dates)
                    {
                        if (!dateMap.ContainsKey(date.Id))
                        {
                            dateMap[date.Id] = new ActivityDateSummary(date, ActivityDateType.Subscription);

                        }
                    }
                }
            }

            // 3. Suchergebnis
            // das Semester suchen, dass zum Datum passt
            // Grundannahme:  Vorlesungszeiten überlappen sich nicht


            var displayDate = true;
            var semester = new SemesterService().GetSemester(startDate, endDate);
            if (semester != null)
            {
                // Daten anzeigen, wenn Booking Enabled oder
                // wenn Benutzer Member der FK09 ist (=Prof, LB, Sekretariat)
                displayDate = semester.BookingEnabled;
                if (!displayDate)
                {
                    displayDate = new MemberService(Db, UserManager).IsUserMemberOf(User.Identity.Name, "FK 09") ||
                        User.IsInRole("SysAdmin");
                }
            }

            var room = db.Rooms.SingleOrDefault(l => l.Id == roomId);

            if (room != null)
            {
                var allDates = db.ActivityDates.Where(c =>
                    c.Begin >= startDate && c.End <= endDate &&
                    c.Rooms.Any(oc => oc.Id == room.Id)).ToList();

                foreach (var date in allDates)
                {
                    if (!dateMap.ContainsKey(date.Id) && displayDate)
                    {
                        dateMap[date.Id] = new ActivityDateSummary(date, ActivityDateType.SearchResult);
                    }
                }
            }

            cal.AddRange(GetCalendarEvents(dateMap.Values.ToList(), true, false));

            return Json(cal);
        }

        [HttpPost]
        public JsonResult ActivityPlan(string start, string end)
        {
            var startDate = GetDateTime(start);
            var endDate = GetDateTime(end);

            return Json(
                GetCalendarEvents(
                    GetActivityPlan(User.Identity.Name, startDate, endDate),
                    true, true));
        }

        [HttpPost]
        public JsonResult ActivityPrintPlan(Guid dozId, string start, string end)
        {
            var startDate = GetDateTime(start);
            var endDate = GetDateTime(end);

            var events = new List<CalendarEventModel>();

            // start und end sind "echte" Daten, d.h. eine Woche

            // Am Schluss muss alles in "Wochentage" umgerechnet werden

            // Alle Events abstrakt nah Wochentag
            var semester = GetSemester();

            // Alle Vorlesungen
            var courses =
                Db.Activities.OfType<Course>()
                    .Where(c => 
                        c.SemesterGroups.Any(g => g.Semester.Id == semester.Id) &&
                        c.Dates.Any(oc => oc.Hosts.Any(l => l.Id == dozId)))
                    .ToList();

            var activityService = new ActivityService();
            foreach (var course in courses)
            {
                var days = (from occ in course.Dates
                    select
                        new
                        {
                            Day = occ.Begin.DayOfWeek,
                            Begin = occ.Begin.TimeOfDay,
                            End = occ.End.TimeOfDay,
                        }).Distinct().ToList();

                var eventViewModel = new CalenderEventPrintViewModel();
                eventViewModel.Course = course;
                eventViewModel.Rooms = Db.Rooms.Where(x => x.Dates.Any(y => y.Activity.Id == course.Id)).Distinct().ToList();


                if (days.Count == 1)
                {
                    var day = days.First();

                    var calDay = startDate.AddDays((int) day.Day - (int) startDate.DayOfWeek);
                    var calBegin = calDay.Add(day.Begin);
                    var calEnd = calDay.Add(day.End);

                    // Einfacher Eintrag
                    events.Add(new CalendarEventModel
                    {
                        title = course.Name,
                        allDay = false,
                        start = calBegin.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                        end = calEnd.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                        textColor = "#000",
                        backgroundColor = "#fff",
                        borderColor = "#000",
                        //htmlToolbarInfo = this.RenderViewToString("_CalendarEventToolbarInfo", eventViewModel),
                        //htmlToolbar = this.RenderViewToString("_CalendarEventToolbar", eventViewModel),
                        htmlContent = this.RenderViewToString("_CalendarEventContentPrint", eventViewModel),
                    });

                }
                else
                {
                    foreach (var day in days)
                    {
                        var calDay = startDate.AddDays((int)day.Day - (int)startDate.DayOfWeek);
                        var calBegin = calDay.Add(day.Begin);
                        var calEnd = calDay.Add(day.End);

                        // Einfacher Eintrag
                        events.Add(new CalendarEventModel
                        {
                            title = course.Name,
                            allDay = false,
                            start = calBegin.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                            end = calEnd.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                            textColor = "#000",
                            backgroundColor = "#fff",
                            borderColor = "#000",
                            //htmlToolbarInfo = this.RenderViewToString("_CalendarEventToolbarInfo", eventViewModel),
                            //htmlToolbar = this.RenderViewToString("_CalendarEventToolbar", eventViewModel),
                            htmlContent = this.RenderViewToString("_CalendarEventContentPrint", eventViewModel),
                        });
                    }
                }
            }

            
            
            return Json(events);
        }


        [HttpPost]
        public JsonResult ActivityPlanMobile(string start, string end)
        {
            var startDate = GetDateTime(start);
            var endDate = GetDateTime(end);

            return Json(
                GetCalendarEventsMobile(
                    GetActivityPlan(User.Identity.Name, startDate, endDate),
                    true, true));
        }


        [HttpPost]
        public JsonResult DailyRota(string start, string end)
        {
            var startDate = GetDateTime(start);
            var endDate = GetDateTime(end);

            var db = new TimeTableDbContext();

            var allDates = db.ActivityDates.Where(c =>
                c.Begin >= startDate && c.End <= endDate).ToList();

            var eventDates = new List<ActivityDateSummary>();

            foreach (var date in allDates)
            {
                eventDates.Add(new ActivityDateSummary
                {
                    Date = date,
                    DateType = ActivityDateType.SearchResult,
                });
            }

            /*
             * wenn man eigene events bauen möchte
            var events = new List<CalendarEventModel>();

            foreach (var date in allDates)
            {
                events.Add(new CalendarEventModel
                {
                    title = sb.ToString(),
                    allDay = false,
                    start = date.Date.Begin.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                    end = date.Date.End.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                    textColor = date.TextColor,
                    backgroundColor = date.BackgroundColor,
                    borderColor = "#000",
                    htmlToolbar = this.RenderViewToString("_CalendarEventToolbar", eventViewModel),
                    htmlContent = this.RenderViewToString("_CalendarEventContent", eventViewModel),
                });
            }
             */


            return Json(
                GetCalendarEvents(
                    eventDates,
                    true, true, false));
        }

        [HttpPost]
        public JsonResult RoomPrintPlan(Guid roomId, string start, string end)
        {
            var startDate = GetDateTime(start);
            var endDate = GetDateTime(end);

            var events = new List<CalendarEventModel>();

            // start und end sind "echte" Daten, d.h. eine Woche

            // Am Schluss muss alles in "Wochentage" umgerechnet werden

            // Alle Events abstrakt nah Wochentag
            var semester = GetSemester();

            // Alle Vorlesungen
            var courses =
                Db.Activities.OfType<Course>()
                    .Where(c =>
                        c.SemesterGroups.Any(g => g.Semester.Id == semester.Id) &&
                        c.Dates.Any(oc => oc.Rooms.Any(l => l.Id == roomId)))
                    .ToList();

            var activityService = new ActivityService();
            foreach (var course in courses)
            {
                var days = (from occ in course.Dates
                            select
                                new
                                {
                                    Day = occ.Begin.DayOfWeek,
                                    Begin = occ.Begin.TimeOfDay,
                                    End = occ.End.TimeOfDay,
                                }).Distinct().ToList();

                if (days.Count == 1)
                {
                    var day = days.First();

                    var calDay = startDate.AddDays((int)day.Day - (int)startDate.DayOfWeek);
                    var calBegin = calDay.Add(day.Begin);
                    var calEnd = calDay.Add(day.End);

                    // Einfacher Eintrag
                    events.Add(new CalendarEventModel
                    {
                        title = course.Name,
                        allDay = false,
                        start = calBegin.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                        end = calEnd.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                        textColor = "#000",
                        backgroundColor = "#fff",
                        borderColor = "#000",
                        //htmlToolbarInfo = this.RenderViewToString("_CalendarEventToolbarInfo", eventViewModel),
                        //htmlToolbar = this.RenderViewToString("_CalendarEventToolbar", eventViewModel),
                        //htmlContent = this.RenderViewToString("_CalendarEventContent", eventViewModel),
                    });

                }
                else
                {
                    foreach (var day in days)
                    {
                        var calDay = startDate.AddDays((int)day.Day - (int)startDate.DayOfWeek);
                        var calBegin = calDay.Add(day.Begin);
                        var calEnd = calDay.Add(day.End);

                        // Einfacher Eintrag
                        events.Add(new CalendarEventModel
                        {
                            title = course.Name,
                            allDay = false,
                            start = calBegin.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                            end = calEnd.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                            textColor = "#000",
                            backgroundColor = "#fff",
                            borderColor = "#000",
                            //htmlToolbarInfo = this.RenderViewToString("_CalendarEventToolbarInfo", eventViewModel),
                            //htmlToolbar = this.RenderViewToString("_CalendarEventToolbar", eventViewModel),
                            //htmlContent = this.RenderViewToString("_CalendarEventContent", eventViewModel),
                        });
                    }
                }
            }



            return Json(events);
        }



        private IEnumerable<ActivityDateSummary>  GetActivityPlan(string userName, DateTime startDate, DateTime endDate)
        {
            var dateMap = new Dictionary<Guid, ActivityDateSummary>();

            var db = new TimeTableDbContext();

            var user = UserManager.FindByName(userName);

            if (user != null)
            {
                // 1. Angebot des angemeldeten Dozentens
                /*
                var doz = db.Members.SingleOrDefault(m => m.UserId == user.Id);

                if (doz != null)
                {
                    var allDates = db.ActivityDates.Where(c =>
                        c.Begin >= startDate && c.End <= endDate &&
                        c.Hosts.Any(oc => oc.ShortName.Equals(doz.ShortName))).ToList();

                    foreach (var date in allDates)
                    {
                        dateMap[date.Id] = new ActivityDateSummary(date, ActivityDateType.Offer);
                    }
                }
                 */
                var allMyDates = db.ActivityDates.Where(c =>
                    c.Begin >= startDate && c.End <= endDate &&
                    c.Hosts.Any(l => !string.IsNullOrEmpty(l.UserId) && l.UserId.Equals(user.Id))).ToList();

                foreach (var date in allMyDates)
                {
                    dateMap[date.Id] = new ActivityDateSummary(date, ActivityDateType.Offer);
                }

                // 2. die gebuchten
                var myOcs = db.Occurrences.Where(o => o.Subscriptions.Any(s => s.UserId.Equals(user.Id))).ToList();

                var ac = new ActivityService();

                foreach (var occ in myOcs)
                {
                    var summary = ac.GetSummary(occ.Id);

                    var dates = summary.GetDates(startDate, endDate);

                    foreach (var date in dates)
                    {
                        if (!dateMap.ContainsKey(date.Id))
                        {
                            dateMap[date.Id] = new ActivityDateSummary(date, ActivityDateType.Subscription);

                        }
                    }
                }
            }

            return dateMap.Values.ToList();
        }

        public FileResult File()
        {
            var user = UserManager.FindByName(User.Identity.Name);

            if (user != null)
            {
                return GetCalendarData(user);
            }
            return null;
        }

        private FileResult GetCalendarData(ApplicationUser user)
        {
            // hier wirklich nur die freigegebenen Semester!
            var semester = GetSemester(user);

            var dateList = GetActivityPlan(user.UserName, semester.StartCourses, semester.EndCourses.AddDays(1));

            var iCal = new DDay.iCal.iCalendar();

            var now = GlobalSettings.Now;

            foreach (var date in dateList)
            {
                var evt = iCal.Create<DDay.iCal.Event>(); 
                evt.Summary = date.Name;
                if (date.Date.Occurrence.IsCanceled)
                {
                    evt.Summary += " - abgesagt!";
                }

                if (date.Date.Activity is OfficeHour && user.MemberState == MemberState.Staff)
                {
                    int n = 0;
                    n += date.Date.Occurrence.Subscriptions.Count;
                    n += date.Date.Slots.Sum(slot => slot.Occurrence.Subscriptions.Count);

                    if (n > 0)
                    {
                        evt.Summary += string.Format(" - {0} Eintragungen", n);
                    }
                    else
                    {
                        evt.Summary += " - Keine Eintragungen";
                    }
                }

                evt.Description = date.Date.Description;

                // Die Endung Z macht, dass die Daten im Google-Calender richtig angezeigt werden
                evt.DTStart = new DDay.iCal.iCalDateTime(date.Date.Begin.ToUniversalTime().ToString(@"yyyyMMdd\THHmmss\Z"));
                evt.DTEnd = new DDay.iCal.iCalDateTime(date.Date.End.ToUniversalTime().ToString(@"yyyyMMdd\THHmmss\Z"));

                evt.Status = date.Date.Occurrence.IsCanceled ? EventStatus.Cancelled : EventStatus.Confirmed;

                var sb = new StringBuilder();
                foreach (var room in date.Date.Rooms)
                {
                    sb.Append(room.Number);
                    if (room != date.Date.Rooms.Last())
                        sb.Append(", ");
                }

                evt.Location = sb.ToString();
                evt.Categories.Add(date.Controller);
                evt.IsAllDay = false;
            }

            ISerializationContext ctx = new SerializationContext();
            ISerializerFactory factory = new DDay.iCal.Serialization.iCalendar.SerializerFactory();
            IStringSerializer serializer = factory.Build(iCal.GetType(), ctx) as IStringSerializer;

            string output = serializer.SerializeToString(iCal);
            var contentType = "text/calendar";
            var bytes = Encoding.UTF8.GetBytes(output);

            return File(bytes, contentType, "nine.ics");
        }

        [AllowAnonymous]
        public FileResult Feed(string token)
        {
            var user = UserManager.FindById(token);

            if (user != null)
            {
                var logger = LogManager.GetLogger("iCal");
                logger.DebugFormat("Feed for [{0}]", user.UserName);
                return GetCalendarData(user);
            }

            return null;
        }


        [AllowAnonymous]
        public FileResult Feed2(string token)
        {
            var user = UserManager.FindById(token);

            if (user != null)
            {
                return GetCalendarData2(user);
            }

            return null;
        }


        private FileResult GetCalendarData2(ApplicationUser user)
        {
            // hier wirklich nur die freigegebenen Semester!
            var semester = GetSemester();

            var dateList = GetActivityPlan(user.UserName, semester.StartCourses, semester.EndCourses.AddDays(1));

            var iCal = new DDay.iCal.iCalendar();
            //iCal.AddTimeZone(new iCalTimeZone() {TZID = "Europe/Berlin"});
            // http://en.wikipedia.org/wiki/TZID

            var now = GlobalSettings.Now;

            foreach (var date in dateList)
            {
                var evt = iCal.Create<DDay.iCal.Event>();
                evt.Summary = date.Name;
                evt.Description = date.Date.Description;

                /* Mit Zeitzone => hat bisher nix gebracht
                evt.DTStart = new DDay.iCal.iCalDateTime(date.Date.Begin, "Europe/Berlin");
                evt.DTEnd = new DDay.iCal.iCalDateTime(date.Date.End, "Europe/Berlin");
                evt.UID = date.Id;
                evt.DTStamp = new DDay.iCal.iCalDateTime(now, "Europe/Berlin");
                 */


                evt.DTStart = new DDay.iCal.iCalDateTime(date.Date.Begin.ToUniversalTime().ToString(@"yyyyMMdd\THHmmss\Z"));
                evt.DTEnd = new DDay.iCal.iCalDateTime(date.Date.End.ToUniversalTime().ToString(@"yyyyMMdd\THHmmss\Z"));

                evt.Status = date.Date.Occurrence.IsCanceled ? EventStatus.Cancelled : EventStatus.Confirmed;

                evt.Transparency = TransparencyType.Opaque;

                evt.Alarms.Add(new Alarm()
                {
                    Duration = new TimeSpan(0, 5, 0),
                    Repeat = 5,
                    Summary = date.Name,
                    Action = AlarmAction.Display,
                    Trigger = new Trigger(new TimeSpan(0, 0, 0)),
                });

                var sb = new StringBuilder();
                foreach (var room in date.Date.Rooms)
                {
                    sb.Append(room.Number);
                    if (room != date.Date.Rooms.Last())
                        sb.Append(", ");
                }

                evt.Location = sb.ToString();
                evt.Categories.Add(date.Controller);
                evt.IsAllDay = false;
            }

            ISerializationContext ctx = new SerializationContext();
            ISerializerFactory factory = new DDay.iCal.Serialization.iCalendar.SerializerFactory();
            IStringSerializer serializer = factory.Build(iCal.GetType(), ctx) as IStringSerializer;

            string output = serializer.SerializeToString(iCal);
            var contentType = "text/calendar";
            var bytes = Encoding.UTF8.GetBytes(output);

            return File(bytes, contentType, "nine.ics");
        }

    
    }
}