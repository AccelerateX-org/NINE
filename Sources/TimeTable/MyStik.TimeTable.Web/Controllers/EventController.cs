using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.WebPages;
using log4net;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.SignalR.Hubs;
using MyStik.TimeTable.Data;
using MyStik.TimeTable.DataServices;
using MyStik.TimeTable.Web.Models;
using MyStik.TimeTable.Web.Services;
using MyStik.TimeTable.Web.Helpers;

namespace MyStik.TimeTable.Web.Controllers
{
    public class EventController : BaseController
    {
        //
        // GET: /Activity/
        public ActionResult Index(Guid? id)
        {
            var events = Db.Activities.OfType<Event>().ToList();
            events = events.Where(ev => ev.Dates.Any() && ev.Dates.First().Begin >= GlobalSettings.Today).OrderBy(ev => ev.Dates.First().Begin).ToList();

            var modelList = new List<EventViewModel>();

            var semester = GetSemester();

            foreach (var @event in events)
            {
                var nextDate = @event.Dates.FirstOrDefault();

                modelList.Add(new EventViewModel
                {
                    Event = @event,
                    NextDate = nextDate,
                    State = ActivityService.GetActivityState(nextDate.Occurrence, AppUser, semester),
                });
            }

            ViewBag.UserRight = GetUserRight();

            return View(modelList);
        }

        public ActionResult IndexMobile()
          {

              var model = new EventMobileViewModel();

              model.DatumFeiertage = new List<Milan>();

              var semesters = Db.Semesters.ToList(); 

            foreach (var semester in semesters)
            {
                var xyz = new Milan
                {
                    Datum = semester.StartCourses,
                    Feiertage = "Vorlesungsbeginn"
                };
                model.DatumFeiertage.Add(xyz);

                var xyz2 = new Milan
                {
                    Datum = semester.EndCourses,
                    Feiertage = "Vorlesungsende"
                };
                model.DatumFeiertage.Add(xyz2);


                foreach (var termin in semester.Dates)
                {
                    var abc = new Milan
                    {
                        Datum = termin.From,
                        Feiertage = termin.Description
                    };
                    model.DatumFeiertage.Add(abc);
                }
            
            
            
            }

            return View(model);
        }

















        public ActionResult CreateEvent()
        {
            var sem = GetSemester();

            var org = GetMyOrganisation(); // Db.Organisers.SingleOrDefault(o => o.ShortName.Equals("FK 09"));


            EventCreateModel2 model = new EventCreateModel2();

            model.Semester = sem;
            model.SemesterId = sem.Id;
            model.OrgId = org.Id;

            return View(model);
        }

        public ActionResult CreateSeriesEvent(string id)
        {
            var memberService = new MemberService(Db, UserManager);
            var member = memberService.GetMember(User.Identity.Name, id);

            var org = Db.Organisers.SingleOrDefault(o => o.ShortName.ToUpper().Equals(id.ToUpper()));

            EventCreateSeriesModel model = new EventCreateSeriesModel()
            {
                OrgId = org.Id,
                Lecturer = member != null ? member.ShortName : string.Empty,
                Day = DateTime.Today,
                EndDay = DateTime.Today.AddDays(1),
                SubscriptionStartDay = DateTime.Today,
                SubscriptionEndDay = DateTime.Today.AddDays(1),
                Capacity = -1
            };

            SetTimeSelections();

            return View(model);
        }

