using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Http;
using MyStik.TimeTable.Data;
using MyStik.TimeTable.Web.Api.DTOs;
using MyStik.TimeTable.Web.Api.Services;
using MyStik.TimeTable.Web.Models;
using MyStik.TimeTable.Web.Services;

namespace MyStik.TimeTable.Web.Api.Controller
{
    /*
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

            if (string.IsNullOrEmpty(model.userid))
                return new List<CalendarDateDto>().AsQueryable();

            var userService = new UserInfoService();

            ApplicationUser user = null;

            if (model.userid.StartsWith("#id#"))
            {
                var userName = model.userid.Remove(0, 4);
                var email = string.Format("{0}@acceleratex.org", userName);

                user = userService.GetUserByEmail(email);
            }
            else
            {
                user = userService.GetUser(model.userid);
            }


            var from = model.date.Date;
            var until = from.AddDays(1);

            var list = new List<CalendarDateDto>();

            var allDates = Db.ActivityDates.Where(x => 
                (x.Activity.Occurrence.Subscriptions.Any(s => !string.IsNullOrEmpty(s.UserId) && s.UserId.Equals(user.Id)) ||
                 x.Hosts.Any(m => !string.IsNullOrEmpty(m.UserId) && m.UserId.Equals(user.Id))
                 )
                &&
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


        [Route("myday")]
        [HttpPost]
        public IQueryable<ActiveEvent> MyDay([FromBody] CalendarDayRequestModel model)
        {
            var converter = new CourseConverter(Db);
            var userService = new UserInfoService();
            var user = userService.GetUser(model.userid);

            var from = model.date.Date;
            var until = from.AddDays(1);

            var list = new List<ActiveEvent>();

            var allDates = Db.ActivityDates.Where(x =>
                (x.Activity.Occurrence.Subscriptions.Any(s => !string.IsNullOrEmpty(s.UserId) && s.UserId.Equals(user.Id)) ||
                 x.Hosts.Any(m => !string.IsNullOrEmpty(m.UserId) && m.UserId.Equals(user.Id))
                )
                &&
                x.Begin >= from && x.End <= until).ToList();


            foreach (var activityDate in allDates)
            {
                var calendarDate = new ActiveEvent();

                calendarDate.course = activityDate.Activity.Name;
                calendarDate.starttime = activityDate.Begin;
                calendarDate.endtime = activityDate.End;

                var sb = new StringBuilder();
                foreach (var host in activityDate.Hosts)
                {
                    sb.Append(host.Name);
                    if (host != activityDate.Hosts.Last())
                    {
                        sb.Append(", ");
                    }
                }
                calendarDate.teacher = sb.ToString();
                sb.Clear();

                foreach (var room in activityDate.Rooms)
                {
                    sb.Append(room.Number);
                    if (room != activityDate.Rooms.Last())
                    {
                        sb.Append(", ");
                    }
                }
                calendarDate.room = sb.ToString();
                sb.Clear();

                foreach (var virtualRoom in activityDate.VirtualRooms)
                {
                    sb.Append(virtualRoom.Room.Name);
                    if (virtualRoom != activityDate.VirtualRooms.Last())
                    {
                        sb.Append(", ");
                    }
                }
                calendarDate.virtual_room = sb.ToString();
                sb.Clear();

                if (activityDate.Occurrence.IsCanceled)
                {
                    calendarDate.special = "X";
                }


                if (activityDate.Activity is Course course)
                {
                    calendarDate.moodle = course.UrlMoodleCourse;
                    calendarDate.moodle_key = course.KeyMoodleCourse;
                }


                list.Add(calendarDate);
            }



            return list.AsQueryable();
        }


    }
    */
}
