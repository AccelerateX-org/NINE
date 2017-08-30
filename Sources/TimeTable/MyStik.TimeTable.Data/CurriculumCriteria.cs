using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyStik.TimeTable.Data
{
    public class CurriculumCriteria
    {
        public CurriculumCriteria()
        {
            Accreditations = new HashSet<ModuleAccreditation>();    
            Rules = new HashSet<CriteriaRule>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        /// <summary>
        /// Der zugehörige Studiengang
        /// </summary>
        public virtual Curriculum Curriculum { get; set; }

        /// <summary>
        /// Bezeichnung, z.B. Grundstudium
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Kurzbezeichnung, z.B. GS
        /// </summary>
        public string ShortName { get; set; }

        /// <summary>
        /// minimales ECTS-Ziel
        /// </summary>
        public int MinECTS { get; set; }

        /// <summary>
        /// maximales ECTS-Ziel
        /// </summary>
        public int MaxECTS { get; set; }


        /// <summary>
        /// Auswahl
        /// -1: alle
        /// 0: beliebig
        /// n: bestimmte Anzahl
        /// </summary>
        public int Option { get; set; }


        public virtual ICollection<ModuleAccreditation> Accreditations { get; set; }

        public virtual ICollection<CriteriaRule> Rules { get; set; } 
    }
}
