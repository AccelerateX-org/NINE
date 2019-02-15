using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using log4net;
using MyStik.TimeTable.Data;
using MyStik.TimeTable.DataServices;
using MyStik.TimeTable.DataServices.Booking;
using MyStik.TimeTable.Web.Models;
using MyStik.TimeTable.Web.Services;

namespace MyStik.TimeTable.Web.Controllers
{
    public class DictionaryController : BaseController
    {
        public ActionResult Index()
        {
            var user = GetCurrentUser();

            var model = new HomeViewModel();

            // Alle Semester mit veröffentlichten Semestergruppen
            var allPublishedSemester = Db.Semesters.Where(x => x.Groups.Any()).OrderByDescending(s => s.EndCourses).Take(4).ToList();
            foreach (var semester in allPublishedSemester)
            {
                var isStaff = user.MemberState == MemberState.Staff;
                var activeOrgs = SemesterService.GetActiveOrganiser(semester, !isStaff);

                var semModel = new SemesterActiveViewModel
                {
                    Semester = semester,
                    Organisers = activeOrgs.ToList()
                };

                model.ActiveSemester.Add(semModel);
            }

            ViewBag.UserRight = GetUserRight();

            return View(model);
        }

        public ActionResult Semester(Guid semId)
        {
            var semester = SemesterService.GetSemester(semId);

            var user = GetCurrentUser();
            var isStaff = user.MemberState == MemberState.Staff;

            var curricula = SemesterService.GetActiveCurricula(semester, !isStaff).ToList();

            var orgs = curricula.GroupBy(x => x.Organiser).Select(x => x.Key).OrderBy(x => x.ShortName).ToList();

            var model = new SemesterActiveViewModel
            {
                Semester = semester,
                Organisers = orgs
            };

            return View(model);
        }

        public ActionResult Organiser(Guid semId, Guid orgId)
        {
            var semester = SemesterService.GetSemester(semId);

            var organiser = Db.Organisers.SingleOrDefault(x => x.Id == orgId);

            var model = new SemesterActiveViewModel
            {
                Semester = semester,
                Curricula = organiser.Curricula.OrderBy(x => x.Name).ToList(),
                Organiser = organiser
            };

            return View(model);
        }

        public ActionResult Curriculum(Guid semId, Guid currId)
        {
            var semester = SemesterService.GetSemester(semId);

            var curr = Db.Curricula.SingleOrDefault(x => x.Id == currId);

            var model = new SemesterActiveViewModel
            {
                Semester = semester,
                Organiser = curr.Organiser,
                Curriculum = curr
            };


            return View(model);
        }

        public ActionResult Group(Guid semId, Guid groupId)
        {
            var user = GetCurrentUser();
            var semester = SemesterService.GetSemester(semId);
            var capGroup = Db.CapacityGroups.SingleOrDefault(x => x.Id == groupId);

            var semGroup =
                Db.SemesterGroups.FirstOrDefault(x =>
                    x.CapacityGroup.Id == capGroup.Id && x.Semester.Id == semester.Id);

            if (user.MemberState != MemberState.Staff && !semGroup.IsAvailable)
            {
                return View("NotAvailable", semGroup);
            }

            var allTopics = Db.SemesterTopics
                .Where(x => x.Activities.Any(s => s.SemesterGroups.Any(g => g.Id == semGroup.Id))).ToList();

            var model = new SemesterActiveViewModel
            {
                Semester = semester,
                Organiser = capGroup.CurriculumGroup.Curriculum.Organiser,
                Curriculum = capGroup.CurriculumGroup.Curriculum,
                CapacityGroup = capGroup,
                SemesterGroup = semGroup
            };

            return View(allTopics.Any() ? "GroupByTopic" : "Group", model);
        }

