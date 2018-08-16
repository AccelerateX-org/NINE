using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
    [Authorize]
    public class ActivityController : BaseController
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return RedirectToAction("PersonalPlan");
        }

        public ActionResult State(Guid id)
        {
            var semester = SemesterService.GetSemester(id);
            var user = AppUser;

            // Anzahl der LVs

            var activities = Db.Activities.OfType<Course>().Where(a =>
                a.SemesterGroups.Any(g => g.Semester.Id == semester.Id) &&
                a.Occurrence.Subscriptions.Any(u => u.UserId.Equals(user.Id))).ToList();

            var model = new ActivitySemesterViewModel
            {
                Semester = semester,
                Courses = activities
            };

            return View(model);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult AdminPlan()
        {
            ViewBag.Organiser = GetMyOrganisation();
            return View();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult AdminDashboard()
        {
            ViewBag.Organiser = "FK 09";
            return View();
        }

        /// <summary>
        /// Nur Anzeige des Kalenders
        /// </summary>
        /// <returns></returns>
        public ActionResult PersonalPlan()
        {
            var model = new ActivityPlanModel();
            ViewBag.UserRight = GetUserRight();
            return View(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult PersonalPlanDaily()
        {
            var model = new AgendaViewModel();

            var user = UserManager.FindByName(User.Identity.Name);


            var begin = DateTime.Today;
            var end = DateTime.Today.AddDays(7);

            // Alle Dates bei denen der Benutzer als Dozent eingetragen ist
            var lectureDates =
                Db.ActivityDates.Where(d =>
                    d.Hosts.Any(l => !string.IsNullOrEmpty(l.UserId) && l.UserId.Equals(user.Id)) &&
                    d.End >= begin && d.End <= end).OrderBy(d => d.Begin).ToList();

            foreach (var date in lectureDates)
            {
                var agendaDay = model.Days.SingleOrDefault(d => d.Day.Date == date.Begin.Date);
                if (agendaDay == null)
                {
                    agendaDay = new AgendaDayViewModel
                    {
                        Day = date.Begin.Date
                    };
                    model.Days.Add(agendaDay);
                }

                var agendaActivity = new AgendaActivityViewModel
                {
                    Date = date
                };

                agendaDay.Activities.Add(agendaActivity);
            }

            // Alle Eintragungen
            var subscriptions =
            Db.ActivityDates.Where(d =>
                (d.Activity.Occurrence.Subscriptions.Any(s => s.UserId.Equals(user.Id)) ||
                 d.Occurrence.Subscriptions.Any(s => s.UserId.Equals(user.Id)) ||
                 d.Slots.Any(slot => slot.Occurrence.Subscriptions.Any(s => s.UserId.Equals(user.Id)))) &&
                d.End >= begin && d.End <= end).OrderBy(d => d.Begin).ToList();

            foreach (var date in subscriptions)
            {
                var agendaDay = model.Days.SingleOrDefault(d => d.Day.Date == date.Begin.Date);
                if (agendaDay == null)
                {
                    agendaDay = new AgendaDayViewModel
                    {
                        Day = date.Begin.Date
                    };
                    model.Days.Add(agendaDay);
                }

                var agendaActivity = new AgendaActivityViewModel
                {
                    Date = date
                };

                // den slot prüfen
                agendaActivity.Slot =
                    date.Slots.FirstOrDefault(x => x.Occurrence.Subscriptions.Any(s => s.UserId.Equals(user.Id)));

                agendaDay.Activities.Add(agendaActivity);
            }


            return View(model);
        }


        public ActionResult PersonalPlanWeekly(Guid id)
        {
            var model = new ActivityPlanModel();

            model.Semester = SemesterService.GetSemester(id);

            return View(model);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpPost]
        public PartialViewResult SubscribeActivity(Guid Id)
        {
            //Db.Database.BeginTransaction()



            var activity = Db.Occurrences.SingleOrDefault(ac => ac.Id == Id);

            var model = new OccurrenceStateModel();
            
            if (activity != null)
            {
                model.Occurrence = activity;

                var userProfile = UserManager.FindByName(User.Identity.Name);


                var occService = new OccurrenceService(UserManager);
                var msg = occService.SubscribeOccurrence(activity.Id, userProfile);

                model.HasError = msg.HasError;
                model.ErrorMessage = msg.Message;
                model.Subscription = msg.Subscription;


                var logger = LogManager.GetLogger("SubscribeActivity");
                var ac = new ActivityService();
                var summary = ac.GetSummary(Id);
                if (msg.HasError)
                {
                    logger.InfoFormat("{0} ({1}) by [{2}]: {3}",
                        summary.Activity.Name, summary.Activity.ShortName, User.Identity.Name, msg.Message);
                }
                else
                {
                    logger.InfoFormat("{0} ({1}) by [{2}]",
                        summary.Activity.Name, summary.Activity.ShortName, User.Identity.Name);
                }


                if (new SystemConfig().MailSubscriptionEnabled)
                {
                    var mailModel = new SubscriptionMailModel
                    {
                        Summary = summary,
                        Subscription = model.Subscription,
                        User = userProfile,
                    };

                    var mail = new MailController();
                    mail.Subscription(mailModel).Deliver();
                }
            }

            return PartialView("_SubscriptionState", model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpPost]
        public PartialViewResult DischargeActivity(Guid Id)
        {
            DeleteSubscription(Id);

            // Status neu ermittelt
            var activity = Db.Occurrences.SingleOrDefault(ac => ac.Id == Id);
            var model = ActivityService.GetActivityState(activity, AppUser);

            return PartialView("_SubscriptionState", model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public PartialViewResult LockOccurrence(Guid id)
        {
            var occurrence = Db.Occurrences.SingleOrDefault(occ => occ.Id == id);

            if (occurrence != null)
            {
                occurrence.IsAvailable = false;
                Db.SaveChanges();
            }

            return PartialView("_LockState", occurrence);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public PartialViewResult UnLockOccurrence(Guid id)
        {
            var occurrence = Db.Occurrences.SingleOrDefault(occ => occ.Id == id);

            if (occurrence != null)
            {
                occurrence.IsAvailable = true;
                Db.SaveChanges();
            }

            return PartialView("_LockState", occurrence);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public ActionResult RemoveActivity(Guid Id)
        {
            DeleteSubscription(Id);
            return RedirectToAction("Index");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Id"></param>
        public void DeleteSubscription(Guid Id)
        {
            var activity = Db.Occurrences.SingleOrDefault(ac => ac.Id == Id);

            if (activity != null)
            {
                var logger = LogManager.GetLogger("DischargeActivity");

                var userProfile = UserManager.FindByName(User.Identity.Name);

                // das können auch mehrere sein!
                // es ist nicht klar, welche Subscription gemeint ist
                // die erste ist dann die beste
                var subscription = activity.Subscriptions.FirstOrDefault(s => s.UserId.Equals(userProfile.Id));

                var ac = new ActivityService();
                var summary = ac.GetSummary(Id);

                if (subscription != null)
                {
                    var allDrawings = Db.SubscriptionDrawings.Where(x => x.Subscription.Id == subscription.Id).ToList();
                    foreach (var drawing in allDrawings)
                    {
                        Db.SubscriptionDrawings.Remove(drawing);
                    }


                    var bets = subscription.Bets.ToList();
                    foreach (var bet in bets)
                    {
                        Db.LotteriyBets.Remove(bet);
                    }

                    activity.Subscriptions.Remove(subscription);
                    Db.Subscriptions.Remove(subscription);

                    Db.SaveChanges();

                    logger.InfoFormat("{0} ({1}) by [{2}]",
                        summary.Activity.Name, summary.Activity.ShortName, User.Identity.Name);


                    if (new SystemConfig().MailSubscriptionEnabled)
                    {
                        var mailModel = new SubscriptionMailModel
                        {
                            Summary = summary,
                            Subscription = subscription,
                            User = userProfile,
                        };

                        var mail = new MailController();
                        mail.Discharge(mailModel).Deliver();
                    }
                }
                else
                {
                    logger.ErrorFormat("Missing subscription for activity {0} and user {1}", summary.Activity.Name, User.Identity.Name);
                }

            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Id"></param>
        public OccurrenceSubscription Subscribe(Guid Id)
        {
            var activity = Db.Occurrences.SingleOrDefault(ac => ac.Id == Id);

            var model = new OccurrenceStateModel();

            if (activity != null)
            {
                model.Occurrence = activity;

                var userProfile = UserManager.FindByName(User.Identity.Name);


                var occService = new OccurrenceService(UserManager);
                var msg = occService.SubscribeOccurrence(activity.Id, userProfile);

                model.HasError = msg.HasError;
                model.ErrorMessage = msg.Message;
                model.Subscription = msg.Subscription;


                var logger = LogManager.GetLogger("SubscribeActivity");
                var ac = new ActivityService();
                var summary = ac.GetSummary(Id);
                if (msg.HasError)
                {
                    logger.InfoFormat("{0} ({1}) by [{2}]: {3}",
                        summary.Activity.Name, summary.Activity.ShortName, User.Identity.Name, msg.Message);
                }
                else
                {
                    logger.InfoFormat("{0} ({1}) by [{2}]",
                        summary.Activity.Name, summary.Activity.ShortName, User.Identity.Name);
                }


                if (new SystemConfig().MailSubscriptionEnabled)
                {
                    var mailModel = new SubscriptionMailModel
                    {
                        Summary = summary,
                        Subscription = model.Subscription,
                        User = userProfile,
                    };

                    var mail = new MailController();
                    mail.Subscription(mailModel).Deliver();
                }

                return msg.Subscription;
            }

            return null;
        }



        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public PartialViewResult Agenda()
        {
            var model = new AgendaViewModel();

            var user = UserManager.FindByName(User.Identity.Name);


            var begin = DateTime.Today;
            var end = DateTime.Today.AddDays(7);

            // Alle Dates bei denen der Benutzer als Dozent eingetragen ist
            var lectureDates =
                Db.ActivityDates.Where(d => 
                    d.Hosts.Any(l => !string.IsNullOrEmpty(l.UserId) && l.UserId.Equals(user.Id)) &&
                    d.End >= begin && d.End <= end).OrderBy(d => d.Begin).ToList();

            foreach (var date in lectureDates)
            {
                var agendaDay = model.Days.SingleOrDefault(d => d.Day.Date == date.Begin.Date);
                if (agendaDay == null)
                {
                    agendaDay = new AgendaDayViewModel
                    {
                        Day = date.Begin.Date
                    };
                    model.Days.Add(agendaDay);
                }

                var agendaActivity = new AgendaActivityViewModel
                {
                    Date = date
                };

                agendaDay.Activities.Add(agendaActivity);
            }

            // Alle Eintragungen
            var subscriptions = 
            Db.ActivityDates.Where(d =>
                (d.Activity.Occurrence.Subscriptions.Any(s => s.UserId.Equals(user.Id)) ||
                 d.Occurrence.Subscriptions.Any(s => s.UserId.Equals(user.Id)) ||
                 d.Slots.Any(slot => slot.Occurrence.Subscriptions.Any(s => s.UserId.Equals(user.Id)))) &&
                d.End >= begin && d.End <= end).OrderBy(d => d.Begin).ToList();

            foreach (var date in subscriptions)
            {
                var agendaDay = model.Days.SingleOrDefault(d => d.Day.Date == date.Begin.Date);
                if (agendaDay == null)
                {
                    agendaDay = new AgendaDayViewModel
                    {
                        Day = date.Begin.Date
                    };
                    model.Days.Add(agendaDay);
                }

                var agendaActivity = new AgendaActivityViewModel
                {
                    Date = date
                };

                // den slot prüfen
                agendaActivity.Slot =
                    date.Slots.FirstOrDefault(x => x.Occurrence.Subscriptions.Any(s => s.UserId.Equals(user.Id)));

                agendaDay.Activities.Add(agendaActivity);
            }




            return PartialView(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Current()
        {
            var model = new ActivityCurrentModel();

            model.CurrentDates =
                Db.ActivityDates.Where(d => d.Begin <= DateTime.Now && DateTime.Now <= d.End && d.Occurrence != null && d.Occurrence.IsCanceled == false).OrderBy(d => d.Begin).ToList();

            model.CanceledDates =
                Db.ActivityDates.Where(d =>
                    (d.Begin <= DateTime.Now && DateTime.Now <= d.End) && d.Occurrence != null && d.Occurrence.IsCanceled
                    ).ToList();


            return View(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult DailyRota()
        {
            var org = GetMyOrganisation();

            ViewBag.UserRight = GetUserRight(User.Identity.Name, org.ShortName);

            ViewBag.Organiser = org;

            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        [HttpPost]
        public PartialViewResult RoomPlan(string date)
        {
            var day =  string.IsNullOrEmpty(date) ? DateTime.Today  : DateTime.ParseExact(date, "dd.MM.yyyy", null);
            var nextDay = day.AddDays(1);

            var org = GetMyOrganisation();

            var model = new List<RoomActivityModel>();

            // Alle Aktivitäten, die in Räumen des aktuellen
            // Veranstalters stattfinden
            /*
            var allActivities = Db.ActivityDates.Where(x => 
                x.Begin >= day && x.End < nextDay &&
                x.Rooms.Any(r => r.Assignments.Any(a => a.Organiser.Id == org.Id))
                ).ToList();

            // oder
            // Alle Räume, die durch Veranstaltungen belegt sind
            var allRooms = Db.Rooms.Where(r =>
                r.Assignments.Any(a => a.Organiser.Id == org.Id) &&
                r.Dates.Any(x => x.Begin >= day && x.End < nextDay)
                )
                .OrderBy(r => r.Number)
                .ToList();
            */

            // Alle Räume des Veranstalters
            var allRooms = Db.Rooms.Where(r =>
                r.Assignments.Any(a => a.Organiser.Id == org.Id))
                .OrderBy(r => r.Number)
                .ToList();

            foreach (var room in allRooms)
            {
                // Alle Termine in diesem Raum
                var allActivities = room.Dates.Where(x =>
                    x.Begin >= day && x.End < nextDay)
                    .OrderBy(d => d.Begin)
                    .ToList();

                var roomModel = new RoomActivityModel
                {
                    Room = room,
                    Dates = allActivities
                };

                model.Add(roomModel);

            }




            return PartialView("_RoomPlan", model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        [HttpPost]
        public FileResult RoomPlanData(string date)
        {
            var day = string.IsNullOrEmpty(date) ? DateTime.Today : DateTime.ParseExact(date, "dd.MM.yyyy", null);
            var nextDay = day.AddDays(1);

            var org = GetMyOrganisation();

            var model = new List<RoomActivityModel>();


            // Alle Räume des Veranstalters
            var allRooms = Db.Rooms.Where(r =>
                r.Assignments.Any(a => a.Organiser.Id == org.Id))
                .OrderBy(r => r.Number)
                .ToList();

            foreach (var room in allRooms)
            {
                // Alle Termine in diesem Raum
                var allActivities = room.Dates.Where(x =>
                    x.Begin >= day && x.End < nextDay)
                    .OrderBy(d => d.Begin)
                    .ToList();

                var roomModel = new RoomActivityModel
                {
                    Room = room,
                    Dates = allActivities
                };

                model.Add(roomModel);

            }



            var ms = new MemoryStream();
            var writer = new StreamWriter(ms, Encoding.Default);


            writer.Write(
                "Raum;Von;Bis;Bezeichnung;Dozenten;Status");

            writer.Write(Environment.NewLine);

            foreach (var room in model)
            {
                if (room.Dates.Any())
                {
                    foreach (var activityDate in room.Dates)
                    {
                        writer.Write(room.Room.Number);
                        writer.Write(";");
                        writer.Write(activityDate.Begin.ToShortTimeString());
                        writer.Write(";");
                        writer.Write(activityDate.End.ToShortTimeString());
                        writer.Write(";");
                        writer.Write(activityDate.Activity.Name);
                        writer.Write(";");
                        foreach (var dateHost in activityDate.Hosts)
                        {
                            writer.Write(dateHost.Name);
                            if (dateHost != activityDate.Hosts.Last())
                            {
                                writer.Write(", ");
                            }
                        }
                        writer.Write(";");
                        if (activityDate.Occurrence != null && activityDate.Occurrence.IsCanceled)
                        {
                            writer.Write("abgesagt");
                        }
                        writer.Write(Environment.NewLine);
                    }
                }
                else
                {
                    writer.Write(room.Room.Number);
                    writer.Write(";");
                    writer.Write(";;;;keine Belegung");
                    writer.Write(Environment.NewLine);
                }

            }

            writer.Flush();
            writer.Dispose();

            var sb = new StringBuilder();
            sb.Append("Belegungsplan");
            sb.Append("_");
            sb.Append(day.ToString("yyyyMMdd"));
            sb.Append(".csv");

            return File(ms.GetBuffer(), "text/csv", sb.ToString());
        }

        public ActionResult FacultyPlanDaily()
        {
            var model = new DashboardViewModel();
            var org = GetMyOrganisation();

            // Was läuft gerade
            var now = DateTime.Now;
            var endOfDay = DateTime.Today.AddDays(1);

            var nowPlaying = Db.ActivityDates.Where(d =>
                    d.Activity is Course &&
                    (d.Begin <= now && now < d.End || d.Begin > now && d.Begin < endOfDay) &&
                    d.Activity.SemesterGroups.Any(g =>
                        g.CapacityGroup != null &&
                        g.CapacityGroup.CurriculumGroup.Curriculum.Organiser.Id == org.Id))
                .OrderBy(d => d.Begin).ThenBy(d => d.End).ToList();

            model.Organiser = org;
            model.NowPlayingDates = nowPlaying;


            return View(model);
        }

        public ActionResult PersonalCoursePlan(Guid? id)
        {
            var user = AppUser;
            var semester = SemesterService.GetSemester(id);
            var org = GetMyOrganisation();


            var model = new ActivityPlanModel();
            model.Organiser = org;
            model.Semester = semester;

            // Alle Termine als Teilnehmer
            /*
            var activities = Db.Activities.Where(a => a.Occurrence.Subscriptions.Any(u => u.UserId.Equals(user.Id))).ToList();
            foreach (var activity in activities)
            {
                if ((activity.Dates.Any() && activity.Dates.Last().End >= GlobalSettings.Today) ||
                    !activity.Dates.Any())
                {
                    model.MySubscriptions.Add(new ActivitySubscriptionModel
                    {
                        Activity = new ActivitySummary {Activity = activity},
                        State = ActivityService.GetActivityState(activity.Occurrence, user, semester)
                    });

                    if (activity.Occurrence.LotteryEnabled)
                        model.HasLottery = true;
                }
            }

            var dates = Db.ActivityDates.Where(d => d.Occurrence.Subscriptions.Any(u => u.UserId.Equals(user.Id))).ToList();
            foreach (var activityDate in dates)
            {
                if (activityDate.End >= GlobalSettings.Today)
                {
                    model.MySubscriptions.Add(new ActivitySubscriptionModel
                    {
                        Activity = new ActivityDateSummary {Date = activityDate},
                        State = ActivityService.GetActivityState(activityDate.Occurrence, user, semester)
                    });
                }
            }

            var slots = Db.ActivitySlots.Where(s => s.Occurrence.Subscriptions.Any(u => u.UserId.Equals(user.Id))).ToList();
            foreach (var activitySlot in slots)
            {
                if (activitySlot.ActivityDate.End >= GlobalSettings.Today)
                {
                    model.MySubscriptions.Add(new ActivitySubscriptionModel
                    {
                        Activity = new ActivitySlotSummary {Slot = activitySlot},
                        State = ActivityService.GetActivityState(activitySlot.Occurrence, user, semester)
                    });
                }
            }
            */

            // Alle Termine als Veranstalter im Semester
            var lectureActivities =
                Db.Activities.OfType<Course>().Where(a =>
                    a.SemesterGroups.Any(x => x.Semester.Id == semester.Id) &&
                    a.Dates.Any(d => d.Hosts.Any(l => !string.IsNullOrEmpty(l.UserId) && l.UserId.Equals(user.Id)))).ToList();
            foreach (var activity in lectureActivities)
            {
                var summary = new ActivitySummary {Activity = activity};

                var currentDate =
                    activity.Dates.Where(d => d.Begin <= DateTime.Now && DateTime.Now <= d.End)
                        .OrderBy(d => d.Begin)
                        .FirstOrDefault();
                var nextDate =
                    activity.Dates.Where(d => d.Begin >= DateTime.Now)
                        .OrderBy(d => d.Begin)
                        .FirstOrDefault();

                if (currentDate != null)
                {
                    summary.CurrentDate = new CourseDateStateModel
                    {
                        Summary = new ActivityDateSummary {Date = currentDate},
                        State = ActivityService.GetSubscriptionState(currentDate.Occurrence, currentDate.Begin, currentDate.End),
                    };
                }

                if (nextDate != null)
                {
                    summary.NextDate = new CourseDateStateModel
                    {
                        Summary = new ActivityDateSummary {Date = nextDate},
                        State = ActivityService.GetSubscriptionState(nextDate.Occurrence, nextDate.Begin, nextDate.End ),
                    };
                }

                model.MyActivities.Add(summary);
            }

            return View(model);
        }

        public ActionResult Today()
        {
            var org = GetMyOrganisation();

            var beginOfDay = DateTime.Today;
            var endOfDay = beginOfDay.AddDays(1);

            var nowPlaying = Db.ActivityDates.Where(d =>
                    d.Activity is Course &&
                    (d.Begin > beginOfDay && d.Begin < endOfDay) &&
                    d.Activity.SemesterGroups.Any(g =>
                        g.CapacityGroup != null &&
                        g.CapacityGroup.CurriculumGroup.Curriculum.Organiser.Id == org.Id))
                .OrderBy(d => d.Begin).ThenBy(d => d.End).ToList();


            ViewBag.Organiser = org;

            return View(nowPlaying);
        }
    }
}