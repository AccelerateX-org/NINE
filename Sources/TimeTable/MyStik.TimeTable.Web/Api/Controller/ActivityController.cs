using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using MyStik.TimeTable.Web.Api.Contracts;
using MyStik.TimeTable.Web.Api.Responses;
using MyStik.TimeTable.Web.Api.Services;
using MyStik.TimeTable.Web.Models;
using MyStik.TimeTable.Web.Services;

namespace MyStik.TimeTable.Web.Api.Controller
{
    /*
    public class ActivityRequestModel
    {
        public Guid UserId { get; set; }

        public string start { get; set; }
            
        public string end { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    [System.Web.Http.RoutePrefix("api/v2/activities")]
    public class ActivityController : ApiBaseController
    {
        private DateTime GetDateTime(string time)
        {
            var dt = DateTime.Parse(time);
            return dt;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [System.Web.Http.Route("")]
        [HttpPost]
        public IQueryable<CalendarEventModel> PersonalAgenda([FromBody] ActivityRequestModel model)
        {
            var from = DateTime.Now;
            var until = from.AddDays(7);

            if (!string.IsNullOrEmpty(model.start) && !string.IsNullOrEmpty(model.end))
            {
                from = GetDateTime(model.start);
                until = GetDateTime(model.end);
            }


            var userService = new UserInfoService();
            var user = userService.GetUser(model.UserId.ToString());

            var calendarService = new CalendarService();
            var calenderEventService = new CalendarEventService();

            var activities = calendarService.GetActivityPlan(user, from, until);

            var cal = calenderEventService.GetCalendarEvents(activities);

            return cal.AsQueryable();
        }


    }
    */
}