        public ActionResult GroupListByTopic(Guid semId, Guid groupId)
        { 
            var semester = SemesterService.GetSemester(semId);
            var capGroup = Db.CapacityGroups.SingleOrDefault(x => x.Id == groupId);

            var model = new SemesterActiveViewModel
            {
                Semester = semester,
                Organiser = capGroup.CurriculumGroup.Curriculum.Organiser,
                Curriculum = capGroup.CurriculumGroup.Curriculum,
                CapacityGroup = capGroup,
            };

            var semGroup =
                Db.SemesterGroups.FirstOrDefault(x =>
                    x.CapacityGroup.Id == capGroup.Id && x.Semester.Id == semester.Id);

            if (semGroup == null)
                return View("CourseList", model);

            model.SemesterGroup = semGroup;

            var allTopics = Db.SemesterTopics.Where(x => x.Activities.Any(s => s.SemesterGroups.Any(g => g.Id == semGroup.Id))).ToList();


            List<Course> activities = null;

            if (Request.IsAuthenticated)
            {
                var user = GetCurrentUser();
                activities = Db.Activities.OfType<Course>().Where(a =>
                    a.SemesterGroups.Any(g => g.Semester.Id == semester.Id) &&
                    a.Occurrence.Subscriptions.Any(u => u.UserId.Equals(user.Id))).ToList();
            }

            var courseService = new CourseService(Db);

            foreach (var topic in allTopics)
            {
                var courses = topic.Activities.OfType<Course>().ToList();

                var model2 = new List<CourseSummaryModel>();


                foreach (var course in courses)
                {
                    var summary = courseService.GetCourseSummary(course);

                    if (Request.IsAuthenticated)
                    {
                        var user = GetCurrentUser();

                        var state = ActivityService.GetActivityState(course.Occurrence, user);

                        summary.User = user;
                        summary.Subscription = state.Subscription;

                        summary.Lottery =
                            Db.Lotteries.FirstOrDefault(x => x.Occurrences.Any(y => y.Id == course.Occurrence.Id));

                        // Konflikte suchen
                        foreach (var date in course.Dates)
                        {
                            var conflictingActivities = activities.Where(x => x.Dates.Any(d =>
                                    (d.End > date.Begin && d.End <= date.End) || // Veranstaltung endet im Zeitraum
                                    (d.Begin >= date.Begin && d.Begin < date.End) || // Veranstaltung beginnt im Zeitraum
                                    (d.Begin <= date.Begin && d.End >= date.End) // Veranstaltung zieht sich über gesamten Zeitraum
                            )).ToList();

                            if (conflictingActivities.Any())
                            {
                                foreach (var conflictingActivity in conflictingActivities.Where(x => x.Id != course.Id))        // nicht mit dem Vergleichen, wo selbst eingetragen
                                {
                                    summary.ConflictingDates[date] = new List<ActivityDate>();

                                    var conflictingDates = conflictingActivity.Dates.Where(d =>
                                            (d.End > date.Begin && d.End <= date.End) || // Veranstaltung endet im Zeitraum
                                            (d.Begin >= date.Begin &&
                                             d.Begin < date.End) || // Veranstaltung beginnt im Zeitraum
                                            (d.Begin <= date.Begin &&
                                             d.End >= date.End) // Veranstaltung zieht sich über gesamten Zeitraum
                                    ).ToList();

                                    summary.ConflictingDates[date].AddRange(conflictingDates);
                                }
                            }

                        }

                    }


                    model2.Add(summary);
                }

                model.Topics.Add(new TopicSummaryModel
                {
                    Topic = topic,
                    Courses = model2
                });

            }

            // jetzt noch die ohne Topics
            var withoutTopic = semGroup.Activities.OfType<Course>().Where(x => !x.SemesterTopics.Any()).ToList();

            if (withoutTopic.Any())
            {
                var model2 = new List<CourseSummaryModel>();

                foreach (var course in withoutTopic)
                {
                    var summary = courseService.GetCourseSummary(course.Id);

                    if (Request.IsAuthenticated)
                    {
                        var user = GetCurrentUser();

                        var state = ActivityService.GetActivityState(course.Occurrence, user);

                        summary.User = user;
                        summary.Subscription = state.Subscription;

                        summary.Lottery =
                            Db.Lotteries.FirstOrDefault(x => x.Occurrences.Any(y => y.Id == course.Occurrence.Id));

                        // Konflikte suchen
                        foreach (var date in course.Dates)
                        {
                            var conflictingActivities = activities.Where(x => x.Dates.Any(d =>
                                    (d.End > date.Begin && d.End <= date.End) || // Veranstaltung endet im Zeitraum
                                    (d.Begin >= date.Begin && d.Begin < date.End) || // Veranstaltung beginnt im Zeitraum
                                    (d.Begin <= date.Begin && d.End >= date.End) // Veranstaltung zieht sich über gesamten Zeitraum
                            )).ToList();

                            if (conflictingActivities.Any())
                            {
                                foreach (var conflictingActivity in conflictingActivities.Where(x => x.Id != course.Id))        // nicht mit dem Vergleichen, wo selbst eingetragen
                                {
                                    summary.ConflictingDates[date] = new List<ActivityDate>();

                                    var conflictingDates = conflictingActivity.Dates.Where(d =>
                                            (d.End > date.Begin && d.End <= date.End) || // Veranstaltung endet im Zeitraum
                                            (d.Begin >= date.Begin &&
                                             d.Begin < date.End) || // Veranstaltung beginnt im Zeitraum
                                            (d.Begin <= date.Begin &&
                                             d.End >= date.End) // Veranstaltung zieht sich über gesamten Zeitraum
                                    ).ToList();

                                    summary.ConflictingDates[date].AddRange(conflictingDates);
                                }
                            }

                        }

                    }

                    model2.Add(summary);
                }

                model.Topics.Add(new TopicSummaryModel
                {
                    Topic = null,
                    Courses = model2
                });
            }


            return View("GroupByTopic", model);
        }


