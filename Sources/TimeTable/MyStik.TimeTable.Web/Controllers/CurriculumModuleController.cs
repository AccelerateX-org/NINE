using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using MyStik.TimeTable.Data;
using MyStik.TimeTable.Web.Models;
using MyStik.TimeTable.Web.Services;

namespace MyStik.TimeTable.Web.Controllers
{
    public class CurriculumModuleController : BaseController
    {
        // GET: CurriculumModule
        public ActionResult Index()
        {
            var member = GetMyMembership();

            var model = Db.CurriculumModules.Where(x => x.MV.Id == member.Id).ToList();


            return View(model);
        }


        public ActionResult Create()
        {
            var org = GetMyOrganisation();


            var model = new CurriculumModuleCreateModel();


            return View(model);
        }


        [HttpPost]
        public ActionResult Create(CurriculumModuleCreateModel model)
        {
            var member = GetMyMembership();

            // TODO: eine Art "Auto-Akkreditierung???
            var curriculum = Db.Curricula.SingleOrDefault(x => x.Id == model.CurriculumId);

            var module = new CurriculumModule
            {
                
                Name = model.Name,
                ShortName = model.ShortName,
                ModuleId = model.ModuleId,
                MV = member
            };


            var primaryTechingUnit = "";
            switch (model.LectureType)
            {
                case 1:
                    primaryTechingUnit = "Seminaristischer Unterricht";
                    break;
                case 2:
                    primaryTechingUnit = "Seminar";
                    break;
                case 3:
                    primaryTechingUnit = "Vorlesung";
                    break;
            }


            var primaryCourse = new ModuleSubject()
            {
                Name = primaryTechingUnit,
                Module = module
            };
            Db.ModuleCourses.Add(primaryCourse);


            if (model.HasPractice)
            {
                var secCourse = new ModuleSubject
                {
                    Name = "Übung",
                    Module = module
                };
                Db.ModuleCourses.Add(secCourse);
            }

            if (model.HasTutorium)
            {
                var secCourse = new ModuleSubject
                {
                    Name = "Tutorium",
                    Module = module
                };
                Db.ModuleCourses.Add(secCourse);
            }


            if (model.HasLaboratory)
            {
                var secCourse = new ModuleSubject
                {
                    Name = "Laborpraktikum",
                    Module = module
                };
                Db.ModuleCourses.Add(secCourse);
            }


            Db.CurriculumModules.Add(module);
            Db.SaveChanges();


            return RedirectToAction("Details", new {id = module.Id});
        }


        public ActionResult Details(Guid id)
        {
            var model = Db.CurriculumModules.SingleOrDefault(x => x.Id == id);

            var sem = SemesterService.GetSemester(DateTime.Today);

            var nextSem = SemesterService.GetNextSemester(sem);


            ViewBag.SemesterList = new List<Semester>();
            ViewBag.SemesterList.Add(sem);
            ViewBag.SemesterList.Add(nextSem);

            return View(model);
        }


        public ActionResult Admin(Guid id)
        {
            var model = Db.CurriculumModules.SingleOrDefault(x => x.Id == id);


            return View(model);
        }


        public ActionResult Delete(Guid id)
        {
            var model = Db.CurriculumModules.SingleOrDefault(x => x.Id == id);


            foreach (var subject in model.ModuleSubjects.ToList())
            {
                foreach (var opportunity in subject.Opportunities.ToList())
                {
                    Db.SubjectOpportunities.Remove(opportunity);
                }
                Db.ModuleCourses.Remove(subject);
            }

            foreach (var moduleCourse in model.ModuleSubjects.ToList())
            {
                Db.ModuleCourses.Remove(moduleCourse);
            }

            foreach (var accreditation in model.Accreditations.ToList())
            {
                Db.Accreditations.Remove(accreditation);
            }

            foreach (var description in model.Descriptions.ToList())
            {
                foreach (var examinationUnit in description.ExaminationUnits.ToList())
                {
                    foreach (var aid in examinationUnit.ExaminationAids.ToList())
                    {
                        Db.ExaminationAids.Remove(aid);
                    }

                    Db.ExaminationUnits.Remove(examinationUnit);
                }

                Db.ModuleDescriptions.Remove(description);
            }


            var mappings = Db.ModuleMappings.Where(x => x.Module.Id == model.Id).ToList();
            foreach (var mapping in mappings)
            {
                mapping.Module = null;
            }

            Db.CurriculumModules.Remove(model);
            Db.SaveChanges();


            return RedirectToAction("Index");
        }

