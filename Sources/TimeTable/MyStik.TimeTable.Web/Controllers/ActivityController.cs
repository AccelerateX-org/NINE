using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using log4net;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.SignalR.Messaging;
using MyStik.TimeTable.Data;
using MyStik.TimeTable.Web.Models;
using MyStik.TimeTable.Web.Services;
using static System.Data.Entity.Infrastructure.Design.Executor;

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



        public ActionResult Details(Guid id)
        {
            var activity = Db.Activities.SingleOrDefault(x => x.Id == id);

            if (activity == null)
                return View("Error");

            if (activity is Course)
                return RedirectToAction("Details", "Course", new {id = id});

            if (activity is Event)
                return RedirectToAction("Details", "Event", new { id = id });


            if (activity is Reservation)
                return RedirectToAction("Details", "Reservation", new {id = id});

            return View("Error");
        }



        /// <summary>
        /// 
        /// </summary>
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
            var begin = DateTime.Today;
            var end = DateTime.Today.AddDays(7);

            var model = GetAgenda(begin, end);

            return View(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult PersonalPlanToday()
        {
            var begin = DateTime.Today;
            var end = DateTime.Today.AddDays(1);

            var model = GetAgenda(begin, end);

            return View("DayCalendar", model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult PersonalPlanTomorrow()
        {
            var begin = DateTime.Today.AddDays(1);
            var end = DateTime.Today.AddDays(2);

            var model = GetAgenda(begin, end);

            return View("DayCalendar", model);
        }




        private AgendaViewModel GetAgenda(DateTime begin, DateTime end)
        { 
            var model = new AgendaViewModel();

            var user = UserManager.FindByName(User.Identity.Name);


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

            return model;
        }


        /// <summary>
        /// 
        /// </summary>
        public ActionResult PersonalPlanWeekly(Guid id)
        {
            var courseService = new CourseService(Db);

            var model = new ActivityPlanModel();

            var user = GetCurrentUser();
            var semester = SemesterService.GetSemester(id);
            var members = MemberService.GetFacultyMemberships(user.Id);

            ViewBag.IsLecturer = members.Count > 0;

            model.Semester = semester;

            var courses = new List<Course>();

            var mylisten =
                Db.Activities.OfType<Course>()
                    .Where(c =>
                        c.Semester.Id == semester.Id &&
                        c.Occurrence.Subscriptions.Any(x => x.UserId == user.Id))
                    .ToList();

            courses.AddRange(mylisten);

            var member = members.FirstOrDefault();
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
                var summary = courseService.GetCourseSummary(course);
                model.Courses.Add(summary);
            }

            var currentSemester = semester;
            var nextSemester = SemesterService.GetNextSemester(semester);
            var prevSemester = SemesterService.GetPreviousSemester(semester);

            ViewBag.CurrentSemester = currentSemester;
            ViewBag.NextSemester = nextSemester;
            ViewBag.PrevSemester = prevSemester;


            return View(model);
        }

        public ActionResult MemberPlanWeekly(Guid id, Guid memberId)
        {
            var courseService = new CourseService(Db);

            var model = new ActivityPlanModel();

            var user = GetCurrentUser();
            var semester = SemesterService.GetSemester(id);
            var members = MemberService.GetFacultyMemberships(user.Id);

            ViewBag.IsLecturer = members.Count > 0;
            ViewBag.Member = MemberService.GetMember(memberId);

            model.Semester = semester;


            var courses = new List<Course>();

            // Alle Vorlesungen, die ich halte
            var member = MemberService.GetMember(memberId);
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
                var summary = courseService.GetCourseSummary(course);
                model.Courses.Add(summary);
            }


            return View(model);
        }


        /// <summary>
        /// 
        /// </summary>
        public ActionResult PersonalPlanList(Guid id)
        {
            var semester = SemesterService.GetSemester(id);
            var user = AppUser;

            var courseService = new CourseService(Db);

            var model = new DashboardStudentViewModel();

            model.Semester = semester;
            model.User = user;
            model.Student = StudentService.GetCurrentStudent(user).FirstOrDefault();
            model.Courses = new List<CourseSummaryModel>();

            var courses = Db.Activities.OfType<Course>().Where(a =>
                a.SemesterGroups.Any(g => g.Semester.Id == semester.Id) &&
                a.Occurrence.Subscriptions.Any(u => u.UserId.Equals(model.User.Id))).ToList();
            foreach (var course in courses)
            {
                var summary = courseService.GetCourseSummary(course);
                model.Courses.Add(summary);

                var state = ActivityService.GetActivityState(course.Occurrence, user);

                summary.User = user;
                summary.Subscription = state.Subscription;

                summary.Lottery =
                    Db.Lotteries.FirstOrDefault(x => x.Occurrences.Any(y => y.Id == course.Occurrence.Id));

            }

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
        public PartialViewResult Agenda(string day)
        {
            var model = new AgendaViewModel();

            var user = UserManager.FindByName(User.Identity.Name);

            var dateDay = DateTime.Parse(day);

            var begin = dateDay;
            var end = dateDay.AddDays(1);

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
            var myOcs = Db.Occurrences.Where(o => o.Subscriptions.Any(s => s.UserId.Equals(user.Id))).ToList();

            var ac = new ActivityService();

            foreach (var occ in myOcs)
            {
                var summary = ac.GetSummary(occ.Id);

                if (summary != null)
                {
                    var dates = summary.GetDates(begin, end);

                    foreach (var date in dates)
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

                        if (agendaDay.Activities.All(x => x.Date.Id == date.Id))
                        {
                            var agendaActivity = new AgendaActivityViewModel
                            {
                                Date = date
                            };

                            // den slot prüfen
                            agendaActivity.Slot =
                                date.Slots.FirstOrDefault(
                                    x => x.Occurrence.Subscriptions.Any(s => s.UserId.Equals(user.Id)));

                            agendaDay.Activities.Add(agendaActivity);
                        }
                    }
                }
            }
            /*
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
            */

            // alle eigenen Reservierungen
            var reservations =
                Db.Activities.OfType<Reservation>()
                    .Where(r => r.UserId.Equals(user.Id) && r.Dates.Any(d => d.Begin >= begin && d.End <= end ))
                    .ToList();

            foreach (var reservation in reservations)
            {
                var dates = reservation.Dates.Where(d => d.Begin >= begin && d.End <= end).ToList();
                foreach (var date in dates)
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
            }



            return PartialView(model);
        }

        [HttpPost]
        public PartialViewResult PersonalAgenda(string day, int delta, int dayOfWeek, bool nextDate)
        {
            var model = new AgendaViewModel();

            var user = UserManager.FindByName(User.Identity.Name);

            var dateDay = DateTime.Parse(day);

            var sb = new StringBuilder();
            sb.AppendFormat("{0}: ", dateDay.ToShortDateString());
            if (delta == 1)
            {
                sb.Append("Tagesprogram");
            }
            else if (delta == 7)
            {
                sb.Append("7-Tage Vorschau");
            }
            else
            {
                sb.Append("Fehler");
            }

            if (dayOfWeek <= 0)
            {
                sb.AppendFormat(" | ab aktuellem Tag");
            }
            else if (dayOfWeek == 1)
            {
                sb.AppendFormat(" | ab Wochenbeginn");
            }
            else
            {
                sb.Append("Fehler");
            }

            if (nextDate) {
                sb.AppendFormat(" | nächste Termine");
            }


            var begin = dateDay.Date;
            var end = dateDay.AddDays(delta).Date;
            var sem = SemesterService.GetSemester(dateDay);

            sb.AppendFormat(" | {0} - {1} | {2}", begin, end, sem.Name);

            model.Title = sb.ToString();


            // nur der aktuelle Tag => mit und ohne next Dates

            // Wochenvorschau => mit und ohne next Dates

            // Datum | Begin => Sortierung
            // Activity
            // Rooms => aus Date
            // Hosts => aus Date


            // Alle Aktivitäten, in denen ich als Dozent eingetragen bin
            // oder in denen ich eingetragen bin
            // mit Terminen in der Zukunft
            // Aktivitäten, die zum Semester des gewählten Tages gehören
            // bei denen ich als Dozent eingetragen bin und die keine Termine mehr haben

            // Aktivitäten, die zum Semester des gewählten Tages gehören
            // in denen ich eingetragen bin und die keine Termine mehr haben
            /*
             * var coursesWithDates =
                           Db.Activities.Where(d =>
                               (
                                   (d.Occurrence != null && d.Occurrence.Subscriptions.Any(s => s.UserId.Equals(user.Id)) ||
                                   d.Dates.Any(x => x.Hosts.Any(l => !string.IsNullOrEmpty(l.UserId) && l.UserId.Equals(user.Id))))
                                   &&
                                   d.Dates.Any(x => x.End >= begin)
                               )
                               ||
                               (
                                   (d.Occurrence != null && d.Occurrence.Subscriptions.Any(s => s.UserId.Equals(user.Id)) ||
                                   d.Dates.Any(x => x.Hosts.Any(l => !string.IsNullOrEmpty(l.UserId) && l.UserId.Equals(user.Id)))) 
                                   &&
                                   d.Semester != null && d.Semester.Id == sem.Id && !d.Dates.Any(x => x.End >= begin)
                               )
                               ).ToList();
           */
            var activities = new List<AgendaActivityViewModel>();

            var coursesWithDates =
                Db.Activities.Where(d =>
                    (
                        (d.Occurrence != null && d.Occurrence.Subscriptions.Any(s => s.UserId.Equals(user.Id)) ||
                        d.Dates.Any(x => x.Hosts.Any(l => !string.IsNullOrEmpty(l.UserId) && l.UserId.Equals(user.Id))))
                        &&
                        d.Dates.Any(x => x.End >= begin)
                    )
                    ).Distinct().ToList();

            // Termine nehmen
            // von bis
            foreach (var c in coursesWithDates) 
            {
                var subscription = c.Occurrence.Subscriptions.FirstOrDefault(x => x.UserId.Equals(user.Id));
                var dates = c.Dates.Where(d => d.End >= begin && d.End <= end).ToList();

                if (dates.Any())
                {
                    foreach (var d in dates) {
                        var agenda = new AgendaActivityViewModel
                        {
                            Activity = c,
                            Date = d,
                            Subscription = subscription,
                        };
                        activities.Add(agenda);
                    }
                }
                else
                {
                    if (nextDate)
                    {
                        var nextCourseDate = c.Dates.Where(x => x.End > begin).OrderBy(x => x.Begin).FirstOrDefault();
                        var agenda = new AgendaActivityViewModel
                        {
                            Activity = c,
                            Subscription = subscription,
                            Date = nextCourseDate
                        };
                        activities.Add(agenda);
                    }
                    else
                    {
                        // die nächsten Termine werden nicht angezeigt.
                    }
                }

            }

            var allMySemesterWithOfficeHours =
                Db.Activities.OfType<OfficeHour>().Where(x =>
                    x.Semester.Id == sem.Id &&
                    x.Dates.Any(d => d.End >= begin)).ToList();

            foreach (var officeHour in allMySemesterWithOfficeHours)
            {

                var dates = officeHour.Dates.Where(d => d.Occurrence.Subscriptions.Any(s => s.UserId.Equals(user.Id)))
                    .ToList();

                foreach (var d in dates)
                {
                    var subscription = d.Occurrence.Subscriptions.FirstOrDefault(x => x.UserId.Equals(user.Id));
                    var agenda = new AgendaActivityViewModel
                    {
                        Activity = officeHour,
                        Date = d,
                        Subscription = subscription,
                    };
                    activities.Add(agenda);
                }


                // alle slots
                dates = officeHour.Dates.Where(d => d.Slots.Any(s => s.Occurrence.Subscriptions.Any(g => g.UserId.Equals(user.Id))))
                    .ToList();

                foreach (var date in dates)
                {
                    var slots = date.Slots.Where(d => d.Occurrence.Subscriptions.Any(s => s.UserId.Equals(user.Id)))
                        .ToList();

                    foreach(var slot in slots)
                    {
                        var subscription = slot.Occurrence.Subscriptions.FirstOrDefault(x => x.UserId.Equals(user.Id));
                        var agenda = new AgendaActivityViewModel
                        {
                            Activity = officeHour,
                            Date = date,
                            Subscription = subscription,
                        };
                        activities.Add(agenda);
                    }

                }
            }

            model.Activities = activities;
            model.ActivitiesWithDates = activities.Where(x => x.Date != null).OrderBy(x => x.Date.Begin).ToList();
            model.ActivitiesNoDates = activities.Where(x => x.Date == null).ToList();

            // alle eigenen Reservierungen
            var reservations =
                Db.Activities.OfType<Reservation>()
                    .Where(r => r.UserId.Equals(user.Id) && r.Dates.Any(d => d.End >= begin))
                    .ToList();



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
        public ActionResult DailyRota(Guid? id)
        {
            var org = id.HasValue == false ? GetMyOrganisation() : GetOrganiser(id.Value);

            ViewBag.UserRight = GetUserRight(org);

            var culture = Thread.CurrentThread.CurrentUICulture;
            ViewBag.Culture = culture;


            return View(org);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        [HttpPost]
        public PartialViewResult RoomPlan(string date, Guid orgId)
        {
            //var day =  string.IsNullOrEmpty(date) ? DateTime.Today  : DateTime.ParseExact(date, "dd.MM.yyyy", null);
            var day = string.IsNullOrEmpty(date) ? DateTime.Today : DateTime.Parse(date);
            var nextDay = day.AddDays(1);

            var org = GetOrganisation(orgId);

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
            //var day = string.IsNullOrEmpty(date) ? DateTime.Today : DateTime.ParseExact(date, "dd.MM.yyyy", null);
            var day = string.IsNullOrEmpty(date) ? DateTime.Today : DateTime.Parse(date);
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

        /// <summary>
        /// 
        /// </summary>
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

        /// <summary>
        /// 
        /// </summary>
        public ActionResult Today(Guid? id)
        {
            var org = id == null ? GetMyOrganisation() : GetOrganiser(id.Value);

            var beginOfDay = DateTime.Today;

            ViewBag.Organiser = org;
            ViewBag.Date = beginOfDay;
            var culture = Thread.CurrentThread.CurrentUICulture;
            ViewBag.Culture = culture;

            return View();
        }

        public ActionResult Tomorrow()
        {
            var org = GetMyOrganisation();

            var beginOfDay = DateTime.Today.AddDays(1);
            var endOfDay = beginOfDay.AddDays(1);

            var nowPlaying = Db.ActivityDates.Where(d =>
                    d.Activity is Course &&
                    (d.Begin > beginOfDay && d.Begin < endOfDay) &&
                    d.Activity.SemesterGroups.Any(g =>
                        g.CapacityGroup != null &&
                        g.CapacityGroup.CurriculumGroup.Curriculum.Organiser.Id == org.Id))
                .OrderBy(d => d.Begin).ThenBy(d => d.End).ToList();


            ViewBag.Organiser = org;
            ViewBag.Date = beginOfDay;

            return View("Today", nowPlaying);
        }

        public PartialViewResult Programm(string date, Guid orgId)
        {
            var beginOfDay = DateTime.Parse(date);
            var endOfDay = beginOfDay.AddDays(1);

            var org = GetOrganiser(orgId);

            var nowPlaying = Db.ActivityDates.Where(d =>
                        (d.Begin > beginOfDay && d.Begin < endOfDay) &&                         // alles an diesem Tag
                        (d.Activity.Organiser.Id == org.Id)                                     // was zu dem Organiser gehört
                    )
                .OrderBy(d => d.Begin).ThenBy(d => d.End).ToList();


            return PartialView("_Programm", nowPlaying);
        }

    }
}