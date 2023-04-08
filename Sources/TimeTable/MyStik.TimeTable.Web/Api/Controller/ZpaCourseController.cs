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
    [RoutePrefix("api/v2/apps/zpa/courses")]

    public class ZpaCourseController : ApiBaseController
    {
        /// <summary>
        /// 
        /// </summary>
        [Route("{organiser}/{curriculum}/{semester}")]
        public IQueryable<ZpaCourseDto> GetCourses(string organiser, string curriculum, string semester)
        {
            var list = new List<ZpaCourseDto>();

            var org = Db.Organisers.SingleOrDefault(x => x.ShortName.Equals(organiser));
            if (org == null)
                return list.AsQueryable();

            var curr = org.Curricula.FirstOrDefault(x => x.ShortName.Equals(curriculum));
            if (curr == null)
                return list.AsQueryable();

            var sem = Db.Semesters.SingleOrDefault(x => x.Name.Equals(semester));
            if (sem == null)
                return list.AsQueryable();

            var allCourses = Db.Activities.OfType<Course>().Where(x =>
                x.SemesterGroups.Any(g =>
                    g.CapacityGroup.CurriculumGroup.Curriculum.Id == curr.Id &&
                    g.Semester.Id == sem.Id)).ToList();

            var converter = new CourseConverter(Db);
            foreach (var course in allCourses)
            {
                list.Add(converter.ConvertZpa(course));
            }

            return list.AsQueryable();
        }
    }
}

