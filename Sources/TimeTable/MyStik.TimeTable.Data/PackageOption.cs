using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyStik.TimeTable.Data
{
    public class PackageOption
    {
        public PackageOption()
        {
            Requirements = new HashSet<CurriculumRequirement>();
        }


        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public string Name { get; set; }

        /// <summary>
        /// Der zugehörige Studiengang
        /// </summary>
        public virtual CurriculumPackage Package { get; set; }

        public virtual ICollection<CurriculumRequirement> Requirements { get; set; }

    }
}
