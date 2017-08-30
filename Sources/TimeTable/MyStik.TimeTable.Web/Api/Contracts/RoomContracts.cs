using System.Collections.Generic;

namespace MyStik.TimeTable.Web.Api.Contracts
{
    /// <summary>
    /// Contract zur Abfrage der freie Räume
    /// </summary>
    public class FreeRoomContract
    {
        /// <summary>
        /// Guid des Raums als string
        /// </summary>
        public string RoomId { get; set; }
        /// <summary>
        /// Nummer des Raums
        /// </summary>
        public string RoomNumber { get; set; }
        /// <summary>
        /// Infostring zum Datum, d.h. bei keinem gebuchten Termin am Tag "Derzeit keine Buchung!", sonst "null"
        /// </summary>
        public string InfoString { get; set; }
        /// <summary>
        /// Datum des nächsten Termins im Format dd.MM.yyyy
        /// </summary>
        public string NextOccurrenceDate { get; set; }
        /// <summary>
        /// Uhrzeit des Starts des nächsten Termins am Tag "Date" im Format hh:mm
        /// </summary>
        public string NextOccurrenceTime { get; set; }
        /// <summary>
        /// Restliche freie Zeit hh:mm, wenn keine weitere Belegung vorhanden Maxwert von 99:99
        /// </summary>
        public string RemainingFreeTime { get; set; }
        /// <summary>
        /// Name des nächsten Termins, der im Raum stattfindet
        /// </summary>
        public string NextOccurrenceName { get; set; }
        /// <summary>
        /// Guid des nächsten Termins
        /// </summary>
        public string NextOccurrenceId { get; set; }

    }
    /// <summary>
    /// Contract zur Abfrage der freien Räume mit Zeitspanne
    /// </summary>
    public class FreeRoomTimespansContract
    {
        /// <summary>
        /// Guid des Raums als string
        /// </summary>
        public string RoomId { get; set; }
        /// <summary>
        /// Nummer des Raums
        /// </summary>
        public string RoomNumber { get; set; }
        /// <summary>
        /// Uhrzeit ab dem der Raum am Tag "Date" frei ist, im Format hh:mm
        /// </summary>
        public string FreeFrom { get; set; }
        /// <summary>
        /// Uhrzeit bis zu dem der Raum am Tag "Date" frei ist, im Format hh:mm
        /// </summary>
        public string FreeUntil { get; set; }
        /// <summary>
        /// Datum der Freizeit im Format dd.MM.yyyy
        /// </summary>
        public string FreeDate {get;set;}
        /// <summary>
        /// Name der nächsten Veranstaltung
        /// </summary>
        public string NextOccurrenceName { get; set; }
        /// <summary>
        /// Guid der nächsten Veranstaltung als string
        /// </summary>
        public string NextOccurrenceId { get; set; }

    }
    /// <summary>
    /// Contract zur Abfrage aller Räume
    /// </summary>
    public class AllRoomContract
    {
        /// <summary>
        /// Guid des Raums als sting
        /// </summary>
        public string RoomId { get; set; }
        /// <summary>
        /// Nummer des Raums
        /// </summary>
        public string RoomNumber { get; set; }
        /// <summary>
        /// Kapazität des Raums
        /// </summary>
        public int RoomCapacity { get; set; }
    }
    /// <summary>
    /// Contract zur Abfrage der Termine eines Raums
    /// </summary>
    public class RoomDateContract
    {
        /// <summary>
        /// Guid des Raums als sting
        /// </summary>
        public string RoomId { get; set; }
        /// <summary>
        /// Nummer des Raums
        /// </summary>
        public string RoomNumber { get; set; }
        /// <summary>
        /// Kapazität des Raums
        /// </summary>
        public int RoomCapacity { get; set; }
        /// <summary>
        /// Liste der Termine des Raums, siehe RoomDate
        /// </summary>
        public IEnumerable<RoomDate> Dates {get;set;}
    }
    /// <summary>
    /// Contract über Listenelement der Termine eines Raums
    /// </summary>
    public class RoomDate
    {
        /// <summary>
        /// Guid des Termins als string
        /// </summary>
        public string DateId { get; set; }
        /// <summary>
        /// Titel der Veranstaltung
        /// </summary>
        public string Titel { get; set; }
        /// <summary>
        /// Startzeitpunkt des Termins am Tag "Date", im Format hh:mm
        /// </summary>
        public string Start { get; set; }
        /// <summary>
        /// Endzeitpunkt des Termins am Tag "Date", im Format hh:mm
        /// </summary>
        public string End { get; set; }
        /// <summary>
        /// Datum des Termins im Format dd.MM.yyyy
        /// </summary>
        public string Date { get; set; }
        /// <summary>
        /// Information, ob die Vorlesung ausfällt (=true)
        /// </summary>
        public bool IsCanceled { get; set; }
        /// <summary>
        /// Liste der Dozenten, welche die Vorlesung halten, siehe RoomLecturer
        /// </summary>
        public IEnumerable<RoomLecturer> Lecturers {get;set;}

    }
    /// <summary>
    /// Contract über Listenelement der Dozenten eines Termins eines Raums
    /// </summary>
    public class RoomLecturer
    {
        /// <summary>
        /// Guid des Dozenten als string
        /// </summary>
        public string LecturerId {get;set;}
        /// <summary>
        /// Name des Dozenten
        /// </summary>
        public string LecturerName {get;set;}
    }
}
