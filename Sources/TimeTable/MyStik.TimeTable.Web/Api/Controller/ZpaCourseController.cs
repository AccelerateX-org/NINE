using MyStik.TimeTable.Web.Api.DTOs;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Http;
using MyStik.TimeTable.Data;
using MyStik.TimeTable.Web.Api.Services;

namespace MyStik.TimeTable.Web.Api.Controller
{
    /*
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

            var org = Db.Organisers.Include(activityOrganiser =>
                activityOrganiser.Curricula.Select(curriculum1 => curriculum1.LabelSet.ItemLabels)).SingleOrDefault(x => x.ShortName.Equals(organiser));
            if (org == null)
                return list.AsQueryable();

            var curr = org.Curricula.FirstOrDefault(x => x.ShortName.Equals(curriculum));
            if (curr == null)
                return list.AsQueryable();

            var sem = Db.Semesters.SingleOrDefault(x => x.Name.Equals(semester));
            if (sem == null)
                return list.AsQueryable();

            var courses = Db.Activities.OfType<Course>().Where(x =>
                x.Semester.Id == sem.Id &&
                x.SubjectTeachings.Any(t => t.Subject.SubjectAccreditations.Any(a => a.Slot.AreaOption.Area.Curriculum.Id == curr.Id))
            ).ToList();

            if (curr.LabelSet != null)
            {
                foreach (var label in curr.LabelSet.ItemLabels.ToList())
                {
                    var labeledCourses = Db.Activities.OfType<Course>()
                        .Where(x =>
                            x.Semester.Id == sem.Id &&
                            //x.Organiser.Id == org.Id &&
                            x.LabelSet != null &&
                            x.LabelSet.ItemLabels.Any(l => l.Id == label.Id))
                        .ToList();

                    courses.AddRange(labeledCourses);
                }

                courses = courses.Distinct().ToList();
            }

            var converter = new CourseConverter(Db);
            foreach (var course in courses)
            {
                list.Add(converter.ConvertZpa(course));
            }

            return list.AsQueryable();
        }
    }
    */
}