        [HttpPost]
        public ActionResult CreateSeriesEvent(EventCreateSeriesModel model) {
            // Veranstalter abfragen (Organisation, bspw. FS09)
            var org = Db.Organisers.SingleOrDefault(o => o.Id == model.OrgId);

            // Veranstaltung anlegen
            var @event = new Event()
            {
                Name = model.Name,
                ShortName = model.ShortName,
                Description = model.Description,
                Occurrence = new Occurrence(),          // Die buchbarkeit findet auf Terminebene statt
                IsInternal = model.IsInternal,
                Organiser = org,
            };

            DateTime subscriptionStart = model.SubscriptionStartDay.AddHours(model.SubscriptionStartHour).AddMinutes(model.SubscriptionStartMinute);
            DateTime subscriptionEnd = model.SubscriptionEndDay.AddHours(model.SubscriptionEndHour).AddMinutes(model.SubscriptionEndMinute);

            // Veranstaltung speichern
            Db.Activities.Add(@event);
            Db.SaveChanges();

            // Abfrage des Dozenten/Veranstalter (Person)
            OrganiserMember doz;
            if (string.IsNullOrEmpty(model.Lecturer)) // Wenn das Feld nicht ausgefüllt wurde...
            {
                // ... wird der aktuell eingeloggte User verwendet
                var memberService = new MemberService(Db, UserManager);
                var member = memberService.GetMember(User.Identity.Name, org.ShortName);
                doz = org.Members.SingleOrDefault(m => m.ShortName.ToUpper().Equals(member.ShortName));
            }
            else // ansonsten wird der eingetragene Benutzer verwendet
            {
                doz = org.Members.SingleOrDefault(m => m.ShortName.ToUpper().Equals(model.Lecturer));
            }

            // Raumabfrage aus Datenbank
            var room = Db.Rooms.SingleOrDefault(r => r.Number.ToUpper().Equals(model.Room.ToUpper()));

            // Einzeltermine werden erzeugt
            var dates = new EventHelpers(@event, model);

            // Einzeltermine in Datenbank ablegen
            foreach (ActivityDate date in dates.dates)
            {
                date.Activity = @event;
                date.Hosts = new Collection<OrganiserMember> { doz };
                date.Rooms = new Collection<Room> { room };
                date.Occurrence = new Occurrence
                        {
                            Capacity = model.Capacity,
                            IsAvailable = true,
                            FromIsRestricted = model.IsSubscriptionStartRestricted,
                            FromDateTime = subscriptionStart,
                            UntilIsRestricted = model.IsSubscriptionEndRestricted,
                            UntilDateTime = subscriptionEnd,
                            IsCanceled = false,
                            IsMoved = false,
                        };
                Db.ActivityDates.Add(date);
                Db.SaveChanges();
            }

            // Fertig, leite weiter zu entsprechender Action
            return RedirectToAction("Events", "Organiser", new { id = org.ShortName });
        }

        [HttpPost]
        public ActionResult CreateEvent(EventCreateModel2 model)
        {
            var semester = Db.Semesters.SingleOrDefault(s => s.Id == model.SemesterId);
            var org = Db.Organisers.SingleOrDefault(o => o.Id == model.OrgId);

            var @event = new Event
            {
                Name = model.Name,
                ShortName = model.ShortName,
                Organiser = org,
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
                //Description = model.Description,
            };


            foreach (var groupId in model.GroupIds)
            {
                var semGroup = Db.SemesterGroups.SingleOrDefault(g => g.Id == groupId);

                if (semGroup != null)
                {
                    @event.SemesterGroups.Add(semGroup);

                    var occGroup = new OccurrenceGroup
                    {
                        Capacity = 0,
                        FitToCurriculumOnly = true,
                        Occurrence = @event.Occurrence
                    };
                    occGroup.SemesterGroups.Add(semGroup);
                    semGroup.OccurrenceGroups.Add(occGroup);
                    @event.Occurrence.Groups.Add(occGroup);
                    Db.OccurrenceGroups.Add(occGroup);
                }
            }

            var member = GetMyMembership();

            if (member != null)
            {
                // das Objeklt muss aus dem gleichen Kontext kommen
                var me = Db.Members.SingleOrDefault(m => m.Id == member.Id);

                ActivityOwner owner = new ActivityOwner
                {
                    Activity = @event,
                    Member = me,
                    IsLocked = false
                };

                @event.Owners.Add(owner);
                Db.ActivityOwners.Add(owner);
            }


            Db.Activities.Add(@event);
            Db.SaveChanges();

            return PartialView("_CreateEventSuccess", @event);
        }


