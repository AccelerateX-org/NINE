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
        public ActionResult Index(string searchText, bool? isGlobal)
        {
            var user = GetCurrentUser();
            var org = GetMyOrganisation();
            if (isGlobal.HasValue && isGlobal.Value)
                org = null;

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

            var courseList = new CourseService(Db).SearchCourses(semester.Id, searchText, org);
            foreach (var course in courseList)
            {
                course.State = ActivityService.GetActivityState(course.Course.Occurrence, AppUser);
            }

            // Veranstaltungen des nächsten Semesters 
            // user = student => nur wenn freigegeben
            var nextCoursesList = new List<CourseSummaryModel>();
            var bSearchForCourses = true;
            if (user.MemberState != MemberState.Staff)
            {
                // nur wenn alle Gruppen freigegeben sind
                var isAvailable = nextSemester.Groups.All(x => x.IsAvailable);
                bSearchForCourses = isAvailable;
            }

            if (bSearchForCourses)
            {
                nextCoursesList.AddRange(new CourseService(Db).SearchCourses(nextSemester.Id, searchText, org));
                foreach (var course in nextCoursesList)
                {
                    course.State = ActivityService.GetActivityState(course.Course.Occurrence, AppUser);
                }
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
                                            (string.IsNullOrEmpty(x.Name) && x.Name.Contains(searchText))).ToList();

            var model = new SearchViewModel
            {
                Organiser = org,
                SearchText = searchText,
                Semester = semester,
                NextSemester = nextSemester,
                Courses = courseList,
                NextCourses = nextCoursesList,
                Lecturers = lecturerList,
                Rooms = rooms
            };

            ViewBag.UserRight = GetUserRight();

            return View(model);
        }
    }
}