using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyStik.TimeTable.Data
{
    public class CourseModuleNexus
    {

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        /// <summary>
        /// Das Modul (oberhalb der Akkreditierung)
        /// </summary>
        public virtual CurriculumRequirement Requirement { get; set; }

        /// <summary>
        /// Die konkrete Lehrveranstaltung
        /// </summary>
        public virtual Course Course { get; set; }

        /// <summary>
        /// Die Lerneinheit (unterhalb der Akkreditierung)
        /// </summary>
        public virtual ModuleCourse ModuleCourse { get; set; }
    }
}