        public ActionResult Admin(Guid? id)
        {
            if (id.HasValue)
            {
                var course = Db.Activities.OfType<Event>().SingleOrDefault(c => c.Id == id);
                if (course == null)
                    return RedirectToAction("MissingCourse", "Error");

                // nur Hosts und Admin dürfen die Seite aufrufen
                var userRights = GetUserRight(User.Identity.Name, course);
                if (!(userRights.IsHost || userRights.IsOrgAdmin))
                {
                    return RedirectToAction("Details", new {id = id});
                }



                if (string.IsNullOrEmpty(course.Name))
                    course.Name = "N.N.";

                if (string.IsNullOrEmpty(course.ShortName))
                    course.ShortName = "N.N.";


                // Alle Services, die benötigt werden

                var model = new EventDetailViewModel()
                {
                    Event = course,
                    Lecturers = Db.Members.Where(l => l.Dates.Any(occ => occ.Activity.Id == course.Id)).ToList(),
                    Description2 = course.Description,
                };


                ViewBag.Curricula = Db.Curricula.Select(c => new SelectListItem
                {
                    Text = c.ShortName,
                    Value = c.Id.ToString(),
                });

                var semester = GetSemester();

                var curr = Db.Curricula.First();

                if (curr != null)
                {

                    var semesterGroups = Db.SemesterGroups.Where(g =>
                        g.Semester.Id == semester.Id &&
                        g.CapacityGroup.CurriculumGroup.Curriculum.Id == curr.Id).OrderBy(g => g.CapacityGroup.CurriculumGroup.Name).ToList();


                    var semGroups = semesterGroups.Select(semGroup => new SelectListItem
                    {
                        Text = semGroup.FullName,
                        Value = semGroup.Id.ToString()
                    }).ToList();

                    ViewBag.Groups = semGroups;
                }

                ViewBag.UserRight = userRights;


                return View(model);
            }

            // Verwaltung aller Events
            var org = User.IsInRole("SysAdmin") ? 
                Db.Organisers.SingleOrDefault(o => o.ShortName.Equals("FK 09")) : 
                GetMyOrganisation();

            if (org != null)
            {
                var events = User.IsInRole("SysAdmin") ?
                    Db.Activities.OfType<Event>() :
                    Db.Activities.OfType<Event>().Where(ev => ev.Organiser.Id == org.Id);

                var modelList = new List<EventViewModel>();

                foreach (var @event in events)
                {
                    modelList.Add(new EventViewModel
                    {
                        Event = @event,
                        NextDate = null,
                        State = null
                    });
                }

                ViewBag.UserRight = GetUserRight();

                return View("EventAdminList", modelList);
            }

            return new EmptyResult();
        }


        public ActionResult Details(Guid id)
        {
            var course = Db.Activities.OfType<Event>().SingleOrDefault(c => c.Id == id);
            if (course == null)
                return RedirectToAction("MissingCourse", "Error");

            if (string.IsNullOrEmpty(course.Name))
                course.Name = "N.N.";

            if (string.IsNullOrEmpty(course.ShortName))
                course.ShortName = "N.N.";

            var model = new EventDetailViewModel()
            {
                Event = course,
                Lecturers = Db.Members.Where(l => l.Dates.Any(occ => occ.Activity.Id == course.Id)).ToList(),
            };

            ViewBag.UserRight = GetUserRight(User.Identity.Name, course);


            return View(model);
        }


        public ActionResult ChangeGroups(Guid id)
        {
            var course = Db.Activities.OfType<Event>().SingleOrDefault(c => c.Id == id);
            if (course == null)
                return RedirectToAction("MissingCourse", "Error");

            var model = new EventCreateModel2
            {
                Event = course,
                EventId = course.Id,
                Semester = GetSemester()
            };


            return View(model);
        }

