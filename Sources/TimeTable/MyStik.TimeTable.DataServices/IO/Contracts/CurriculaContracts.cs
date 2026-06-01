using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyStik.TimeTable.DataServices.IO.Contracts
{
    public class CurriculumApiContract
    {
        public Guid Id { get; set; }

        public string InstitutionId { get; set; }

        public string OrganiserId { get; set; }

        /// <summary>
        /// Das ist der Tag, d.h. der primuss code
        /// </summary>
        public string CurriculumId { get; set; }
        // Es sollte ein Versionsdatum geben, wenn Id angegeben ist
        public string AmendmentId { get; set; }

        // Altrnative zu Id und Version
        public string Alias { get; set; }

        public string Name { get; set; }

        /// <summary>
        /// Bezeichnung des Abschluss
        /// </summary>
        public string Degree { get; set; }

        /// <summary>
        /// Einstufung des Abschlusses
        /// </summary>
        public string Level { get; set; }

        /// <summary>
        /// Definiert im ECTS
        /// </summary>
        public double CreditPoints { get; set; }

        /// <summary>
        /// Dauer in Jahren (so steht es im BayHIG)
        /// </summary>
        public double Duration { get; set; }

        /// <summary>
        /// Mit Integration in die berufliche Tätigkeit bzw. Kooperation mit Unternehmen
        /// </summary>
        public bool AsDual { get; set; }

        /// <summary>
        /// In Teilzeit
        /// </summary>
        public bool AsPartTime { get; set; }

        /// <summary>
        /// Weiterbildung (Master) bzw. Weiterqualifikation (Bachelor)
        /// </summary>
        public bool IsQualification { get; set; }

        /// <summary>
        /// Bisher ist keine Mehrsprachigkeit bekannt
        /// </summary>
        public string Language { get; set; }

        public bool InSummerTerm { get; set; }

        public bool InWinterTerm { get; set; }

        public List<CurriculumUnitApiContract> Units { get; set; }
    }

    public class CurriculumUnitApiContract
    {
        /// <summary>
        /// SlotId
        /// </summary>
        public Guid Id { get; set; }
        
        public string ThemeId { get; set; }

        public string OptionId { get; set; }

        public string SlotId { get; set; }

        public string Name { get; set; }

        public int Semester { get; set; }

        public double CreditPoints { get; set; }
    }

    public class CurriculumLoadingApiContract
    {
        // Schlüssel der Unit
        public string UnitId { get; set; }

        public List<CurriculumChipApiContract> Chips { get; set; }
    }

    public class CurriculumChipApiContract
    {
        public string SubjectId { get; set; }

        public double CreditPoints { get; set; }
    }
}
