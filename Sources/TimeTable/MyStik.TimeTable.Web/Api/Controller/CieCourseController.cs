using MyStik.TimeTable.Web.Api.DTOs;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using MyStik.TimeTable.Data;
using MyStik.TimeTable.Web.Api.Services;

namespace MyStik.TimeTable.Web.Api.Controller
{
    /// <summary>
    /// 
    /// </summary>
    [RoutePrefix("api/v2/apps/cie/courses")]

    public class CieCourseController : ApiBaseController
    {
        /// <summary>
        /// 
        /// </summary>
        [Route("{semester}")]
        public IQueryable<CourseSummaryDto> GetCourses(string semester)
        {
            var list = new List<CourseSummaryDto>();

            var sem = Db.Semesters.FirstOrDefault(x => x.Name.Equals(semester));
            if (sem == null)
                return list.AsQueryable();

            var courses = Db.Activities.OfType<Course>().Where(x => x.SemesterGroups.Any(g =>
                g.IsAvailable && g.Semester.Id == sem.Id && g.CapacityGroup.CurriculumGroup.Curriculum.ShortName.StartsWith("CIE"))).ToList();

            var converter = new CourseConverter(Db);
            foreach (var course in courses)
            {
                var summary = converter.ConvertSummary(course);

                converter.ConvertDates(summary, course);


                // Sonderlocken
                if (summary.IsCoterie)
                {
                    summary.Category = "Red";
                }
                else
                {
                    if (summary.HasHomeBias)
                    {
                        summary.Category = "Yellow";
                    }
                    else
                    {
                        summary.Category = "Green";
                    }
                }

                var isBachelor = course.SemesterGroups.Any(g =>
                    g.CapacityGroup.CurriculumGroup.Curriculum.ShortName.Equals("CIE-B"));

                var isMaster = course.SemesterGroups.Any(g =>
                    g.CapacityGroup.CurriculumGroup.Curriculum.ShortName.Equals("CIE-M"));

                if (isBachelor && isMaster)
                {
                    summary.Level = "Bachelor / Master";
                }
                else
                {
                    summary.Level = "none";

                    if (isBachelor)
                    {
                        summary.Level = "Bachelor";
                    }

                    if (isMaster)
                    {
                        summary.Level = "Master";
                    }
                }

                var group = course.SemesterGroups.FirstOrDefault(x =>
                    x.CapacityGroup.CurriculumGroup.Curriculum.ShortName.StartsWith("CIE"));

                var org = group.CapacityGroup.CurriculumGroup.Curriculum.Organiser;
                
                summary.Department = new OrganiserDto();
                summary.Department.Name = org.Name;
                summary.Department.ShortName = org.ShortName;
                summary.Department.Color = org.HtmlColor;


                summary.Sws = 4;
                summary.Ects = 5;
                summary.UsCredits = 4;


                list.Add(summary);
            }

            return list.AsQueryable();
        }
    }
}