        [HttpPost]
        public PartialViewResult ChangeGroups(EventCreateModel2 model)
        {
            var course = Db.Activities.OfType<Event>().SingleOrDefault(c => c.Id == model.EventId);
            if (course == null)
                return PartialView("_SaveSuccess");

            var sem = GetSemester();

            // nur die aktuellen Gruppen löschen
            var groupList = course.SemesterGroups.Where(g => g.Semester.Id == sem.Id).ToList();
            foreach (var group in groupList)
            {
                course.SemesterGroups.Remove(group);
            }

            if (model.GroupIds != null)
            {
                foreach (var groupId in model.GroupIds)
                {
                    course.SemesterGroups.Add(Db.SemesterGroups.SingleOrDefault(g => g.Id == groupId));
                }
            }

            Db.SaveChanges();


            return PartialView("_SaveSuccess");
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">ActivitiyDateId</param>
        /// <returns></returns>
        public ActionResult SetInfo(Guid id)
        {
            var date = Db.ActivityDates.SingleOrDefault(o => o.Id == id);


            var model = new EventDateInfoModel()
            {
                Course = date.Activity as Event,
                Date = date,
                ActivityDateId = date.Id,
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult SetInfo(EventDateInfoModel model)
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

        public ActionResult EditEvent(Guid id)
        {
            var @event = Db.Activities.OfType<Event>().SingleOrDefault(c => c.Id == id);

            var courseService = new EventService(id);

            EventDetailViewModel model = new EventDetailViewModel
            {
                Event = @event,
                Groups = courseService.GetSubscriptionGroups()
            };

            ViewBag.Curricula = Db.Curricula.Select(c => new SelectListItem
            {
                Text = c.ShortName,
                Value = c.Id.ToString(),
            });

            var semester = GetSemester();

            var curr = Db.Curricula.First();

            if (curr != null)
            {

                var semesterGroups = Db.SemesterGroups.Where(g =>
                    g.Semester.Id == semester.Id &&
                    g.CapacityGroup.CurriculumGroup.Curriculum.Id == curr.Id).OrderBy(g => g.CapacityGroup.CurriculumGroup.Name).ToList();


                var semGroups = semesterGroups.Select(semGroup => new SelectListItem
                {
                    Text = semGroup.FullName,
                    Value = semGroup.Id.ToString()
                }).ToList();

                ViewBag.Groups = semGroups;
            }

            ViewBag.UserRight = GetUserRight(User.Identity.Name, @event);


            return View(model);
        }

        [HttpPost]
        public ActionResult UpdateGeneralSettings(EventDetailViewModel model)
        {
            var course = Db.Activities.OfType<Event>().SingleOrDefault(c => c.Id == model.Event.Id);

            if (course != null)
            {
                if (!string.IsNullOrEmpty(model.Event.Name))
                    course.Name = model.Event.Name;

                if (!string.IsNullOrEmpty(model.Event.ShortName))
                    course.ShortName = model.Event.ShortName;

                if (!string.IsNullOrEmpty(model.Event.Description))
                    course.Description = model.Event.Description;

                course.Published = model.Event.Published;

                Db.SaveChanges();
            }

            return RedirectToAction("Index", new { id = course.Id });
        }

        [HttpPost]
        public PartialViewResult AddGroup(Guid eventId , Guid semGroupId)
        {
            var course = Db.Activities.OfType<Event>().SingleOrDefault(c => c.Id == eventId);

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
                // Events haben keine Kapazitätsbeschränkung auf Ebene von Gruppen
                var occGroup = new OccurrenceGroup
                {
                    FitToCurriculumOnly = false,
                    Capacity = -1,
                };

                occGroup.SemesterGroups.Add(semGroup);

                course.Occurrence.Groups.Add(occGroup);
            }
            else
            {
                group.Capacity = -1;
            }
            Db.SaveChanges();


            var courseService = new EventService(eventId);
            var groups = courseService.GetSubscriptionGroups();

            return PartialView("_GroupList", groups);
        }

        [HttpPost]
        public PartialViewResult RemoveGroup(Guid courseId, Guid groupId)
        {
            var course = Db.Activities.OfType<Event>().SingleOrDefault(c => c.Id == courseId);
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


            return PartialView("_RemoveSubscription");
        }


        public ActionResult MoveDate(Guid id)
        {
            var semester = GetSemester();
            var summary = ActivityService.GetSummary(id) as ActivityDateSummary;

            var memberService = new MemberService(Db, UserManager);
            var roomService = new MyStik.TimeTable.Web.Services.RoomService();

            var orgName = User.IsInRole("SysAdmin") ?
                "FK 09" :
                memberService.GetOrganisationName(semester, User.Identity.Name);

            var userRight = GetUserRight(User.Identity.Name, "FK 09");

            // Alle Räume, auf die der Veranstalter Zugriff hat
            var rooms = roomService.GetRooms(orgName, userRight.IsOrgAdmin);

            var model = new EventMoveDateModel
            {
                Event = summary.Activity as Event,
                ActivityDateId = summary.Date.Id,
                Date = summary.Date,
                NewDate = summary.Date.Begin.ToShortDateString(),
                NewBegin = summary.Date.Begin.TimeOfDay.ToString(),
                NewEnd = summary.Date.End.TimeOfDay.ToString(),
                //LecturerList = GetLecturerList(summary.Date.Id, summary.Date.Begin, summary.Date.End),
                Rooms = rooms,
                Lecturers = Db.Members.Where(m => m.Organiser.ShortName.Equals("FK 09")).ToList(),
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult MoveDate(EventMoveDateModel model)
        {
            var logger = LogManager.GetLogger("Event");

            var activityDate = Db.ActivityDates.SingleOrDefault(d => d.Id == model.ActivityDateId);

            if (activityDate != null)
            {
                var day = DateTime.Parse(model.NewDate);
                var from = DateTime.Parse(model.NewBegin);
                var to = DateTime.Parse(model.NewEnd);


                var start = day.Add(from.TimeOfDay);
                var end = day.Add(to.TimeOfDay);

                logger.InfoFormat("Course date for {0} moved by {1} from [{2} - {3}] to [{4} - {5}]",
                    activityDate.Activity.Name, User.Identity.Name, activityDate.Begin, activityDate.End, start, end);

                activityDate.Begin = start;
                activityDate.End = end;


                // es kommen die neuen Räume und Dozenten
                // => zuerst alle löschen!

                activityDate.Hosts.Clear();
                activityDate.Rooms.Clear();

                if (model.RoomIds != null)
                {
                    foreach (var roomId in model.RoomIds)
                    {
                        activityDate.Rooms.Add(Db.Rooms.SingleOrDefault(r => r.Id == roomId));
                    }
                }

                if (model.LecturerIds != null)
                {
                    foreach (var dozId in model.LecturerIds)
                    {
                        activityDate.Hosts.Add(Db.Members.SingleOrDefault(m => m.Id == dozId));
                    }
                }

                Db.SaveChanges();
            }

            return PartialView("_SaveSuccess");
        }

        [HttpPost]
        public PartialViewResult AddRoom(Guid id, DateTime date, TimeSpan from, TimeSpan to, string room)
        {
            var activityDate = Db.ActivityDates.SingleOrDefault(d => d.Id == id);
            var model = Db.Rooms.SingleOrDefault(r => r.Number.Equals(room));

            activityDate.Rooms.Add(model);
            Db.SaveChanges();

            return CheckRoomList(id, date, from, to);
        }

        [HttpPost]
        public PartialViewResult RemoveRoom(Guid id, Guid roomId)
        {
            var activityDate = Db.ActivityDates.SingleOrDefault(d => d.Id == id);
            var room = Db.Rooms.SingleOrDefault(r => r.Id.Equals(roomId));

            activityDate.Rooms.Remove(room);
            Db.SaveChanges();

            return PartialView("_EmptyRow");
        }

        [HttpPost]
        public PartialViewResult CheckRoomList(Guid id, DateTime date, TimeSpan from, TimeSpan to)
        {
            DateTime start = date.AddHours(from.Hours).AddMinutes(from.Minutes);
            DateTime end = date.AddHours(to.Hours).AddMinutes(to.Minutes);
            var model = GetRoomList(id, start, end);

            return PartialView("_RoomState", model);
        }


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


        [HttpPost]
        public PartialViewResult CheckLecturerList(Guid id, DateTime date, TimeSpan from, TimeSpan to)
        {
            DateTime start = date.AddHours(from.Hours).AddMinutes(from.Minutes);
            DateTime end = date.AddHours(to.Hours).AddMinutes(to.Minutes);
            var model = GetLecturerList(id, start, end);

            return PartialView("_LecturerState", model);
        }

        [HttpPost]
        public PartialViewResult AddLecturer(Guid id, DateTime date, TimeSpan from, TimeSpan to, string lecturer)
        {
            var activityDate = Db.ActivityDates.SingleOrDefault(d => d.Id == id);
            var model = Db.Members.SingleOrDefault(r => r.ShortName.Equals(lecturer));

            activityDate.Hosts.Add(model);
            Db.SaveChanges();

            return CheckLecturerList(id, date, from, to);
        }

        [HttpPost]
        public PartialViewResult RemoveLecturer(Guid id, Guid lecturerId)
        {
            var activityDate = Db.ActivityDates.SingleOrDefault(d => d.Id == id);
            var lecturer = Db.Members.SingleOrDefault(r => r.Id.Equals(lecturerId));

            activityDate.Hosts.Remove(lecturer);
            Db.SaveChanges();

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

        [HttpPost]
        public ActionResult CancelDate(OccurrenceMailingModel model)
        {
            var logger = LogManager.GetLogger("Event");
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
                }

                Db.SaveChanges();

                // Die Subscription stecken in der Activity
                var summary = ActivityService.GetSummary(model.OccurrenceId) as ActivityDateSummary;

                logger.InfoFormat("Event date {0} on {1} canceled by {2}", summary.Activity.Name, summary.Date.Begin, User.Identity.Name);


                // Wenn es keine Eintragungen gibt, dann muss auch keine mail versendet werden
                if (!summary.Subscriptions.Any())
                {
                    return RedirectToAction(summary.Action, summary.Controller, new { id = summary.Id });
                }
            }


            var m = new MailingController();
            m.ControllerContext = ControllerContext;
            return m.CustomOccurrenceMail(model);
        }

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

        [HttpPost]
        public ActionResult ReactivateDate(OccurrenceMailingModel model)
        {
            var logger = LogManager.GetLogger("Event");

            var occ = Db.Occurrences.SingleOrDefault(o => o.Id == model.OccurrenceId);

            if (occ != null)
            {
                occ.IsCanceled = false;
                Db.SaveChanges();

                // Die Subscription stecken in der Activity
                var summary = ActivityService.GetSummary(model.OccurrenceId) as ActivityDateSummary;

                logger.InfoFormat("Event date {0} on {1} reactivated by {2}", summary.Activity.Name, summary.Date.Begin, User.Identity.Name);


                if (!summary.Subscriptions.Any())
                {
                    return RedirectToAction(summary.Action, summary.Controller, new { id = summary.Id });
                }
            }

            var m = new MailingController();
            m.ControllerContext = ControllerContext;
            return m.CustomOccurrenceMail(model);
        }

        public ActionResult DeleteEvent(Guid id)
        {
            var course = Db.Activities.OfType<Event>().SingleOrDefault(c => c.Id == id);

            var model = new EventDeleteModel { Course = course };

            return View(model);
        }

        [HttpPost]
        public ActionResult DeleteEvent(EventDeleteModel model)
        {

            var timeTableService = new TimeTableInfoService();

            timeTableService.DeleteEvent(model.Course.Id);


            // Kehre zurück zur Seite der Aktivität
            return RedirectToAction("Admin");
        }

        public ActionResult CreateDate(Guid id)
        {

            var activity = Db.Activities.SingleOrDefault(a => a.Id == id);
            if (activity != null)
            {
                var today = GlobalSettings.Today;
                var from = GlobalSettings.Now;
                var to = from.AddMinutes(90);

                var date = new ActivityDate
                {
                    Begin = today.AddHours(from.Hour).AddMinutes(from.Minute),
                    End = today.AddHours(to.Hour).AddMinutes(to.Minute),
                    Occurrence = new Occurrence { Capacity = -1, IsAvailable = true, FromIsRestricted = false, UntilIsRestricted = false },
                };

                activity.Dates.Add(date);
                Db.SaveChanges();
            }

            return RedirectToAction("Admin", "Event", new { id = id });
        }

        public ActionResult DeleteDate(Guid id)
        {
            var occ = Db.Occurrences.SingleOrDefault(o => o.Id == id);

            var dateSummary = ActivityService.GetSummary(id) as ActivityDateSummary;

            var date = Db.ActivityDates.SingleOrDefault(d => d.Id == dateSummary.Date.Id);

            var actSummary = new ActivitySummary { Activity = date.Activity };

            var subList = occ.Subscriptions.ToList();
            foreach (var subscription in subList)
            {
                occ.Subscriptions.Remove(subscription);
                Db.Subscriptions.Remove(subscription);

            }
            Db.Occurrences.Remove(occ);
            date.Occurrence = null;


            date.Activity.Dates.Remove(date);
            date.Hosts.Clear();
            date.Rooms.Clear();
            Db.ActivityDates.Remove(date);

            Db.SaveChanges();


            // Kehre zurück zur Seite der Aktivität
            return RedirectToAction(actSummary.Action, actSummary.Controller, new { id = actSummary.Id });
        }



        public ActionResult Subscribers(Guid id)
        {
            var summary = ActivityService.GetSummary(id) as ActivityDateSummary;

            var model = new EventDateInfoModel();

            model.Course = summary.Activity as Event;
            model.Date = summary.Date;

            int iNumber = 0;
            foreach (var subscription in summary.Date.Occurrence.Subscriptions.OrderBy(s => s.TimeStamp))
            {
                var subscriber = UserManager.FindById(subscription.UserId);
                if (subscriber != null)
                {
                    iNumber++;
                    model.Member.Add(
                        new CourseMemberModel()
                        {
                            Number = iNumber,
                            Subscription = subscription,
                            User = subscriber,
                        }
                        );
                }
            }

            ViewBag.UserRight = GetUserRight(User.Identity.Name, model.Course);


            return View(model);
        }

        [HttpPost]
        public PartialViewResult RemoveSubscription(Guid id, string userId)
        {
            var logger = LogManager.GetLogger("Course");

            var occurrence = Db.Occurrences.SingleOrDefault(oc => oc.Id == id);

            if (occurrence != null && !string.IsNullOrEmpty(userId))
            {
                var subscription =
                    occurrence.Subscriptions.SingleOrDefault(s => s.UserId.Equals(userId));

                if (subscription != null)
                {
                    occurrence.Subscriptions.Remove(subscription);
                    Db.Subscriptions.Remove(subscription);
                    Db.SaveChanges();

                    var summary = ActivityService.GetSummary(id);
                    var user = UserManager.FindById(subscription.UserId);

                    logger.InfoFormat("Subscription removed: {0}, {1} by {2}", summary.Name, user.UserName, User.Identity.Name);

                    var mailModel = new SubscriptionMailModel
                    {
                        Summary = summary,
                        Subscription = subscription,
                        User = user,
                        SenderUser = UserManager.FindByName(User.Identity.Name),
                    };

                    var mail = new MailController();
                    mail.RemoveSubscription(mailModel).Deliver();

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


            return PartialView("_RemoveSubscription");
        }

        [HttpPost]
        public PartialViewResult ClearSubscriptions(Guid occurrenceId)
        {
            var logger = LogManager.GetLogger("Course");

            var occurrence = Db.Occurrences.SingleOrDefault(oc => oc.Id == occurrenceId);

            if (occurrence != null)
            {
                var summary = ActivityService.GetSummary(occurrenceId);

                foreach (var subscription in occurrence.Subscriptions.ToList())
                {
                    occurrence.Subscriptions.Remove(subscription);
                    Db.SaveChanges();

                    var mailModel = new SubscriptionMailModel
                    {
                        Summary = summary,
                        Subscription = subscription,
                        User = UserManager.FindById(subscription.UserId),
                        SenderUser = UserManager.FindByName(User.Identity.Name),
                    };

                    var mail = new MailController();
                    mail.RemoveSubscription(mailModel).Deliver();
                }

                logger.InfoFormat("Subscription cleared {0} by {1}", summary.Name, User.Identity.Name);
            }

            return PartialView("_RemoveSubscription");
        }

        [HttpPost]
        public PartialViewResult ChangeGeneralSettings(EventDetailViewModel model)
        {
            var course = Db.Activities.OfType<Event>().SingleOrDefault(c => c.Id == model.Event.Id);

            if (course != null)
            {
                if (!string.IsNullOrEmpty(model.Event.Name) &&
                    !model.Event.Name.Equals(course.Name))
                {
                    course.Name = model.Event.Name;
                }

                if (!string.IsNullOrEmpty(model.Event.ShortName) &&
                    !model.Event.ShortName.Equals(course.ShortName))
                {
                    course.ShortName = model.Event.ShortName;
                }

                course.Description = model.Description2;

                Db.SaveChanges();
            }

            return PartialView("_SaveSuccess");
        }

    }
}