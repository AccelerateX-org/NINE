using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Web.Mvc;
using Hangfire;
using log4net;
using log4net.Core;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using MyStik.TimeTable.Data;
using MyStik.TimeTable.DataServices;
using MyStik.TimeTable.DataServices.Booking;
using MyStik.TimeTable.DataServices.Booking.Data;
using MyStik.TimeTable.DataServices.Lottery;
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
            var user = GetCurrentUser();
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


            if (user.MemberState != MemberState.Staff)
                return View("SimpleList", alLotteries.OrderBy(x => x.Name).ToList());


            var nextSemester = SemesterService.GetNextSemester(semester);
            if (nextSemester != null && nextSemester.Groups.Any())
            {
                ViewBag.NextSemester = nextSemester;
            }

            ViewBag.PreviousSemester = SemesterService.GetPreviousSemester(semester);


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

            var me = GetMyMembership();

            var semester = SemesterService.GetSemester(model.SemesterId);

            var lottery = new Lottery
            {
                Name = model.Name,
                Description = model.Description,
                DrawingFrequency = DrawingFrequency.Daily,
                IsScheduled = false,
                IsFixed = false,
                IsActiveFrom = DateTime.Today,
                IsActiveUntil = DateTime.Today,
                FirstDrawing = DateTime.Today,
                LastDrawing = DateTime.Today,
                DrawingTime = TimeSpan.Zero,
                MinSubscription = 1,
                MaxSubscription = 1,
                MaxConfirm = 1,
                MaxExceptionConfirm = 1,
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
        public ActionResult EditGeneral(Guid id)
        {
            var lottery = Db.Lotteries.SingleOrDefault(l => l.Id == id);


            var model = new LotteryCreateModel
            {
                Lottery = lottery,
                LotteryId = lottery.Id,
                Name = lottery.Name,
                Description = lottery.Description,
            };

            return View(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult EditGeneral(LotteryCreateModel model)
        {
            var lottery = Db.Lotteries.SingleOrDefault(l => l.Id == model.LotteryId);


            if (!string.IsNullOrEmpty(model.Name))
            {
                lottery.Name = model.Name;
            }

            lottery.Description = model.Description;


            /*
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
            */
            
            Db.SaveChanges();
            logger.InfoFormat("Einstellungen zu Lotterie {0} verändert", lottery.Name);


            return RedirectToAction("Details", new {id = lottery.Id});
        }


        public ActionResult EditProcess(Guid id)
        {
            var lottery = Db.Lotteries.SingleOrDefault(l => l.Id == id);


            var model = new LotteryCreateModel
            {
                Lottery = lottery,
                LotteryId = lottery.Id,
                Name = lottery.Name,
                IsAvailableFrom = lottery.IsActiveFrom.Value.ToShortDateString(),
                IsAvailableUntil = lottery.IsActiveUntil.Value.ToShortDateString(),
                IsAvailableFromTime = lottery.IsActiveFrom.Value.TimeOfDay.ToString(),
                IsAvailableUntilTime = lottery.IsActiveUntil.Value.TimeOfDay.ToString(),
                FirstDrawing = lottery.FirstDrawing.ToShortDateString(),
                LastDrawing = lottery.LastDrawing.ToShortDateString(),
                FirstDrawingTime = lottery.FirstDrawing.TimeOfDay.ToString(),
                LastDrawingTime = lottery.LastDrawing.TimeOfDay.ToString(),
                MaxConfirm = lottery.MaxConfirm,
                MaxConfirmException = lottery.MaxExceptionConfirm
            };

            return View(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult EditProcess(LotteryCreateModel model)
        {
            var lottery = Db.Lotteries.SingleOrDefault(l => l.Id == model.LotteryId);

            var time = DateTime.Parse(model.FirstDrawing);
            var time2 = DateTime.Parse(model.LastDrawing);
            var time3 = DateTime.Parse(model.IsAvailableFrom);
            var time4 = DateTime.Parse(model.IsAvailableUntil);
            lottery.IsActiveFrom = new DateTime?(time3.Add(TimeSpan.Parse(model.IsAvailableFromTime)));
            lottery.IsActiveUntil = new DateTime?(time4.Add(TimeSpan.Parse(model.IsAvailableUntilTime)));
            lottery.FirstDrawing = time.Add(TimeSpan.Parse(model.FirstDrawingTime));
            lottery.LastDrawing = time2.Add(TimeSpan.Parse(model.LastDrawingTime));
            lottery.MaxConfirm = model.MaxConfirm;
            lottery.MaxExceptionConfirm = model.MaxConfirmException;

            Db.SaveChanges();
            logger.InfoFormat("Einstellungen zu Lotterie {0} verändert", lottery.Name);

            return RedirectToAction("Details", new { id = lottery.Id });
        }


        public ActionResult EditSubscriptionPeriod(Guid id)
        {
            var lottery = Db.Lotteries.SingleOrDefault(l => l.Id == id);


            var model = new LotteryCreateModel
            {
                Lottery = lottery,
                LotteryId = lottery.Id,
                Name = lottery.Name
            };

            model.Description = lottery.Description;
            model.IsAvailableFrom = lottery.IsActiveFrom.Value.ToShortDateString();
            model.IsAvailableUntil = lottery.IsActiveUntil.Value.ToShortDateString();
            model.IsAvailableFromTime = lottery.IsActiveFrom.Value.TimeOfDay.ToString();
            model.IsAvailableUntilTime = lottery.IsActiveUntil.Value.TimeOfDay.ToString();
            model.MinSubscription = lottery.MinSubscription;
            model.MaxSubscription = lottery.MaxSubscription;
            model.ProcessType = lottery.IsFixed ? 2 : 1;
            model.AllowManualSubscription = lottery.AllowManualSubscription;
            model.LoIneeded = lottery.LoINeeded;
            model.UseJinx = !lottery.IsScheduled;
            model.UseLock = lottery.UseLapCount;
            model.AllowPartTime = !lottery.blockPartTime;
            model.AllowFullTime = !lottery.blockFullTime;


            return View(model);
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult EditSubscriptionPeriod(LotteryCreateModel model)
        {
            var lottery = Db.Lotteries.SingleOrDefault(l => l.Id == model.LotteryId);

            var time = DateTime.Parse(model.IsAvailableFrom);
            lottery.IsActiveFrom = new DateTime?(time.Add(TimeSpan.Parse(model.IsAvailableFromTime)));
            lottery.MaxSubscription = model.MaxSubscription;
            lottery.MinSubscription = model.MinSubscription;
            lottery.IsFixed = model.ProcessType != 1;
            lottery.AllowManualSubscription = model.AllowManualSubscription;
            lottery.LoINeeded = model.LoIneeded;
            lottery.IsScheduled = !model.UseJinx;
            lottery.UseLapCount = model.UseLock;
            lottery.blockPartTime = !model.AllowPartTime;
            lottery.blockFullTime = !model.AllowFullTime;


            Db.SaveChanges();
            logger.InfoFormat("Einstellungen zu Lotterie {0} verändert", lottery.Name);


            return RedirectToAction("Details", new { id = lottery.Id });
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Details(Guid id)
        {
            var org = GetMyOrganisation();

            var service = new CourseService(Db);
            var service2 = new StudentService(Db);
            var service1 = new LotteryService(Db, id);
            var lottery = service1.GetLottery();
            if (lottery.IsActiveFrom == null)
            {
                lottery.IsActiveFrom = new DateTime?(DateTime.MaxValue);
                Db.SaveChanges();
            }
            if (lottery.IsActiveUntil == null)
            {
                lottery.IsActiveUntil = new DateTime?(DateTime.MaxValue);
                Db.SaveChanges();
            }
            var model1 = new LotteryOverviewModel();
            model1.Lottery = lottery;
            var model = model1;
            var user = GetCurrentUser();

            if (user.MemberState == MemberState.Student)
            {
                var currentStudent = service2.GetCurrentStudent(user);
                if (currentStudent != null)
                {
                    model.Student = currentStudent;
                    model.Game = lottery.Games.FirstOrDefault<LotteryGame>(x => x.UserId.Equals(user.Id));
                }
            }
            var lotteryCourseList = service1.GetLotteryCourseList();

            foreach (var course in lotteryCourseList)
            {
                var item = new LotteryOverviewCourseModel();
                item.CourseSummary = service.GetCourseSummary(course);
                model.Courses.Add(item);
            }
            foreach (var course2 in lotteryCourseList)
            {
                var subscription = course2.Occurrence.Subscriptions.FirstOrDefault(x => x.UserId.Equals(user.Id));
                if (subscription != null)
                {
                    var model4 = new LotteryOverviewCourseModel();
                    model4.CourseSummary = service.GetCourseSummary(course2);
                    model4.Subscription = subscription;
                    model.CoursesSelected.Add(model4);
                }
            }


            ViewBag.UserRight = GetUserRight(org);

            return View("DetailsNew", model);
        }





        public ActionResult DrawingPots(Guid id)
        {
            var org = GetMyOrganisation();
            var model = new DrawingService(Db, id);

            model.InitLotPots();
            ViewBag.UserRight = GetUserRight(org);

            return View(model);
        }


        public ActionResult Students(Guid id)
        {
            var model = new DrawingService(Db, id);

            model.InitLotPots();

            var org = GetMyOrganisation();
            ViewBag.UserRight = GetUserRight(org);


            return !model.Lottery.IsFixed ? View(model) : View("StudentsPrio", model);
        }


        public FileResult RawData(Guid id)
        {
            var model = new DrawingService(Db, id);

            model.InitLotPots();

            var ms = new MemoryStream();
            var writer = new StreamWriter(ms, Encoding.Default);


            // SG; Semester;Bedarf;Flexibel;Prio
            writer.Write("Studiengang;Semester;Bedarf;Flexibel");
            for (var i = 1; i <= model.Lottery.MaxSubscription; i++)
            {
                writer.Write(";{0}", i);
            }
            writer.Write(Environment.NewLine);

            foreach (var student in model.Games)
            {
                writer.Write("{0};{1};{2};{3}",
                    student.Student.Curriculum.ShortName,
                    student.Student.FirstSemester.Name,
                    student.CoursesWanted,
                    student.AcceptDefault
                );

                // egal ob schon ein Platz oder noch Warteliste
                // entscheidend ist die Prio
                for (var i = 1; i <= model.Lottery.MaxSubscription; i++)
                {
                    var seat = student.Seats.FirstOrDefault(x => x.Priority == i);
                    var lot = student.Lots.FirstOrDefault(x => x.Priority == i);

                    var cName = "";
                    if (seat != null)
                        cName = seat.Course.ShortName;
                    if (lot != null)
                        cName = lot.Course.ShortName;

                    writer.Write(";{0}", cName);
                }
                writer.Write(Environment.NewLine);
            }


            writer.Flush();
            writer.Dispose();

            var sb = new StringBuilder();
            sb.Append("Wahl_");
            sb.Append(model.Lottery.Name);
            sb.Append("_");
            sb.Append(DateTime.Now.ToString("yyyyMMdd_HHmmss"));
            sb.Append(".csv");

            return File(ms.GetBuffer(), "text/csv", sb.ToString());
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Clearance(Guid id)
        {
            var model = Db.Lotteries.SingleOrDefault(l => l.Id == id);

            return View(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult ClearanceConfirmed(Guid id)
        {
            var model = new DrawingService(Db, id);

            var drawing = new LotteryDrawing();
            drawing.Start = DateTime.Now;

            var subService = new SubscriptionService(Db);

            foreach (var course in model.Courses)
            {
                foreach (var subscription in course.Occurrence.Subscriptions.ToList())
                {
                    // nur die Eintragungen ohne Priorität
                    if (!subscription.Priority.HasValue)
                    {

                        var mailService = new LotteryMailService(model);
                        mailService.SendLotteryRemoveMail(drawing, GetMyMembership(), model.Lottery, course, subscription);

                        subService.DeleteSubscription(subscription);
                    }
                }
            }

            // Am Ende eine Mail an den Ausführenden senden, wer alles informiert wurde



            return RedirectToAction("Details", new { id = id });
        }


        public ActionResult ClearanceTotalConfirmed(Guid id)
        {
            var model = new DrawingService(Db, id);

            var drawing = new LotteryDrawing();
            drawing.Start = DateTime.Now;

            var subService = new SubscriptionService(Db);

            foreach (var course in model.Courses)
            {
                foreach (var subscription in course.Occurrence.Subscriptions.ToList())
                {
                    var mailService = new LotteryMailService(model);
                    mailService.SendLotteryRemoveMail(drawing, GetMyMembership(), model.Lottery, course, subscription);

                    subService.DeleteSubscription(subscription);
                }
            }

            // Am Ende eine Mail an den Ausführenden senden, wer alles informiert wurde



            return RedirectToAction("Details", new { id = id });
        }





        /// <summary>
        /// Confirmation for reset of lottery
        /// </summary>
        /// <param name="id">id of lottery</param>
        /// <returns></returns>
        public ActionResult Reset(Guid id)
        {
            var model = Db.Lotteries.SingleOrDefault(l => l.Id == id);

            return View(model);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult ResetConfirmed(Guid id)
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
                    sub.HostRemark = string.Empty;

                    if (lottery.MaxConfirm == 0 && lottery.MaxExceptionConfirm == 0)
                    {
                        sub.Priority = 1;
                    }

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
                var courseSummary = courseService.GetCourseSummary(actSummary.Activity as Course);

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

            // Den Bericht anlegen
            var userService = new UserInfoService();
            var sb = new StringBuilder();

            sb.AppendLine("<table class=\"table table-condensed\">");
            sb.AppendLine("<thead>");
            sb.AppendLine("<tr>");
            sb.AppendLine("<th>Datum</th>");
            sb.AppendLine("<th>LV</th>");
            sb.AppendLine("<th>StudentIn</th>");
            sb.AppendLine("<th>Prio</th>");
            sb.AppendLine("<th>Bemerkung</th>");
            sb.AppendLine("</tr>");
            sb.AppendLine("</thead>");
            sb.AppendLine("<tbody>");

            foreach (var message in model.Messages)
            {
                sb.AppendLine("<tr>");
                sb.AppendLine($"<td>{message.TimeStamp}</td>");

                if (message.Course != null)
                {
                    sb.AppendLine($"<td>{message.Course.ShortName}</td>");
                }
                else
                {
                    sb.AppendLine("<td></td>");
                }


                if (!string.IsNullOrEmpty(message.UserId))
                {
                    var user = userService.GetUser(message.UserId);
                    if (user != null)
                    {
                        sb.AppendLine($"<td>{user.FullName}</td>");
                    }
                    else
                    {
                        sb.AppendLine("<td>unbekannt</td>");
                    }
                }
                else
                {
                    sb.AppendLine("<td></td>");
                }

                if (message.Subscription != null)
                {
                    sb.AppendLine($"<td>{message.Subscription.Priority}</td>");
                }
                else
                {
                    sb.AppendLine("<td></td>");
                }


                sb.AppendLine($"<td>{message.Remark}</td>");
                sb.AppendLine("</tr>");

            }
            sb.AppendLine("</tbody>");
            sb.AppendLine("</table>");


            drawing.Message = sb.ToString();

            // Mailversand
            drawing.End = DateTime.Now;
            Db.LotteryDrawings.Add(drawing);

            Db.SaveChanges();


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
                    (a.Name.Contains(searchText) || a.ShortName.Contains(searchText)) &&
                    a.SemesterGroups.Any(s => s.Semester.Id == sem.Id && s.CapacityGroup.CurriculumGroup.Curriculum.Organiser.Id == org.Id))
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
            var org = GetMyOrganisation();
            ViewBag.UserRights = GetUserRight(org);


            var service1 = new CourseService(Db);
            var service2 = new StudentService(Db);
            var lottery = new LotteryService(Db, id).GetLottery();
            var model1 = new LotteryOverviewModel();
            model1.Lottery = lottery;
            var model = model1;
            var service = new DrawingService(Db, id);
            service.InitLotPots();
            model.DrawingService = service;


            return View("DrawingNew", model);
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


        public ActionResult Delete(Guid id)
        {
            var model = Db.Lotteries.SingleOrDefault(l => l.Id == id);

            return View(model);
        }


        /// <summary>
            /// 
            /// </summary>
            /// <param name="id"></param>
            /// <returns></returns>
            public ActionResult DeleteConfirmed(Guid id)
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

            var model = new WpmSubscriptionMasterModel();
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
            return RedirectToAction("Overview", new {id = id});
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="studId"></param>
        /// <returns></returns>
        public ActionResult Overview(Guid id)
        {
            var courseService = new CourseService(Db);
            var lotteryService = new LotteryService(Db, id);
            var studentService = new StudentService(Db);

            var user = GetCurrentUser();

            if (user.MemberState == MemberState.Guest)
                return View("ForStudentsOnly");


            var student = studentService.GetCurrentStudent(user);
            if (student == null)
                return View("ForStudentsOnly");

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
            var service2 = new SubscriptionService(Db);

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
                var courseSummary = courseService.GetCourseSummary(course);

                if (courseSummary.Course.Dates.Any())
                {
                    var firstDate = courseSummary.Course.Dates.Min(x => x.Begin);
                    var lastDate = courseSummary.Course.Dates.Max(x => x.Begin);

                    var activities = Db.Activities.OfType<Course>().Where(a =>
                        a.Occurrence.Subscriptions.Any(u => u.UserId.Equals(user.Id)) &&
                        a.Dates.Any(d => d.Begin >= firstDate && d.End <= lastDate)).ToList();
                    courseSummary.ConflictingDates = courseService.GetConflictingDates(course, activities);
                }


                courseSummary.Subscription = service2.GetSubscription(course.Occurrence.Id, user.Id);


                var bookingLists = new BookingService(Db).GetBookingLists(course.Occurrence.Id);
                var bookingState = new BookingState
                {
                    Student = student,
                    Occurrence = course.Occurrence,
                    BookingLists = bookingLists
                };
                bookingState.Init();



                var isSelectable = true;
                var msg = new StringBuilder();

                if (student?.Curriculum == null || course.Occurrence.IsCoterie && !courseSummary.Curricula.Contains(student.Curriculum))
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
                    CourseSummary = courseSummary,
                    IsSelectable = isSelectable,
                    Message = msg.ToString(),
                    BookingState = bookingState
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
            game.CoursesWanted = confirm;           // wird im Algorithmus MaxWinners ignoriert - andernfalls sollte hier die Anzahl an gewählten Kursen angegeben werden
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

            // Reihenfolge = Priorität
            // Vergabe der Punkte über den Lapcount - keine Budgets
            var logger = LogManager.GetLogger("SubscribeActivity");

            var i = 1;
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

                // Vergabe der Prioritäten entsprechend Einstellung
                subscription.Priority = i;

                if (lottery.MaxConfirm == 0 && lottery.MaxExceptionConfirm == 0)
                {
                    // Bei Maximierung der Anzahl bekommen alle Eintragungen Prio 1
                }
                else
                {
                    // sonst immer Prioritäten vergeben
                    // Nach Reihenfolge der Nennungen
                    i++;
                }


                logger.InfoFormat("{0} ({1}) by [{2}]: for lottery {3} with prio {4}",
                    course.Course.Name, course.Course.ShortName, User.Identity.Name, lottery.Name, subscription.Priority.Value);
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
                var confirmCount = rnd.Next(lottery.MaxConfirm, lottery.MaxExceptionConfirm + 1);
                // welche Pechregel?
                var acceptAny = rnd.Next(0, 2) == 1 ? true : false;

                game.AcceptDefault = acceptAny;
                game.CoursesWanted = confirmCount;
                game.LastChange = DateTime.Now;


                // wie viele Kurse wählen?
                var selectionCount = rnd.Next(lottery.MinSubscription, lottery.MaxSubscription + 1);

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


            return View("TestRunStart", model);
        }




        public ActionResult TestDrawing(Guid id)
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

            return View("TestRunResult", model);
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

        public ActionResult Download(Guid id)
        {
            var lotteryService = new LotteryService(Db, id);
            var subscriptionService = new SubscriptionService(Db);

            var lottery = lotteryService.GetLottery();
            var courses = lotteryService.GetLotteryCourseList();


            var ms = new MemoryStream();
            var writer = new StreamWriter(ms, Encoding.Default);

            writer.Write(
                "Stud;LV;Doz;Prio;Status;Eingetragen");

            writer.Write(Environment.NewLine);
            foreach (var course in courses)
            {
                foreach (var subscription in course.Occurrence.Subscriptions)
                {
                    var user = UserManager.FindById(subscription.UserId);
                    var student = StudentService.GetCurrentStudent(user);

                    var lectures =
                        Db.Members.Where(l => l.Dates.Any(occ => occ.Activity.Id == course.Id)).ToList();



                    if (user != null)
                    {
                        var userName = user.FullName;
                        var courseName = string.IsNullOrEmpty(course.ShortName) ? course.Name : course.ShortName;
                        var lecName = lectures.FirstOrDefault() != null ? lectures.First().ShortName : $"N.N. ({courseName})";
                        var prio = subscription.Priority ?? 0;
                        var timeStamp = subscription.TimeStamp.ToString("yyyy-MM-dd HH:mm:ss");

                        var subState = subscription.OnWaitingList ? "Warteliste" : "Teilnehmer";

                        writer.Write("{0};{1};{2};{3};{4};{5}",
                            userName, courseName, lecName, prio, subState, timeStamp);
                        writer.Write(Environment.NewLine);
                    }
                }
            }

            writer.Flush();
            writer.Dispose();

            var sb = new StringBuilder();
            sb.Append("WF_");
            sb.Append(lottery.Name);
            sb.Append("_");
            sb.Append(DateTime.Today.ToString("yyyyMMdd"));
            sb.Append(".csv");

            return File(ms.GetBuffer(), "text/csv", sb.ToString());
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Repair(Guid id)
        {
            var model = new DrawingService(Db, id);
            var lottery = model.Lottery;

            /*
            model.InitLotPots();
            model.Analyse();

            


            var list = new List<OccurrenceSubscription>();

            foreach (var game in model.Games)
            {
                var surplusList = game.Seats.Where(x => x.IsSurplus).ToList();

                foreach (var lot in surplusList)
                {
                    //subscriptionService.DeleteSubscription(lot.Subscription);
                    list.Add(lot.Subscription);
                }
            }



            var deleteDB = new TimeTableDbContext();
            var subscriptionService = new SubscriptionService(deleteDB);
            foreach (var subscription in list)
            {
                var sub = deleteDB.Subscriptions.OfType<OccurrenceSubscription>()
                    .SingleOrDefault(x => x.Id == subscription.Id);
                subscriptionService.DeleteSubscription(sub);
            }

            //deleteDB.SaveChanges();


            logger.InfoFormat("Lotterie {0} repariert - alle überzählige Eintragungen gelöscht", lottery.Name);



            //return View(list);
            */

            return RedirectToAction("Students", new { id = lottery.Id });
        }

        public ActionResult ChangeSelection(Guid id)
        {
            var lottery = new LotteryService(Db, id).GetLottery();
            var model1 = new LotteryOverviewModel();
            model1.Lottery = lottery;
            var model = model1;
            return View(model);
        }

        public ActionResult ClearGames(Guid id)
        {
            var service1 = new CourseService(Db);
            var service2 = new StudentService(Db);
            var service = new SubscriptionService(Db);
            var service3 = new LotteryService(Db, id);
            var lotteryCourseList = service3.GetLotteryCourseList();
            foreach (var game in service3.GetLottery().Games.ToList<LotteryGame>())
            {
                Db.LotteryGames.Remove(game);
            }
            using (var enumerator2 = lotteryCourseList.GetEnumerator())
            {
                while (enumerator2.MoveNext())
                {
                    foreach (var subscription in enumerator2.Current.Occurrence.Subscriptions.ToList<OccurrenceSubscription>())
                    {
                        service.DeleteSubscription(subscription);
                    }
                }
            }
            Db.SaveChanges();
            return RedirectToAction("Details", new { id = id });
        }


        public ActionResult DeleteCourses(Guid id)
        {
            var myOrganisation = GetMyOrganisation();
            ViewBag.UserRight = GetUserRight(myOrganisation);

            var service = new CourseService(Db);
            var service2 = new StudentService(Db);
            var service1 = new LotteryService(Db, id);
            var lottery = service1.GetLottery();
            var model1 = new LotteryOverviewModel();
            model1.Lottery = lottery;
            var model = model1;
            var user = GetCurrentUser();
            if (user.MemberState == MemberState.Student)
            {
                var currentStudent = service2.GetCurrentStudent(user);
                if (currentStudent != null)
                {
                    model.Student = currentStudent;
                    model.Game = lottery.Games.FirstOrDefault<LotteryGame>(x => x.UserId.Equals(user.Id));
                }
            }
            var lotteryCourseList = service1.GetLotteryCourseList();
            foreach (var course in lotteryCourseList)
            {
                var item = new LotteryOverviewCourseModel();
                item.CourseSummary = service.GetCourseSummary(course);
                model.Courses.Add(item);
            }
            foreach (var course2 in lotteryCourseList)
            {
                var subscription = course2.Occurrence.Subscriptions.FirstOrDefault<OccurrenceSubscription>(x => x.UserId.Equals(user.Id));
                if (subscription != null)
                {
                    var model4 = new LotteryOverviewCourseModel();
                    model4.CourseSummary = service.GetCourseSummary(course2);
                    model4.Subscription = subscription;
                    var item = model4;
                    model.CoursesSelected.Add(item);
                }
            }
            return View(model);
        }


        public ActionResult DeleteCourse(Guid lotteryId, Guid courseId)
        {
            var lottery = new LotteryService(Db, lotteryId).GetLottery();
            var course = new CourseService(Db).GetCourse(courseId);
            var model1 = new LotteryDeleteCourseModel();
            model1.Lottery = lottery;
            model1.Course = course;
            var model = model1;
            return View(model);
        }

        [HttpPost]
        public ActionResult DeleteCourse(LotteryDeleteCourseModel model)
        {
            var user = GetCurrentUser();
            var lottery = new LotteryService(Db, model.Lottery.Id).GetLottery();
            var course = new CourseService(Db).GetCourse(model.Course.Id);
            if (course.ShortName.Equals(model.Code))
            {
                var subscription = course.Occurrence.Subscriptions.FirstOrDefault<OccurrenceSubscription>(x => x.UserId.Equals(user.Id));
                if (subscription != null)
                {
                    new SubscriptionService(Db).DeleteSubscription(subscription);
                    return RedirectToAction("Details", new { id = lottery.Id });
                }
            }
            ModelState.AddModelError("Code", "Der angegebene Code ist falsch");
            var model1 = new LotteryDeleteCourseModel();
            model1.Lottery = lottery;
            model1.Course = course;
            var model2 = model1;
            return View(model2);
        }

        public PartialViewResult GetBookingList(Guid id)
        {
            try
            {
                var bookingModel = this.GetBookingModel(id);
                return this.PartialView("_BookingList", bookingModel);
            }
            catch (Exception ex)
            {
                var model = new HandleErrorInfo(ex, "Lottery", "GetBookingList");
                return PartialView("_Error", model);
            }

        }


        private LotteryOverviewModel GetBookingModel(Guid id)
        {
            var service = new CourseService(Db);
            var service2 = new SubscriptionService(Db);
            var user = GetCurrentUser();
            var currentStudent = new StudentService(Db).GetCurrentStudent(user);
            var service1 = new LotteryService(Db, id);
            var lottery = service1.GetLottery();
            var game = lottery.Games.FirstOrDefault<LotteryGame>(x => x.UserId.Equals(user.Id));
            var model1 = new LotteryOverviewModel();
            model1.Lottery = lottery;
            model1.Student = currentStudent;
            model1.Game = game;
            var model = model1;
            foreach (var course in service1.GetLotteryCourseList())
            {
                var courseSummary = service.GetCourseSummary(course);

                if (courseSummary.Course.Dates.Any())
                {
                    var firstDate = courseSummary.Course.Dates.Min(x => x.Begin);
                    var lastDate = courseSummary.Course.Dates.Max(x => x.Begin);

                    var activities = Db.Activities.OfType<Course>().Where(a =>
                        a.Occurrence.Subscriptions.Any(u => u.UserId.Equals(user.Id)) &&
                        a.Dates.Any(d => d.Begin >= firstDate && d.End <= lastDate)).ToList();
                    courseSummary.ConflictingDates = service.GetConflictingDates(course, activities);
                }

                courseSummary.Subscription = service2.GetSubscription(course.Occurrence.Id, user.Id);
                var bookingLists = new BookingService(Db).GetBookingLists(course.Occurrence.Id);
                var state1 = new BookingState();
                state1.Student = currentStudent;
                state1.Occurrence = course.Occurrence;
                state1.BookingLists = bookingLists;
                var state = state1;
                state.Init();
                var flag = true;
                var builder = new StringBuilder();
                if ((currentStudent?.Curriculum == null) || (course.Occurrence.IsCoterie && !courseSummary.Curricula.Contains(currentStudent.Curriculum)))
                {
                    flag = false;
                    builder.AppendLine("<li><i class=\"fa fa-li fa-ban\"></i> Lehrveranstaltung steht f\x00fcr Ihren Studiengang nicht zur Verf\x00fcgung</li>");
                }
                if (!course.Occurrence.IsAvailable)
                {
                    flag = false;
                    builder.AppendLine("<li><i class=\"fa fa-li fa-lock\"></i>Lehrveranstaltung ist f\x00fcr Eintragungen gesperrt</li>");
                }
                if (state.MyBookingList != null && state.MyBookingList.FreeSeats < 1)
                {
                    flag = false;
                    builder.AppendLine("<li><i class=\"fa fa-li fa-times\"></i>Keine freien Pl\x00e4tze verf\x00fcgbar</li>");
                }
                var model4 = new LotteryOverviewCourseModel();
                model4.CourseSummary = courseSummary;
                model4.IsSelectable = flag;
                model4.Message = builder.ToString();
                model4.BookingState = state;
                model4.Subscription = courseSummary.Subscription;
                var item = model4;
                model.Courses.Add(item);
                if (courseSummary.Subscription != null)
                {
                    model.CoursesSelected.Add(item);
                }
            }
        return model;
    }


        public PartialViewResult Subscribe(Guid lotteryId, Guid courseId)
        {
            var logger = LogManager.GetLogger("Booking");
            var currentUser = GetCurrentUser();
            var ticket = new BookingService(Db).Subscribe(currentUser.Id, courseId);
            if (ticket.SucceedingSubscription != null)
            {
                new SubscriptionMailService().SendSucceedingEMail(ticket.Course, ticket.SucceedingSubscription);
                var user = GetUser(ticket.SucceedingSubscription.UserId);
                logger.InfoFormat("{0} ({1}) for [{2}]: set on participient list", ticket.Course.Name, ticket.Course.ShortName, user.UserName);
            }
            var bookingModel = this.GetBookingModel(lotteryId);
            return this.PartialView("_BookingList", bookingModel);
        }


        public ActionResult CheckConsistency(Guid id)
        {
            var userService = new UserInfoService();
            var service1 = new LotteryService(Db, id);
            var lottery = service1.GetLottery();
            var courses = service1.GetLotteryCourseList();

            foreach (var course in courses)
            {
                var userIDs = course.Occurrence.Subscriptions.Select(x => x.UserId).Distinct().ToList();

                foreach (var userID in userIDs)
                {
                    var user = userService.GetUser(userID);

                    var subscriptions = course.Occurrence.Subscriptions.Where(x => x.UserId.Equals(userID)).OrderBy(x => x.TimeStamp).ToList();

                    if (user == null)
                    {
                        foreach (var subscription in subscriptions)
                        {
                            Db.Subscriptions.Remove(subscription);
                        }
                    }
                    else
                    {
                        // alle bis auf die erste löschen
                        if (subscriptions.Count > 1)
                        {
                            foreach (var subscription in subscriptions)
                            {
                                if (subscription != subscriptions.First())
                                {
                                    Db.Subscriptions.Remove(subscription);
                                }
                            }
                        }
                    }
                }
            }

            Db.SaveChanges();
                
            return RedirectToAction("Details", new {id = id});
        }

        public ActionResult Reports(Guid id)
        {
            var service1 = new LotteryService(Db, id);
            var lottery = service1.GetLottery();

            return View(lottery);
        }


        public ActionResult DeleteReport(Guid id)
        {
            var drawing = Db.LotteryDrawings.SingleOrDefault(x => x.Id == id);

            var lottery = drawing.Lottery;

            Db.LotteryDrawings.Remove(drawing);
            Db.SaveChanges();



            return RedirectToAction("Details", new {id=lottery.Id});
        }


        public ActionResult Show(Guid id)
        {
            var lottery = Db.Lotteries.SingleOrDefault(x => x.Id == id);

            lottery.IsActive = true;

            Db.SaveChanges();

            return RedirectToAction("Details", new { id = lottery.Id });
        }

        public ActionResult Hide(Guid id)
        {
            var lottery = Db.Lotteries.SingleOrDefault(x => x.Id == id);

            lottery.IsActive = false;

            Db.SaveChanges();

            return RedirectToAction("Details", new { id = lottery.Id });
        }


        public ActionResult StatisticsOverall(Guid id)
        {
            var org = GetMyOrganisation();

            var semester = SemesterService.GetSemester(id);


            var alLotteries = Db.Lotteries.Where(x =>
                x.Semester != null && x.Semester.Id == semester.Id &&
                x.Organiser != null && x.Organiser.Id == org.Id).ToList();


            var userService = new UserInfoService();

            var model = new List<LotteryStudentStatisticsModel>();


            foreach (var lottery in alLotteries)
            {
                foreach (var occurrence in lottery.Occurrences)
                {
                    // nur die Plätze - keine Wartelisten
                    foreach (var subscription in occurrence.Subscriptions.Where(x => !x.OnWaitingList).ToList())
                    {
                        var user = userService.GetUser(subscription.UserId);
                        var student = Db.Students.Where(x => x.UserId.Equals(subscription.UserId))
                            .OrderByDescending(x => x.Created).FirstOrDefault();

                        var studentModel = model.SingleOrDefault(x => x.StudentUser.Id.Equals(subscription.UserId));

                        if (studentModel == null)
                        {

                            studentModel = new LotteryStudentStatisticsModel
                            {
                                Student = student,
                                StudentUser = user,
                                LotterySubscription = new List<LotterySubscriptionStatisticsModel>()
                            };

                            model.Add(studentModel);
                        }

                        studentModel.LotterySubscription.Add(
                            new LotterySubscriptionStatisticsModel
                            {
                                Lottery = lottery,
                                Subscription = subscription
                            });
                    }
                }
            }


            return View(model);
        }


        public ActionResult ClearanceMinMax(Guid id)
        {
            var lottery = Db.Lotteries.SingleOrDefault(x => x.Id == id);

            var model = new DrawingService(Db, id);

            model.InitLotPots();

            var min = lottery.MaxConfirm;
            var max = lottery.MaxExceptionConfirm;

            foreach (var student in model.Games)
            {
                if (student.LotteryGame.CoursesWanted > max)
                {
                    student.LotteryGame.CoursesWanted = max;

                    if (min < max)
                    {
                        student.LotteryGame.CoursesWanted = min;
                    }
                }
            }

            Db.SaveChanges();


            return RedirectToAction("Drawing", new {id = id});
        }

        public ActionResult Capacities(Guid id)
        {
            var org = GetMyOrganisation();
            var model = new DrawingService(Db, id);

            model.InitLotPots();
            ViewBag.UserRight = GetUserRight(org);

            return View(model);
        }
    }

}