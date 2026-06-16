using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyStik.TimeTable.DataServices.IO.Contracts
{
    #region Entity Contracts
    public class ModuleContextApiContract
    {
        public string Institution { get; set; }
        public string Organiser { get; set; }
        public string Catalog { get; set; }
        public string Topic { get; set; }
    }

    public class ModuleEntityApiContract
    {
        public Guid Id { get; set; }
        public string Key { get; set; }
        public ModuleContextApiContract Context { get; set; }
    }
    #endregion

    #region Details
    public class ModuleDetailsApiContract : ModuleEntityApiContract
    {
       
        public List<ModuleIdentifierApiContract> Identifiers { get; set; }

        public List<ModuleEditorApiContract> Editors { get; set; }

        public List<ModuleUsageApiContract> Usages { get; set; }

        public List<ModuleInstructionApiContract> Instructions { get; set; }

        public List<ModuleChallengeApiContract> Challenges { get; set; }

        public List<ModuleObjectiveApiContract> Objectives { get; set; }
    }

    public class ModuleIdentifierApiContract
    {
        public string Language { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
    }

    public class ModuleEditorApiContract
    {
        public string MemberKey { get; set; }
        public string Name { get; set; }
    }

    public class ModuleUsageApiContract
    {
        public string InstructionKey { get; set; }
        public string ChipKey { get; set; }
    }



    public class ModuleInstructionApiContract
    {
        /// <summary>
        /// Kennzeichnung, z.B. nach Typ VO, UE oder nach Inhalt, z.B. Q1, Q2
        /// </summary>
        public string InstructionKey { get; set; }

        /// <summary>
        /// Erforderlich, wenn damit unterschiedliche Fächer gemeint sind
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// lehrformat nach ASPO, z.B. SU, Ü, Pra, Proj
        /// </summary>
        public string Format { get; set; }

        /// <summary>
        /// Angabe der Semesterwochenstunde
        /// </summary>
        public double ContactHours { get; set; }
     
    }

    public class ModuleChallengeApiContract
    {
        public string ChallengeKey { get; set; }

        public string Title { get; set; }

        public List<ModuleChallengeFractionApiContract> Fractions { get; set; }
    }

    public class ModuleChallengeFractionApiContract
    {
        public double Weight { get; set; }
        
        /// <summary>
        /// Prüfungsformat nach ASPO, z.B. schrP, mdlP, ModA
        /// </summary>
        public string Format { get; set; }
    }



    public class ModuleObjectiveApiContract
    {
        public string ObjectiveKey { get; set; }

        public string Target { get; set; }
        public string Description { get; set; }
        public string Competence { get; set; }
        /// <summary>
        /// Bloom etc.
        /// </summary>
        public string Taxonomy { get; set; }
        public int Level { get; set; }
    }
    #endregion

    #region Description (Semester Level)

    public class ModuleDescriptionApiContract
    {
        public string ModuleKey { get; set; }

        public string SemesterKey { get; set; }

        public List<ModuleTeachingApiContract> Teachings { get; set; }

        public List<ModuleExaminationApiContract> Examinations { get; set; }

        public List<ModuleAchievementApiContract> Achievements { get; set; }
    }

    public class ModuleTeachingApiContract
    {
        public string InstructionKey { get; set; }
        public string CourseKey { get; set; }
    }

    public class ModuleExaminationApiContract
    {
        public string ChallengeKey { get; set; }
        /// <summary>
        /// Die Hilfsmittel etc. wandern zur Prüfung
        /// </summary>
        public string ExamKey { get; set; }

        /// <summary>
        /// Provisorium: Beschreibung der Prüfung, z.B. "3 Stunden Klausur, Hilfsmittel: Taschenrechner, Formelsammlung"
        /// </summary>
        public string Description { get; set; }
    }
    public class ModuleAchievementApiContract
    {
        public string ObjectiveKey { get; set; }
        /// <summary>
        /// Ansatz: das ist Material, das gebucht werden kann, ein moodle Kurs, Bücher - noch sehr unklar
        /// </summary>
        public string MaterialKey { get; set; }
        /// <summary>
        /// provisorisch: Beschreibung der Leistung, z.B. "Nachweis von 80% Anwesenheit, 5 Hausaufgaben, Mitarbeit im Unterricht"
        /// </summary>
        public string Description { get; set; }
    }

    #endregion
}
