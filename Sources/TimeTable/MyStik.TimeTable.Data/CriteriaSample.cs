using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyStik.TimeTable.Data
{
    public class CriteriaSample
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public string Name { get; set; }

        /// <summary>
        /// Der zugehörige Studiengang
        /// </summary>
        public virtual CurriculumVariation Variation { get; set; }


        public virtual CurriculumCriteria Criteria { get; set; }


        public bool IsExcluded { get; set; }

    }
}
