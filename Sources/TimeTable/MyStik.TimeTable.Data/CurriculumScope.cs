using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyStik.TimeTable.Data
{
    public class CurriculumScope
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public virtual ItemLabel Label { get; set; }

        public bool IsRequired { get; set; }

        /// <summary>
        /// Wählbar frühestens
        /// </summary>
        public int EarliestSection { get; set; }

        /// <summary>
        /// Wählbar spätestens
        /// </summary>
        public int LatestSection { get; set; }

        public virtual Curriculum Curriculum { get; set; }
    }
}
