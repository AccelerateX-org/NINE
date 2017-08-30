using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using MyStik.TimeTable.Data;
using MyStik.TimeTable.DataServices;
using MyStik.TimeTable.Web.Models;
using MyStik.TimeTable.Web.Services;

namespace MyStik.TimeTable.Web.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public class PlanerController : BaseController
    {
        /// <summary>
        /// Anzeige der Stundenplanung
        /// Es werden nur die Fakultäten und Studiengänge angezeigt, die über aktive 
        /// Studiengänge verfügen
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {

            // das aktuelle Semester
            var semester = GetSemester();
            var myOrg = GetMyOrganisation();


            var semService = new SemesterService(Db);
            var semSubService = new SemesterSubscriptionService(Db);

            // Alle Fakultäten, die aktive Semestergruppen haben
            var acticeorgs = //semService.GetActiveOrganiser(semester);

            Db.Organisers.Where(
                x => x.IsFaculty && x.Activities.Any(a =>
                         a.SemesterGroups.Any(s => s.Semester.EndCourses >= DateTime.Today && s.IsAvailable))).ToList();


            var semGroup = semSubService.GetSemesterGroup(AppUser.Id, semester);
            

            // Dieses Modell ist für die Selektion in den Auswahllisten verantwortlich
            var model = new GroupSelectionViewModel
            {
                Faculty = myOrg.Id.ToString(),
                Curriculum = semGroup != null ? semGroup.CapacityGroup.CurriculumGroup.Curriculum.Id.ToString() : string.Empty,
                Group = semGroup != null ? semGroup.Id.ToString() : string.Empty,
                Semester = semester.Id.ToString(),
                Subscription = semSubService.GetSubscription(AppUser.Id, semester.Id)
            };


            // Immer alle anzeigen, die was haben
            ViewBag.Faculties = acticeorgs.OrderBy(f => f.ShortName).Select(f => new SelectListItem
            {
                Text = f.ShortName,
                Value = f.Id.ToString(),
            });


            // nur das aktuelle Semester
            // alle Semester in der Zukunft mit veröffentlichten Semestergruppen
            ViewBag.Semesters = Db.Semesters
                .Where(x => x.EndCourses >= DateTime.Today && x.Groups.Any(g => g.IsAvailable))
                .OrderByDescending(x => x.EndCourses)
                .Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString()

                });

            // Liste der Studiengänge
            // myOrg ist immer gültig
            // wenn myOrg was anbietet, dann daraus
            // sonst org nehmen die was anbietet!
            var selectedOrg = myOrg;
            if (acticeorgs.Any() && !acticeorgs.Contains(myOrg))
            {
                selectedOrg = acticeorgs.First();
            }

            var activecurr = semService.GetActiveCurricula(selectedOrg, semester).OrderBy(f => f.Name).ThenBy(f => f.ShortName);

            ViewBag.Curricula = activecurr.Select(f => new SelectListItem
            {
                Text = f.Name,
                Value = f.Id.ToString(),
            });


            // Liste der Gruppen hängt jetzt von der Einschreibung ab
            
            if (semGroup != null)
            {
                // es sind keine Studiengänge verfügbar => Leere Liste
                if (!activecurr.Any())
                {
                    ViewBag.Groups = new SelectList(
                        new List<SelectListItem>
                            {
                                new SelectListItem { Selected = true, Text = "", Value = "-1"}
                            }, "Value", "Text", 1);
                    ViewBag.Topics = new SelectList(
                        new List<SelectListItem>
                        {
                            new SelectListItem { Selected = true, Text = "", Value = "-1"}
                        }, "Value", "Text", 1);
                }
                else
                {
                     var myCurr = semGroup.CapacityGroup.CurriculumGroup.Curriculum;

                    var semesterGroups = Db.SemesterGroups.Where(g =>
                        g.Semester.Id == semester.Id &&
                        g.IsAvailable &&
                        g.CapacityGroup.CurriculumGroup.Curriculum.Id == myCurr.Id).OrderBy(g => g.CapacityGroup.CurriculumGroup.Name).ToList();


                    var semGroups = semesterGroups.Select(s => new SelectListItem
                    {
                        Text = s.GroupName,
                        Value = s.Id.ToString()
                    }).ToList();

                    ViewBag.Groups = semGroups;

                    var topics = Db.SemesterTopics.Where(x => 
                        x.Semester.Id == semester.Id && 
                        x.Topic.Chapter.Curriculum.Id == myCurr.Id &&
                        x.Activities.Any(s => s.SemesterGroups.Any(g => g.Id == semGroup.Id))
                        ).ToList();

                    var topics2 = topics.Select(s => new SelectListItem
                    {
                        Text = s.TopicName,
                        Value = s.Id.ToString()
                    }).ToList();

                    ViewBag.Topics = topics2;
                }
            }
            else
            {
                // keine Einschreibung vorhanden
                if (!activecurr.Any())
                {
                    ViewBag.Groups = new SelectList(
                        new List<SelectListItem>
                            {
                                new SelectListItem { Selected = true, Text = "", Value = "-1"}
                            }, "Value", "Text", 1);

                    ViewBag.Topics = new SelectList(
                        new List<SelectListItem>
                        {
                            new SelectListItem { Selected = true, Text = "", Value = "-1"}
                        }, "Value", "Text", 1);
                }
                else
                {
                    // nimm den ersten
                    var myCurr = activecurr.First();

                    var semesterGroups = Db.SemesterGroups.Where(g =>
                        g.Semester.Id == semester.Id &&
                        g.IsAvailable &&
                        g.CapacityGroup.CurriculumGroup.Curriculum.Id == myCurr.Id).OrderBy(g => g.CapacityGroup.CurriculumGroup.Name).ToList();


                    var semGroups = semesterGroups.Select(s => new SelectListItem
                    {
                        Text = s.GroupName,
                        Value = s.Id.ToString()
                    }).ToList();

                    ViewBag.Groups = semGroups;

                    // TODO: Auffüllen
                    ViewBag.Topics = new SelectList(
                        new List<SelectListItem>
                        {
                            new SelectListItem { Selected = true, Text = "", Value = "-1"}
                        }, "Value", "Text", 1);

                }
            }


            ViewBag.UserRight = GetUserRight();

            return View(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Courses()
        {
            var semSubService = new SemesterSubscriptionService();

            var semester = GetSemester();
            var user = AppUser;


            var model = new DashboardStudentViewModel();

            model.Semester = GetSemester();
            model.User = user;
            model.SemesterGroup = semSubService.GetSemesterGroup(model.User.Id, model.Semester);


            
            var activities = Db.Activities.OfType<Course>().Where(a => 
                a.SemesterGroups.Any(g => g.Semester.Id == semester.Id) &&
                a.Occurrence.Subscriptions.Any(u => u.UserId.Equals(model.User.Id))).ToList();
            foreach (var activity in activities)
            {
                // Die Platzverlosungen, bei denen ich dabei bin
                var lottery = Db.Lotteries.SingleOrDefault(l => l.Occurrences.Any(o => o.Id == activity.Occurrence.Id));

                model.MyCourseSubscriptions.Add(new ActivitySubscriptionStateModel
                {
                    Activity = new ActivitySummary { Activity = activity },
                    State = ActivityService.GetActivityState(activity.Occurrence, user, semester),
                    Lottery = lottery
                });


                // und hier den Tagesplan
                var sub = activity.Occurrence.Subscriptions.FirstOrDefault(s => s.UserId.Equals(user.Id));
                if (sub.OnWaitingList == false)
                {

                    foreach (var courseDate in activity.Dates)
                    {
                        var dayPlan = model.CourseDates.SingleOrDefault(x => x.Day == courseDate.Begin.Date);
                        if (dayPlan == null)
                        {
                            dayPlan = new UserCourseDatePlanModel { Day = courseDate.Begin.Date };
                            model.CourseDates.Add(dayPlan);
                        }

                        dayPlan.Dates.Add(courseDate);
                    }
                }
            }

            return View(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult OfficeHour()
        {
            var semSubService = new SemesterSubscriptionService();

            var semester = GetSemester();
            var user = AppUser;


            var model = new List<OfficeHourSubscriptionViewModel>();

            ViewBag.Semester = GetSemester();


            var activities = Db.Activities.OfType<OfficeHour>().Where(a =>
                a.Dates.Any(d => d.Occurrence.Subscriptions.Any(u => u.UserId.Equals(user.Id)) ||
                d.Slots.Any(s => s.Occurrence.Subscriptions.Any(u => u.UserId.Equals(user.Id))))).ToList();

            foreach (var activity in activities)
            {
                var actModel = new OfficeHourSubscriptionViewModel
                {
                    Host = activity.Owners.First().Member
                };

                if (activity.Dates.Any(d => d.Slots.Any()))
                {
                    foreach (var date in activity.Dates)
                    {
                        actModel.MySlots.AddRange(date.Slots
                            .Where(d => d.Occurrence.Subscriptions.Any(u => u.UserId.Equals(user.Id))).ToList());
                    }
                }
                else
                {
                    actModel.MyDates.AddRange(activity.Dates
                        .Where(d => d.Occurrence.Subscriptions.Any(u => u.UserId.Equals(user.Id))).ToList());
                }
                model.Add(actModel);
            }


            return View(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Events()
        {
            var semSubService = new SemesterSubscriptionService();

            var semester = GetSemester();
            var user = AppUser;


            var model = new DashboardStudentViewModel();

            model.Semester = GetSemester();
            model.User = user;
            model.SemesterGroup = semSubService.GetSemesterGroup(model.User.Id, model.Semester);


            // alle Events bei denen ich bei mindestens einem Termin eingetragen bin
            var activities = Db.Activities.OfType<Event>().Where(a =>
                a.SemesterGroups.Any(g => g.Semester.Id == semester.Id) &&
                a.Dates.Any(d => d.Occurrence.Subscriptions.Any(u => u.UserId.Equals(model.User.Id)))).ToList();
            foreach (var activity in activities)
            {
                // Die Platzverlosungen, bei denen ich dabei bin
                //var lottery = Db.Lotteries.SingleOrDefault(l => l.Occurrences.Any(o => o.Id == activity.Occurrence.Id));

                model.MyCourseSubscriptions.Add(new ActivitySubscriptionStateModel
                {
                    Activity = new ActivitySummary { Activity = activity },
                    State = ActivityService.GetActivityState(activity.Occurrence, user, semester),
                });
            }

            return View(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="semGroupId"></param>
        /// <param name="topicId"></param>
        /// <returns></returns>
        [HttpPost]
        public PartialViewResult CourseListByProgram(Guid semGroupId, Guid? topicId)
        {
            var semester = GetSemester();
            var user = AppUser;
            var courseService = new CourseService(UserManager);

            var semGroup = Db.SemesterGroups.SingleOrDefault(g => g.Id == semGroupId);
            var allTopics = Db.SemesterTopics.Where(x => x.Activities.Any(s => s.SemesterGroups.Any(g => g.Id == semGroupId))).ToList();

            if (allTopics.Any())
            {
                var model = new List<TopicSummaryModel>();

                foreach (var topic in allTopics)
                {
                    var courses = topic.Activities.ToList();

                    var model2 = new List<CourseSummaryModel>();


                    foreach (var course in courses)
                    {
                        var summary = courseService.GetCourseSummary(course);

                        summary.State = ActivityService.GetActivityState(course.Occurrence, user, semester);
                        summary.SemesterGroup = semGroup;
                        summary.Summary = new ActivitySummary(course);

                        summary.HasLottery = Db.Lotteries.Any(x => x.Occurrences.Any(y => y.Id == course.Occurrence.Id));

                        model2.Add(summary);
                    }

                    model.Add(new TopicSummaryModel
                    {
                        Topic = topic,
                        Courses = model2
                    });

                }

                // jetzt noch die ohne Topics
                var withoutTopic = semGroup.Activities.Where(x => !x.SemesterTopics.Any()).ToList();

                if (withoutTopic.Any())
                {
                    var model2 = new List<CourseSummaryModel>();

                    foreach (var course in withoutTopic)
                    {
                        var summary = courseService.GetCourseSummary(course);

                        summary.State = ActivityService.GetActivityState(course.Occurrence, user, semester);
                        summary.SemesterGroup = semGroup;
                        summary.Summary = new ActivitySummary(course);

                        summary.HasLottery = Db.Lotteries.Any(x => x.Occurrences.Any(y => y.Id == course.Occurrence.Id));

                        model2.Add(summary);
                    }

                    model.Add(new TopicSummaryModel
                    {
                        Topic = null,
                        Courses = model2
                    });
                }


                return PartialView("_CourseListByTopics", model);
            }
            else
            {
                var courses = semGroup.Activities.ToList();

                var model = new List<CourseSummaryModel>();

                foreach (var course in courses)
                {
                    var summary = courseService.GetCourseSummary(course);

                    summary.State = ActivityService.GetActivityState(course.Occurrence, user, semester);
                    summary.SemesterGroup = semGroup;
                    summary.Summary = new ActivitySummary(course);

                    summary.HasLottery = Db.Lotteries.Any(x => x.Occurrences.Any(y => y.Id == course.Occurrence.Id));


                    model.Add(summary);
                }

                return PartialView("_CourseList", model);
            }
        }

    }
}