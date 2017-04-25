using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyStik.TimeTable.Web.Api.Contracts;

namespace MyStik.TimeTable.Web.Api.Responses
{
    //Sprechstunde der Dozenten
    /// <summary>
    /// Response zur Abfrage der Sprechstunden
    /// </summary>
    public class LecturersOfficeHourResponse
    {
        /// <summary>
        /// Liste aller Sprechstunden, siehe LecturerOfficeHourContract
        /// </summary>
        public IEnumerable<LecturerOfficeHourContract> OfficeHours {get;set;}
    }
    //Sprechstunden eines Dozenten
    /// <summary>
    /// Response zur Abfrage der Sprechstundentermine eines Dozenten
    /// </summary>
    public class LecturerOfficeHoursResponse
    {
        /// <summary>
        /// Sprechstunden eines Dozenten, siehe LecturerOfficeHourContract
        /// </summary>
        public LecturerOfficeHourContract OfficeHours { get; set; }
    }
    /// <summary>
    /// Response zur Abfrage der Dozenten
    /// </summary>
    public class LecturersResponse
    {
        /// <summary>
        /// Liste aller Dozenten, siehe LecturerContract
        /// </summary>
        public IEnumerable<LecturerContract> Lecturers { get; set; }
    }
    /// <summary>
    /// Response zur Abfrage der Kurse eines Dozenten
    /// </summary>
    public class LecturerCoursesResponse
    {
        /// <summary>
        /// Alle Kurse, die ein Dozent hält, siehe LecturerCoursesContract
        /// </summary>
        public LecturerCoursesContract LecturerCourses { get; set; }
    }

}