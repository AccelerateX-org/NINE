using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Hangfire;
using log4net;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity;
using MyStik.TimeTable.Data;
using MyStik.TimeTable.DataServices;
using MyStik.TimeTable.Web.Models;
using MyStik.TimeTable.Web.Services;

namespace MyStik.TimeTable.Web.Controllers
{
    public class WpmController : BaseController
    {
        [Authorize(Roles = "SysAdmin")]
        public ActionResult Index()
        {
            return View();
        }

        /*
        public ActionResult Hangfire()
        {
            RecurringJob.AddOrUpdate<LotteryService>("WPM-Platzverlosung", x => x.RunLottery(), Cron.Daily(1));

            return RedirectToAction("Index");
        }
         */

        public ActionResult List()
        {
            // Datenbank reparieren
            var semester = GetSemester();
            var occService = new OccurrenceService(UserManager);
            var courseService = new CourseService(UserManager);
            var semesterService = new SemesterService();

            var user = AppUser;
            Curriculum userCurr = null;

            if (user != null)
            {
                var sub = semesterService.GetSubscription(semester, user.Id);
                if (sub != null)
                {
                    userCurr = sub.SemesterGroup.CapacityGroup.CurriculumGroup.Curriculum;
                }
            }


            var wpmList =  Db.Activities.OfType<Course>().Where(c => 
                c.Occurrence.LotteryEnabled &&
                c.SemesterGroups.Any(g => g.Semester.Id == semester.Id)).ToList();


            var model = new WPMListModel();
            model.User = user;
            model.Semester = semester;
            model.Curricula.AddRange(Db.Curricula.Where(c => !c.ShortName.Equals("Export")).ToList());

            
            // jedes WPM durchgehen
            foreach (var wpm in wpmList)
            {
                var detail = new WPMDetailModel();

                detail.Course = wpm;
                detail.Summary = courseService.GetCourseSummary(wpm);
                detail.OccurrenceState = ActivityService.GetActivityState(wpm.Occurrence, user, semester);
                detail.Bookable = false;

                if (wpm.Occurrence.UseGroups)
                {
                    // Bei Gruppen wird keine Gesamtkapazität angegeben
                }
                else
                {
                    // ohne Gruppen
                    detail.Capacity = wpm.Occurrence.Capacity;
                    detail.Participients = occService.GetParticipiantList(wpm.Occurrence, string.Empty).Count;
                    detail.Pending = occService.GetPendingList(wpm.Occurrence, string.Empty).Count;
                    detail.Waiting = occService.GetWaitingList(wpm.Occurrence, string.Empty, 0).Count;
                    detail.Free = detail.Capacity - detail.Participients - detail.Pending;

                    detail.CapacityState = detail.Free > 0 ? "success" : "danger";
                    detail.ChancesState = "danger";
                    
                    if (detail.Waiting < detail.Free + detail.Pending)
                    {
                        detail.ChancesState = "warning";
                    }

                    if (detail.Waiting < detail.Free)
                    {
                        detail.ChancesState = "success";
                    }
                    
                }

                // Alle Semestergruppen durchgehen, zu dem das WPM gehört!
                foreach (var group in wpm.SemesterGroups)
                {
                    var curriculum = group.CapacityGroup.CurriculumGroup.Curriculum;
                    
                    // was anderes darf es nicht geben
                    if (curriculum != null)
                    {
                        // nachsehen, ob der Benutzer den Kurs auch buchen dürfte
                        if (userCurr != null && (userCurr.Id == curriculum.Id))
                        {
                            detail.Bookable = true;
                        }


                        // das ist die zugehörige Kapazitätsgruppe
                        var capGroup =
                            wpm.Occurrence.Groups.SingleOrDefault(
                                g => g.SemesterGroups.Any(s => s.Id == group.Id));


                        // sollte vorhanden sein, daher hier ausgleichen
                        // dirty code!
                        if (capGroup == null)
                        {
                            capGroup = new OccurrenceGroup
                            {
                                Capacity = 0,
                                FitToCurriculumOnly = true,
                                Occurrence = wpm.Occurrence
                            };
                            capGroup.SemesterGroups.Add(group);
                            wpm.Occurrence.Groups.Add(capGroup);
                            Db.OccurrenceGroups.Add(capGroup);
                            Db.SaveChanges();
                        }


                        // Zunächst das Modell je nach Studiengang anlegen
                        if (!detail.Capacites.ContainsKey(curriculum))
                        {
                            detail.Capacites[curriculum] = new CapacityModel();
                        }

                        var capModel = detail.Capacites[curriculum];

                        capModel.Capacity = capGroup.Capacity;
                        capModel.Participients = occService.GetParticipiantList(wpm.Occurrence, curriculum.ShortName).Count;
                        capModel.Pending = occService.GetPendingList(wpm.Occurrence, curriculum.ShortName).Count;
                        capModel.Waiting = occService.GetWaitingList(wpm.Occurrence, curriculum.ShortName, 0).Count;
                        capModel.Free = capModel.Capacity - capModel.Participients - capModel.Pending;


                        capModel.CapacityState = capModel.Free > 0 ? "success" : "danger";
                        capModel.ChancesState = "danger";

                        if (capModel.Waiting < capModel.Free + capModel.Pending)
                        {
                            capModel.ChancesState = "warning";
                        }

                        if (capModel.Waiting < capModel.Free)
                        {
                            capModel.ChancesState = "success";
                        }
                    }
                }



                model.WPM.Add(detail);
            }


            return View(model);
        }

