using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MyStik.TimeTable.Data;
using MyStik.TimeTable.DataServices;
using MyStik.TimeTable.Web.Models;
using MyStik.TimeTable.Web.Services;

namespace MyStik.TimeTable.Web.Controllers
{
    [AllowAnonymous]
    public class DictionaryController : BaseController
    {
        public ActionResult Index()
        {
            var model = new HomeViewModel();


            // Alle Semester mit veröffentlichten Semestergruppen
            var allPublishedSemester = Db.Semesters.Where(x => x.Groups.Any(g => g.IsAvailable)).OrderByDescending(s => s.EndCourses).Take(4).ToList();
            foreach (var semester in allPublishedSemester)
            {
                var activeOrgs = SemesterService.GetActiveOrganiser(semester, true);

                var semModel = new SemesterActiveViewModel
                {
                    Semester = semester,
                    Organisers = activeOrgs.ToList()
                };

                model.ActiveSemester.Add(semModel);
            }

            return View(model);
        }

        public ActionResult Semester(Guid semId)
        {
            var semester = SemesterService.GetSemester(semId);

            var curricula = SemesterService.GetActiveCurricula(semester, true).ToList();

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

            if (allTopics.Any())
            {
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


                return View("CourseListByTopics", model);
            }
            else
            {
                var courses = semGroup.Activities.OfType<Course>().ToList();

                foreach (var course in courses)
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


                    model.Courses.Add(summary);
                }

                return View("CourseList", model);
            }
        }


        public PartialViewResult CourseListForGroup(Guid semGroupId)
        {
            var semGroup = Db.SemesterGroups.SingleOrDefault(x => x.Id == semGroupId);

            var model = new SemesterActiveViewModel
            {
                Semester = semGroup.Semester,
                Organiser = semGroup.CapacityGroup.CurriculumGroup.Curriculum.Organiser,
                Curriculum = semGroup.CapacityGroup.CurriculumGroup.Curriculum,
                CapacityGroup = semGroup.CapacityGroup,
                SemesterGroup = semGroup
            };

            List<Course> activities = null;

            if (Request.IsAuthenticated)
            {
                var user = GetCurrentUser();
                activities = Db.Activities.OfType<Course>().Where(a =>
                    a.SemesterGroups.Any(g => g.Semester.Id == semGroup.Semester.Id) &&
                    a.Occurrence.Subscriptions.Any(u => u.UserId.Equals(user.Id))).ToList();
            }

            var courseService = new CourseService(Db);

            var courses = semGroup.Activities.OfType<Course>().ToList();

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
                                (d.Begin <= date.Begin &&
                                 d.End >= date.End) // Veranstaltung zieht sich über gesamten Zeitraum
                        )).ToList();

                        if (conflictingActivities.Any())
                        {
                            foreach (var conflictingActivity in conflictingActivities.Where(x => x.Id != course.Id)
                            ) // nicht mit dem Vergleichen, wo selbst eingetragen
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


                model.Courses.Add(summary);
            }

            return PartialView("_GroupList", model);
        }

        public PartialViewResult CourseListForGroupNew(Guid semGroupId)
        {
            var semGroup = Db.SemesterGroups.SingleOrDefault(x => x.Id == semGroupId);

            var model = new SemesterActiveViewModel
            {
                Semester = semGroup.Semester,
                Organiser = semGroup.CapacityGroup.CurriculumGroup.Curriculum.Organiser,
                Curriculum = semGroup.CapacityGroup.CurriculumGroup.Curriculum,
                CapacityGroup = semGroup.CapacityGroup,
                SemesterGroup = semGroup
            };

            List<Course> activities = null;

            if (Request.IsAuthenticated)
            {
                var user = GetCurrentUser();
                activities = Db.Activities.OfType<Course>().Where(a =>
                    a.SemesterGroups.Any(g => g.Semester.Id == semGroup.Semester.Id) &&
                    a.Occurrence.Subscriptions.Any(u => u.UserId.Equals(user.Id))).ToList();
            }

            var courseService = new CourseService(Db);

            var courses = semGroup.Activities.OfType<Course>().ToList();

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
                                (d.Begin <= date.Begin &&
                                 d.End >= date.End) // Veranstaltung zieht sich über gesamten Zeitraum
                        )).ToList();

                        if (conflictingActivities.Any())
                        {
                            foreach (var conflictingActivity in conflictingActivities.Where(x => x.Id != course.Id)
                            ) // nicht mit dem Vergleichen, wo selbst eingetragen
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


                model.Courses.Add(summary);
            }

            return PartialView("_GroupListNew", model);
        }


    }
}