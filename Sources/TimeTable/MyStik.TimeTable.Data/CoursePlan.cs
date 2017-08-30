using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyStik.TimeTable.Data
{
    public class CoursePlan
    {
        public CoursePlan()
        {
            ModuleMappings = new HashSet<ModuleMapping>();
            Semester = new List<Semester>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public string Name { get; set;}

        public string Description { get; set; }

        public bool IsFavorit { get; set; }

        public string UserId { get; set; }

        /// <summary>
        /// Achievements
        /// verbindung zu den Modulen
        /// </summary>
        public virtual ICollection<ModuleMapping> ModuleMappings { get; set; }

        /// <summary>
        /// Beginn des Plans
        /// </summary>
        public virtual ICollection<Semester> Semester { get; set; }

        /// <summary>
        /// Der zugehörige Studiengang
        /// </summary>
        public virtual Curriculum Curriculum { get; set; }
    }
}
