using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MyStik.TimeTable.Web.Models;
using MyStik.TimeTable.Web.Services;

namespace MyStik.TimeTable.Web.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public class SearchController : BaseController
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="searchText"></param>
        /// <returns></returns>
        public ActionResult Index(string searchText)
        {
            var semester = SemesterService.GetSemester(DateTime.Today);
            var nextSemester = SemesterService.GetNextSemester(semester);

            if (string.IsNullOrEmpty(searchText))
            {
                var defaultModel = new SearchViewModel
                {
                    SearchText = searchText,
                    Semester = semester,
                    NextSemester = nextSemester,
                    Courses = new List<CourseSummaryModel>(),
                    NextCourses = new List<CourseSummaryModel>(),
                    Lecturers = new List<LecturerViewModel>()
                };

                ViewBag.UserRight = GetUserRight();


                return View(defaultModel);
            }

            var courseList = new CourseService(Db).SearchCourses(semester.Name, searchText);
            foreach (var course in courseList)
            {
                course.State = ActivityService.GetActivityState(course.Course.Occurrence, AppUser);
            }

            var nextCoursesList = new CourseService(Db).SearchCourses(nextSemester.Name, searchText);
            foreach (var course in nextCoursesList)
            {
                course.State = ActivityService.GetActivityState(course.Course.Occurrence, AppUser);
            }

            // alle Mitglieder
            var activeLecturers =
            Db.Members.Where(m => 
                (m.Name.Contains(searchText) || m.ShortName.Contains(searchText)) && !m.Organiser.IsStudent)
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
                NextSemester = nextSemester,
                Courses = courseList,
                NextCourses = nextCoursesList,
                Lecturers = lecturerList
            };

            ViewBag.UserRight = GetUserRight();

            return View(model);
        }
    }
}