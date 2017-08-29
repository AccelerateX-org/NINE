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
            var semester = GetSemester();

            if (string.IsNullOrEmpty(searchText))
            {
                var defaultModel = new SearchViewModel
                {
                    SearchText = searchText,
                    Semester = semester,
                    Courses = new List<CourseSummaryModel>(),
                    Lecturers = new List<LecturerViewModel>()
                };

                ViewBag.UserRight = GetUserRight();


                return View(defaultModel);
            }

            var courseList = new CourseService(UserManager).SearchCourses(semester.Name, searchText);
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

            // alle Mitglieder
            var activeLecturers =
            Db.Members.Where(m => 
                (m.Name.Contains(searchText) || m.ShortName.Contains(searchText)))
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