using MyStik.TimeTable.Data;
using MyStik.TimeTable.Web.Api.Contracts;
using MyStik.TimeTable.Web.Api.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;

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
        [HttpGet]
        [Route("")]
        [ResponseType(typeof(List<SemesterDto>))]

        public IHttpActionResult GetSemester()
        {
            var limit = DateTime.Today.AddYears(1);
            var history = Db.Semesters.Where(x => x.StartCourses <= limit).OrderByDescending(x => x.StartCourses);

            var result = new List<SemesterDto>();

            foreach (var semester in history)
            {
                result.Add(new SemesterDto
                {
                    Semester_Id = semester.Name,
                    Begin = semester.StartCourses,
                    End = semester.EndCourses
                });    
            }
            return Ok(result);
        }

        [HttpGet]
        [ResponseType(typeof(List<SemesterStatisticsDto>))]
        [Route("{semester_id}/segments/{organiser_id}")]

        public IHttpActionResult GetOrganiserSemester(string semester_id, string organiser_id)
        {
            var semester = Db.Semesters.SingleOrDefault(s => s.Name == semester_id);
            var org = Db.Organisers.SingleOrDefault(o => o.ShortName == organiser_id);

            if (semester == null || org == null)
                return NotFound();

            var dates = Db.SemesterDates.Where(d => d.Semester.Id == semester.Id && d.Organiser != null && d.Organiser.Id == org.Id).OrderBy(d => d.From).ToList();

            var response = new List<SemesterStatisticsDto>();

            foreach (var date in dates)
            {
                response.Add(new SemesterStatisticsDto
                {
                    Name = date.Description,
                    Begin = date.From,
                    End = date.To
                });
            }
            ;

            return Ok(response);
        }

    }
}
