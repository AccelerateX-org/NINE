using System;
using System.Linq;
using MyStik.TimeTable.Data;

namespace MyStik.TimeTable.DataServices
{
    public class CascadingDeleteService
    {
        private TimeTableDbContext _db;

        public CascadingDeleteService(TimeTableDbContext db)
        {
            _db = db;
        }


        public void DeleteActivityDate(Guid id)
        {
            var date = _db.ActivityDates.SingleOrDefault(x => x.Id == id);

            if (date == null)
                return;

            DeleteOccurrence(date.Occurrence);


            // Änderungen löschen
            var changes = date.Changes.ToList();
            foreach (var dateChange in changes)
            {
                var notifications = dateChange.NotificationStates.ToList();

                foreach (var notificationState in notifications)
                {
                    _db.NotificationStates.Remove(notificationState);
                }

                _db.DateChanges.Remove(dateChange);
            }

            date.Hosts.Clear();
            date.Rooms.Clear();

            foreach (var vRoom in date.VirtualRooms.ToList())
            {
                _db.VirtualRoomAccesses.Remove(vRoom);
            }

            _db.ActivityDates.Remove(date);

            _db.SaveChanges();
        }


        private void DeleteOccurrence(Occurrence occ)
        {
            if (occ != null)
            {
                _db.Occurrences.Remove(occ);
            }
        }
    }
}
