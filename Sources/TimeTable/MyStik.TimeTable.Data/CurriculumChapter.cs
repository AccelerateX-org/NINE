using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyStik.TimeTable.Data
{
    public class CurriculumChapter
    {
        public CurriculumChapter()
        {
            Topics = new HashSet<CurriculumTopic>();
            Criteria = new HashSet<CurriculumCriteria>();
        }


        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public string Name { get; set; }

        /// <summary>
        /// Der zugehörige Studiengang
        /// </summary>
        public virtual Curriculum Curriculum { get; set; }

        /// <summary>
        /// Die Topics
        /// </summary>
        public virtual ICollection<CurriculumTopic> Topics { get; set; }


        public virtual ICollection<CurriculumCriteria> Criteria { get; set; }

    }
}
