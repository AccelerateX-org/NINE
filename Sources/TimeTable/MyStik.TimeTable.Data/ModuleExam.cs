using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyStik.TimeTable.Data
{
    public enum ExamType
    {
        SP,     // Schriftliche Prüfung
        PA,     // Projekt / Studienarbeit
        MP      // Mündliche Prüfung
    }

    /// <summary>
    /// Modul (Teil-)Prüfung
    /// </summary>
    public class ModuleExam
    {
        public ModuleExam()
        {
            Examiners = new HashSet<OrganiserMember>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }


        /// <summary>
        /// Typ der Prüfung
        /// </summary>
        public ExamType ExamType { get; set; }

        /// <summary>
        /// Gewicht
        /// </summary>
        public double Weight { get; set; }

        /// <summary>
        /// Bezeichnung aus externer Quelle, z.B. Prüfungsamt
        /// da packen wir zunächst den Typü rein
        /// </summary>
        public string ExternalId { get; set; }

        /// <summary>
        /// Das zugehörige Modul
        /// </summary>
        public virtual CurriculumModule Module { get; set; }


        /// <summary>
        /// Prüfungsdauer, z.B. 90 min, 6 Monate
        /// </summary>
        public TimeSpan? Duration { get; set; }

        /// <summary>
        /// zugelassene Prüfer
        /// </summary>
        public virtual ICollection<OrganiserMember> Examiners { get; set; }

        /// <summary>
        /// Die echten Prüfungen
        /// </summary>
        public virtual ICollection<Exam> Exams { get; set; }

    }
}
