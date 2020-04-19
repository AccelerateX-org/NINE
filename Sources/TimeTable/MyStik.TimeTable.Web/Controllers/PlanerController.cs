using System;
using System.Collections.Generic;
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
        public ActionResult Index(Guid? semId, Guid? orgId)
        {
            var semSubService = new SemesterSubscriptionService();

            var semester = SemesterService.GetSemester(semId);
            var user = AppUser;

            var student = StudentService.GetCurrentStudent(user);
            if (student?.FirstSemester == null || student.Curriculum == null || student.HasCompleted)
            {
                return RedirectToAction("Change", "Subscription");
            }


            ActivityOrganiser org = student.Curriculum.Organiser;

            var model = new PlanerGroupViewModel();

            model.Semester = semester;
            model.Organiser = org;
            

            return View(model);
        }

        public ActionResult Chapters(Guid semId, Guid currId)
        {
            var semester = SemesterService.GetSemester(semId);
            var currciculum = GetCurriculum(currId);
            var user = AppUser;

            var model = new PlanerGroupViewModel();
            model.Semester = semester;
            model.Curriculum = currciculum;
            model.Organiser = currciculum.Organiser;

            return View(model);
        }

        public ActionResult Chapter(Guid semId, Guid chapterId)
        {
            var semester = SemesterService.GetSemester(semId);
            var chapter = Db.CurriculumChapters.SingleOrDefault(x => x.Id == chapterId);
            var user = AppUser;

            var model = new PlanerGroupViewModel();
            model.Semester = semester;
            model.Curriculum = chapter.Curriculum;
            model.Organiser = chapter.Curriculum.Organiser;
            model.Chapter = chapter;

            // Kursliste
            var courses = Db.Activities.OfType<Course>().Where(x => x.SemesterTopics.Any(y => y.Semester.Id == semester.Id && y.Topic.Chapter.Id == chapter.Id))
                .ToList();

            model.Courses = GetCourseSummaries(courses, user, semester);

            // die Liste aller Semester, in denen es Semestergruppen gab, gibt oder geben wird
            var allSemester = Db.Semesters
                .Where(x => x.Topics.Any(g => g.Topic.Chapter.Id == chapterId))
                .OrderBy(x => x.StartCourses).ToList();

            // nur die aktiven!
            if (AppUser.MemberState != MemberState.Staff)
            {
                allSemester = allSemester.Where(x => x.Groups.Any(g => g.IsAvailable)).ToList();
            }

            // ist das angefagte dabei?
            if (!allSemester.Contains(semester))
            {
                allSemester.Add(semester);
                allSemester = allSemester.OrderBy(x => x.StartCourses).ToList();
            }

            ViewBag.Semesters = allSemester
                .Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString(),
                    Selected = (x.Id == semester.Id)
                });



            return View(model);
        }

        [HttpPost]
        public PartialViewResult ChapterList(Guid semId, Guid chapterId)
        {
            var semester = SemesterService.GetSemester(semId);
            var chapter = Db.CurriculumChapters.SingleOrDefault(x => x.Id == chapterId);
            var user = AppUser;

            var model = new PlanerGroupViewModel();
            model.Semester = semester;
            model.Curriculum = chapter.Curriculum;
            model.Organiser = chapter.Curriculum.Organiser;
            model.Chapter = chapter;

            // Kursliste
            var courses = Db.Activities.OfType<Course>().Where(x => x.SemesterTopics.Any(y => y.Semester.Id == semester.Id && y.Topic.Chapter.Id == chapter.Id))
                .ToList();

            model.Courses = GetCourseSummaries(courses, user, semester);

            return PartialView("_CourseListFit", model.Courses);
        }


        public ActionResult Topic(Guid semId, Guid topicId)
        {
            var semester = SemesterService.GetSemester(semId);
            var topic = Db.CurriculumTopics.SingleOrDefault(x => x.Id == topicId);
            var user = AppUser;

            var model = new PlanerGroupViewModel();
            model.Semester = semester;
            model.Curriculum = topic.Chapter.Curriculum;
            model.Organiser = topic.Chapter.Curriculum.Organiser;
            model.Topic = topic;

            // Kursliste
            var courses = Db.Activities.OfType<Course>().Where(x => x.SemesterTopics.Any(y => y.Semester.Id == semester.Id &&  y.Topic.Id == topic.Id))
                .ToList();

            model.Courses = GetCourseSummaries(courses, user, semester);

            // die Liste aller Semester, in denen es Semestergruppen gab, gibt oder geben wird
            var allSemester = Db.Semesters
                .Where(x => x.Topics.Any(g => g.Topic.Id == topicId))
                .OrderBy(x => x.StartCourses).ToList();

            // nur die aktiven!
            if (AppUser.MemberState != MemberState.Staff)
            {
                allSemester = allSemester.Where(x => x.Groups.Any(g => g.IsAvailable)).ToList();
            }

            // ist das angefagte dabei?
            if (!allSemester.Contains(semester))
            {
                allSemester.Add(semester);
                allSemester = allSemester.OrderBy(x => x.StartCourses).ToList();
            }


            ViewBag.Semesters = allSemester
                .Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString(),
                    Selected = (x.Id == semester.Id)
                });



            return View(model);
        }

        [HttpPost]
        public PartialViewResult TopicList(Guid semId, Guid topicId)
        {
            var semester = SemesterService.GetSemester(semId);
            var topic = Db.CurriculumTopics.SingleOrDefault(x => x.Id == topicId);
            var user = AppUser;

            var model = new PlanerGroupViewModel();
            model.Semester = semester;
            model.Curriculum = topic.Chapter.Curriculum;
            model.Organiser = topic.Chapter.Curriculum.Organiser;
            model.Topic = topic;

            // Kursliste
            var courses = Db.Activities.OfType<Course>().Where(x => x.SemesterTopics.Any(y => y.Semester.Id == semester.Id && y.Topic.Id == topic.Id))
                .ToList();

            model.Courses = GetCourseSummaries(courses, user, semester);

            return PartialView("_CourseListFit", model.Courses);
        }


        public ActionResult CurriculumGroup(Guid semId, Guid groupId)
        {
            var semester = SemesterService.GetSemester(semId);
            var group = Db.CurriculumGroups.SingleOrDefault(x => x.Id == groupId);
            var user = AppUser;

            var model = new PlanerGroupViewModel();
            model.Semester = semester;
            model.Curriculum = group.Curriculum;
            model.Organiser = group.Curriculum.Organiser;
            model.CurriculumGroup = group;

            // Kursliste
            var courses = Db.Activities.OfType<Course>().Where(x => x.SemesterGroups.Any(y => y.Semester.Id == semester.Id && y.CapacityGroup.CurriculumGroup.Id == group.Id))
                .ToList();

            model.Courses = GetCourseSummaries(courses, user, semester);


            // die Liste aller Semester, in denen es Semestergruppen gab, gibt oder geben wird
            var allSemester = Db.Semesters
                .Where(x => x.Groups.Any(g => g.CapacityGroup.CurriculumGroup.Id == groupId))
                .OrderBy(x => x.StartCourses).ToList();

            // nur die aktiven!
            if (AppUser.MemberState != MemberState.Staff)
            {
                allSemester = allSemester.Where(x => x.Groups.Any(g => g.IsAvailable)).ToList();
            }

            // ist das angefagte dabei?
            if (!allSemester.Contains(semester))
            {
                allSemester.Add(semester);
                allSemester = allSemester.OrderBy(x => x.StartCourses).ToList();
            }
            

            ViewBag.Semesters = allSemester
                    .Select(x => new SelectListItem
                    {
                        Text = x.Name,
                        Value = x.Id.ToString(),
                        Selected = (x.Id == semester.Id)
                    });

            return View(model);
        }

        [HttpPost]
        public PartialViewResult CurriculumGroupList(Guid semId, Guid groupId)
        {
            var semester = SemesterService.GetSemester(semId);
            var group = Db.CurriculumGroups.SingleOrDefault(x => x.Id == groupId);
            var user = AppUser;

            var model = new PlanerGroupViewModel();
            model.Semester = semester;
            model.Curriculum = group.Curriculum;
            model.Organiser = group.Curriculum.Organiser;
            model.CurriculumGroup = group;

            // Kursliste
            var courses = Db.Activities.OfType<Course>().Where(x => x.SemesterGroups.Any(y => y.Semester.Id == semester.Id && y.CapacityGroup.CurriculumGroup.Id == group.Id))
                .ToList();

            model.Courses = GetCourseSummaries(courses, user, semester);

            return PartialView("_CourseListFit", model.Courses);
        }



        public ActionResult CapacityGroup(Guid semId, Guid groupId)
        {
            var semester = SemesterService.GetSemester(semId);
            var group = Db.CapacityGroups.SingleOrDefault(x => x.Id == groupId);
            var user = AppUser;

            var model = new PlanerGroupViewModel();
            model.Semester = semester;
            model.Curriculum = group.CurriculumGroup.Curriculum;
            model.Organiser = group.CurriculumGroup.Curriculum.Organiser;
            model.CapacityGroup = group;

            // Kursliste
            var courses = Db.Activities.OfType<Course>().Where(x => x.SemesterGroups.Any(y => y.Semester.Id == semester.Id && y.CapacityGroup.Id == group.Id))
                .ToList();

            model.Courses = GetCourseSummaries(courses, user, semester);

            // die Liste aller Semester, in denen es Semestergruppen gab, gibt oder geben wird
            var allSemester = Db.Semesters
                .Where(x => x.Groups.Any(g => g.CapacityGroup.Id == groupId))
                .OrderBy(x => x.StartCourses).ToList();
            // nur die aktiven!
            if (AppUser.MemberState != MemberState.Staff)
            {
                allSemester = allSemester.Where(x => x.Groups.Any(g => g.IsAvailable)).ToList();
            }

            // ist das angefagte dabei?
            if (!allSemester.Contains(semester))
            {
                allSemester.Add(semester);
                allSemester = allSemester.OrderBy(x => x.StartCourses).ToList();
            }

            ViewBag.Semesters = allSemester
                .Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString(),
                    Selected = (x.Id == semester.Id)
                });



            return View(model);
        }

        [HttpPost]
        public PartialViewResult CapacityGroupList(Guid semId, Guid groupId)
        {
            var semester = SemesterService.GetSemester(semId);
            var group = Db.CapacityGroups.SingleOrDefault(x => x.Id == groupId);
            var user = AppUser;

            var model = new PlanerGroupViewModel();
            model.Semester = semester;
            model.Curriculum = group.CurriculumGroup.Curriculum;
            model.Organiser = group.CurriculumGroup.Curriculum.Organiser;
            model.CapacityGroup = group;

            // Kursliste
            var courses = Db.Activities.OfType<Course>()
                .Where(x => x.SemesterGroups.Any(y => y.Semester.Id == semester.Id && y.CapacityGroup.Id == group.Id))
                .ToList();

            model.Courses = GetCourseSummaries(courses, user, semester);

            return PartialView("_CourseListFit", model.Courses);
        }



        public ActionResult Groups(Guid semId, Guid currId)
        {
            var semester = SemesterService.GetSemester(semId);
            var currciculum = GetCurriculum(currId);
            var user = AppUser;

            var model = new PlanerGroupViewModel();
            model.Semester = semester;
            model.Curriculum = currciculum;
            model.Organiser = currciculum.Organiser;

            return View(model);

        }


        public ActionResult Faculty(Guid orgId, Guid semId)
        {
            var org = GetOrganiser(orgId);
            var sem = SemesterService.GetSemester(semId);

            var model = new PlanerGroupViewModel();

            model.Semester = sem;
            model.Organiser = org;


            return View(model);
        }

        public ActionResult Search(Guid id)
        {

            var semester = SemesterService.GetSemester(id);
            var user = AppUser;

            var student = StudentService.GetCurrentStudent(user);


            var wd = new List<SelectionHelper>
            {
                new SelectionHelper {Text = "Montag", Value = 1},
                new SelectionHelper {Text = "Dienstag", Value = 2},
                new SelectionHelper {Text = "Mittwoch", Value = 3},
                new SelectionHelper {Text = "Donnerstag", Value = 4},
                new SelectionHelper {Text = "Freitag", Value = 5},
                new SelectionHelper {Text = "Samstag", Value = 6}
            };

            ViewBag.WeekDays = new SelectList(wd, "Value", "Text", "Montag");

            var n = SemesterService.GetSemesterIndex(student.FirstSemester);

            var op = new List<SelectionHelper>
            {
                new SelectionHelper {Text = $"Nur in meinem aktuellen Semester ({n}. Semester)", Value = 1},
                new SelectionHelper {Text =
                    $"In meinem Studiengang ({student.Curriculum.ShortName})", Value = 2},
                new SelectionHelper {Text =
                    $"Im gesamten Angebot meiner Fakultät ({student.Curriculum.Organiser.ShortName})", Value = 3},
                new SelectionHelper {Text = "Im gesamten Angebot aller Fakultäten", Value = 4}
            };

            ViewBag.Options = new SelectList(op, "Value", "Text");



            var model = new CourseSearchModel
            {
                DayOfWeek = 1,
                NewBegin = "08:00",
                NewEnd = "12:00",
                Option = 1,
                Semester = semester
            };



            return View(model);
        }

        [HttpPost]
        public PartialViewResult Search(Guid semId, int day, string from, string to, int radius)
        {
            var org = GetMyOrganisation();

            DateTime start = DateTime.Parse(from);
            DateTime end = DateTime.Parse(to);
            DayOfWeek dx = (DayOfWeek)day;

            var semester = SemesterService.GetSemester(semId);
            var user = AppUser;
            var student = StudentService.GetCurrentStudent(user);



            var courseService = new CourseService(Db);

            // Ermittlung der Semestergruppe
            // das Fachsemester
            var n = SemesterService.GetSemesterIndex(student.FirstSemester);


            List<Course> courses = null;

            if (radius == 1)        // nur in meiner Semestergruppe
            {
                courses = Db.Activities.OfType<Course>().Where(x =>
                        x.SemesterGroups.Any(g =>
                            g.Semester.Id == semId &&
                            g.CapacityGroup.CurriculumGroup.Curriculum.Id == student.Curriculum.Id &&
                            g.CapacityGroup.CurriculumGroup.Name.StartsWith(n.ToString())))
                    .ToList();
            }
            else if (radius == 2)   // nur in meinem Studiengang
            {
                courses = Db.Activities.OfType<Course>().Where(x =>
                        x.SemesterGroups.Any(g =>
                            g.Semester.Id == semId && 
                            g.CapacityGroup.CurriculumGroup.Curriculum.Id == student.Curriculum.Id))
                    .ToList();
            }
            else if (radius == 3)   // gesamt Fakultät
            {
                courses = Db.Activities.OfType<Course>().Where(x =>
                        x.SemesterGroups.Any(g =>
                            g.Semester.Id == semId && g.CapacityGroup.CurriculumGroup.Curriculum.Organiser.Id == org.Id))
                    .ToList();
            }
            else                    // komplettes Programm
            {
                courses = Db.Activities.OfType<Course>().Where(x =>
                        x.SemesterGroups.Any(g =>
                            g.Semester.Id == semId))
                    .ToList();
            }


            var coursesInTime = 
                courses.Where(x =>
                    x.Dates.Any(d => d.Begin.DayOfWeek == dx && start.TimeOfDay <= d.Begin.TimeOfDay && d.End.TimeOfDay <= end.TimeOfDay)).ToList();

            // da bin ich eingetragen
            var activities = Db.Activities.OfType<Course>().Where(a =>
                a.SemesterGroups.Any(g => g.Semester.Id == semester.Id) &&
                a.Occurrence.Subscriptions.Any(u => u.UserId.Equals(user.Id))).ToList();


            var model = new List<CourseSummaryModel>();

            foreach (var course in coursesInTime)
            {
                var summary = courseService.GetCourseSummary(course);
                summary.SemesterGroup = course.SemesterGroups.FirstOrDefault();
                summary.State = ActivityService.GetActivityState(course.Occurrence, user);
                summary.Summary = new ActivitySummary(course);
                summary.Lottery =
                    Db.Lotteries.FirstOrDefault(x => x.Occurrences.Any(y => y.Id == course.Occurrence.Id));

                summary.FittingDates = course.Dates.Where(d =>
                        d.Begin.DayOfWeek == dx && start.TimeOfDay <= d.Begin.TimeOfDay &&
                        d.End.TimeOfDay <= end.TimeOfDay)
                    .ToList();

                summary.NonFittingDates = course.Dates.Where(d =>
                        !(d.Begin.DayOfWeek == dx && start.TimeOfDay <= d.Begin.TimeOfDay &&
                        d.End.TimeOfDay <= end.TimeOfDay))
                    .ToList();

                summary.ConflictingDates = new Dictionary<ActivityDate, List<ActivityDate>>();

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
                            summary.ConflictingDates[date]  = new List<ActivityDate>();

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


                model.Add(summary);
            }


            return PartialView("_CourseListFit", model);
        }



        /// <summary>
        /// Anzeige der Stundenplanung
        /// Es werden nur die Fakultäten und Studiengänge angezeigt, die über aktive 
        /// Studiengänge verfügen
        /// </summary>
        /// <returns></returns>
        public ActionResult Dictionary()
        {
            // das aktuelle Semester
            var semester = SemesterService.GetSemester(DateTime.Today);
            var myOrg = GetMyOrganisation();


            var semService = new SemesterService(Db);
            var semSubService = new SemesterSubscriptionService(Db);

            var semGroup = semSubService.GetSemesterGroup(AppUser.Id, semester);

            // Zeig alles an!
            // Alle Fakultäten, die aktive Semestergruppen haben
            /*
            var acticeorgs = Db.Organisers.Where(x => 
                x.IsFaculty && 
                x.Activities.Any(a =>a.SemesterGroups.Any(s => s.Semester.EndCourses >= DateTime.Today && s.IsAvailable)))
            .ToList();
            */

            var allOrgs = Db.Organisers.ToList();
            var acticeorgs = new List<ActivityOrganiser>();
            foreach (var org in allOrgs)
            {
                var hasActiveCourses = 
                Db.Activities.OfType<Course>().Any(x => x.SemesterGroups.Any(s =>
                    s.Semester.EndCourses >= DateTime.Today && s.IsAvailable &&
                    s.CapacityGroup.CurriculumGroup.Curriculum.Organiser.Id == org.Id));

                if (hasActiveCourses)
                    acticeorgs.Add(org);
            }


            

            // Dieses Modell ist für die Selektion in den Auswahllisten verantwortlich
            var model = new GroupSelectionViewModel
            {
                Faculty = myOrg.Id.ToString(),
                Curriculum = semGroup?.CapacityGroup.CurriculumGroup.Curriculum.Id.ToString() ?? string.Empty,
                Group = semGroup?.Id.ToString() ?? string.Empty,
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

            var activecurr = semService.GetActiveCurricula(selectedOrg, semester, true).OrderBy(f => f.Name).ThenBy(f => f.ShortName);

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
        public ActionResult Courses(Guid? id)
        {
            var semester = SemesterService.GetSemester(id);
            var user = AppUser;


            var model = new DashboardStudentViewModel();

            model.Semester = semester;
            model.User = user;
            model.Student = StudentService.GetCurrentStudent(user);

            if (model.Student != null)
            {
                model.Organiser = model.Student.Curriculum.Organiser;
            }
            else
            {
                model.Organiser = GetMyOrganisation();
            }

            var courses = Db.Activities.OfType<Course>().Where(a => 
                a.SemesterGroups.Any(g => g.Semester.Id == semester.Id) &&
                a.Occurrence.Subscriptions.Any(u => u.UserId.Equals(model.User.Id))).ToList();
            model.Courses = GetCourseSummaries(courses, user, semester);

            return View(model);
        }


        public ActionResult List(Guid? id)
        {
            var semSubService = new SemesterSubscriptionService();

            var semester = SemesterService.GetSemester(id);
            var user = AppUser;


            var model = new DashboardStudentViewModel();

            model.Semester = semester;
            model.User = user;
            model.SemesterGroup = semSubService.GetSemesterGroup(model.User.Id, semester);



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
                    State = ActivityService.GetActivityState(activity.Occurrence, user),
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
        public ActionResult OfficeHour(Guid? id)
        {
            var semSubService = new SemesterSubscriptionService();

            var semester = SemesterService.GetSemester(id);
            var user = AppUser;


            var model = new List<OfficeHourSubscriptionViewModel>();

            ViewBag.Semester = semester;


            var activities = Db.Activities.OfType<OfficeHour>().Where(a =>
                a.Dates.Any(d => d.Occurrence.Subscriptions.Any(u => u.UserId.Equals(user.Id)) ||
                d.Slots.Any(s => s.Occurrence.Subscriptions.Any(u => u.UserId.Equals(user.Id))))).ToList();

            foreach (var activity in activities)
            {
                // Reparatur
                OrganiserMember host = null;
                if (activity.Owners.Any())
                {
                    host = activity.Owners.First().Member;
                }
                else
                {
                    if (activity.Dates.Any() && activity.Dates.First().Hosts.Any())
                    {
                        host = activity.Dates.First().Hosts.First();
                    }
                }

                var actModel = new OfficeHourSubscriptionViewModel
                {
                    Host = host
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
        public ActionResult Events(Guid? id)
        {
            var semSubService = new SemesterSubscriptionService();

            var semester = SemesterService.GetSemester(id);
            var user = AppUser;


            var model = new DashboardStudentViewModel();

            model.Semester = semester;
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
                    State = ActivityService.GetActivityState(activity.Occurrence, user),
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
            var user = AppUser;
            var courseService = new CourseService(Db);

            var semGroup = Db.SemesterGroups.SingleOrDefault(g => g.Id == semGroupId);
            if (semGroup == null)
            {
                var model = new List<CourseSummaryModel>();
                return PartialView("_CourseList", model);
            }


            var allTopics = Db.SemesterTopics.Where(x => x.Activities.Any(s => s.SemesterGroups.Any(g => g.Id == semGroupId))).ToList();
            var semester = semGroup.Semester;

            if (allTopics.Any())
            {
                var model = new List<TopicSummaryModel>();

                foreach (var topic in allTopics)
                {
                    var courses = topic.Activities.OfType<Course>().ToList();

                    var model2 = new List<CourseSummaryModel>();


                    foreach (var course in courses)
                    {
                        var summary = courseService.GetCourseSummary(course);

                        summary.State = ActivityService.GetActivityState(course.Occurrence, user);
                        summary.SemesterGroup = semGroup;
                        summary.Summary = new ActivitySummary(course);

                        summary.Lottery =
                            Db.Lotteries.FirstOrDefault(x => x.Occurrences.Any(y => y.Id == course.Occurrence.Id));


                        model2.Add(summary);
                    }

                    model.Add(new TopicSummaryModel
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
                        var summary = courseService.GetCourseSummary(course);

                        summary.State = ActivityService.GetActivityState(course.Occurrence, user);
                        summary.SemesterGroup = semGroup;
                        summary.Summary = new ActivitySummary(course);

                        summary.Lottery =
                            Db.Lotteries.FirstOrDefault(x => x.Occurrences.Any(y => y.Id == course.Occurrence.Id));


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
                var courses = semGroup.Activities.OfType<Course>().ToList();

                var model = new List<CourseSummaryModel>();

                foreach (var course in courses)
                {
                    var summary = courseService.GetCourseSummary(course);

                    summary.State = ActivityService.GetActivityState(course.Occurrence, user);
                    summary.SemesterGroup = semGroup;
                    summary.Summary = new ActivitySummary(course);

                    summary.Lottery =
                        Db.Lotteries.FirstOrDefault(x => x.Occurrences.Any(y => y.Id == course.Occurrence.Id));

                    model.Add(summary);
                }

                return PartialView("_CourseList", model);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="orgId"></param>
        /// <param name="semId"></param>
        /// <returns></returns>
        [HttpPost]
        public PartialViewResult CourseListByFaculty(Guid orgId, Guid semId)
        {
            var semester = SemesterService.GetSemester(semId);
            var user = AppUser;
            var courseService = new CourseService(Db);

            var model = new List<CourseSummaryModel>();

            var org = GetOrganiser(orgId);
            foreach (var curriculum in org.Curricula)
            {
                var groups = Db.SemesterGroups
                    .Where(g => g.Semester.Id == semId &&
                                g.CapacityGroup.CurriculumGroup.Curriculum.Id == curriculum.Id).ToList();

                foreach (var semesterGroup in groups)
                {
                    var courses = semesterGroup.Activities.OfType<Course>().ToList();

                    foreach (var course in courses)
                    {
                        var summary = courseService.GetCourseSummary(course);

                        summary.State = ActivityService.GetActivityState(course.Occurrence, user);
                        summary.SemesterGroup = semesterGroup;
                        summary.Summary = new ActivitySummary(course);

                        summary.Lottery =
                            Db.Lotteries.FirstOrDefault(x => x.Occurrences.Any(y => y.Id == course.Occurrence.Id));



                        model.Add(summary);
                    }
                }
            }


            return PartialView("_CourseListComplete", model);
        }



        private ICollection<CourseSummaryModel> GetCourseSummaries(ICollection<Course> courses, ApplicationUser user, Semester semester)
        {
            var courseService = new CourseService(Db);

            var activities = Db.Activities.OfType<Course>().Where(a =>
                a.SemesterGroups.Any(g => g.Semester.Id == semester.Id) &&
                a.Occurrence.Subscriptions.Any(u => u.UserId.Equals(user.Id))).ToList();


            var model = new List<CourseSummaryModel>();

            foreach (var course in courses)
            {
                var summary = courseService.GetCourseSummary(course);
                summary.SemesterGroup = course.SemesterGroups.FirstOrDefault();
                summary.State = ActivityService.GetActivityState(course.Occurrence, user);
                summary.Subscription = summary.State.Subscription;
                summary.User = user;

                //summary.Summary = new ActivitySummary(course);
                summary.Lottery =
                    Db.Lotteries.FirstOrDefault(x => x.Occurrences.Any(y => y.Id == course.Occurrence.Id));

                /*
                 * Spezielles Suchergebnis!
                summary.FittingDates = course.Dates.Where(d =>
                        d.Begin.DayOfWeek == dx && start.TimeOfDay <= d.Begin.TimeOfDay &&
                        d.End.TimeOfDay <= end.TimeOfDay)
                    .ToList();

                summary.NonFittingDates = course.Dates.Where(d =>
                        !(d.Begin.DayOfWeek == dx && start.TimeOfDay <= d.Begin.TimeOfDay &&
                        d.End.TimeOfDay <= end.TimeOfDay))
                    .ToList();
                */

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


                model.Add(summary);
            }

            return model;
        }


        public ActionResult Semester()
        {
            var user = GetCurrentUser();
            var allMySemester = 
            Db.Semesters.Where(x =>
                    x.Groups.Any(g =>
                        g.Activities.Any(a => a.Occurrence.Subscriptions.Any(s => s.UserId.Equals(user.Id)))))
                .OrderByDescending(x => x.StartCourses)
                .ToList();

            var student = StudentService.GetCurrentStudent(user);

            var model = new StudentPlanerModel();
            model.User = user;
            model.Student = student;

            // wenn es kein Student ist, dann nur eine Seite mit Link auf Startseite => Wahl des Studiengangs
            if (student != null)
            {
                var latestSemester = SemesterService.GetLatestSemester(student.Curriculum.Organiser);
                // das aktuelle Semester als Zusatz setzen
                model.LatestSemester = latestSemester;

                foreach (var semester in allMySemester)
                {
                    var courses = Db.Activities.OfType<Course>().Where(a =>
                        a.SemesterGroups.Any(g => g.Semester.Id == semester.Id) &&
                        a.Occurrence.Subscriptions.Any(u => u.UserId.Equals(model.User.Id))).ToList();

                    var semModel = new StudentSemesterPlanerModel();
                    semModel.Semester = semester;
                    semModel.Courses.AddRange(courses);

                    model.Semester.Add(semModel);

                    if (semester.Id == latestSemester.Id)
                    {
                        model.LatestSemester = null;
                    }
                }
            }


            return View(model);
        }

        public ActionResult Schedule(Guid? id)
        {
            var user = GetCurrentUser();
            var org = GetMyOrganisation();
            var semester = SemesterService.GetSemester(id);

            var model = new SemesterActivityModel();

            model.Semester = semester;
            model.MyCourses.AddRange(GetLecturerCourses(semester, user));
            model.Organiser = org;

            return View(model);
        }

        private List<ActivitySummary> GetLecturerCourses(Semester semester, ApplicationUser user)
        {
            List<ActivitySummary> model = new List<ActivitySummary>();

            var lectureActivities =
                Db.Activities.OfType<Course>().Where(a =>
                    a.SemesterGroups.Any(g => g.Semester.Id == semester.Id) &&
                    a.Dates.Any(d => d.Hosts.Any(l => !string.IsNullOrEmpty(l.UserId) && l.UserId.Equals(user.Id)))).ToList();

            foreach (var activity in lectureActivities)
            {
                var summary = new ActivitySummary { Activity = activity };

                // nur die, bei denen es noch Termine in der Zukunft gibt
                if (activity.Dates.Any(x => x.End >= DateTime.Now))
                {
                    var currentDate =
                        activity.Dates.Where(d => d.Begin <= DateTime.Now && DateTime.Now <= d.End)
                            .OrderBy(d => d.Begin)
                            .FirstOrDefault();
                    var nextDate =
                        activity.Dates.Where(d => d.Begin >= DateTime.Now)
                            .OrderBy(d => d.Begin)
                            .FirstOrDefault();

                    if (currentDate != null)
                    {
                        summary.CurrentDate = new CourseDateStateModel
                        {
                            Summary = new ActivityDateSummary { Date = currentDate },
                            State = ActivityService.GetSubscriptionState(currentDate.Occurrence, currentDate.Begin, currentDate.End),
                        };
                    }

                    if (nextDate != null)
                    {
                        summary.NextDate = new CourseDateStateModel
                        {
                            Summary = new ActivityDateSummary { Date = nextDate },
                            State = ActivityService.GetSubscriptionState(nextDate.Occurrence, nextDate.Begin, nextDate.End),
                        };
                    }
                }
                model.Add(summary);
            }

            return model;
        }

    }
}
