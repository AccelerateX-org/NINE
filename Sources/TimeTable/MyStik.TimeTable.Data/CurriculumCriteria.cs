using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyStik.TimeTable.Data
{
    /// <summary>
    /// Der Modulbereich
    /// </summary>
    public class CurriculumCriteria
    {
        public CurriculumCriteria()
        {
            Accreditations = new HashSet<ModuleAccreditation>();    
            Rules = new HashSet<CriteriaRule>();
            Groups = new HashSet<CurriculumGroup>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        /// <summary>
        /// Der zugehörige Studiengang
        /// deprecated
        /// geht zukünftig über die Option
        /// </summary>
        //public virtual Curriculum Curriculum { get; set; }

        /// <summary>
        /// Die zugehörige Anforderung
        /// </summary>
        public virtual CurriculumRequirement Requirement { get; set; }

        /// <summary>
        /// Bezeichnung, z.B. Grundstudium
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Nummer des Fachsemesters
        /// </summary>
        public int Term { get; set; }

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


        /// <summary>
        /// Der fachliche Bereich, z.B. GOP, Studienrichtung, Modulbereich
        /// </summary>
        public virtual CurriculumChapter Chapter { get; set; }

        /// <summary>
        /// die zeitliche Anordnung
        /// es gilt 1 Kriterium ertsreckt sich genau über ein Semester
        /// </summary>
        public virtual ICollection<CurriculumGroup> Groups { get; set; }

        /// <summary>
        /// Die Akkreditierungen
        /// </summary>
        public virtual ICollection<ModuleAccreditation> Accreditations { get; set; }

        /// <summary>
        /// Das sind die zeitlichen Regeln, hier steckt die Verknüpfung zu den Gruppen
        /// </summary>
        public virtual ICollection<CriteriaRule> Rules { get; set; } 

    }
}
