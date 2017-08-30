using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyStik.TimeTable.Data
{
    /// <summary>
    /// Studiengang
    /// </summary>
    public class Curriculum
    {
        public Curriculum()
        {
            CurriculumGroups = new HashSet<CurriculumGroup>();
            GroupAliases = new HashSet<GroupAlias>();
            Modules = new HashSet<CurriculumModule>();
            Criterias = new HashSet<CurriculumCriteria>();
            Chapters = new HashSet<CurriculumChapter>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string ShortName { get; set; }

        /// <summary>
        /// Studiengangleiter
        /// </summary>
        public virtual ActivityOrganiser Organiser { get; set; }

        /// <summary>
        /// Studiengruppen - integraler Bestandteil
        /// </summary>
        public virtual ICollection<CurriculumGroup> CurriculumGroups { get; set; }

        /// <summary>
        /// Werden diese noch benötigt?
        /// </summary>
        public virtual ICollection<GroupAlias> GroupAliases { get; set; }

        /// <summary>
        /// Module
        /// raus!
        /// </summary>
        public virtual ICollection<CurriculumModule> Modules { get; set; }

        /// <summary>
        /// Anforderungen
        /// </summary>
        public virtual ICollection<CurriculumCriteria> Criterias { get; set; }

        /// <summary>
        /// Inhaltliche Struktur
        /// </summary>
        public virtual ICollection<CurriculumChapter> Chapters { get; set; }
    }
}
