using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Hangfire;
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
    public class LotteryController : BaseController
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Index(Guid? id)
        {
            var org = GetMyOrganisation();

            Semester semester;
            if (id.HasValue)
            {
                semester = GetSemester(id.Value);
            }
            else
            {
                semester = GetSemester();
            }

            // alte reparieren
            foreach (var lottery in Db.Lotteries.ToList())
            {
                if (lottery.Organiser == null)
                {
                    if (lottery.Owner != null)
                    {
                        lottery.Organiser = lottery.Owner.Organiser;
                    }
                }

                if (lottery.Semester == null)
                {
                    lottery.Semester = semester;
                }
            }
            Db.SaveChanges();


            var alLotteries = Db.Lotteries.Where(x => 
                x.Semester!= null && x.Semester.Id == semester.Id &&
                x.Organiser != null && x.Organiser.Id == org.Id).ToList();

            ViewBag.UserRight = GetUserRight(org);
            ViewBag.Semester = semester;
            ViewBag.Organiser = org;

            var nextSemester = new SemesterService().GetNextSemester(semester);
            if (nextSemester != null && nextSemester.Groups.Any())
            {
                ViewBag.NextSemester = nextSemester;
            }


            var model = new List<LotterySummaryModel>();

            foreach (var lottery in alLotteries)
            {
                var totalCapacity = 0;
                var totalSubscriptions = 0;
              
                var userIds = new List<string>();

                foreach (var lotteryOccurrence in lottery.Occurrences)
                {
                    if (lotteryOccurrence.UseGroups)
                    {
                        totalCapacity += lotteryOccurrence.Groups.Sum(x => x.Capacity);
                    }
                    else
                    {
                        totalCapacity += lotteryOccurrence.Capacity > 0 ? lotteryOccurrence.Capacity : 0;
                    }

                    totalSubscriptions += lotteryOccurrence.Subscriptions.Count;

                    var userId2s = lotteryOccurrence.Subscriptions.Select(x => x.UserId).Distinct().ToList();
                    userIds.AddRange(userId2s);
                }

                var realUserIds = userIds.Distinct();

                /*
                foreach (var userId in realUserIds)
                {
                    var subCount = lottery.Occurrences.Count(x => x.Subscriptions.Any(s => s.UserId.Equals(userId)));
                    totalSubscriptionPerUser += subCount;
                }
                var avgSubscriptionCount = totalSubscriptionPerUser / (double)lottery.Occurrences.Count;
                */


                var lm = new LotterySummaryModel
                {
                    Lottery = lottery,
                    Capacity = totalCapacity,
                    TotalSubscriptionCount = totalSubscriptions,
                    TotalSubscriberCount = realUserIds.Count(),
                    AvgSubscriptionCount = 0
                };

                model.Add(lm);
            }

            return View(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Test()
        {
            var model = Db.Lotteries.ToList();

            return View(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Create()
        {
            return View();
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Create(LotteryCreateModel model)
        {
            var firstDate = DateTime.Parse(model.FirstDrawing);
            var lastDate = DateTime.Parse(model.LastDrawing);
            var time = TimeSpan.Parse(model.DrawingTime);

            var me = GetMyMembership();

            var lottery = new Lottery
            {
                Name = model.Name,
                JobId = model.JobId,
                Description = model.Description,
                DrawingFrequency = DrawingFrequency.Daily,
                FirstDrawing = firstDate,
                LastDrawing = lastDate,
                DrawingTime = time,
                MaxConfirm = model.MaxConfirm,
                Owner = me,
                Organiser = me.Organiser,
                Semester = GetSemester()
            };


            Db.Lotteries.Add(lottery);
            Db.SaveChanges();


            return RedirectToAction("Index");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Edit(Guid id)
        {
            var lottery = Db.Lotteries.SingleOrDefault(l => l.Id == id);


            var model = new LotteryCreateModel
            {
                LotteryId = lottery.Id,
                Name = lottery.Name,
                JobId = lottery.Id.ToString(),
                Description = lottery.Description,
                FirstDrawing = lottery.FirstDrawing.ToShortDateString(),
                LastDrawing = lottery.LastDrawing.ToShortDateString(),
                DrawingTime = lottery.DrawingTime.ToString(),
                MaxConfirm = lottery.MaxConfirm,
            };

            return View(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Edit(LotteryCreateModel model)
        {
            var lottery = Db.Lotteries.SingleOrDefault(l => l.Id == model.LotteryId);


            var firstDate = DateTime.Parse(model.FirstDrawing);
            var lastDate = DateTime.Parse(model.LastDrawing);
            var time = TimeSpan.Parse(model.DrawingTime);

            var me = GetMyMembership();


            lottery.Name = model.Name;
            //    JobId = model.JobId
            lottery.Description = model.Description;
            lottery.DrawingFrequency = DrawingFrequency.Daily;
            lottery.FirstDrawing = firstDate;
            lottery.LastDrawing = lastDate;
            lottery.DrawingTime = time;
            lottery.MaxConfirm = model.MaxConfirm;
            lottery.Owner = me;

            Db.SaveChanges();

            // jetzt den BackgroundJob anlegen

            RecurringJob.AddOrUpdate<LotteryService>(
                lottery.Id.ToString(), 
                x => x.RunLottery(lottery.Id), Cron.Daily(lottery.DrawingTime.Hours, 
                lottery.DrawingTime.Minutes), TimeZoneInfo.Local);

            return RedirectToAction("Details", new {id = lottery.Id});
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Details(Guid id)
        {
            var lottery = Db.Lotteries.SingleOrDefault(l => l.Id == id);
            var org = GetMyOrganisation();

            var model = new LotteryLotPotModel
            {
                Lottery = lottery,
                LotteryId = lottery.Id,
            };


            var actService = new ActivityService(Db);
            var courseService = new CourseService();

            foreach (var occurrence in lottery.Occurrences)
            {
                var actSummary = actService.GetSummary(occurrence);
                var courseSummary = courseService.GetCourseSummary(actSummary.Activity);

                model.PotElements.Add(new LotteryLotPotCourseModel
                {
                    ActivitySummary = actSummary,
                    CourseSummary = courseSummary
                });
            }

            ViewBag.UserRight = GetUserRight(org);


            return View(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Run(Guid id)
        {
            var lottery = Db.Lotteries.SingleOrDefault(l => l.Id == id);

            if (lottery != null)
            {
                var service = new LotteryService();
                service.ExecuteLottery(lottery.Id);
            }

            return RedirectToAction("Details", new {id=id});
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Select(Guid id)
        {
            var lottery = Db.Lotteries.SingleOrDefault(l => l.Id == id);

            var model = new LotteryLotPotModel
            {
                Lottery = lottery,
                LotteryId = lottery.Id,
            };


            var actService = new ActivityService(Db);
            
            foreach (var occurrence in lottery.Occurrences)
            {
                var actSummary = actService.GetSummary(occurrence);

                model.PotElements.Add(new LotteryLotPotCourseModel
                {
                    ActivitySummary = actSummary,
                    CourseSummary = null
                });
            }

            var org = GetMyOrganisation();
            var semester = GetSemester();

            var allGroups = Db.SemesterGroups.Where(x =>
                x.Semester.Id == semester.Id &&
                x.CapacityGroup.CurriculumGroup.Curriculum.Organiser.Id == org.Id).ToList();


            ViewBag.GroupList = allGroups.Select(f => new SelectListItem
            {
                Text = f.GroupName,
                Value = f.Id.ToString(),
            });
            ;


            return View(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="searchText"></param>
        /// <returns></returns>
        [HttpPost]
        public PartialViewResult Search(string searchText)
        {
            var sem = GetSemester();
            var org = GetMyOrganisation();

            var courses = Db.Activities.OfType<Course>().Where(a =>
            (a.Organiser != null && a.Organiser.Id == org.Id ) && 
            (a.Name.Contains(searchText) || a.ShortName.Contains(searchText)) &&
                    a.SemesterGroups.Any(s => s.Semester.Id == sem.Id))
                .ToList();

            return PartialView("_CourseTable", courses);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult State(Guid id)
        {
            var lottery = Db.Lotteries.SingleOrDefault(l => l.Id == id);
            var semester = GetSemester();

            // Datenbank reparieren
            var occService = new OccurrenceService(UserManager);
            var courseService = new CourseService(UserManager);
            var semesterService = new SemesterService();

            var user = UserManager.FindByName(User.Identity.Name);
            Curriculum userCurr = null;

            if (user != null)
            {
                var sub = semesterService.GetSubscription(semester, user.Id);
                if (sub != null)
                {
                    userCurr = sub.SemesterGroup.CapacityGroup.CurriculumGroup.Curriculum;
                }
            }


            var wpmList = lottery.Occurrences.Select(occurrence => 
                Db.Activities.OfType<Course>().SingleOrDefault(c => c.Occurrence.Id == occurrence.Id)).ToList();


            var model = new WPMListModel();
            model.Lottery = lottery;
            model.User = user;
            model.Curricula.AddRange(Db.Curricula.Where(c => !c.ShortName.Equals("Export")).ToList());


            // jedes WPM durchgehen
            foreach (var wpm in wpmList)
            {
                var detail = new WPMDetailModel();

                detail.Course = wpm;
                detail.Summary = courseService.GetCourseSummary(wpm);
                //detail.OccurrenceState = ActivityService.GetActivityState(wpm.Occurrence, UserManager.FindByName(User.Identity.Name));
                //detail.Bookable = false;

                if (wpm.Occurrence.UseGroups)
                {
                    // Bei Gruppen wird keine Gesamtkapazität angegeben
                }
                else
                {
                    // ohne Gruppen
                    detail.Capacity = wpm.Occurrence.Capacity;
                    detail.Participients = occService.GetParticipiantCount(wpm.Occurrence);
                    detail.Pending = occService.GetPendingCount(wpm.Occurrence);
                    detail.Waiting = occService.GetWaitingCount(wpm.Occurrence);
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
                        capModel.Participients = occService.GetParticipiantCount(wpm.Occurrence, curriculum.ShortName, semester);
                        capModel.Pending = occService.GetPendingCount(wpm.Occurrence, curriculum.ShortName, semester);
                        capModel.Waiting = occService.GetWaitingCount(wpm.Occurrence, curriculum.ShortName, semester);
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Drawing(Guid id)
        {
            var lottery = Db.Lotteries.SingleOrDefault(l => l.Id == id);

            
            return View(lottery);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult ClearDrawings(Guid id)
        {
            var lottery = Db.Lotteries.SingleOrDefault(l => l.Id == id);

            foreach (var lotteryDrawing in lottery.Drawings.ToList())
            {
                foreach (var occurrenceDrawing in lotteryDrawing.OccurrenceDrawings.ToList())
                {
                    foreach (var subscriptionDrawing in occurrenceDrawing.SubscriptionDrawings.ToList())
                    {
                        Db.SubscriptionDrawings.Remove(subscriptionDrawing);
                    }
                    Db.OccurrenceDrawings.Remove(occurrenceDrawing);
                }

                Db.LotteryDrawings.Remove(lotteryDrawing);
            }

            Db.SaveChanges();

            return RedirectToAction("Details", new { id = id });
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Delete(Guid id)
        {
            var lottery = Db.Lotteries.SingleOrDefault(l => l.Id == id);

            var drawings = lottery.Drawings.ToList();
            foreach (var drawing in drawings)
            {
                Db.LotteryDrawings.Remove(drawing);
            }
            
            lottery.Occurrences.Clear();

            Db.Lotteries.Remove(lottery);

            Db.SaveChanges();

            return RedirectToAction("Index");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult DeleteAll(Guid id)
        {
            var org = GetMyOrganisation();
            var semester = GetSemester(id);

            var model = Db.Lotteries.Where(x =>
                x.Semester != null && x.Semester.Id == semester.Id &&
                x.Organiser != null && x.Organiser.Id == org.Id).ToList();

            foreach (var lottery in model)
            {
                var drawings = lottery.Drawings.ToList();
                foreach (var drawing in drawings)
                {
                    Db.LotteryDrawings.Remove(drawing);
                }

                lottery.Occurrences.Clear();

                Db.Lotteries.Remove(lottery);
            }


            Db.SaveChanges();

            return RedirectToAction("Index", new {id = id});
        }




        /// <summary>
        /// 
        /// </summary>
        /// <param name="LotteryId"></param>
        /// <param name="CourseIds"></param>
        /// <returns></returns>
        [HttpPost]
        public PartialViewResult ChangeLotPot(Guid LotteryId, Guid[] CourseIds)
        {
            // Übergeben werden die OccourrenceIds der Kurse!!!

            var lottery = Db.Lotteries.SingleOrDefault(l => l.Id == LotteryId);


            if (CourseIds != null)
            {
                var oldList = lottery.Occurrences.ToList();
                foreach (var occId in CourseIds)
                {
                    var occ = oldList.SingleOrDefault(o => o.Id == occId);
                    // schon drin => aus der Liste löschen
                    if (occ != null)
                    {
                        oldList.Remove(occ);
                    }
                    else
                    {
                        // das ist neu => hinzufügen
                        var occ2 = Db.Occurrences.SingleOrDefault(o => o.Id == occId);

                        // Aus Gründen der Kompatibilität beim Eintragen!
                        occ2.LotteryEnabled = true;

                        lottery.Occurrences.Add(occ2);
                    }

                }
                // die in der oldList verbliebenen Einträge kommen raus
                foreach (var occ in oldList)
                {
                    occ.LotteryEnabled = false;
                    lottery.Occurrences.Remove(occ);
                }
            }
            else
            {
                lottery.Occurrences.Clear();
            }


            Db.SaveChanges();

            //
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult MySelection(Guid id)
        {
            var userDb = new ApplicationDbContext();
            var occurrenceService = new OccurrenceService(UserManager);
            var courseService = new CourseService(UserManager);
            var activityService = new ActivityService();
            var semSubService = new SemesterSubscriptionService();
            var lotteryService = new LotteryService();

            var model = new WPMAdminModel();

            model.User = userDb.Users.SingleOrDefault(u => u.UserName.ToUpper().Equals(User.Identity.Name.ToUpper()));
            model.Lottery = lotteryService.GetLottery(id);

            var semester = GetSemester();
            var user = AppUser;

            var wpmList = lotteryService.GetLotteryCourseList(id);

            var nConfirmed = 0;

            var curriculum = semSubService.GetBestCurriculum(model.User.Id, semester);

            if (curriculum != null)
            {
                model.Curriculum = curriculum;

                foreach (var wpm in wpmList)
                {
                    // es könnten ja auch mehrere Eintragungen vorliegen
                    var wpmsubs = wpm.Occurrence.Subscriptions.Where(s => s.UserId.Equals(model.User.Id));

                    if (wpmsubs.Any())
                    {
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
                    else
                    {
                        var wpmSubscriptionModel = new WPMSubscriptionModel
                        {
                            Course = wpm,
                            Subscription = null,
                            IsAvailable = true,
                        };

                        var summary = courseService.GetCourseSummary(wpm);
                        summary.State = ActivityService.GetActivityState(wpm.Occurrence, user, semester);

                        wpmSubscriptionModel.Summary = summary;

                        wpmSubscriptionModel.IsValid = courseService.IsAvailableFor(wpm, curriculum.ShortName);

                        occurrenceService.GetCapacity(wpm.Occurrence, wpmSubscriptionModel, curriculum.ShortName, 0);

                        model.Available.Add(wpmSubscriptionModel);
                    }

                }
            }

            model.Confirmed = nConfirmed;

            ViewBag.UserRight = GetUserRight();

            return View(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="occId"></param>
        /// <param name="lotId"></param>
        /// <returns></returns>
        public ActionResult Confirm(Guid occId, Guid lotId)
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

            return RedirectToAction("MySelection", new { id = lotId });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="occId"></param>
        /// <param name="lotId"></param>
        /// <returns></returns>
        public ActionResult Release(Guid occId, Guid lotId)
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




            return RedirectToAction("MySelection", new { id = lotId });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="occId"></param>
        /// <param name="lotId"></param>
        /// <returns></returns>
        public ActionResult Discharge(Guid occId, Guid lotId)
        {
            var acController = new ActivityController();
            acController.ControllerContext = ControllerContext;
            acController.DeleteSubscription(occId);

            return RedirectToAction("MySelection", new {id =lotId});
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="occId"></param>
        /// <param name="lotId"></param>
        /// <returns></returns>
        public ActionResult Subscribe(Guid occId, Guid lotId)
        {
            var acController = new ActivityController();
            acController.ControllerContext = ControllerContext;
            acController.Subscribe(occId);

            return RedirectToAction("MySelection", new { id = lotId });
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult List(Guid id)
        {
            // Datenbank reparieren
            var occService = new OccurrenceService(UserManager);
            var courseService = new CourseService(UserManager);
            var semesterService = new SemesterService();
            var lotteryService = new LotteryService();

            var user = AppUser;
            var semester = GetSemester();

            Curriculum userCurr = null;

            if (user != null)
            {
                var sub = semesterService.GetSubscription(semester, user.Id);
                if (sub != null)
                {
                    userCurr = sub.SemesterGroup.CapacityGroup.CurriculumGroup.Curriculum;
                }
            }


            var wpmList = lotteryService.GetLotteryCourseList(id);


            var model = new WPMListModel();
            model.Lottery = lotteryService.GetLottery(id);
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Subscribers(Guid id)
        {
            var semester = GetSemester();

            var lotteryService = new LotteryService();

            var wpmList = lotteryService.GetLotteryCourseList(id);

            var wpmSubscribedList = wpmList.Where(c => c.Occurrence.Subscriptions.Any()).ToList();

            var allSubscriptionLists = wpmSubscribedList.Select(s => s.Occurrence.Subscriptions).ToList();

            var allSubscriptions = new List<OccurrenceSubscription>();
            foreach (var subs in allSubscriptionLists)
            {
                allSubscriptions.AddRange(subs);
            }

            WpmSubscriptionMasterModel model = new WpmSubscriptionMasterModel();
            model.Lottery = lotteryService.GetLottery(id);

            foreach (var subscription in allSubscriptions)
            {
                WpmSubscriptionDetailModel detailModel = null;

                if (!model.Subscriptions.ContainsKey(subscription.UserId))
                {
                    var user = UserManager.FindById(subscription.UserId);

                    if (user != null)
                    {
                        detailModel = new WpmSubscriptionDetailModel();

                        model.Subscriptions[subscription.UserId] = detailModel;

                        var semSub =
                            Db.Subscriptions.OfType<SemesterSubscription>()
                                .SingleOrDefault(
                                    s => s.UserId.Equals(user.Id) && s.SemesterGroup.Semester.Id == semester.Id);


                        model.Subscriptions[subscription.UserId].User = user;
                        if (semSub != null)
                        {
                            model.Subscriptions[subscription.UserId].Group = semSub.SemesterGroup;
                        }
                        model.Subscriptions[subscription.UserId].FirstAction = DateTime.Now;
                        model.Subscriptions[subscription.UserId].LastAction = new DateTime(2015, 1, 1);
                    }
                }
                else
                {
                    detailModel = model.Subscriptions[subscription.UserId];
                }

                if (detailModel != null)
                {

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
            }


            var list = model.Subscriptions.Values.ToList();

            model.SubscriptionList =
                list.Where(g => g.Group != null).OrderBy(m => m.Group.FullName)
                    .ThenByDescending(m => m.Confirmed)
                    .ThenByDescending(m => m.Reservations)
                    .ThenByDescending(m => m.Waiting)
                    .ToList();

            var group2 = list.Where(g => g.Group == null).ToList();
            foreach (var elem in group2)
            {
                model.SubscriptionList.Add(elem);
            }

            return View(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Reset(Guid id)
        {
            var lottery = Db.Lotteries.SingleOrDefault(l => l.Id == id);

            foreach (var wpm in lottery.Occurrences)
            {
                foreach (var sub in wpm.Subscriptions)
                {
                    sub.IsConfirmed = false;
                    sub.LapCount = 0;
                    sub.OnWaitingList = true;
                }
            }

            Db.SaveChanges();

            return RedirectToAction("Drawing", new {id = id });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Report(Guid id)
        {
            var drawing = Db.LotteryDrawings.SingleOrDefault(l => l.Id == id);


            return View(drawing);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult LotPot(Guid id)
        {
            var course = Db.Activities.OfType<Course>().SingleOrDefault(x => x.Id == id);

            return View(course);
        }


    }
}