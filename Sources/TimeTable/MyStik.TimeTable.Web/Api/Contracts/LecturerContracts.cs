using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;

namespace MyStik.TimeTable.Web.Api.Contracts
{
    /// <summary>
    /// Contract zur Abfrage von Dozenten
    /// </summary>
    public class LecturerInfoContract
    {
        /// <summary>
        /// Guid des Dozenten als string
        /// </summary>
        public string LecturerId { get; set; }
        /// <summary>
        /// Name des Dozenten
        /// </summary>
        public string LecturerName { get; set; }
        /// <summary>
        /// falls verfügbar, Büro des Dozenten
        /// </summary>
        public string OfficeRoom { get; set; }
        /// <summary>
        /// Liste der Termine des Dozenten, siehe Lectures
        /// </summary>
        public IEnumerable<DatesContract> Lectures { get; set; }

    }
    /// <summary>
    /// Contract über die Abfrage der Sprechstunden
    /// </summary>
    public class LecturerOfficeHourContract
    {
        /// <summary>
        /// Guid des Dozenten als string
        /// </summary>
        public string LecturerId { get; set; }
        /// <summary>
        /// Name des Dozenten
        /// </summary>
        public string LecturerName { get; set; }
        /// <summary>
        /// Guid des Raums des Dozenten als string
        /// </summary>
        public string LecturerRoomId { get; set; }
        /// <summary>
        /// Nummer des Raums
        /// </summary>
        public string LecturerRoomNumber { get; set; }
        /// <summary>
        /// Liste der Termine/Slots von Sprechstunden, siehe LecturerOfficeHourDateSlot
        /// </summary>
        public IEnumerable<LecturerOfficeHourDateSlot> OfficeHours { get; set; }

    }
    /// <summary>
    /// Contract für Listenelemente der Sprechstundentermine
    /// </summary>
    public class LecturerOfficeHourDateSlot
    {
        /// <summary>
        /// Id des Slots
        /// </summary>
        public string OfficeHourSlotId { get; set; }
        /// <summary>
        /// Startzeitpunkt
        /// </summary>
        public DateTime from { get; set; }
        /// <summary>
        /// Endzeitpunkt
        /// </summary>
        public DateTime until { get; set; }
        /// <summary>
        /// Anzahl an möglichen Buchungen
        /// </summary>
        public int NumberOfPossibleSubscribers { get; set; }
        /// <summary>
        /// Anzahl an derzeitigen Buchungen
        /// </summary>
        public int CurrentNumberOfSubscribers { get; set; }
        /// <summary>
        /// Startzeitpunkt, ab dem die Buchung möglich ist
        /// </summary>
        public DateTime? isBookablefrom {get;set;}
        /// <summary>
        /// Endzeitpunkt bis zu dem die Buchung möglich ist
        /// </summary>
        public DateTime? isBookableuntil {get;set;}

    }
    /// <summary>
    /// Contract zur Abfrage der Kurse eines Dozenten
    /// </summary>
    public class LecturerCoursesContract
    {
        /// <summary>
        /// Guid des Dozenten als string
        /// </summary>
        public string LecturerId { get; set; }
        /// <summary>
        /// Name des Dozenten
        /// </summary>
        public string LecturerName { get; set; }
        /// <summary>
        /// Kurszname des Dozenten
        /// </summary>
        public string LecturerShortname { get; set; }
        /// <summary>
        /// Liste der Vorlesungen eines Dozenten, siehe LectureCourses
        /// </summary>
        public IEnumerable<LectureCourses> LectureCourses { get; set; }
    }
    /// <summary>
    /// Contract für Listenelement der Kurse eines Dozenten
    /// </summary>
    public class LectureCourses
    {
        /// <summary>
        /// Guid der Vorlesung als sting
        /// </summary>
        public string LectureId { get; set; }
        /// <summary>
        /// Name der Vorlesung
        /// </summary>
        public string Title { get; set; }
    }
    /// <summary>
    /// Contract zur Abfrage der DOzenten
    /// </summary>
    public class LecturerContract
    {
        /// <summary>
        /// Guid des Dozenten als string
        /// </summary>
        public string LecturerId { get; set; }
        /// <summary>
        /// Name des Dozenten
        /// </summary>
        public string LecturerName { get; set; }
        /// <summary>
        /// Kurzname des Dozenten
        /// </summary>
        public string LecturerShortname { get; set; }
    }

}