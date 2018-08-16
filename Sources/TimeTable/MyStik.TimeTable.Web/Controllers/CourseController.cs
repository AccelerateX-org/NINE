using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using log4net;
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
        /// Administration einer Lehrveranstaltung
        /// </summary>
        /// <param name="id">CourseId</param>
        /// <returns></returns>
        public ActionResult Admin(Guid id)
        {
            var course = Db.Activities.OfType<Course>().SingleOrDefault(c => c.Id == id);
            if (course == null)
                return RedirectToAction("MissingCourse", "Error");

            // nur Hosts und Admin dürfen die Seite aufrufen
            var userRights = GetUserRight(User.Identity.Name, course);
            if (!(userRights.IsHost || userRights.IsCourseAdmin || userRights.IsOwner))
            {
                return RedirectToAction("Details", new {id = id});
            }

            if (course.IsInternal && !userRights.IsCourseAdmin)
            {
                return RedirectToAction("Details", new { id = id });
            }


            ViewBag.UserRight = userRights;

            if (string.IsNullOrEmpty(course.Name))
            {
                course.Name = "N.N.";
                Db.SaveChanges();
            }

            if (string.IsNullOrEmpty(course.ShortName))
            {
                course.ShortName = "N.N.";
                Db.SaveChanges();
            }


            // Alle Services, die benötigt werden
            var courseService = new CourseService(Db);
            var courseSummaryService = new CourseSummaryService(Db);

            // Termin diese Woche
            // Termine nächste Woche
            // nächster Termin

            var model = new CourseDetailViewModel()
            {
                Course = course,
                Summary = courseSummaryService.GetCourseSummary(id),
                NextDate = courseService.GetNextDate(course),
                DatesThisWeek = courseService.GetDatesThisWeek(course),
                DatesNextWeek = courseService.GetDatesNextWeek(course),
            };


            return View("AdminNew", model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Details(Guid id)
        {
            var courseSummaryService = new CourseSummaryService(Db);

            var model = courseSummaryService.GetCourseSummary(id);

            var userRights = GetUserRight(User.Identity.Name, model.Course);
            ViewBag.UserRight = userRights;

            return View("DetailsNew", model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult ChangeGroups(Guid id)
        {
            var course = Db.Activities.OfType<Course>().SingleOrDefault(c => c.Id == id);
            if (course == null)
                return RedirectToAction("MissingCourse", "Error");

            var org = GetMyOrganisation();
            var sem = SemesterService.GetSemester(DateTime.Today);
            var courseSummaryService = new CourseSummaryService(Db);
            var summary = courseSummaryService.GetCourseSummary(id);

            var model = new CourseCreateModel3
            {
                Course = course,
                CourseId = course.Id,
                Summary = summary, 
                Semester = sem,
                SemesterId = sem.Id,
                OrganiserId = org.Id
            };


            var acticeorgs =
                Db.Organisers.Where(
                    x => x.IsFaculty && x.Activities.Any(a =>
                             a.SemesterGroups.Any(s => s.Semester.EndCourses >= DateTime.Today))).ToList();

            ViewBag.Faculties = acticeorgs.Select(c => new SelectListItem
            {
                Text = c.ShortName,
                Value = c.Id.ToString(),
            });

            // Alle Semester, die in Zukunft enden
            ViewBag.Semesters = Db.Semesters
                .Where(x => x.EndCourses >= DateTime.Today && x.Groups.Any())
                .OrderByDescending(x => x.EndCourses)
                .Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString()

                });

            // bei der ersten Anzeige wird kein onChange ausgelöst
            var currList = Db.Curricula.Where(c => c.Organiser.Id == org.Id).ToList();
            var curr = currList.FirstOrDefault();
            model.CurriculumId = curr != null ? curr.Id : Guid.Empty;
            ViewBag.Curricula = currList.Select(c => new SelectListItem
            {
                Text = c.ShortName,
                Value = c.Id.ToString(),
            });

            var semGroups = Db.SemesterGroups.Where(g => g.Semester.Id == sem.Id &&
                                                         g.CapacityGroup.CurriculumGroup.Curriculum.Id == curr.Id)
                .ToList();

            var group = semGroups.FirstOrDefault();
            model.GroupId = group != null ? group.Id : Guid.Empty;
            ViewBag.Groups = semGroups.Select(c => new SelectListItem
            {
                Text = c.GroupName,
                Value = c.Id.ToString(),
            });


            return View(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ChangeGroups(CourseCreateModel3 model)
        {
            var course = Db.Activities.OfType<Course>().SingleOrDefault(c => c.Id == model.CourseId);
            if (model.GroupIds == null)
                return RedirectToAction("Admin", new { id = course.Id });


            // die aktuell vorhandenen Gruppen
            var groupList = course.SemesterGroups.ToList();

            foreach (var groupId in model.GroupIds)
            {
                // ist die Gruppe bereits vorhanden?
                var group = course.SemesterGroups.FirstOrDefault(g => g.Id == groupId);
                if (group != null)
                {
                    // Gruppe bereits vorhanden, aus der Liste entfernen,
                    // sont keine weitere Aktion
                    if (groupList.Contains(group))
                    {
                        groupList.Remove(group);
                    }
                }
                else
                {
                    // neue Gruppe
                    // hinzufügen
                    var semGroup = Db.SemesterGroups.SingleOrDefault(g => g.Id == groupId);
                    if (semGroup != null)
                    {
                        course.SemesterGroups.Add(semGroup);

                        // zugehörige Occurrencegroup anlegen
                        var occGroup = new OccurrenceGroup
                        {
                            Capacity = 0,
                            FitToCurriculumOnly = true,
                            Occurrence = course.Occurrence
                        };
                        occGroup.SemesterGroups.Add(semGroup);
                        semGroup.OccurrenceGroups.Add(occGroup);
                        course.Occurrence.Groups.Add(occGroup);
                        Db.OccurrenceGroups.Add(occGroup);
                    }
                }
            }


            // in der Liste dürften jetzt nur noch die zu löschenden Gruppen sein
            foreach (var semGroup in groupList)
            {
                course.SemesterGroups.Remove(semGroup);

                // zugehörige OccurrenceGroup löschen
                var group = course.Occurrence.Groups.FirstOrDefault(g => g.SemesterGroups.Contains(semGroup));
                if (group != null)
                {
                    course.Occurrence.Groups.Remove(group);
                    Db.OccurrenceGroups.Remove(group);
                }
            }


            Db.SaveChanges();

            return RedirectToAction("Admin", new { id = course.Id });
        }


        public ActionResult ChangeTopics(Guid id)
        {
            var course = Db.Activities.OfType<Course>().SingleOrDefault(c => c.Id == id);
            if (course == null)
                return RedirectToAction("MissingCourse", "Error");

            var org = GetMyOrganisation();
            var sem = SemesterService.GetSemester(DateTime.Today);
            var courseSummaryService = new CourseSummaryService(Db);
            var summary = courseSummaryService.GetCourseSummary(id);

            var model = new CourseCreateModel3
            {
                Course = course,
                CourseId = course.Id,
                Summary = summary,
                Semester = sem,
                SemesterId = sem.Id,
                OrganiserId = org.Id
            };


            var acticeorgs =
                Db.Organisers.Where(
                    x => x.IsFaculty && x.Activities.Any(a =>
                             a.SemesterGroups.Any(s => s.Semester.EndCourses >= DateTime.Today))).ToList();

            ViewBag.Faculties = acticeorgs.Select(c => new SelectListItem
            {
                Text = c.ShortName,
                Value = c.Id.ToString(),
            });

            // Alle Semester, die in Zukunft enden
            ViewBag.Semesters = Db.Semesters
                .Where(x => x.EndCourses >= DateTime.Today && x.Groups.Any())
                .OrderByDescending(x => x.EndCourses)
                .Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString()

                });

            // bei der ersten Anzeige wird kein onChange ausgelöst
            var currList = Db.Curricula.Where(c => c.Organiser.Id == org.Id).ToList();
            var curr = currList.FirstOrDefault();
            model.CurriculumId = curr != null ? curr.Id : Guid.Empty;
            ViewBag.Curricula = currList.Select(c => new SelectListItem
            {
                Text = c.ShortName,
                Value = c.Id.ToString(),
            });

            var semGroups = Db.SemesterTopics.Where(g => g.Semester.Id == sem.Id &&
                                                         g.Topic.Chapter.Curriculum.Id == curr.Id)
                .ToList();

            var group = semGroups.FirstOrDefault();
            model.GroupId = group != null ? group.Id : Guid.Empty;
            ViewBag.Groups = semGroups.Select(c => new SelectListItem
            {
                Text = c.TopicName,
                Value = c.Id.ToString(),
            });


            return View(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ChangeTopics(CourseCreateModel3 model)
        {
            var course = Db.Activities.OfType<Course>().SingleOrDefault(c => c.Id == model.CourseId);
            if (model.GroupIds == null)
                return RedirectToAction("Admin", new {id=course.Id});


            // die aktuell vorhandenen Gruppen
            var groupList = course.SemesterTopics.ToList();

            foreach (var groupId in model.GroupIds)
            {
                // ist die Gruppe bereits vorhanden?
                var group = groupList.FirstOrDefault(g => g.Id == groupId);
                if (group != null)
                {
                    // Gruppe bereits vorhanden, aus der Liste entfernen,
                    // sont keine weitere Aktion
                    groupList.Remove(group);
                }
                else
                {
                    // neue Gruppe
                    // hinzufügen
                    var semGroup = Db.SemesterTopics.SingleOrDefault(g => g.Id == groupId);
                    if (semGroup != null)
                    {
                        course.SemesterTopics.Add(semGroup);
                    }
                }
            }


            // in der Liste dürften jetzt nur noch die zu löschenden Gruppen sein
            foreach (var semGroup in groupList)
            {
                course.SemesterTopics.Remove(semGroup);
            }

            Db.SaveChanges();

            return RedirectToAction("Admin", new { id = course.Id });
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Index(Guid? id)
        {
            if (id.HasValue)
                return RedirectToAction("Details", new {id = id});

            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpPost]
        public PartialViewResult RemoveSubscription(Guid id, string userId)
        {
            var logger = LogManager.GetLogger("Course");

            var occurrence = Db.Occurrences.SingleOrDefault(oc => oc.Id == id);

            if (occurrence != null && !string.IsNullOrEmpty(userId))
            {
                var subscription =
                    occurrence.Subscriptions.FirstOrDefault(s => s.UserId.Equals(userId));

                if (subscription != null)
                {
                    var allDrawings = Db.SubscriptionDrawings.Where(x => x.Subscription.Id == subscription.Id).ToList();
                    foreach (var drawing in allDrawings)
                    {
                        Db.SubscriptionDrawings.Remove(drawing);
                    }

                    var allBets = Db.LotteriyBets.Where(x => x.Subscription.Id == subscription.Id).ToList();
                    foreach (var lotteryBet in allBets)
                    {
                        Db.LotteriyBets.Remove(lotteryBet);
                    }



                    occurrence.Subscriptions.Remove(subscription);
                    Db.Subscriptions.Remove(subscription);
                    Db.SaveChanges();

                    var summary = ActivityService.GetSummary(id);
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

            }
            else
            {
                logger.ErrorFormat("Occurrence or user missing [{0}], [{1}]", id, userId);
            }


            return PartialView("_RemoveSubscription");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="occurrenceId"></param>
        /// <returns></returns>
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
                    var allDrawings = Db.SubscriptionDrawings.Where(x => x.Subscription.Id == subscription.Id).ToList();
                    foreach (var drawing in allDrawings)
                    {
                        Db.SubscriptionDrawings.Remove(drawing);
                    }

                    var allBets = Db.LotteriyBets.Where(x => x.Subscription.Id == subscription.Id).ToList();
                    foreach (var lotteryBet in allBets)
                    {
                        Db.LotteriyBets.Remove(lotteryBet);
                    }

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
                }

                Db.SaveChanges();

                // Die Subscription stecken in der Activity
                var summary = ActivityService.GetSummary(model.OccurrenceId) as ActivityDateSummary;

                logger.InfoFormat("Course date {0} on {1} canceled by {2}", summary.Activity.Name, summary.Date.Begin,
                    User.Identity.Name);


                // Wenn es keine Eintragungen gibt, dann muss auch keine mail versendet werden
                if (!summary.Subscriptions.Any())
                {
                    return RedirectToAction(summary.Action, summary.Controller, new {id = summary.Id});
                }
            }


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
                }

                Db.SaveChanges();

                // Die Subscription stecken in der Activity
                var summary = ActivityService.GetSummary(model.OccurrenceId);

                logger.InfoFormat("all course dates {0} canceled by {2}", summary.Activity.Name, User.Identity.Name);

                if (!summary.Subscriptions.Any())
                {
                    return RedirectToAction(summary.Action, summary.Controller, new {id = summary.Id});
                }
            }


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
                    return RedirectToAction(summary.Action, summary.Controller, new {id = summary.Id});
                }
            }

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
            };

            ViewBag.Organiser = Db.Organisers.OrderBy(x => x.ShortName).Select(c => new SelectListItem
            {
                Text = c.ShortName,
                Value = c.Id.ToString(),
            });


            return View(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public PartialViewResult MoveDate(CourseMoveDateModel model)
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

            // Um die Anzahl der Transaktionen klein zu halten werden erst
            // an dieser Stelle alle Änderungen am Datum und 
            // dem ChangeObjekt gespeichert
            Db.SaveChanges();

            // Jetzt erst die Notification auslösen
            if (changeObject != null)
            {
                NotificationService nservice = new NotificationService();
                nservice.CreateSingleNotification(changeObject.Id.ToString());
            }
            return PartialView("_SaveSuccess");
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
            foreach (var changeObject in changes)
            {
                NotificationService nservice = new NotificationService();
                nservice.CreateSingleNotification(changeObject.Id.ToString());
            }

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


            var model = new {HasConflicts = hasConflicts};

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
            var model = new RoomListStateModel() {ActivityDateId = activityDateId};

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
                    model.RoomStates.Add(new RoomStateModel {Room = room, Conflicts = conflicts});
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
            var model = new LecturerListStateModel() {ActivityDateId = activityDateId};

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
                    model.LecturerStates.Add(new LecturerStateModel {Lecturer = lecturer, Conflicts = conflicts});
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

            var actSummary = new ActivitySummary {Activity = date.Activity};

            occ.Subscriptions.ForEach(s => Db.Subscriptions.Remove(s));
            Db.Occurrences.Remove(occ);
            date.Occurrence = null;


            date.Activity.Dates.Remove(date);
            date.Hosts.Clear();
            date.Rooms.Clear();
            Db.ActivityDates.Remove(date);

            Db.SaveChanges();


            // Kehre zurück zur Seite der Aktivität
            return RedirectToAction(actSummary.Action, actSummary.Controller, new {id = actSummary.Id});
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult DeleteAllDates(Guid id)
        {
            var course = Db.Activities.OfType<Course>().SingleOrDefault(c => c.Id == id);

            var model = new CourseDeleteModel {Course = course};

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

            var actSummary = new ActivitySummary {Activity = course};

            if (course != null)
                new CourseService(Db).DeleteAllDates(course.Id);

            // Kehre zurück zur Seite der Aktivität
            return RedirectToAction(actSummary.Action, actSummary.Controller, new {id = actSummary.Id});
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult DeleteCourse(Guid id)
        {
            var course = Db.Activities.OfType<Course>().SingleOrDefault(c => c.Id == id);

            var model = new CourseDeleteModel {Course = course};

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

            var model = new CourseDeleteModel {Course = course};

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

            return RedirectToAction(summary.Action, summary.Controller, new {id = summary.Id});
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
                    var isWdh = bool.Parse(elems[3]);


                    ICollection<DateTime> dayList;
                    // wenn Wiederholung, dann muss auch ein Enddatum angegeben sein
                    // sonst nimm nur den Einzeltag
                    if (isWdh && !string.IsNullOrEmpty(elems[4]))
                    {
                        var lastDay = DateTime.Parse(elems[4]);
                        var frequency = int.Parse(elems[5]);
                        dayList = semesterService.GetDays(day, lastDay, frequency);
                    }
                    else
                    {
                        dayList = new List<DateTime> {day};
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">SemesterId</param>
        /// <returns></returns>
        public ActionResult CreateCourse(Guid? id)
        {
            var sem = SemesterService.GetSemester(id);

            var org = GetMyOrganisation();

            CourseCreateModel2 model = new CourseCreateModel2();

            model.SemesterId = sem.Id;
            model.OrganiserId = org.Id;
            model.OrganiserId2 = org.Id;
            model.OrganiserId3 = org.Id;

            // Liste aller Fakultäten
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

            // Alle Semester, die in Zukunft enden und Semestergruppen haben
            ViewBag.Semester = Db.Semesters
                .Where(x => x.EndCourses >= DateTime.Today && x.Groups.Any())
                .OrderBy(s => s.StartCourses)
                .Select(c => new SelectListItem
                {
                    Text = c.Name,
                    Value = c.Id.ToString(),
                });
            model.SemesterId = sem.Id;

            ViewBag.Curricula = null;
            ViewBag.Groups = null;
            ViewBag.Chapters = null;
            ViewBag.Topics = null;




            // bei der ersten Anzeige wird kein onChange ausgelöst
            var currList = Db.Curricula.Where(c => c.Organiser.Id == org.Id).ToList();
            var curr = currList.FirstOrDefault();
            if (curr != null)
            {
                model.CurriculumId = curr.Id;
                ViewBag.Curricula = currList.Select(c => new SelectListItem
                {
                    Text = c.ShortName,
                    Value = c.Id.ToString(),
                });


                // alle Studiengruppen
                var currGroups = curr.CurriculumGroups.ToList();
                var group = currGroups.FirstOrDefault();
                model.CurrGroupId = @group?.Id ?? Guid.Empty;
                ViewBag.CurrGroups = currGroups.Select(c => new SelectListItem
                {
                    Text = c.Name,
                    Value = c.Id.ToString(),
                });

                // alle Kapazitätsgruppen

                if (group != null)
                {
                    var capGroups = group.CapacityGroups.ToList();
                    var firstGroup = capGroups.FirstOrDefault();
                    model.CapGroupId = firstGroup?.Id ?? Guid.Empty;

                    ViewBag.CapGroups = capGroups.Select(c => new SelectListItem
                    {
                        Text = c.Name,
                        Value = c.Id.ToString(),
                    });
                }


                var chapters = curr.Chapters.ToList();
                var chapter = chapters.FirstOrDefault();
                model.ChapterId = chapter?.Id ?? Guid.Empty;
                ViewBag.Chapters = chapters.Select(c => new SelectListItem
                {
                    Text = c.Name,
                    Value = c.Id.ToString(),
                });

                if (chapter != null)
                {
                    var topics = chapter.Topics.ToList();
                    var topic = topics.FirstOrDefault();
                    model.TopicId = topic?.Id ?? Guid.Empty;

                    ViewBag.Topics = topics.Select(c => new SelectListItem
                    {
                        Text = c.Name,
                        Value = c.Id.ToString(),
                    });
                }
            }


            return View(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult CreateCourse(CourseCreateModelExtended model)
        {
            var org = GetMyOrganisation();
            var semester = SemesterService.GetSemester(model.SemesterId);

            var course = new Course
            {
                Name = model.Name,
                ShortName = string.IsNullOrEmpty(model.ShortName) ? model.Name : model.ShortName,
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
            };

            // da kommen Kapazitätsgruppen
            // d.h. Semestergruppe suchen und ggf. anlegen
            if (model.GroupIds != null)
            {
                foreach (var groupId in model.GroupIds)
                {
                    var capGroup = Db.CapacityGroups.SingleOrDefault(g => g.Id == groupId);

                    var semGroup =
                        Db.SemesterGroups.SingleOrDefault(
                            g => g.Semester.Id == semester.Id && g.CapacityGroup.Id == groupId);

                    // die Semestergruppe gibt es nicht => anlegen
                    if (semGroup == null)
                    {
                        semGroup = new SemesterGroup
                        {
                            CapacityGroup = capGroup,
                            Semester = semester,
                            IsAvailable = false,
                        };
                        Db.SemesterGroups.Add(semGroup);
                    }
                    course.SemesterGroups.Add(semGroup);

                    var occGroup = new OccurrenceGroup
                    {
                        Capacity = 0,
                        FitToCurriculumOnly = true,
                        Occurrence = course.Occurrence
                    };
                    occGroup.SemesterGroups.Add(semGroup);
                    semGroup.OccurrenceGroups.Add(occGroup);
                    course.Occurrence.Groups.Add(occGroup);
                    Db.OccurrenceGroups.Add(occGroup);
                }
            }

            // jetzt die Themenbereiche
            if (model.TopicIds != null)
            {
                foreach (var topicId in model.TopicIds)
                {
                    var topic = Db.CurriculumTopics.SingleOrDefault(x => x.Id == topicId);

                    var semTopic =
                        Db.SemesterTopics.SingleOrDefault(
                            x => x.Semester.Id == model.SemesterId && x.Topic.Id == topicId);

                    if (semTopic == null)
                    {
                        semTopic = new SemesterTopic
                        {
                            Semester = semester,
                            Topic = topic
                        };
                        Db.SemesterTopics.Add(semTopic);
                    }

                    course.SemesterTopics.Add(semTopic);
                }
            }



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
                    var isWdh = bool.Parse(elems[3]);


                    ICollection<DateTime> dayList;
                    // wenn Wiederholung, dann muss auch ein Enddatum angegeben sein
                    // sonst nimm nur den Einzeltag
                    if (isWdh && !string.IsNullOrEmpty(elems[4]))
                    {
                        var lastDay = DateTime.Parse(elems[4]);
                        var frequency = int.Parse(elems[5]);
                        dayList = semesterService.GetDays(day, lastDay, frequency);
                    }
                    else
                    {
                        dayList = new List<DateTime> {day};
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


            Db.Activities.Add(course);
            Db.SaveChanges();

            return PartialView("_CreateCourseSuccess", course);
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

            return RedirectToAction("Index", new {id = course.Id});
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

            return RedirectToAction("Index", new {id = course.Id});
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

            return RedirectToAction("EditCourse", new {id = course.Id});
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult UploadDocument(Guid id)
        {
            var model = new CourseDocumentUploadModel {CourseId = id};

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

                return RedirectToAction("Index", new {id = course.Id});
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

        /// <summary>
        /// Absage über Schaltfläche
        /// </summary>
        /// <param name="dateId"></param>
        /// <returns></returns>
        [HttpPost]
        public PartialViewResult CancelDate2(Guid dateId)
        {
            var date = Db.ActivityDates.SingleOrDefault(d => d.Id == dateId);

            // Alle Räume freigeben
            // Die Zuordnung zum Dozent bleibt aber bestehen!
            if (date != null)
            {
                var changeService = new ChangeService(Db);
                // Absage => auch Raumänderung (Raum wird freigegeben)
                var changeObject = changeService.CreateActivityDateStateChange(date, true);
                if (changeObject != null)
                {
                    changeObject.UserId = AppUser.Id;
                    Db.DateChanges.Add(changeObject);
                }


                date.Occurrence.IsCanceled = true;
                date.Rooms.Clear();

                Db.SaveChanges();

                NotificationService nservice = new NotificationService();
                nservice.CreateSingleNotification(changeObject.Id.ToString());

            }

            var course = date.Activity as Course;
            var userRights = GetUserRight(User.Identity.Name, course);
            ViewBag.UserRight = userRights;


            return PartialView("_DateTable", course);
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

                        NotificationService nservice = new NotificationService();
                        nservice.CreateSingleNotification(changeObject.Id.ToString());
                    }

                    date.Occurrence.IsCanceled = true;
                    date.Rooms.Clear();

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
        [HttpPost]
        public ActionResult ReactivateDate2(Guid dateId)
        {
            var date = Db.ActivityDates.SingleOrDefault(d => d.Id == dateId);

            if (date != null)
            {
                var changeService = new ChangeService(Db);

                // Reaktivierung => KEINE Raumänderung
                var changeObject = changeService.CreateActivityDateStateChange(date, false);
                if (changeObject != null)
                {
                    changeObject.UserId = AppUser.Id;
                    Db.DateChanges.Add(changeObject);

                    Db.SaveChanges();

                    NotificationService nservice = new NotificationService();
                    nservice.CreateSingleNotification(changeObject.Id.ToString());
                }


                date.Occurrence.IsCanceled = false;

                Db.SaveChanges();

            }

            var course = date.Activity as Course;
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
                    // Reaktivierung => KEINE Raumänderung
                    var changeObject = changeService.CreateActivityDateStateChange(date, false);
                    if (changeObject != null)
                    {
                        changeObject.UserId = AppUser.Id;
                        Db.DateChanges.Add(changeObject);

                        Db.SaveChanges();

                        NotificationService nservice = new NotificationService();
                        nservice.CreateSingleNotification(changeObject.Id.ToString());
                    }


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
        [HttpPost]
        public PartialViewResult DeleteDateConfirm(Guid dateId)
        {
            var date = Db.ActivityDates.SingleOrDefault(d => d.Id == dateId);
            return PartialView("_DlgConfirmDeleteDate", date);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dateId"></param>
        /// <returns></returns>
        [HttpPost]
        public PartialViewResult DeleteDateConfirmed(Guid dateId)
        {
            var date = Db.ActivityDates.SingleOrDefault(d => d.Id == dateId);
            var activity = date.Activity as Course;

            DeleteService.DeleteActivityDate(dateId);

            var userRights = GetUserRight(User.Identity.Name, activity);
            ViewBag.UserRight = userRights;


            return PartialView("_DateTable", activity);
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
                    // Raumänderung
                    var changeObject = changeService.CreateActivityDateRoomChange(date);
                    if (changeObject != null)
                    {
                        changeObject.UserId = AppUser.Id;
                        Db.DateChanges.Add(changeObject);

                        Db.SaveChanges();

                        NotificationService nservice = new NotificationService();
                        nservice.CreateSingleNotification(changeObject.Id.ToString());
                    }

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
            var changeService = new ChangeService(Db);

            foreach (var dateId in dateIds)
            {
                var date = Db.ActivityDates.SingleOrDefault(d => d.Id == dateId);

                if (date != null)
                {
                    if (room != null)
                    {
                        // Raumänderung
                        var changeObject = changeService.CreateActivityDateRoomChange(date);
                        if (changeObject != null)
                        {
                            changeObject.UserId = AppUser.Id;
                            Db.DateChanges.Add(changeObject);

                            Db.SaveChanges();

                            NotificationService nservice = new NotificationService();
                            nservice.CreateSingleNotification(changeObject.Id.ToString());

                        }


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

        [HttpPost]
        public PartialViewResult ChangeDateInformation(Guid dateId)
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

            return PartialView("_DlgEditInformation", model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public PartialViewResult ChangeDateInformationConfirmed(CourseDateInformationModel model)
        {
            var date = Db.ActivityDates.SingleOrDefault(d => d.Id == model.DateId);
            var course = date.Activity as Course;

            date.Title = model.Title;
            date.Description = model.DateDescription;
            date.Occurrence.Information = model.ShortInfo;

            Db.SaveChanges();

            var userRights = GetUserRight(User.Identity.Name, course);
            ViewBag.UserRight = userRights;

            return PartialView("_DateTable", course);
        }




        public ActionResult Lock(Guid id)
        {
            var occurrence = Db.Occurrences.SingleOrDefault(oc => oc.Id == id);
            occurrence.IsAvailable = false;
            Db.SaveChanges();

            var course = Db.Activities.OfType<Course>().SingleOrDefault(c => c.Occurrence.Id == id);

            return RedirectToAction("Admin", new {id = course.Id});
        }


        public ActionResult UnLock(Guid id)
        {
            var occurrence = Db.Occurrences.SingleOrDefault(oc => oc.Id == id);
            occurrence.IsAvailable = true;
            Db.SaveChanges();

            var course = Db.Activities.OfType<Course>().SingleOrDefault(c => c.Occurrence.Id == id);

            return RedirectToAction("Admin", new {id = course.Id});
        }

        public ActionResult SubscriptionList(Guid id)
        {
            var model = Db.Activities.OfType<Course>().SingleOrDefault(x => x.Id == id);
            return View(model);
        }

        public ActionResult SubscriptionList(Course c, bool withPart, bool withWaiting, bool withDates)
        {
            var model = Db.Activities.OfType<Course>().SingleOrDefault(x => x.Id == c.Id);
            return View(model);
        }

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

        public ActionResult DeleteOwner(Guid id)
        {
            var owner = Db.ActivityOwners.SingleOrDefault(x => x.Id == id);

            var activity = owner.Activity;

            activity.Owners.Remove(owner);
            Db.ActivityOwners.Remove(owner);
            Db.SaveChanges();

            return RedirectToAction("Owners", new {id = activity.Id});

        }

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

        public ActionResult Download(Guid id)
        {
            var course = Db.Activities.OfType<Course>().SingleOrDefault(c => c.Id == id);
            var org = GetMyOrganisation();

            var courseService = new CourseService(Db);
            var courseSummaryService = new CourseSummaryService(Db);


            var model = new CourseDetailViewModel()
            {
                Course = course,
                Summary = courseSummaryService.GetCourseSummary(id),
            };


            var userRights = GetUserRight(User.Identity.Name, model.Course);
            ViewBag.UserRight = userRights;

            return View(model);
        }

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
                        writer.Write(Environment.NewLine);
                }
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

        public ActionResult AdminNewParticipients(Guid id)
        {
            var course = Db.Activities.OfType<Course>().SingleOrDefault(c => c.Id == id);
            var courseSummaryService = new CourseSummaryService(Db);

            var model = new CourseDetailViewModel()
            {
                Course = course,
                Summary = courseSummaryService.GetCourseSummary(id),
            };

            return View(model);
        }

        public PartialViewResult SubscriptionProfile(Guid id)
        {
            var studentService = new StudentService(Db);

            var subscription = Db.Subscriptions.OfType<OccurrenceSubscription>().SingleOrDefault(x => x.Id == id);
            var user = UserManager.FindById(subscription.UserId);
            var student = studentService.GetCurrentStudent(subscription.UserId);
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

            var model = new UserCoursePlanViewModel
            {
                User = user,
                Student = student,
                Subscription = subscription,
                Semester = semester
            };


            var courses =
                Db.Activities.OfType<Course>()
                    .Where(c => c.Occurrence.Subscriptions.Any(s => s.UserId.Equals(user.Id)) &&
                                c.SemesterGroups.Any((g => g.Semester.Id == semester.Id)))
                    .OrderBy(c => c.Name)
                    .ToList();

            foreach (var c in courses)
            {
                var summary = new CourseSummaryModel();

                summary.Course = c;

                var lectures =
                    Db.Members.Where(l => l.Dates.Any(occ => occ.Activity.Id == c.Id)).ToList();
                summary.Lecturers.AddRange(lectures);

                var rooms =
                    Db.Rooms.Where(l => l.Dates.Any(occ => occ.Activity.Id == c.Id)).ToList();
                summary.Rooms.AddRange(rooms);


                var days = (from occ in c.Dates
                            select
                                new
                                {
                                    Day = occ.Begin.DayOfWeek,
                                    Begin = occ.Begin.TimeOfDay,
                                    End = occ.End.TimeOfDay,
                                }).Distinct();

                foreach (var day in days)
                {
                    var defaultDay = c.Dates.FirstOrDefault(d => d.Begin.DayOfWeek == day.Day);

                    var courseDate = new CourseDateModel
                    {
                        DayOfWeek = day.Day,
                        StartTime = day.Begin,
                        EndTime = day.End,
                        DefaultDate = defaultDay.Begin
                    };
                    summary.Dates.Add(courseDate);
                }

                summary.Lottery =
                    Db.Lotteries.FirstOrDefault(x => x.Occurrences.Any(y => y.Id == course.Occurrence.Id));


                model.CourseSubscriptions.Add(new UserCourseSubscriptionViewModel
                {
                    CourseSummary = summary,
                    Subscription = subscription
                });

            }

            return PartialView("_SubscriptionProfile", model);
        }




        public ActionResult SetOnWaitingList2(Guid id, string userId)
        {
            var logger = LogManager.GetLogger("Course");

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

            // TODO: Mail schreiben


            return RedirectToAction("AdminNewParticipients", new {id = course.Id});
        }

        public ActionResult SetOnParticipiantList2(Guid id, string userId)
        {
            var logger = LogManager.GetLogger("Course");

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

            // TODO: Mail schreiben


            return RedirectToAction("AdminNewParticipients", new { id = course.Id });
        }


        public ActionResult RemoveSubscription2(Guid id, string userId)
        {
            var logger = LogManager.GetLogger("Course");

            var occurrence = Db.Occurrences.SingleOrDefault(oc => oc.Id == id);
            var course = Db.Activities.OfType<Course>().SingleOrDefault(c => c.Occurrence.Id == id);
            var host = GetCurrentUser();

            if (occurrence != null && !string.IsNullOrEmpty(userId))
            {
                var subscription =
                    occurrence.Subscriptions.FirstOrDefault(s => s.UserId.Equals(userId));

                if (subscription != null)
                {
                    var allDrawings = Db.SubscriptionDrawings.Where(x => x.Subscription.Id == subscription.Id).ToList();
                    foreach (var drawing in allDrawings)
                    {
                        Db.SubscriptionDrawings.Remove(drawing);
                    }

                    var allBets = Db.LotteriyBets.Where(x => x.Subscription.Id == subscription.Id).ToList();
                    foreach (var lotteryBet in allBets)
                    {
                        Db.LotteriyBets.Remove(lotteryBet);
                    }



                    occurrence.Subscriptions.Remove(subscription);
                    Db.Subscriptions.Remove(subscription);
                    Db.SaveChanges();

                    var mailService = new SubscriptionMailService();
                    mailService.SendSubscriptionEMail(course, userId, host);

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

        public ActionResult AdminNewInfos(Guid id)
        {
            var course = Db.Activities.OfType<Course>().SingleOrDefault(c => c.Id == id);

            var courseService = new CourseService(Db);
            var courseSummaryService = new CourseSummaryService(Db);


            var model = new CourseDetailViewModel()
            {
                Course = course,
                Summary = courseSummaryService.GetCourseSummary(id),
                Description2 = course.Description
            };



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

            return RedirectToAction("Admin", new {id = course.Id});
        }

        public ActionResult AdminNewRules(Guid id)
        {
            var course = Db.Activities.OfType<Course>().SingleOrDefault(c => c.Id == id);

            var courseService = new CourseService(Db);
            var courseSummaryService = new CourseSummaryService(Db);


            var model = new CourseDetailViewModel()
            {
                Course = course,
                Summary = courseSummaryService.GetCourseSummary(id),
            };

            if (course.Occurrence.IsCoterie)
            {
                model.optionsAccess = 3;            // red
            }
            else
            {
                if (course.Occurrence.HasHomeBias)
                {
                    model.optionsAccess = 2;        // yellow
                }
                else
                {
                    model.optionsAccess = 1;        // green
                }
            }

            if (course.Occurrence.UseGroups)
            {
                if (course.Occurrence.UseExactFit)
                {
                    model.optionsLimit = 4; // Gruppen => eine OccGroup pro Semestergruppe
                    // gibt es nicht mehr
                }
                else
                {
                    model.optionsLimit = 3; // Studiengänge => eine OccGroup pro Studiengang, d.h. alle SemGroups eines Studiengangs werden zusammengefasst
                }


            }
            else
            {
                if (course.Occurrence.Capacity <= 0)
                {
                    model.optionsLimit = 1; // keine Beschränkung
                }
                else
                {
                    model.optionsLimit = 2; // Gesamtplätze
                    model.Capacity = course.Occurrence.Capacity;
                }
            }

            return View(model);
        }

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

            return Json(new { result = "Redirect", url = Url.Action("Admin", new { id = course.Id }) });

            //return RedirectToAction("Admin", new { id = course.Id });
        }


        public ActionResult AdminNewDates(Guid id)
        {
            var course = Db.Activities.OfType<Course>().SingleOrDefault(c => c.Id == id);
            var org = GetMyOrganisation();

            var courseService = new CourseService(Db);
            var courseSummaryService = new CourseSummaryService(Db);


            var model = new CourseDetailViewModel()
            {
                Course = course,
                Summary = courseSummaryService.GetCourseSummary(id),
                Organiser = org
            };


            var userRights = GetUserRight(User.Identity.Name, model.Course);
            ViewBag.UserRight = userRights;

            return View(model);
        }


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



            return RedirectToAction("AdminNewInfos", new {id = course.Id});
        }


        public ActionResult AdminNewModule(Guid id)
        {
            var course = Db.Activities.OfType<Course>().SingleOrDefault(c => c.Id == id);
            var courseSummaryService = new CourseSummaryService(Db);

            var model = new CourseDetailViewModel()
            {
                Course = course,
                Summary = courseSummaryService.GetCourseSummary(id),
            };

            // ein Modul finden
            if (course.Nexus.Any())
            {
                model.Module = course.Nexus.First().Requirement;
            }


            return View(model);
        }
    }
}