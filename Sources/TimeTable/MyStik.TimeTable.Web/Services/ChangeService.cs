using System;
using System.Collections.Generic;
using MyStik.TimeTable.Data;

namespace MyStik.TimeTable.Web.Services
{
    /// <summary>
    /// 
    /// </summary>
    public class ChangeService
    {
        private TimeTableDbContext _db;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dbContext"></param>
        public ChangeService(TimeTableDbContext dbContext)
        {
            _db = dbContext;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="date">Termin mit alten Informationen</param>
        /// <param name="newBegin">neuer Begin</param>
        /// <param name="newEnd">neues Ende</param>
        /// <param name="newRoomIds">Ids der neuen Räume</param>
        /// <returns>Das Change Objekt</returns>
        public ActivityDateChange CreateActivityDateChange(ActivityDate date, DateTime newBegin, DateTime newEnd, ICollection<Guid> newRoomIds)
        {
            var change = new ActivityDateChange
            {
                TimeStamp = DateTime.Now,
                Date = date,
                NewBegin = newBegin,
                NewEnd = newEnd,
                OldBegin = date.Begin,
                OldEnd = date.End,
                HasTimeChange = false
            };

            // was in jedem Fall benötigt wird
            // die userId muss der Aufrufer (ein Controller) setzen

            // Hat sich Anfang oder Ende geändert?
            if (date.Begin != newBegin || date.End != newEnd)
            {
                change.HasTimeChange = true;
            }

            // Raumänderungen prüfen
            // nur Räume nicht zeitliche Belegung!
            change.HasRoomChange = false;

            // Kopie der neuen Räume
            
            var roomIdList = newRoomIds != null ? new List<Guid>(newRoomIds) : new List<Guid>();

            // alle bisherigen Räume durchgehen
            // wenn einer nicht mehr in der neuen Liste ist, dann ist er wohl gelöscht
            foreach (var room in date.Rooms)
            {
                // der bisherige Raum ist auch in der neuen Liste vorhanden
                if (roomIdList.Contains(room.Id))
                {
                    // rausnehmen, muss nicht weiter geprüft werden
                    roomIdList.Remove(room.Id);
                }
                else
                {
                    // raum wurde entfernt
                    // neuen RoomChange anlegen
                    // unabhängig davon, ob sich sonst was am termin geändert hat
                    change.HasRoomChange = true;
                }
            }

            // die in der Liste verbliebenen Räume müssen wohl neu sein
            foreach (var roomId in roomIdList)
            {
                // ein neuer Raum
                change.HasRoomChange = true;
            }


            // statusänderung gibt es hier nicht
            change.HasStateChange = false;

            // sollte sich nichts geändert haben, dann auch den change nicht zurückgeben
            if (!change.HasTimeChange && !change.HasRoomChange)
                return null;

            return change;
        }


        internal ActivityDateChange CreateActivityDateStateChange(ActivityDate date, bool hasRoomChange)
        {
            var change = new ActivityDateChange
            {
                TimeStamp = DateTime.Now,
                Date = date,
                NewBegin = date.Begin,
                NewEnd = date.End,
                OldBegin = date.Begin,
                OldEnd = date.End,
                HasTimeChange = false,
                HasStateChange = true,
                HasRoomChange = hasRoomChange
            };

            return change;
        }

        internal ActivityDateChange CreateActivityDateRoomChange(ActivityDate date)
        {
            var change = new ActivityDateChange
            {
                TimeStamp = DateTime.Now,
                Date = date,
                NewBegin = date.Begin,
                NewEnd = date.End,
                OldBegin = date.Begin,
                OldEnd = date.End,
                HasTimeChange = false,
                HasStateChange = false,
                HasRoomChange = true
            };

            return change;
        }
    
    }

}