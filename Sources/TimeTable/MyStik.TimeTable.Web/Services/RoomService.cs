using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MyStik.TimeTable.Data;
using MyStik.TimeTable.DataServices;
using MyStik.TimeTable.Web.Models;

namespace MyStik.TimeTable.Web.Services
{
    public class RoomService
    {
        private TimeTableDbContext _db = new TimeTableDbContext();

        /// <summary>
        /// Alle Räume, die im angegebenen Zeitraum frei sind
        /// </summary>
        /// <param name="from">Beginn des Zeitraums</param>
        /// <param name="until">Ende des Zeitraums</param>
        /// <param name="allAvailable"></param>
        /// <returns>Liste der freien Räume</returns>
        public IEnumerable<Room> GetFreeRooms(DateTime from, DateTime until, bool allAvailable)
        {
            if (allAvailable)
            {
                return _db.Rooms.Where(room => !room.Dates.Any(d =>
                    (d.End > @from && d.End <= until) || // Veranstaltung endet im Zeitraum
                    (d.Begin >= @from && d.Begin < until) || // Veranstaltung beginnt im Zeitraum
                    (d.Begin <= @from && d.End >= until) // Veranstaltung zieht sich über gesamten Zeitraum
                    )).ToList();
            }
            else
            {
                return _db.Rooms.Where(room => room.Capacity > 0 &&
                    !room.Dates.Any(d =>
                    (d.End > @from && d.End <= until) || // Veranstaltung endet im Zeitraum
                    (d.Begin >= @from && d.Begin < until) || // Veranstaltung beginnt im Zeitraum
                    (d.Begin <= @from && d.End >= until) // Veranstaltung zieht sich über gesamten Zeitraum
                    )).ToList();
            }

        }

        public IEnumerable<Room> GetAllRooms(bool allAvailable)
        {
            if (allAvailable)
            {
                return _db.Rooms.ToList();
            }
            else
            {
                return _db.Rooms.Where(r => r.Capacity > 0).ToList();
            }
        }

        /// <summary>
        /// Alle Räume, die im Zeitraum frei sind
        /// </summary>
        /// <param name="dayOfWeek"></param>
        /// <param name="fromTime"></param>
        /// <param name="untilTime"></param>
        /// <param name="semester"></param>
        /// <param name="allAvailable"></param>
        /// <returns></returns>
        internal IEnumerable<Room> GetFreeRooms(DayOfWeek dayOfWeek, TimeSpan fromTime, TimeSpan untilTime, Semester semester, bool allAvailable)
        {
            var dayList = new SemesterService().GetDays(semester.Name, dayOfWeek);

            // ich fange mit allen an!
            var rooms = _db.Rooms.ToList();

            foreach (var day in dayList)
            {
                DateTime @from = day.AddHours(fromTime.Hours).AddMinutes(fromTime.Minutes);
                DateTime until = day.AddHours(untilTime.Hours).AddMinutes(untilTime.Minutes);

                // alle belegten Räume
                var occRooms =
                    rooms.Where(r => r.Dates.Any(d => (
                        (d.End > @from && d.End <= until) || // Veranstaltung endet im Zeitraum
                        (d.Begin >= @from && d.Begin < until) || // Veranstaltung beginnt im Zeitraum
                        (d.Begin <= @from && d.End >= until) // Veranstaltung zieht sich über gesamten Zeitraum
                        ))).ToList();


                foreach (var occRoom in occRooms)
                {
                    rooms.Remove(occRoom);
                }

                // wenn kein Raum mehr übrig ist, dann kann man gleich aufhören
                if (!rooms.Any())
                    break;
            }

            // die die übrig bleiben sind wohl nicht besetzt!
            if (allAvailable)
            {
                return rooms;
            }

            return rooms.Where(r => r.Capacity > 0).ToList();
        }


        internal IEnumerable<Room> GetFreeRooms(DayOfWeek dayOfWeek, TimeSpan fromTime, TimeSpan untilTime, Semester semester, bool allAvailable, List<Room> allRooms)
        {
            var dayList = new SemesterService().GetDays(semester.Name, dayOfWeek);

            // ich fange mit allen an!
            var rooms = allRooms;

            foreach (var day in dayList)
            {
                DateTime @from = day.AddHours(fromTime.Hours).AddMinutes(fromTime.Minutes);
                DateTime until = day.AddHours(untilTime.Hours).AddMinutes(untilTime.Minutes);

                // alle belegten Räume
                var occRooms =
                    rooms.Where(r => r.Dates.Any(d => (
                        (d.End > @from && d.End <= until) || // Veranstaltung endet im Zeitraum
                        (d.Begin >= @from && d.Begin < until) || // Veranstaltung beginnt im Zeitraum
                        (d.Begin <= @from && d.End >= until) // Veranstaltung zieht sich über gesamten Zeitraum
                        ))).ToList();


                foreach (var occRoom in occRooms)
                {
                    rooms.Remove(occRoom);
                }

                // wenn kein Raum mehr übrig ist, dann kann man gleich aufhören
                if (!rooms.Any())
                    break;
            }

            // die die übrig bleiben sind wohl nicht besetzt!
            if (allAvailable)
            {
                return rooms;
            }

            return rooms.Where(r => r.Capacity > 0).ToList();
        }

        internal IEnumerable<Room> GetAllRooms(bool allAvailable, List<Room> allRooms)
        {
            if (allAvailable)
            {
                return allRooms.ToList();
            }
            else
            {
                return allRooms.Where(r => r.Capacity > 0).ToList();
            }
        }

