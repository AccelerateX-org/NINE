using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyStik.TimeTable.Data
{
    /// <summary>
    /// Eine Einheit der Selbstverwaltung
    /// - Institution
    /// - Organiser
    /// - Curriculum
    ///
    /// Zuordnung zu Elementen
    /// - Assessments (Auswahlkommission)
    /// </summary>
    public class Autonomy
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public virtual ICollection<Committee> Committees { get; set; }

        public virtual ICollection<CommitteeCompetence> Competences { get; set; }
    }

    public class CommitteeCompetence
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public virtual Committee Committee { get; set; }
        public virtual Autonomy Autonomy { get; set; }
    }
}
