using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyStik.TimeTable.Data
{
    public class Course : Activity
    {
        public Course()
        {
            ModuleCourses = new HashSet<ModuleCourse>();
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
        /// </summary>
        public virtual ICollection<ModuleCourse> ModuleCourses { get; set; } 
    }
}
