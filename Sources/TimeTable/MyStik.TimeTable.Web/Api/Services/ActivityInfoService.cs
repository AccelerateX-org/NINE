using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyStik.TimeTable.Web.Api.Contracts;
using MyStik.TimeTable.Web.Services;
using MyStik.TimeTable.Data;

namespace MyStik.TimeTable.Web.Api.Services
{
    public class ActivityInfoService
    {
        protected readonly TimeTableDbContext Db = new TimeTableDbContext();

        //Persönliche Termine abfragen am Tagen abfragen
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

                //ALternative langsamere Abfrage
                ////Gebuchten Termine des Tages abrufen
                //var activityofday = Db.Activities.Where(a =>
                //a.Occurrence.Subscriptions.Any(u => u.UserId.Equals(userId)) &&
                //a.Dates.Any(d =>
                //    (d.Begin >= FromTime && d.End <= UntilTime) ||
                //    (d.Begin <= FromTime && d.End <= UntilTime) ||
                //    (d.Begin >= FromTime && d.End >= UntilTime) ||
                //    (d.Begin <= FromTime && d.End >= UntilTime))).ToList();

                // jede Activiy durchgehen und den nächsten Termine bestimmen
                foreach (var activity in activityofday)
                {
                    dates.AddRange(GetDateContract(activity, day));
                }

                // Die Veranstaltungen des Dozenten
                var lectureActivities =
                    Db.Activities.Where(a =>
                        a.Dates.Any(d =>
                            d.Hosts.Any(l => !string.IsNullOrEmpty(l.UserId) && l.UserId.Equals(UserId)) &&
                            d.Begin >= FromTime && d.End <= UntilTime
                            )).ToList();

                foreach (var activity in lectureActivities)
                {
                    dates.AddRange(GetDateContract(activity, day));
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


        private List<DateContract> GetDateContract(Activity activity, DateTime day)
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
                    LecturerList.OrderBy(l => l.LecturerName);

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
                    RoomList.OrderBy(r => r.RoomNumber);

                    //Termin zu Liste hinzufügen
                    dates.Add(new DateContract
                    {
                        StartTime = nextDate.Begin.TimeOfDay.ToString("hh\\:mm"),
                        EndTime = nextDate.End.TimeOfDay.ToString("hh\\:mm"),
                        IsCanceled = nextDate.Occurrence.IsCanceled,
                        Titel = activity.Name,
                        Rooms = RoomList.Any() ? RoomList : null,
                        Lecturers = LecturerList.Any() ? LecturerList : null,
                    });
                }
            }

            return dates;
        }

        //Gibt die info zu einem Termin zurück
        public DatesContract GetDateInfo(string DateId)
        {
            var dates = Db.ActivityDates.Where(d => d.Id.ToString().Equals(DateId)).FirstOrDefault();

            var date = new DatesContract();

            if (dates != null)
            {
                date.DateId = dates.Id.ToString();
                date.Title = dates.Activity.Name;
                date.Start = dates.Begin.TimeOfDay.ToString("hh\\:mm");
                date.End = dates.End.TimeOfDay.ToString("hh\\:mm");
                date.Date = dates.Begin.Date.ToString("dd.MM.yyyy");
                date.IsCanceled = dates.Occurrence.IsCanceled;

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