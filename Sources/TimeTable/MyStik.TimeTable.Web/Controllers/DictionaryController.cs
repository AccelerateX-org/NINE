using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using log4net;
using MyStik.TimeTable.Data;
using MyStik.TimeTable.DataServices.Booking;
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

            var allPublishedSemester =
                Db.Activities.OfType<Course>().Where(x => x.Semester != null).Select(x => x.Semester).Distinct()
                    .OrderByDescending(s => s.EndCourses).Take(4).ToList();

            foreach (var semester in allPublishedSemester)
            {
                var activeOrgs = SemesterService.GetActiveOrganiser(semester);

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

            var activeOrgs = SemesterService.GetActiveOrganiser(semester);

            var model = new SemesterActiveViewModel
            {
                Semester = semester,
                Organisers = activeOrgs
            };

            return View(model);
        }

        public ActionResult Organiser(Guid? semId, Guid orgId)
        {
            var semester = semId.HasValue ? SemesterService.GetSemester(semId) : SemesterService.GetSemester(DateTime.Today);

            var organiser = Db.Organisers.SingleOrDefault(x => x.Id == orgId);

            var model = new SemesterActiveViewModel
            {
                Semester = semester,
                Curricula = organiser.Curricula.Where(x => !x.IsDeprecated).OrderBy(x => x.Name).ToList(),
                Organiser = organiser
            };

            return View(model);
        }

        public ActionResult Curriculum(Guid semId, Guid currId)
        {
            var semester = SemesterService.GetSemester(semId);

            var curr = Db.Curricula.SingleOrDefault(x => x.Id == currId);

            if (curr.LabelSet == null)
            {
                var labelSet = new ItemLabelSet();
                curr.LabelSet = labelSet;

                Db.ItemLabelSets.Add(labelSet);
                Db.SaveChanges();
            }



            var model = new SemesterActiveViewModel
            {
                Semester = semester,
                Organiser = curr.Organiser,
                Curriculum = curr
            };

            ViewBag.UserRight = GetUserRight(curr.Organiser);

            return View(model);
        }

        public ActionResult Planer(Guid semId, Guid currId)
        {
            var semester = SemesterService.GetSemester(semId);

            var curr = Db.Curricula.SingleOrDefault(x => x.Id == currId);

            var model = new SemesterActiveViewModel
            {
                Semester = semester,
                Organiser = curr.Organiser,
                Curriculum = curr
            };

            var allSlots = Db.CurriculumSlots.Where(x =>
                x.AreaOption != null && x.AreaOption.Area.Curriculum.Id == curr.Id).ToList();

            var minSem = allSlots.Min(x => x.Semester);
            var maxSem = allSlots.Max(x => x.Semester);

            var listSemester = new List<SelectListItem>();
            for (var i = minSem; i <= maxSem; i++)
            {
                var selectSemester = new SelectListItem
                {
                    Value = i.ToString(),
                    Text = $"Fachsemester {i}"
                };

                listSemester.Add(selectSemester);
            }

            ViewBag.ListSemester = listSemester;


            return View(model);
        }

        [HttpPost]
        public PartialViewResult GetOptions(Guid currId, Guid semId, int semNo)
        {
            var allSlots = Db.CurriculumSlots.Where(x =>
                x.AreaOption != null && x.AreaOption.Area.Curriculum.Id == currId).ToList();

            var semSlots = allSlots.Where(x => x.Semester == semNo).ToList();

            var optionSet = semSlots.Select(x => x.AreaOption).Distinct().ToList();

            var model = optionSet
                .OrderBy(g => g.Name)
                .ToList();

            return PartialView("_OptionSelectList", model);
        }


        [HttpPost]
        public PartialViewResult GetLabels(Guid currId, Guid semId, Guid? optId, int semNo)
        {
            if (optId.HasValue && semNo > 0)
            {

                // Alle LVs in dem Semester
                var labels = Db.Activities.OfType<Course>().Where(x => 
                        x.Semester.Id == semId && 
                        x.SubjectTeachings.Any(s => s.Subject.SubjectAccreditations.Any(
                            t => t.Slot.AreaOption.Area.Curriculum.Id == currId &&
                                                                           t.Slot.AreaOption.Id == optId &&
                                                                           t.Slot.Semester == semNo)))
                    .Select(c => c.LabelSet).ToList();

                var itemLabels = new List<ItemLabel>();

                foreach (var labelSet in labels.Where(x => x != null))
                {
                    itemLabels.AddRange(labelSet.ItemLabels);
                }

                var selectLabels = itemLabels.Distinct();


                var model = selectLabels
                    .OrderBy(g => g.Name)
                    .ToList();

                return PartialView("_LabelSelectList", model);
            }

            var curr = Db.Curricula.SingleOrDefault(x => x.Id == currId);

            var allItemLabels = new List<ItemLabel>();
            if (curr.Organiser.Institution != null)
            {
                allItemLabels.AddRange(curr.Organiser.Institution.LabelSet.ItemLabels);
            }

            allItemLabels.AddRange(curr.Organiser.LabelSet.ItemLabels);
            allItemLabels.AddRange(curr.LabelSet.ItemLabels);

            var allSelectLabels = allItemLabels.Distinct();

            var allModel = allSelectLabels
                .OrderBy(g => g.Name)
                .ToList();

            return PartialView("_LabelSelectList", allModel);

        }


        [HttpPost]
        public PartialViewResult GetLabeledCourses(Guid currId, Guid semId, Guid? optId, Guid? labelId, int semNo)
        {
            var cs = new CourseService();
            var model = new List<CourseSummaryModel>();


            if (optId.HasValue && semNo > 0)
            {
                // Alle LVs in dem Semester
                var courses = Db.Activities.OfType<Course>().Where(x => 
                        x.Semester.Id == semId &&
                        x.SubjectTeachings.Any(s => s.Subject.SubjectAccreditations.Any(
                            t => t.Slot.AreaOption.Area.Curriculum.Id == currId &&
                                             t.Slot.AreaOption.Id == optId &&
                                             t.Slot.Semester == semNo)))
                    .ToList();

                var labeledCourses = new List<Course>();

                if (labelId == null)
                {
                    labeledCourses.AddRange(courses);
                }
                else
                {
                    if (labelId == Guid.Empty)
                    {
                        // alle ohne Label
                        var allCourses = courses.Where(x =>
                            ((x.LabelSet == null) ||
                             (x.LabelSet != null && !x.LabelSet.ItemLabels.Any()))
                        ).ToList();

                        labeledCourses.AddRange(allCourses);
                    }
                    else
                    {
                        foreach (var course in courses)
                        {
                            if (course.LabelSet == null ||
                                !course.LabelSet.ItemLabels.Any() ||
                                course.LabelSet.ItemLabels.Any(x => x.Id == labelId))
                            {
                                labeledCourses.Add(course);
                            }
                        }
                    }
                }

                foreach (var labeledCourse in labeledCourses.OrderBy(g => g.ShortName))
                {
                    model.Add(cs.GetCourseSummary(labeledCourse));
                }

                return PartialView("_CourseList", model);
            }

            // keine Option
            var curr = Db.Curricula.SingleOrDefault(x => x.Id == currId);

            if (labelId == null)
            {
                var allCourses = Db.Activities.OfType<Course>().Where(x =>
                    x.Semester.Id == semId && x.Organiser.Id == curr.Organiser.Id).ToList();

                foreach (var labeledCourse in allCourses.OrderBy(g => g.ShortName))
                {
                    model.Add(cs.GetCourseSummary(labeledCourse));
                }

                return PartialView("_CourseList", model);
            }
            else
            {

                if (labelId == Guid.Empty)
                {
                    // alle Kurse zu diesem Studiengang
                    var courses = Db.Activities.OfType<Course>().Where(x =>
                            x.Semester.Id == semId &&
                            x.SubjectTeachings.Any(t => t.Subject.SubjectAccreditations.Any(a => a.Slot.AreaOption.Area.Curriculum.Id == currId)))
                        .ToList();

                    if (courses.Any())
                    {
                        // wen es welche gibt, dann
                        // aus denen dann die ohne Labels filtern

                        // alle ohne ein Label
                        var allCourses = courses.Where(x =>
                            ((x.LabelSet == null) ||
                             (x.LabelSet != null && !x.LabelSet.ItemLabels.Any()))
                        ).ToList();

                        foreach (var labeledCourse in courses.OrderBy(g => g.ShortName))
                        {
                            model.Add(cs.GetCourseSummary(labeledCourse));
                        }


                        return PartialView("_CourseList", model);
                    }
                    else
                    {
                        // sonst: nimm alles was zum Org gehört => es gibt keine Zuordnung zu einem Studiengang, weder über Labelm noch Module

                        var allCourses = Db.Activities.OfType<Course>().Where(x =>
                            x.Semester.Id == semId && x.Organiser.Id == curr.Organiser.Id &&
                            ((x.LabelSet == null) ||
                             (x.LabelSet != null && !x.LabelSet.ItemLabels.Any()))
                        ).ToList();

                        foreach (var labeledCourse in courses.OrderBy(g => g.ShortName))
                        {
                            model.Add(cs.GetCourseSummary(labeledCourse));
                        }

                        return PartialView("_CourseList", model);
                    }

                }
                else
                {
                    var allCourses = Db.Activities.OfType<Course>().Where(x =>
                        x.Semester.Id == semId && x.Organiser.Id == curr.Organiser.Id &&
                        x.LabelSet != null &&
                        x.LabelSet.ItemLabels.Any(l => l.Id == labelId)).ToList();

                    foreach (var labeledCourse in allCourses.OrderBy(g => g.ShortName))
                    {
                        model.Add(cs.GetCourseSummary(labeledCourse));
                    }

                    return PartialView("_CourseList", model);
                }
            }
        }




        public ActionResult Group(Guid semId, Guid groupId)
        {
            var user = GetCurrentUser();
            var semester = SemesterService.GetSemester(semId);
            var capGroup = Db.CapacityGroups.SingleOrDefault(x => x.Id == groupId);

            var semGroup =
                Db.SemesterGroups.FirstOrDefault(x =>
                    x.CapacityGroup.Id == capGroup.Id && x.Semester.Id == semester.Id);

            if (user == null || (user.MemberState != MemberState.Staff && !semGroup.IsAvailable))
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

        public ActionResult Label(Guid semId, Guid orgId, Guid labelId, Guid currId)
        {
            var semester = SemesterService.GetSemester(semId);
            var org = GetOrganiser(orgId);
            var label = Db.ItemLabels.SingleOrDefault(x => x.Id == labelId);
            var curr = Db.Curricula.SingleOrDefault(x => x.Id == currId);

            var courses = Db.Activities.OfType<Course>()
                .Where(x =>
                    x.Semester.Id == semester.Id &&
                    x.Organiser.Id == org.Id &&
                    x.LabelSet != null &&
                    x.LabelSet.ItemLabels.Any(l => l.Id == label.Id))
                .ToList();


            var cs = new CourseService();
            var courseSummaries = new List<CourseSummaryModel>();

            foreach (var labeledCourse in courses.OrderBy(g => g.ShortName))
            {
                courseSummaries.Add(cs.GetCourseSummary(labeledCourse));
            }


            var model = new SemesterActiveViewModel
            {
                Curriculum = curr,
                Semester = semester,
                Organiser = org,
                Label = label,
                Courses = courseSummaries
            };


            return View(model);
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


        public ActionResult Slot(Guid currId, Guid semId, Guid optionId, int sem)
        {
            var semester = SemesterService.GetSemester(semId);
            var option = Db.AreaOptions.SingleOrDefault(x => x.Id == optionId);
            var curriculum = Db.Curricula.SingleOrDefault(x => x.Id == currId);

            var slots = option.Slots.Where(x => x.Semester == sem).ToList();

            var model = new SlotSemesterModel
            {
                Curriculum = curriculum,
                Semester = semester,
                Option = option,
                NUmberSemester = sem,
                Slots = slots
            };


            return View(model);
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