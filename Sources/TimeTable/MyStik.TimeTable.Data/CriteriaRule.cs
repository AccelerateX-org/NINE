using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyStik.TimeTable.Data
{
    public class CriteriaRule
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public virtual CurriculumCriteria Criteria { get; set; }

        public virtual CurriculumGroup Group { get; set; }

        /// <summary>
        /// frühestens ab
        /// </summary>
        public bool? AtEarliest { get; set; }

        /// <summary>
        /// spätestens bis
        /// </summary>
        public bool? AtLatest { get; set; }

        // TOD hier noch andere Abhängigkeiten
    }
}