        [Authorize(Roles = "SysAdmin")]
        public ActionResult Reset()
        {
            var semester = GetSemester();

                var wpmList = Db.Activities.OfType<Course>().Where(c =>
        c.Occurrence.LotteryEnabled &&
        c.SemesterGroups.Any(g => g.Semester.Id == semester.Id)).ToList();

            foreach (var wpm in wpmList)
            {
                var subList = wpm.Occurrence.Subscriptions.ToList();
                subList.ForEach(s => s.OnWaitingList = true);
                subList.ForEach(s => s.LapCount = 0);
                subList.ForEach(s => s.IsConfirmed = false);
            }

            Db.SaveChanges();

            return RedirectToAction("Gamble");
        }


        [Authorize(Roles = "SysAdmin")]
        public ActionResult Gamble()
        {
            return View();
        }

        public ActionResult Subscribers()
        {
            var semester = GetSemester();



            var wpmList = Db.Activities.OfType<Course>().Where(c =>
                c.Occurrence.LotteryEnabled &&
                c.SemesterGroups.Any(g => g.Semester.Id == semester.Id)).ToList();

            var wpmSubscribedList = wpmList.Where(c => c.Occurrence.Subscriptions.Any()).ToList();

            var allSubscriptionLists = wpmSubscribedList.Select(s => s.Occurrence.Subscriptions).ToList();

            var allSubscriptions = new List<OccurrenceSubscription>();
            foreach (var subs in allSubscriptionLists)
            {
                allSubscriptions.AddRange(subs);
            }

            WpmSubscriptionMasterModel model = new WpmSubscriptionMasterModel();

            foreach (var subscription in allSubscriptions)
            {
                if (!model.Subscriptions.ContainsKey(subscription.UserId))
                {
                    model.Subscriptions[subscription.UserId] = new WpmSubscriptionDetailModel();

                    var user = UserManager.FindById(subscription.UserId);

                    var semSub =
                    Db.Subscriptions.OfType<SemesterSubscription>()
                        .SingleOrDefault(s => s.UserId.Equals(user.Id) && s.SemesterGroup.Semester.Id == semester.Id);


                    model.Subscriptions[subscription.UserId].User = user;
                    if (semSub != null)
                    {
                        model.Subscriptions[subscription.UserId].Group = semSub.SemesterGroup;
                    }

                    model.Subscriptions[subscription.UserId].FirstAction = DateTime.Now;
                    model.Subscriptions[subscription.UserId].LastAction = new DateTime(2015, 1, 1);
                }

                var detailModel = model.Subscriptions[subscription.UserId];

                // Einbauen
                if (subscription.OnWaitingList)
                {
                    detailModel.Waiting++;
                }
                else
                {
                    if (subscription.IsConfirmed)
                    {
                        detailModel.Confirmed++;
                    }
                    else
                    {
                        detailModel.Reservations++;
                    }
                }

                if (subscription.TimeStamp < detailModel.FirstAction)
                {
                    detailModel.FirstAction = subscription.TimeStamp;
                }

                if (subscription.TimeStamp > detailModel.LastAction)
                {
                    detailModel.LastAction = subscription.TimeStamp;
                }

            }


            var list = model.Subscriptions.Values.ToList();

            list =
                list.OrderBy(m => m.Group.FullName)
                    .ThenByDescending(m => m.Confirmed)
                    .ThenByDescending(m => m.Reservations)
                    .ThenByDescending(m => m.Waiting)
                    .ToList();

            return View(list);
        }


