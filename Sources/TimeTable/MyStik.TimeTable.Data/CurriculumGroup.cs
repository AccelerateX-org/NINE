using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyStik.TimeTable.Data
{
    public class CurriculumGroup
    {
        public CurriculumGroup()
        {
            SemesterGroups = new HashSet<SemesterGroup>();
            Modules = new HashSet<CurriculumModule>();
            CapacityGroups = new HashSet<CapacityGroup>();
            Accreditations = new HashSet<ModuleAccreditation>();
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
        public virtual ICollection<SemesterGroup> SemesterGroups { get; set; }

        /// <summary>
        /// Liste der Module
        /// m-n Beziehung
        /// Überflüssig! Wurde durch Akkreditierung ersetzt
        /// </summary>
        public virtual ICollection<CurriculumModule> Modules { get; set; }


        /// <summary>
        /// Liste der regeln
        /// </summary>
        public virtual ICollection<CriteriaRule> Rules { get; set; }

        /// <summary>
        /// Alle Akkreditierungen => Zugang zu Modulen
        /// </summary>
        public virtual ICollection<ModuleAccreditation> Accreditations { get; set; }

    }
}
