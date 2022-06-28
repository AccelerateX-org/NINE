using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

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
        public ModuleCourse()
        {
            //Courses = new List<Course>();
            CapacityCourses = new List<CapacityCourse>();
            Nexus = new HashSet<CourseModuleNexus>();
            Opportunities = new HashSet<SubjectOpportunity>();
        }


        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        /// <summary>
        /// optionale Bezeichnung
        /// da packen wir den Typ rein
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Typ der Lehrveranstaltung
        /// </summary>
        public CourseType CourseType { get; set; }

        /// <summary>
        /// Anteil der SWS am Gesamtpaket
        /// </summary>
        public int SWS { get; set; }

        /// <summary>
        /// Fachbezeichnung in externer Quelle, z.B. gpUntis
        /// obsolet
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
        // public virtual ICollection<Course> Courses { get; set; }

        /// <summary>
        /// Unterteilung in Parallelgruppen
        /// immmer mindestens 1
        /// </summary>
        public virtual ICollection<CapacityCourse> CapacityCourses { get; set; }

        public virtual ICollection<CourseModuleNexus> Nexus { get; set; }

        public virtual ICollection<SubjectOpportunity> Opportunities { get; set; }
    }


    public class SubjectOpportunity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }


        public virtual Semester Semester { get; set; }


        public virtual ModuleCourse Subject { get; set; }


        public virtual Course Course { get; set; }
    }
}
