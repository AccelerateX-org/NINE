using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

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
    public class ModuleSubject
    {
        public ModuleSubject()
        {
            //Courses = new List<Course>();
            //CapacityCourses = new List<CapacityCourse>();
            //Nexus = new HashSet<CourseModuleNexus>();
            //Opportunities = new HashSet<SubjectOpportunity>();
        }


        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        /// <summary>
        /// optionale Bezeichnung
        /// da packen wir den Typ rein
        /// </summary>
        public string Name { get; set; }

        public string NameEn { get; set; }

        /// <summary>
        /// Typ der Lehrveranstaltung
        /// deprecrated
        /// </summary>
        public CourseType CourseType { get; set; }

        /// <summary>
        /// Lehrformat
        /// </summary>
        public virtual TeachingFormat TeachingFormat { get; set; }

        /// <summary>
        /// Anteil der SWS am Gesamtpaket
        /// </summary>
        public double SWS { get; set; }

        /// <summary>
        /// Fachbezeichnung in externer Quelle, z.B. gpUntis
        /// obsolet
        /// </summary>
        public string ExternalId { get; set; }


        public string Tag { get; set; }

        public string Alias => $"{Module.Name}";

        public string FullTag => $"{Module.FullTag}#{Tag}";

        public string FullName
        {
            get
            {
                var sb = new StringBuilder();

                if (TeachingFormat != null)
                {
                    if (!string.IsNullOrEmpty(Name))
                    {
                        sb.AppendFormat("{0} | {1} | {2} SWS", Name, TeachingFormat.Tag, SWS);
                    }
                    else
                    {
                        sb.AppendFormat("{0} | {1} SWS", TeachingFormat.Tag, SWS);
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(Name))
                    {
                        sb.AppendFormat("{0} | {1} SWS", Name, SWS);
                    }
                    else
                    {
                        sb.AppendFormat("LV | {0} SWS", SWS);
                    }
                }

                return sb.ToString();

            }
        }

        /// <summary>
        /// Das zugehörige Modul
        /// </summary>
        public virtual CurriculumModule Module { get; set; }


        //public virtual ICollection<SubjectOpportunity> Opportunities { get; set; }
        //public virtual ICollection<TeachingDescription> Teachings { get; set; }

        public virtual ICollection<SubjectAccreditation> SubjectAccreditations { get; set; }

        public virtual ICollection<SubjectTeaching> SubjectTeachings { get; set; }
    }

    /*
    public class SubjectOpportunity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }


        public virtual Semester Semester { get; set; }


        public virtual ModuleSubject Subject { get; set; }


        public virtual Course Course { get; set; }
    }
    */

    public class TeachingFormat
    {
        public TeachingFormat()
        {
            Subjects = new List<ModuleSubject>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Tag { get; set; }

        public string Description { get; set; }

        public int CWN { get; set; }

        public ICollection<ModuleSubject> Subjects { get; set; }
    }
}
