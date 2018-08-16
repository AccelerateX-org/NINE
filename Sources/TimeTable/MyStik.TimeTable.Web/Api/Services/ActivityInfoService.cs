using System;
using System.Collections.Generic;
using System.Linq;
using MyStik.TimeTable.Web.Api.Contracts;
using MyStik.TimeTable.Data;

namespace MyStik.TimeTable.Web.Api.Services
{
    /// <summary>
    /// 
    /// </summary>
    public class ActivityInfoService
    {
        /// <summary>
        /// 
        /// </summary>
        protected readonly TimeTableDbContext Db = new TimeTableDbContext();

        /// <summary>
        /// Persönliche Termine abfragen am Tagen abfragen
        /// </summary>
        /// <param name="UserId"></param>
        /// <param name="from"></param>
        /// <param name="until"></param>
        /// <returns></returns>
        public IEnumerable<OwnDatesContract> GetPersonalDates(string UserId, DateTime from, DateTime until)
        {
            var days = new List<DateTime>();
            var start = from;

            //Alle Tage in Liste erstellen
            while (start.Day != until.Day)
            {
                days.Add(start);
                start = start.AddDays(1);
            }

            //Alle Tage im Zeitraum
            var DateCoursesList = new List<OwnDatesContract>();

            //Für jeden Tag die persönlichen Termine
            foreach (var day in days)
            {
                //neuer Tag für Liste erstellen
                var DateCourses = new OwnDatesContract();

                DateCourses.StatedDate = day.Date.ToString("dd.MM.yyyy");

                //Einzelnen Termine eines Tags
                var dates = new List<DateContract>();
                //Zeitraum in dem der Termin sein kann
                var FromTime = day.Date;
                var UntilTime = day.Date.AddDays(1);

                //Kürzere Abfrage, d.h. es muss nur an diesen tag sein da Zeitspanne  von 0-0 sehr groß ist für Performance
                //Gebuchten Termine die an diesem Tag anfangen und enden
                var activityofday = Db.Activities.Where(a =>
                a.Occurrence.Subscriptions.Any(u => u.UserId.Equals(UserId)) &&
                a.Dates.Any(d =>
                    (d.Begin >= FromTime && d.End <= UntilTime))).ToList();

                // jede Activiy durchgehen und den nächsten Termine bestimmen
                foreach (var activity in activityofday)
                {
                    dates.AddRange(GetDateContract(activity, day, UserId));
                }

                // Alternative: die Dates abfragen



                // Die Veranstaltungen des Dozenten
                var lectureActivities =
                    Db.Activities.Where(a =>
                        a.Dates.Any(d =>
                            d.Hosts.Any(l => !string.IsNullOrEmpty(l.UserId) && l.UserId.Equals(UserId)) &&
                            d.Begin >= FromTime && d.End <= UntilTime
                            )).ToList();

                foreach (var activity in lectureActivities)
                {
                    dates.AddRange(GetDateContract(activity, day, UserId));
                }


                //Termine zum Tag geordnet hinzufügen 
                DateCourses.Dates = dates.OrderBy(d => d.StartTime).ToList();






                //Defaultwert
                DateCourses.InfoString = null;

                //Falls an diesem Tag keine Termine gebucht
                if (DateCourses.Dates.FirstOrDefault() == null)
                {
                    //Meldung, dass keine Termine gebucht wurden
                    DateCourses.InfoString = "Derzeit keine Buchung!";
                    DateCourses.Dates = null;
                }
                //vor nächsten Tag, derzeitige Terminliste speichern
                DateCoursesList.Add(DateCourses);
            }

            //Order by statedDate
            return DateCoursesList.OrderBy(d => DateTime.Parse(d.StatedDate));
        }


