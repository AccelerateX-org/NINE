using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyStik.TimeTable.Data
{
    public class SubjectAccreditation
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public virtual Semester Semester { get; set; }

        public virtual CurriculumSlot Slot { get; set; }

        /// <summary>
        /// Kennzeichnung der Alternative
        /// </summary>
        public string Tag { get; set; }

        /// <summary>
        /// Ein Slot kann mehrere Fächer umfassen
        /// </summary>
        public virtual  ICollection<TeachingUnit> Subjects { get; set; }
    }
}