        public ActionResult GroupList(Guid semId, Guid groupId)
        {
            var semester = SemesterService.GetSemester(semId);
            var capGroup = Db.CapacityGroups.SingleOrDefault(x => x.Id == groupId);

            var model = new SemesterActiveViewModel
            {
                Semester = semester,
                Organiser = capGroup.CurriculumGroup.Curriculum.Organiser,
                Curriculum = capGroup.CurriculumGroup.Curriculum,
                CapacityGroup = capGroup,
            };

            var semGroup =
                Db.SemesterGroups.FirstOrDefault(x =>
                    x.CapacityGroup.Id == capGroup.Id && x.Semester.Id == semester.Id);



            model.SemesterGroup = semGroup;

            return View("Group", model);
        }


        [HttpPost]
        public PartialViewResult CourseListForGroup(Guid semGroupId, bool showPersonalDates)
        {
            var user = GetCurrentUser();
            var semGroup = Db.SemesterGroups.SingleOrDefault(x => x.Id == semGroupId);

            var model = new SemesterActiveViewModel
            {
                Semester = semGroup.Semester,
                Organiser = semGroup.CapacityGroup.CurriculumGroup.Curriculum.Organiser,
                Curriculum = semGroup.CapacityGroup.CurriculumGroup.Curriculum,
                CapacityGroup = semGroup.CapacityGroup,
                SemesterGroup = semGroup
            };

            var courseService = new CourseService(Db);

            var courses = semGroup.Activities.OfType<Course>().ToList();

            if (user != null && showPersonalDates)
            {
                var activities = Db.Activities.OfType<Course>().Where(a =>
                    a.SemesterGroups.Any(s => s.Semester.Id == semGroup.Semester.Id) &&
                    a.Occurrence.Subscriptions.Any(u => u.UserId.Equals(user.Id))).ToList();

                foreach (var course in activities)
                {
                    if (!courses.Contains(course))
                    {
                        courses.Add(course);
                    }
                }
            }


            foreach (var course in courses)
            {
                var summary = courseService.GetCourseSummary(course);

                if (Request.IsAuthenticated && showPersonalDates)
                {

                    var state = ActivityService.GetActivityState(course.Occurrence, user);

                    summary.User = user;
                    summary.Subscription = state.Subscription;

                    summary.Lottery =
                        Db.Lotteries.FirstOrDefault(x => x.Occurrences.Any(y => y.Id == course.Occurrence.Id));
                }


                model.Courses.Add(summary);
            }

            return PartialView("_GroupList", model);
        }


