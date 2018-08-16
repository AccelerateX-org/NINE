using System;
using System.Collections.Generic;
using MyStik.TimeTable.Web.Api.Contracts;
using MyStik.TimeTable.Web.Api.Responses;
using MyStik.TimeTable.Web.Api.Services;
using MyStik.TimeTable.Web.Services;

namespace MyStik.TimeTable.Web.Api.Controller
{
    /// <summary>
    /// 
    /// </summary>
    public class ActivityController : ApiBaseController
    {
        //VorlesungsAPI

        /// <summary>
        /// Abfrage des persönlichen Stundenplans für eine Woche
        /// </summary>
        /// <param name="UserId">Die UserId des Accounts in der Datenbank</param>
        /// <returns>Liste nach Wochentagen mit den persönlichen/gebuchten Kursterminen</returns>
        public PersonalPlanResponse GetPersonalDates(string UserId)
        {
            //Da hier Abfrage der persönlichen Termine nur für kommende Woche, Festlegung der Variablen des Zeitraums
            //Alle eigenen Termine von jetzt bis zur nächsten Woche
            var from = DateTime.Now;
            var until = from.AddDays(7);

            var userService = new UserInfoService();
            var user = userService.GetUser(UserId);
            if (user != null  && user.UserName.Equals("demo@fillter.de"))
            {
                // Demodaten
                var dateList = new List<OwnDatesContract>();
                var courseList = new List<DateContract>();
                var roomList = new List<DateRoomContract>();
                var lecturerList = new List<DateLecturerContract>();

                var demoRoom = new DateRoomContract
                {
                    RoomId = Guid.NewGuid().ToString(),
                    RoomNumber = "R 1.083"
                };
                roomList.Add(demoRoom);

                var demoLecturer = new DateLecturerContract
                {
                    LecturerId = Guid.NewGuid().ToString(),
                    LecturerName = "Demo Dozent"
                };
                lecturerList.Add(demoLecturer);

                var demoCourse = new DateContract
                {
                    StartTime = "10:00",
                    EndTime = "11:30",
                    IsCanceled = false,
                    Titel = "Demovorlesung",
                    Rooms = roomList,
                    Lecturers = lecturerList
                };

                courseList.Add(demoCourse);

                var today = DateTime.Today;

                var demoDate = new OwnDatesContract
                {
                    StatedDate = today.ToString("dd.MM.yyyy"),
                    InfoString = null,
                    Dates = courseList
                };
                dateList.Add(demoDate);

                var date = today;
                var endOfWeek = today.AddDays(7);
                while (date <= endOfWeek)
                {
                    demoDate = new OwnDatesContract
                    {
                        StatedDate = date.ToString("dd.MM.yyyy"),
                        InfoString = null,
                        Dates = new List<DateContract>()
                    };
                    dateList.Add(demoDate);

                    date = date.AddDays(1);
                }


                var demoResponse = new PersonalPlanResponse
                {
                    Courses = dateList,
                };
                //Rückgabe der Response
                return demoResponse;

            }

            //Initialisierung des ActivityInfoService
            var activityService = new ActivityInfoService();

            //Abfrage der persönlichen Termine mit Hilfe des ActivityInfoService und den Parametern
            var activityList = activityService.GetPersonalDates(UserId, from, until);

            //Erstellen des Response mit Hilfe der activityList
            var response = new PersonalPlanResponse
            {
                Courses = activityList,
            };
            //Rückgabe der Response
            return response;
        }

        //Alle eigenen Termine in Zeitraum
        /// <summary>
        /// Abfrage aller eigenen Termine im gewünschten Zeitraum
        /// </summary>
        /// <param name="UserId">Die UserId des Accounts in der Datenbank</param>
        /// <param name="From">Anfangsdatum  des Zeitraums im Format dd.MM.yyyy</param>
        /// <param name="Until">Enddatum des Zeitraums im Format dd.MM.yyyy</param>
        /// <returns>Persönlichen Termine für jeden Tag im gewählten Zeitraum</returns>
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


        //TODO
        //Einbuchen in Event
        //Ausbuchen aus Event

        //Newsletter APIs
        //Abfragen der verfügbaren Newsletter
        //Einbuchen von Newsletter
        //Ausbuchen von Newsletter

    }
}
