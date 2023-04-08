using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using MyStik.TimeTable.DataServices;
using MyStik.TimeTable.Web.Api.DTOs;
using MyStik.TimeTable.Web.Services;

namespace MyStik.TimeTable.Web.Api.Controller
{
    [RoutePrefix("api/v2/teaching")]

    public class TeachingController : ApiBaseController
    {
        [Route("courses")]
        public IQueryable<SubscriptionDto> Courses([FromBody] SubscriptionCourseModel model)
        {
            var memberService = new MemberService(Db);
            var semService = new SemesterService(Db);
            var teachingService = new TeachingService(Db);

            var list = new List<SubscriptionDto>();

            var user = GetUser(model.Id.ToString());
            var members = memberService.GetFacultyMemberships(user.Id);


            var semester = semService.GetSemester(DateTime.Today);


            var teaching = teachingService.GetActivities(semester, user, members);


            foreach (var course in teaching.Courses)
            {

                var subModel = new SubscriptionDto();

                subModel.CourseId = course.Course.Id;
                subModel.Title = course.Course.Name;

                list.Add(subModel);
            }

            return list.AsQueryable();

        }



    }
}
