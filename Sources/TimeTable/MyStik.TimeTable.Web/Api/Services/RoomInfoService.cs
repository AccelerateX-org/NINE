using System;
using System.Collections.Generic;
using System.Linq;
using MyStik.TimeTable.Web.Api.Contracts;
using MyStik.TimeTable.Web.Services;
using MyStik.TimeTable.Data;

namespace MyStik.TimeTable.Web.Api.Services
{
    /// <summary>
    /// 
    /// </summary>
    public class RoomInfoService
    {
        /// <summary>
        /// Ermittelt alle Räume des R-Baus, die zum aktuellen Zeitpunkt noch mindestens x-Minuten frei sind
        /// </summary>
        /// <param name="code"></param>
        /// <param name="minutes">Zeitraum, in der ein Raum mindestestens frei sein muss.</param>
        /// <returns>Informationen zu den freien Räumen</returns>
        public IEnumerable<FreeRoomContract> GetFreeRoomsNow(string code, int minutes=30)
        {
            //Initialisierung des RoomService von NINE
            var roomService = new RoomService();

            // Alle Räume, die jetzt noch mindestens ... frei sind
            // auf den nächsten 15 min Slot stützen
            var atTime = DateTime.Now;
            var minFree = new TimeSpan(0, 0, minutes, 0);
            var factor = (int)Math.Round((double)(atTime.Minute % 15) / 15, 0);
            var delta = factor * 15 - atTime.Minute % 15;

            var from = atTime.AddMinutes(delta).AddSeconds(-atTime.Second);
            var until = from.Add(minFree);

            //Nutzung des RoomService durch Abfrage der freien Räume im Zeitraum "from" bis "until" und Zwischenspeicherung dieser als Liste roomList
            var roomList = roomService.GetFreeRooms(null, false, from, until);

            //Erstellen einer neuen Liste mit FreeRoomContract-Elementen zur späteren Rückgabe
            var roomContracts = new List<FreeRoomContract>();
            //Schleife, die jeden Raum der Liste durchgeht
            foreach (var room in roomList)
            {
                var nextDate = room.Dates.Where(occ => occ.Begin >= from).OrderBy(occ => occ.Begin).FirstOrDefault();

                DateTime? ok = null;

                //1.Fall: Wenn es ein Termin gibt der stattfindet und wenn der Termin heute ist
                if (nextDate != null && nextDate.Begin.Date == DateTime.Now.Date)
                {
                    //Da Termin existiert und heute ist, Datum übernehmen
                    ok = nextDate.Begin;

                    //Berechnung der restlichen freien Zeit
                    TimeSpan Span = (nextDate.Begin - DateTime.Now);

                    //Hinzufügen eines neuen FreeRoomContract-Elements zur späteren Response roomContracts
                    roomContracts.Add(new FreeRoomContract
                    {
                        //Hinzufügen der einzelnen vereinbarten Informationen
                        RoomId = room.Id.ToString(),
                        RoomNumber = room.Number,
                        InfoString = null,
                        NextOccurrenceDate = ok.Value.Date.ToString("dd.MM.yyyy"),
                        NextOccurrenceTime = ok.Value.TimeOfDay.ToString("hh\\:mm"),
                        NextOccurrenceId = nextDate.Id.ToString(),
                        NextOccurrenceName = nextDate.Activity.Name,
                        RemainingFreeTime = Span.ToString(@"hh\:mm"),

                    });
                }

                //2.Fall: der Raum hat keine zukünftigen Termine an diesem Tag, sondern später oder gar keine mehr
                else
                {
                    //Hinzufügen eines neuen FreeRoomContract-Elements zur späteren Response roomContracts
                    roomContracts.Add(new FreeRoomContract
                    {
                        //Hinzufügen der einzelnen Informationen
                        RoomId = room.Id.ToString(),
                        RoomNumber = room.Number,
                        //da am heutigen Tag keine Belegung mehr vorhanden, Informationen für den Infostring
                        InfoString = "Derzeit keine Belegung!",
                        NextOccurrenceDate = null,
                        NextOccurrenceTime = null,
                        NextOccurrenceId = null,
                        NextOccurrenceName = null,
                        //Zur Sortierung und Unterscheidung maximaler Wert "99:99" sodass immer an erster Stelle
                        RemainingFreeTime = "99:99",

                    });
                    
                }

            }
            //Sortierung der Responeliste und Rückgabe an Controller
            //Zuerst Sortierung nach allen Räumen die den ganzen Tag frei sind und dann nach der übrigen Freetime und zum Schluss nach der Raumnummer
            return roomContracts.OrderByDescending(r => r.RemainingFreeTime).ThenBy(r => r.RoomNumber);
        }




