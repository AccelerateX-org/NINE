using System;
using System.Collections.Generic;
using System.Dynamic;
using System.EnterpriseServices;
using System.Linq;
using System.Net.Configuration;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using log4net;
using log4net.Repository.Hierarchy;
using Microsoft.AspNet.Identity;
using MyStik.TimeTable.Data;
using MyStik.TimeTable.DataServices;
using MyStik.TimeTable.Web.Models;
using MyStik.TimeTable.Web.Services;

namespace MyStik.TimeTable.Web.Controllers
{
    [Authorize]
    public class ActivityController : BaseController
    {
        public ActionResult Index()
        {
            return RedirectToAction("PersonalPlan");
        }

        public ActionResult AdminPlan()
        {
            ViewBag.Semester = GetSemester();
            return View();
        }

        public ActionResult AdminDashboard()
        {
            ViewBag.Organiser = "FK 09";
            return View();
        }


        public ActionResult PersonalPlan()
        {
            var user = AppUser;
            var semester = GetSemester();

            var model = new ActivityPlanModel();
            model.IsDuringLottery = new SystemConfig().IsLotteryEnabled;
            model.HasLottery = false;

            // Alle Termine als Teilnehmer
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


            // Alle Termine als Veranstalter
            // Alle egal, ob das Semester nun aktiv ist oder nicht
            /*
            var doz = Db.Members.SingleOrDefault(m => m.UserId == user.Id);
            if (doz != null)
            {
                var lectureActivities =
                    Db.Activities.Where(a => 
                        a.Dates.Any(d => d.Hosts.Any(l => l.ShortName.Equals(doz.ShortName)))).ToList();
             */
            var lectureActivities =
                Db.Activities.Where(a =>
                    a.Dates.Any(d => d.Hosts.Any(l => !string.IsNullOrEmpty(l.UserId) && l.UserId.Equals(user.Id)))).ToList();
            foreach (var activity in lectureActivities)
                {
                    // nur die, bei denen es noch Termine in der Zukunft gibt
                    if ((activity.Dates.Any() && activity.Dates.OrderBy(d => d.End).Last().End >= GlobalSettings.Today))
                    {
                        var summary = new ActivitySummary {Activity = activity};

                            var currentDate =
                                activity.Dates.Where(d => d.Begin <= GlobalSettings.Now && GlobalSettings.Now <= d.End)
                                    .OrderBy(d => d.Begin)
                                    .FirstOrDefault();
                            var nextDate =
                                activity.Dates.Where(d => d.Begin >= GlobalSettings.Now)
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
                }
            //}

            var userRight = GetUserRight(User.Identity.Name, "FK 09", true);

            ViewBag.CalendarToken = user.Id;
            ViewBag.CalendarPeriod = GetSemester().Name;
            ViewBag.UserRight = userRight;
            ViewBag.IsProf = HasUserRole(User.Identity.Name, "FK 09", "Prof");

            if (userRight.IsOrgMember)
            {
                var member = GetMember(User.Identity.Name, "FK 09");
                if (member != null)
                {
                    ViewBag.ShortName = member.ShortName;

                    if (ViewBag.IsProf)
                    {
                        var officeHour = new OfficeHourService().GetOfficeHour(member.ShortName, semester.Name);
                        ViewBag.HasOfficeHour = (officeHour != null);
                        ViewBag.HostId = member.Id;
                    }
                }
            }

            // test => funktioniert so
            // ViewBag.MenuId = "menu-dashboard";

            return View(model);
        }

        public ActionResult PersonalPlanMobile()
        {
            return View();
        }



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

        [HttpPost]
        public PartialViewResult DischargeActivity(Guid Id)
        {
            DeleteSubscription(Id);

            // Status neu ermittelt
            var activity = Db.Occurrences.SingleOrDefault(ac => ac.Id == Id);
            var model = ActivityService.GetActivityState(activity, AppUser, GetSemester());

            return PartialView("_SubscriptionState", model);
        }

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


        public ActionResult RemoveActivity(Guid Id)
        {
            DeleteSubscription(Id);
            return RedirectToAction("Index");
        }

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

        [HttpPost]
        public PartialViewResult Agenda()
        {
            var model = new AgendaViewModel();

            var user = UserManager.FindByName(User.Identity.Name);


            var begin = GlobalSettings.Today;
            var end = GlobalSettings.Today.AddDays(7);

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

                agendaDay.Activities.Add(agendaActivity);
            }




            return PartialView(model);
        }

        public ActionResult Current()
        {
            var model = new ActivityCurrentModel();

            model.CurrentDates =
                Db.ActivityDates.Where(d => d.Begin <= GlobalSettings.Now && GlobalSettings.Now <= d.End && d.Occurrence.IsCanceled == false).OrderBy(d => d.Begin).ToList();

            model.CanceledDates =
                Db.ActivityDates.Where(d =>
                    (d.Begin <= GlobalSettings.Now && GlobalSettings.Now <= d.End) && d.Occurrence.IsCanceled
                    ).ToList();


            return View(model);
        }

    
    }
}