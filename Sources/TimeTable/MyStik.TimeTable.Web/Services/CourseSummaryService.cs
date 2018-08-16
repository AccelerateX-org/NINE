using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MyStik.TimeTable.Data;
using MyStik.TimeTable.Web.Models;

namespace MyStik.TimeTable.Web.Services
{
    public class CourseSummaryService : BaseService
    {
        private CourseService CourseService { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="db"></param>
        public CourseSummaryService(TimeTableDbContext db) : base(db)
        {
            CourseService = new CourseService(db);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public CourseSummaryModel GetCourseSummary(Guid id)
        {
            var course = CourseService.GetCourse(id);

            var summary = new CourseSummaryModel();
            summary.Course = course;

            var lectures =
                Db.Members.Where(l => l.Dates.Any(occ => occ.Activity.Id == course.Id)).ToList();
            summary.Lecturers.AddRange(lectures);

            var rooms =
                Db.Rooms.Where(l => l.Dates.Any(occ => occ.Activity.Id == course.Id)).ToList();
            summary.Rooms.AddRange(rooms);

            var days = (from occ in course.Dates
                select
                    new
                    {
                        Day = occ.Begin.DayOfWeek,
                        Begin = occ.Begin.TimeOfDay,
                        End = occ.End.TimeOfDay,
                    }).Distinct();

            foreach (var day in days)
            {
                var defaultDay = course.Dates.FirstOrDefault(d => d.Begin.DayOfWeek == day.Day);

                var courseDate = new CourseDateModel
                {
                    DayOfWeek = day.Day,
                    StartTime = day.Begin,
                    EndTime = day.End,
                    DefaultDate = defaultDay.Begin
                };
                summary.Dates.Add(courseDate);
            }

            foreach (var semesterGroup in course.SemesterGroups)
            {
                if (!summary.Curricula.Contains(semesterGroup.CapacityGroup.CurriculumGroup.Curriculum))
                {
                    summary.Curricula.Add(semesterGroup.CapacityGroup.CurriculumGroup.Curriculum);
                }
            }


            return summary;

        }
    }
}