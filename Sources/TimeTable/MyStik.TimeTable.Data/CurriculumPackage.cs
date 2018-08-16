using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyStik.TimeTable.Data
{
    /// <summary>
    /// Das Kriterienpaket
    /// </summary>
    public class CurriculumPackage
    {
        public CurriculumPackage()
        {
            Options = new HashSet<PackageOption>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public string Name { get; set; }

        /// <summary>
        /// Der zugehörige Studiengang
        /// </summary>
        public virtual Curriculum Curriculum { get; set; }

        /// <summary>
        /// Die Optionen, z.B. Studienrichtungem
        /// </summary>
        public virtual ICollection<PackageOption> Options { get; set; }

    }
}
