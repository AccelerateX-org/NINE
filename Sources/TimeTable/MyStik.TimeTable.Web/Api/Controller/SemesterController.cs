using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using MyStik.TimeTable.Data;
using MyStik.TimeTable.Web.Api.DTOs;

namespace MyStik.TimeTable.Web.Api.Controller
{
    /// <summary>
    /// https://docs.microsoft.com/de-de/aspnet/web-api/overview/web-api-routing-and-actions/create-a-rest-api-with-attribute-routing
    /// </summary>
    [RoutePrefix("api/v2/semester")]
    public class SemesterController : ApiBaseController
    {
        /// <summary>
        /// 
        /// </summary>
        [Route("")]
        public IQueryable<SemesterDto> GetSemester()
        {
            var history = Db.Semesters.Where(x => x.StartCourses <= DateTime.Today);

            var result = new List<SemesterDto>();

            foreach (var semester in history)
            {
                result.Add(new SemesterDto
                {
                    Name = semester.Name
                });    
            }

            return result.AsQueryable();
        }

        /// <summary>
        /// 
        /// </summary>
        [Route("{name}/statistics")]
        public IQueryable<SemesterStatisticsDto> GetSemesterStatistics(string name)
        {
            var semester = Db.Semesters.FirstOrDefault(x => x.Name.Equals(name));

            var query = Db.Subscriptions.OfType<SemesterSubscription>()
                .Where(s => s.SemesterGroup.Semester.Id == semester.Id)
                .GroupBy(x => x.SemesterGroup.CapacityGroup.CurriculumGroup.Curriculum)
                .Select(n => new
                    {
                        Name = n.Key.Organiser.ShortName,
                        Name2 = n.Key.ShortName,
                        Count = n.Count()
                    })
                ;


            var result = new List<SemesterStatisticsDto>();

            foreach (var group in query)
            {
                var stat = new SemesterStatisticsDto();

                stat.Curriculum = $"{@group.Name}-{@group.Name2}";
                stat.Subscriptions = group.Count;

                result.Add(stat);
            }

            return result.AsQueryable();
        }

    }
}
