using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyStik.TimeTable.Data
{
    public class Committee
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        /// <summary>
        /// deprecated
        /// </summary>
        public virtual Curriculum Curriculum { get; set; }


        public virtual Autonomy Autonomy { get; set; }


        public virtual ICollection<CommitteeMember> Members { get; set; }

        /// <summary>
        /// thematische Veranstaltungsreihe, z.B. "Sitzung", "Arbeitstreffen"
        /// </summary>
        public virtual ICollection<Meeting> Meetings { get; set; }
    }

    /// <summary>
    /// Mitgliedschaft im Gremium über die existierenden Rollen
    /// nicht direkt den User
    /// </summary>
    public class CommitteeMember
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public DateTime? From { get; set; }

        public DateTime? Until { get; set; }

        
        /// <summary>
        /// Vorsitz
        /// </summary>
        public bool HasChair { get; set; }

        /// <summary>
        /// Ist Stellverterter
        /// </summary>
        public bool IsSubstitute { get; set; }

        /// <summary>
        /// Stimmrecht
        /// </summary>
        public bool HasVotingRight { get; set; }


        public virtual Committee Committee { get; set; }

        /// <summary>
        /// Aktuell nur Member => veraltet, umhängen und dann löschen
        /// </summary>
        public virtual OrganiserMember Member { get; set; }

        /// <summary>
        /// Oder ein Student => veraltet => löschen
        /// </summary>
        public virtual Student Student { get; set; }

        /// <summary>
        /// Besser ale Stduent und Member
        /// den realen Status muss man eh anders festlegen
        /// </summary>
        public string UserId { get; set; }

    }
}
