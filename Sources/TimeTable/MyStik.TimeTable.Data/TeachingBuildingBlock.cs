using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyStik.TimeTable.Data
{
    /// <summary>
    /// Das Modul aus dem Modulkatalog
    /// </summary>
    public class TeachingBuildingBlock
    {

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
        /// Alle  Unterrichtseinheiten
        /// </summary>
        public virtual ICollection<TeachingUnit> TeachingUnits { get; set; }

        /// <summary>
        /// Alle Prüfungen
        /// </summary>
        public virtual ICollection<ExaminationUnit> ExaminationUnits { get; set; }

        /// <summary>
        /// Zuordnung zu Studiengängen
        /// </summary>
        public virtual ICollection<ModuleAccreditation> CurriculumModules { get; set; }

    }
}
