using System;
using System.Collections.Generic;
using System.Linq;
using MyStik.TimeTable.Data;
using MyStik.TimeTable.DataServices;
using MyStik.TimeTable.Web.Models;

namespace MyStik.TimeTable.Web.Services
{
    /// <summary>
    /// 
    /// </summary>
    public class RoomService
    {
        private TimeTableDbContext _db = new TimeTableDbContext();

        /// <summary>
        /// Alle Räume, die im angegebenen Zeitraum frei sind
        /// </summary>
        /// <param name="isRoomAdmin"></param>
        /// <param name="from">Beginn des Zeitraums</param>
        /// <param name="until">Ende des Zeitraums</param>
        /// <param name="orgId"></param>
        /// <returns>Liste der freien Räume</returns>
        public IEnumerable<Room> GetFreeRooms(Guid? orgId, bool isRoomAdmin, DateTime from, DateTime until)
        {
            if (orgId.HasValue)
            {
                if (isRoomAdmin)
                {
                    return _db.Rooms.Where(r =>
                        r.Assignments.Any(a => a.Organiser.Id == orgId.Value) &&
                        !r.Dates.Any(d =>
                                (d.End > @from && d.End <= until) || // Veranstaltung endet im Zeitraum
                                (d.Begin >= @from && d.Begin < until) || // Veranstaltung beginnt im Zeitraum
                                (d.Begin <= @from && d.End >= until) // Veranstaltung zieht sich über gesamten Zeitraum
                        )).ToList();
                }

                return _db.Rooms.Where(r =>
                    r.Assignments.Any(a => a.Organiser.Id == orgId.Value && !a.InternalNeedConfirmation) &&
                    !r.Dates.Any(d =>
                            (d.End > @from && d.End <= until) || // Veranstaltung endet im Zeitraum
                            (d.Begin >= @from && d.Begin < until) || // Veranstaltung beginnt im Zeitraum
                            (d.Begin <= @from && d.End >= until) // Veranstaltung zieht sich über gesamten Zeitraum
                    )).ToList();
            }

            return _db.Rooms.Where(r =>
                !r.Dates.Any(d =>
                        (d.End > @from && d.End <= until) || // Veranstaltung endet im Zeitraum
                        (d.Begin >= @from && d.Begin < until) || // Veranstaltung beginnt im Zeitraum
                        (d.Begin <= @from && d.End >= until) // Veranstaltung zieht sich über gesamten Zeitraum
                )).ToList();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="allAvailable"></param>
        /// <returns></returns>
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
        /// 
        /// </summary>
        /// <param name="orgId"></param>
        /// <param name="isRoomAdmin"></param>
        /// <param name="dayList"></param>
        /// <param name="fromTime"></param>
        /// <param name="untilTime"></param>
        /// <returns></returns>
        internal IEnumerable<Room> GetFreeRooms(Guid? orgId, bool isRoomAdmin, List<DateTime> dayList, TimeSpan fromTime, TimeSpan untilTime)
        {
            // ich fange mit allen an!
            List<Room> rooms = null;
            if (orgId.HasValue)
            {
                if (isRoomAdmin)
                {
                    rooms = _db.Rooms.Where(r =>
                        r.Assignments.Any(a => a.Organiser.Id == orgId.Value)).ToList();
                }
                else
                {
                    rooms = _db.Rooms.Where(r =>
                        r.Assignments.Any(a => a.Organiser.Id == orgId.Value && !a.InternalNeedConfirmation)).ToList();
                }
            }
            else
            {
                rooms = _db.Rooms.ToList();
            }

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
            return rooms.ToList();
        }


        internal IEnumerable<Room> GetFreeRooms(DayOfWeek dayOfWeek, TimeSpan fromTime, TimeSpan untilTime, Semester semester, bool allAvailable, List<Room> allRooms)
        {
            var dayList = new SemesterService().GetDays(semester.Id, dayOfWeek);

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
        /// <param name="orgId">Id des Veranstalters</param>
        /// <param name="isOrgAdmin"></param>
        /// <returns>Liste der Räume des Veranstalters</returns>
        internal ICollection<Room> GetRooms(Guid orgId, bool isOrgAdmin)
        {
            if (isOrgAdmin)
                return _db.Rooms.Where(r =>
                    r.Assignments.Any(a => a.Organiser.Id == orgId) ||
                    !r.Assignments.Any()).ToList();

            return _db.Rooms.Where(r =>
                r.Assignments.Any(a => a.Organiser.Id == orgId && !a.InternalNeedConfirmation)).ToList();
        }

    
        internal ActivityDate GetCurrentDate(Room room)
        {
            var now = DateTime.Now;
            return room.Dates.FirstOrDefault(d => d.Begin <= now && d.End >= now);
        }

        internal ActivityDate GetCurrentDate(Room room, DateTime now)
        {
            return room.Dates.FirstOrDefault(d => d.Begin <= now && d.End >= now);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="room"></param>
        /// <returns></returns>
        internal ActivityDate GetNextDate(Room room)
        {
            var now = DateTime.Now;
            return room.Dates.Where(d => d.Begin >= now).OrderBy(d => d.Begin).FirstOrDefault();
        }


        internal ActivityDate GetNextDate(Room room, DateTime now)
        {
            return room.Dates.Where(d => d.Begin >= now).OrderBy(d => d.Begin).FirstOrDefault();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="orgId"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public ICollection<RoomInfoModel> GetAvaliableRoomsNow(Guid orgId, int offset=15)
        {
            var start = DateTime.Now;

            // der raum muss ab jetzt mindestens für eine Mindestanzahl an min frei sein, sonst macht es keinen Sinn

            var end = start.AddMinutes(offset);

            return GetAvaliableRooms(orgId, start, end);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="orgId"></param>
        /// <param name="offset"></param>
        /// <param name="duration"></param>
        /// <returns></returns>
        public ICollection<RoomInfoModel> GetAvaliableRoomsNext(Guid orgId, int offset = 15, int duration = 45)
        {
            var start = DateTime.Now.AddMinutes(offset);

            // der raum muss ab jetzt mindestens für eine Mindestanzahl an min frei sein, sonst macht es keinen Sinn

            var end = start.AddMinutes(duration);

            return GetAvaliableRooms(orgId, start, end);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="orgId"></param>
        /// <param name="from"></param>
        /// <param name="until"></param>
        /// <param name="isRoomAdmin"></param>
        /// <returns></returns>
        public ICollection<RoomInfoModel> GetAvaliableRooms(Guid orgId, DateTime from, DateTime until, bool isRoomAdmin = false)
        {
            var roomList = new List<RoomInfoModel>();

            List<Room> rooms;

            if (isRoomAdmin)
            {
                rooms = _db.Rooms.Where(room =>
                    room.Assignments.Any(a => a.Organiser.Id == orgId) &&
                    !room.Dates.Any(d =>
                            (d.End > @from && d.End <= until) || // Veranstaltung endet im Zeitraum
                            (d.Begin >= @from && d.Begin < until) || // Veranstaltung beginnt im Zeitraum
                            (d.Begin <= @from && d.End >= until) // Veranstaltung zieht sich über gesamten Zeitraum
                    )).ToList();
            }
            else
            {
                rooms = _db.Rooms.Where(room =>
                    room.Assignments.Any(a => a.Organiser.Id == orgId && !a.InternalNeedConfirmation) &&
                    !room.Dates.Any(d =>
                            (d.End > @from && d.End <= until) || // Veranstaltung endet im Zeitraum
                            (d.Begin >= @from && d.Begin < until) || // Veranstaltung beginnt im Zeitraum
                            (d.Begin <= @from && d.End >= until) // Veranstaltung zieht sich über gesamten Zeitraum
                    )).ToList();
            }

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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="orgId"></param>
        /// <returns></returns>
        public ICollection<RoomInfoModel> GetNextAvaliableRoomsNow(Guid orgId)
        {
            var start = DateTime.Now;

            // der raum muss ab jetzt mindestens für 15 min Frei sein, sonst macht es keinen Sinn

            var end = start.AddMinutes(15);

            return GetNextAvaliableRooms(orgId, start, end);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="orgId"></param>
        /// <param name="from"></param>
        /// <param name="until"></param>
        /// <returns></returns>
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


        public ICollection<RoomInfoModel> GetAvailableLearningRooms(Guid orgId, DateTime at)
        {
            // Alle Lernräume
            var rooms = _db.Rooms.Where(room =>
                room.Assignments.Any(a => a.Organiser.Id == orgId && !a.InternalNeedConfirmation) &&
                room.IsForLearning).ToList();

            var roomList = new List<RoomInfoModel>();

            foreach (var room in rooms)
            {
                roomList.Add(new RoomInfoModel
                {
                    Room = room,
                    CurrentDate = GetCurrentDate(room, at),
                    NextDate = GetNextDate(room, at)
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

        internal RoomScheduleModel GetRoomSchedule(Guid id, Semester semester)
        {
            var room = _db.Rooms.SingleOrDefault(r => r.Id == id);

            var model = new RoomScheduleModel();
            model.Room = room;
            model.Semester = semester;

            var allDates = room.Dates.Where(x => x.Begin >= semester.StartCourses && x.End <= semester.EndCourses.AddDays(1))
                .OrderBy(x => x.Begin)
                .ToList();
            var ratio = 0;

            while (allDates.Any())
            {
                var date = allDates.First();
                var roomDateModel = new RoomDateSummaryModel
                {
                    Activity = date.Activity,
                    DayOfWeek = date.Begin.DayOfWeek,
                    Start = date.Begin,
                    End = date.End,
                    SlotCount = 1
                };

                roomDateModel.Dates.Add(date);

                var dayBegin = date.Begin.AddDays(7);
                var dayEnd = date.End.AddDays(7);

                while (dayBegin <= semester.EndCourses.AddDays(1))
                {
                    var matchingDates = allDates.Where(x =>
                        x.Activity.Id == date.Activity.Id &&
                        x.Begin == dayBegin && x.End == dayEnd).ToList();

                    foreach (var matchingDate in matchingDates)
                    {
                        roomDateModel.Dates.Add(matchingDate);
                        allDates.Remove(matchingDate);
                    }


                    dayBegin = dayBegin.AddDays(7);
                    dayEnd = dayEnd.AddDays(7);
                    roomDateModel.SlotCount++;
                }

                // regelmäßig oder nicht
                var frequency = roomDateModel.Dates.Count / (double)roomDateModel.SlotCount;
                if (frequency > ratio && roomDateModel.Dates.Count > 1)
                {
                    // regelmäßig
                    model.RegularDates.Add(roomDateModel);
                }
                else
                {
                    // einzeltermine
                    foreach (var activityDate in roomDateModel.Dates)
                    {
                        model.SingleDates.Add(activityDate);
                    }
                }

                allDates.Remove(date);
            }

            return model;
        }


        internal RoomScheduleModel GetRoomSchedule(Guid id, DateTime from, DateTime until)
        {
            var room = _db.Rooms.SingleOrDefault(r => r.Id == id);

            var model = new RoomScheduleModel();
            model.Room = room;
            model.Semester = new SemesterService().GetSemester(from);

            var allDates = room.Dates.Where(x => x.Begin >= from && x.End <= until.AddDays(1))
                .OrderBy(x => x.Begin)
                .ToList();
            var ratio = 0;

            while (allDates.Any())
            {
                var date = allDates.First();
                var roomDateModel = new RoomDateSummaryModel
                {
                    Activity = date.Activity,
                    DayOfWeek = date.Begin.DayOfWeek,
                    Start = date.Begin,
                    End = date.End,
                    SlotCount = 1
                };

                roomDateModel.Dates.Add(date);

                var dayBegin = date.Begin.AddDays(7);
                var dayEnd = date.End.AddDays(7);

                while (dayBegin <= until.AddDays(1))
                {
                    var matchingDates = allDates.Where(x =>
                        x.Activity.Id == date.Activity.Id &&
                        x.Begin == dayBegin && x.End == dayEnd).ToList();

                    foreach (var matchingDate in matchingDates)
                    {
                        roomDateModel.Dates.Add(matchingDate);
                        allDates.Remove(matchingDate);
                    }


                    dayBegin = dayBegin.AddDays(7);
                    dayEnd = dayEnd.AddDays(7);
                    roomDateModel.SlotCount++;
                }

                // regelmäßig oder nicht
                // 1 Woche => alle regelmäßig
                model.RegularDates.Add(roomDateModel);

                allDates.Remove(date);
            }

            return model;
        }

    }
}