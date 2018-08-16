using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Hangfire;
using log4net;
using log4net.Core;
using Microsoft.AspNet.Identity;
using MyStik.TimeTable.Data;
using MyStik.TimeTable.DataServices;
using MyStik.TimeTable.DataServices.Drawing;
using MyStik.TimeTable.Web.Jobs;
using MyStik.TimeTable.Web.Models;
using MyStik.TimeTable.Web.Services;

namespace MyStik.TimeTable.Web.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public class LotteryController : BaseController
    {
        private ILog logger = LogManager.GetLogger("Lottery");


        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Index(Guid? id)
        {
            var org = GetMyOrganisation();

            var semester = SemesterService.GetSemester(id);

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
                x.Semester != null && x.Semester.Id == semester.Id &&
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
        public ActionResult Create(Guid id)
        {
            var model = new LotteryCreateModel();

            model.SemesterId = id;

            return View(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Create(LotteryCreateModel model)
        {
            var firstDate = DateTime.Today;
            var lastDate = DateTime.Today;
            var time = TimeSpan.Zero;

            var me = GetMyMembership();

            var semester = SemesterService.GetSemester(model.SemesterId);

            var lottery = new Lottery
            {
                Name = model.Name,
                Description = model.Description,
                DrawingFrequency = DrawingFrequency.Daily,
                IsScheduled = false,
                FirstDrawing = firstDate,
                LastDrawing = lastDate,
                DrawingTime = time,
                MaxConfirm = 1,
                MaxSubscription = 1,
                IsActive = false,
                Owner = me,
                Organiser = me.Organiser,
                Semester = semester
            };


            Db.Lotteries.Add(lottery);
            Db.SaveChanges();

            logger.InfoFormat("Lotterie {0} angelegt", lottery.Name);

            return RedirectToAction("Details", new {id = lottery.Id});
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
                MaxConfirmException = lottery.MaxExceptionConfirm,
                MaxSubscription = lottery.MaxSubscription,
                MinSubscription = lottery.MinSubscription,
                IsActive = lottery.IsActive,
                IsAvailable = lottery.IsAvailable,
                IsAvailableFrom = lottery.IsActiveFrom.HasValue ? lottery.IsActiveFrom.Value.ToShortDateString() : "",
                IsAvailableUntil =
                    lottery.IsActiveUntil.HasValue ? lottery.IsActiveUntil.Value.ToShortDateString() : "",
                IsFlexible = false, // aktuell noch nicht möglich, weil kein Konzept dafür !lottery.IsFixed,
                AllowManualSubscription = lottery.AllowManualSubscription,
                LoIneeded = lottery.LoINeeded,
                IsScheduled = lottery.IsScheduled
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

            var isAvailableFrom = DateTime.Parse(model.IsAvailableFrom);
            var isAvailableUntil = DateTime.Parse(model.IsAvailableUntil);

            var me = GetMyMembership();


            lottery.Name = model.Name;
            //    JobId = model.JobId
            lottery.Description = model.Description;
            lottery.DrawingFrequency = DrawingFrequency.Daily;
            lottery.FirstDrawing = firstDate;
            lottery.LastDrawing = lastDate;
            lottery.DrawingTime = time;
            lottery.MaxConfirm = model.MaxConfirm;
            lottery.MaxExceptionConfirm = model.MaxConfirmException;
            lottery.MaxSubscription = model.MaxSubscription;
            lottery.MinSubscription = model.MinSubscription;
            lottery.IsActive = model.IsActive;
            lottery.Owner = me;
            lottery.IsAvailable = model.IsAvailable;
            lottery.IsActiveFrom = isAvailableFrom;
            lottery.IsActiveUntil = isAvailableUntil;
            lottery.IsFixed = !model.IsFlexible;
            lottery.AllowManualSubscription = model.AllowManualSubscription;
            lottery.LoINeeded = model.LoIneeded;
            lottery.IsScheduled = model.IsScheduled;

            
            Db.SaveChanges();
            logger.InfoFormat("Einstellungen zu Lotterie {0} verändert", lottery.Name);

            if (lottery.IsScheduled)
            {
                // jetzt den BackgroundJob anlegen bzw. aktualisieren
                RecurringJob.AddOrUpdate<LotteryJob>(
                    lottery.Id.ToString(),
                    x => x.RunLottery(lottery.Id), Cron.Daily(lottery.DrawingTime.Hours,
                        lottery.DrawingTime.Minutes), TimeZoneInfo.Local);

                logger.InfoFormat("Automatische Ausführung für Lotterie {0} angelegt", lottery.Name);
            }
            else
            {
                RecurringJob.RemoveIfExists(lottery.Id.ToString());

                logger.InfoFormat("Automatische Ausführung für Lotterie {0} gelöscht", lottery.Name);
            }

            return RedirectToAction("Details", new {id = lottery.Id});
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Details(Guid id)
        {
            var org = GetMyOrganisation();
            var model = new DrawingService(Db, id);

            if (model.Lottery.LastDrawing < new DateTime(2018, 6, 30))
            {
                return RedirectToAction("DetailsOld", new {id = id});
            }

            model.InitLotPots();

            ViewBag.UserRight = GetUserRight(org);

            return View(model);
        }


        public ActionResult DrawingPots(Guid id)
        {
            var model = new DrawingService(Db, id);

            model.InitLotPots();

            return View(model);
        }


        public ActionResult Students(Guid id)
        {
            var model = new DrawingService(Db, id);

            model.InitLotPots();

            return View(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Reset(Guid id)
        {

            var model = new DrawingService(Db, id);

            model.InitLotPots();

            // pro Studierendem eine E-Mail
            var drawing = new LotteryDrawing();
            drawing.Start = DateTime.Now;

            var mailService = new LotteryMailService(model);
            mailService.SendLotteryResetMails(drawing, GetMyMembership());

            var lottery = model.Lottery;

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

            logger.InfoFormat("Lotterie {0} zurückgesetzt - alle auf Warteliste", lottery.Name);


            return RedirectToAction("Details", new { id = id });
        }


        public ActionResult ClearLists(Guid id)
        {
            var model = new DrawingService(Db, id);

            model.InitLotPots();
            var lottery = model.Lottery;

            // pro Studierendem eine E-Mail
            var drawing = new LotteryDrawing();
            drawing.Start = DateTime.Now;

            var mailService = new LotteryMailService(model);
            mailService.SendLotteryClearedMails(drawing, GetMyMembership());


            // jetzt alle Wartelisten löschen
            var lotteryService = new LotteryService(Db, id);
            var subscriptionService = new SubscriptionService(Db);

            var courses = lotteryService.GetLotteryCourseList();

            foreach (var course in courses)
            {
                var subscriptions = course.Occurrence.Subscriptions.Where(s => s.OnWaitingList).ToList();
                foreach (var subscription in subscriptions)
                {
                    subscriptionService.DeleteSubscription(subscription);
                }
            }

            Db.SaveChanges();

            logger.InfoFormat("Lotterie {0} abgeschlossen - alle Eintragungen auf Warteliste gelöscht", lottery.Name);


            return RedirectToAction("Details", new { id = id });
        }






        public ActionResult DetailsOld(Guid id)
        {
            var lottery = Db.Lotteries.SingleOrDefault(l => l.Id == id);
            var org = GetMyOrganisation();

            var model = new LotteryLotPotModel
            {
                Lottery = lottery,
                LotteryId = lottery.Id,
            };


            var actService = new ActivityService(Db);
            var courseService = new CourseService(Db);

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
            var drawing = new LotteryDrawing();
            drawing.Start = DateTime.Now;

            var model = new DrawingService(Db, id);
            drawing.Lottery = model.Lottery;

            logger.InfoFormat("Manuelle Ausführung Lotterie {0} Verteilung gestartet", drawing.Lottery.Name);


            model.InitLotPots();
            ViewBag.Rounds = model.ExecuteDrawing();

            Db.SaveChanges();

            // Mailversand
            drawing.End = DateTime.Now;

            logger.InfoFormat("Manuelle Ausführung Lotterie {0} Verteilung beendet", drawing.Lottery.Name);
            logger.InfoFormat("Manuelle Ausführung Lotterie {0} Mailversand gestartet", drawing.Lottery.Name);


            var mailService = new LotteryMailService(model);
            mailService.SendDrawingMails(drawing);

            logger.InfoFormat("Manuelle Ausführung Lotterie {0} Mailversand beendet", drawing.Lottery.Name);


            return View("DrawingPots", model);

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
            var semester = SemesterService.GetSemester(DateTime.Today);

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
        /// <param name="semId"></param>
        /// <returns></returns>
        [HttpPost]
        public PartialViewResult Search(string searchText, Guid semId)
        {
            var sem = SemesterService.GetSemester(semId);
            var org = GetMyOrganisation();

            var courses = Db.Activities.OfType<Course>().Where(a =>
                    (a.Organiser != null && a.Organiser.Id == org.Id) &&
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
            var semester = SemesterService.GetSemester(DateTime.Today);

            // Datenbank reparieren
            var occService = new OccurrenceService(UserManager);
            var courseService = new CourseService(Db);
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
                        capModel.Participients =
                            occService.GetParticipiantCount(wpm.Occurrence, curriculum.ShortName, semester);
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

            return RedirectToAction("Details", new {id = id});
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Delete(Guid id)
        {
            var lottery = Db.Lotteries.SingleOrDefault(l => l.Id == id);
            var semester = lottery.Semester;

            var drawings = lottery.Drawings.ToList();
            foreach (var drawing in drawings)
            {
                Db.LotteryDrawings.Remove(drawing);
            }

            var budgets = lottery.Budgets.ToList();
            foreach (var budget in budgets)
            {
                var bets = budget.Bets.ToList();

                foreach (var bet in bets)
                {
                    Db.LotteriyBets.Remove(bet);
                }

                Db.LotteriyBudgets.Remove(budget);
            }

            var games = lottery.Games.ToList();
            foreach (var game in games)
            {
                Db.LotteryGames.Remove(game);
            }


            lottery.Occurrences.Clear();

            Db.Lotteries.Remove(lottery);

            Db.SaveChanges();

            logger.InfoFormat("Lotterie {0} gelöscht", lottery.Name);


            return RedirectToAction("Index", new {id = semester.Id});
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult DeleteAll(Guid id)
        {
            var org = GetMyOrganisation();
            var semester = SemesterService.GetSemester(id);

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
        public ActionResult Subscribers(Guid id)
        {

            var lotteryService = new LotteryService(Db, id);
            var lottery = lotteryService.GetLottery();

            var semester = lottery.Semester;

            // Alle LVs der Lottery
            var wpmList = lotteryService.GetLotteryCourseList();

            // Alle Kurse der Lottery mit Einstragungen
            var wpmSubscribedList = wpmList.Where(c => c.Occurrence.Subscriptions.Any()).ToList();

            // Liste aller Eintragungslisten
            var allSubscriptionLists = wpmSubscribedList.Select(s => s.Occurrence.Subscriptions).ToList();

            var allSubscriptions = new List<OccurrenceSubscription>();
            foreach (var subs in allSubscriptionLists)
            {
                allSubscriptions.AddRange(subs);
            }

            WpmSubscriptionMasterModel model = new WpmSubscriptionMasterModel();
            model.Lottery = lottery;

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

                        var alleSemesterSubscriptions = Db.Subscriptions.OfType<SemesterSubscription>().Where(x =>
                                x.SemesterGroup.Semester.Id == semester.Id && x.UserId.Equals(user.Id))
                            .OrderBy(x => x.TimeStamp).ToList();

                        while (alleSemesterSubscriptions.Count > 1)
                        {
                            var last = alleSemesterSubscriptions.Last();
                            Db.Subscriptions.Remove(last);
                            alleSemesterSubscriptions.Remove(last);
                            Db.SaveChanges();
                        }

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
        /// Auswertung einer Ziehung
        /// </summary>
        /// <param name="id">Ziehung</param>
        /// <returns></returns>
        public ActionResult Report(Guid id)
        {
            var drawing = Db.LotteryDrawings.SingleOrDefault(l => l.Id == id);


            return View(drawing);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lotteryId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public ActionResult SubscriberReport(Guid lotteryId, string userId)
        {
            var drawings = Db.SubscriptionDrawings.Where(
                x => x.OccurrenceDrawing.LotteryDrawing.Lottery.Id == lotteryId &&
                     x.Subscription.UserId.Equals(userId)).ToList();

            return View(drawings);
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

        public ActionResult Course(Guid lotteryId, Guid courseId)
        {
            var model = new LotteryCourseDetailViewModel();

            var lottery = Db.Lotteries.SingleOrDefault(l => l.Id == lotteryId);
            var course = Db.Activities.OfType<Course>().SingleOrDefault(x => x.Id == courseId);


            model.Course = course;
            model.Lottery = lottery;

            // alle anderen Kurse suchen
            var courseList = new List<Course>();

            foreach (var occurrence in lottery.Occurrences)
            {
                if (occurrence.Id != course.Occurrence.Id)
                {
                    var c = Db.Activities.OfType<Course>().SingleOrDefault(x => x.Occurrence.Id == occurrence.Id);
                    courseList.Add(c);
                }
            }

            // Alle Teilnehmer durchgehen
            foreach (var subscription in course.Occurrence.Subscriptions)
            {
                var user = UserManager.FindById(subscription.UserId);

                var m = new LotteryCourseSubscriber
                {
                    User = user,
                };

                m.Subscription.Course = course;
                m.Subscription.Subscription = subscription;

                // Alternativen
                var altCourses = courseList.Where(x => x.Occurrence.Subscriptions.Any(s => s.UserId.Equals(user.Id)));
                foreach (var altCourse in altCourses)
                {
                    var alt = new CourseSubscriptionViewModel
                    {
                        Course = altCourse,
                        Subscription = altCourse.Occurrence.Subscriptions.FirstOrDefault(x => x.UserId.Equals(user.Id))
                    };

                    m.Alternatives.Add(alt);
                }

                // wohin damit
                if (subscription.OnWaitingList)
                {
                    model.WaitingList.Add(m);
                }
                else
                {
                    if (subscription.IsConfirmed)
                    {
                        model.Participients.Add(m);
                    }
                    else
                    {
                        model.Reservations.Add(m);
                    }
                }
            }

            return View(model);
        }

        public ActionResult RemoveSubscription(Guid lotteryId, Guid occurrenceId, string userId)
        {
            var logger = LogManager.GetLogger("Course");

            var occurrence = Db.Occurrences.SingleOrDefault(oc => oc.Id == occurrenceId);
            var summary = ActivityService.GetSummary(occurrenceId);

            var subscription =
                occurrence.Subscriptions.FirstOrDefault(s => s.UserId.Equals(userId));

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

                occurrence.Subscriptions.Remove(subscription);
                Db.Subscriptions.Remove(subscription);
                Db.SaveChanges();

                var user = UserManager.FindById(subscription.UserId);

                logger.InfoFormat("Subscription removed: {0}, {1} by {2}", summary.Name, user.UserName,
                    User.Identity.Name);

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




            return RedirectToAction("Course", new {lotteryId = lotteryId, courseId = summary.Activity.Id});
        }

        public ActionResult RepairSubscription(Guid lotId, Guid subId)
        {
            var subscription = Db.Subscriptions.OfType<OccurrenceSubscription>().SingleOrDefault(x => x.Id == subId);

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

                Db.Subscriptions.Remove(subscription);
                Db.SaveChanges();
            }

            return RedirectToAction("Repair", new {id = lotId});
        }



        public ActionResult SetOnParticipiantList(Guid lotteryId, Guid occurrenceId, string userId)
        {
            var logger = LogManager.GetLogger("Course");

            var occurrence = Db.Occurrences.SingleOrDefault(oc => oc.Id == occurrenceId);
            var course = Db.Activities.OfType<Course>().SingleOrDefault(c => c.Occurrence.Id == occurrenceId);

            if (occurrence != null && !string.IsNullOrEmpty(userId))
            {
                var subscriptions =
                    occurrence.Subscriptions.Where(s => s.UserId.Equals(userId)).OrderBy(s => s.TimeStamp).ToList();

                var theOnlySubscription = subscriptions.LastOrDefault();

                if (theOnlySubscription != null)
                {
                    theOnlySubscription.OnWaitingList = false;
                    theOnlySubscription.LapCount = 0;
                    theOnlySubscription.IsConfirmed = true;
                    Db.SaveChanges();
                }
                else
                {
                    logger.ErrorFormat("subscription missing {0}, {1}", occurrence.Id, userId);
                }

            }
            else
            {
                logger.ErrorFormat("Occurrence or user missing [{0}], [{1}]", occurrenceId, userId);
            }


            return RedirectToAction("Course", new {lotteryId = lotteryId, courseId = course.Id});
        }

        public ActionResult SetOnWaitingList(Guid lotteryId, Guid occurrenceId, string userId)
        {
            var logger = LogManager.GetLogger("Course");

            var occurrence = Db.Occurrences.SingleOrDefault(oc => oc.Id == occurrenceId);
            var course = Db.Activities.OfType<Course>().SingleOrDefault(c => c.Occurrence.Id == occurrenceId);

            if (occurrence != null && !string.IsNullOrEmpty(userId))
            {
                var subscriptions =
                    occurrence.Subscriptions.Where(s => s.UserId.Equals(userId)).OrderBy(s => s.TimeStamp).ToList();

                var theOnlySubscription = subscriptions.LastOrDefault();

                if (theOnlySubscription != null)
                {
                    theOnlySubscription.OnWaitingList = true;
                    theOnlySubscription.LapCount = 0;
                    theOnlySubscription.IsConfirmed = false;
                    Db.SaveChanges();
                }
                else
                {
                    logger.ErrorFormat("subscription missing {0}, {1}", occurrence.Id, userId);
                }

            }
            else
            {
                logger.ErrorFormat("Occurrence or user missing [{0}], [{1}]", occurrenceId, userId);
            }


            return RedirectToAction("Course", new {lotteryId = lotteryId, courseId = course.Id});
        }

        public ActionResult Budgets(Guid id)
        {
            var model = Db.Lotteries.SingleOrDefault(x => x.Id == id);

            return View(model);
        }

        public ActionResult AddBudget(Guid id)
        {
            var model = new LotteryBudget
            {
                Name = "Neues Budget",
                Size = 100,
                IsRestricted = false,
                Lottery = Db.Lotteries.SingleOrDefault(x => x.Id == id)
            };


            return View(model);
        }

        [HttpPost]
        public ActionResult AddBudget(LotteryBudget model)
        {
            var lottery = Db.Lotteries.SingleOrDefault(x => x.Id == model.Lottery.Id);

            model.Fraction = (double) (model.Fraction / 100);
            model.Lottery = lottery;
            Db.LotteriyBudgets.Add(model);

            Db.SaveChanges();


            return RedirectToAction("Budgets", new {id = model.Lottery.Id});
        }


        public ActionResult EditBudget(Guid id)
        {
            var model = Db.LotteriyBudgets.SingleOrDefault(x => x.Id == id);

            model.Fraction = model.Fraction * 100;

            return View(model);
        }


        [HttpPost]
        public ActionResult EditBudget(LotteryBudget model)
        {
            var budget = Db.LotteriyBudgets.SingleOrDefault(x => x.Id == model.Id);

            budget.Name = model.Name;
            budget.Description = model.Description;
            budget.Size = model.Size;
            budget.Fraction = (double) (model.Fraction / 100);
            budget.IsRestricted = model.IsRestricted;

            Db.SaveChanges();


            return RedirectToAction("Budgets", new {id = model.Lottery.Id});
        }

        public ActionResult DeleteBudget(Guid id)
        {
            var budget = Db.LotteriyBudgets.SingleOrDefault(x => x.Id == id);

            var lottery = budget.Lottery;

            var bets = budget.Bets.ToList();

            foreach (var bet in bets)
            {
                Db.LotteriyBets.Remove(bet);
            }

            Db.LotteriyBudgets.Remove(budget);

            Db.SaveChanges();



            return RedirectToAction("Budgets", new {id = lottery.Id});
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">Id der Lotterie</param>
        /// <returns></returns>
        public ActionResult MySelection(Guid id)
        {
            var userDb = new ApplicationDbContext();
            var lotteryService = new LotteryService(Db, id);

            var model = new LotteryGambleViewModel();
            var lottery = lotteryService.GetLottery();
            var user = AppUser;

            // ist Lotterie gestartet?
            var now = DateTime.Now;
            var isRunning = lottery.Drawings.Any(x => x.Start <= now && now < x.End);


            model.User = userDb.Users.SingleOrDefault(u => u.UserName.ToUpper().Equals(User.Identity.Name.ToUpper()));
            model.Lottery = lottery;



            if (isRunning)
            {
                return View("_LotteryRunning", model);
            }


            var wpmList = lotteryService.GetLotteryCourseList();

            foreach (var wpm in wpmList)
            {
                // immer die doppelten entfernen
                // TODO Doppelte Bets entfernen
                // TODO Doppelte Eintragungen entfernen => immer den kleineren
                // TODO Budgetgrenze prüfen???
                // occurrenceService.CheckDoubles(wpm.Occurrence, model.User.Id);
                var courseModel = GetCourseViewModel(model.Lottery, wpm, user);
                model.Courses.Add(courseModel);

                // Wie viele sind bereits angenommen?
                if (courseModel.Subscription != null &&
                    !courseModel.Subscription.OnWaitingList &&
                    courseModel.Subscription.IsConfirmed)
                    model.Confirmed++;

                if (courseModel.Subscription != null)
                    model.Subscribed++;
            }


            // Ist gleichverteilt gesetzt?
            foreach (var lotteryBudget in lottery.Budgets)
            {
                var budgetModel = new LotteryGambleBudgetStateViewModel {Budget = lotteryBudget};

                // alle Einsätze
                foreach (var courseViewModel in model.Courses)
                {
                    // nur Wartelisteneinträge
                    if (courseViewModel.Subscription != null && courseViewModel.Subscription.OnWaitingList)
                    {
                        var bet = courseViewModel.Subscription.Bets.FirstOrDefault(x =>
                            x.Budget.Id == lotteryBudget.Id);
                        budgetModel.Bets.Add(bet);
                    }
                }

                model.BudgetStates.Add(budgetModel);
            }


            ViewBag.UserRight = GetUserRight();
            // TODO: reale Loszeit

            var today = DateTime.Today;
            var firstDrawing = lottery.FirstDrawing.Add(lottery.DrawingTime);
            var lastDrawing = lottery.LastDrawing.Add(lottery.DrawingTime);
            var todayDrawing = today.Add(lottery.DrawingTime);

            DateTime? nextDrawing = todayDrawing;

            if (lastDrawing < todayDrawing)
            {
                nextDrawing = null;
            }
            else
            {
                if (todayDrawing < firstDrawing)
                {
                    nextDrawing = firstDrawing;
                }
                else
                {
                    if (todayDrawing <= DateTime.Now)
                    {
                        nextDrawing = todayDrawing.AddDays(1);
                    }
                }
            }


            ViewBag.NextDrawing = nextDrawing;

            return View("MySelectionNew", model);
        }



        public ActionResult Bet(Guid courseId, Guid lotId)
        {
            var lottery = Db.Lotteries.SingleOrDefault(x => x.Id == lotId);
            var course = Db.Activities.OfType<Course>().SingleOrDefault(x => x.Id == courseId);
            var user = GetCurrentUser();


            var model = GetCourseViewModel(lottery, course, user);


            return View(model);
        }





        [HttpPost]
        public PartialViewResult EnterBet(Guid courseId, Guid lotteryId, Guid[] bets, int[] points)
        {
            for (int i = 0; i < bets.Length; i++)
            {
                var id = bets[i];

                var myBet = Db.LotteriyBets.SingleOrDefault(x => x.Id == id);

                myBet.Amount = points[i];
            }

            Db.SaveChanges();

            return PartialView("_EmptyRow");
        }


        [HttpPost]
        public PartialViewResult EditBet(Guid courseId, Guid lotteryId)
        {
            var course = Db.Activities.OfType<Course>().SingleOrDefault(x => x.Id == courseId);
            var lottery = Db.Lotteries.SingleOrDefault(x => x.Id == lotteryId);

            var model = GetCourseViewModel(lottery, course, AppUser);

            return PartialView("_BetEditor", model);
        }

        [HttpPost]
        public PartialViewResult Refresh(Guid courseId, Guid lotteryId)
        {
            var course = Db.Activities.OfType<Course>().SingleOrDefault(x => x.Id == courseId);
            var lottery = Db.Lotteries.SingleOrDefault(x => x.Id == lotteryId);

            var model = GetCourseViewModel(lottery, course, AppUser);

            return PartialView("_BetState", model);
        }


        public ActionResult Repair(Guid id)
        {
            var lotteryService = new LotteryService(Db, id);
            var lottery = lotteryService.GetLottery();

            // Alle LVs der Lottery
            var courseList = lotteryService.GetLotteryCourseList();

            // Alle Kurse der Lottery mit Einstragungen
            /*
            var occurrences = courseList.Where(c => c.Occurrence.Subscriptions.Any()).Select(o => o.Occurrence).ToList();

            var result = Db.Subscriptions.OfType<OccurrenceSubscription>()
                .Join(Db.Occurrences,
                    s => s.Occurrence.Id,
                    o => o.Id,
                    (s, o) => new { Subscription = s, Occurrence = o })
                .Where(x => x.Subscription.Occurrence.Id == x.Occurrence.Id).ToList();

            var n = result.Count;
            */

            var allSubscriptionLists = courseList.Select(s => s.Occurrence.Subscriptions).ToList();

            var allSubscriptions = new List<OccurrenceSubscription>();
            foreach (var subs in allSubscriptionLists)
            {
                allSubscriptions.AddRange(subs);
            }

            var model = new LotterySummaryModel
            {
                Lottery = lottery,
                Subscriptions = allSubscriptions.GroupBy(g => g.UserId).ToList()
            };

            return View(model);
        }


        public ActionResult AssignPoints(Guid betId, int? amount)
        {
            var bet = Db.LotteriyBets.SingleOrDefault(x => x.Id == betId);
            var lottery = bet.Budget.Lottery;

            if (amount.HasValue)
            {
                bet.Amount = amount.Value;
                Db.SaveChanges();
            }

            return RedirectToAction("Repair", new {id = lottery.Id});
        }

        public ActionResult RemoveBet(Guid id)
        {
            var bet = Db.LotteriyBets.SingleOrDefault(x => x.Id == id);

            var lottery = bet.Budget.Lottery;

            Db.LotteriyBets.Remove(bet);
            Db.SaveChanges();

            return RedirectToAction("LotPot", new {id = lottery.Id});
        }

        public ActionResult Check(Guid id)
        {
            var org = GetMyOrganisation();
            var semester = SemesterService.GetSemester(id);
            var userServive = new UserInfoService();

            var alLotteries = Db.Lotteries.Where(x =>
                x.Semester != null && x.Semester.Id == semester.Id &&
                x.Organiser != null && x.Organiser.Id == org.Id).ToList();


            var model = new List<LotteryCheckModel>();

            foreach (var lottery in alLotteries)
            {
                // Alle LVs der Lottery
                var courseList = lottery.Occurrences.Select(
                    occurrence => Db.Activities.OfType<Course>().SingleOrDefault(
                        c => c.Occurrence.Id == occurrence.Id)).Where(course => course != null);

                // Alle Studierenden der Lotterie ermitteln
                var allSubscriptionLists = courseList.Select(s => s.Occurrence.Subscriptions).ToList();
                var allSubscriptions = new List<OccurrenceSubscription>();
                foreach (var subs in allSubscriptionLists)
                {
                    allSubscriptions.AddRange(subs);
                }

                var subscribers = allSubscriptions.GroupBy(g => g.UserId).ToList();

                // Alle Studierende durchgehen
                foreach (var subscriber in subscribers)
                {
                    // Alle Eintragungen zusammengefasst nach Occurrences
                    var occurrences = subscriber.GroupBy(x => x.Occurrence.Id).ToList();

                    // Mehr Eintragungen als erlaubt
                    if (occurrences.Count > lottery.MaxSubscription)
                    {
                        var user = userServive.GetUser(subscriber.Key);

                        var lm = new LotteryCheckModel
                        {
                            User = user,
                            Lottery = lottery,
                            Message = "Hat mehr Eintragungen als erlaubt"
                        };

                        model.Add(lm);
                    }

                    // irgendwo doppelt?
                    var originCourses = subscriber.Select(x => x.Occurrence.Id).Distinct().ToList();
                    if (occurrences.Count > originCourses.Count)
                    {
                        var user = userServive.GetUser(subscriber.Key);

                        var lm = new LotteryCheckModel
                        {
                            User = user,
                            Lottery = lottery,
                            Message = "Ist mehrfach in einem Kurs eingetragen"
                        };

                        model.Add(lm);

                    }

                    foreach (var subscription in subscriber)
                    {
                        if (subscription.Bets.Count > lottery.Budgets.Count)
                        {
                            var user = userServive.GetUser(subscriber.Key);

                            var lm = new LotteryCheckModel
                            {
                                User = user,
                                Lottery = lottery,
                                Message = "Mehr Einsätze als Budgets"
                            };

                            model.Add(lm);
                        }


                        if (subscription.Bets.Sum(x => x.Amount) > 100)
                        {
                            var user = userServive.GetUser(subscriber.Key);

                            var lm = new LotteryCheckModel
                            {
                                User = user,
                                Lottery = lottery,
                                Message = "Mehr gesetzt als erlaubt"
                            };

                            model.Add(lm);
                        }


                    }

                }





            }



            return View(model);
        }


        private LotteryGambleCourseViewModel GetCourseViewModel(Lottery lottery, Course wpm, ApplicationUser user)
        {
            var courseService = new CourseService(Db);

            var courseModel = new LotteryGambleCourseViewModel
            {
                Course = wpm,
                Summary = courseService.GetCourseSummary(wpm),
                Lottery = lottery
            };

            foreach (var subscription in wpm.Occurrence.Subscriptions)
            {
                var subModel = new LotteryGambleSubscriptionViewModel();

                subModel.Subscription = subscription;

                // für die eigene Eintragung die zusätzlichen Infos aufbereiten
                if (subscription.UserId.Equals(user.Id))
                {
                    // alle Budgets prüfen
                    // und ggf. in der Datenbank anlegen
                    foreach (var budget in lottery.Budgets)
                    {
                        var bet = subscription.Bets.SingleOrDefault(x => x.Budget.Id == budget.Id);

                        if (bet != null) continue;
                        bet = new LotteryBet
                        {
                            Subscription = subscription,
                            Budget = budget,
                            IsApproved = !budget.IsRestricted,
                            AmountConsumed = 0,
                            Amount = 0
                        };
                        subscription.Bets.Add(bet);
                        Db.LotteriyBets.Add(bet);
                        Db.SaveChanges();
                    }

                    // Jeder Einsatz gehört zu exakt einem Budget
                    foreach (var bet in subscription.Bets)
                    {
                        var betModel = new LotteryGambleBudgetViewModel
                        {
                            User = user,
                            Bet = bet,
                            Budget = bet.Budget
                        };

                        courseModel.Budgets.Add(betModel);
                    }

                    courseModel.Subscription = subscription;
                }

                courseModel.Subscriptions.Add(subModel);
            }

            if (!courseModel.Budgets.Any())
            {
                foreach (var budget in lottery.Budgets)
                {
                    var betModel = new LotteryGambleBudgetViewModel
                    {
                        User = user,
                        Bet = null,
                        Budget = budget
                    };

                    courseModel.Budgets.Add(betModel);
                }
            }


            return courseModel;
        }

        public ActionResult Overview(Guid id)
        {
            var courseService = new CourseService(Db);
            var lotteryService = new LotteryService(Db, id);
            var studentService = new StudentService(Db);

            var user = GetCurrentUser();
            var student = studentService.GetCurrentStudent(user);
            var lottery = lotteryService.GetLottery();
            var courses = lotteryService.GetLotteryCourseList();

            var game = lottery.Games.FirstOrDefault(x => x.UserId.Equals(user.Id));


            var model = new LotteryOverviewModel
            {
                Lottery = lottery,
                Student = student,
                Game = game
            };


            // Die Liste der Kurse
            foreach (var course in courses)
            {
                model.Courses.Add(new LotteryOverviewCourseModel
                {
                    CourseSummary = courseService.GetCourseSummary(course)
                });
            }



            // die gewählten Kurse angeben
            foreach (var course in courses)
            {
                var subscription = course.Occurrence.Subscriptions.FirstOrDefault(x => x.UserId.Equals(user.Id));
                if (subscription != null)
                {
                    var courseModel = new LotteryOverviewCourseModel
                    {
                        CourseSummary = courseService.GetCourseSummary(course),
                        Subscription = subscription
                    };

                    model.CoursesSelected.Add(courseModel);
                }
            }



            return View(model);

        }

        public ActionResult Selection(Guid id)
        {
            var courseService = new CourseService(Db);
            var lotteryService = new LotteryService(Db, id);

            var lottery = lotteryService.GetLottery();
            var courses = lotteryService.GetLotteryCourseList();

            // nur noch die Kurse, die gewählt werden können


            var model = new LotteryOverviewModel
            {
                Lottery = lottery,
            };

            // Die Liste der Kurse
            foreach (var course in courses)
            {
                var summary = courseService.GetCourseSummary(course);
                var isSelectable = true;
                var msg = new StringBuilder();

                if (!course.Occurrence.IsAvailable)
                {
                    isSelectable = false;
                    msg.AppendLine("<li><i class=\"fa fa-li fa-lock\"></i>Lehrveranstaltung ist für Eintragungen gesperrt</li>");
                }

                if (course.Occurrence.Capacity > 0 && course.Occurrence.Subscriptions.Count(x => !x.OnWaitingList) >=
                    course.Occurrence.Capacity)
                {
                    isSelectable = false;
                    msg.AppendLine("<li><i class=\"fa fa-li fa-times\"></i>Keine freien Plätze in der Lehrveranstaltung verfügbar</li>");
                }

                model.Courses.Add(new LotteryOverviewCourseModel
                {
                    CourseSummary = summary,
                    IsSelectable = isSelectable,
                    Message = msg.ToString()
                });

            }


            return View(model);
        }

        public ActionResult SelectCourses(Guid id)
        {
            var courseService = new CourseService(Db);
            var lotteryService = new LotteryService(Db, id);
            var studentService = new StudentService(Db);

            var user = GetCurrentUser();
            var student = studentService.GetCurrentStudent(user);
            var lottery = lotteryService.GetLottery();
            var courses = lotteryService.GetLotteryCourseList();

            var game = lottery.Games.FirstOrDefault(x => x.UserId.Equals(user.Id));

            // nur noch die Kurse, die gewählt werden können


            var model = new LotteryOverviewModel
            {
                Lottery = lottery,
                Student = student,
                Game = game
            };

            // Die Liste der Kurse
            foreach (var course in courses)
            {
                var summary = courseService.GetCourseSummary(course);
                var isSelectable = true;
                var msg = new StringBuilder();

                if (student?.Curriculum == null || course.Occurrence.IsCoterie && !summary.Curricula.Contains(student.Curriculum))
                {
                    isSelectable = false;
                    msg.AppendLine("<li><i class=\"fa fa-li fa-ban\"></i> Lehrveranstaltung steht für Ihren Studiengang nicht zur Verfügung</li>");
                }

                if (!course.Occurrence.IsAvailable)
                {
                    isSelectable = false;
                    msg.AppendLine("<li><i class=\"fa fa-li fa-lock\"></i>Lehrveranstaltung ist für Eintragungen gesperrt</li>");
                }

                if (course.Occurrence.Capacity > 0 && course.Occurrence.Subscriptions.Count(x => !x.OnWaitingList) >=
                    course.Occurrence.Capacity)
                {
                    isSelectable = false;
                    msg.AppendLine("<li><i class=\"fa fa-li fa-times\"></i>Keine freien Plätze in der Lehrveranstaltung verfügbar</li>");
                }

                model.Courses.Add(new LotteryOverviewCourseModel
                {
                    CourseSummary = summary,
                    IsSelectable = isSelectable,
                    Message = msg.ToString()
                });

            }


            return View(model);
        }


        [HttpPost]
        public PartialViewResult CoursesSelected(Guid lotteryId, ICollection<Guid> courseIds)
        {
            var courseService = new CourseService(Db);
            var lotteryService = new LotteryService(Db, lotteryId);
            var studentService = new StudentService(Db);

            var user = GetCurrentUser();
            var student = studentService.GetCurrentStudent(user);
            var lottery = lotteryService.GetLottery();
            var courses = lotteryService.GetLotteryCourseList();

            var game = lottery.Games.FirstOrDefault(x => x.UserId.Equals(user.Id));

            var model = new LotteryOverviewModel
            {
                Lottery = lottery,
                Student = student,
                Game = game
            };

            // Die Liste der Kurse
            foreach (var course in courses)
            {
                // jetzt die Auswahl
                if (courseIds.Contains(course.Id))
                {
                    model.Courses.Add(new LotteryOverviewCourseModel
                    {
                        CourseSummary = courseService.GetCourseSummary(course)
                    });
                }
            }


            return PartialView("_RankCourses", model);
        }


        [HttpPost]
        public PartialViewResult CoursesOrdered(Guid lotteryId, ICollection<Guid> courseIds, int confirm,
            bool acceptAny)
        {
            var courseService = new CourseService(Db);
            var lotteryService = new LotteryService(Db, lotteryId);
            var studentService = new StudentService(Db);

            var user = GetCurrentUser();
            var student = studentService.GetCurrentStudent(user);
            var lottery = lotteryService.GetLottery();
            var courses = lotteryService.GetLotteryCourseList();

            var game = lottery.Games.FirstOrDefault(x => x.UserId.Equals(user.Id));

            var model = new LotteryOverviewModel
            {
                Lottery = lottery,
                Student = student,
                Game = game,
                ConfirmCount = confirm,
                AcceptAny = acceptAny
            };

            // Die Liste der Kurse nach Wahl
            foreach (var courseId in courseIds)
            {
                model.Courses.Add(new LotteryOverviewCourseModel
                {
                    CourseSummary = courseService.GetCourseSummary(courseId)
                });
            }


            return PartialView("_ConfirmSelection", model);
        }



        [HttpPost]
        public ActionResult SelectionConfirmed(Guid lotteryId, ICollection<Guid> courseIds, int confirm, bool acceptAny)
        {
            var courseService = new CourseService(Db);
            var lotteryService = new LotteryService(Db, lotteryId);
            var studentService = new StudentService(Db);
            var subscriptionService = new SubscriptionService(Db);

            var user = GetCurrentUser();
            var student = studentService.GetCurrentStudent(user);
            var lottery = lotteryService.GetLottery();
            var courses = lotteryService.GetLotteryCourseList();

            var game = lottery.Games.FirstOrDefault(x => x.UserId.Equals(user.Id));

            // Es darf kein Spiel existieren
            if (game == null)
            {
                game = new LotteryGame();
                game.Created = DateTime.Now;
                game.Lottery = lottery;
                game.UserId = user.Id;

                Db.LotteryGames.Add(game);
            }

            // Alle Einstellungen überschreiben
            game.AcceptDefault = acceptAny;
            game.CoursesWanted = confirm;
            game.LastChange = DateTime.Now;

            // jetzt die Eintragungen
            // Zuerst bei allen bestehenden Eintragungen mit Warteliste => Löschen
            // Grund: Eintragung für Lotterien nur hier
            var coursesOnWaitinglist = courses.Where(x =>
                x.Occurrence.Subscriptions.Any(s => s.UserId.Equals(user.Id) && s.OnWaitingList));
            foreach (var course in coursesOnWaitinglist)
            {
                var subscriptions = course.Occurrence.Subscriptions.Where(s => s.UserId.Equals(user.Id)).ToList();
                foreach (var subscription in subscriptions)
                {
                    subscriptionService.DeleteSubscription(subscription);
                }
            }

            int[] points = {25, 18, 15, 12, 10, 8, 6, 4, 2, 1};

            // Reihenfolge = Priorität
            // Vergabe der Punkte über den Lapcount - keine Budgets
            var i = 0;
            foreach (var courseId in courseIds)
            {
                var course = courseService.GetCourseSummary(courseId);

                // liegt bereits eine Eintragung vor?
                var subscription = course.Course.Occurrence.Subscriptions.FirstOrDefault(x => x.UserId.Equals(user.Id));
                if (subscription == null)
                {
                    // Eintragung anlegen
                    subscription = new OccurrenceSubscription();
                    subscription.OnWaitingList = true;
                    subscription.UserId = user.Id;
                    subscription.TimeStamp = DateTime.Now;
                    subscription.Occurrence = course.Course.Occurrence;

                    Db.Subscriptions.Add(subscription);
                }

                i++;
                subscription.Priority = i;      // Punkteumrechnung kommt später

            }

            Db.SaveChanges();






            var model = new LotteryOverviewModel
            {
                Lottery = lottery,
                Student = student,
                Game = game,
                ConfirmCount = confirm,
                AcceptAny = acceptAny
            };

            // Die Liste der Kurse nach Wahl
            foreach (var courseId in courseIds)
            {
                model.Courses.Add(new LotteryOverviewCourseModel
                {
                    CourseSummary = courseService.GetCourseSummary(courseId)
                });
            }

            // Mail versenden
            var mailModel = new LotterySelectionMailModel();

            mailModel.User = user;
            mailModel.Lottery = lottery;
            mailModel.AcceptAny = model.AcceptAny;
            mailModel.ConfirmCount = model.ConfirmCount;
            mailModel.Courses.AddRange(model.Courses);
            mailModel.SelectDate = DateTime.Now;

            var mail = new MailController();
            mail.LotterySelectionEMail(mailModel).Deliver();


            return PartialView("_SelectionFinished", model);
        }

        public ActionResult ClearSelection(Guid id)
        {
            var courseService = new CourseService(Db);
            var lotteryService = new LotteryService(Db, id);
            var studentService = new StudentService(Db);
            var subscriptionService = new SubscriptionService(Db);

            var user = GetCurrentUser();
            var student = studentService.GetCurrentStudent(user);
            var lottery = lotteryService.GetLottery();
            var courses = lotteryService.GetLotteryCourseList();

            var game = lottery.Games.FirstOrDefault(x => x.UserId.Equals(user.Id));

            foreach (var course in courses)
            {
                var subscriptions = course.Occurrence.Subscriptions.Where(s => s.UserId.Equals(user.Id) && s.OnWaitingList).ToList();
                foreach (var subscription in subscriptions)
                {
                    subscriptionService.DeleteSubscription(subscription);
                }
            }

            Db.SaveChanges();


            return RedirectToAction("Overview", new { id = id });
        }


        public ActionResult RemoveSelection(Guid id)
        {
            var courseService = new CourseService(Db);
            var lotteryService = new LotteryService(Db, id);
            var studentService = new StudentService(Db);
            var subscriptionService = new SubscriptionService(Db);

            var user = GetCurrentUser();
            var student = studentService.GetCurrentStudent(user);
            var lottery = lotteryService.GetLottery();
            var courses = lotteryService.GetLotteryCourseList();



            return View(lottery);
        }



        public ActionResult RemoveSelectionConfirmed(Guid id)
        {
            var courseService = new CourseService(Db);
            var lotteryService = new LotteryService(Db, id);
            var studentService = new StudentService(Db);
            var subscriptionService = new SubscriptionService(Db);

            var user = GetCurrentUser();
            var student = studentService.GetCurrentStudent(user);
            var lottery = lotteryService.GetLottery();
            var courses = lotteryService.GetLotteryCourseList();

            var game = lottery.Games.FirstOrDefault(x => x.UserId.Equals(user.Id));

            foreach (var course in courses)
            {
                var subscriptions = course.Occurrence.Subscriptions.Where(s => s.UserId.Equals(user.Id)).ToList();
                foreach (var subscription in subscriptions)
                {
                    subscriptionService.DeleteSubscription(subscription);
                }
            }

            Db.LotteryGames.Remove(game);
            Db.SaveChanges();


            return RedirectToAction("Overview", new {id = id});
        }


        public ActionResult EditReference(Guid lotteryId, Guid courseId)
        {
            var user = GetCurrentUser();

            var course = Db.Activities.OfType<Course>().SingleOrDefault(x => x.Id == courseId);
            var lottery = Db.Lotteries.SingleOrDefault(x => x.Id == lotteryId);

            var subscription = course.Occurrence.Subscriptions.FirstOrDefault(x => x.UserId.Equals(user.Id));


            var model = new CourseReferenceModel();
            model.Lottery = lottery;
            model.Course = course;
            model.Message = subscription.SubscriberRemark;
            model.Subscription = subscription;



            return View(model);
        }

        [HttpPost]
        public ActionResult EditReference(CourseReferenceModel model)
        {
            var lottery = Db.Lotteries.SingleOrDefault(x => x.Id == model.Lottery.Id);
            var subscription = Db.Subscriptions.OfType<OccurrenceSubscription>().SingleOrDefault(x => x.Id == model.Subscription.Id);

            subscription.SubscriberRemark = model.Message;
            Db.SaveChanges();



            return RedirectToAction("Overview", new { id = lottery.Id });
        }


        public ActionResult InitTestUser(Guid id)
        {
            var lotteryService = new LotteryService(Db, id);

            var lottery = lotteryService.GetLottery();


            var org = GetMyOrganisation();
            var semester = SemesterService.GetSemester(DateTime.Today);

            var curricula = org.Curricula.ToList();

            for (var i = 1; i <= 100; i++)
            {
                var userName = $"{org.ShortName}.test.stud{i:000}";

                var user = UserManager.FindByName(userName);
                if (user == null)
                {
                    user = new ApplicationUser
                    {
                        Email = $"{userName}@acceleratex.org",
                        UserName = userName,
                        FirstName = userName,
                        LastName = "TEST",
                        MemberState = MemberState.Student
                    };

                    var result = UserManager.Create(user, "Pas1234?");
                    if (result == null) throw new ArgumentNullException("result");
                    user = UserManager.FindByName(userName);
                }


                var student = Db.Students.Where(x => x.UserId.Equals(user.Id)).OrderByDescending(x => x.Created)
                    .FirstOrDefault();

                if (student == null)
                {
                    curricula.Shuffle();

                    student = new Student();
                    student.Curriculum = curricula.First();
                    student.UserId = user.Id;
                    student.FirstSemester = semester;
                    student.Created = DateTime.Now;

                    Db.Students.Add(student);
                }
            }

            Db.SaveChanges();

            return RedirectToAction("TestRun", new { id = lottery.Id });
        }



        public ActionResult InitTest(Guid id)
        {
            var lotteryService = new LotteryService(Db, id);

            var lottery = lotteryService.GetLottery();

            var model = new LotteryTestModel();
            model.Lottery = lottery;
            model.Capacity = 10;
            model.IsCoterie = true;

            return View(model);
        }

        [HttpPost]
        public ActionResult InitTest(LotteryTestModel model)
        {
            var lotteryService = new LotteryService(Db, model.Lottery.Id);

            var lottery = lotteryService.GetLottery();
            var courses = lotteryService.GetLotteryCourseList();

            foreach (var course in courses)
            {
                if (course.Occurrence.Capacity <= 0)
                {
                    course.Occurrence.Capacity = model.Capacity;
                }
                else
                {
                    course.Occurrence.Capacity += model.Capacity;
                }

                course.Occurrence.IsCoterie = model.IsCoterie;
                course.Occurrence.UseGroups = false;
            }

            Db.SaveChanges();

            return RedirectToAction("TestRun", new { id = lottery.Id });
        }



        public ActionResult SubscribeTest(Guid id)
        {
            var userDb = new ApplicationDbContext();
            var lotteryService = new LotteryService(Db, id);

            var lottery = lotteryService.GetLottery();
            var courses = lotteryService.GetLotteryCourseList();
            var org = GetMyOrganisation();

            var userName = $"{org.ShortName}.test.stud";

            var users = userDb.Users.Where(x => x.UserName.StartsWith(userName)).ToList();

            var rnd = new Random();

            // jeden testuser durchgehen
            foreach (var user in users)
            {
                var student = Db.Students.Where(x => x.UserId.Equals(user.Id)).OrderByDescending(x => x.Created)
                    .FirstOrDefault();

                // das Spiel anlegen
                var game = lottery.Games.FirstOrDefault(x => x.UserId.Equals(user.Id));

                // Es darf kein Spiel existieren
                if (game == null)
                {
                    game = new LotteryGame();
                    game.Created = DateTime.Now;
                    game.Lottery = lottery;
                    game.UserId = user.Id;

                    Db.LotteryGames.Add(game);
                }

                // Alle Einstellungen überschreiben
                // bei Random.Next ist die Obergrenze exklusive, daher das +1
                // wie viele Kurse belegen?
                int confirmCount = rnd.Next(lottery.MaxConfirm, lottery.MaxExceptionConfirm + 1);
                // welche Pechregel?
                bool acceptAny = rnd.Next(0, 2) == 1 ? true : false;

                game.AcceptDefault = acceptAny;
                game.CoursesWanted = confirmCount;
                game.LastChange = DateTime.Now;


                // wie viele Kurse wählen?
                int selectionCount = rnd.Next(lottery.MinSubscription, lottery.MaxSubscription + 1);

                // Liste aller möglichen Kurse
                var coursePortfolio = new List<Course>();

                foreach (var course in courses)
                {
                    // Berücksichtigung der "geschlossenen Gesellschaft"
                    if (course.Occurrence.IsCoterie)
                    {
                        if (course.SemesterGroups.Any(x =>
                            x.CapacityGroup.CurriculumGroup.Curriculum.Id == student.Curriculum.Id))
                        {
                            coursePortfolio.Add(course);
                        }
                    }
                    else
                    {
                        coursePortfolio.Add(course);
                    }
                }

                // Per Zufall wählen
                if (coursePortfolio.Any())
                {
                    coursePortfolio.Shuffle();

                    for (var i = 1; i <= selectionCount; i++)
                    {
                        var course = coursePortfolio[i - 1];
                        var subscription =
                            course.Occurrence.Subscriptions.FirstOrDefault(x => x.UserId.Equals(user.Id));
                        if (subscription == null)
                        {
                            // Eintragung anlegen
                            subscription = new OccurrenceSubscription();
                            subscription.OnWaitingList = true;
                            subscription.UserId = user.Id;
                            subscription.TimeStamp = DateTime.Now;
                            subscription.Occurrence = course.Occurrence;

                            Db.Subscriptions.Add(subscription);
                        }

                        subscription.Priority = i; // Punkteumrechnung kommt später
                    }
                }
            }


            Db.SaveChanges();

            return RedirectToAction("TestRun", new { id = lottery.Id });

        }

        public ActionResult TestRun(Guid id)
        {
            var model = new DrawingService(Db, id);

            model.InitLotPots();

            var org = GetMyOrganisation();

            var userDb = new ApplicationDbContext();
            var userName = $"{org.ShortName}.test.stud";

            var users = userDb.Users.Where(x => x.UserName.StartsWith(userName)).ToList();

            // jeden testuser durchgehen
            var students = new List<Student>();
            foreach (var user in users)
            {
                var student = Db.Students.Where(x => x.UserId.Equals(user.Id)).OrderByDescending(x => x.Created)
                    .FirstOrDefault();
                if (student != null)
                {
                    students.Add(student);
                }
            }
            ViewBag.Students = students;

            return View(model);
        }




        public ActionResult DrawLotPots(Guid id)
        {
            var drawing = new LotteryDrawing();
            drawing.Start = DateTime.Now;

            var model = new DrawingService(Db, id);
            drawing.Lottery = model.Lottery;

            logger.InfoFormat("Testlauf Lotterie {0} Verteilung gestartet", drawing.Lottery.Name);

            model.InitLotPots();
            ViewBag.Rounds = model.ExecuteDrawing();

            logger.InfoFormat("Testlauf Lotterie {0} Verteilung beendet", drawing.Lottery.Name);

            // Mailversand
            drawing.End = DateTime.Now;

            if (Request.IsLocal)
            {
                logger.InfoFormat("Testlauf Lotterie {0} Mailversand gestartet", drawing.Lottery.Name);
                var mailService = new LotteryMailService(model);
                mailService.SendDrawingMails(drawing);
                logger.InfoFormat("Testlauf Lotterie {0} Mailversand beendet", drawing.Lottery.Name);

            }

            return View("TestRun", model);
        }


        public ActionResult ResetTest(Guid id)
        {
            var lotteryService = new LotteryService(Db, id);
            var subscriptionService = new SubscriptionService(Db);

            var lottery = lotteryService.GetLottery();
            var courses = lotteryService.GetLotteryCourseList();

            foreach (var course in courses)
            {
                var subscriptions = course.Occurrence.Subscriptions.ToList();
                foreach (var subscription in subscriptions)
                {
                    if (subscription.Priority == 0)
                    {
                        subscriptionService.DeleteSubscription(subscription);
                    }
                    else
                    {
                        subscription.OnWaitingList = true;
                    }
                }
            }

            Db.SaveChanges();

            return RedirectToAction("TestRun", new { id = lottery.Id });
        }


        public ActionResult ClearTest(Guid id)
        {
            var lotteryService = new LotteryService(Db, id);
            var subscriptionService = new SubscriptionService(Db);

            var lottery = lotteryService.GetLottery();
            var courses = lotteryService.GetLotteryCourseList();

            foreach (var course in courses)
            {
                var subscriptions = course.Occurrence.Subscriptions.ToList();
                foreach (var subscription in subscriptions)
                {
                    subscriptionService.DeleteSubscription(subscription);
                }
            }

            foreach (var lotteryGame in lottery.Games.ToList())
            {
                Db.LotteryGames.Remove(lotteryGame);
            }
            Db.SaveChanges();

            return RedirectToAction("TestRun", new { id = lottery.Id });
        }

    }
}