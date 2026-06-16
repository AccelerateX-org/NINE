using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyStik.TimeTable.DataServices.IO.Contracts
{

    #region Entity Contracts
    public class CurriculumContextApiContract
    {
        public string Institution { get; set; }
        public string Organiser { get; set; }
        public string Program { get; set; }
        /// <summary>
        /// yyyy-MM-dd
        /// </summary>
        public string Amendment { get; set; }
    }


    public class CurriculumEntityApiContract
    {
        public Guid Id { get; set; }
        public string Key { get; set; }
        public CurriculumContextApiContract Context { get; set; }    
}

    #endregion
    #region Details
    public class CurriculumDetailsApiContract : CurriculumEntityApiContract
    {
        // Alternative zu Id und Version
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
        
        public string SlotId { get; set; }

        public string Title { get; set; }

        public int Semester { get; set; }

        public double CreditPoints { get; set; }
    }
    #endregion

    public class CurriculumUnitDetailsApiContract
    {
        public Guid Id { get; set; }
        public string Key { get; set; }
        public string Title { get; set; } 
        public string Description { get; set; }

        public List<CurriculumChipApiContract> Alternatives { get; set; }
    }

    public class CurriculumInstanceApiContract
    {
        public string CurriculumKey { get; set; }
        public string SemesterKey { get; set; }
        public string Description { get; set; }

        public List<CurriculumModuleApiContract> Modules { get; set; }
    }

    public class CurriculumModuleApiContract
    {
        public string ModuleKey { get; set; }
    }



    public class CurriculumLoadingApiContract
    {
        // Schlüssel der Unit
        public string UnitId { get; set; }

        public List<CurriculumChipApiContract> Chips { get; set; }
    }

    public class CurriculumChipApiContract
    {
        public string ChipKey { get; set; }

        public string SubjectKey { get; set; }

        public double CreditPoints { get; set; }
    }
}
