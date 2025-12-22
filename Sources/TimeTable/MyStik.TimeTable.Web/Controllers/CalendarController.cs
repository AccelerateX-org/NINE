using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Ical.Net;
using Ical.Net.CalendarComponents;
using Ical.Net.DataTypes;
using Ical.Net.Serialization;
using log4net;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity;
using MyStik.TimeTable.Data;
using MyStik.TimeTable.Web.Helpers;
using MyStik.TimeTable.Web.Models;
using MyStik.TimeTable.Web.Services;
using MyStik.TimeTable.Web.Utils.Helper;
using RoomService = MyStik.TimeTable.Web.Services.RoomService;

namespace MyStik.TimeTable.Web.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public class CalendarController : BaseController
    {
        private string _calDateFormatCalendar = "yyyy-MM-ddTHH:mm:ss";


        public ActionResult Index()
        {

#if DEBUG
            var day = new DateTime(2020, 4, 21);
#else
            var day = DateTime.Today;
#endif

            var model = GetMyCalendar(day);

            return View("Index", model);
        }

        public ActionResult Today()
        {
            var day = DateTime.Today;

            var model = GetMyCalendar(day);

            return View("Index", model);
        }
        public ActionResult Tomorrow()
        {
            var day = DateTime.Today.AddDays(1);

            var model = GetMyCalendar(day);

            return View("Index", model);
        }


        private CalendarMyDayModel GetMyCalendar(DateTime day)
        {
            var user = GetCurrentUser();
            var currentSemester = SemesterService.GetSemester(day);
            var begin = day;
            var end = begin.AddDays(1);


            var dateList = new List<CalendarMyDayDateModel>();

            var subscriptionDatess =
                Db.ActivityDates.Where(d =>
                    (d.Activity.Occurrence.Subscriptions.Any(s => s.UserId.Equals(user.Id)) ||
                     d.Occurrence.Subscriptions.Any(s => s.UserId.Equals(user.Id)) ||
                     d.Slots.Any(slot => slot.Occurrence.Subscriptions.Any(s => s.UserId.Equals(user.Id)))) &&
                    d.End >= begin && d.End <= end).OrderBy(d => d.Begin).ToList();

            foreach (var date in subscriptionDatess)
            {
                var slot = date.Slots.FirstOrDefault(x =>
                    x.Occurrence.Subscriptions.Any(s => s.UserId.Equals(user.Id)));

                var subscription = 
                    slot != null ?
                        slot.Occurrence.Subscriptions.FirstOrDefault(x => x.UserId.Equals(user.Id)) :
                        date.Occurrence.Subscriptions.FirstOrDefault(x => x.UserId.Equals(user.Id));


                var mDate = new CalendarMyDayDateModel
                {
                    Date = date,
                    Slot = slot,
                    Subscription = subscription,
                    IsHost = false
                };

                dateList.Add(mDate);
            }


            var lectureDates =
                Db.ActivityDates.Where(d =>
                    d.Hosts.Any(l => !string.IsNullOrEmpty(l.UserId) && l.UserId.Equals(user.Id)) &&
                    d.End >= begin && d.End <= end).OrderBy(d => d.Begin).ToList();


            foreach (var date in lectureDates)
            {
                var mDate = dateList.SingleOrDefault(x => x.Date.Id == date.Id);

                if (mDate == null)
                {
                    mDate = new CalendarMyDayDateModel
                    {
                        Date = date,
                        IsHost = true
                    };

                    dateList.Add(mDate);
                }
                else
                {
                    mDate.IsHost = true;
                }
            }


            var model = new CalendarMyDayModel
            {
                CurrentSemester = currentSemester,
                Day = day,
                Dates = dateList.OrderBy(x => x.Date.Begin).ToList()
            };

            return model;
        }



        private IEnumerable<CalendarEventModel> GetCalendarEvents(IEnumerable<ActivityDateSummary> dates, bool showPersonalDates)
        {
            var events = new List<CalendarEventModel>();

            var datesToDelete = new List<ActivityDate>();

            foreach (var date in dates)
            {
                if (date.Activity != null)
                {
                    // State
                    // wenn Course => Activity
                    // wenn Sprechstunde => Date oder Slot

                    var eventViewModel = new CalenderDateEventViewModel
                    {
                        Summary = date,
                        State =
                            date.Date.Activity is Course
                                ? ActivityService.GetActivityState(date.Date.Activity.Occurrence, AppUser)
                                : null,
                        Lottery = date.Activity.Occurrence != null 
                                    ? Db.Lotteries.FirstOrDefault(x => x.Occurrences.Any(y => y.Id == date.Activity.Occurrence.Id))
                                    : null
                    };

                    if (showPersonalDates && eventViewModel.State != null)
                    {
                        date.Subscription = eventViewModel.State.Subscription;
                    }


                    // Workaround für fullcalendar
                    // wenn der Kalendereintrag ein zu geringe höhe hat,
                    // dann wird statt des Endes der Titel angezeigt
                    // Den Titel rendern wir selber, d.h. i.d.R. geben wir ihn nicht an!
                    // file: fullcalendar.js line 3945
                    /*
                    var duration = date.Date.End - date.Date.Begin;
                        if (duration.TotalMinutes <= 60)
                            string title = null;
                            title = date.Date.End.TimeOfDay.ToString(@"hh\:mm");
                     */

                    if (date.Slot!=null)
                    {
                        events.Add(new CalendarEventModel
                        {
                            id = date.Date.Id.ToString(),
                            courseId = date.Activity.Id.ToString(),
                            title = string.Empty,
                            allDay = false,
                            start = date.Date.Begin.ToString(_calDateFormatCalendar),
                            end = date.Date.End.ToString(_calDateFormatCalendar),
                            textColor = date.TextColor,
                            backgroundColor = date.BackgroundColor,
                            borderColor = "#ff0000",
                            htmlContent = this.RenderViewToString("_CalendarDateEventContent", eventViewModel),
                        });
                    }
                    else
                    {
                        var sbr = new StringBuilder();
                        sbr.Append(date.Activity.ShortName);
                        if (date.Date.Rooms.Any())
                        {
                            if (date.Date.Rooms.Count() == 1)
                            {
                                sbr.AppendFormat(" ({0})", date.Date.Rooms.First().Number);
                            }
                            else
                            {
                                sbr.AppendFormat(" ({0}, ...)", date.Date.Rooms.First().Number);
                            }
                        }

                        events.Add(new CalendarEventModel
                        {
                            id = date.Date.Id.ToString(),
                            title = sbr.ToString(),
                            allDay = false,
                            start = date.Date.Begin.ToString(_calDateFormatCalendar),
                            end = date.Date.End.ToString(_calDateFormatCalendar),
                            textColor = date.TextColor,
                            backgroundColor = date.BackgroundColor,
                            borderColor = "#000",
                            courseId = date.Activity.Id.ToString(),
                            htmlContent = string.Empty // this.RenderViewToString("_CalendarDateEventContent", eventViewModel),
                        });
                    }
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

        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="semGroupId"></param>
        /// <param name="topicId"></param>
        /// <param name="showPersonalDates"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        public JsonResult GroupDayPlan(Guid semGroupId, Guid? topicId, bool showPersonalDates, string start, string end)
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
            var semGroup = Db.SemesterGroups.SingleOrDefault(x => x.Id == semGroupId);

            if (semGroup != null && user != null)
            {
                var courses = semGroup.Activities.ToList();
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

            cal.AddRange(GetCalendarEvents(dateMap.Values.ToList(), showPersonalDates));

            return Json(cal);
        }


        [HttpPost]
        [AllowAnonymous]
        public JsonResult LabelEvents(string start, string end, Guid semId, Guid? orgId, Guid labelId, Guid? currId)
        {
            var startDate = GetDateTime(start);
            var endDate = GetDateTime(end);


            var semester = SemesterService.GetSemester(semId);
            var label = Db.ItemLabels.SingleOrDefault(x => x.Id == labelId);

            var courses = new List<Course>();

            if (orgId == null && currId == null)
            {
                courses.AddRange(Db.Activities.OfType<Course>()
                    .Where(x =>
                        x.Semester.Id == semester.Id &&
                        x.LabelSet != null &&
                        x.LabelSet.ItemLabels.Any(l => l.Id == label.Id))
                    .ToList());
            }
            else
            {
                var org = GetOrganiser(orgId.Value);
                var curr = Db.Curricula.SingleOrDefault(x => x.Id == currId);

                courses.AddRange(Db.Activities.OfType<Course>()
                    .Where(x =>
                        x.Semester.Id == semester.Id &&
                        x.Organiser.Id == org.Id &&
                        x.LabelSet != null &&
                        x.LabelSet.ItemLabels.Any(l => l.Id == label.Id))
                    .ToList());
            }


            var cs = new CourseService();
            var courseSummaries = new List<CourseSummaryModel>();

            foreach (var labeledCourse in courses.OrderBy(g => g.ShortName))
            {
                courseSummaries.Add(cs.GetCourseSummary(labeledCourse));
            }


            var cal = new List<CalendarEventModel>();
            var dateMap = new Dictionary<Guid, ActivityDateSummary>();

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

            cal.AddRange(GetCalendarEvents(dateMap.Values.ToList(), false));

            return Json(cal);
        }


        [HttpPost]
        [AllowAnonymous]
        public JsonResult LabelEventsWeekly(string start, string end, Guid semId, Guid? segId, Guid? orgId, Guid labelId, Guid? currId, string color, bool isDraggable = false)
        {
            var startDate = GetDateTime(start);
            var endDate = GetDateTime(end);

            var courseService = new CourseService(Db);

            var bgColor = string.IsNullOrEmpty(color) ? "#47aba1" : color;

            // mandatory parameters
            var semester = SemesterService.GetSemester(semId);
            var label = Db.ItemLabels.SingleOrDefault(x => x.Id == labelId);

            // optional parameters
            var segment = segId != null ? semester.Dates.SingleOrDefault(x => x.Id == segId.Value) : null;
            var org = orgId != null ? GetOrganiser(orgId.Value) : null;
            var curr = currId != null ? Db.Curricula.SingleOrDefault(x => x.Id == currId.Value) : null;

            var courses = new List<Course>();

            if (org == null && curr == null)
            {
                if (segment != null)
                {
                    courses.AddRange(Db.Activities.OfType<Course>()
                        .Where(x =>
                            x.Semester.Id == semester.Id &&
                            x.Segment != null && x.Segment.Id == segment.Id &&
                            x.LabelSet != null &&
                            x.LabelSet.ItemLabels.Any(l => l.Id == label.Id))
                        .ToList());
                }
                else
                {
                    courses.AddRange(Db.Activities.OfType<Course>()
                        .Where(x =>
                            x.Semester.Id == semester.Id &&
                            x.LabelSet != null &&
                            x.LabelSet.ItemLabels.Any(l => l.Id == label.Id))
                        .ToList());
                }
            }
            else
            {
                if (segment != null)
                {
                    if (org != null)
                    {
                        courses.AddRange(Db.Activities.OfType<Course>()
                            .Where(x =>
                                x.Semester.Id == semester.Id &&
                                x.Segment != null && x.Segment.Id == segment.Id &&
                                x.Organiser.Id == org.Id &&
                                x.LabelSet != null &&
                                x.LabelSet.ItemLabels.Any(l => l.Id == label.Id))
                            .ToList());
                    }
                    else
                    {
                        courses.AddRange(Db.Activities.OfType<Course>()
                            .Where(x =>
                                x.Semester.Id == semester.Id &&
                                x.Segment != null && x.Segment.Id == segment.Id &&
                                x.LabelSet != null &&
                                x.LabelSet.ItemLabels.Any(l => l.Id == label.Id))
                            .ToList());
                    }
                }
                else
                {
                    if (org != null)
                    {
                        courses.AddRange(Db.Activities.OfType<Course>()
                            .Where(x =>
                                x.Semester.Id == semester.Id &&
                                x.Organiser.Id == org.Id &&
                                x.LabelSet != null &&
                                x.LabelSet.ItemLabels.Any(l => l.Id == label.Id))
                            .ToList());
                    }
                    else
                    {
                        courses.AddRange(Db.Activities.OfType<Course>()
                            .Where(x =>
                                x.Semester.Id == semester.Id &&
                                x.LabelSet != null &&
                                x.LabelSet.ItemLabels.Any(l => l.Id == label.Id))
                            .ToList());
                    }
                }
            }


            var cs = new CourseService();
            var courseSummaries = new List<CourseSummaryModel>();

            foreach (var labeledCourse in courses.OrderBy(g => g.ShortName))
            {
                courseSummaries.Add(cs.GetCourseSummary(labeledCourse));
            }


            var cal = new List<CalendarEventModel>();

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

                var eventViewModel = new CalenderCourseEventViewModel();
                var summary = courseService.GetCourseSummary(course);
                eventViewModel.CourseSummary = summary;

                foreach (var day in days)
                {
                    var calDay = startDate.AddDays((int)day.Day - (int)startDate.DayOfWeek);
                    var calBegin = calDay.Add(day.Begin);
                    var calEnd = calDay.Add(day.End);

                    var sbr = new StringBuilder();
                    sbr.Append(course.ShortName);
                    if (summary.Rooms.Any())
                    {
                        if (summary.Rooms.Count() == 1)
                        {
                            sbr.AppendFormat(" ({0})", summary.Rooms.First().Number);
                        }
                        else
                        {
                            sbr.AppendFormat(" ({0}, ...)", summary.Rooms.First().Number);
                        }
                    }

                    var calEvent = new CalendarEventModel
                    {
                        id = course.Id.ToString(),
                        title = sbr.ToString(),
                        allDay = false,
                        start = calBegin.ToString(_calDateFormatCalendar),
                        end = calEnd.ToString(_calDateFormatCalendar),
                        textColor = "#000",
                        backgroundColor = bgColor,
                        borderColor = "#000",
                        courseId = course.Id.ToString(),
                        htmlContent = string.Empty,   //  this.RenderViewToString("_CalendarCourseEventContent", eventViewModel)
                        editable = isDraggable
                    };

                    cal.Add(calEvent);
                }

            }

            return Json(cal);
        }


        [HttpPost]
        [AllowAnonymous]
        public JsonResult SlotEvents(string start, string end, Guid semId, Guid orgId, Guid slotId, Guid currId)
        {
            var startDate = GetDateTime(start);
            var endDate = GetDateTime(end);


            var semester = SemesterService.GetSemester(semId);
            var org = GetOrganiser(orgId);
            var slot = Db.CurriculumSlots.SingleOrDefault(x => x.Id == slotId);
            var curr = Db.Curricula.SingleOrDefault(x => x.Id == currId);

            var courses = Db.Activities.OfType<Course>()
                .Where(x =>
                    x.Semester.Id == semester.Id &&
                    x.Organiser.Id == org.Id &&
                    x.SubjectTeachings.Any(t => t.Subject.SubjectAccreditations.Any(a => a.Slot.Id == slot.Id)))
                .ToList();

            var cs = new CourseService();
            var courseSummaries = new List<CourseSummaryModel>();

            foreach (var labeledCourse in courses.OrderBy(g => g.ShortName))
            {
                courseSummaries.Add(cs.GetCourseSummary(labeledCourse));
            }


            var cal = new List<CalendarEventModel>();
            var dateMap = new Dictionary<Guid, ActivityDateSummary>();

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

            cal.AddRange(GetCalendarEvents(dateMap.Values.ToList(), false));

            return Json(cal);

        }


        [HttpPost]
        [AllowAnonymous]
        public JsonResult SlotEventsWeekly(string start, string end, Guid semId, Guid orgId, Guid slotId, Guid currId)
        {
            var startDate = GetDateTime(start);
            var endDate = GetDateTime(end);


            var semester = SemesterService.GetSemester(semId);
            var org = GetOrganiser(orgId);
            var slot = Db.CurriculumSlots.SingleOrDefault(x => x.Id == slotId);
            var curr = Db.Curricula.SingleOrDefault(x => x.Id == currId);

            var courses = Db.Activities.OfType<Course>()
                .Where(x =>
                    x.Semester.Id == semester.Id &&
                    x.Organiser.Id == org.Id &&
                    x.SubjectTeachings.Any(t => t.Subject.SubjectAccreditations.Any(a => a.Slot.Id == slot.Id)))
                .ToList();

            var cs = new CourseService();
            var courseSummaries = new List<CourseSummaryModel>();

            foreach (var labeledCourse in courses.OrderBy(g => g.ShortName))
            {
                courseSummaries.Add(cs.GetCourseSummary(labeledCourse));
            }


            var cal = new List<CalendarEventModel>();
            var dateMap = new Dictionary<Guid, ActivityDateSummary>();

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

            cal.AddRange(GetCalendarEvents(dateMap.Values.ToList(), false));

            return Json(cal);

        }



        [HttpPost]
        [AllowAnonymous]
        public JsonResult SlotSemesterEvents(string start, string end, Guid semId, Guid orgId, Guid currId, string slotIds)
        {
            var startDate = GetDateTime(start);
            var endDate = GetDateTime(end);


            var semester = SemesterService.GetSemester(semId);
            var org = GetOrganiser(orgId);
            var curr = Db.Curricula.SingleOrDefault(x => x.Id == currId);

            var cal = new List<CalendarEventModel>();
            var dateMap = new Dictionary<Guid, ActivityDateSummary>();


            if (!string.IsNullOrEmpty(slotIds))
            {
                var slotList = slotIds.Split(';');
                foreach (var ddd in slotList)
                {
                    var slotId = Guid.Parse(ddd);
                    var slot = Db.CurriculumSlots.SingleOrDefault(x => x.Id == slotId);

                    var courses = Db.Activities.OfType<Course>()
                        .Where(x =>
                            x.Semester.Id == semester.Id &&
                            x.Organiser.Id == org.Id &&
                            x.SubjectTeachings.Any(t => t.Subject.SubjectAccreditations.Any(a => a.Slot.Id == slot.Id)))
                        .ToList();

                    var cs = new CourseService();
                    var courseSummaries = new List<CourseSummaryModel>();

                    foreach (var labeledCourse in courses.OrderBy(g => g.ShortName))
                    {
                        courseSummaries.Add(cs.GetCourseSummary(labeledCourse));
                    }


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

                cal.AddRange(GetCalendarEvents(dateMap.Values.ToList(), false));
            }

            return Json(cal);

        }




        [HttpPost]
        public JsonResult CatalogEvents(string start, string end, Guid semId, Guid catId, Guid? currId, Guid? labelId)
        {
            var startDate = GetDateTime(start);
            var endDate = GetDateTime(end);

            var semester = SemesterService.GetSemester(semId);
            var catalog = Db.CurriculumModuleCatalogs.SingleOrDefault(x => x.Id == catId);

            var allCourses = Db.Activities.OfType<Course>()
                .Where(x =>
                    x.Semester.Id == semester.Id && x.Organiser.Id == catalog.Organiser.Id &&
                    x.SubjectTeachings.Any(t => t.Subject.Module.Catalog.Id == catalog.Id))
                .ToList();

            if (currId.HasValue)
            {
                var curr = Db.Curricula.SingleOrDefault(x => x.Id == currId.Value);
                if (curr != null)
                {
                    allCourses =
                        allCourses
                            .Where(x => x.SubjectTeachings.Any(t =>
                                t.Subject.SubjectAccreditations.Any(
                                    a => a.Slot.AreaOption.Area.Curriculum.Id == curr.Id))).ToList();
                }
            }


            var cs = new CourseService();
            var courseSummaries = new List<CourseSummaryModel>();

            foreach (var labeledCourse in allCourses.OrderBy(g => g.ShortName))
            {
                courseSummaries.Add(cs.GetCourseSummary(labeledCourse));
            }

            var cal = new List<CalendarEventModel>();
            var dateMap = new Dictionary<Guid, ActivityDateSummary>();

            foreach (var course in allCourses)
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


            cal.AddRange(GetCalendarEvents(dateMap.Values.ToList(), false));

            return Json(cal);

        }


        [HttpPost]
        public JsonResult CatalogEventsWeekly(string start, string end, Guid semId, Guid catId, Guid? currId, Guid? labelId)
        {
            var startDate = GetDateTime(start);
            var endDate = GetDateTime(end);

            var courseService = new CourseService(Db);

            var semester = SemesterService.GetSemester(semId);
            var catalog = Db.CurriculumModuleCatalogs.SingleOrDefault(x => x.Id == catId);

            var allCourses = Db.Activities.OfType<Course>()
                .Where(x =>
                    x.Semester.Id == semester.Id && 
                    x.Organiser.Id == catalog.Organiser.Id &&
                    x.SubjectTeachings.Any(t => t.Subject.Module.Catalog.Id == catalog.Id))
                .ToList();

            // zuerst die zum Studiengang
            /*
            if (currId.HasValue)
            {
                var curr = Db.Curricula.SingleOrDefault(x => x.Id == currId.Value);
                if (curr != null)
                {
                    allCourses =
                        allCourses
                            .Where(x => x.SubjectTeachings.Any(t =>
                                t.Subject.SubjectAccreditations.Any(
                                    a => a.Slot.AreaOption.Area.Curriculum.Id == curr.Id))).ToList();
                }
            }
            */

            // und dann das Label
            if (labelId.HasValue)
            {
                allCourses =
                    allCourses
                        .Where(x => x.LabelSet != null &&
                                    x.LabelSet.ItemLabels.Any(l => l.Id == labelId.Value)).ToList();
            }


            var cs = new CourseService();
            var courseSummaries = new List<CourseSummaryModel>();

            foreach (var labeledCourse in allCourses.OrderBy(g => g.ShortName))
            {
                courseSummaries.Add(cs.GetCourseSummary(labeledCourse));
            }

            var cal = new List<CalendarEventModel>();
            var courses = allCourses;

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

                var eventViewModel = new CalenderCourseEventViewModel();
                var summary = courseService.GetCourseSummary(course);
                eventViewModel.CourseSummary = summary;

                foreach (var day in days)
                {
                    var calDay = startDate.AddDays((int)day.Day - (int)startDate.DayOfWeek);
                    var calBegin = calDay.Add(day.Begin);
                    var calEnd = calDay.Add(day.End);

                    var sbr = new StringBuilder();
                    sbr.Append(course.ShortName);
                    if (summary.Rooms.Any())
                    {
                        if (summary.Rooms.Count() == 1)
                        {
                            sbr.AppendFormat(" ({0})", summary.Rooms.First().Number);
                        }
                        else
                        {
                            sbr.AppendFormat(" ({0}, ...)", summary.Rooms.First().Number);
                        }
                    }

                    var calEvent = new CalendarEventModel
                    {
                        id = course.Id.ToString(),
                        title = sbr.ToString(),
                        allDay = false,
                        start = calBegin.ToString(_calDateFormatCalendar),
                        end = calEnd.ToString(_calDateFormatCalendar),
                        textColor = "#000",
                        backgroundColor = "#fff",
                        borderColor = "#000",
                        courseId = course.Id.ToString(),
                        htmlContent = string.Empty   //  this.RenderViewToString("_CalendarCourseEventContent", eventViewModel)
                    };

                    cal.Add(calEvent);
                }

            }

            return Json(cal);

        }





        /// <summary>
        /// nur die Kurse
        /// </summary>
        /// <param name="dozId"></param>
        /// <param name="showPersonalDates"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
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
                    if (date.Activity is Course)
                    {
                        dateMap[date.Id] = new ActivityDateSummary(date, ActivityDateType.Offer);
                    }
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
                        if (!dateMap.ContainsKey(date.Id) && date.Activity is Course)
                        {
                            dateMap[date.Id] = new ActivityDateSummary(date, ActivityDateType.Subscription);

                        }
                    }
                }
            }


            // 3. das Suchergebnis
            // das Semester suchen, dass zum Datum passt
            // Grundannahme:  Vorlesungszeiten überlappen sich nicht
            var member = db.Members.SingleOrDefault(x => x.Id == dozId);
            if (member != null)
            {
            }
            var allDates = db.ActivityDates.Where(c =>
                c.Begin >= startDate && c.End <= endDate &&
                c.Hosts.Any(oc => oc.Id == member.Id)).ToList();

            foreach (var date in allDates)
            {
                if (!dateMap.ContainsKey(date.Id) && date.Activity is Course)
                {
                    dateMap[date.Id] = new ActivityDateSummary(date, ActivityDateType.SearchResult);
                }
            }
        

        /*
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
             * */

            cal.AddRange(GetCalendarEvents(dateMap.Values.ToList(), showPersonalDates));

            return Json(cal);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="roomId"></param>
        /// <param name="showPersonalDates"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult CourseEventsByRoom(Guid roomId, bool showPersonalDates, string start, string end)
        {
            try
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

                var room = db.Rooms.SingleOrDefault(l => l.Id == roomId);

                if (room != null)
                {
                    var allDates = db.ActivityDates.Where(c =>
                        c.Begin >= startDate && c.End <= endDate &&
                        c.Rooms.Any(oc => oc.Id == room.Id)).ToList();

                    foreach (var date in allDates)
                    {
                        if (!dateMap.ContainsKey(date.Id))
                        {
                            dateMap[date.Id] = new ActivityDateSummary(date, ActivityDateType.SearchResult);
                        }
                    }
                }

                cal.AddRange(GetCalendarEvents(dateMap.Values.ToList(), showPersonalDates));

                return Json(cal);

            }
            catch (Exception e)
            {
                return Json(e.ToString());
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult ActivityPlan(string start, string end)
        {
            var startDate = GetDateTime(start);
            var endDate = GetDateTime(end);
            var user = GetCurrentUser();

            var calendarService = new CalendarService();

            return Json(
                GetCalendarEvents(
                    calendarService.GetActivityPlan(user, startDate, endDate),
                    false));
        }

        [HttpPost]
        public JsonResult MemberActivityPlan(string start, string end, Guid memberId)
        {
            var member = MemberService.GetMember(memberId);

            var startDate = GetDateTime(start);
            var endDate = GetDateTime(end);
            var user = GetUser(member.UserId);

            var calendarService = new CalendarService();

            return Json(
                GetCalendarEvents(
                    calendarService.GetActivityPlan(user, startDate, endDate),
                    false));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult ActivityPrintPlan(string start, string end, Guid? id)
        {
            var startDate = GetDateTime(start);
            var endDate = GetDateTime(end);

            var events = new List<CalendarEventModel>();

            var courseService = new CourseService(Db);

            // start und end sind "echte" Daten, d.h. eine Woche

            // Am Schluss muss alles in "Wochentage" umgerechnet werden

            // Alle Events abstrakt nah Wochentag
            var semester = SemesterService.GetSemester(id);
            var member = GetMyMembership();
            var user = GetCurrentUser();

            var courses = new List<Course>();


            // Alle Vorlesungen, die ich halte
            if (member != null)
            {
                var mylecture =
                    Db.Activities.OfType<Course>()
                        .Where(c =>
                            c.SemesterGroups.Any(g => g.Semester.Id == semester.Id) &&
                            c.Dates.Any(oc => oc.Hosts.Any(l => l.Id == member.Id)))
                        .ToList();
                courses.AddRange(mylecture);
            }

            // Alle Kurse, in denen ich in diesem Semester eingetragen bin
            var mylisten =
                Db.Activities.OfType<Course>()
                    .Where(c =>
                        c.SemesterGroups.Any(g => g.Semester.Id == semester.Id) &&
                        c.Occurrence.Subscriptions.Any(x => x.UserId == user.Id))
                    .ToList();

            courses.AddRange(mylisten);

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

                var eventViewModel = new CalenderCourseEventViewModel();
                eventViewModel.CourseSummary = courseService.GetCourseSummary(course);


                if (days.Count == 1)
                {
                    var day = days.First();

                    var calDay = startDate.AddDays((int) day.Day - (int) startDate.DayOfWeek);
                    var calBegin = calDay.Add(day.Begin);
                    var calEnd = calDay.Add(day.End);

                    // Einfacher Eintrag
                    events.Add(new CalendarEventModel
                    {
                        title = string.Empty,
                        allDay = false,
                        start = calBegin.ToString(_calDateFormatCalendar),
                        end = calEnd.ToString(_calDateFormatCalendar),
                        textColor = "#000",
                        backgroundColor = "#fff",
                        borderColor = "#000",
                        htmlContent = this.RenderViewToString("_CalendarCourseEventContent", eventViewModel),
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
                            title = string.Empty,
                            allDay = false,
                            start = calBegin.ToString(_calDateFormatCalendar),
                            end = calEnd.ToString(_calDateFormatCalendar),
                            textColor = "#000",
                            backgroundColor = "#fff",
                            borderColor = "#000",
                            htmlContent = this.RenderViewToString("_CalendarCourseEventContent", eventViewModel),
                        });
                    }
                }
            }

            
            
            return Json(events);
        }


        [HttpPost]
        public JsonResult PersonalPlanWeekly(string start, string end, Guid? semId, bool? showWaiting)
        {
            var startDate = GetDateTime(start);
            var endDate = GetDateTime(end);

            var courseService = new CourseService(Db);

            var events = new List<CalendarEventModel>();

            // start und end sind "echte" Daten, d.h. eine Woche

            // Am Schluss muss alles in "Wochentage" umgerechnet werden

            // Alle Events abstrakt nah Wochentag
            var semester = SemesterService.GetSemester(semId);
            var user = GetCurrentUser();

            var courses = new List<Course>();


            // Alle Kurse, in denen ich in diesem Semester eingetragen bin
            if (showWaiting.HasValue && showWaiting.Value)
            {
                var mylisten =
                    Db.Activities.OfType<Course>()
                        .Where(c =>
                            c.Semester.Id == semester.Id &&
                            c.Occurrence.Subscriptions.Any(x => x.UserId == user.Id))
                        .ToList();

                courses.AddRange(mylisten);
            }
            else
            {
                var mylisten =
                    Db.Activities.OfType<Course>()
                        .Where(c =>
                            c.Semester.Id == semester.Id &&
                            c.Occurrence.Subscriptions.Any(x => x.UserId == user.Id && !x.OnWaitingList))
                        .ToList();

                courses.AddRange(mylisten);
            }



            // Alle Vorlesungen, die ich halte
            var member = GetMyMembership();
            if (member != null)
            {
                var mylecture =
                    Db.Activities.OfType<Course>()
                        .Where(c =>
                            c.Semester.Id == semester.Id &&
                            c.Dates.Any(oc => oc.Hosts.Any(l => l.Id == member.Id)))
                        .ToList();
                foreach (var lectureCourse in mylecture)
                {
                    if (!courses.Contains(lectureCourse))
                    {
                        courses.Add(lectureCourse);
                    }
                }
            }



            foreach (var course in courses)
            {
                var color = "#ddd";

                var hasSubscription = course.Occurrence.Subscriptions.Any(x => x.UserId == user.Id);
                if (hasSubscription)
                {
                    var isWaiting = course.Occurrence.Subscriptions.Any(x => x.UserId == user.Id && x.OnWaitingList);
                    color = isWaiting ? "#c89f23" : "#37918b";
                }



                var days = (from occ in course.Dates
                            select
                                new
                                {
                                    Day = occ.Begin.DayOfWeek,
                                    Begin = occ.Begin.TimeOfDay,
                                    End = occ.End.TimeOfDay,
                                }).Distinct().ToList();


                var theDay = days.First();
                var count = 0;
                if (days.Count > 1)
                {
                    foreach (var day in days)
                    {
                        var n = course.Dates.Count(x =>
                            x.Begin.DayOfWeek == day.Day && x.Begin.TimeOfDay == day.Begin &&
                            x.End.TimeOfDay == day.End);
                        if (n > count)
                        {
                            theDay = day;
                            count = n;
                        }
                    }
                }


                var eventViewModel = new CalenderCourseEventViewModel();
                var summary = courseService.GetCourseSummary(course);
                eventViewModel.CourseSummary = summary;


                    var calDay = startDate.AddDays((int)theDay.Day - (int)startDate.DayOfWeek);
                    var calBegin = calDay.Add(theDay.Begin);
                    var calEnd = calDay.Add(theDay.End);

                    var sbr = new StringBuilder();
                    sbr.Append(course.ShortName);
                    if (summary.Rooms.Any())
                    {
                        if (summary.Rooms.Count() == 1)
                        {
                            sbr.AppendFormat(" ({0})", summary.Rooms.First().Number);
                        }
                        else
                        {
                            sbr.AppendFormat(" ({0}, ...)", summary.Rooms.First().Number);
                        }
                    }

                    // Einfacher Eintrag
                    events.Add(new CalendarEventModel
                    {
                        id = course.Id.ToString(),
                        courseId = course.Id.ToString(),
                        title = sbr.ToString(),
                        allDay = false,
                        start = calBegin.ToString(_calDateFormatCalendar),
                        end = calEnd.ToString(_calDateFormatCalendar),
                        textColor = "#000",
                        backgroundColor = color,
                        borderColor = "#000",
                        htmlContent = string.Empty // this.RenderViewToString("_CalendarCourseEventContent", eventViewModel),
                    });
            }


            return Json(events);
        }



        [HttpPost]
        public JsonResult MemberPlanWeekly(string start, string end, Guid? semId, Guid? segId, Guid memberId, string color)
        {
            var startDate = GetDateTime(start);
            var endDate = GetDateTime(end);

            var courseService = new CourseService(Db);

            var events = new List<CalendarEventModel>();

            var bgColor = string.IsNullOrEmpty(color) ? "#78a3aa" : color;


            // start und end sind "echte" Daten, d.h. eine Woche

            // Am Schluss muss alles in "Wochentage" umgerechnet werden

            // Alle Events abstrakt nah Wochentag
            var semester = SemesterService.GetSemester(semId);
            var segment = segId != null ? semester.Dates.SingleOrDefault(x => x.Id == segId.Value) : null;

            var courses = new List<Course>();

            // Alle Vorlesungen, die ich halte
            var member = MemberService.GetMember(memberId);
            if (member != null)
            {
                var mylecture = segment != null ?
                    Db.Activities.OfType<Course>()
                        .Where(c =>
                            c.Semester.Id == semester.Id && c.Segment != null && c.Segment.Id == segment.Id &&
                            c.Dates.Any(oc => oc.Hosts.Any(l => l.Id == member.Id)))
                        .ToList() :
                    Db.Activities.OfType<Course>()
                        .Where(c =>
                            c.Semester.Id == semester.Id &&
                            c.Dates.Any(oc => oc.Hosts.Any(l => l.Id == member.Id)))
                        .ToList();


                foreach (var lectureCourse in mylecture)
                {
                    if (!courses.Contains(lectureCourse))
                    {
                        courses.Add(lectureCourse);
                    }
                }
            }


            // Im Stundenplan werden nur die regelmäßigen Termine eingetragen
            // das sind alle irgendwie und echt regelmäßigen
            // Block- und Wochenende werden nicht aufgeführt
            foreach (var course in courses)
            {
                var eventViewModel = new CalenderCourseEventViewModel();
                var summary = courseService.GetCourseSummary(course);
                eventViewModel.CourseSummary = summary;

                if (summary.IsPureBlock() || summary.IsPureWeekEndCourse()) continue;

                var theDay = summary.GetDefaultDate();

                var calDay = startDate.AddDays((int)theDay.DayOfWeek - (int)startDate.DayOfWeek);
                var calBegin = calDay.Add(theDay.StartTime);
                var calEnd = calDay.Add(theDay.EndTime);

                var sbr = new StringBuilder();
                sbr.Append(course.ShortName);
                if (summary.Rooms.Any())
                {
                    if (summary.Rooms.Count() == 1)
                    {
                        sbr.AppendFormat(" ({0})", summary.Rooms.First().Number);
                    }
                    else
                    {
                        sbr.AppendFormat(" ({0}, ...)", summary.Rooms.First().Number);
                    }
                }

                // Einfacher Eintrag
                events.Add(new CalendarEventModel
                {
                    id = course.Id.ToString(),
                    courseId = course.Id.ToString(),
                    title = sbr.ToString(),
                    allDay = false,
                    start = calBegin.ToString(_calDateFormatCalendar),
                    end = calEnd.ToString(_calDateFormatCalendar),
                    textColor = "#000",
                    backgroundColor = bgColor,
                    borderColor = "#000",
                    htmlContent = string.Empty // this.RenderViewToString("_CalendarCourseEventContent", eventViewModel),
                });
            }


            return Json(events);
        }






        [HttpPost]
        public JsonResult RoomPlanWeekly(string start, string end, Guid? semId, Guid? segId, Guid roomId, string color)
        {
                var startDate = GetDateTime(start);
                var endDate = GetDateTime(end);

                var semester = SemesterService.GetSemester(semId);
                var segment = segId != null ? semester.Dates.SingleOrDefault(x => x.Id == segId.Value) : null;


            var events = new List<CalendarEventModel>();
                var courses = new List<Course>();

                var bgColor = string.IsNullOrEmpty(color) ? "#feb151" : color;


                var courseService = new CourseService(Db);

                // 3. Suchergebnis
                // das Semester suchen, dass zum Datum passt
                // Grundannahme:  Vorlesungszeiten überlappen sich nicht

                var room = Db.Rooms.SingleOrDefault(l => l.Id == roomId);

                if (room != null)
                {
                    var mylecture = segment != null ?
                        Db.Activities.OfType<Course>()
                            .Where(c =>
                                c.Semester.Id == semester.Id &&
                                c.Segment != null && c.Segment.Id == segment.Id &&
                                c.Dates.Any(oc => oc.Rooms.Any(l => l.Id == room.Id)))
                            .ToList() :
                        Db.Activities.OfType<Course>()
                            .Where(c =>
                                c.Semester.Id == semester.Id &&
                                c.Dates.Any(oc => oc.Rooms.Any(l => l.Id == room.Id)))
                            .ToList();
                    foreach (var lectureCourse in mylecture)
                    {
                        if (!courses.Contains(lectureCourse))
                        {
                            courses.Add(lectureCourse);
                        }
                    }
                }

                foreach (var course in courses)
                {
                    var eventViewModel = new CalenderCourseEventViewModel();
                    var summary = courseService.GetCourseSummary(course);
                    eventViewModel.CourseSummary = summary;

                    if (summary.IsPureBlock() || summary.IsPureWeekEndCourse()) continue;

                    var theDay = summary.GetDefaultDate();

                    var calDay = startDate.AddDays((int)theDay.DayOfWeek - (int)startDate.DayOfWeek);
                    var calBegin = calDay.Add(theDay.StartTime);
                    var calEnd = calDay.Add(theDay.EndTime);

                    var sbr = new StringBuilder();
                    sbr.Append(course.ShortName);
                    if (summary.Rooms.Any())
                    {
                        if (summary.Rooms.Count() == 1)
                        {
                            sbr.AppendFormat(" ({0})", summary.Rooms.First().Number);
                        }
                        else
                        {
                            sbr.AppendFormat(" ({0}, ...)", summary.Rooms.First().Number);
                        }
                    }

                    // Einfacher Eintrag
                    events.Add(new CalendarEventModel
                    {
                        id = course.Id.ToString(),
                        courseId = course.Id.ToString(),
                        title = sbr.ToString(),
                        allDay = false,
                        start = calBegin.ToString(_calDateFormatCalendar),
                        end = calEnd.ToString(_calDateFormatCalendar),
                        textColor = "#000",
                        backgroundColor = bgColor,
                        borderColor = "#000",
                        htmlContent = string.Empty // this.RenderViewToString("_CalendarCourseEventContent", eventViewModel),
                    });
                }


                return Json(events);
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult DailyRota(string start, string end)
        {
            var startDate = GetDateTime(start);
            var endDate = GetDateTime(end);

            var db = new TimeTableDbContext();
            var org = GetMyOrganisation();

            var allDates = db.ActivityDates.Where(c =>
                (c.Activity.Organiser != null && c.Activity.Organiser.Id == org.Id) &&
                (c.Begin >= startDate && c.End <= endDate)).ToList();

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
                    start = date.Date.Begin.ToString(_calDateFormatCalendar),
                    end = date.Date.End.ToString(_calDateFormatCalendar),
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
                    false));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="roomId"></param>
        /// <param name="semId"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult RoomPrintPlan(Guid roomId, Guid? semId, string start, string end)
        {
            var startDate = GetDateTime(start);
            var endDate = GetDateTime(end);

            var events = new List<CalendarEventModel>();

            // start und end sind "echte" Daten, d.h. eine Woche

            // Am Schluss muss alles in "Wochentage" umgerechnet werden

            // Alle Events abstrakt nah Wochentag
            var semester = SemesterService.GetSemester(semId);

            var roomService = new RoomService();

            var model = roomService.GetRoomSchedule(roomId, semester);


                foreach (var date in model.RegularDates)
                {
                    var day = date.Dates.First();


                    var calDay = startDate.AddDays((int)day.Begin.DayOfWeek - (int)startDate.DayOfWeek);
                    var calBegin = calDay.Add(day.Begin.TimeOfDay);
                    var calEnd = calDay.Add(day.End.TimeOfDay);

                    var duration = day.End.TimeOfDay - day.Begin.TimeOfDay;
                    var title = date.Activity.Name;

                    if (duration.TotalMinutes <= 60)
                        title = string.Empty;                   // nur Kurzname
                    else if (duration.TotalMinutes <= 90)
                        title = title.Truncate(20);             // Langname stark gekürzt
                    else
                        title = title.Truncate(50);             // Lamgname

                    var hostNames = "N.N.";
                    if (day.Hosts.Any())
                    {
                        var sbHost = new StringBuilder();
                        foreach (var host in day.Hosts)
                        {
                            sbHost.Append(host.Name);
                            if (host != day.Hosts.Last())
                            {
                                sbHost.Append(", ");
                            }
                        }
                        hostNames = sbHost.ToString();
                    }
                    

                    var sb = new StringBuilder();
                    if (duration.TotalMinutes <= 60)
                    {
                        sb.AppendFormat("{0} [{2}]", date.Activity.ShortName, title, hostNames);
                    }
                    else
                    {
                        sb.AppendFormat("{0} - {1} [{2}]", date.Activity.ShortName, title, hostNames);
                    }

                    // Einfacher Eintrag
                    events.Add(new CalendarEventModel
                    {
                        title = sb.ToString(),
                        allDay = false,
                        start = calBegin.ToString(_calDateFormatCalendar),
                        end = calEnd.ToString(_calDateFormatCalendar),
                        textColor = "#000",
                        backgroundColor = "#fff",
                        borderColor = "#000",
                        //htmlToolbarInfo = this.RenderViewToString("_CalendarEventToolbarInfo", eventViewModel),
                        //htmlToolbar = this.RenderViewToString("_CalendarEventToolbar", eventViewModel),
                        //htmlContent = this.RenderViewToString("_CalendarEventContent", eventViewModel),
                    });
                }



            return Json(events);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="roomId"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult RoomPrintPlanWeek(Guid roomId, string start, string end)
        {
            try
            {

                var startDate = GetDateTime(start);
                var endDate = GetDateTime(end);

                var events = new List<CalendarEventModel>();

                var db = new TimeTableDbContext();

                var user = UserManager.FindByName(User.Identity.Name);

                var dateMap = new Dictionary<Guid, ActivityDateSummary>();

                // 3. Suchergebnis
                // das Semester suchen, dass zum Datum passt
                // Grundannahme:  Vorlesungszeiten überlappen sich nicht

                var room = db.Rooms.SingleOrDefault(l => l.Id == roomId);

                if (room != null)
                {
                    var allDates = db.ActivityDates.Where(c =>
                        c.Begin >= startDate && c.End <= endDate &&
                        c.Rooms.Any(oc => oc.Id == room.Id)).ToList();

                    foreach (var date in allDates)
                    {
                        var day = date;


                        var calDay = startDate.AddDays((int)day.Begin.DayOfWeek - (int)startDate.DayOfWeek);
                        var calBegin = calDay.Add(day.Begin.TimeOfDay);
                        var calEnd = calDay.Add(day.End.TimeOfDay);

                        var duration = day.End.TimeOfDay - day.Begin.TimeOfDay;
                        var title = date.Activity.Name;

                        if (duration.TotalMinutes <= 60)
                            title = string.Empty;                   // nur Kurzname
                        else if (duration.TotalMinutes <= 90)
                            title = title.Truncate(20);             // Langname stark gekürzt
                        else
                            title = title.Truncate(50);             // Lamgname

                        var hostNames = "N.N.";
                        if (day.Hosts.Any())
                        {
                            var sbHost = new StringBuilder();
                            foreach (var host in day.Hosts)
                            {
                                sbHost.Append(host.Name);
                                if (host != day.Hosts.Last())
                                {
                                    sbHost.Append(", ");
                                }
                            }
                            hostNames = sbHost.ToString();
                        }


                        var sb = new StringBuilder();
                        if (duration.TotalMinutes <= 60)
                        {
                            sb.AppendFormat("{0} [{2}]", date.Activity.ShortName, title, hostNames);
                        }
                        else
                        {
                            sb.AppendFormat("{0} - {1} [{2}]", date.Activity.ShortName, title, hostNames);
                        }

                        // Einfacher Eintrag
                        events.Add(new CalendarEventModel
                        {
                            title = sb.ToString(),
                            allDay = false,
                            start = calBegin.ToString(_calDateFormatCalendar),
                            end = calEnd.ToString(_calDateFormatCalendar),
                            textColor = "#000",
                            backgroundColor = "#fff",
                            borderColor = "#000",
                            //htmlToolbarInfo = this.RenderViewToString("_CalendarEventToolbarInfo", eventViewModel),
                            //htmlToolbar = this.RenderViewToString("_CalendarEventToolbar", eventViewModel),
                            //htmlContent = this.RenderViewToString("_CalendarEventContent", eventViewModel),
                        });

                    }
                }

                return Json(events);

            }
            catch (Exception e)
            {
                return Json(e.ToString());
            }
        }



        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        public FileResult File(string userId, string date)
        {
            ApplicationUser user = null;

            if (!string.IsNullOrEmpty(userId) && userId.StartsWith("#id#"))
            {
                var userName = userId.Remove(0, 4);
                var email = string.Format("{0}@acceleratex.org", userName);

                user = UserManager.FindByEmail(email);
            }
            else
            {
                user = UserManager.FindByName(User.Identity.Name);
            }



            if (user != null)
            {
                return GetCalendarData(user);
            }
            return null;
        }

        private FileResult GetCalendarData(ApplicationUser user)
        {
            // immer nach vorne sehen
            var semester = SemesterService.GetSemester(DateTime.Today);
            var nextSemester = SemesterService.GetNextSemester(DateTime.Today);

            var from = semester.StartCourses;
            var to = semester.EndCourses;

            if (nextSemester != null)
            {
                to = nextSemester.EndCourses;
            }

            var calendarService = new CalendarService();
            var dateList = calendarService.GetActivityPlan(user, from, to);

            //var tz = "Europe/Berlin";
            var tz = "Europe/Berlin";

            var iCal = new Calendar();


            iCal.AddTimeZone(new VTimeZone(tz));

            var now = DateTime.Now;

            foreach (var date in dateList)
            {
                var evt = iCal.Create<CalendarEvent>(); 
                evt.Summary = date.Name;
                
                if (date.Date.Occurrence != null && date.Date.Occurrence.IsCanceled)
                {
                    evt.Summary += " - abgesagt!";
                }

                if (date.Date.Activity is OfficeHour && user.MemberState == MemberState.Staff)
                {
                    int n = 0;
                    if (date.Date.Occurrence != null)
                    {
                        n += date.Date.Occurrence.Subscriptions.Count;
                    }
                    n += date.Date.Slots.Sum(slot => slot.Occurrence.Subscriptions.Count);

                    if (n > 0)
                    {
                        evt.Summary += $" - {n} Eintragungen";
                    }
                    else
                    {
                        evt.Summary += " - Keine Eintragungen";
                    }
                }

                evt.Description = date.Date.Description;


                // Die Endung Z macht, dass die Daten im Google-Calender richtig angezeigt werden
                //evt.DtStart = new Ical.Net.CalDateTime(date.Date.Begin.ToUniversalTime().ToString(@"yyyyMMdd\THHmmss\Z"));
                //evt.DtEnd = new Ical.Net.CalDateTime(date.Date.End.ToUniversalTime().ToString(@"yyyyMMdd\THHmmss\Z"));
                if (date.Slot != null)
                {
                    evt.Start = new CalDateTime(date.Slot.Begin);
                    evt.End = new CalDateTime(date.Slot.End);
                }
                else
                {
                    evt.Start = new CalDateTime(date.Date.Begin);
                    evt.End = new CalDateTime(date.Date.End);
                }

                evt.Status = (date.Date.Occurrence != null && date.Date.Occurrence.IsCanceled) ? EventStatus.Cancelled : EventStatus.Confirmed;

                var sb = new StringBuilder();
                foreach (var room in date.Date.Rooms)
                {
                    sb.Append(room.Number);
                    if (room != date.Date.Rooms.Last())
                        sb.Append(", ");
                }

                if (date.Date.VirtualRooms.Any())
                {
                    sb.Append(", ");
                    foreach (var vRoom in date.Date.VirtualRooms)
                    {
                        sb.Append(vRoom.Room.Name);
                        if (vRoom != date.Date.VirtualRooms.Last())
                            sb.Append(", ");
                    }
                }

                evt.Location = sb.ToString();
                evt.Categories.Add(date.Controller);
                //evt.IsAllDay = false;
            }


            var contentType = "text/calendar";
            var serializer = new CalendarSerializer(new SerializationContext());
            string output = serializer.SerializeToString(iCal);
            var bytes = Encoding.UTF8.GetBytes(output);

            return File(bytes, contentType, "nine.ics");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
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


        /// <summary>
        /// 
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GroupWeekPlan(string start, string end, Guid? semGroupId, bool showPersonalDates)
        {
            try
            {


                var startDate = GetDateTime(start);
                var endDate = GetDateTime(end);

                var courseService = new CourseService(Db);

                var events = new List<CalendarEventModel>();

                // start und end sind "echte" Daten, d.h. eine Woche

                // Am Schluss muss alles in "Wochentage" umgerechnet werden

                // Alle Events abstrakt nah Wochentag
                var semesterGroup = Db.SemesterGroups.SingleOrDefault(x => x.Id == semGroupId);
                var semester = semesterGroup.Semester;
                var member = GetMyMembership();
                var user = GetCurrentUser();

                var courses = new List<Course>();


                // Alle Vorlesungen, der Semestergruppe
                var mylecture =
                    Db.Activities.OfType<Course>()
                        .Where(c => c.SemesterGroups.Any(g => g.Id == semGroupId))
                        .ToList();
                courses.AddRange(mylecture);


                if (user != null && showPersonalDates)
                {
                    var activities = Db.Activities.OfType<Course>().Where(a =>
                        a.SemesterGroups.Any(s => s.Semester.Id == semester.Id) &&
                        a.Occurrence.Subscriptions.Any(u => u.UserId.Equals(user.Id))).ToList();

                    foreach (var course in activities)
                    {
                        if (!courses.Contains(course))
                        {
                            courses.Add(course);
                        }
                    }
                }



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

                    var eventViewModel = new CalenderCourseEventViewModel();
                    eventViewModel.CourseSummary = courseService.GetCourseSummary(course);

                    var bckColor = "#fff";
                    if (showPersonalDates)
                    {
                        var state = ActivityService.GetActivityState(course.Occurrence, user);
                        if (state.Subscription != null)
                        {
                            if (state.Subscription.OnWaitingList)
                            {
                                bckColor = "#c89f23";
                            }
                            else
                            {
                                bckColor = "#37918b";
                            }
                        }
                    }


                    foreach (var day in days)
                    {
                        var calDay = startDate.AddDays((int) day.Day - (int) startDate.DayOfWeek);
                        var calBegin = calDay.Add(day.Begin);
                        var calEnd = calDay.Add(day.End);


                        var calEvent = new CalendarEventModel
                        {
                            id = course.Id.ToString(),
                            title = string.Empty,
                            allDay = false,
                            start = calBegin.ToString(_calDateFormatCalendar),
                            end = calEnd.ToString(_calDateFormatCalendar),
                            textColor = "#000",
                            backgroundColor = bckColor,
                            borderColor = "#000",
                            courseId = course.Id.ToString(),
                            htmlContent = this.RenderViewToString("_CalendarCourseEventContent", eventViewModel)
                        };


                        events.Add(calEvent);
                    }
                }

                return Json(events);
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="semGroupId"></param>
        /// <param name="topicId"></param>
        /// <param name="showPersonalDates"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        public JsonResult GroupCalendar(Guid semGroupId, bool showPersonalDates, string start, string end)
        {
            var startDate = GetDateTime(start);
            var endDate = GetDateTime(end);

            var cal = new List<CalendarEventModel>();

            var db = new TimeTableDbContext();

            var dateMap = new Dictionary<Guid, ActivityDateSummary>();

            var user = GetCurrentUser();

            if (user != null && showPersonalDates)
            {
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
                            var sub = occ.Subscriptions.FirstOrDefault(x => x.UserId.Equals(user.Id));

                            dateMap[date.Id] = new ActivityDateSummary(date, sub);

                        }
                    }
                }
            }

            // 3. das Suchergebnis
            // das Semester suchen, dass zum Datum passt
            // Grundannahme:  Vorlesungszeiten überlappen sich nicht
            var semGroup = Db.SemesterGroups.SingleOrDefault(x => x.Id == semGroupId);

            if (semGroup != null && user != null)
            {
                var courses = semGroup.Activities.ToList();
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


            var events = new List<CalendarEventModel>();


            foreach (var date in dateMap.Values.ToList())
            {
                if (date.Activity != null)
                {
                    // State
                    // wenn Course => Activity
                    // wenn Sprechstunde => Date oder Slot

                    var eventViewModel = new CalenderDateEventViewModel
                    {
                        Summary = date,
                        State = ActivityService.GetActivityState(date.Date.Activity.Occurrence, AppUser),
                        Lottery = date.Activity.Occurrence != null
                                    ? Db.Lotteries.FirstOrDefault(x => x.Occurrences.Any(y => y.Id == date.Activity.Occurrence.Id))
                                    : null
                    };



                    // Workaround für fullcalendar
                    // wenn der Kalendereintrag ein zu geringe höhe hat,
                    // dann wird statt des Endes der Titel angezeigt
                    // Den Titel rendern wir selber, d.h. i.d.R. geben wir ihn nicht an!
                    // file: fullcalendar.js line 3945
                    /*
                    var duration = date.Date.End - date.Date.Begin;
                        if (duration.TotalMinutes <= 60)
                            string title = null;
                            title = date.Date.End.TimeOfDay.ToString(@"hh\:mm");
                     */
                    var sb = new StringBuilder();

                    if (date.Slot != null)
                    {
                        events.Add(new CalendarEventModel
                        {
                            title = sb.ToString(),
                            allDay = false,
                            start = date.Date.Begin.ToString(_calDateFormatCalendar),
                            end = date.Date.End.ToString(_calDateFormatCalendar),
                            textColor = date.TextColor,
                            backgroundColor = date.BackgroundColor,
                            borderColor = "#ff0000",
                            htmlContent = this.RenderViewToString("_CalendarDateEventContent", eventViewModel),
                        });
                    }
                    else
                    {
                        events.Add(new CalendarEventModel
                        {
                            id = date.Activity.Id.ToString(),
                            courseId = date.Activity.Id.ToString(),
                            title = sb.ToString(),
                            allDay = false,
                            start = date.Date.Begin.ToString(_calDateFormatCalendar),
                            end = date.Date.End.ToString(_calDateFormatCalendar),
                            textColor = date.TextColor,
                            backgroundColor = date.BackgroundColor,
                            borderColor = "#000",
                            htmlContent = this.RenderViewToString("_CalendarDateEventContent", eventViewModel),
                        });
                    }
                }
                else
                {
                }
            }



            cal.AddRange(events);

            return Json(cal);
        }


        [HttpPost]
        public JsonResult MemberAvailabilityWeekly(string start, string end, Guid? semId, Guid? segId, Guid memberId)
        {
            var startDate = GetDateTime(start);
            var endDate = GetDateTime(end);

            var events = new List<CalendarEventModel>();

            // start und end sind "echte" Daten, d.h. eine Woche

            // Am Schluss muss alles in "Wochentage" umgerechnet werden

            // Alle Events abstrakt nah Wochentag
            var semester = SemesterService.GetSemester(semId);

            var courses = new List<PersonalDate>();

            // Alle Vorlesungen, die ich halte
            var member = MemberService.GetMember(memberId);
            if (member != null && semester != null)
            {
                if (segId.HasValue)
                {
                    var mylecture =
                        Db.Activities.OfType<PersonalDate>()
                            .Where(c =>
                                c.Semester.Id == semester.Id &&
                                c.Segment != null && c.Segment.Id == segId.Value &&
                                c.Owners.Any(oc => oc.Member.Id == member.Id))
                            .ToList();
                    courses.AddRange(mylecture);
                }
                else
                {
                    var mylecture =
                        Db.Activities.OfType<PersonalDate>()
                            .Where(c =>
                                c.Semester.Id == semester.Id &&
                                c.Owners.Any(oc => oc.Member.Id == member.Id))
                            .ToList();
                    courses.AddRange(mylecture);
                }
            }


            // Im Stundenplan werden nur die regelmäßigen Termine eingetragen
            // das sind alle irgendwie und echt regelmäßigen
            // Block- und Wochenende werden nicht aufgeführt
            foreach (var course in courses)
            {
                var eventViewModel = new CalenderCourseEventViewModel();

                var color = "#0d0";

                var days = (from occ in course.Dates
                    select
                        new
                        {
                            Day = occ.Begin.DayOfWeek,
                            Begin = occ.Begin.TimeOfDay,
                            End = occ.End.TimeOfDay,
                        }).Distinct();

                foreach (var day in days)
                {

                    var calDay = startDate.AddDays((int)day.Day - (int)startDate.DayOfWeek);
                    var calBegin = calDay.Add(day.Begin);
                    var calEnd = calDay.Add(day.End);

                    var sbr = new StringBuilder();
                    sbr.Append(course.ShortName);

                    // Einfacher Eintrag
                    events.Add(new CalendarEventModel
                    {
                        id = course.Id.ToString(),
                        courseId = course.Id.ToString(),
                        title = sbr.ToString(),
                        allDay = false,
                        start = calBegin.ToString(_calDateFormatCalendar),
                        end = calEnd.ToString(_calDateFormatCalendar),
                        textColor = "#000",
                        backgroundColor = color,
                        borderColor = "#000",
                        htmlContent =
                            string.Empty // this.RenderViewToString("_CalendarCourseEventContent", eventViewModel),
                    });
                }
            }

            return Json(events);
        }

        [HttpPost]
        public JsonResult MemberAvailability(string start, string end, Guid memberId)
        {
            var member = MemberService.GetMember(memberId);

            var startDate = GetDateTime(start);
            var endDate = GetDateTime(end);
            var user = GetUser(member.UserId);
            var semester = SemesterService.GetSemester(startDate);

            var events = new List<CalendarEventModel>();

            var mylecture =
                Db.Activities.OfType<PersonalDate>()
                    .Where(c =>
                        c.Semester.Id == semester.Id &&
                        c.Owners.Any(oc => oc.Member.Id == member.Id))
                    .ToList();

            foreach (var personalDate in mylecture)
            {
                var dates = personalDate.Dates.Where(x => x.Begin >= startDate && x.End <= endDate).ToList();
                
                foreach (var date in dates)
                {
                    events.Add(new CalendarEventModel
                    {
                        id = date.Id.ToString(),
                        courseId = date.Activity.Id.ToString(),
                        title = "",
                        allDay = false,
                        start = date.Begin.ToString(_calDateFormatCalendar),
                        end = date.End.ToString(_calDateFormatCalendar),
                        textColor = "#000",
                        backgroundColor = "#0d0",
                        borderColor = "#000",
                        htmlContent =
                            string.Empty // this.RenderViewToString("_CalendarCourseEventContent", eventViewModel),
                    });

                }

            }

            return Json(events);
        }


        [HttpPost]
        public JsonResult PersonalAvailabilityWeekly(string start, string end, Guid? semId)
        {
            var startDate = GetDateTime(start);
            var endDate = GetDateTime(end);

            var events = new List<CalendarEventModel>();

            // start und end sind "echte" Daten, d.h. eine Woche

            // Am Schluss muss alles in "Wochentage" umgerechnet werden

            // Alle Events abstrakt nah Wochentag
            var semester = SemesterService.GetSemester(semId);

            var courses = new List<PersonalDate>();

            // Alle Vorlesungen, die ich halte
            var member = GetMyMembership();
            if (member != null)
            {
                var mylecture =
                    Db.Activities.OfType<PersonalDate>()
                        .Where(c =>
                            c.Semester.Id == semester.Id &&
                            c.Owners.Any(oc => oc.Member.Id == member.Id))
                        .ToList();
                courses.AddRange(mylecture);
            }


            // Im Stundenplan werden nur die regelmäßigen Termine eingetragen
            // das sind alle irgendwie und echt regelmäßigen
            // Block- und Wochenende werden nicht aufgeführt
            foreach (var course in courses)
            {
                var eventViewModel = new CalenderCourseEventViewModel();

                var color = "#0d0";

                var days = (from occ in course.Dates
                            select
                                new
                                {
                                    Day = occ.Begin.DayOfWeek,
                                    Begin = occ.Begin.TimeOfDay,
                                    End = occ.End.TimeOfDay,
                                }).Distinct();

                foreach (var day in days)
                {

                    var calDay = startDate.AddDays((int)day.Day - (int)startDate.DayOfWeek);
                    var calBegin = calDay.Add(day.Begin);
                    var calEnd = calDay.Add(day.End);

                    var sbr = new StringBuilder();
                    sbr.Append(course.ShortName);

                    // Einfacher Eintrag
                    events.Add(new CalendarEventModel
                    {
                        id = course.Id.ToString(),
                        courseId = course.Id.ToString(),
                        title = sbr.ToString(),
                        allDay = false,
                        start = calBegin.ToString(_calDateFormatCalendar),
                        end = calEnd.ToString(_calDateFormatCalendar),
                        textColor = "#000",
                        backgroundColor = color,
                        borderColor = "#000",
                        htmlContent =
                            string.Empty // this.RenderViewToString("_CalendarCourseEventContent", eventViewModel),
                    });
                }
            }

            return Json(events);
        }

        [HttpPost]
        public JsonResult PersonalAvailability(string start, string end)
        {
            var member = GetMyMembership();

            var startDate = GetDateTime(start);
            var endDate = GetDateTime(end);
            var user = GetUser(member.UserId);
            var semester = SemesterService.GetSemester(startDate);

            var events = new List<CalendarEventModel>();

            var mylecture =
                Db.Activities.OfType<PersonalDate>()
                    .Where(c =>
                        c.Semester.Id == semester.Id &&
                        c.Owners.Any(oc => oc.Member.Id == member.Id))
                    .ToList();

            foreach (var personalDate in mylecture)
            {
                var dates = personalDate.Dates.Where(x => x.Begin >= startDate && x.End <= endDate).ToList();

                foreach (var date in dates)
                {
                    events.Add(new CalendarEventModel
                    {
                        id = date.Id.ToString(),
                        courseId = date.Activity.Id.ToString(),
                        title = "",
                        allDay = false,
                        start = date.Begin.ToString(_calDateFormatCalendar),
                        end = date.End.ToString(_calDateFormatCalendar),
                        textColor = "#000",
                        backgroundColor = "#0d0",
                        borderColor = "#000",
                        htmlContent =
                            string.Empty // this.RenderViewToString("_CalendarCourseEventContent", eventViewModel),
                    });

                }

            }

            return Json(events);
        }



    }
}