using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using MyStik.TimeTable.Web.Models;
using MyStik.TimeTable.Web.Services;

namespace MyStik.TimeTable.Web.Controllers
{
    public class SearchController : BaseController
    {
        // GET: Search
        public ActionResult Index(string searchText)
        {

            var semester = GetSemester();

            var courseList = new CourseService(UserManager).SearchCourses(semester.Name, searchText);


            if (User.IsInRole("SysAdmin"))
            {
                var courseList2 = new CourseService(UserManager).SearchUnassignedCourses(searchText);
                
                courseList.AddRange(courseList2);
            }
            
            
            foreach (var course in courseList)
            {
                var lectures =
                    Db.Members.Where(l => l.Dates.Any(occ => occ.Activity.Id == course.Course.Id)).ToList();

                course.Lecturers.AddRange(lectures);

                var rooms =
                    Db.Rooms.Where(l => l.Dates.Any(occ => occ.Activity.Id == course.Course.Id)).ToList();
                course.Rooms.AddRange(rooms);

                course.State = ActivityService.GetActivityState(course.Course.Occurrence, AppUser, semester);
            }


            var activeLecturers =
            Db.Members.Where(m => 
                (m.Name.Contains(searchText) || m.ShortName.Contains(searchText)) &&
                m.Dates.Any(d => d.Activity.SemesterGroups.Any(s => s.Semester.Id == semester.Id)))
                .OrderBy(m => m.Name)
                .ToList();

            var lecturerList = new List<LecturerViewModel>();
            foreach (var lecturer in activeLecturers)
            {
                var viewModel = new LecturerViewModel { Lecturer = lecturer };

                lecturerList.Add(viewModel);
            }




            var model = new SearchViewModel
            {
                SearchText = searchText,
                Semester = semester,
                Courses = courseList,
                Lecturers = lecturerList
            };

            ViewBag.UserRight = GetUserRight();

            return View(model);
        }
    }
}