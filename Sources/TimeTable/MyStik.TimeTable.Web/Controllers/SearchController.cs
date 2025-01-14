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
        /// <param name="semId"></param>
        /// <param name="isGlobal"></param>
        /// <returns></returns>
        public ActionResult Index(string searchText, Guid? semId, bool? isGlobal = true)
        {
            var user = GetCurrentUser();
            var org = GetMyOrganisation();
            if (isGlobal.HasValue && isGlobal.Value)
                org = null;

            var semester = semId != null ? SemesterService.GetSemester(semId) : SemesterService.GetSemester(DateTime.Today);
            var nextSemester = SemesterService.GetNextSemester(semester);
            var previousSemester = SemesterService.GetPreviousSemester(semester);

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

            var courseList = new CourseService(Db).SearchCourses(searchText, org, semester);
            foreach (var course in courseList)
            {
                course.State = ActivityService.GetActivityState(course.Course.Occurrence, user);
            }

            // alle Mitglieder
            var activeLecturers = org != null ?
            Db.Members.Where(m => 
                 m.Organiser.Id == org.Id &&
                 ((m.Name.Contains(searchText) || m.ShortName.Contains(searchText)) && !m.Organiser.IsStudent))
                .OrderBy(m => m.Name)
                .ToList()
            :
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

            // Räume
            var rooms = Db.Rooms.Where(x => x.Number.Contains(searchText) || 
                                            (!string.IsNullOrEmpty(x.Name) && x.Name.Contains(searchText))).ToList();

            var model = new SearchViewModel
            {
                Organiser = org,
                SearchText = searchText,
                Semester = semester,
                NextSemester = nextSemester,
                Courses = courseList,
                NextCourses = new List<CourseSummaryModel>(),
                Lecturers = lecturerList,
                Rooms = rooms
            };

            ViewBag.UserRight = GetUserRight();
            ViewBag.PrevSemester = previousSemester;
            ViewBag.CurrentSemester = semester;
            ViewBag.NextSemester = nextSemester;

            return View(model);
        }
    }
}