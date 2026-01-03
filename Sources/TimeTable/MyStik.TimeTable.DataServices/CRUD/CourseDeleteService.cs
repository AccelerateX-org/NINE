using MyStik.TimeTable.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyStik.TimeTable.DataServices.CRUD
{
    public class CourseDeleteService
    {
        private TimeTableDbContext _db;

        public CourseDeleteService(TimeTableDbContext db)
        {
            _db = db;
        }

        public bool DeleteCourse(Guid courseId)
        {
            var course = _db.Activities.SingleOrDefault(c => c.Id == courseId);
            if (course == null)
            {
                return false;
            }

            // Entfernen von Abhängigkeiten, falls vorhanden
            var subjectTeachings = _db.SubjectTeachings.Where(st => st.Course.Id == courseId).ToList();
            foreach (var teaching in subjectTeachings)
            {
                _db.SubjectTeachings.Remove(teaching);
            }
            var scriptPublishings = _db.ScriptPublishings.Where(sp => sp.Course.Id == courseId).ToList();
            foreach (var publishing in scriptPublishings)
            {
                _db.ScriptPublishings.Remove(publishing);
            }

            // jetzt noch die Aktivität löschen
            var activityService = new ActivityDeleteService(_db);

            return activityService.DeleteActivity(courseId);
        }

        public void DeleteActivityDate(Guid id, bool deleteOccurrence = true)
        {
            var date = _db.ActivityDates.SingleOrDefault(x => x.Id == id);

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
