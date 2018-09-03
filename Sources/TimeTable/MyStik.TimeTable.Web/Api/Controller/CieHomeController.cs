using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using MyStik.TimeTable.Web.Api.DTOs;

namespace MyStik.TimeTable.Web.Api.Controller
{
    /// <summary>
    /// 
    /// </summary>
    [RoutePrefix("api/v2/apps/cie")]

    public class CieHomeController : ApiBaseController
    {
        /// <summary>
        /// Lists all semester with CIE Course
        /// </summary>
        [Route("")]
        public IQueryable<SemesterDto> GetSemester()
        {
            var semester = 
            Db.Semesters.Where(x => x.Groups.Any(g =>
                g.IsAvailable && g.CapacityGroup.CurriculumGroup.Curriculum.ShortName.StartsWith("CIE"))).OrderByDescending(x => x.EndCourses).Take(3).ToList();

            var list = new List<SemesterDto>();

            foreach (var sem in semester)
            {
                list.Add(new SemesterDto
                {
                    Name = sem.Name,
                });
            }


            return list.AsQueryable();
        }

    }
}
