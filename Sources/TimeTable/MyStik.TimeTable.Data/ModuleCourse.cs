using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyStik.TimeTable.Data
{
    public enum CourseType
    {
        Seminar,
        Lecture,
        Practice
    }


    /// <summary>
    /// Lehrveranstaltung eines Moduls
    /// </summary>
    public class ModuleCourse
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        /// <summary>
        /// optionale Bezeichnung
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Typ der Lehrveranstaltung
        /// </summary>
        public CourseType CourseType { get; set; }

        /// <summary>
        /// Anzahl der SWS
        /// </summary>
        public int SWS { get; set; }

        /// <summary>
        /// Fachbezeichnung in externer Quelle, z.B. gpUntis
        /// </summary>
        public string ExternalId { get; set; }

        /// <summary>
        /// Das zugehörige Modul
        /// </summary>
        public virtual CurriculumModule Module { get; set; }

        
        /// <summary>
        /// Alle real stattfindenden Kurse
        /// Die Semesterzuordnung steckt im Kurs an sich über die Zuordnung
        /// zu den Semestergruppen. Muss daher hier nicht extra gemacht werden
        /// </summary>
        public virtual ICollection<Course> Courses { get; set; }
    }
}
