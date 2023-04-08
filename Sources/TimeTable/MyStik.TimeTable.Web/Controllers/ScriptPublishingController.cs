using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MyStik.TimeTable.Data;
using MyStik.TimeTable.Web.Models;
using MyStik.TimeTable.Web.Services;

namespace MyStik.TimeTable.Web.Controllers
{
    public class ScriptPublishingController : BaseController
    {
        public ActionResult Index()
        {
            var user = GetCurrentUser();

            var scripts = Db.ScriptDocuments.Where(x => x.UserId.Equals(user.Id)).ToList();


            return View(scripts);
        }


        public ActionResult Details(Guid id)
        {
            var model = Db.ScriptDocuments.SingleOrDefault(x => x.Id == id);

            return View(model);
        }


        public ActionResult Create()
        {
            var user = GetCurrentUser();
            var org = GetMyOrganisation();
            var members = MemberService.GetFacultyMemberships(user.Id);


            var semester = SemesterService.GetNewestSemester(org);

            var TeachingService = new TeachingService(Db);
            var userService = new UserInfoService();


            var activities = TeachingService.GetActivities(semester, user, members);

            var coursesNames = activities.Courses.GroupBy(x => x.Course.Name).Distinct();


            var modules = new List<SelectListItem>();

            foreach (var coursesName in coursesNames)
            {
                modules.Add(new SelectListItem
                {
                    Text = coursesName.Key,
                    Value = coursesName.Key
                });
            }


            var model = new ScriptCreatetModel();
            model.Semester = semester;
            model.SemesterId = semester.Id;

            ViewBag.Modules = modules;

            return View(model);
        }


        [HttpPost]
        public ActionResult Create(ScriptCreatetModel model)
        {
            var courseSummaryService = new CourseService(Db);

            var user = GetCurrentUser();
            var org = GetMyOrganisation();
            var members = MemberService.GetFacultyMemberships(user.Id);
            var member = GetMyMembership();

            var semester = SemesterService.GetSemester(model.SemesterId);

            var courseName = model.Module;

            // Alle Kurse aus dem Semester und der Fakultät mit dem Namenm
            var courses = Db.Activities.OfType<Course>().Where(x => x.Name.Equals(courseName) &&
                                                                    x.SemesterGroups.Any(g =>
                                                                        g.Semester.Id == semester.Id &&
                                                                        g.CapacityGroup.CurriculumGroup.Curriculum
                                                                            .Organiser.Id == org.Id)).ToList();

            var file = model.ScriptDoc;

            var storage = new BinaryStorage
            {
                Category = "Skript",
                FileType = file.ContentType,
                BinaryData = new byte[file.ContentLength],
                Created = DateTime.Now,
                Name = file.FileName,
                Description = string.Empty
            };

            file.InputStream.Read(storage.BinaryData, 0, file.ContentLength);

            Db.Storages.Add(storage);

            var scriptDoc = new ScriptDocument
            {
                Created = DateTime.Now,
                Title = model.Title,
                UserId = user.Id,
                Version = model.Version,
                Storage = storage,
                Publishings = new List<ScriptPublishing>()
            };

            Db.ScriptDocuments.Add(scriptDoc);


            // filltern nach Dozent
            foreach (var course in courses)
            {
                var addLink = true;
                if (!model.AllCourses)
                {
                    
                    var summary = courseSummaryService.GetCourseSummary(course);
                    var isLecturerInCourse = summary.Lecturers.Any(m => m.Id == member.Id);
                    addLink = isLecturerInCourse;
                }

                if (addLink)
                {
                    var pub = new ScriptPublishing
                    {
                        Course = course,
                        Published = DateTime.Now,
                        ScriptDocument = scriptDoc
                    };

                    Db.ScriptPublishings.Add(pub);
                }

            }




            Db.SaveChanges();


            return RedirectToAction("Index");
        }


        public ActionResult Edit(Guid id)
        {

            var doc = Db.ScriptDocuments.SingleOrDefault(x => x.Id == id);


            return View(doc);
        }


