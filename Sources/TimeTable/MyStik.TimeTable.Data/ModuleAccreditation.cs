using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyStik.TimeTable.Data
{
    public class ModuleAccreditation
    {
        public ModuleAccreditation()
        {
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        /// <summary>
        /// Der Kurzname innerhalb des Studiengangs
        /// </summary>
        public string ShortName { get; set; }

        /// <summary>
        /// Eine Art fachlicher Schlüssel im Modulkatalog des Studiengangs
        /// </summary>
        public string Number { get; set; }

        /// <summary>
        /// Ist es ein Pflichtmodul
        /// </summary>
        public bool IsMandatory { get; set; }

        //public virtual TeachingBuildingBlock TeachingBuildingBlock { get; set; }

        //public virtual CertificateSubject CertificateSubject { get; set; }

        /// <summary>
        /// Die akkreditierte Modul
        /// VERALTET
        /// </summary>
        public virtual CurriculumModule Module { get; set; }

        /// <summary>
        /// Zuordnung zu einer Anforderung
        /// Aspekt "Prüfung"
        /// VERALTET
        /// </summary>
        public virtual CurriculumCriteria Criteria { get; set; } 

        public virtual ItemLabelSet LabelSet { get; set; }


        public virtual CurriculumSlot Slot { get; set; }

        public List<ExaminationDescription> ExaminationDescriptions { get; set; } = new List<ExaminationDescription>();

        public List<TeachingDescription> TeachingDescriptions { get; set; } = new List<TeachingDescription>();
    }
}
