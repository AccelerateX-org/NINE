using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MyStik.TimeTable.Data
{
    public class Course : Activity
    {
        public Course()
        {
            Nexus = new HashSet<CourseModuleNexus>();
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
        /// Liste aller Modul-LVs zu der dieser Kurs gehört
        /// können theoretische mehrere sein, z.B. Pflicht und WPM
        /// Das wird jetzt überflüssig
        /// </summary>
        // public virtual ICollection<ModuleCourse> ModuleCourses { get; set; }

        public virtual ICollection<CourseModuleNexus> Nexus { get; set; }


        public virtual ICollection<ScriptPublishing> ScriptPublishings { get; set; }

    }
}
