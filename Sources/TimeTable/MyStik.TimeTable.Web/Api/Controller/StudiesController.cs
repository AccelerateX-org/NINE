using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using MyStik.TimeTable.Data;
using MyStik.TimeTable.DataServices;
using MyStik.TimeTable.Web.Api.DTOs;
using MyStik.TimeTable.Web.Services;

namespace MyStik.TimeTable.Web.Api.Controller
{

    /// <summary>
    /// 
    /// </summary>
    [RoutePrefix("api/v2/studies")]
    public class StudiesController : ApiBaseController
    {
        [Route("subscriptions")]
        public IQueryable<SubscriptionDto> Subscriptions([FromBody] SubscriptionCourseModel model)
        {
            var studService = new StudentService(Db);
            var semService = new SemesterService(Db);

            var list = new List<SubscriptionDto>();

            var user = GetUser(model.Id.ToString());
            var student = studService.GetCurrentStudent(user.Id);
            var semester = semService.GetSemester(DateTime.Today);

            var courses = Db.Activities.OfType<Course>().Where(a =>
                a.SemesterGroups.Any(g => g.Semester.Id == semester.Id) &&
                a.Occurrence.Subscriptions.Any(u => u.UserId.Equals(user.Id))).ToList();
            foreach (var course in courses)
            {
                var subscription =
                    course.Occurrence.Subscriptions.FirstOrDefault(x => x.UserId.Equals(user.Id));

                var subModel = new SubscriptionDto();

                subModel.CourseId = course.Id;
                subModel.IsValid = true;
                subModel.OnWaitingList = subscription.OnWaitingList;
                subModel.Title = course.Name;

                list.Add(subModel);

            }

            return list.AsQueryable();

        }
    }
}

