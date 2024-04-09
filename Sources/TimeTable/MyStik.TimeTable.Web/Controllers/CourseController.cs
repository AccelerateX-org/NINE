using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;
using log4net;
using Markdig;
using Microsoft.Ajax.Utilities;
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
    public class CourseController : BaseController
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Details(Guid id)
        {
            var courseService = new CourseService(Db);

            CourseSelectModel model = null;

            if (!Request.IsAuthenticated)
            {
                model = courseService.GetCourseSelectModel(id, null);
            }
            else
            {
                var user = GetCurrentUser();
                model = courseService.GetCourseSelectModel(id, user.Id);
            }

            if (model.Summary.Course.LabelSet == null)
            {
                var labelSet = new ItemLabelSet();
                Db.ItemLabelSets.Add(labelSet);
                model.Summary.Course.LabelSet = labelSet;
                Db.SaveChanges();
            }

            if (!string.IsNullOrEmpty(model.Summary.Course.Description))
            {
                var pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().UseBootstrap().Build();
                var result = Markdown.ToHtml(model.Summary.Course.Description, pipeline);
                model.CourseDescriptionHtml = result;
            }

            var userRights = GetUserRight(User.Identity.Name, model.Summary.Course);
            ViewBag.UserRight = userRights;

            return View("Details", model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Index(Guid? id)
        {
            if (id.HasValue)
                return RedirectToAction("Details", new { id = id });

            return View();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult CancelDate(Guid id)
        {
            var summary = ActivityService.GetSummary(id);

            var mailModel = new SubscriptionMailModel
            {
                Summary = summary,
                SenderUser = UserManager.FindByName(User.Identity.Name),
            };

            var mailing = new MailController();

            string sText;
            var email = mailing.CancelOccurrence(mailModel);
            using (var sr = new StreamReader(email.Mail.AlternateViews[0].ContentStream))
            {
                sText = sr.ReadToEnd();
            }

            var model = new OccurrenceMailingModel
            {
                OccurrenceId = id,
                Summary = summary,
                Body = sText,
                Subject = email.Mail.Subject
            };

            return View(model);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult CancelDate(OccurrenceMailingModel model)
        {
            var logger = LogManager.GetLogger("Course");
            var occ = Db.Occurrences.SingleOrDefault(o => o.Id == model.OccurrenceId);

            if (occ != null)
            {
                occ.IsCanceled = true;

                var date = Db.ActivityDates.SingleOrDefault(d => d.Occurrence.Id == occ.Id);
                // Alle Räume freigeben
                // Die Zuordnung zum Dozent bleibt aber bestehen!
                if (date != null)
                {
                    date.Rooms.Clear();
                    date.VirtualRooms.Clear();
                }

                Db.SaveChanges();

                // Die Subscription stecken in der Activity
                var summary = ActivityService.GetSummary(model.OccurrenceId) as ActivityDateSummary;

                logger.InfoFormat("Course date {0} on {1} canceled by {2}", summary.Activity.Name, summary.Date.Begin,
                    User.Identity.Name);


                // Wenn es keine Eintragungen gibt, dann muss auch keine mail versendet werden
                if (!summary.Subscriptions.Any())
                {
                    return RedirectToAction(summary.Action, summary.Controller, new { id = summary.Id });
                }
            }

            model.UseParticipients = true;
            model.UseWaitingList = true;

            var m = new MailingController();
            m.ControllerContext = ControllerContext;
            return m.CustomOccurrenceMail(model);
        }

        /// <summary>
        /// CourseId
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult CancelAllDates(Guid id)
        {
            var course = Db.Activities.OfType<Course>().SingleOrDefault(c => c.Id == id);
            var summary = ActivityService.GetSummary(course.Occurrence.Id);

            var mailModel = new SubscriptionMailModel
            {
                Summary = summary,
                SenderUser = UserManager.FindByName(User.Identity.Name),
            };

            var mailing = new MailController();

            string sText;
            var email = mailing.CancelAllOccurrences(mailModel);
            using (var sr = new StreamReader(email.Mail.AlternateViews[0].ContentStream))
            {
                sText = sr.ReadToEnd();
            }

            var model = new OccurrenceMailingModel
            {
                OccurrenceId = course.Occurrence.Id,
                Summary = summary,
                Body = sText,
                Subject = email.Mail.Subject
            };

            return View(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult CancelAllDates(OccurrenceMailingModel model)
        {
            var logger = LogManager.GetLogger("Course");

            var course = Db.Activities.OfType<Course>().SingleOrDefault(c => c.Occurrence.Id == model.OccurrenceId);

            if (course != null)
            {
                foreach (var date in course.Dates)
                {
                    date.Occurrence.IsCanceled = true;
                    date.Rooms.Clear();
                    date.VirtualRooms.Clear();
                }

                Db.SaveChanges();

                // Die Subscription stecken in der Activity
                var summary = ActivityService.GetSummary(model.OccurrenceId);

                logger.InfoFormat("all course dates {0} canceled by {2}", summary.Activity.Name, User.Identity.Name);

                if (!summary.Subscriptions.Any())
                {
                    return RedirectToAction(summary.Action, summary.Controller, new { id = summary.Id });
                }
            }

            model.UseParticipients = true;
            model.UseWaitingList = true;

            var m = new MailingController();
            m.ControllerContext = ControllerContext;
            return m.CustomOccurrenceMail(model);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult ReactivateDate(Guid id)
        {
            var summary = ActivityService.GetSummary(id);

            var mailModel = new SubscriptionMailModel
            {
                Summary = summary,
                SenderUser = UserManager.FindByName(User.Identity.Name),
            };

            var mailing = new MailController();

            string sText;
            var email = mailing.ReactivateOccurrence(mailModel);
            using (var sr = new StreamReader(email.Mail.AlternateViews[0].ContentStream))
            {
                sText = sr.ReadToEnd();
            }

            var model = new OccurrenceMailingModel
            {
                OccurrenceId = id,
                Summary = summary,
                Body = sText,
                Subject = email.Mail.Subject
            };

            return View(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ReactivateDate(OccurrenceMailingModel model)
        {
            var logger = LogManager.GetLogger("Course");

            var occ = Db.Occurrences.SingleOrDefault(o => o.Id == model.OccurrenceId);

            if (occ != null)
            {
                occ.IsCanceled = false;
                Db.SaveChanges();

                // Die Subscription stecken in der Activity
                var summary = ActivityService.GetSummary(model.OccurrenceId) as ActivityDateSummary;

                logger.InfoFormat("Course date {0} on {1} reactivated by {2}", summary.Activity.Name,
                    summary.Date.Begin, User.Identity.Name);


                if (!summary.Subscriptions.Any())
                {
                    return RedirectToAction(summary.Action, summary.Controller, new { id = summary.Id });
                }
            }

            model.UseParticipients = true;
            model.UseWaitingList = true;

            var m = new MailingController();
            m.ControllerContext = ControllerContext;
            return m.CustomOccurrenceMail(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult MoveDate(Guid id)
        {
            var summary = ActivityService.GetSummary(id) as ActivityDateSummary;
            var course = summary.Activity;
            var date = summary.Date;

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
                Title = date.Title,
                Description = date.Description,
                ShortInfo = date.Occurrence.Information,
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
        public ActionResult MoveDate(CourseMoveDateModel model)
        {
            var logger = LogManager.GetLogger("Course");

            var activityDate = Db.ActivityDates.SingleOrDefault(d => d.Id == model.ActivityDateId);

            if (activityDate == null)
                return PartialView("_SaveError"); // muss noch gemacht werden

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

            activityDate.Description = model.Description;
            activityDate.Title = model.Title;
            activityDate.Occurrence.Information = model.ShortInfo;

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

            return RedirectToAction("AdminNewDates", new { id = activityDate.Activity.Id });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dateIds"></param>
        /// <param name="newBegin"></param>
        /// <param name="newEnd"></param>
        /// <returns></returns>
        [HttpPost]
        public PartialViewResult MoveDates(ICollection<Guid> dateIds, string newBegin, string newEnd)
        {
            // Das sind die Zeiten!
            var from = DateTime.Parse(newBegin);
            var to = DateTime.Parse(newEnd);

            Course course = null;
            var changeService = new ChangeService(Db);

            var changes = new List<ActivityDateChange>();

            foreach (var dateId in dateIds)
            {
                var date = Db.ActivityDates.SingleOrDefault(d => d.Id == dateId);

                if (date != null)
                {
                    // Berechnung der neuen Zeiten
                    var start = date.Begin.Date.Add(from.TimeOfDay);
                    var end = date.End.Date.Add(to.TimeOfDay);

                    // Das Änderungsobjekt anlegen
                    var changeObject = changeService.CreateActivityDateChange(date, start, end, null);
                    if (changeObject != null)
                    {
                        changeObject.UserId = AppUser.Id;
                        Db.DateChanges.Add(changeObject);

                        changes.Add(changeObject);
                    }

                    // Änderungen am DateObjekt vornehmen
                    date.Begin = start;
                    date.End = end;

                    if (course == null)
                        course = date.Activity as Course;
                }
            }

            Db.SaveChanges();

            // Jetzt erst die Notification auslösen
            /*
            foreach (var changeObject in changes)
            {
                NotificationService nservice = new NotificationService();
                nservice.CreateSingleNotification(changeObject.Id.ToString());
            }
            */

            var userRights = GetUserRight(User.Identity.Name, course);
            ViewBag.UserRight = userRights;


            return PartialView("_DateTable", course);
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="date"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>

        [HttpPost]
        public PartialViewResult CheckRoomList(Guid id, DateTime date, TimeSpan from, TimeSpan to)
        {
            DateTime start = date.AddHours(from.Hours).AddMinutes(from.Minutes);
            DateTime end = date.AddHours(to.Hours).AddMinutes(to.Minutes);
            var model = GetRoomList(id, start, end);

            return PartialView("_RoomState", model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="date"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult CheckCurriculum(Guid id, DateTime date, TimeSpan from, TimeSpan to)
        {
            var activityDate = Db.ActivityDates.SingleOrDefault(d => d.Id == id);

            // jetzt die Vorlesung
            var course = activityDate.Activity as Course;

            DateTime start = date.AddHours(from.Hours).AddMinutes(from.Minutes);
            DateTime end = date.AddHours(to.Hours).AddMinutes(to.Minutes);

            // Alle Kurse, die in dem Zeitraum stattfinden
            var courses = Db.ActivityDates.Where(d => (d.End > start && d.End < end ||
                                                       d.Begin > start && d.Begin < end) &&
                                                      d.Activity.Id != course.Id).ToList();

            var hasConflicts = false;

            foreach (var activityDate1 in courses)
            {
                // Mich interessieren nur LVs
                if (activityDate1.Activity is Course)
                {
                    // EIn Kurs, der aber bereits ein anderer ist!
                    var course1 = activityDate1.Activity as Course;

                    foreach (var group in course.SemesterGroups)
                    {
                        if (course1.SemesterGroups.Contains(group))
                        {
                            hasConflicts = true;
                        }
                    }
                }
            }


            var model = new { HasConflicts = hasConflicts };

            return Json(model);


        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="date"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="room"></param>
        /// <returns></returns>
        [HttpPost]
        public PartialViewResult AddRoom(Guid id, DateTime date, TimeSpan from, TimeSpan to, string room)
        {
            var logger = LogManager.GetLogger("Course");
            var activityDate = Db.ActivityDates.SingleOrDefault(d => d.Id == id);
            var model = Db.Rooms.SingleOrDefault(r => r.Number.Equals(room));

            if (model != null && activityDate != null)
            {
                activityDate.Rooms.Add(model);
                Db.SaveChanges();

                logger.InfoFormat("Room {0} for {1} on [{2}-{3}] was added by {4}", model.Number,
                    activityDate.Activity.ShortName,
                    activityDate.Begin, activityDate.End, User.Identity.Name);
            }
            else
            {
                logger.ErrorFormat("Room {0} or ActivityDate {1} not found", room, id);
            }

            return CheckRoomList(id, date, from, to);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="roomId"></param>
        /// <returns></returns>
        [HttpPost]
        public PartialViewResult RemoveRoom(Guid id, Guid roomId)
        {
            var logger = LogManager.GetLogger("Course");
            var activityDate = Db.ActivityDates.SingleOrDefault(d => d.Id == id);
            var room = Db.Rooms.SingleOrDefault(r => r.Id.Equals(roomId));

            if (room != null && activityDate != null)
            {
                activityDate.Rooms.Remove(room);
                Db.SaveChanges();

                logger.InfoFormat("Room {0} for {1} on [{2}-{3}] was removed by {4}", room.Number,
                    activityDate.Activity.ShortName,
                    activityDate.Begin, activityDate.End, User.Identity.Name);
            }
            else
            {
                logger.ErrorFormat("Room {0} or ActivityDate {1} not found", roomId, id);
            }

            return PartialView("_EmptyRow");
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="activityDateId"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        private RoomListStateModel GetRoomList(Guid activityDateId, DateTime from, DateTime to)
        {
            var model = new RoomListStateModel() { ActivityDateId = activityDateId };

            var date = Db.ActivityDates.SingleOrDefault(d => d.Id == activityDateId);

            if (date != null)
            {
                foreach (var room in date.Rooms)
                {
                    var conflicts = Db.ActivityDates.Where(d =>
                        d.Id != activityDateId &&
                        d.Rooms.Any(r => r.Id == room.Id) &&
                        ((d.Begin > from && d.Begin < to) || // der Anfang liegt drin
                         (d.End < to && d.End > from) // das Ende liegt drin
                        )).ToList();
                    model.RoomStates.Add(new RoomStateModel { Room = room, Conflicts = conflicts });
                }
            }

            return model;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="date"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        [HttpPost]
        public PartialViewResult CheckLecturerList(Guid id, DateTime date, TimeSpan from, TimeSpan to)
        {
            DateTime start = date.AddHours(from.Hours).AddMinutes(from.Minutes);
            DateTime end = date.AddHours(to.Hours).AddMinutes(to.Minutes);
            var model = GetLecturerList(id, start, end);

            return PartialView("_LecturerState", model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="date"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="lecturer"></param>
        /// <returns></returns>
        [HttpPost]
        public PartialViewResult AddLecturer(Guid id, DateTime date, TimeSpan from, TimeSpan to, string lecturer)
        {
            var logger = LogManager.GetLogger("Course");
            var activityDate = Db.ActivityDates.SingleOrDefault(d => d.Id == id);
            var fk09 = Db.Organisers.SingleOrDefault(org => org.ShortName.Equals("FK 09"));
            var model = fk09.Members.SingleOrDefault(r => r.ShortName.Equals(lecturer));

            if (model != null && activityDate != null)
            {
                activityDate.Hosts.Add(model);
                Db.SaveChanges();

                logger.InfoFormat("Lecturer {0} for {1} on {2} was added by {3}", model.ShortName,
                    activityDate.Activity.ShortName,
                    activityDate.Begin, User.Identity.Name);
            }
            else
            {
                logger.ErrorFormat("Lecturer {0} or ActivityDate {1} not found", lecturer, id);
            }


            return CheckLecturerList(id, date, from, to);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="lecturerId"></param>
        /// <returns></returns>
        [HttpPost]
        public PartialViewResult RemoveLecturer(Guid id, Guid lecturerId)
        {
            var logger = LogManager.GetLogger("Course");
            var activityDate = Db.ActivityDates.SingleOrDefault(d => d.Id == id);
            var lecturer = Db.Members.SingleOrDefault(r => r.Id.Equals(lecturerId));

            if (lecturer != null && activityDate != null)
            {
                activityDate.Hosts.Remove(lecturer);
                Db.SaveChanges();

                logger.InfoFormat("Lecturer {0} for {1} on {2} was removed by {3}", lecturer.ShortName,
                    activityDate.Activity.ShortName,
                    activityDate.Begin, User.Identity.Name);
            }
            else
            {
                logger.ErrorFormat("Lecturer {0} or ActivityDate {1} not found", lecturerId, id);
            }

            return PartialView("_EmptyRow");
        }



        private LecturerListStateModel GetLecturerList(Guid activityDateId, DateTime from, DateTime to)
        {
            var model = new LecturerListStateModel() { ActivityDateId = activityDateId };

            var date = Db.ActivityDates.SingleOrDefault(d => d.Id == activityDateId);

            if (date != null)
            {
                foreach (var lecturer in date.Hosts)
                {
                    var conflicts = Db.ActivityDates.Where(d =>
                        d.Id != activityDateId &&
                        d.Hosts.Any(r => r.Id == lecturer.Id) &&
                        ((d.Begin > from && d.Begin < to) || // der Anfang liegt drin
                         (d.End < to && d.End > from) // das Ende liegt drin
                        )).ToList();
                    model.LecturerStates.Add(new LecturerStateModel { Lecturer = lecturer, Conflicts = conflicts });
                }
            }

            return model;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult DeleteDate(Guid id)
        {
            var occ = Db.Occurrences.SingleOrDefault(o => o.Id == id);

            var dateSummary = ActivityService.GetSummary(id) as ActivityDateSummary;

            var date = Db.ActivityDates.SingleOrDefault(d => d.Id == dateSummary.Date.Id);

            var actSummary = new ActivitySummary { Activity = date.Activity };

            occ.Subscriptions.ForEach(s => Db.Subscriptions.Remove(s));
            Db.Occurrences.Remove(occ);
            date.Occurrence = null;


            date.Activity.Dates.Remove(date);
            date.Hosts.Clear();
            date.Rooms.Clear();

            foreach (var vRoom in date.VirtualRooms.ToList())
            {
                Db.VirtualRoomAccesses.Remove(vRoom);
            }

            Db.ActivityDates.Remove(date);

            Db.SaveChanges();


            // Kehre zurück zur Seite der Aktivität
            return RedirectToAction(actSummary.Action, actSummary.Controller, new { id = actSummary.Id });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult DeleteAllDates(Guid id)
        {
            var course = Db.Activities.OfType<Course>().SingleOrDefault(c => c.Id == id);

            var model = new CourseDeleteModel { Course = course };

            return View(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DeleteAllDates(CourseDeleteModel model)
        {
            var course = Db.Activities.OfType<Course>().SingleOrDefault(c => c.Id == model.Course.Id);

            var actSummary = new ActivitySummary { Activity = course };

            if (course != null)
                new CourseService(Db).DeleteAllDates(course.Id);

            // Kehre zurück zur Seite der Aktivität
            return RedirectToAction(actSummary.Action, actSummary.Controller, new { id = actSummary.Id });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult DeleteCourse(Guid id)
        {
            var course = Db.Activities.OfType<Course>().SingleOrDefault(c => c.Id == id);

            var model = new CourseDeleteModel { Course = course };

            return View(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DeleteCourse(CourseDeleteModel model)
        {
            var course = Db.Activities.OfType<Course>().SingleOrDefault(c => c.Id == model.Course.Id);

            if (course.Occurrence.Subscriptions.Any())
            {
                var model2 = new CourseDeleteModel { Course = course };

                return View(model2);
            }

            if (!course.ShortName.Equals(model.Code))
            {
                var model2 = new CourseDeleteModel { Course = course };

                return View(model2);
            }



            var timeTableService = new TimeTableInfoService(Db);

            timeTableService.DeleteCourse(model.Course.Id);

            // Kehre zurück zur Seite der Aktivität
            return RedirectToAction("Index", "Home");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult CleanCourse(Guid id)
        {
            var course = Db.Activities.OfType<Course>().SingleOrDefault(c => c.Id == id);

            var model = new CourseDeleteModel { Course = course };

            return View(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult CleanCourse(CourseDeleteModel model)
        {
            // Kehre zurück zur Seite der Aktivität
            return RedirectToAction("Index", "Home");
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">ActivitiyDateId</param>
        /// <returns></returns>
        public ActionResult SetInfo(Guid id)
        {
            var date = Db.ActivityDates.SingleOrDefault(o => o.Id == id);


            var model = new CourseDateInfoModel()
            {
                Course = date.Activity as Course,
                Date = date,
                ActivityDateId = date.Id,
            };

            return View(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SetInfo(CourseDateInfoModel model)
        {
            var date = Db.ActivityDates.SingleOrDefault(o => o.Id == model.ActivityDateId);

            if (date != null)
            {
                date.Title = model.Date.Title;
                date.Description = model.Date.Description;
                date.Occurrence.Information = model.Date.Occurrence.Information;
                Db.SaveChanges();
            }

            var summary = ActivityService.GetSummary(date.Occurrence.Id);

            return RedirectToAction(summary.Action, summary.Controller, new { id = summary.Id });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="courseId">Id der zugehörigen Lehrveranstaltung</param>
        /// <returns></returns>
        [HttpPost]
        public PartialViewResult CreateDate(Guid courseId)
        {
            var activity = Db.Activities.SingleOrDefault(a => a.Id == courseId);
            if (activity != null)
            {
                var today = DateTime.Today;
                var from = DateTime.Now;
                var to = from.AddMinutes(90);

                var date = new ActivityDate
                {
                    Begin = today.AddHours(from.Hour).AddMinutes(from.Minute),
                    End = today.AddHours(to.Hour).AddMinutes(to.Minute),
                    Occurrence = new Occurrence
                    {
                        Capacity = -1,
                        IsAvailable = true,
                        FromIsRestricted = false,
                        UntilIsRestricted = false
                    },
                };

                activity.Dates.Add(date);
                Db.SaveChanges();
            }

            var course = activity as Course;
            var userRights = GetUserRight(User.Identity.Name, course);
            ViewBag.UserRight = userRights;

            return PartialView("_DateTable", course);

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="courseId"></param>
        /// <returns></returns>
        public ActionResult CreateDate2(Guid courseId)
        {
            var course = Db.Activities.OfType<Course>().SingleOrDefault(c => c.Id == courseId);
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
            ViewBag.RoomOrganiser = Db.Organisers.Where(x => x.Id == org.Id).Select(c => new SelectListItem
            {
                Text = c.ShortName,
                Value = c.Id.ToString(),
            });

            var culture = Thread.CurrentThread.CurrentUICulture;
            ViewBag.Culture = culture;

            return View("CreateDate2", model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult CreateCourseDate(CourseDateCreateModelExtended model)
        {
            var course = Db.Activities.OfType<Course>().SingleOrDefault(c => c.Id == model.CourseId);

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
                    if (frequency > 0 &&!string.IsNullOrEmpty(elems[4]))
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

            return Json(new { result = "Redirect", url = Url.Action("AdminNewDates", new { id = course.Id }) });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">id des Kurses</param>
        /// <returns></returns>
        public ActionResult EditOccurrenceRule(Guid id)
        {
            return View();
        }

        public ActionResult Create()
        {
            var user = GetCurrentUser();
            var members = MemberService.GetFacultyMemberships(user.Id);


            return View(members);
        }

        public ActionResult CreateCoursePortal(Guid? orgId)
        {
            var org = orgId == null ? GetMyOrganisation() : GetOrganisation(orgId.Value);

            return View(org);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">SemesterId</param>
        /// <returns></returns>
        public ActionResult CreateCoursePeriod(Guid? orgId, Guid? semId)
        {
            var sem = SemesterService.GetSemester(semId);

            var org = orgId == null ? GetMyOrganisation() : GetOrganisation(orgId.Value);

            var model = new CourseCreateModel2
            {
                SemesterId = sem.Id,
                OrganiserId = org.Id,
                OrganiserId2 = org.Id,
                OrganiserId3 = org.Id
            };

            // Liste aller Fakultäten
            ViewBag.Organiser = Db.Organisers
                .Where(x => x.Id == org.Id)
                .OrderBy(x => x.ShortName)
                .Select(c => new SelectListItem
                {
                    Text = c.ShortName,
                    Value = c.Id.ToString(),
                });

            // Liste aller Fakultäten, auf die Zugriff auf Räume bestehen
            // aktuell nur meine
            ViewBag.RoomOrganiser = Db.Organisers.Where(x => x.Id == org.Id).Select(c => new SelectListItem
            {
                Text = c.ShortName,
                Value = c.Id.ToString(),
            });

            // Alle Semester, die in Zukunft enden
            ViewBag.Semester = Db.Semesters
                .Where(x => x.EndCourses >= DateTime.Today)
                .OrderBy(s => s.StartCourses)
                .Take(3)
                .Select(c => new SelectListItem
                {
                    Text = c.Name,
                    Value = c.Id.ToString(),
                });
            model.SemesterId = sem.Id;

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
        public ActionResult CreateCoursePeriod(CourseCreateModelExtended model)
        {
            var org = GetOrganiser(model.OrganiserId);
            var semester = SemesterService.GetSemester(model.SemesterId);

            var course = new Course
            {
                Name = model.Name,
                ShortName = string.IsNullOrEmpty(model.ShortName) ? model.Name : model.ShortName,
                Organiser = org,
                Semester = semester,
                IsProjected = true,
                IsInternal = true,
                Occurrence = new Occurrence
                {
                    Capacity = -1,
                    IsAvailable = false,
                    FromIsRestricted = false,
                    UntilIsRestricted = false,
                    IsCanceled = false,
                    IsMoved = false,
                    UseGroups = false,
                },
            };



            var member = GetMyMembership(org.Id);

            if (member != null)
            {
                // das Objeklt muss aus dem gleichen Kontext kommen
                var me = Db.Members.SingleOrDefault(m => m.Id == member.Id);

                ActivityOwner owner = new ActivityOwner
                {
                    Activity = course,
                    Member = me,
                    IsLocked = false
                };

                course.Owners.Add(owner);
                Db.ActivityOwners.Add(owner);
            }


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
                    var lastday = DateTime.Parse(elems[1]);
                    var begin = TimeSpan.Parse(elems[2]);
                    var end = TimeSpan.Parse(elems[3]);
                    var frq = int.Parse(elems[4]);

                    var dayList = semesterService.GetDays(day, lastday, frq);

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


            Db.Activities.Add(course);
            Db.SaveChanges();

            if (model.showDetails)
            {
                var url = Url.Action("Details", "Course", new { id = course.Id });

                return Content(url);
            }

            var url2 = Url.Action("CreateCoursePeriod", "Course", new { orgId = course.Organiser.Id });

            return Content(url2);
        }


        public ActionResult CreateCourseBlock(Guid? orgId, Guid? semId)
        {
            var sem = SemesterService.GetSemester(semId);

            var org = orgId == null ? GetMyOrganisation() : GetOrganisation(orgId.Value);

            var model = new CourseCreateModel2
            {
                SemesterId = sem.Id,
                OrganiserId = org.Id,
                OrganiserId2 = org.Id,
                OrganiserId3 = org.Id
            };

            // Liste aller Fakultäten
            ViewBag.Organiser = Db.Organisers
                .Where(x => x.Id == org.Id)
                .OrderBy(x => x.ShortName)
                .Select(c => new SelectListItem
                {
                    Text = c.ShortName,
                    Value = c.Id.ToString(),
                });

            // Liste aller Fakultäten, auf die Zugriff auf Räume bestehen
            // aktuell nur meine
            ViewBag.RoomOrganiser = Db.Organisers.Where(x => x.Id == org.Id).Select(c => new SelectListItem
            {
                Text = c.ShortName,
                Value = c.Id.ToString(),
            });

            // Alle Semester, die in Zukunft enden
            ViewBag.Semester = Db.Semesters
                .Where(x => x.EndCourses >= DateTime.Today)
                .OrderBy(s => s.StartCourses)
                .Take(3)
                .Select(c => new SelectListItem
                {
                    Text = c.Name,
                    Value = c.Id.ToString(),
                });
            model.SemesterId = sem.Id;

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
        public ActionResult CreateCourseBlock(CourseCreateModelExtended model)
        {
            var org = GetOrganiser(model.OrganiserId);
            var semester = SemesterService.GetSemester(model.SemesterId);

            var course = new Course
            {
                Name = model.Name,
                ShortName = string.IsNullOrEmpty(model.ShortName) ? model.Name : model.ShortName,
                Organiser = org,
                Semester = semester,
                IsProjected = true,
                IsInternal = true,
                Occurrence = new Occurrence
                {
                    Capacity = -1,
                    IsAvailable = false,
                    FromIsRestricted = false,
                    UntilIsRestricted = false,
                    IsCanceled = false,
                    IsMoved = false,
                    UseGroups = false,
                },
            };



            var member = GetMyMembership(org.Id);

            if (member != null)
            {
                // das Objeklt muss aus dem gleichen Kontext kommen
                var me = Db.Members.SingleOrDefault(m => m.Id == member.Id);

                ActivityOwner owner = new ActivityOwner
                {
                    Activity = course,
                    Member = me,
                    IsLocked = false
                };

                course.Owners.Add(owner);
                Db.ActivityOwners.Add(owner);
            }


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
            if (model.Dates != null)
            {
                foreach (var date in model.Dates)
                {
                    var elems = date.Split('#');
                    var day = DateTime.Parse(elems[0]);
                    var begin = TimeSpan.Parse(elems[1]);
                    var end = TimeSpan.Parse(elems[2]);


                    ICollection<DateTime> dayList;
                    dayList = new List<DateTime> { day };
           

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


            Db.Activities.Add(course);
            Db.SaveChanges();

            if (model.showDetails)
            {
                var url = Url.Action("Details", "Course", new { id = course.Id });

                return Content(url);
            }

            var url2 = Url.Action("CreateCourseBlock", "Course", new { orgId = course.Organiser.Id });

            return Content(url2);
        }


        public ActionResult CreateCourseMass(Guid? orgId, Guid? semId)
        {
            var sem = SemesterService.GetSemester(semId);

            var org = orgId == null ? GetMyOrganisation() : GetOrganisation(orgId.Value);

            var model = new CourseCreateModel2
            {
                SemesterId = sem.Id,
                OrganiserId = org.Id,
                OrganiserId2 = org.Id,
                OrganiserId3 = org.Id
            };

            // Liste aller Fakultäten
            ViewBag.Organiser = Db.Organisers
                .Where(x => x.Id == org.Id)
                .OrderBy(x => x.ShortName)
                .Select(c => new SelectListItem
                {
                    Text = c.ShortName,
                    Value = c.Id.ToString(),
                });

            // Liste aller Fakultäten, auf die Zugriff auf Räume bestehen
            // aktuell nur meine
            ViewBag.RoomOrganiser = Db.Organisers.Where(x => x.Id == org.Id).Select(c => new SelectListItem
            {
                Text = c.ShortName,
                Value = c.Id.ToString(),
            });

            // Alle Semester, die in Zukunft enden
            ViewBag.Semester = Db.Semesters
                .Where(x => x.EndCourses >= DateTime.Today)
                .OrderBy(s => s.StartCourses)
                .Take(3)
                .Select(c => new SelectListItem
                {
                    Text = c.Name,
                    Value = c.Id.ToString(),
                });
            model.SemesterId = sem.Id;


            return View(model);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult CreateCourseMass(CourseCreateModelExtended model)
        {
            var org = GetOrganiser(model.OrganiserId);
            var semester = SemesterService.GetSemester(model.SemesterId);

            if (model.Dates != null)
            {
                foreach (var date in model.Dates)
                {
                    string[] elems = date.Split('#');
                    var day = elems[0];
                    var begin = elems[1];


                    var course = new Course
                    {
                        Name = begin,
                        ShortName = day,
                        Organiser = org,
                        Semester = semester,
                        IsProjected = true,
                        IsInternal = true,
                        Occurrence = new Occurrence
                        {
                            Capacity = -1,
                            IsAvailable = false,
                            FromIsRestricted = false,
                            UntilIsRestricted = false,
                            IsCanceled = false,
                            IsMoved = false,
                            UseGroups = false,
                        },
                    };



                    var member = GetMyMembership(org.Id);

                    if (member != null)
                    {
                        // das Objeklt muss aus dem gleichen Kontext kommen
                        var me = Db.Members.SingleOrDefault(m => m.Id == member.Id);

                        ActivityOwner owner = new ActivityOwner
                        {
                            Activity = course,
                            Member = me,
                            IsLocked = false
                        };

                        course.Owners.Add(owner);
                        Db.ActivityOwners.Add(owner);
                    }


                    Db.Activities.Add(course);
                }

                Db.SaveChanges();
            }


            var url2 = Url.Action("Faculty", "University", new { id = org.Id });

            return Content(url2);
        }


        public ActionResult CreateCourseSubject(Guid subjectId, Guid semId, Guid orgId)
        {
            var org = GetOrganiser(orgId);
            var semester = SemesterService.GetSemester(semId);
            var subject = Db.ModuleCourses.SingleOrDefault(x => x.Id == subjectId);


            var course = new Course
            {
                Name = $"{subject.Module.Name} ({subject.Name})",
                ShortName = $"{subject.Module.Tag} ({subject.TeachingFormat.Tag})",
                Organiser = org,
                Semester = semester,
                IsProjected = true,
                IsInternal = true,
                Occurrence = new Occurrence
                {
                    Capacity = -1,
                    IsAvailable = false,
                    FromIsRestricted = false,
                    UntilIsRestricted = false,
                    IsCanceled = false,
                    IsMoved = false,
                    UseGroups = false,
                },
            };

            var member = GetMyMembership(org.Id);

            if (member != null)
            {
                // das Objeklt muss aus dem gleichen Kontext kommen
                var me = Db.Members.SingleOrDefault(m => m.Id == member.Id);

                ActivityOwner owner = new ActivityOwner
                {
                    Activity = course,
                    Member = me,
                    IsLocked = false
                };

                course.Owners.Add(owner);
                Db.ActivityOwners.Add(owner);
            }

            Db.Activities.Add(course);

            var teaching = new SubjectTeaching
            {
                Course = course,
                Subject = subject,
            };

            Db.SubjectTeachings.Add(teaching);

            Db.SaveChanges();

            return RedirectToAction("Details", new { id = course.Id });
        }




        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public PartialViewResult GetMyCourseList()
        {
            var member = GetMyMembership();

            if (member != null)
            {
                var model =
                    Db.Activities.OfType<Course>().Where(c => c.Owners.Any(o => o.Member.Id == member.Id)).ToList();
                return PartialView("_CourseTable", model);
            }

            return PartialView("_CourseTable", new List<Course>());
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="group"></param>
        /// <param name="day"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="room"></param>
        /// <param name="doz"></param>
        /// <returns></returns>
        [HttpPost]
        public PartialViewResult CheckConflicts(string group, int day, TimeSpan from, TimeSpan to, string room,
            string doz)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">CourseId</param>
        /// <returns></returns>
        public ActionResult EditCourse(Guid id)
        {
            var course = Db.Activities.OfType<Course>().SingleOrDefault(c => c.Id == id);


            return View(course);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult UpdateGeneralSettings(Course model)
        {
            var course = Db.Activities.OfType<Course>().SingleOrDefault(c => c.Id == model.Id);

            if (!string.IsNullOrEmpty(model.Name))
                course.Name = model.Name;

            if (!string.IsNullOrEmpty(model.ShortName))
                course.ShortName = model.ShortName;

            if (!string.IsNullOrEmpty(model.UrlMoodleCourse))
                course.UrlMoodleCourse = model.UrlMoodleCourse;

            if (!string.IsNullOrEmpty(model.Description))
                course.Description = model.Description;

            Db.SaveChanges();

            return RedirectToAction("Index", new { id = course.Id });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="courseId"></param>
        /// <param name="semGroupId"></param>
        /// <param name="capacity"></param>
        /// <returns></returns>
        [HttpPost]
        public PartialViewResult AddGroup(Guid courseId, Guid semGroupId, int capacity)
        {
            var course = Db.Activities.OfType<Course>().SingleOrDefault(c => c.Id == courseId);

            var semGroup = Db.SemesterGroups.SingleOrDefault(g => g.Id == semGroupId);

            // TODO: ERROR pages
            if (semGroup == null || course == null)
                return null;

            // Semestergruppe in Studenplanzuordnung

            // Semestergruppen dürfen nicht doppelt vorkommen
            if (!course.SemesterGroups.Contains(semGroup))
            {
                course.SemesterGroups.Add(semGroup);
            }

            // TODO: aktuell dürfen noch keine Gruppen vorkommen, die mehr als eine Semestergruppe haben
            // zudem gilt auch hier: jede Semestergruppe darf einmal vorkommen
            var group = course.Occurrence.Groups.FirstOrDefault(g => g.SemesterGroups.Contains(semGroup));
            if (group == null)
            {
                // eine neue Gruppe muss her
                var occGroup = new OccurrenceGroup
                {
                    FitToCurriculumOnly = false,
                    Capacity = capacity,
                };

                occGroup.SemesterGroups.Add(semGroup);

                course.Occurrence.Groups.Add(occGroup);
            }
            else
            {
                group.Capacity = capacity;
            }

            Db.SaveChanges();


            var courseService = new CourseService(Db);
            var groups = courseService.GetSubscriptionGroups(course);

            return PartialView("_GroupList", groups);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="courseId"></param>
        /// <param name="groupId"></param>
        /// <returns></returns>
        [HttpPost]
        public PartialViewResult RemoveGroup(Guid courseId, Guid groupId)
        {
            var course = Db.Activities.OfType<Course>().SingleOrDefault(c => c.Id == courseId);
            var semGroup = Db.SemesterGroups.SingleOrDefault(c => c.Id == groupId);


            if (course != null && semGroup != null && course.SemesterGroups.Contains(semGroup))
            {
                course.SemesterGroups.Remove(semGroup);
                Db.SaveChanges();
            }

            // TODO: aktuell dürfen noch keine Gruppen vorkommen, die mehr als eine Semestergruppe haben
            // zudem gilt auch hier: jede Semestergruppe darf einmal vorkommen
            var group = course.Occurrence.Groups.FirstOrDefault(g => g.SemesterGroups.Contains(semGroup));
            if (group != null)
            {
                course.Occurrence.Groups.Remove(group);
                Db.SaveChanges();
            }


            // Dummy, da nix zurückgegeben wird, die Zeile wird gelöscht
            return PartialView("_RemoveSubscription");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult UpdateOccurrenceSettings(Course model)
        {
            var course = Db.Activities.OfType<Course>().SingleOrDefault(c => c.Id == model.Id);

            if (course != null)
            {
                course.Occurrence.Capacity = model.Occurrence.Capacity;
                Db.SaveChanges();
            }

            return RedirectToAction("Index", new { id = course.Id });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">CourseId</param>
        /// <param name="uploadFile"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult UploadRessource(Guid id, HttpPostedFileBase uploadFile)
        {
            var course = Db.Activities.OfType<Course>().SingleOrDefault(c => c.Id == id);

            var storage = course.Ressources.SingleOrDefault(r => r.Category.Equals("Modulbeschreibung"));

            if (uploadFile != null)
            {
                if (storage == null)
                {
                    storage = new BinaryStorage
                    {
                        Category = "Modulbeschreibung",
                    };

                    Db.Storages.Add(storage);
                    course.Ressources.Add(storage);
                    Db.SaveChanges();
                }


                storage.FileType = uploadFile.ContentType;
                storage.BinaryData = new byte[uploadFile.ContentLength];

                uploadFile.InputStream.Read(storage.BinaryData, 0, uploadFile.ContentLength);

                Db.SaveChanges();
            }

            return RedirectToAction("EditCourse", new { id = course.Id });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult UploadDocument(Guid id)
        {
            var model = new CourseDocumentUploadModel { CourseId = id };

            return View(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult UploadDocument(CourseDocumentUploadModel model)
        {
            var course = Db.Activities.OfType<Course>().SingleOrDefault(c => c.Id == model.CourseId);

            if (course != null)
            {
                var storage = new BinaryStorage
                {
                    Category = "Material",
                    Name = Path.GetFileName(model.Document.FileName),
                    Description = model.Description,
                };

                Db.Storages.Add(storage);
                course.Ressources.Add(storage);
                Db.SaveChanges();

                storage.FileType = model.Document.ContentType;
                storage.BinaryData = new byte[model.Document.ContentLength];

                model.Document.InputStream.Read(storage.BinaryData, 0, model.Document.ContentLength);

                Db.SaveChanges();

                return RedirectToAction("Index", new { id = course.Id });
            }

            return View(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="CourseId"></param>
        /// <param name="DocId"></param>
        /// <returns></returns>
        [HttpPost]
        public PartialViewResult DeleteDocument(Guid CourseId, Guid DocId)
        {
            var course = Db.Activities.OfType<Course>().SingleOrDefault(c => c.Id == CourseId);
            if (course != null)
            {
                var doc = course.Ressources.SingleOrDefault(r => r.Id == DocId);
                if (doc != null)
                {
                    course.Ressources.Remove(doc);
                    Db.Storages.Remove(doc);
                    Db.SaveChanges();
                }
            }

            return PartialView("_RemoveSubscription");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        /*
        [HttpPost]
        public PartialViewResult ChangeCapacitySettings(CourseDetailViewModel model)
        {
            if (model.SelectedOption == null)
                return PartialView("_SaveSuccess");

            var course = Db.Activities.OfType<Course>().SingleOrDefault(c => c.Id == model.Course.Id);
            if (course == null)
                return PartialView("_SaveSuccess");

            switch (model.SelectedOption.Id)
            {
                case 1:
                    // keine Beschränkung
                    course.Occurrence.Capacity = -1;
                    course.Occurrence.UseGroups = false;
                    course.Occurrence.UseExactFit = false;
                    break;
                case 2:
                    // Beschränkung auf globaler ebene
                    course.Occurrence.Capacity = model.SelectedOption.Capacity;
                    course.Occurrence.UseGroups = false;
                    course.Occurrence.UseExactFit = false;
                    break;
                case 3:
                    course.Occurrence.Capacity = 0;
                    course.Occurrence.UseGroups = true;
                    course.Occurrence.UseExactFit = false;
                    break;
                case 4:
                    course.Occurrence.Capacity = 0;
                    course.Occurrence.UseGroups = true;
                    course.Occurrence.UseExactFit = true;
                    break;
                default:
                    break;
            }

            course.Occurrence.IsCoterie = model.IsCoterie;
            course.Occurrence.HasHomeBias = model.HasHomeBias;

            Db.SaveChanges();

            return PartialView("_SaveSuccess");
        }
        */

        /// <summary>
        /// Absage über Schaltfläche
        /// </summary>
        /// <param name="dateId"></param>
        /// <returns></returns>
        public ActionResult CancelDate2(Guid dateId)
        {
            var date = Db.ActivityDates.SingleOrDefault(d => d.Id == dateId);

            // Alle Räume freigeben
            // Die Zuordnung zum Dozent bleibt aber bestehen!
            if (date != null)
            {
                date.Occurrence.IsCanceled = true;
                date.Rooms.Clear();
                date.VirtualRooms.Clear();

                Db.SaveChanges();
            }

            var course = date.Activity as Course;
            var userRights = GetUserRight(User.Identity.Name, course);
            ViewBag.UserRight = userRights;


            return RedirectToAction("AdminNewDates", new { id = course.Id });
        }

        /// <summary>
        /// Absage über Menüpunkt
        /// </summary>
        /// <param name="dateIds">Liste der ausgewählten Termine</param>
        /// <returns></returns>
        [HttpPost]
        public PartialViewResult CancelDateList(ICollection<Guid> dateIds)
        {
            Course course = null;
            var changeService = new ChangeService(Db);

            foreach (var dateId in dateIds)
            {
                var date = Db.ActivityDates.SingleOrDefault(d => d.Id == dateId);

                // Alle Räume freigeben
                // Die Zuordnung zum Dozent bleibt aber bestehen!
                if (date != null)
                {
                    // Absage => auch Raumänderung (Raum wird freigegeben)
                    var changeObject = changeService.CreateActivityDateStateChange(date, true);
                    if (changeObject != null)
                    {
                        changeObject.UserId = AppUser.Id;
                        Db.DateChanges.Add(changeObject);

                        Db.SaveChanges();

                        //NotificationService nservice = new NotificationService();
                        //nservice.CreateSingleNotification(changeObject.Id.ToString());
                    }

                    date.Occurrence.IsCanceled = true;
                    date.Rooms.Clear();
                    date.VirtualRooms.Clear();

                    course = date.Activity as Course;

                }

                Db.SaveChanges();



            }

            var userRights = GetUserRight(User.Identity.Name, course);
            ViewBag.UserRight = userRights;

            return PartialView("_DateTable", course);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dateId"></param>
        /// <returns></returns>
        public ActionResult ReactivateDate2(Guid dateId)
        {
            var date = Db.ActivityDates.SingleOrDefault(d => d.Id == dateId);

            if (date != null)
            {
                date.Occurrence.IsCanceled = false;
                Db.SaveChanges();
            }

            var course = date.Activity as Course;
            var userRights = GetUserRight(User.Identity.Name, course);
            ViewBag.UserRight = userRights;


            return RedirectToAction("AdminNewDates", new { id = course.Id });

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dateIds"></param>
        /// <returns></returns>
        [HttpPost]
        public PartialViewResult ReactivateDateList(ICollection<Guid> dateIds)
        {
            Course course = null;
            var changeService = new ChangeService(Db);

            foreach (var dateId in dateIds)
            {
                var date = Db.ActivityDates.SingleOrDefault(d => d.Id == dateId);

                // Alle Räume freigeben
                // Die Zuordnung zum Dozent bleibt aber bestehen!
                if (date != null)
                {
                    date.Occurrence.IsCanceled = false;
                    course = date.Activity as Course;
                }

                Db.SaveChanges();



            }

            var userRights = GetUserRight(User.Identity.Name, course);
            ViewBag.UserRight = userRights;

            return PartialView("_DateTable", course);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dateId"></param>
        /// <returns></returns>
        public ActionResult DeleteDateConfirm(Guid dateId)
        {
            var date = Db.ActivityDates.SingleOrDefault(d => d.Id == dateId);
            return View("ConfirmDeleteDate", date);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dateId"></param>
        /// <returns></returns>
        public ActionResult DeleteDateConfirmed(Guid dateId)
        {
            var date = Db.ActivityDates.SingleOrDefault(d => d.Id == dateId);
            var activity = date.Activity as Course;

            DeleteService.DeleteActivityDate(dateId);

            var userRights = GetUserRight(User.Identity.Name, activity);
            ViewBag.UserRight = userRights;


            return RedirectToAction("AdminNewDates", new { id = activity.Id });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dateIds"></param>
        /// <returns></returns>
        [HttpPost]
        public PartialViewResult DeleteDateList(ICollection<Guid> dateIds)
        {
            Course course = null;
            foreach (var dateId in dateIds)
            {
                var date = Db.ActivityDates.SingleOrDefault(d => d.Id == dateId);
                if (course == null && date != null)
                    course = date.Activity as Course;

                DeleteService.DeleteActivityDate(dateId);
            }


            var userRights = GetUserRight(User.Identity.Name, course);
            ViewBag.UserRight = userRights;


            return PartialView("_DateTable", course);
        }

        /// <summary>
        /// Löscht bei allen angegebenen Terminen die Rauminformation
        /// </summary>
        /// <param name="dateIds"></param>
        /// <returns></returns>
        [HttpPost]
        public PartialViewResult RemoveRooms(ICollection<Guid> dateIds)
        {
            Course course = null;

            var changeService = new ChangeService(Db);

            foreach (var dateId in dateIds)
            {
                var date = Db.ActivityDates.SingleOrDefault(d => d.Id == dateId);

                // Alle Räume freigeben
                if (date != null)
                {
                    date.Rooms.Clear();

                    if (course == null)
                        course = date.Activity as Course;
                }

                Db.SaveChanges();


            }

            var userRights = GetUserRight(User.Identity.Name, course);
            ViewBag.UserRight = userRights;

            return PartialView("_DateTable", course);
        }

        /// <summary>
        /// Löscht bei allen angegebenen Terminen die Rauminformation
        /// </summary>
        /// <param name="dateIds"></param>
        /// <returns></returns>
        [HttpPost]
        public PartialViewResult RemoveVirtualRooms(ICollection<Guid> dateIds)
        {
            Course course = null;

            foreach (var dateId in dateIds)
            {
                var date = Db.ActivityDates.SingleOrDefault(d => d.Id == dateId);

                // Alle Räume freigeben
                if (date != null)
                {
                    date.VirtualRooms.Clear();

                    if (course == null)
                        course = date.Activity as Course;
                }

                Db.SaveChanges();
            }

            var userRights = GetUserRight(User.Identity.Name, course);
            ViewBag.UserRight = userRights;

            return PartialView("_DateTable", course);
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="dateIds"></param>
        /// <param name="roomId"></param>
        /// <returns></returns>
        [HttpPost]
        public PartialViewResult AddRoomToDates(ICollection<Guid> dateIds, Guid roomId)
        {
            var room = Db.Rooms.SingleOrDefault(r => r.Id == roomId);

            Course course = null;

            foreach (var dateId in dateIds)
            {
                var date = Db.ActivityDates.SingleOrDefault(d => d.Id == dateId);

                if (date != null)
                {
                    if (room != null)
                    {
                        date.Rooms.Add(room);
                    }

                    if (course == null)
                        course = date.Activity as Course;
                }

                Db.SaveChanges();


            }

            var userRights = GetUserRight(User.Identity.Name, course);
            ViewBag.UserRight = userRights;


            return PartialView("_DateTable", course);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="dateIds"></param>
        /// <param name="roomId"></param>
        /// <returns></returns>
        [HttpPost]
        public PartialViewResult AddVirtualRoomToDates(ICollection<Guid> dateIds, Guid roomId)
        {
            var room = Db.VirtualRooms.SingleOrDefault(r => r.Id == roomId);

            Course course = null;

            foreach (var dateId in dateIds)
            {
                var date = Db.ActivityDates.SingleOrDefault(d => d.Id == dateId);

                if (date != null)
                {
                    if (room != null)
                    {
                        var roomAssign = date.VirtualRooms.FirstOrDefault(x => x.Room.Id == room.Id);
                        if (roomAssign == null)
                        {
                            roomAssign = new VirtualRoomAccess
                            {
                                Date = date,
                                Room = room,
                                isDefault = true
                            };


                            date.VirtualRooms.Add(roomAssign);

                            Db.VirtualRoomAccesses.Add(roomAssign);
                        }


                    }

                    if (course == null)
                        course = date.Activity as Course;
                }


            }

            Db.SaveChanges();

            var userRights = GetUserRight(User.Identity.Name, course);
            ViewBag.UserRight = userRights;


            return PartialView("_DateTable", course);
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="dateIds"></param>
        /// <param name="hostId"></param>
        /// <returns></returns>
        [HttpPost]
        public PartialViewResult AddHostToDates(ICollection<Guid> dateIds, Guid hostId)
        {
            var host = Db.Members.SingleOrDefault(r => r.Id == hostId);

            Course course = null;
            foreach (var dateId in dateIds)
            {
                var date = Db.ActivityDates.SingleOrDefault(d => d.Id == dateId);

                if (date != null)
                {
                    if (host != null)
                    {
                        date.Hosts.Add(host);
                    }

                    if (course == null)
                        course = date.Activity as Course;
                }

                Db.SaveChanges();
            }

            var userRights = GetUserRight(User.Identity.Name, course);
            ViewBag.UserRight = userRights;

            return PartialView("_DateTable", course);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dateIds"></param>
        /// <returns></returns>
        [HttpPost]
        public PartialViewResult RemoveHosts(ICollection<Guid> dateIds)
        {
            Course course = null;
            foreach (var dateId in dateIds)
            {
                var date = Db.ActivityDates.SingleOrDefault(d => d.Id == dateId);

                // Alle Räume freigeben
                if (date != null)
                {
                    date.Hosts.Clear();

                    if (course == null)
                        course = date.Activity as Course;
                }

                Db.SaveChanges();
            }

            var userRights = GetUserRight(User.Identity.Name, course);
            ViewBag.UserRight = userRights;

            return PartialView("_DateTable", course);
        }




        /// <summary>
        /// 
        /// </summary>
        /// <param name="dateId"></param>
        /// <returns></returns>

        public ActionResult ChangeDateInformation(Guid dateId)
        {
            var date = Db.ActivityDates.SingleOrDefault(d => d.Id == dateId);

            var model = new CourseDateInformationModel
            {
                DateId = date.Id,
                Title = date.Title,
                DateDescription = date.Description,
                ShortInfo = date.Occurrence.Information,
                Date = date
            };

            return View("EditInformation", model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ChangeDateInformationConfirmed(CourseDateInformationModel model)
        {
            var date = Db.ActivityDates.SingleOrDefault(d => d.Id == model.DateId);
            var course = date.Activity as Course;

            date.Title = model.Title;
            date.Description = model.DateDescription;
            date.Occurrence.Information = model.ShortInfo;

            Db.SaveChanges();

            var userRights = GetUserRight(User.Identity.Name, course);
            ViewBag.UserRight = userRights;

            return RedirectToAction("AdminNewDates", new { id = course.Id });
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Lock(Guid id)
        {
            var occurrence = Db.Occurrences.SingleOrDefault(oc => oc.Id == id);
            occurrence.IsAvailable = false;
            Db.SaveChanges();

            var course = Db.Activities.OfType<Course>().SingleOrDefault(c => c.Occurrence.Id == id);

            return RedirectToAction("Details", new { id = course.Id });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult UnLock(Guid id)
        {
            var occurrence = Db.Occurrences.SingleOrDefault(oc => oc.Id == id);
            occurrence.IsAvailable = true;
            Db.SaveChanges();

            var course = Db.Activities.OfType<Course>().SingleOrDefault(c => c.Occurrence.Id == id);

            return RedirectToAction("Details", new { id = course.Id });
        }




        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Freeze(Guid id)
        {
            var occurrence = Db.Activities.OfType<Course>().SingleOrDefault(oc => oc.Id == id);
            occurrence.IsInternal = true;
            Db.SaveChanges();

            return RedirectToAction("Details", new { id = id });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult UnFreeze(Guid id)
        {
            var occurrence = Db.Activities.OfType<Course>().SingleOrDefault(oc => oc.Id == id);
            occurrence.IsInternal = false;
            Db.SaveChanges();

            return RedirectToAction("Details", new { id = id });
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult SubscriptionList(Guid id)
        {
            var model = Db.Activities.OfType<Course>().SingleOrDefault(x => x.Id == id);
            return View(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="c"></param>
        /// <param name="withPart"></param>
        /// <param name="withWaiting"></param>
        /// <param name="withDates"></param>
        /// <returns></returns>
        public ActionResult SubscriptionList(Course c, bool withPart, bool withWaiting, bool withDates)
        {
            var model = Db.Activities.OfType<Course>().SingleOrDefault(x => x.Id == c.Id);
            return View(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Owners(Guid id)
        {
            var model = Db.Activities.OfType<Course>().SingleOrDefault(x => x.Id == id);

            ViewBag.Organiser = Db.Organisers.Select(c => new SelectListItem
            {
                Text = c.ShortName,
                Value = c.Id.ToString(),
            });


            return View(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult DeleteOwner(Guid id)
        {
            var owner = Db.ActivityOwners.SingleOrDefault(x => x.Id == id);

            var activity = owner.Activity;

            activity.Owners.Remove(owner);
            Db.ActivityOwners.Remove(owner);
            Db.SaveChanges();

            return RedirectToAction("Owners", new { id = activity.Id });

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="courseId"></param>
        /// <param name="dozId"></param>
        /// <returns></returns>
        public PartialViewResult AddOwner(Guid courseId, Guid dozId)
        {
            var course = Db.Activities.OfType<Course>().SingleOrDefault(x => x.Id == courseId);
            var member = Db.Members.SingleOrDefault(x => x.Id == dozId);

            var owner = course.Owners.FirstOrDefault(x => x.Member.Id == member.Id);
            if (owner == null)
            {
                owner = new ActivityOwner
                {
                    Activity = course,
                    Member = member,
                };

                Db.ActivityOwners.Add(owner);
                Db.SaveChanges();
            }

            return PartialView("_OwnerRow", owner);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Download(Guid id)
        {
            var course = Db.Activities.OfType<Course>().SingleOrDefault(c => c.Id == id);
            var org = GetMyOrganisation();

            var courseService = new CourseService(Db);

            var model = new CourseDetailViewModel()
            {
                Course = course,
                Summary = courseService.GetCourseSummary(id),
            };


            var userRight = GetUserRight(User.Identity.Name, model.Course);
            ViewBag.UserRight = userRight;

            if (userRight.IsHost || userRight.IsOwner || userRight.IsCourseAdmin)
                return View(model);

            return RedirectToAction("Details", new { id = id });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public FileResult ParticipientTable(Guid id)
        {
            var course = Db.Activities.OfType<Course>().SingleOrDefault(x => x.Id == id);


            var ms = new MemoryStream();
            var writer = new StreamWriter(ms, Encoding.Default);

            writer.WriteLine("LV Name;{0}", course.Name);
            writer.WriteLine("LV Kurzname;{0}", course.ShortName);


            writer.Write("Name;Vorname");

            foreach (var date in course.Dates.OrderBy(x => x.Begin))
            {
                writer.Write(";{0}", date.Begin.ToShortDateString());
            }

            writer.Write(Environment.NewLine);

            foreach (var subscription in course.Occurrence.Subscriptions.OrderBy(s => s.TimeStamp))
            {
                var user = UserManager.FindById(subscription.UserId);
                if (user != null)
                {
                    writer.Write("{0};{1}",
                        user.LastName, user.FirstName);
                }
                else
                {
                    writer.Write("n.n.;kein Benutzerkonto");

                }

                writer.Write(Environment.NewLine);
            }

            writer.Flush();
            writer.Dispose();

            var sb = new StringBuilder();
            sb.Append("Teilnehmer_");
            sb.Append(course.ShortName);
            sb.Append("_");
            sb.Append(DateTime.Today.ToString("yyyyMMdd"));
            sb.Append(".csv");

            return File(ms.GetBuffer(), "text/csv", sb.ToString());

        }

        private List<SubscriptionDetailViewModel> FillUserList(List<SubscriptionDetailViewModel> list)
        {
            // Hier die Listen durchgehen und die user dran setzen

            foreach (var viewModel in list)
            {
                viewModel.User = UserManager.FindById(viewModel.Subscription.UserId);
            }

            return list;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult AdminNewParticipients(Guid id)
        {
            var course = Db.Activities.OfType<Course>().SingleOrDefault(c => c.Id == id);
            var courseService = new CourseService(Db);

            var model = new CourseDetailViewModel()
            {
                Course = course,
                Summary = courseService.GetCourseSummary(course),
            };

            var userRight = GetUserRight(User.Identity.Name, model.Course);
            ViewBag.UserRight = userRight;

            if (userRight.IsHost || userRight.IsOwner || userRight.IsCourseAdmin)
                return View(model);

            return RedirectToAction("Details", new { id = id });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public PartialViewResult SubscriptionProfile(Guid id)
        {
            var studentService = new StudentService(Db);
            var courseService = new CourseService(Db);


            var subscription = Db.Subscriptions.OfType<OccurrenceSubscription>().SingleOrDefault(x => x.Id == id);

            var course = Db.Activities.OfType<Course>()
                .SingleOrDefault(x => x.Occurrence.Id == subscription.Occurrence.Id);

            var semester = SemesterService.GetSemester(DateTime.Today);
            if (course.Semester != null)
            {
                semester = course.Semester;
            }
            else
            {
                if (course.SemesterGroups.Any())
                {
                    semester = course.SemesterGroups.First().Semester;
                }
            }

            var user = UserManager.FindById(subscription.UserId);
            var student = studentService.GetCurrentStudent(subscription.UserId);


            var model = new UserCoursePlanViewModel
            {
                User = user,
                Student = student,
                Subscription = subscription,
                Semester = semester,
                Course = course,
                Summary = courseService.GetCourseSummary(course),
            };


            var courses =
                Db.Activities.OfType<Course>()
                    .Where(c => c.Occurrence.Subscriptions.Any(s => s.UserId.Equals(subscription.UserId)) &&
                                c.SemesterGroups.Any((g => g.Semester.Id == semester.Id)))
                    .OrderBy(c => c.Name)
                    .ToList();

            var courseSerive = new CourseService(Db);

            foreach (var c in courses)
            {
                var summary = courseSerive.GetCourseSummary(c);

                model.CourseSubscriptions.Add(new UserCourseSubscriptionViewModel
                {
                    CourseSummary = summary,
                    Subscription = subscription
                });

            }

            ViewBag.UserRight = GetUserRight(User.Identity.Name, course);

            return PartialView("_SubscriptionProfile", model);
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public ActionResult SetOnWaitingList2(Guid id, string userId)
        {
            var logger = LogManager.GetLogger("Booking");

            var occurrence = Db.Occurrences.SingleOrDefault(oc => oc.Id == id);
            var course = Db.Activities.OfType<Course>().SingleOrDefault(c => c.Occurrence.Id == id);
            var host = GetCurrentUser();

            if (occurrence != null && !string.IsNullOrEmpty(userId))
            {
                // immer doppelte reparieren
                var subscriptions =
                    occurrence.Subscriptions.Where(s => s.UserId.Equals(userId)).OrderBy(s => s.TimeStamp).ToList();

                var theOnlySubscription = subscriptions.LastOrDefault();

                if (subscriptions.Count > 1)
                {
                    foreach (var subscription in subscriptions)
                    {
                        if (subscription != subscriptions.Last())
                        {
                            var allDrawings = Db.SubscriptionDrawings.Where(x => x.Subscription.Id == subscription.Id)
                                .ToList();
                            foreach (var drawing in allDrawings)
                            {
                                Db.SubscriptionDrawings.Remove(drawing);
                            }

                            occurrence.Subscriptions.Remove(subscription);
                            Db.Subscriptions.Remove(subscription);
                        }
                    }

                    Db.SaveChanges();
                }

                if (theOnlySubscription != null)
                {
                    theOnlySubscription.OnWaitingList = true;
                    theOnlySubscription.LapCount = 0;
                    theOnlySubscription.IsConfirmed = false;
                    Db.SaveChanges();

                    var mailService = new SubscriptionMailService();
                    mailService.SendSubscriptionEMail(course, theOnlySubscription, host);

                    var subscriber = GetUser(theOnlySubscription.UserId);
                    logger.InfoFormat("{0} ({1}) for [{2}]: set on participient list",
                        course.Name, course.ShortName, subscriber.UserName);

                }
                else
                {
                    logger.ErrorFormat("subscription missing {0}, {1}", occurrence.Id, userId);
                }

            }
            else
            {
                logger.ErrorFormat("Occurrence or user missing [{0}], [{1}]", id, userId);
            }




            return RedirectToAction("AdminNewParticipients", new { id = course.Id });
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult SetOnWaitingList(Guid id)
        {
            var logger = LogManager.GetLogger("SubscribeActivity");

            var occurrence = Db.Occurrences.SingleOrDefault(oc => oc.Id == id);
            var course = Db.Activities.OfType<Course>().SingleOrDefault(c => c.Occurrence.Id == id);
            var host = GetCurrentUser();

            if (occurrence != null)
            {
                // nur die Teilnehmer
                var subscriptions = occurrence.Subscriptions.Where(x => !x.OnWaitingList).ToList();

                foreach (var subscription in subscriptions)
                {
                    subscription.OnWaitingList = true;
                    subscription.LapCount = 0;
                    subscription.IsConfirmed = false;
                    Db.SaveChanges();

                    var mailService = new SubscriptionMailService();
                    mailService.SendSubscriptionEMail(course, subscription, host);

                    var subscriber = GetUser(subscription.UserId);
                    logger.InfoFormat("{0} ({1}) for [{2}]: set on waiting list",
                        course.Name, course.ShortName, subscriber.UserName);
                }
            }


            return RedirectToAction("AdminNewParticipients", new { id = course.Id });
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public ActionResult SetOnParticipiantList2(Guid id, string userId)
        {
            var logger = LogManager.GetLogger("SubscribeActivity");

            var occurrence = Db.Occurrences.SingleOrDefault(oc => oc.Id == id);
            var course = Db.Activities.OfType<Course>().SingleOrDefault(c => c.Occurrence.Id == id);
            var host = GetCurrentUser();

            if (occurrence != null && !string.IsNullOrEmpty(userId))
            {
                // immer doppelte reparieren
                var subscriptions =
                    occurrence.Subscriptions.Where(s => s.UserId.Equals(userId)).OrderBy(s => s.TimeStamp).ToList();

                var theOnlySubscription = subscriptions.LastOrDefault();

                if (subscriptions.Count > 1)
                {
                    foreach (var subscription in subscriptions)
                    {
                        if (subscription != subscriptions.Last())
                        {
                            var allDrawings = Db.SubscriptionDrawings.Where(x => x.Subscription.Id == subscription.Id)
                                .ToList();
                            foreach (var drawing in allDrawings)
                            {
                                Db.SubscriptionDrawings.Remove(drawing);
                            }

                            occurrence.Subscriptions.Remove(subscription);
                            Db.Subscriptions.Remove(subscription);
                        }
                    }

                    Db.SaveChanges();
                }

                if (theOnlySubscription != null)
                {
                    theOnlySubscription.OnWaitingList = false;
                    theOnlySubscription.LapCount = 0;
                    theOnlySubscription.IsConfirmed = true;
                    Db.SaveChanges();

                    var mailService = new SubscriptionMailService();
                    mailService.SendSubscriptionEMail(course, theOnlySubscription, host);

                    var subscriber = GetUser(theOnlySubscription.UserId);
                    logger.InfoFormat("{0} ({1}) for [{2}]: set on participient list",
                        course.Name, course.ShortName, subscriber.UserName);
                }
                else
                {
                    logger.ErrorFormat("subscription missing {0}, {1}", occurrence.Id, userId);
                }

            }
            else
            {
                logger.ErrorFormat("Occurrence or user missing [{0}], [{1}]", id, userId);
            }



            return RedirectToAction("AdminNewParticipients", new { id = course.Id });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult SetOnParticipiantList(Guid id)
        {
            var logger = LogManager.GetLogger("SubscribeActivity");

            var occurrence = Db.Occurrences.SingleOrDefault(oc => oc.Id == id);
            var course = Db.Activities.OfType<Course>().SingleOrDefault(c => c.Occurrence.Id == id);
            var host = GetCurrentUser();

            if (occurrence != null)
            {
                // nur die, die auf Warteliste sind
                var subscriptions = occurrence.Subscriptions.Where(x => x.OnWaitingList).ToList();

                foreach (var subscription in subscriptions)
                {
                    subscription.OnWaitingList = false;
                    subscription.LapCount = 0;
                    subscription.IsConfirmed = false;

                    Db.SaveChanges();

                    var mailService = new SubscriptionMailService();
                    mailService.SendSubscriptionEMail(course, subscription, host);

                    var subscriber = GetUser(subscription.UserId);
                    logger.InfoFormat("{0} ({1}) for [{2}]: set on participient list",
                        course.Name, course.ShortName, subscriber.UserName);
                }
            }


            return RedirectToAction("AdminNewParticipients", new { id = course.Id });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public ActionResult RemoveSubscription2(Guid id)
        {
            var logger = LogManager.GetLogger("DischargeActivity");

            var subscription = Db.Subscriptions.OfType<OccurrenceSubscription>().SingleOrDefault(s => s.Id == id);
            var course = Db.Activities.OfType<Course>()
                .SingleOrDefault(c => c.Occurrence.Id == subscription.Occurrence.Id);
            var host = GetCurrentUser();



            if (subscription != null)
            {
                var subService = new SubscriptionService(Db);
                subService.DeleteSubscription(subscription);


                var subscriber = GetUser(subscription.UserId);

                if (subscriber != null)
                {
                    var mailService = new SubscriptionMailService();
                    mailService.SendSubscriptionEMail(course, subscriber.Id, host);

                    logger.InfoFormat("{0} ({1}) for [{2}]: removed from occurrence",
                        course.Name, course.ShortName, subscriber.UserName);
                }
            }
            else
            {
                logger.ErrorFormat("subscription missing {0}", id);
            }



            return RedirectToAction("AdminNewParticipients", new { id = course.Id });
        }

        /*
        public ActionResult RemoveSubscription2(Guid id, string userId)
        {
            var logger = LogManager.GetLogger("DischargeActivity");

            var occurrence = Db.Occurrences.SingleOrDefault(oc => oc.Id == id);
            var course = Db.Activities.OfType<Course>().SingleOrDefault(c => c.Occurrence.Id == id);
            var host = GetCurrentUser();

            if (occurrence != null && !string.IsNullOrEmpty(userId))
            {
                var subscription =
                    occurrence.Subscriptions.FirstOrDefault(s => s.UserId.Equals(userId));

                if (subscription != null)
                {
                    var subService = new SubscriptionService(Db);
                    subService.DeleteSubscription(subscription);

                    var mailService = new SubscriptionMailService();
                    mailService.SendSubscriptionEMail(course, userId, host);

                    var subscriber = GetUser(subscription.UserId);
                    logger.InfoFormat("{0} ({1}) for [{2}]: removed from occurrence",
                        course.Name, course.ShortName, subscriber.UserName);
                }
                else
                {
                    logger.ErrorFormat("subscription missing {0}, {1}", occurrence.Id, userId);
                }

            }
            else
            {
                logger.ErrorFormat("Occurrence or user missing [{0}], [{1}]", id, userId);
            }


            return RedirectToAction("AdminNewParticipients", new { id = course.Id });
        }
        */

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult RemoveSubscription(Guid id)
        {
            var logger = LogManager.GetLogger("DischargeActivity");

            var occurrence = Db.Occurrences.SingleOrDefault(oc => oc.Id == id);
            var course = Db.Activities.OfType<Course>().SingleOrDefault(c => c.Occurrence.Id == id);
            var host = GetCurrentUser();

            if (occurrence != null)
            {
                // immer doppelte reparieren
                var subscriptions = occurrence.Subscriptions.ToList();

                foreach (var subscription in subscriptions)
                {
                    var subService = new SubscriptionService(Db);
                    subService.DeleteSubscription(subscription);

                    var mailService = new SubscriptionMailService();
                    mailService.SendSubscriptionEMail(course, subscription.UserId, host);

                    var subscriber = GetUser(subscription.UserId);
                    logger.InfoFormat("{0} ({1}) for [{2}]: removed from occurrence",
                        course.Name, course.ShortName, subscriber.UserName);

                }
            }


            return RedirectToAction("AdminNewParticipients", new { id = course.Id });
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult AdminNewInfos(Guid id)
        {
            var course = Db.Activities.OfType<Course>().SingleOrDefault(c => c.Id == id);

            var courseService = new CourseService(Db);


            var model = new CourseDetailViewModel()
            {
                Course = course,
                Summary = courseService.GetCourseSummary(course),
                Description2 = course.Description
            };

            var str = "# Heading\r\nSome *embedded* Markdown which `md-block` can convert for you!";

            var _pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().UseBootstrap().Build();

            var result = Markdown.ToHtml(str, _pipeline);

            ViewBag.MarkdownHtml = result;

            return View(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AdminNewInfos(CourseDetailViewModel model)
        {
            var course = Db.Activities.OfType<Course>().SingleOrDefault(c => c.Id == model.Course.Id);

            if (course != null)
            {
                if (!string.IsNullOrEmpty(model.Course.Name) &&
                    !model.Course.Name.Equals(course.Name))
                {
                    course.Name = model.Course.Name;
                }

                if (!string.IsNullOrEmpty(model.Course.ShortName) &&
                    !model.Course.ShortName.Equals(course.ShortName))
                {
                    course.ShortName = model.Course.ShortName;
                }

                course.Description = model.Description2;
                course.UrlMoodleCourse = model.Course.UrlMoodleCourse;
                course.KeyMoodleCourse = model.Course.KeyMoodleCourse;

                Db.SaveChanges();
            }

            return RedirectToAction("Details", new { id = course.Id });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult AdminNewRules(Guid id)
        {
            var course = Db.Activities.OfType<Course>().SingleOrDefault(c => c.Id == id);

            var courseService = new CourseService(Db);


            var model = new CourseDetailViewModel()
            {
                Course = course,
                Summary = courseService.GetCourseSummary(course),
            };

            var userRight = GetUserRight(User.Identity.Name, model.Course);
            ViewBag.UserRight = userRight;

            if (userRight.IsHost || userRight.IsOwner || userRight.IsCourseAdmin)
                return View(model);

            return RedirectToAction("Details", new { id = id });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="courseId"></param>
        /// <param name="optAccess"></param>
        /// <param name="optLimit"></param>
        /// <param name="capacity"></param>
        /// <param name="groupIds"></param>
        /// <param name="groupCaps"></param>
        /// <returns></returns>
        /*
        [HttpPost]
        public ActionResult SaveAdminNewRules(Guid courseId, int optAccess, int optLimit, int capacity, Guid[] groupIds, int[] groupCaps)
        {
            var course = Db.Activities.OfType<Course>().SingleOrDefault(c => c.Id == courseId);

            if (course != null)
            {
                if (optAccess == 1)
                {
                    course.Occurrence.IsCoterie = false;
                    course.Occurrence.HasHomeBias = false;
                }
                else if (optAccess == 2)
                {
                    course.Occurrence.IsCoterie = false;
                    course.Occurrence.HasHomeBias = true;

                }
                else
                {
                    course.Occurrence.IsCoterie = true;
                    course.Occurrence.HasHomeBias = true;
                }

                if (optLimit == 1)
                {
                    course.Occurrence.Capacity = -1;
                    course.Occurrence.UseGroups = false;
                    course.Occurrence.UseExactFit = false;
                }
                else if (optLimit == 2)
                {
                    course.Occurrence.Capacity = capacity;
                    course.Occurrence.UseGroups = false;
                    course.Occurrence.UseExactFit = false;
                }
                else if (optLimit == 3)
                {
                    course.Occurrence.Capacity = -1;
                    course.Occurrence.UseGroups = true;
                    course.Occurrence.UseExactFit = false;

                    // die occurrence groups
                    for (int i = 0; i < groupIds.Length; i++)
                    {
                        var groupId = groupIds[i];
                        var group = course.Occurrence.Groups.SingleOrDefault(x => x.Id == groupId);
                        if (group != null)
                        {
                            group.Capacity = groupCaps[i];
                        }
                    }
                }
                else 
                {
                    course.Occurrence.Capacity = -1;
                    course.Occurrence.UseGroups = true;
                    course.Occurrence.UseExactFit = true;
                }

                Db.SaveChanges();
            }

            return Json(new { result = "Redirect", url = Url.Action("Details", new { id = course.Id }) });
        }
        */

        public ActionResult AddQuota(Guid id)
        {
            var course = Db.Activities.OfType<Course>().SingleOrDefault(c => c.Id == id);

            var model = new CourseLabelViewModel()
            {
                Course = course,
                Organisers = Db.Organisers.Where(x => !x.IsStudent && x.IsFaculty).OrderBy(x => x.ShortName).ToList()
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult AddQuota(Guid courseId, Guid currid, int capa)
        {
            var course = Db.Activities.OfType<Course>().SingleOrDefault(c => c.Id == courseId);
            var curr = Db.Curricula.SingleOrDefault(x => x.Id == currid);

            int cpacity = capa == -1 ? int.MaxValue : capa;

            if (curr != null)
            {
                // Aktuell keine doppelten zulassen
                if (course.Occurrence.SeatQuotas.Any(x => x.Curriculum.Id == curr.Id))
                {
                    return null;
                }

                var quota = new SeatQuota
                {
                    Curriculum = curr,
                    ItemLabelSet = new ItemLabelSet(),
                    MaxCapacity = cpacity,
                    MinCapacity = 0,
                    Description = string.Empty,
                    Occurrence = course.Occurrence
                };

                Db.SeatQuotas.Add(quota);
                Db.SaveChanges();
            }
            else
            {
                if (course.Occurrence.SeatQuotas.Any(x => x.Curriculum == null))
                {
                    return null;
                }

                var quota = new SeatQuota
                {
                    Curriculum = null,
                    ItemLabelSet = new ItemLabelSet(),
                    MaxCapacity = cpacity,
                    MinCapacity = 0,
                    Description = string.Empty,
                    Occurrence = course.Occurrence
                };

                Db.SeatQuotas.Add(quota);
                Db.SaveChanges();
            }

            return null;
        }

        public ActionResult DeleteQuota(Guid id)
        {
            var quota = Db.SeatQuotas.FirstOrDefault(x => x.Id == id);
            var course = Db.Activities.OfType<Course>().SingleOrDefault(x => x.Occurrence.Id == quota.Occurrence.Id);

            Db.SeatQuotas.Remove(quota);
            Db.SaveChanges();

            return RedirectToAction("AdminNewRules", new { id = course.Id });
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult AdminNewDates(Guid id)
        {
            var course = Db.Activities.OfType<Course>().SingleOrDefault(c => c.Id == id);
            var org = course.Organiser != null ? course.Organiser : GetMyOrganisation();

            var courseService = new CourseService(Db);

            var model = new CourseDetailViewModel()
            {
                Course = course,
                Summary = courseService.GetCourseSummary(course),
                Organiser = org
            };


            var userRight = GetUserRight(User.Identity.Name, model.Course);
            ViewBag.UserRight = userRight;

            if (userRight.IsHost || userRight.IsOwner || userRight.IsCourseAdmin)
                return View(model);

            return RedirectToAction("Details", new { id = id });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult CreateCourseWizard()
        {
            var org = GetMyOrganisation();

            var n = Db.Activities.OfType<Course>().Count();
            var t = DateTime.Now.ToString("yyyyMMdd_hhmmss");

            var name = $"Kurs {n} - {t}";


            var course = new Course
            {
                Name = name,
                ShortName = name,
                Organiser = org,
                Occurrence = new Occurrence
                {
                    Capacity = -1,
                    IsAvailable = false,
                    FromIsRestricted = false,
                    UntilIsRestricted = false,
                    IsCanceled = false,
                    IsMoved = false,
                    UseGroups = false,
                },
            };

            var member = GetMyMembership();

            if (member != null)
            {
                // das Objeklt muss aus dem gleichen Kontext kommen
                var me = Db.Members.SingleOrDefault(m => m.Id == member.Id);

                ActivityOwner owner = new ActivityOwner
                {
                    Activity = course,
                    Member = me,
                    IsLocked = false
                };

                course.Owners.Add(owner);
                Db.ActivityOwners.Add(owner);
            }

            Db.Activities.Add(course);
            Db.SaveChanges();



            return RedirectToAction("AdminNewInfos", new { id = course.Id });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult AdminNewModule(Guid id)
        {
            var course = Db.Activities.OfType<Course>().SingleOrDefault(c => c.Id == id);
            var courseService = new CourseService(Db);

            var model = new CourseDetailViewModel()
            {
                Course = course,
                Summary = courseService.GetCourseSummary(course),
            };

            // ein Modul finden
            /*
            if (course.Nexus.Any())
            {
                model.Module = course.Nexus.First().Requirement;
            }
            */


            return View(model);
        }

        public ActionResult RawAdmin(Guid id)
        {
            var model = Db.Activities.OfType<Course>().SingleOrDefault(x => x.Id == id);

            return View(model);
        }

        public ActionResult RawRemoveSubscription(Guid courseId, Guid id)
        {
            var subscription = Db.Subscriptions.SingleOrDefault(x => x.Id == id);
            Db.Subscriptions.Remove(subscription);
            Db.SaveChanges();

            return RedirectToAction("RawAdmin", new { id = courseId });
        }


        public ActionResult ParticipiantDetails(Guid id)
        {
            var studentService = new StudentService(Db);
            var courseService = new CourseService(Db);

            var subscription = Db.Subscriptions.OfType<OccurrenceSubscription>().SingleOrDefault(x => x.Id == id);
            var userId = subscription.UserId;

            var course = Db.Activities.OfType<Course>()
                .SingleOrDefault(x => x.Occurrence.Id == subscription.Occurrence.Id);

            var semester = course.Semester;

            var user = UserManager.FindById(subscription.UserId);
            var student = studentService.GetCurrentStudent(subscription.UserId);

            var model = new UserCoursePlanViewModel
            {
                User = user,
                Student = student,
                Subscription = subscription,
                Semester = semester,
                Course = course,
                Summary = courseService.GetCourseSummary(course),
            };


            var courses =
                Db.Activities.OfType<Course>()
                    .Where(c => c.Occurrence.Subscriptions.Any(s => s.UserId.Equals(subscription.UserId)) &&
                                c.Semester.Id == semester.Id)
                    .OrderBy(c => c.Name)
                    .ToList();

            var courseSerive = new CourseService(Db);

            foreach (var c in courses)
            {
                var summary = courseSerive.GetCourseSummary(c);
                var courseSubscription = c.Occurrence.Subscriptions.FirstOrDefault(x => x.UserId.Equals(userId));

                model.CourseSubscriptions.Add(new UserCourseSubscriptionViewModel
                {
                    CourseSummary = summary,
                    Subscription = courseSubscription
                });

            }

            ViewBag.UserRight = GetUserRight(User.Identity.Name, course);


            return View(model);
        }

        public ActionResult AdminNewLabels(Guid id)
        {
            var course = Db.Activities.OfType<Course>().SingleOrDefault(c => c.Id == id);


            return View(course);
        }

        public ActionResult EditLabel(Guid courseId, Guid labelId)
        {
            var course = Db.Activities.OfType<Course>().SingleOrDefault(c => c.Id == courseId);
            var label = course.LabelSet.ItemLabels.FirstOrDefault(x => x.Id == labelId);

            ViewBag.Course = course;

            return View(label);
        }

        public ActionResult RemoveLabel(Guid courseId, Guid labelId)
        {
            var course = Db.Activities.OfType<Course>().SingleOrDefault(c => c.Id == courseId);
            var label = course.LabelSet.ItemLabels.FirstOrDefault(x => x.Id == labelId);

            if (label != null)
            {
                course.LabelSet.ItemLabels.Remove(label);
                Db.SaveChanges();
            }

            return RedirectToAction("AdminNewLabels", new { id = course.Id });
        }



        public ActionResult ChangeLabels(Guid id)
        {
            var course = Db.Activities.OfType<Course>().SingleOrDefault(c => c.Id == id);

            var model = new CourseLabelViewModel()
            {
                Course = course,
                Organisers = Db.Organisers.Where(x => x.LabelSet != null && !x.IsStudent && x.Curricula.Any()).OrderBy(x => x.ShortName).ToList()
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult AssignCourseLabels(Guid courseId, Guid[] labelIds)
        {
            var course = Db.Activities.OfType<Course>().SingleOrDefault(c => c.Id == courseId);

            foreach (var labelId in labelIds)
            {
                var label = Db.ItemLabels.SingleOrDefault(x => x.Id == labelId);

                if (label != null && !course.LabelSet.ItemLabels.Contains(label))
                {
                    course.LabelSet.ItemLabels.Add(label);
                }
            }

            Db.SaveChanges();

            return null;
        }

        public ActionResult AdminNewSubjects(Guid id)
        {
            var course = Db.Activities.OfType<Course>().SingleOrDefault(c => c.Id == id);




            return View(course);
        }

        public ActionResult CreateTeaching(Guid id)
        {
            var course = Db.Activities.OfType<Course>().SingleOrDefault(c => c.Id == id);

            var orgs = Db.Organisers.Where(x => x.ModuleCatalogs.Any()).ToList();

            var model = new CourseLabelViewModel()
            {
                Course = course,
                Organisers = orgs
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult CreateTeaching(Guid courseId, Guid subjectId)
        {
            var course = Db.Activities.OfType<Course>().SingleOrDefault(c => c.Id == courseId);

            var subject = Db.ModuleCourses.SingleOrDefault(x => x.Id == subjectId);

            if (course != null && subject != null)
            {
                var teaching = course.SubjectTeachings.FirstOrDefault(x =>
                    x.Subject.Id == subjectId);

                if (teaching == null)
                {
                    teaching = new SubjectTeaching
                    {
                        Course = course,
                        Subject = subject,
                    };

                    Db.SubjectTeachings.Add(teaching);
                    Db.SaveChanges();
                }
            }


            return null;
        }



        public ActionResult AdminNewDictionary(Guid id)
        {
            var course = Db.Activities.OfType<Course>().SingleOrDefault(c => c.Id == id);

            // Alle Semester, die in Zukunft enden
            ViewBag.Semester = Db.Semesters
                .Where(x => x.EndCourses >= DateTime.Today)
                .OrderBy(s => s.StartCourses)
                .Take(3)
                .Select(c => new SelectListItem
                {
                    Text = c.Name,
                    Value = c.Id.ToString(),
                });

            var model = new CourseHistoryModel()
            {
                Course = course,
                Semester = course.Semester,
                SemesterId = course.Semester.Id,
            };


            return View(model);
        }

        [HttpPost]
        public ActionResult AdminNewDictionary(CourseHistoryModel model)
        {
            var course = Db.Activities.OfType<Course>().SingleOrDefault(c => c.Id == model.Course.Id);
            var semester = SemesterService.GetSemester(model.SemesterId);

            course.Semester = semester;
            Db.SaveChanges();

            return RedirectToAction("Details", new { id = course.Id });
        }

        public ActionResult AdminNewState(Guid id)
        {
            var course = Db.Activities.OfType<Course>().SingleOrDefault(c => c.Id == id);

            var model = new CourseStateModel()
            {
                Course = course,
                CourseId = course.Id,
                locked = course.Occurrence.IsAvailable ? 0 : 1,
                editable = course.IsInternal ? 0 : 1,
                projected = course.IsProjected ? 1 : 0
            };

            ViewBag.UserRight = GetUserRight(User.Identity.Name, course);

            return View(model);
        }

        [HttpPost]
        public ActionResult AdminNewState(CourseStateModel model)
        {
            var course = Db.Activities.OfType<Course>().SingleOrDefault(c => c.Id == model.CourseId);

            course.IsProjected = model.projected == 1;
            course.Occurrence.IsAvailable = model.locked != 1;
            course.IsInternal = model.editable != 1;

            Db.SaveChanges();

            return RedirectToAction("Details", new { id = course.Id });
        }

        [HttpPost]
        public ActionResult SetQuota(Guid courseId, Guid quotaId, int delta)
        {
            var quota = Db.SeatQuotas.SingleOrDefault(x => x.Id == quotaId);

            if (delta > 0)
            {
                quota.MaxCapacity = delta;
            }
            else
            {
                quota.MaxCapacity = int.MaxValue;
            }

            Db.SaveChanges();

            return RedirectToAction("AdminNewRules", new { id = courseId });
        }


        public ActionResult EditQuota(Guid id)
        {


            return View();
        }


        public ActionResult Copy(Guid id)
        {
            var course = Db.Activities.OfType<Course>().SingleOrDefault(c => c.Id == id);


            return View(course);
        }

    }
}