        public ActionResult Courses(Guid moduleId, Guid? semId)
        {
            if (semId == null)
            {
                semId = SemesterService.GetSemester(DateTime.Today).Id;
            }

            var module = Db.CurriculumModules.SingleOrDefault(x => x.Id == moduleId);
            var semester = SemesterService.GetSemester(semId);

            var courseSummaryService = new CourseService(Db);

            var model = new ModuleSemesterCoursesModel
            {
                Module = module,
                Semester = semester
            };

            /*
            foreach (var moduleCourse in module.ModuleCourses)
            {
                var courses =
                    moduleCourse.Nexus.Where(x => x.Course.SemesterGroups.Any(g => g.Semester.Id == semId))
                        .Select(x => x.Course).Distinct().ToList();

                foreach (var course in courses)
                {
                    var summary = courseSummaryService.GetCourseSummary(course);

                    var semCourse = new ModuleSemesterCourseModel()
                    {
                        ModuleCourse = moduleCourse,
                        CourseSummary = summary
                    };

                    model.Courses.Add(semCourse);
                }
            }
            */

            return View(model);
        }

        public ActionResult Selection(Guid moduleId, Guid semId)
        {
            var module = Db.CurriculumModules.SingleOrDefault(x => x.Id == moduleId);
            var semester = SemesterService.GetSemester(semId);

            var courseSummaryService = new CourseService(Db);

            var model = new ModuleSemesterCoursesModel
            {
                Module = module,
                Semester = semester
            };

            /*
            foreach (var moduleCourse in module.ModuleCourses)
            {
                var courses =
                    moduleCourse.Nexus.Where(x => x.Course.SemesterGroups.Any(g => g.Semester.Id == semId))
                        .Select(x => x.Course).Distinct().ToList();


                foreach (var course in courses)
                {
                    var summary = courseSummaryService.GetCourseSummary(course);

                    var semCourse = new ModuleSemesterCourseModel()
                    {
                        ModuleCourse = moduleCourse,
                        CourseSummary = summary
                    };

                    model.Courses.Add(semCourse);
                }
            }
            */


            return View(model);
        }

        [HttpPost]
        public PartialViewResult Search(string searchText, Guid moduleCourseId, Guid semId)
        {
            var sem = SemesterService.GetSemester(semId);
            var org = GetMyOrganisation();

            var courses = Db.Activities.OfType<Course>().Where(a =>
                    (a.Name.Contains(searchText) || a.ShortName.Contains(searchText)) &&
                    a.SemesterGroups.Any(s =>
                        s.Semester.Id == sem.Id && s.CapacityGroup.CurriculumGroup.Curriculum.Organiser.Id == org.Id))
                .ToList();

            var courseSummaryService = new CourseService(Db);
            var moduleCourse = Db.ModuleCourses.SingleOrDefault(x => x.Id == moduleCourseId);
            var module = moduleCourse.Module;

            var model = new List<ModuleSemesterCourseModel>();
            /*
            foreach (var course in courses)
            {
                // suchen, ob schon im Modul vorhanden, egal unter welchem Lehrformat
                var exist = Db.CourseNexus.Any(x => x.Course.Id == course.Id && x.ModuleCourse.Module.Id == module.Id);

                if (!exist)
                {
                    var summary = courseSummaryService.GetCourseSummary(course);

                    var semCourse = new ModuleSemesterCourseModel()
                    {
                        ModuleCourse = moduleCourse,
                        CourseSummary = summary
                    };

                    model.Add(semCourse);
                }
            }
            */



            return PartialView("_CourseTable", model);
        }

        [HttpPost]
        public PartialViewResult SaveCourseList(Guid moduleId, string[] courseIds)
        {
            var module = Db.CurriculumModules.SingleOrDefault(x => x.Id == moduleId);

            if (courseIds == null)
                return null;


            // die bestehende Liste aller Nexi
            /*
            var oldNexusList = new List<CourseModuleNexus>();
            foreach (var moduleCourse in module.ModuleCourses)
            {
                foreach (var courseNexus in moduleCourse.Nexus)
                {
                    oldNexusList.Add(courseNexus);
                }
            }
            */

            foreach (var tempId in courseIds)
            {
                var n = tempId.IndexOf("--");


                var courseId = Guid.Parse(tempId.Substring(0, n));
                var moduleCourseId = Guid.Parse(tempId.Substring(n + 2));

                //var nexus = oldNexusList.FirstOrDefault(x =>
                //    x.ModuleCourse.Id == moduleCourseId && x.Course.Id == courseId);


                // schon drin => aus der Liste löschen
                /*
                if (nexus != null)
                {
                    //oldNexusList.Remove(nexus);
                }
                else
                {
                    // das ist neu => hinzufügen
                    var course = Db.Activities.OfType<Course>().SingleOrDefault(x => x.Id == courseId);
                    var moduleCourse = Db.ModuleCourses.SingleOrDefault(x => x.Id == moduleCourseId);

                    nexus = new CourseModuleNexus
                    {
                        Course = course,
                        ModuleCourse = moduleCourse
                    };

                    Db.CourseNexus.Add(nexus);
                }
                */
            }

            // die in der oldList verbliebenen Einträge kommen raus
            /*
            foreach (var oldNexus in oldNexusList)
            {
                Db.CourseNexus.Remove(oldNexus);
            }
            */

            Db.SaveChanges();

            return null;
        }

