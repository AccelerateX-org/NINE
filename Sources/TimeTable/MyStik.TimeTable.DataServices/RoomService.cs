using System;
using System.Linq;
using MyStik.TimeTable.Data;

namespace MyStik.TimeTable.DataServices
{
    public class RoomService
    {
        private TimeTableDbContext _db;

        public RoomService() { 
            _db = new TimeTableDbContext();
        }

        public RoomService(TimeTableDbContext db)
        {
            _db = db;
        }

        public PlanningGrid GetPlanningGrid(Guid roomId, Guid orgId, Guid? semId, Guid? segId, DateTime date)
       {
            var room = _db.Rooms.SingleOrDefault(r => r.Id == roomId);
            var org = _db.Organisers.SingleOrDefault(o => o.Id == orgId);
            var semester = semId.HasValue ? _db.Semesters.SingleOrDefault(s => s.Id == semId.Value) : null; 
            var segment = segId.HasValue ? _db.SemesterDates.SingleOrDefault(sg => sg.Id == segId.Value) : null;
            
            if (room == null || org == null)
                return null;

            if (room.Availability == null)
                return null;

            var grids = room.Availability.PlanningGrids.Where(x => x.Organiser != null && x.Organiser.Id == org.Id).ToList();

            // 1. Dauerhafte Verfügbarkeiten
            var grid = grids.FirstOrDefault(x => x.Semester == null && x.Segment == null);
            if (grid != null)
                return grid;

            // 2. Semesterbezogene Verfügbarkeiten
            if (semester != null && segment != null)
            {
                grid = grids.FirstOrDefault(x => x.Semester != null && x.Semester.Id == semester.Id && x.Segment != null && x.Segment.Id == segment.Id);
                // kein Raster für das Semester/Segment gefunden
                if (grid == null)
                    return null;

                // 3. Gültigkeitszeitraum prüfen
                // Im Semester dauerhaft gültig
                if (grid.ValidFrom == null && grid.ValidTo == null)
                    return grid;

                // Innerhalb des Gültigkeitszeitraums
                if (grid.ValidFrom != null && grid.ValidTo != null &&
                    grid.ValidFrom.Value <= date && date <= grid.ValidTo.Value)
                    return grid;

                // Ausserhalb des Gültigkeitszeitraums
            }

            // 2. Semesterbezogene Verfügbarkeiten
            if (semester != null && segment == null)
            {
                grid = grids.FirstOrDefault(x => x.Semester != null && x.Semester.Id == semester.Id && x.Segment == null);
                // kein Raster für das Semester/Segment gefunden
                if (grid == null)
                    return null;

                // 3. Gültigkeitszeitraum prüfen
                // Im Semester dauerhaft gültig
                if (grid.ValidFrom == null && grid.ValidTo == null)
                    return grid;

                // Innerhalb des Gültigkeitszeitraums
                if (grid.ValidFrom != null && grid.ValidTo != null &&
                    grid.ValidFrom.Value <= date && date <= grid.ValidTo.Value)
                    return grid;

                // Ausserhalb des Gültigkeitszeitraums
            }


            return null;
        }

        public RoomBooking CreateRoomBooking(Guid roomId, Guid dateId, Guid memberId)
        {
            var room = _db.Rooms.SingleOrDefault(r => r.Id == roomId);
            var date = _db.ActivityDates.SingleOrDefault(d => d.Id == dateId);
            var member = _db.Members.SingleOrDefault(m => m.Id == memberId);

            // Buchung anlegen
            var booking = new RoomBooking
            {
                Room = room,
                Date = date,
                Booker = member,
                BookingDate = DateTime.Now,
                IsCreated = true,
            };

            // jetzt noch den Buchungsstatus erstellen
            // Die Verfügbarkeit bestimmen
            // liegt keine oder keine gültige Verfügbakeit vor, dann ist die Buchung abgelehnt
            // d.h. keine Bestätigung und kein Acknowledgement
            //
            // wenn eine gültige Verfügbarkeit vorliegt, dann
            // wenn Raum für alle buchbar, dann ist die Buchung bestätigt
            // wenn Raum nur für bestimmte Personen buchbar, dann ist die Buchung ausstehend => muss confirmed werden
            // in allen Fällen muss dann noch das Acknowledgement erfolgen

            var semester = date.Activity.Semester;
            if (semester == null)
            {
                semester = _db.Semesters.OrderByDescending(s => s.StartCourses).FirstOrDefault(s => s.StartCourses <= date.Begin);
            }

            var segment = date.Activity.Segment;
            var segmentId = segment != null ? segment.Id : (Guid?)null;

            var grid = GetPlanningGrid(roomId, date.Activity.Organiser.Id, date.Activity.Semester.Id, segmentId, DateTime.Today);

            if (grid == null)
            {
                // keine Verfügbarkeit => abgelehnt
                booking.IsConfirmed = false;
                booking.ConfirmationDate = DateTime.Now;
                booking.Confirmer = null;
                booking.ConfirmationComment = "Keine gültige Verfügbarkeit vorhanden";
            }
            else
            {
                var slot = grid.PlanningSlots.SingleOrDefault(s => (int)s.DayOfWeek == (int)date.Begin.DayOfWeek &&
                                                                 s.From <= date.Begin.TimeOfDay &&
                                                                 s.To >= date.End.TimeOfDay);  
                if (slot == null)
                {
                    booking.IsConfirmed = false;
                    booking.ConfirmationDate = DateTime.Now;
                    booking.Confirmer = null;
                    booking.ConfirmationComment = "außerhalb der Verfügbarkeit";
                }
                else
                {
                    // Slot gefunden
                    // Assignment prüfen
                    var assignment = room.Assignments.FirstOrDefault(x => x.Organiser != null && x.Organiser.Id == date.Activity.Organiser.Id);

                    if (!assignment.InternalNeedConfirmation)
                    {
                        // für alle buchbar => automatisch bestätigt
                        booking.IsConfirmed = true;
                        booking.ConfirmationDate = DateTime.Now;
                        booking.Confirmer = null;
                        booking.ConfirmationComment = "automatisch bestätigt";
                    }
                    else
                    {
                        // nur für bestimmte Personen buchbar => ausstehend
                        booking.IsConfirmed = null;
                    }
                }
            }

            _db.RoomBookings.Add(booking);
            _db.SaveChanges();

            return booking;
        }
    }
}