        [HttpPost]
        public PartialViewResult CourseListForTopic(Guid semGroupId, bool showPersonalDates)
        {
            var semGroup = Db.SemesterGroups.SingleOrDefault(x => x.Id == semGroupId);
            var semester = semGroup.Semester;
            var capGroup = semGroup.CapacityGroup;

            var model = new SemesterActiveViewModel
            {
                Semester = semester,
                Organiser = capGroup.CurriculumGroup.Curriculum.Organiser,
                Curriculum = capGroup.CurriculumGroup.Curriculum,
                CapacityGroup = capGroup,
            };


            model.SemesterGroup = semGroup;

            var allTopics = Db.SemesterTopics.Where(x => x.Activities.Any(s => s.SemesterGroups.Any(g => g.Id == semGroup.Id))).ToList();


            List<Course> activities = null;

            if (Request.IsAuthenticated)
            {
                var user = GetCurrentUser();
                activities = Db.Activities.OfType<Course>().Where(a =>
                    a.SemesterGroups.Any(g => g.Semester.Id == semester.Id) &&
                    a.Occurrence.Subscriptions.Any(u => u.UserId.Equals(user.Id))).ToList();
            }

            var courseService = new CourseService(Db);

            foreach (var topic in allTopics)
            {
                var courses = topic.Activities.OfType<Course>().ToList();

                var model2 = new List<CourseSummaryModel>();


                foreach (var course in courses)
                {
                    var summary = courseService.GetCourseSummary(course);

                    if (Request.IsAuthenticated)
                    {
                        var user = GetCurrentUser();

                        var state = ActivityService.GetActivityState(course.Occurrence, user);

                        summary.User = user;
                        summary.Subscription = state.Subscription;

                        summary.Lottery =
                            Db.Lotteries.FirstOrDefault(x => x.Occurrences.Any(y => y.Id == course.Occurrence.Id));

                        // Konflikte suchen
                        foreach (var date in course.Dates)
                        {
                            var conflictingActivities = activities.Where(x => x.Dates.Any(d =>
                                    (d.End > date.Begin && d.End <= date.End) || // Veranstaltung endet im Zeitraum
                                    (d.Begin >= date.Begin && d.Begin < date.End) || // Veranstaltung beginnt im Zeitraum
                                    (d.Begin <= date.Begin && d.End >= date.End) // Veranstaltung zieht sich über gesamten Zeitraum
                            )).ToList();

                            if (conflictingActivities.Any())
                            {
                                foreach (var conflictingActivity in conflictingActivities.Where(x => x.Id != course.Id))        // nicht mit dem Vergleichen, wo selbst eingetragen
                                {
                                    summary.ConflictingDates[date] = new List<ActivityDate>();

                                    var conflictingDates = conflictingActivity.Dates.Where(d =>
                                            (d.End > date.Begin && d.End <= date.End) || // Veranstaltung endet im Zeitraum
                                            (d.Begin >= date.Begin &&
                                             d.Begin < date.End) || // Veranstaltung beginnt im Zeitraum
                                            (d.Begin <= date.Begin &&
                                             d.End >= date.End) // Veranstaltung zieht sich über gesamten Zeitraum
                                    ).ToList();

                                    summary.ConflictingDates[date].AddRange(conflictingDates);
                                }
                            }

                        }

                    }


                    model2.Add(summary);
                }

                model.Topics.Add(new TopicSummaryModel
                {
                    Topic = topic,
                    Courses = model2
                });

            }

            // jetzt noch die ohne Topics
            var withoutTopic = semGroup.Activities.OfType<Course>().Where(x => !x.SemesterTopics.Any()).ToList();

            if (withoutTopic.Any())
            {
                var model2 = new List<CourseSummaryModel>();

                foreach (var course in withoutTopic)
                {
                    var summary = courseService.GetCourseSummary(course.Id);

                    if (Request.IsAuthenticated)
                    {
                        var user = GetCurrentUser();

                        var state = ActivityService.GetActivityState(course.Occurrence, user);

                        summary.User = user;
                        summary.Subscription = state.Subscription;

                        summary.Lottery =
                            Db.Lotteries.FirstOrDefault(x => x.Occurrences.Any(y => y.Id == course.Occurrence.Id));

                        // Konflikte suchen
                        foreach (var date in course.Dates)
                        {
                            var conflictingActivities = activities.Where(x => x.Dates.Any(d =>
                                    (d.End > date.Begin && d.End <= date.End) || // Veranstaltung endet im Zeitraum
                                    (d.Begin >= date.Begin && d.Begin < date.End) || // Veranstaltung beginnt im Zeitraum
                                    (d.Begin <= date.Begin && d.End >= date.End) // Veranstaltung zieht sich über gesamten Zeitraum
                            )).ToList();

                            if (conflictingActivities.Any())
                            {
                                foreach (var conflictingActivity in conflictingActivities.Where(x => x.Id != course.Id))        // nicht mit dem Vergleichen, wo selbst eingetragen
                                {
                                    summary.ConflictingDates[date] = new List<ActivityDate>();

                                    var conflictingDates = conflictingActivity.Dates.Where(d =>
                                            (d.End > date.Begin && d.End <= date.End) || // Veranstaltung endet im Zeitraum
                                            (d.Begin >= date.Begin &&
                                             d.Begin < date.End) || // Veranstaltung beginnt im Zeitraum
                                            (d.Begin <= date.Begin &&
                                             d.End >= date.End) // Veranstaltung zieht sich über gesamten Zeitraum
                                    ).ToList();

                                    summary.ConflictingDates[date].AddRange(conflictingDates);
                                }
                            }

                        }

                    }

                    model2.Add(summary);
                }

                model.Topics.Add(new TopicSummaryModel
                {
                    Topic = null,
                    Courses = model2
                });
            }

            return PartialView("_GroupListByTopicNew", model);
        }




