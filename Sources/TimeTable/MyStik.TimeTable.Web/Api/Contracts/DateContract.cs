using System.Collections.Generic;

namespace MyStik.TimeTable.Web.Api.Contracts
{
    /// <summary>
    /// Contract zur Abfrage der Infos eines Termins
    /// </summary>
    public class DatesContract
    {
        /// <summary>
        /// Guid des Termins als string
        /// </summary>
        public string DateId { get; set; }
        /// <summary>
        /// Titel des Termins/Vorlesung
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// Uhrzeit des Starts des Termins am Tag "Date" im Format hh:mm
        /// </summary>
        public string Start { get; set; }
        /// <summary>
        /// Uhrzeit des Endes des Termins am Tag "Date" im Format hh:mm
        /// </summary>
        public string End { get; set; }
        /// <summary>
        /// Datum des Termins im Format dd.MM.yyyy
        /// </summary>
        public string Date { get; set; }
        /// <summary>
        /// Information, ob Vorlesung ausfällt (=true)
        /// </summary>
        public bool IsCanceled { get; set; }
        /// <summary>
        /// Liste der Räume, siehe DateRoomContract
        /// </summary>
        public IEnumerable<DateRoomContract> Rooms { get; set; }
        /// <summary>
        /// Liste der Dozenten, siehe DateLecturerContract
        /// </summary>
        public IEnumerable<DateLecturerContract> Lecturers { get; set; }
    }
    /// <summary>
    /// Conctract zur Abfrage des persönlichen Stundenplans
    /// </summary>
    public class OwnDatesContract
    {
        /// <summary>
        /// Ein Datum im Zeitraum des persönlichen Stundenplans im Format dd.MM.yyyy
        /// </summary>
        public string StatedDate { get; set; }
        /// <summary>
        /// Infostring zum Datum, d.h. bei keinem gebuchten Termin am Tag "Derzeit keine Buchung!", sonst "null"
        /// </summary>
        public string InfoString { get; set; }
        /// <summary>
        /// Liste der Termine am Tag(StatedDate), siehe DateContract
        /// </summary>
        public IEnumerable<DateContract>Dates {get;set;}
    }
    /// <summary>
    /// Contract über Infos eines Termins beim persönlichen Stundenplan
    /// </summary>
    public class DateContract
    {
        /// <summary>
        /// Uhrzeit des Starts des Termins am  StatedDate im Format hh:mm
        /// </summary>
        public string StartTime {get;set;}
        /// <summary>
        /// Uhrzeit des Endes des Termins am StatedDate im Format hh:mm
        /// </summary>
        public string EndTime {get;set;}
        /// <summary>
        /// Information, ob Vorlesung ausfällt (=true)
        /// </summary>
        public bool IsCanceled { get; set; }
        /// <summary>
        /// Titel des Termins
        /// </summary>
        public string Titel { get; set; }
        /// <summary>
        /// Liste der Räume, siehe DateRoomContract
        /// </summary>
        public IEnumerable<DateRoomContract> Rooms{get;set;}
        /// <summary>
        /// Liste der Dozenten, siehe DateLecturerContract
        /// </summary>
        public IEnumerable<DateLecturerContract> Lecturers { get; set; }
    }
    /// <summary>
    /// Contract über Rauminfos eines Termins
    /// </summary>
    public class DateRoomContract
    {
        /// <summary>
        /// Guid des Raums als string
        /// </summary>
        public string RoomId { get; set; }
        /// <summary>
        /// Nummer des Raums
        /// </summary>
        public string RoomNumber { get; set;}
    }
    /// <summary>
    /// Contract über Dozenteninfos eines Termins
    /// </summary>
    public class DateLecturerContract
    {
        /// <summary>
        /// Guid des Dozenten als string
        /// </summary>
        public string LecturerId { get; set; }
        /// <summary>
        /// Name des Dozenten
        /// </summary>
        public string LecturerName { get; set; }
    }
}
