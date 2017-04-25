using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyStik.TimeTable.Web.Api.Contracts
{
    /// <summary>
    /// Contract für Abfrage der verfügbaren Fakultäten
    /// </summary>
    public class FacultiesContracts
    {
        /// <summary>
        /// Guid der Fakultät als String
        /// </summary>
        public string FacultyId { get; set; }
        /// <summary>
        /// Name der Fakultät 
        /// </summary>
        public string FucultyName { get; set; }
        /// <summary>
        /// Kürzel der Fakultät z.B. FK09
        /// </summary>
        public string FucultyShortname { get; set; }

    }
    /// <summary>
    /// Contract zur Abfrage der Studienprogramme
    /// </summary>
    public class CurriculumStudyprogramContract
    {
        /// <summary>
        /// Guid des Studienprograms als string
        /// </summary>
        public string StudyprogramId { get; set; }
        /// <summary>
        /// Namen des Studienprogramms
        /// </summary>
        public string StudyprogramName { get; set; }
        /// <summary>
        /// Kurznamen des Studienprogramms
        /// </summary>
        public string StudyprogramShortname { get; set; }
    }
    /// <summary>
    /// Contract zur Abfrage der Studiengruppen
    /// </summary>
    public class CurriculumStudygroupsContract
    {
        /// <summary>
        /// Guid der Studiengruppe als string
        /// </summary>
        public string StudygroupId { get; set; }
        /// <summary>
        /// Name der Studiengruppe
        /// </summary>
        public string StudygroupName { get; set; }
    }
    /// <summary>
    /// Contract zur Abfrage der Kurse einer Studiengruppe
    /// </summary>
    public class CurriculumCourseContract
    {
        /// <summary>
        /// Guid der Vorlesung als string
        /// </summary>
        public string LectureId { get; set; }
        /// <summary>
        /// Name der Vorlesung
        /// </summary>
        public string Title { get; set; }

    }
    /// <summary>
    /// Conctract zur Abfrage der Termine eines Kurses
    /// </summary>
    public class CurriculumDateContract
    {
        /// <summary>
        /// Guid der Vorlesung als string
        /// </summary>
        public string LectureId { get; set; }
        /// <summary>
        /// Name der Vorlesung
        /// </summary>
        public string LectureName { get; set; }
        /// <summary>
        /// Liste der Termine einer Vorlesung, siehe CurriculumDate
        /// </summary>
        public IEnumerable<CurriculumDate> Dates { get; set; }

    }
    /// <summary>
    /// Contract von Terminen eines Kurses
    /// </summary>
    public class CurriculumDate
    {
        /// <summary>
        /// Liste der Räume, siehe CurriculumRoom
        /// </summary>
        public IEnumerable<CurriculumRoom> Rooms { get; set; }
        /// <summary>
        /// Liste der Dozenten, siehe CurriculumLecturer
        /// </summary>
        public IEnumerable<CurriculumLecturer>Lecturers {get;set;}
        /// <summary>
        /// Datum des Termins im Format dd.MM.yyyy
        /// </summary>
        public string Date {get;set;}
        /// <summary>
        /// Zeit des Begins der Vorlesung am Tag "Date"im Format hh:mm
        /// </summary>
        public string Begin {get;set;}
        /// <summary>
        /// Zeit des Endes der Vorlesung am Tag "Date"im Format hh:mm
        /// </summary>
        public string End {get;set;}
        /// <summary>
        /// Information, ob Vorlesung ausfällt (=true)
        /// </summary>
        public bool isCanceled { get; set; }
    }
    /// <summary>
    /// Contract über Dozenten eines Vorlesungstermins
    /// </summary>
    public class CurriculumLecturer
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
    /// <summary>
    /// Contract über Räume eines Vorlesungstermins
    /// </summary>
    public class CurriculumRoom
    {
        /// <summary>
        /// Guid des Raums als string
        /// </summary>
        public string RoomId { get; set; }
        /// <summary>
        /// Raumnummer des Raums
        /// </summary>
        public string RoomNumber { get; set; }
    }
}
