using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace MyStik.TimeTable.Data
{
    /// <summary>
    /// Ein unabhängiges Modul
    /// Es hat keinen fachlichen Schlüssel, sondern steht für sich alleine
    /// Fachliche Schlüssel im Katalog und/oder den Zuordnungen zu einem Slot
    /// in einem Curriculum
    /// </summary>
    public class TeachingBuildingBlock
    {
        public TeachingBuildingBlock()
        {
            Lecturers = new HashSet<Lecturer>();
            TeachingUnits = new HashSet<TeachingUnit>();
            TeachingAssessments = new HashSet<TeachingAssessment>();
            Modules = new HashSet<TeachingModule>();
            Publishings = new HashSet<ModulePublishing>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        /// <summary>
        /// Bezeichnung
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Beschreibung
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Alle Dozenten (inkl. der Modulverantwortung)
        /// </summary>
        public virtual ICollection<Lecturer> Lecturers { get; set; }

        /// <summary>
        /// Alle  Unterrichtseinheiten = Fächer (Subjects)
        /// </summary>
        public virtual ICollection<TeachingUnit> TeachingUnits { get; set; }


        public virtual ICollection<TeachingAssessment> TeachingAssessments { get; set; }

        /// <summary>
        /// Zuordnung zu Studiengängen
        /// </summary>
        // public virtual ICollection<ModuleAccreditation> CurriculumModules { get; set; }


        /// <summary>
        /// Umsetzung der Module auf Semesterebene
        /// </summary>
        public virtual ICollection<TeachingModule> Modules { get; set; }


        public virtual ICollection<ModulePublishing> Publishings { get; set; }

    }
}
