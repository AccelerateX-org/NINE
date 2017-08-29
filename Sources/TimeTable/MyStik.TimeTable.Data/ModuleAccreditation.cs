using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyStik.TimeTable.Data
{
    public class ModuleAccreditation
    {
        public ModuleAccreditation()
        {
            Groups = new HashSet<CurriculumGroup>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
    
        /// <summary>
        /// Die akkreditierte Modul
        /// </summary>
        public virtual CurriculumModule Module { get; set; }

        /// <summary>
        /// Zuordnung zu einer Anforderung
        /// Aspekt "Prüfung"
        /// </summary>
        public virtual CurriculumCriteria Criteria { get; set; }

        /// <summary>
        /// Zuordnung zu allen denkbaren, sinnvollen Studiengruppen
        /// Aspekt "Stundenplan" / "Lehrveranstaltung"
        /// </summary>
        public virtual ICollection<CurriculumGroup> Groups { get; set; }


    }
}