        private List<DateContract> GetDateContract(Activity activity, DateTime day, string userId)
        {
            var dates = new List<DateContract>();

            // die möglichen Termine, die an diesen Tag beginnen
            var nextDates = activity.Dates.Where(d => d.Begin.Date == day.Date).OrderBy(d => d.Begin).ToList();

            //Alle Termine des Tages eines Fachs speichern
            foreach (var nextDate in nextDates)
            {
                if (nextDate != null)
                {
                    //Dozenten und Räume des Termins ermitteln
                    var LecturerList = new List<DateLecturerContract>();
                    //Dozenten
                    foreach (var host in nextDate.Hosts)
                    {
                        LecturerList.Add(new DateLecturerContract
                        {
                            LecturerId = host.Id.ToString() != null ? host.Id.ToString() : "N.N.",
                            LecturerName = host.Name != null ? host.Name : "N.N.",
                        });
                    }
                    LecturerList = LecturerList.OrderBy(l => l.LecturerName).ToList();

                    //Räume
                    var RoomList = new List<DateRoomContract>();

                    foreach (var room in nextDate.Rooms)
                    {
                        RoomList.Add(new DateRoomContract
                        {
                            RoomId = room.Id.ToString() != null ? room.Id.ToString() : "N.N.",
                            RoomNumber = room.Number != null ? room.Number : "N.N.",
                        });
                    }
                    RoomList = RoomList.OrderBy(r => r.RoomNumber).ToList();


                    // Bei Sprechstunden ggf. den Slot rausholen
                    var start = nextDate.Begin;
                    var end = nextDate.End;
                    if (nextDate.Activity is OfficeHour)
                    {
                        var slot = nextDate.Slots.FirstOrDefault(
                            x => x.Occurrence.Subscriptions.Any(s => s.UserId.Equals(userId)));

                        if (slot != null)
                        {
                            start = slot.Begin;
                            end = slot.End;
                        }
                    }


                    //Termin zu Liste hinzufügen
                    dates.Add(new DateContract
                    {
                        StartTime = start.TimeOfDay.ToString("hh\\:mm"),
                        EndTime = end.TimeOfDay.ToString("hh\\:mm"),
                        IsCanceled =  nextDate.Occurrence?.IsCanceled ?? false,
                        Titel = activity.Name,
                        Rooms = RoomList.Any() ? RoomList : null,
                        Lecturers = LecturerList.Any() ? LecturerList : null,
                    });
                }
            }

            return dates;
        }

        /// <summary>
        /// Gibt die info zu einem Termin zurück
        /// </summary>
        /// <param name="DateId"></param>
        /// <returns></returns>
        public DatesContract GetDateInfo(string DateId)
        {
            var dates = Db.ActivityDates.FirstOrDefault(d => d.Id.ToString().Equals(DateId));

            var date = new DatesContract();

            if (dates != null)
            {
                date.DateId = dates.Id.ToString();
                date.Title = dates.Activity.Name;
                date.Start = dates.Begin.TimeOfDay.ToString("hh\\:mm");
                date.End = dates.End.TimeOfDay.ToString("hh\\:mm");
                date.Date = dates.Begin.Date.ToString("dd.MM.yyyy");
                date.IsCanceled = dates.Occurrence?.IsCanceled ?? false;

                var lecturerList = new List<DateLecturerContract>();

                foreach (var host in dates.Hosts)
                {
                    lecturerList.Add(new DateLecturerContract
                    {
                        LecturerId = host.Id.ToString(),
                        LecturerName = host.Name,
                    });
                }
                date.Lecturers = lecturerList;

                var roomList = new List<DateRoomContract>();

                foreach (var host in dates.Hosts)
                {
                    roomList.Add(new DateRoomContract
                    {
                        RoomId = dates.Rooms.FirstOrDefault() != null ? dates.Rooms.First().Id.ToString() : "N.N.",
                        RoomNumber = dates.Rooms.FirstOrDefault() != null ? dates.Rooms.First().Number : "N.N.",

                    });
                }
                date.Rooms = roomList;

            }

            return date;
        }
    }
}