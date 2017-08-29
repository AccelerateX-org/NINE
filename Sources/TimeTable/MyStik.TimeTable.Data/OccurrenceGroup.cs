using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyStik.TimeTable.Data
{
    public class OccurrenceGroup
    {
        public OccurrenceGroup()
        {
            this.SemesterGroups = new HashSet<SemesterGroup>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        /// <summary>
        /// Zuordnung zur Occurrence
        /// </summary>
        public virtual Occurrence Occurrence { get; set; }

        /// <summary>
        /// Anzahl Teilnehmer für alle Gruppen
        /// </summary>
        public int Capacity { get; set; }

        
        /// <summary>
        /// Wenn gesetzt, dann muss nur der Studiengang passen
        /// Sonst auch die Gruppe
        /// Wird nicht benutzt, sondern global Schalter auf der Ebene der Occurrence
        /// </summary>
        public bool FitToCurriculumOnly { get; set; }

        /// <summary>
        /// Semestergruppen, die teilnehmen dürfen
        /// </summary>
        public virtual ICollection<SemesterGroup> SemesterGroups { get; set; }

    }
}
