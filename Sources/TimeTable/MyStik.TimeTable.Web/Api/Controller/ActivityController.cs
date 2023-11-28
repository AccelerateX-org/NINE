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


        //Alle eigenen Termine in Zeitraum
        /// <summary>
        /// Abfrage aller eigenen Termine im gewünschten Zeitraum
        /// </summary>
        /// <param name="UserId">Die UserId des Accounts in der Datenbank</param>
        /// <param name="From">Anfangsdatum  des Zeitraums im Format dd.MM.yyyy</param>
        /// <param name="Until">Enddatum des Zeitraums im Format dd.MM.yyyy</param>
        /// <returns>Persönlichen Termine für jeden Tag im gewählten Zeitraum</returns>
        /*
        [System.Web.Http.Route("span")]
        public PersonalPlanResponse GetPersonalDatesSpan(string UserId, string From, string Until)
        {
            var From2=DateTime.ParseExact(From, "MM-dd-yyyy", null);
            var Until2 = DateTime.ParseExact(Until, "MM-dd-yyyy", null);
            var activityService = new ActivityInfoService();

            var activityList = activityService.GetPersonalDates(UserId, From2, Until2);

            var response = new PersonalPlanResponse
            {
                Courses = activityList,
            };

            return response;
        }

        //Alle Informationen zu einem Termin
        /// <summary>
        /// Abfrage aller Informationen eines Termins
        /// </summary>
        /// <param name="DateId">Id des Termins</param>
        /// <returns>Informationen zu einem bestimmten Termin</returns>
        [System.Web.Http.Route("date")]
        public DateInfoResponse GetDateById (string DateId)
        {
            var activityService = new ActivityInfoService();

            var activity = activityService.GetDateInfo(DateId);

            var response = new DateInfoResponse
            {
                DateInfo = activity,
            };

            return response;
        }

        //Eventinfo API
        //Alle kommenden Events abrufen
        /// <summary>
        /// Abfrage aller kommenden Events
        /// </summary>
        /// <returns>Liste aller zukünftigen Events</returns>
        public EventInfoResponse GetAllEvents()
        {
            var eventService = new EventInfoService();

            var eventList = eventService.GetAllEvents();

            var response = new EventInfoResponse
            {
                Events = eventList,
            };

            return response;
        }

        //Ein spezielles Event abrufen
        /// <summary>
        /// Abfrage der speziellen Informationen eines Events
        /// </summary>
        /// <param name="EventId">EventId des Eventtermins</param>
        /// <returns>Informationen zum gewünschten Event</returns>
        [System.Web.Http.Route("details")]
        public EventSingleResponse GetEvent(string EventId)
        {
            var eventService = new EventInfoService();

            var dbevent = eventService.GetEvent(EventId);

            var response = new EventSingleResponse
            {
                Event = dbevent,
            };

            return response;
        }

        */

    }
}