        private CourseSelectModel GetBookingStateModel(Guid id)
        {
            var user = GetCurrentUser();

            var student = StudentService.GetCurrentStudent(user.Id);

            var courseService = new CourseService(Db);

            var courseSummary = courseService.GetCourseSummary(id);

            var bookingService = new BookingService(Db);

            var bookingLists = bookingService.GetBookingLists(courseSummary.Course.Occurrence.Id);

            var subscriptionService = new SubscriptionService(Db);

            var subscription = subscriptionService.GetSubscription(courseSummary.Course.Occurrence.Id, user.Id);

            // Konflikte suchen
            if (Request.IsAuthenticated)
            {
                var firstDate = courseSummary.Course.Dates.Min(x => x.Begin);
                var lastDate = courseSummary.Course.Dates.Max(x => x.Begin);

                var activities = Db.Activities.OfType<Course>().Where(a =>
                    a.Occurrence.Subscriptions.Any(u => u.UserId.Equals(user.Id)) &&
                    a.Dates.Any(d => d.Begin >= firstDate && d.End <= lastDate)).ToList();

                courseSummary.ConflictingDates = courseService.GetConflictingDates(courseSummary.Course, activities);

                /*
                foreach (var date in courseSummary.Course.Dates)
                {
                    var conflictingActivities = activities.Where(x => 
                        x.Id != courseSummary.Course.Id &&    
                        x.Dates.Any(d =>
                            (d.End > date.Begin && d.End <= date.End) || // Veranstaltung endet im Zeitraum
                            (d.Begin >= date.Begin && d.Begin < date.End) || // Veranstaltung beginnt im Zeitraum
                            (d.Begin <= date.Begin &&
                             d.End >= date.End) // Veranstaltung zieht sich über gesamten Zeitraum
                    )).ToList();

                    courseSummary.ConflictingDates[date] = new List<ActivityDate>();

                    foreach (var conflictingActivity in conflictingActivities)
                    {
                        var conflictingDates = conflictingActivity.Dates.Where(d =>
                                (d.End > date.Begin && d.End <= date.End) || // Veranstaltung endet im Zeitraum
                                (d.Begin >= date.Begin &&
                                 d.Begin < date.End) || // Veranstaltung beginnt im Zeitraum
                                (d.Begin <= date.Begin &&
                                 d.End >= date.End) // Veranstaltung zieht sich über gesamten Zeitraum
                        ).ToList();
                        courseSummary.ConflictingDates[date].AddRange(conflictingDates);
                    }
                }
                */
            }

            var bookingState = new BookingState
            {
                Student = student,
                Occurrence = courseSummary.Course.Occurrence,
                BookingLists = bookingLists
            };
            bookingState.Init();

            var model = new CourseSelectModel
            {
                User = user,
                Student = student,
                Summary = courseSummary,
                BookingState = bookingState,
                Subscription = subscription,
            };

            return model;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">Id des Kurses</param>
        /// <returns></returns>
        public PartialViewResult SelectCourse(Guid id)
        {
            var model = GetBookingStateModel(id);

            var userRights = GetUserRight(User.Identity.Name, model.Summary.Course);
            ViewBag.UserRight = userRights;

            return PartialView("_SelectCourse", model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">Id des Kurses</param>
        /// <returns></returns>

        public PartialViewResult Subscribe(Guid id)
        {
            var logger = LogManager.GetLogger("Booking");

            var user = GetCurrentUser();
            var student = StudentService.GetCurrentStudent(user);
            var course = Db.Activities.OfType<Course>().SingleOrDefault(x => x.Id == id);
            OccurrenceSubscription succeedingSubscription = null;

            Occurrence occ = course.Occurrence;
            OccurrenceSubscription subscription = null;

            using (var transaction = Db.Database.BeginTransaction())
            {
                subscription = occ.Subscriptions.FirstOrDefault(x => x.UserId.Equals(user.Id));

                var bookingService = new BookingService(Db);
                var bookingLists = bookingService.GetBookingLists(occ.Id);
                var bookingState = new BookingState
                {
                    Student = student,
                    Occurrence = occ,
                    BookingLists = bookingLists
                };
                bookingState.Init();

                var bookingList = bookingState.MyBookingList;

                if (subscription == null)
                {
                    // eintragen
                    // den Status aus den Buchungslisten ermitteln
                    // ermittle Buchungsliste
                    // wenn eine Liste
                    // wenn voll, dann Warteliste
                    // sonst Teilnehmer
                    // sonst
                    // Fehlermeldung an Benutzer mit Angabe des Grunds

                    if (bookingList != null)
                    {
                        subscription = new OccurrenceSubscription
                        {
                            TimeStamp = DateTime.Now,
                            Occurrence = occ,
                            UserId = user.Id,
                            OnWaitingList = bookingState.AvailableSeats <= 0
                        };

                        Db.Subscriptions.Add(subscription);
                    }

                }
                else
                {
                    // austragen
                    var subscriptionService = new SubscriptionService(Db);
                    subscriptionService.DeleteSubscription(subscription);

                    // Nachrücken
                    if (bookingList != null)
                    {
                        var succBooking = bookingList.GetSucceedingBooking();
                        if (succBooking != null)
                        {
                            succBooking.Subscription.OnWaitingList = false;
                            succeedingSubscription = succBooking.Subscription;
                        }
                    }
                }

                Db.SaveChanges();
                transaction.Commit();
            }

            // Mail an Nachrücker versenden
            if (succeedingSubscription != null)
            {
                var mailService = new SubscriptionMailService();
                mailService.SendSucceedingEMail(course, succeedingSubscription);

                var subscriber = GetUser(succeedingSubscription.UserId);
                logger.InfoFormat("{0} ({1}) for [{2}]: set on participient list",
                    course.Name, course.ShortName, subscriber.UserName);

            }

            // jetzt neu abrufen und anzeigen
            var model = GetBookingStateModel(course.Id);

            return PartialView("_CourseSummaryBookingBox", model);
        }

    }
}