        /// <summary>
        /// Gibt alle Räume, die einen bestimmten Zeitraum frei sind zurück
        /// </summary>
        /// <param name="from"></param>
        /// <param name="until"></param>
        /// <returns></returns>
        public IEnumerable<FreeRoomContract> GetFreeRooms(DateTime from, DateTime until)
        {
            var roomService = new RoomService();

            var roomList = roomService.GetFreeRooms(null, false, from, until);

            var roomContracts = new List<FreeRoomContract>();

            foreach (var room in roomList)
            {
                var next = room.Dates.Where(occ => occ.Begin >= from).OrderBy(occ => occ.Begin).FirstOrDefault();
                DateTime? ok = null;
                if (next != null)
                    ok = next.Begin;

                roomContracts.Add(new FreeRoomContract
                {
                    RoomId = room.Id.ToString(),
                    RoomNumber = room.Number,
                    //NextOccurrence = ok,
                    NextOccurrenceId = next.Id.ToString(),
                    NextOccurrenceName = next.Activity.Name,

                });
            }
            return roomContracts.OrderBy(r => r.RoomNumber);
        }

        /// <summary>
        /// Gibt alle freien Räume zurück
        /// </summary>
        /// <returns></returns>
        public IEnumerable<AllRoomContract> GetAllRooms()
            {
            var roomService = new RoomService();

            var roomList = roomService.GetAllRooms(false);

            var roomContracts = new List<AllRoomContract>();
        
            foreach (var room in roomList)
                {
                    roomContracts.Add(new AllRoomContract
                        {
                            RoomId = room.Id.ToString(),
                            RoomNumber = room.Number,
                            RoomCapacity =room.Capacity,

                        });
                }
            return roomContracts.OrderBy(r => r.RoomNumber);
            }

        /// <summary>
        /// alle räume die mit bestimmten Anfangsbuchstaben beginnen
        /// </summary>
        /// <param name="startsWith"></param>
        /// <returns></returns>
        public IEnumerable<AllRoomContract>GetRoomsStartWith(string startsWith)
        {
            var roomService = new RoomService();

            var roomList = roomService.GetAllRooms(true).Where(r => r.Number.StartsWith(startsWith)).ToList();

            var roomContracts = new List<AllRoomContract>();

            foreach (var room in roomList)
            {
                roomContracts.Add(new AllRoomContract
                    {
                        RoomId = room.Id.ToString(),
                        RoomNumber = room.Number,
                        RoomCapacity = room.Capacity,
                    });
            }
            return roomContracts.OrderBy(r => r.RoomNumber);
        }

        /// <summary>
        /// Abfrage der Termine eines Raumes
        /// </summary>
        /// <param name="roomId"></param>
        /// <returns></returns>
        public IEnumerable<RoomDateContract> GetRoomDates(string roomId)
        {
            var db = new TimeTableDbContext();

            var room = db.Rooms.Where(r => r.Id.ToString() == roomId).FirstOrDefault();

            var roomContract = new List<RoomDateContract>();

            if(room !=null)
            {
                var roominfo = new RoomDateContract();

                roominfo.RoomId = room.Id.ToString();
                roominfo.RoomNumber = room.Number;
                roominfo.RoomCapacity = room.Capacity;

                var roomDates = new List<RoomDate>();

                foreach (var date in room.Dates)
                {
                    var roomLecturers = new List<RoomLecturer>();

                    foreach(var lecturer in date.Hosts)
                    {
                        roomLecturers.Add(new RoomLecturer
                        {
                            LecturerId = lecturer.Id.ToString(),
                            LecturerName = lecturer.Name,
                        });
                    }

                    roomDates.Add(new RoomDate
                        {
                            DateId =date.Id.ToString(),
                            Titel =date.Activity.Name,
                            Start = date.Begin.TimeOfDay.ToString(@"hh\:mm"),
                            End = date.End.TimeOfDay.ToString(@"hh\:mm"),
                            Date = date.Begin.Date.ToString("dd.MM.yyyy"),
                            IsCanceled=date.Occurrence.IsCanceled,
                            Lecturers = roomLecturers,
                        });
                }
                roominfo.Dates = roomDates.OrderBy(r => r.Titel);

                roomContract.Add(roominfo);
            }
            return roomContract;
        }

