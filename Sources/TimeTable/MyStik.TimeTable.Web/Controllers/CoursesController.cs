using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
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
    public class CoursesController : BaseController
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Index(Guid id)
        {
            var org = GetOrganisation(id);

            var model = new OrganiserViewModel
            {
                Organiser = org,
            };

            // alle Semester
            model.ActiveSemesters.AddRange(
                Db.Semesters
                    .Where(x => x.Groups.Any(g => g.CapacityGroup.CurriculumGroup.Curriculum.Organiser.Id == org.Id))
                    .ToList());

            ViewBag.UserRight = GetUserRight(org);

            return View(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AllowAnonymous]
        public ActionResult Semester(Guid? orgId, Guid? semId)
        {
            var semester = SemesterService.GetSemester(DateTime.Today);
            var org = orgId == null ? GetMyOrganisation() : GetOrganiser(orgId.Value);

            if (orgId.HasValue && semId.HasValue)
            {
                semester = SemesterService.GetSemester(semId.Value);
                org = GetOrganiser(orgId.Value);
            }

            var model = new OrganiserViewModel
            {
                Semester = semester,
                Organiser = org,
            };

            model.PreviousSemester = SemesterService.GetPreviousSemester(semester);
            model.NextSemester = SemesterService.GetNextSemester(semester);

            var lastEnd = DateTime.Today.AddDays(-90);
            var alLotteries = Db.Lotteries.Where(x =>
                x.LastDrawing >= lastEnd && x.IsAvailable &&
                x.Organiser != null && x.Organiser.Id == org.Id).OrderBy(x => x.FirstDrawing).ToList();

            model.ActiveLotteries.AddRange(alLotteries);

            var courses =
                Db.Activities.OfType<Course>().Where(c =>
                        c.Semester.Id == semester.Id &&
                        c.Organiser.Id == org.Id)
                    .OrderBy(c => c.ShortName).ToList();

            var courseService = new CourseService(Db);

            foreach (var course in courses)
            {
                var summary = courseService.GetCourseSummary(course);
                model.Courses.Add(summary);
            }

            ViewBag.UserRight = GetUserRight(org);

            return View(model);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Groups(Guid? id)
        {
            var organiser = GetMyOrganisation();
            if (organiser == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var semester = SemesterService.GetSemester(id);

            var model = new OrganiserViewModel
            {
                Organiser = organiser,
                Semester = semester
            };

            // hier jetzt das ganze zu Fuss
            var studentService = new StudentService(Db);

            var groups = semester.Groups
                .Where(g => g.CapacityGroup.CurriculumGroup.Curriculum.Organiser.Id == organiser.Id)
                .OrderBy(g => g.CapacityGroup.CurriculumGroup.Curriculum.Name)
                .ThenBy(g => g.CapacityGroup.CurriculumGroup.Name).ThenBy(g => g.CapacityGroup.Name).ToList();
            foreach (var group in groups)
            {
                var groupModel = new SemesterGroupViewModel
                {
                    Group = group,
                    UserIds = studentService.GetStudents(group)
                };

                model.SemesterGroups.Add(groupModel);
            }

            model.Groups = groups.GroupBy(x => x.CapacityGroup.CurriculumGroup.Curriculum).ToList();

            ViewBag.UserRight = GetUserRight();

            return View("GroupsNew", model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult List(Guid? id)
        {
            var organiser = GetMyOrganisation();
            if (organiser == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var userRight = GetUserRight(User.Identity.Name, organiser.ShortName);
            if (!userRight.IsOrgMember)
                return RedirectToAction("NoMember");

            var semester = SemesterService.GetSemester(id);

            var model = new OrganiserViewModel
            {
                Organiser = organiser,
                Semester = semester
            };

            var courses =
                Db.Activities.OfType<Course>().Where(c =>
                    c.SemesterGroups.Any(g =>
                        g.Semester.Id == semester.Id &&
                        g.CapacityGroup.CurriculumGroup.Curriculum.Organiser.Id == organiser.Id)
                ).OrderBy(c => c.ShortName).ToList();

            var courseService = new CourseService(Db);

            foreach (var course in courses)
            {
                var summary = courseService.GetCourseSummary(course);
                model.Courses.Add(summary);
            }

            // Benutzerrechte
            ViewBag.UserRight = userRight;

            return View(model);

        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult NoGroups()
        {
            var organiser = GetMyOrganisation();
            if (organiser == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var userRight = GetUserRight(User.Identity.Name, organiser.ShortName);
            if (!userRight.IsOrgMember)
                return RedirectToAction("NoMember");

            var semester = SemesterService.GetSemester(DateTime.Today);

            var model = new OrganiserViewModel
            {
                Organiser = organiser,
                Semester = semester
            };

            var courses =
                Db.Activities.OfType<Course>().Where(c =>
                    c.Organiser.Id == organiser.Id &&
                    !c.SemesterGroups.Any()
                ).OrderBy(c => c.ShortName).ToList();

            var courseService = new CourseService(Db);

            foreach (var course in courses)
            {
                var summary = courseService.GetCourseSummary(course);
                model.Courses.Add(summary);
            }

            // Benutzerrechte
            ViewBag.UserRight = userRight;

            return View(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Enable(Guid id)
        {
            var semester = SemesterService.GetSemester(id);
            var org = GetMyOrganisation();

            var allGroups =
                Db.SemesterGroups
                    .Where(x =>
                        x.Semester.Id == id &&
                        x.CapacityGroup.CurriculumGroup.Curriculum.Organiser.Id == org.Id)
                    .ToList();

            foreach (var semesterGroup in allGroups)
            {
                semesterGroup.IsAvailable = true;
            }

            Db.SaveChanges();

            return RedirectToAction("Admin", new { id = semester.Id });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Disable(Guid id)
        {
            var semester = SemesterService.GetSemester(id);
            var org = GetMyOrganisation();

            var allGroups =
                Db.SemesterGroups
                    .Where(x =>
                        x.Semester.Id == id &&
                        x.CapacityGroup.CurriculumGroup.Curriculum.Organiser.Id == org.Id)
                    .ToList();

            foreach (var semesterGroup in allGroups)
            {
                semesterGroup.IsAvailable = false;
            }

            Db.SaveChanges();

            return RedirectToAction("Admin", new { id = semester.Id });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult InitGroups(Guid id)
        {
            var semester = SemesterService.GetSemester(id);
            var org = GetMyOrganisation();

            if (semester == null)
                return RedirectToAction("Index");

            // Alle Curricula durchgehen
            foreach (var curriculum in Db.Curricula.Where(x => x.Organiser.Id == org.Id).ToList())
            {
                foreach (var curriculumGroup in curriculum.CurriculumGroups.ToList())
                {
                    foreach (var capacityGroup in curriculumGroup.CapacityGroups.ToList())
                    {
                        var exist = semester.Groups.Any(g => g.CapacityGroup.Id == capacityGroup.Id);

                        if (!exist)
                        {
                            var semGroup = new SemesterGroup
                            {
                                CapacityGroup = capacityGroup,
                                CurriculumGroup = capacityGroup.CurriculumGroup, // nur noch aus Gründen der Sicherheit
                                Semester = semester
                            };

                            semester.Groups.Add(semGroup);
                            Db.SemesterGroups.Add(semGroup);
                        }
                    }
                }
            }

            Db.SaveChanges();


            return RedirectToAction("AdminGroups", new { id = id });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult DeleteCourse(Guid id)
        {
            var timeTableService = new TimeTableInfoService(Db);

            timeTableService.DeleteCourse(id);

            // Kehre zurück zur Seite der Aktivität
            return RedirectToAction("NoGroups");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult LockCourses(Guid id)
        {
            var semester = SemesterService.GetSemester(id);
            var org = GetMyOrganisation();

            var allGroups = Db.Activities.OfType<Course>().Where(x =>
                    x.SemesterGroups.Any(s =>
                        s.Semester.Id == id &&
                        s.CapacityGroup.CurriculumGroup.Curriculum.Organiser.Id == org.Id))
                .ToList();
            allGroups.ForEach(x => x.IsInternal = true);

            Db.SaveChanges();

            return RedirectToAction("Admin", new { id = semester.Id });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult UnLockCourses(Guid id)
        {
            var semester = SemesterService.GetSemester(id);
            var org = GetMyOrganisation();

            var allGroups = Db.Activities.OfType<Course>().Where(x =>
                    x.SemesterGroups.Any(s =>
                        s.Semester.Id == id &&
                        s.CapacityGroup.CurriculumGroup.Curriculum.Organiser.Id == org.Id))
                .ToList();
            allGroups.ForEach(x => x.IsInternal = false);

            Db.SaveChanges();

            return RedirectToAction("Admin", new { id = semester.Id });
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult DisableCourses(Guid id)
        {
            var semester = SemesterService.GetSemester(id);
            var org = GetMyOrganisation();

            var allGroups = Db.Activities.OfType<Course>().Where(x =>
                    x.SemesterGroups.Any(s =>
                        s.Semester.Id == id &&
                        s.CapacityGroup.CurriculumGroup.Curriculum.Organiser.Id == org.Id))
                .ToList();
            allGroups.ForEach(x => x.Occurrence.IsAvailable = false);

            Db.SaveChanges();

            return RedirectToAction("Admin", new { id = semester.Id });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult EnableCourses(Guid id)
        {
            var semester = SemesterService.GetSemester(id);
            var org = GetMyOrganisation();

            var allGroups = Db.Activities.OfType<Course>().Where(x =>
                    x.SemesterGroups.Any(s =>
                        s.Semester.Id == id &&
                        s.CapacityGroup.CurriculumGroup.Curriculum.Organiser.Id == org.Id))
                .ToList();
            allGroups.ForEach(x => x.Occurrence.IsAvailable = true);

            Db.SaveChanges();

            return RedirectToAction("Admin", new { id = semester.Id });
        }



        public ActionResult Reports(Guid id)
        {
            var semester = SemesterService.GetSemester(id);
            var org = GetMyOrganisation();

            var model = new OrganiserViewModel
            {
                Semester = semester,
                Organiser = org,
            };


            return View(model);
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public FileResult SemesterReport(Guid? semId, Guid? orgId)
        {
            var semester = SemesterService.GetSemester(semId);
            var org = orgId == null ? GetMyOrganisation() : GetOrganiser(orgId.Value);

            var model = CreateSemesterReport(semester, org);

            var ms = new MemoryStream();
            var writer = new StreamWriter(ms, Encoding.Default);


            writer.Write("Studienangebot;Gruppe;Kürzel;Vorname;Nachname;E-Mail;Kurzname;Titel;Eintragungen");
            writer.Write(Environment.NewLine);


            foreach (var course in model)
            {
                if (course.User != null)
                {
                    writer.Write("{0};{1};{2};{3};{4};{5};{6};{7};{8}",
                        course.Curriculum.ShortName, course.Group.Name,
                        course.Lecturer.ShortName,
                        course.User.FirstName,
                        course.User.LastName,
                        course.User.Email,
                        course.Course.ShortName,
                        course.Course.Name,
                        course.Course.Occurrence.Subscriptions.Count);
                }
                else
                {
                    writer.Write("{0};{1};{2};;;;{3};{4};{5}",
                        course.Curriculum.ShortName, course.Group.Name,
                        course.Lecturer.ShortName,
                        course.Course.ShortName,
                        course.Course.Name,
                        course.Course.Occurrence.Subscriptions.Count);
                }

                writer.Write(Environment.NewLine);
            }


            writer.Flush();
            writer.Dispose();

            var sb = new StringBuilder();
            sb.Append("Lehrangebot_");
            sb.Append(semester.Name);
            sb.Append("_");
            sb.Append(DateTime.Today.ToString("yyyyMMdd"));
            sb.Append(".csv");

            return File(ms.GetBuffer(), "text/csv", sb.ToString());
        }


        public FileResult SemesterReport2(Guid? id)
        {
            var semester = SemesterService.GetSemester(id);
            var org = GetMyOrganisation();

            var model = CreateSemesterReport(semester, org);

            var ms = new MemoryStream();
            var writer = new StreamWriter(ms, Encoding.Default);


            writer.Write("Studienangebot;Gruppe;Kürzel;Vorname;Nachname;Kurzname;Titel;Datum");
            writer.Write(Environment.NewLine);


            foreach (var course in model)
            {
                if (course.Course.Dates.Any())
                {
                    foreach (var date in course.Course.Dates)
                    {
                        if (course.User != null)
                        {
                            writer.Write("{0};{1};{2};{3};{4};{5};{6};{7}",
                                course.Curriculum.ShortName, course.Group.Name,
                                course.Lecturer.ShortName,
                                course.User.FirstName,
                                course.User.LastName,
                                course.Course.ShortName,
                                course.Course.Name,
                                date.Begin.Date.ToShortDateString());
                        }
                        else
                        {
                            writer.Write("{0};{1};{2};;;{3};{4};{5}",
                                course.Curriculum.ShortName, course.Group.Name,
                                course.Lecturer.ShortName,
                                course.Course.ShortName,
                                course.Course.Name,
                                date.Begin.Date.ToShortDateString());
                        }

                        writer.Write(Environment.NewLine);
                    }
                }
                else
                {
                    if (course.User != null)
                    {
                        writer.Write("{0};{1};{2};{3};{4};{5};{6};Keine Termine",
                            course.Curriculum.ShortName, course.Group.Name,
                            course.Lecturer.ShortName,
                            course.User.FirstName,
                            course.User.LastName,
                            course.Course.ShortName,
                            course.Course.Name);
                    }
                    else
                    {
                        writer.Write("{0};{1};{2};;;{3};{4};Keine Termine",
                            course.Curriculum.ShortName, course.Group.Name,
                            course.Lecturer.ShortName,
                            course.Course.ShortName,
                            course.Course.Name);
                    }

                }
            }


            writer.Flush();
            writer.Dispose();

            var sb = new StringBuilder();
            sb.Append("Lehrangebot_");
            sb.Append(semester.Name);
            sb.Append("_");
            sb.Append(DateTime.Today.ToString("yyyyMMdd"));
            sb.Append(".csv");

            return File(ms.GetBuffer(), "text/csv", sb.ToString());
        }




        private List<SemesterCourseViewModel> CreateSemesterReport(Semester semester, ActivityOrganiser org)
        {
            var userInfoService = new UserInfoService();
            var model = new List<SemesterCourseViewModel>();

            // Alle Lehrveranstaltungen in diesem Semester
            var courses = Db.Activities.OfType<Course>().Where(x => x.Organiser.Id == org.Id &&
                                                                    x.Semester.Id == semester.Id &&
                                                                    x.LabelSet != null && x.LabelSet.ItemLabels.Any())
                .Include(activity =>
                    activity.LabelSet.ItemLabels)
                .ToList();


            // für jede Lehrveranstaltung alle Dozenten
            foreach (var course in courses)
            {
                // Alle Dozenten in dieser LV
                var lectures =
                    Db.Members.Where(l => l.Dates.Any(occ => occ.Activity.Id == course.Id)).ToList();


                // Für jede Semestergruppe
                foreach (var semesterGroup in course.LabelSet.ItemLabels)
                {
                    var curriculum =
                        org.Curricula.FirstOrDefault(x => x.LabelSet.ItemLabels.Any(l => l.Id == semesterGroup.Id));

                    if (curriculum != null)
                    {
                        foreach (var lecture in lectures)
                        {
                            var courseModel = new SemesterCourseViewModel
                            {
                                Course = course,
                                Curriculum = curriculum,
                                Lecturer = lecture,
                                User = userInfoService.GetUser(lecture.UserId),
                                Group = semesterGroup
                            };

                            model.Add(courseModel);

                        }
                    }
                }
            }

            model = model.OrderBy(x => x.Curriculum.ShortName).ThenBy(x => x.Lecturer.Name).ToList();

            return model;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Admin(Guid id)
        {
            var semester = SemesterService.GetSemester(id);
            var org = GetMyOrganisation();

            var model = new SemesterStatisticsModel
            {
                Semester = semester,
                Organiser = org,
                Courses = Db.Activities.OfType<Course>().Where(x =>
                    x.SemesterGroups.Any(g =>
                        g.Semester.Id == semester.Id &&
                        g.CapacityGroup.CurriculumGroup.Curriculum.Organiser.Id == org.Id)).ToList(),

                FreezedCourses = Db.Activities.OfType<Course>().Count(x =>
                    x.SemesterGroups.Any(g =>
                        g.Semester.Id == semester.Id &&
                        g.CapacityGroup.CurriculumGroup.Curriculum.Organiser.Id == org.Id) && x.IsInternal),

                UnFreezedCourses = Db.Activities.OfType<Course>().Count(x =>
                    x.SemesterGroups.Any(g =>
                        g.Semester.Id == semester.Id &&
                        g.CapacityGroup.CurriculumGroup.Curriculum.Organiser.Id == org.Id) && !x.IsInternal),

                LockedCourses = Db.Activities.OfType<Course>().Count(x =>
                    x.SemesterGroups.Any(g =>
                        g.Semester.Id == semester.Id &&
                        g.CapacityGroup.CurriculumGroup.Curriculum.Organiser.Id == org.Id) &&
                    !x.Occurrence.IsAvailable),

                UnLockedCourses = Db.Activities.OfType<Course>().Count(x =>
                    x.SemesterGroups.Any(g =>
                        g.Semester.Id == semester.Id &&
                        g.CapacityGroup.CurriculumGroup.Curriculum.Organiser.Id == org.Id) && x.Occurrence.IsAvailable)
            };

            return View(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult CreateSemester(Guid? id)
        {
            var org = GetMyOrganisation();

            Semester nextSemester = null;
            if (id != null)
            {
                var semester = SemesterService.GetSemester(id);
                nextSemester = SemesterService.GetNextSemester(semester);
            }
            else
            {
                nextSemester = SemesterService.GetNextSemester(DateTime.Today);
            }



            return RedirectToAction("InitGroups", new { id = nextSemester.Id });
        }

        public ActionResult CreateCurrentSemester()
        {
            var org = GetMyOrganisation();

            var nextSemester = SemesterService.GetSemester(DateTime.Today);


            return RedirectToAction("InitGroups", new { id = nextSemester.Id });
        }

        public ActionResult RemoveUnused(Guid id)
        {
            var org = GetMyOrganisation();

            var semester = SemesterService.GetSemester(id);

            var groups = semester.Groups
                .Where(g => g.CapacityGroup.CurriculumGroup.Curriculum.Organiser.Id == org.Id && !g.Activities.Any())
                .ToList();


            foreach (var semesterGroup in groups)
            {
                Db.SemesterGroups.Remove(semesterGroup);

            }

            Db.SaveChanges();



            return RedirectToAction("AdminGroups", new { id = id });
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Schedule(Guid id)
        {
            var organiser = GetMyOrganisation();
            if (organiser == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var semester = SemesterService.GetSemester(id);

            var model = new OrganiserViewModel
            {
                Organiser = organiser,
                Semester = semester
            };


            var groups = semester.Groups
                .Where(g => g.CapacityGroup.CurriculumGroup.Curriculum.Organiser.Id == organiser.Id)
                .OrderBy(g => g.CapacityGroup.CurriculumGroup.Curriculum.Name)
                .ThenBy(g => g.CapacityGroup.CurriculumGroup.Name).ThenBy(g => g.CapacityGroup.Name).ToList();
            foreach (var group in groups)
            {
                var groupModel = new SemesterGroupViewModel
                {
                    Group = group,
                };

                model.SemesterGroups.Add(groupModel);
            }

            ViewBag.UserRight = GetUserRight();

            return View(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Calendar(Guid id)
        {
            var organiser = GetMyOrganisation();
            if (organiser == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var semester = SemesterService.GetSemester(id);

            var model = new OrganiserViewModel
            {
                Organiser = organiser,
                Semester = semester
            };


            var groups = semester.Groups
                .Where(g => g.CapacityGroup.CurriculumGroup.Curriculum.Organiser.Id == organiser.Id)
                .OrderBy(g => g.CapacityGroup.CurriculumGroup.Curriculum.Name)
                .ThenBy(g => g.CapacityGroup.CurriculumGroup.Name).ThenBy(g => g.CapacityGroup.Name).ToList();
            foreach (var group in groups)
            {
                var groupModel = new SemesterGroupViewModel
                {
                    Group = group,
                };

                model.SemesterGroups.Add(groupModel);
            }

            ViewBag.UserRight = GetUserRight();

            return View(model);
        }

        public ActionResult Analyse()
        {
            var org = GetMyOrganisation();

            var model = new OrganiserViewModel
            {
                Organiser = org,
            };

            var courseService = new CourseService(Db);

            var courses = Db.Activities.OfType<Course>().Where(x =>
                x.Owners.Any(y => y.Member.Organiser.Id == org.Id) &&
                (!x.Dates.Any() || x.Dates.Any(d => d.End >= DateTime.Today))).ToList();

            foreach (var course in courses)
            {
                var summary = courseService.GetCourseSummary(course);
                model.Courses.Add(summary);
            }


            return View(model);
        }

        public ActionResult Repair(Guid id)
        {
            var org = GetOrganisation(id);
            var userRight = GetUserRight(org);

            if (userRight.IsCurriculumAdmin)
            {
                var model = new OrganiserViewModel
                {
                    Organiser = org,
                };

                var courseService = new CourseService(Db);

                var courses = Db.Activities.OfType<Course>().Where(x =>
                    x.Owners.Any(y => y.Member.Organiser.Id == org.Id) &&
                    (!x.Dates.Any() || x.Dates.Any(d => d.End >= DateTime.Today))).ToList();

                foreach (var course in courses)
                {
                    var summary = courseService.GetCourseSummary(course);
                    model.Courses.Add(summary);
                }


                foreach (var course in model.Courses)
                {
                    if (course.Course.Semester != null)
                        continue;

                    var sem = course.Course.SemesterGroups.Select(x => x.Semester).Distinct()
                        .OrderByDescending(x => x.StartCourses).ToList();

                    if (sem.Any())
                    {
                        course.Course.Semester = sem.First();
                    }
                }

                Db.SaveChanges();
            }

            return RedirectToAction("Analyse");
        }



        public ActionResult AdminGroups(Guid id)
        {
            var organiser = GetMyOrganisation();
            if (organiser == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var semester = SemesterService.GetSemester(id);

            var model = new OrganiserViewModel
            {
                Organiser = organiser,
                Semester = semester
            };

            // hier jetzt das ganze zu Fuss
            var studentService = new StudentService(Db);

            var groups = semester.Groups
                .Where(g => g.CapacityGroup.CurriculumGroup.Curriculum.Organiser.Id == organiser.Id)
                .OrderBy(g => g.CapacityGroup.CurriculumGroup.Curriculum.Name)
                .ThenBy(g => g.CapacityGroup.CurriculumGroup.Name).ThenBy(g => g.CapacityGroup.Name).ToList();
            foreach (var group in groups)
            {
                var groupModel = new SemesterGroupViewModel
                {
                    Group = group,
                    UserIds = studentService.GetStudents(group)
                };

                model.SemesterGroups.Add(groupModel);
            }

            model.Groups = groups.GroupBy(x => x.CapacityGroup.CurriculumGroup.Curriculum).ToList();

            ViewBag.UserRight = GetUserRight();

            return View(model);
        }


        public PartialViewResult DeleteGroup(Guid id)
        {
            var group = Db.SemesterGroups.SingleOrDefault(x => x.Id == id);
            if (group != null)
            {
                Db.SemesterGroups.Remove(group);
                Db.SaveChanges();
            }

            return PartialView("_EmptyRow");
        }

        public ActionResult CopyDay()
        {
            var semester = SemesterService.GetSemester(DateTime.Today);
            var org = GetMyOrganisation();

            var model = new CopyDayViewModel
            {
                Semester = semester,
                Organiser = org,
            };

            ViewBag.Curricula = org.Curricula.OrderBy(f => f.ShortName).Select(f => new SelectListItem
            {
                Text = f.Name,
                Value = f.Id.ToString(),
            });



            return View(model);
        }

        [HttpPost]
        public ActionResult CopyDay(CopyDayViewModel model)
        {
            var semester = SemesterService.GetSemester(DateTime.Today);
            var org = GetMyOrganisation();
            var cur = org.Curricula.SingleOrDefault(x => x.Id == model.CurrId);
            var sourceDay = DateTime.Parse(model.SourceDate);
            var targetDay = DateTime.Parse(model.TargetDate);


            var report = new CopyDayReport
            {
                Curriculum = cur,
                SourceDay = sourceDay,
                TargetDay = targetDay,
                CourseReports = new List<CopyDayCourseReport>()
            };


            var begin = sourceDay;
            var end = sourceDay.AddDays(1);

            var allDates = Db.ActivityDates.Where(x =>
                x.Activity.SemesterGroups.Any(g => g.CapacityGroup.CurriculumGroup.Curriculum.Id == cur.Id) &&
                x.Begin >= begin && x.End < end
            ).ToList();


            foreach (var date in allDates)
            {
                var course = date.Activity as Course;

                if (course != null)
                {
                    // check, ob es den Termin schon gibt
                    var newBegin = targetDay.AddHours(date.Begin.Hour).AddMinutes(date.Begin.Minute);
                    var newEnd = targetDay.AddHours(date.End.Hour).AddMinutes(date.End.Minute);

                    var activityDate = course.Dates.FirstOrDefault(d => d.Begin == newBegin && d.End == newEnd);
                    var isNew = false;
                    var isMove = false;

                    if (activityDate == null)
                    {

                        if (model.Move)
                        {
                            var tempDate = new ActivityDate
                            {
                                Begin = date.Begin,
                                End = date.End,
                            };

                            date.Begin = newBegin;
                            date.End = newEnd;


                            var copyReport = new CopyDayCourseReport
                            {
                                Course = course,
                                SourceDate = tempDate,
                                TargetDate = date,
                                IsNew = false,
                                IsMove = true
                            };

                            report.CourseReports.Add(copyReport);

                        }
                        else
                        {
                            isNew = true;

                            activityDate = new ActivityDate
                            {
                                Activity = course,
                                Begin = newBegin,
                                End = newEnd,
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

                            foreach (var room in date.Rooms)
                            {
                                activityDate.Rooms.Add(room);
                            }

                            foreach (var doz in date.Hosts)
                            {
                                activityDate.Hosts.Add(doz);
                            }

                            Db.ActivityDates.Add(activityDate);

                            var copyReport = new CopyDayCourseReport
                            {
                                Course = course,
                                SourceDate = date,
                                TargetDate = activityDate,
                                IsNew = isNew
                            };

                            report.CourseReports.Add(copyReport);
                        }

                    }



                }
            }

            Db.SaveChanges();

            return View("CopyDayReport", report);
        }


        public ActionResult Responsibility(Guid id, Guid? memberId)
        {
            var user = GetCurrentUser();
            var semester = SemesterService.GetSemester(id);

            var model = new TeachingOverviewModel();
            var members = memberId == null
                ? MemberService.GetMemberships(user.Id).ToList()
                : MemberService.GetMembers(memberId.Value);

            model.Members = members;
            model.Organisers = members.Select(x => x.Organiser).Distinct().ToList();
            model.CurrentSemester = new TeachingSemesterSummaryModel();

            // das ist das der Anzeige
            model.CurrentSemester.Semester = semester;

            var prevSemester = SemesterService.GetPreviousSemester(semester);
            var yearSemester = SemesterService.GetPreviousSemester(prevSemester);

            var currentSemester = SemesterService.GetSemester(DateTime.Today);
            var nextSemester = SemesterService.GetNextSemester(currentSemester);

            ViewBag.PrevSemester = prevSemester;
            ViewBag.YearSemester = yearSemester;
            ViewBag.CurrentSemester = currentSemester;
            ViewBag.NextSemester = nextSemester;

            if (memberId != null)
            {
                ViewBag.MemberId = memberId.Value;
            }

            /*
            if (semester.StartCourses > DateTime.Today)
            {
                return View("Future", model);
            }
            else if (semester.EndCourses < DateTime.Today)
            {
                return View("History", model);
            }
            */

            return View(model);
        }
    }
}
