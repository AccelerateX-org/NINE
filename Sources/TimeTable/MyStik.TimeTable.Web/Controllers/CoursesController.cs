﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
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
        public ActionResult Index()
        {
            var semester = SemesterService.GetSemester(DateTime.Today);
            var org = GetMyOrganisation();

            var model = new OrganiserViewModel
            {
                Semester = semester,
                Organiser = org,
            };

            // alle Semester
            model.ActiveSemesters.AddRange(
                Db.Semesters
                    .Where(x => x.Groups.Any(g => g.CapacityGroup.CurriculumGroup.Curriculum.Organiser.Id == org.Id))
                    .ToList());

            ViewBag.UserRight = GetUserRight();

            return View(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Semester(Guid id)
        {
            var semester = SemesterService.GetSemester(id);
            var org = GetMyOrganisation();

            var model = new OrganiserViewModel
            {
                Semester = semester,
                Organiser = org,
            };

            var lastEnd = DateTime.Today.AddDays(-90);
            var alLotteries = Db.Lotteries.Where(x =>
                x.LastDrawing >= lastEnd && x.IsAvailable &&
                x.Organiser != null && x.Organiser.Id == org.Id).OrderBy(x => x.FirstDrawing).ToList();

            model.ActiveLotteries.AddRange(alLotteries);


            ViewBag.UserRight = GetUserRight();

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

            return RedirectToAction("Admin", new {id = semester.Id});
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

            return RedirectToAction("Admin", new {id = semester.Id});
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


            return RedirectToAction("Groups", new {id = id});
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

            return RedirectToAction("Admin", new {id = semester.Id});
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

            return RedirectToAction("Admin", new {id = semester.Id});
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

            return RedirectToAction("Admin", new {id = semester.Id});
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

            return RedirectToAction("Admin", new {id = semester.Id});
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public FileResult SemesterReport(Guid? id)
        {
            var semester = SemesterService.GetSemester(id);
            var org = GetMyOrganisation();

            var model = CreateSemesterReport(semester, org);

            var ms = new MemoryStream();
            var writer = new StreamWriter(ms, Encoding.Default);


            writer.Write("Studiengang;Gruppe;Dozent;Kurzname;Name;Eintragungen");
            writer.Write(Environment.NewLine);


            foreach (var course in model)
            {

                writer.Write("{0};{1};{2};{3};{4};{5}",
                    course.Curriculum.ShortName, course.Group.FullName,
                    course.Lecturer.Name,
                    course.Course.ShortName,
                    course.Course.Name,
                    course.Course.Occurrence.Subscriptions.Count);
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


        private List<SemesterCourseViewModel> CreateSemesterReport(Semester semester, ActivityOrganiser org)
        {

            var model = new List<SemesterCourseViewModel>();

            // Alle Lehrveranstaltungen in diesem Semester
            var courses = Db.Activities.OfType<Course>().Where(x => x.SemesterGroups.Any(s =>
                    s.Semester.Id == semester.Id && s.CapacityGroup.CurriculumGroup.Curriculum.Organiser.Id == org.Id))
                .ToList();

            // für jede Lehrveranstaltung alle Dozenten
            foreach (var course in courses)
            {
                // Alle Dozenten in dieser LV
                var lectures =
                    Db.Members.Where(l => l.Dates.Any(occ => occ.Activity.Id == course.Id)).ToList();


                // Für jede Semestergruppe
                foreach (var semesterGroup in course.SemesterGroups)
                {

                    foreach (var lecture in lectures)
                    {
                        var courseModel = new SemesterCourseViewModel
                        {
                            Course = course,
                            Curriculum = semesterGroup.CapacityGroup.CurriculumGroup.Curriculum,
                            Lecturer = lecture,
                            Group = semesterGroup
                        };

                        model.Add(courseModel);

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
                FreezedCourses = Db.Activities.OfType<Course>().Count(x => x.SemesterGroups.Any(g => g.Semester.Id == semester.Id) && x.IsInternal),
                UnFreezedCourses = Db.Activities.OfType<Course>().Count(x => x.SemesterGroups.Any(g => g.Semester.Id == semester.Id) && !x.IsInternal),
                LockedCourses = Db.Activities.OfType<Course>().Count(x => x.SemesterGroups.Any(g => g.Semester.Id == semester.Id) && !x.Occurrence.IsAvailable),
                UnLockedCourses = Db.Activities.OfType<Course>().Count(x => x.SemesterGroups.Any(g => g.Semester.Id == semester.Id) && x.Occurrence.IsAvailable)
            };

            return View(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult CreateSemester()
        {
            var org = GetMyOrganisation();
            var nextSemester = SemesterService.GetNextSemester(DateTime.Today);


            return RedirectToAction("InitGroups", new {id = nextSemester.Id});
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

            var courses = Db.Activities.OfType<Course>().Where(x => x.Owners.Any(y => y.Member.Organiser.Id == org.Id)).ToList();

            foreach (var course in courses)
            {
                var summary = courseService.GetCourseSummary(course);
                model.Courses.Add(summary);
            }


            return View(model);
        }
    }
}
