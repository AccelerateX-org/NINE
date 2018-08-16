using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyStik.TimeTable.Data
{
    /// <summary>
    /// Das Modul
    /// </summary>
    public class CurriculumRequirement
    {
        public CurriculumRequirement()
        {
            Criterias = new HashSet<CurriculumCriteria>();
            Nexus = new HashSet<CourseModuleNexus>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string ShortName { get; set; }

        /// <summary>
        /// ID innerhalb des Modulkatalogs
        /// </summary>
        public string CatalogId { get; set; }

        public double ECTS { get; set; }

        public double USCredits { get; set; }

        /// <summary>
        /// Das könnte auch mal die Summe werden
        /// </summary>
        public double SWS { get; set; }

        public virtual PackageOption Option { get; set; }

        /// <summary>
        /// Die Modulbereiche (Teile eines Moduls)
        /// </summary>
        public virtual ICollection<CurriculumCriteria> Criterias { get; set; }

        /// <summary>
        /// Modulverantwortlicher
        /// </summary>
        public virtual OrganiserMember LecturerInCharge { get; set; }

        public virtual ICollection<CourseModuleNexus> Nexus { get; set; }
    }
}
