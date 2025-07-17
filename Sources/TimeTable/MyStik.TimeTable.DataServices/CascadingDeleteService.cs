using System;
using System.Data.Entity;
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


        public void DeleteActivityDate(Guid id, bool deleteOccurrence = true)
        {
            var date = _db.ActivityDates.Include(activityDate => activityDate.Occurrence).Include(activityDate1 => activityDate1.VirtualRooms).Include(activityDate2 =>
                activityDate2.Changes.Select(activityDateChange => activityDateChange.NotificationStates)).SingleOrDefault(x => x.Id == id);

            if (date == null)
                return;

            if (deleteOccurrence)
            {
                DeleteOccurrence(date.Occurrence);
            }


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

            var bookings = _db.RoomBookings.Where(x => x.Date.Id == date.Id).ToList();

            foreach (var booking in bookings)
            {
                _db.RoomBookings.Remove(booking);
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
