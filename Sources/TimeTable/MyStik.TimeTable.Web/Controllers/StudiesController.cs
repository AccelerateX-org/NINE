using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MyStik.TimeTable.Data;
using MyStik.TimeTable.Web.Models;
using MyStik.TimeTable.Web.Services;

namespace MyStik.TimeTable.Web.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public class StudiesController : BaseController
    {
        // GET: Studies
        public ActionResult Index(Guid? id)
        {
            // den Benutzer
            var user = GetCurrentUser();
            var student = GetCurrentStudent(user.Id);
            var cand = Db.Candidatures.Where(x => x.UserId.Equals(user.Id)).ToList();
            var meberships = MemberService.GetFacultyMemberships(user.Id);


            if (student == null)
            {
                if (cand.Any())
                {
                    // kein Student, aber laufende Aufnahmeverfahren
                    return RedirectToAction("Index", "Candidature");
                }

                if (meberships.Any())
                {
                    return RedirectToAction("Curricula", "Subscription");
                }
                    
                // kein Student und auch keine laufenden Aufnahmeverfahren
                return RedirectToAction("Index", "Home");
            }

            // das aktuelle Semester der Organisation
            Semester semester = null;
            Semester prevSemester = null;
            Semester nextSemester = null;

            if (id == null)
            {
                semester = GetLatestSemester(student.Curriculum.Organiser);
            }
            else
            {
                semester = SemesterService.GetSemester(id);
            }

            prevSemester = SemesterService.GetPreviousSemester(semester);
            nextSemester = SemesterService.GetNextSemester(semester);


            var model = new StudentSummaryModel
            {
                Student = student,
                Semester = semester,
                PrevSemester = prevSemester,
                NextSemester = nextSemester,
                Lecturers = new List<StudentLecturerViewModel>()
            };


            model.Thesis = Db.Theses.FirstOrDefault(x => x.Student.Id == student.Id);

            // Alle gebuchten Lehrveranstaltungen
            var courseService = new CourseService(Db);

            model.Courses = new List<CourseSummaryModel>();

            var courses = Db.Activities.OfType<Course>().Where(a =>
                a.SemesterGroups.Any(g => g.Semester.Id == semester.Id) &&
                a.Occurrence.Subscriptions.Any(u => u.UserId.Equals(user.Id))).ToList();
            foreach (var course in courses)
            {
                var summary = courseService.GetCourseSummary(course);
                model.Courses.Add(summary);

                var state = ActivityService.GetActivityState(course.Occurrence, user);

                summary.User = user;
                summary.Subscription = state.Subscription;

                summary.Lottery =
                    Db.Lotteries.FirstOrDefault(x => x.Occurrences.Any(y => y.Id == course.Occurrence.Id));

                //
                foreach (var lecturer in summary.Lecturers)
                {
                    var studentLecturer = model.Lecturers.SingleOrDefault(x => x.Lecturer.Id == lecturer.Id);
                    if (studentLecturer == null)
                    {
                        studentLecturer = new StudentLecturerViewModel
                        {
                            Lecturer = lecturer,
                            Courses = new List<CourseSummaryModel>(),
                            OfficeHours = new List<OfficeHourDateViewModel>()
                        };

                        model.Lecturers.Add(studentLecturer);
                    }

                    studentLecturer.Courses.Add(summary);
                }
            }

            // Alle gebuchten Sprechstundentermine in der Zukunft (Ende muss in Zukunft liegen)

            var myOfficeHours = Db.Activities.OfType<OfficeHour>().Where(x =>
                    x.Semester != null &&
                    x.Dates.Any(d => d.End >= DateTime.Now && (
                        d.Occurrence.Subscriptions.Any(s => s.UserId.Equals(user.Id)) ||
                        d.Slots.Any(s => s.Occurrence.Subscriptions.Any(g => g.UserId.Equals(user.Id))))))
                .ToList();

            foreach (var officeHour in myOfficeHours)
            {
                var lecturer = officeHour.Owners.First().Member;

                var studentLecturer = model.Lecturers.SingleOrDefault(x => x.Lecturer.Id == lecturer.Id);
                if (studentLecturer == null)
                {
                    studentLecturer = new StudentLecturerViewModel
                    {
                        Lecturer = lecturer,
                        Courses = new List<CourseSummaryModel>(),
                        OfficeHours = new List<OfficeHourDateViewModel>()
                    };

                    model.Lecturers.Add(studentLecturer);
                }


                var dates = officeHour.Dates.Where(d => d.Occurrence.Subscriptions.Any(s => s.UserId.Equals(user.Id)))
                    .ToList();

                foreach (var date in dates)
                {
                    var ohDate = new OfficeHourDateViewModel();
                    ohDate.OfficeHour = officeHour;
                    ohDate.Date = date;
                    ohDate.Lecturer = officeHour.Owners.First().Member;
                    ohDate.Subscription = date.Occurrence.Subscriptions.FirstOrDefault(x => x.UserId.Equals(user.Id));

                    studentLecturer.OfficeHours.Add(ohDate);
                }

                // alle slots
                dates = officeHour.Dates.Where(d => d.Slots.Any(s => s.Occurrence.Subscriptions.Any(g => g.UserId.Equals(user.Id))))
                    .ToList();

                foreach (var date in dates)
                {
                    var slots = date.Slots.Where(d => d.Occurrence.Subscriptions.Any(s => s.UserId.Equals(user.Id)))
                        .ToList();

                    foreach (var slot in slots)
                    {
                        var ohDate = new OfficeHourDateViewModel();
                        ohDate.OfficeHour = officeHour;
                        ohDate.Date = date;
                        ohDate.Slot = slot;
                        ohDate.Lecturer = officeHour.Owners.First().Member;
                        ohDate.Subscription = slot.Occurrence.Subscriptions.FirstOrDefault(x => x.UserId.Equals(user.Id));

                        studentLecturer.OfficeHours.Add(ohDate);
                    }
                }
            }



            return View("Index", model);

        }
    }
}
