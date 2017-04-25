using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyStik.TimeTable.Data
{
    public class ModuleMapping
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public virtual CoursePlan Plan { get; set; }

        public virtual CurriculumModule Module { get; set; }

        public virtual Semester Semester { get; set; }

        public int? Mark { get; set; }

        public int Trial { get; set; }

    }
}
