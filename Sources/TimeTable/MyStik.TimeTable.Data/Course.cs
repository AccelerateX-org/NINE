using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MyStik.TimeTable.Data
{
    public class Course : Activity
    {
        public Course()
        {
            //Nexus = new HashSet<CourseModuleNexus>();
            Opportunities = new HashSet<SubjectOpportunity>();
        }

        [Display(Name="Link zu Moodle-Kurs")]
        public string UrlMoodleCourse { get; set; }

        [Display(Name = "Zugangsschlüssel für Moodle-Kurs")]
        public string KeyMoodleCourse { get; set; }

        /// <summary>
        /// Titel, abweichend vom Namen. Gedacht als "Marketingspruch"
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Kennzeichnung der Alternative
        /// </summary>
        public string Tag { get; set; }

        /// <summary>
        /// So ist jetzt das Konzept
        /// 1 Kurs kann nur noch zu einem Fach eines Moduls gehören
        /// Das Modul selbst kann vielen Studiengängen zugeordnet werden
        /// </summary>
        //public virtual  TeachingUnit TeachingUnit { get; set; }


        /// <summary>
        /// Liste aller Modul-LVs zu der dieser Kurs gehört
        /// können theoretische mehrere sein, z.B. Pflicht und WPM
        /// Das wird jetzt überflüssig
        /// </summary>
        // public virtual ICollection<ModuleCourse> ModuleCourses { get; set; }

        //public virtual ICollection<CourseModuleNexus> Nexus { get; set; }


        public virtual ICollection<ScriptPublishing> ScriptPublishings { get; set; }


        public virtual ICollection<SubjectOpportunity> Opportunities { get; set; }

        public virtual ICollection<TeachingDescription> Teachings { get; set; }
       
    }
}