        [HttpPost]
        public ActionResult Edit(ScriptDocument model)
        {
            var doc = Db.ScriptDocuments.SingleOrDefault(x => x.Id == model.Id);

            if (!string.IsNullOrEmpty(model.Title))
            {
                doc.Title = model.Title;
            }

            if (!string.IsNullOrEmpty(model.Version))
            {
                doc.Version = model.Version;
            }

            Db.SaveChanges();

            return RedirectToAction("Index");
        }



        public ActionResult Delete(Guid id)
        {

            var doc = Db.ScriptDocuments.SingleOrDefault(x => x.Id == id);

            Db.Storages.Remove(doc.Storage);

            foreach (var pub in doc.Publishings.ToList())
            {
                Db.ScriptPublishings.Remove(pub);
            }

            Db.ScriptDocuments.Remove(doc);

            Db.SaveChanges();

            return RedirectToAction("Index");
        }



        public ActionResult Add(Guid id)
        {
            var user = GetCurrentUser();
            var org = GetMyOrganisation();
            var members = MemberService.GetFacultyMemberships(user.Id);


            var semester = SemesterService.GetNewestSemester(org);

            var TeachingService = new TeachingService(Db);
            var userService = new UserInfoService();


            var activities = TeachingService.GetActivities(semester, user, members);

            var coursesNames = activities.Courses.GroupBy(x => x.Course.Name).Distinct();


            var modules = new List<SelectListItem>();

            foreach (var coursesName in coursesNames)
            {
                modules.Add(new SelectListItem
                {
                    Text = coursesName.Key,
                    Value = coursesName.Key
                });
            }

            var doc = Db.ScriptDocuments.SingleOrDefault(x => x.Id == id);


            var model = new ScriptCreatetModel();
            model.Semester = semester;
            model.SemesterId = semester.Id;
            model.DocId = doc.Id;

            ViewBag.Modules = modules;



            return View(model);
        }


        [HttpPost]
        public ActionResult Add(ScriptCreatetModel model)
        {
            var courseSummaryService = new CourseService(Db);

            var user = GetCurrentUser();
            var org = GetMyOrganisation();
            var members = MemberService.GetFacultyMemberships(user.Id);
            var member = GetMyMembership();

            var semester = SemesterService.GetSemester(model.SemesterId);

            var courseName = model.Module;

            // Alle Kurse aus dem Semester und der Fakultät mit dem Namenm
            var courses = Db.Activities.OfType<Course>().Where(x => x.Name.Equals(courseName) &&
                                                                    x.SemesterGroups.Any(g =>
                                                                        g.Semester.Id == semester.Id &&
                                                                        g.CapacityGroup.CurriculumGroup.Curriculum
                                                                            .Organiser.Id == org.Id)).ToList();

            var doc = Db.ScriptDocuments.SingleOrDefault(x => x.Id == model.DocId);


            // filltern nach Dozent
            foreach (var course in courses)
            {
                var addLink = true;
                if (!model.AllCourses)
                {

                    var summary = courseSummaryService.GetCourseSummary(course);
                    var isLecturerInCourse = summary.Lecturers.Any(m => m.Id == member.Id);
                    addLink = isLecturerInCourse;
                }

                if (addLink)
                {
                    var pub = new ScriptPublishing
                    {
                        Course = course,
                        Published = DateTime.Now,
                        ScriptDocument = doc
                    };

                    Db.ScriptPublishings.Add(pub);
                }

            }



            Db.SaveChanges();


            return RedirectToAction("Index");
        }

        public ActionResult Withdraw(Guid id)
        {
            var pub = Db.ScriptPublishings.SingleOrDefault(x => x.Id == id);


            var doc = pub.ScriptDocument;


            Db.ScriptPublishings.Remove(pub);
            Db.SaveChanges();


            return RedirectToAction("Details", new {id = doc.Id});
        }


    }
}