        [Authorize(Roles = "SysAdmin")]
        public ActionResult Statistics()
        {
            var model = new WPMSummaryModel();

            var semester = GetSemester();

            var wpmList = Db.Activities.OfType<Course>().Where(c =>
                c.Occurrence.LotteryEnabled &&
                c.SemesterGroups.Any(g => g.Semester.Id == semester.Id)).ToList();

            var wpmSubscribedList = wpmList.Where(c => c.Occurrence.Subscriptions.Any()).ToList();

            var allSubscriptionLists = wpmSubscribedList.Select(s => s.Occurrence.Subscriptions).ToList();

            var allSubscriptions = new List<OccurrenceSubscription>();
            foreach (var subs in allSubscriptionLists)
            {
                allSubscriptions.AddRange(subs);
            }

            var maxPrio = allSubscriptions.Max(s => s.Priority);

            model.WPMTotalCount = wpmList.Count();
            model.WPMSubscribedCount = wpmSubscribedList.Count();
            model.SubscriptionCount = allSubscriptions.Count();

            for (var i = 1; i <= maxPrio; i++)
            {
                model.PriorityList[i] = allSubscriptions.Count(s => s.Priority == i);
            }

            model.SubscriberCount = allSubscriptions.Select(s => s.UserId).Distinct().Count();



            return View(model);
        }

        [Authorize(Roles = "SysAdmin")]
        public ActionResult InitLottery()
        {

            var semester = GetSemester();

            var wpmList = Db.Activities.OfType<Course>()
                .Where(a =>
                    a.SemesterGroups.Any(g => g.Semester.Id == semester.Id) &&
                    a.SemesterGroups.Any(g => g.CapacityGroup.CurriculumGroup.Name.Equals("WPM")))
                .OrderBy(a => a.Name)
                .ToList();

            foreach (var wpm in wpmList)
            {
                wpm.Occurrence.LotteryEnabled = true;
                wpm.Occurrence.IsAvailable = false;
            }

            Db.SaveChanges();

            return RedirectToAction("Index");
        }

        [Authorize(Roles = "SysAdmin")]
        public ActionResult InitSubscriptions()
        {

            var semester = GetSemester();

            var wpmList = Db.Activities.OfType<Course>()
                .Where(a =>
                    a.SemesterGroups.Any(g => g.Semester.Id == semester.Id) &&
                    a.SemesterGroups.Any(g => g.CapacityGroup.CurriculumGroup.Name.Equals("WPM")))
                .OrderBy(a => a.Name)
                .ToList();

            foreach (var wpm in wpmList)
            {
                foreach (var subscription in wpm.Occurrence.Subscriptions)
                {
                    subscription.OnWaitingList = true;
                    subscription.LapCount = 0;
                    subscription.IsConfirmed = false;
                }
            }

            Db.SaveChanges();

            return RedirectToAction("Index");
        }


