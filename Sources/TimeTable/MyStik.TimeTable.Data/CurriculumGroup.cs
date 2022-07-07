using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyStik.TimeTable.Data
{
    public class CurriculumGroup
    {
        public CurriculumGroup()
        {
            //SemesterGroups = new HashSet<SemesterGroup>();
            CapacityGroups = new HashSet<CapacityGroup>();
            //Criterias = new HashSet<CurriculumCriteria>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        /// <summary>
        /// Name der Gruppe nach SPO - Kapazitäten werden auf der Ebene
        /// der Semstergruppen berücksichtigt
        /// Beispiele: 1, 2, 3, 3 TEC, 3 BIO
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Die Nummer des Semesters
        /// </summary>
        public int Term { get; set; }

        /// <summary>
        /// Gruppen, die von Studierenden belegbar sind
        /// Nicht belegbar sind z.B. WPM
        /// </summary>
        public bool IsSubscribable { get; set; }

        /// <summary>
        /// Zuordnung zu Studiengang
        /// </summary>
        public virtual Curriculum Curriculum { get; set; }


        /// <summary>
        /// Liste aller Kapazitätsgruppen
        /// mindestens 1
        /// </summary>
        public virtual ICollection<CapacityGroup> CapacityGroups { get; set; }
        
        /// <summary>
        /// Liste der Semestergruppen - veraltet
        /// </summary>
        // public virtual ICollection<SemesterGroup> SemesterGroups { get; set; }


        //public virtual ICollection<CurriculumCriteria> Criterias { get; set; }

        /// <summary>
        /// Liste der regeln
        /// </summary>
        //public virtual ICollection<CriteriaRule> Rules { get; set; }

    }
}
