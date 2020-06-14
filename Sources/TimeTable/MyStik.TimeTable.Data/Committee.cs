﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        /// Aktuell nur Member
        /// </summary>
        public virtual OrganiserMember Member { get; set; }

        /// <summary>
        /// Oder ein Student
        /// </summary>
        public virtual Student Student { get; set; }

    }
}