        /// <summary>
        /// Abfrage Freizeiten der Räume die an einem gewählten Tag, die mindestens span-minuten frei sind
        /// </summary>
        /// <param name="span"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        public IEnumerable<FreeRoomTimespansContract> GetFreeRoomTimespan (TimeSpan span, DateTime date)
        {
            var db = new TimeTableDbContext();

            var day = date.Date;
            var roomContract = new List<FreeRoomTimespansContract>();

            var roomService = new RoomService();
            //var dateList = db.ActivityDates.Where(d => d.Begin.Day.Equals(day.Day)).GroupBy().ToList();
            var roomList = roomService.GetAllRooms(false);

            foreach (var room in roomList)
            {
                //ab 8Uhr morgens bis spätestens 20Uhr abends
                var firstBegin = day.AddHours(8);
                var lastBegin = day.AddHours(20);

                //Alle heutigen Activities des Raums 
                foreach( var activity in room.Dates.Where(r => r.Begin.Date.Equals(day.Date)).OrderBy(e=>e.Begin))
                {
                    //überprüfen ob zeitraum zwischen events groß genug ist
                    if((activity.Begin-firstBegin)>span && activity.Begin<lastBegin)
                    {
                        roomContract.Add(new FreeRoomTimespansContract
                        {
                            RoomId=room.Id.ToString(),
                            RoomNumber=room.Number,
                            FreeFrom = firstBegin.TimeOfDay.ToString("hh\\:mm"),
                            FreeUntil = activity.Begin.TimeOfDay.ToString("hh\\:mm"),
                            FreeDate = activity.Begin.Date.ToString("dd.MM.yyyy"),
                            NextOccurrenceId= activity.Id.ToString(),
                            NextOccurrenceName=activity.Activity.Name,
                        });

                        firstBegin = activity.End;

                    }
                    else
                    {
                        firstBegin = activity.End;
                    }
                }
            }
            return roomContract.OrderBy(r=>r.RoomNumber).ThenBy(r=>r.FreeFrom);
        }

        /// <summary>
        /// Fragt alle "Freizeiten" von Räumen in einer Woche ab (min X-min) von 8-20Uhr Mo-Fr
        /// </summary>
        /// <param name="span"></param>
        /// <param name="OneDayOfWeek"></param>
        /// <returns></returns>
        public IEnumerable<FreeRoomTimespansContract> GetFreeRoomTimesOfWeek (TimeSpan span, DateTime OneDayOfWeek)
        {
            //ersten Tag der Woche ermitteln
            var start = OneDayOfWeek;
            while (start.DayOfWeek!=DayOfWeek.Monday)
            {
                start = start.Subtract(new TimeSpan(1, 0, 0, 0));
            }
            var dateList = new List<DateTime>();

            var start2 = start;
            //Liste mit den Daten der 5 Tage
            while (start<start2.AddDays(5))
            {
                dateList.Add(start);
                start = start.AddDays(1);
            }

            var roomContract= new List<FreeRoomTimespansContract>();
            //jeder Wochentag
            foreach(var date in dateList)
            {
                //Abfrage der freien Zeiten der Tage
                var responseadd = GetFreeRoomTimespan(span, date);

                foreach(var freeRoom in responseadd)
                {
                    roomContract.Add(freeRoom);
                }
                
            }

            return roomContract;
        }
    }
}
