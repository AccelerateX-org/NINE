using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyStik.TimeTable.Data
{
    /// <summary>
    /// Ein ganzes Programm, an dem Studienangebote teilnehmen
    /// </summary>
    public class CurriculumProgram
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public string Name { get; set; }

        /// <summary>
        /// Die Selbstverwaltung
        /// </summary>
        public virtual Autonomy Autonomy { get; set; }

        public ICollection<CurriculumAccreditation> Accreditations { get; set; }
    }

    public class CurriculumAccreditation
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public virtual CurriculumProgram Program { get; set; }

        public virtual Curriculum Curriculum { get; set; }
    }
}