        public ActionResult Participants(Guid moduleId, Guid semId)
        {
            var module = Db.CurriculumModules.SingleOrDefault(x => x.Id == moduleId);
            var semester = SemesterService.GetSemester(semId);

            var courseSummaryService = new CourseService(Db);

            var model = new ModuleSemesterCoursesModel
            {
                Module = module,
                Semester = semester
            };


            /*
            foreach (var moduleCourse in module.ModuleCourses)
            {
                var courses =
                    moduleCourse.Nexus.Where(x => x.Course.SemesterGroups.Any(g => g.Semester.Id == semId))
                        .Select(x => x.Course).Distinct().ToList();


                foreach (var course in courses)
                {
                    var summary = courseSummaryService.GetCourseSummary(course);

                    var semCourse = new ModuleSemesterCourseModel()
                    {
                        ModuleCourse = moduleCourse,
                        CourseSummary = summary
                    };

                    model.Courses.Add(semCourse);



                    foreach (var subscription in course.Occurrence.Subscriptions)
                    {
                        var participant = model.Participants.SingleOrDefault(x => x.UserId.Equals(subscription.UserId));

                        if (participant == null)
                        {
                            participant = new ModuleParticipantModel
                            {
                                UserId = subscription.UserId
                            };
                            model.Participants.Add(participant);
                        }

                        participant.Courses.Add(new ModuleParticipantSubscriptionModel {
                            Course = course,
                            Subscription = subscription
                            });
                    }
                }

            }
            */

            // die user und students ergänzen
            foreach (var participant in model.Participants)
            {
                var user = UserManager.FindById(participant.UserId);
                var student = user != null ? StudentService.GetCurrentStudent(user) : null;

                participant.User = user;
                participant.Student = student;

            }

            return View(model);
        }



        public FileResult Download(Guid moduleId, Guid semId)
        {
            var module = Db.CurriculumModules.SingleOrDefault(x => x.Id == moduleId);
            var semester = SemesterService.GetSemester(semId);

            var courseSummaryService = new CourseService(Db);

            var model = new ModuleSemesterCoursesModel
            {
                Module = module,
                Semester = semester
            };

            /*
            foreach (var moduleCourse in module.ModuleCourses)
            {
                var courses =
                    moduleCourse.Nexus.Where(x => x.Course.SemesterGroups.Any(g => g.Semester.Id == semId))
                        .Select(x => x.Course).Distinct().ToList();


                foreach (var course in courses)
                {
                    var summary = courseSummaryService.GetCourseSummary(course);

                    var semCourse = new ModuleSemesterCourseModel()
                    {
                        ModuleCourse = moduleCourse,
                        CourseSummary = summary
                    };

                    model.Courses.Add(semCourse);



                    foreach (var subscription in course.Occurrence.Subscriptions)
                    {
                        var participant = model.Participants.SingleOrDefault(x => x.UserId.Equals(subscription.UserId));

                        if (participant == null)
                        {
                            participant = new ModuleParticipantModel
                            {
                                UserId = subscription.UserId
                            };
                            model.Participants.Add(participant);
                        }

                        participant.Courses.Add(new ModuleParticipantSubscriptionModel
                        {
                            Course = course,
                            Subscription = subscription
                        });
                    }
                }

            }
            */

            // die user und students ergänzen
            foreach (var participant in model.Participants)
            {
                var user = UserManager.FindById(participant.UserId);
                var student = user != null ? StudentService.GetCurrentStudent(user) : null;

                participant.User = user;
                participant.Student = student;

            }


            var ms = new MemoryStream();
            var writer = new StreamWriter(ms, Encoding.Default);

            writer.Write(
                "Name;Vorname;Studiengang;Semester;E-Mail");

            foreach (var modelCourse in model.Courses)
            {
                writer.Write(";{0}", modelCourse.CourseSummary.Course.ShortName);
            }

            writer.Write(Environment.NewLine);

            foreach (var participant in model.Participants)
            {
                if (participant.User != null)
                {
                    var student = participant.Student;
                    var group = "";
                    var sem = "";

                    if (student != null)
                    {
                        group = student.Curriculum.ShortName;
                        sem = student.FirstSemester.Name;
                    }


                    writer.Write("{0};{1};{2};{3};{4}",
                        participant.User.LastName, participant.User.FirstName,
                        group, sem,
                        participant.User.Email);

                    foreach (var modelCourse in model.Courses)
                    {
                        var pc = participant.Courses.FirstOrDefault(x => x.Course.Id == modelCourse.CourseSummary.Course.Id);

                        if (pc == null)
                        {
                            writer.Write(";");
                        }
                        else
                        {
                            if (pc.Subscription.OnWaitingList)
                            {
                                writer.Write(";WL");
                            }
                            else
                            {
                                writer.Write(";TN");
                            }
                        }
                    }

                    writer.Write(Environment.NewLine);
                }
            }

            writer.Flush();
            writer.Dispose();



            var sb = new StringBuilder();
            sb.Append("Eintragungen");
            sb.Append(module.ShortName);
            sb.Append("_");
            sb.Append(DateTime.Today.ToString("yyyyMMdd"));
            sb.Append(".csv");

            return File(ms.GetBuffer(), "text/csv", sb.ToString());


        }

    }
}