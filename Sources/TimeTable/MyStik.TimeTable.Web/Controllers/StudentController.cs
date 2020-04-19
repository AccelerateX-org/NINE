using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MyStik.TimeTable.Data;
using MyStik.TimeTable.Web.Models;

namespace MyStik.TimeTable.Web.Controllers
{
    public class StudentController : BaseController
    {
        // GET: Student
        public ActionResult Index()
        {
            var user = GetCurrentUser();
            var student = StudentService.GetCurrentStudent(user.Id);

            if (student != null)
            {
                var thesis = Db.Theses.FirstOrDefault(x => x.Student.Id == student.Id);

                var startSemester = student.FirstSemester;
                var endSemester = SemesterService.GetSemester(DateTime.Today);

                var semesterList = Db.Semesters.Where(x =>
                    x.StartCourses >= startSemester.StartCourses && x.EndCourses <= endSemester.EndCourses).ToList();


                var model = new StudentSummaryModel
                {
                    Student = student,
                    Thesis = thesis,
                    Semester = semesterList
                };

                return View(model);
            }
            else
            {
                var model = new StudentSummaryModel
                {
                    Student = null,
                    Thesis = null,
                    Semester = new List<Semester>()
                };

                return View(model);

            }
        }
    }
}