        public ActionResult MySelection()
        {
            var user = AppUser;
            var semester = GetSemester();

            var occurrenceService = new OccurrenceService(UserManager);
            var courseService = new CourseService(UserManager);
            var semSubService = new SemesterSubscriptionService();

            var model = new WPMAdminModel();
            model.User = user;

            var wpmService = new WpmService();

            var wpmList = wpmService.GetAllLottryCourses(semester);

            var nConfirmed = 0;

            var curriculum = semSubService.GetBestCurriculum(model.User.Id, semester);

            if (curriculum != null)
            {
                model.Curriculum = curriculum;

                foreach (var wpm in wpmList)
                {
                    // es könnten ja auch mehrere Eintragungen vorliegen
                    var wpmsubs = wpm.Occurrence.Subscriptions.Where(s => s.UserId.Equals(model.User.Id));

                    foreach (var subscription in wpmsubs)
                    {
                        var wpmSubscriptionModel = new WPMSubscriptionModel
                        {
                            Course = wpm,
                            Subscription = subscription,
                            IsAvailable = true,
                        };

                        var summary = courseService.GetCourseSummary(wpm);
                        summary.State = ActivityService.GetActivityState(wpm.Occurrence, user, semester);

                        wpmSubscriptionModel.Summary = summary;

                        if (subscription.IsConfirmed)
                        {
                            nConfirmed++;
                        }

                        wpmSubscriptionModel.IsValid = courseService.IsAvailableFor(wpm, curriculum.ShortName);

                        occurrenceService.GetCapacity(wpm.Occurrence, wpmSubscriptionModel, curriculum.ShortName,
                            subscription.LapCount);

                        model.Subscriptions.Add(wpmSubscriptionModel);
                    }

                }
            }

            model.Confirmed = nConfirmed;
            model.MaxConfirmed = wpmService.GetMaxConfiremd(semester);

            ViewBag.UserRight = GetUserRight();
            
            return View(model);
        }

        public ActionResult Confirm(Guid occId)
        {
            var userDb = new ApplicationDbContext();

            var user = userDb.Users.SingleOrDefault(u => u.UserName.ToUpper().Equals(User.Identity.Name.ToUpper()));

            var occurrence = Db.Occurrences.SingleOrDefault(occ => occ.Id == occId);

            if (user == null || occurrence == null)
                return RedirectToAction("Error");

            var subscription = occurrence.Subscriptions.FirstOrDefault(s => s.UserId.Equals(user.Id));
            if (subscription == null)
                return RedirectToAction("Error");

            var logger = LogManager.GetLogger("ConfirmActivity");
            var ac = new ActivityService();
            var summary = ac.GetSummary(occId);
            logger.InfoFormat("{0} ({1}) by [{2}]",
                summary.Activity.Name, summary.Activity.ShortName, User.Identity.Name);


            subscription.IsConfirmed = true;
            Db.SaveChanges();

            return RedirectToAction("MySelection");
        }

        public ActionResult Release(Guid occId)
        {
            var userDb = new ApplicationDbContext();

            var user = userDb.Users.SingleOrDefault(u => u.UserName.ToUpper().Equals(User.Identity.Name.ToUpper()));

            var occurrence = Db.Occurrences.SingleOrDefault(occ => occ.Id == occId);

            if (user == null || occurrence == null)
                return RedirectToAction("Error");

            var subscription = occurrence.Subscriptions.FirstOrDefault(s => s.UserId.Equals(user.Id));
            if (subscription == null)
                return RedirectToAction("Error");



            subscription.IsConfirmed = false;
            subscription.OnWaitingList = true;
            subscription.LapCount = 0;
            Db.SaveChanges();

            var logger = LogManager.GetLogger("ReleaseActivity");
            var ac = new ActivityService();
            var summary = ac.GetSummary(occId);
            logger.InfoFormat("{0} ({1}) by [{2}]",
                summary.Activity.Name, summary.Activity.ShortName, User.Identity.Name);




            return RedirectToAction("MySelection");
        }

        public ActionResult Discharge(Guid occId)
        {
            var acController = new ActivityController();
            acController.ControllerContext = ControllerContext;
            acController.DeleteSubscription(occId);

            return RedirectToAction("MySelection");
        }
    
    }
}