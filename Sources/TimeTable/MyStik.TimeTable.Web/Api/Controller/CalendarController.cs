using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Xml.Linq;
using MyStik.TimeTable.Data;
using MyStik.TimeTable.Web.Api.Contracts;
using MyStik.TimeTable.Web.Api.DTOs;
using MyStik.TimeTable.Web.Api.Responses;
using MyStik.TimeTable.Web.Api.Services;
using MyStik.TimeTable.Web.Services;

namespace MyStik.TimeTable.Web.Api.Controller
{
    /// <summary>
    /// 
    /// </summary>
    public class CalendarDayRequestModel
    {
        public string userid { get; set; }

        public DateTime date { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    [RoutePrefix("api/v2/calendar")]
    public class CalendarController : ApiBaseController
    {
        [Route("day")]
        [HttpPost]
        public IQueryable<CalendarDateDto> Day([FromBody] CalendarDayRequestModel model)
        {
            var converter = new CourseConverter(Db);
            var userService = new UserInfoService();
            var user = userService.GetUser(model.userid);

            var from = model.date.Date;
            var until = from.AddDays(1);

            var list = new List<CalendarDateDto>();

            var allDates = Db.ActivityDates.Where(x => x.Activity.Occurrence.Subscriptions.Any(s => s.UserId.Equals(user.Id)) &&
                                        x.Begin >= from && x.End <= until).ToList();

            foreach (var activityDate in allDates)
            {
                var calendarDate = new CalendarDateDto();

                calendarDate.Name = activityDate.Activity.Name;
                calendarDate.ShortName = activityDate.Activity.ShortName;

                if (activityDate.Activity is Course)
                    calendarDate.Type = CalendarDateType.Course;
                if (activityDate.Activity is Event)
                    calendarDate.Type = CalendarDateType.Event;
                if (activityDate.Activity is OfficeHour)
                    calendarDate.Type = CalendarDateType.OfficeHour;

                calendarDate.Date = converter.ConvertDate(activityDate);

                // calendarDate.Subscription = converter.

                list.Add(calendarDate);
            }



            return list.AsQueryable();
        }

    }
}