        internal void AddAssignment(string roomNumber, string orgName, bool needInternal, bool needExternal)
        {
            var room = _db.Rooms.SingleOrDefault(r => r.Number.Trim().Equals(roomNumber));
            if (room == null)
                return;

            var org = _db.Organisers.SingleOrDefault(o => o.ShortName.Equals(orgName));
            if (org == null)
                return;

            var assignment = _db.RoomAssignments.SingleOrDefault(a => a.Organiser.Id == org.Id && a.Room.Id == room.Id);
            
            if (assignment == null)
            {
                assignment = new RoomAssignment
                {
                    Room = room,
                    Organiser = org
                };

                _db.RoomAssignments.Add(assignment);
            }

            assignment.InternalNeedConfirmation = needInternal;
            assignment.ExternalNeedConfirmation = needExternal;

            _db.SaveChanges();
        }

        /// <summary>
        /// Liefert alle Räume, die einem Veranstalter zugeordnet sine
        /// </summary>
        /// <param name="orgName">Kurzname des Veranstalters</param>
        /// <returns>Liste der Räume des Veranstalters</returns>
        internal ICollection<Room> GetRooms(string orgName, bool isOrgAdmin)
        {
            if (isOrgAdmin)
                return _db.Rooms.Where(r => 
                    r.Assignments.Any(a => a.Organiser.ShortName.Equals(orgName))).ToList();
            

            return _db.Rooms.Where(r => 
                r.Assignments.Any(a => a.Organiser.ShortName.Equals(orgName) && !a.InternalNeedConfirmation)).ToList();
        }


        internal ICollection<Room> GetRooms(Guid orgId, bool isOrgAdmin)
        {
            if (isOrgAdmin)
                return _db.Rooms.Where(r =>
                    r.Assignments.Any(a => a.Organiser.Id == orgId)).ToList();

            return _db.Rooms.Where(r =>
                r.Assignments.Any(a => a.Organiser.Id == orgId && !a.InternalNeedConfirmation)).ToList();
        }

    
        internal ActivityDate GetCurrentDate(Room room)
        {
            var now = GlobalSettings.Now;
            return room.Dates.FirstOrDefault(d => d.Begin <= now && d.End >= now);
        }

        internal ActivityDate GetNextDate(Room room)
        {
            var now = GlobalSettings.Now;
            return room.Dates.Where(d => d.Begin >= now).OrderBy(d => d.Begin).FirstOrDefault();
        }


        public ICollection<RoomInfoModel> GetAvaliableRoomsNow(Guid orgId, int offset=15)
        {
            var start = GlobalSettings.Now;

            // der raum muss ab jetzt mindestens für eine Mindestanzahl an min frei sein, sonst macht es keinen Sinn

            var end = start.AddMinutes(offset);

            return GetAvaliableRooms(orgId, start, end);
        }

        public ICollection<RoomInfoModel> GetAvaliableRoomsNext(Guid orgId, int offset = 15, int duration = 45)
        {
            var start = GlobalSettings.Now.AddMinutes(offset);

            // der raum muss ab jetzt mindestens für eine Mindestanzahl an min frei sein, sonst macht es keinen Sinn

            var end = start.AddMinutes(duration);

            return GetAvaliableRooms(orgId, start, end);
        }



        public ICollection<RoomInfoModel> GetAvaliableRooms(Guid orgId, DateTime from, DateTime until)
        {
            var roomList = new List<RoomInfoModel>();

            var rooms = _db.Rooms.Where(room => 
                room.Assignments.Any(a => a.Organiser.Id == orgId && !a.InternalNeedConfirmation) &&
                !room.Dates.Any(d =>
                    (d.End > @from && d.End <= until) || // Veranstaltung endet im Zeitraum
                    (d.Begin >= @from && d.Begin < until) || // Veranstaltung beginnt im Zeitraum
                    (d.Begin <= @from && d.End >= until) // Veranstaltung zieht sich über gesamten Zeitraum
                    )).ToList();

            foreach (var room in rooms)
            {
                roomList.Add(new RoomInfoModel
                {
                    Room = room,
                    //CurrentDate = GetCurrentDate(room), kann ja nicht sein, da ja oben nach freien Räumen gesucht wird
                    NextDate = GetNextDate(room)
                });
            }

            return roomList;
        }

        public ICollection<RoomInfoModel> GetNextAvaliableRoomsNow(Guid orgId)
        {
            var start = GlobalSettings.Now;

            // der raum muss ab jetzt mindestens für 15 min Frei sein, sonst macht es keinen Sinn

            var end = start.AddMinutes(15);

            return GetNextAvaliableRooms(orgId, start, end);
        }

        public ICollection<RoomInfoModel> GetNextAvaliableRooms(Guid orgId, DateTime from, DateTime until)
        {
            var roomList = new List<RoomInfoModel>();

            // alle Räume, die jetzt belegt sind
            var rooms = _db.Rooms.Where(room =>
                room.Assignments.Any(a => a.Organiser.Id == orgId) &&
                !room.Dates.Any(d =>
                    (d.End > @from && d.Begin <= from)
                    )).ToList();

            foreach (var room in rooms)
            {
                roomList.Add(new RoomInfoModel
                {
                    Room = room,
                    CurrentDate = GetCurrentDate(room), 
                    NextDate = GetNextDate(room)
                });
            }

            return roomList;
        }


        internal bool NeedInternalConfirmation(Room room, string orgName)
        {
            var assign = room.Assignments.FirstOrDefault(a => a.Organiser.ShortName.Equals(orgName));
            if (assign != null)
                return assign.InternalNeedConfirmation;
            return true;
        }
    }
}