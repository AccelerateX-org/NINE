using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyStik.TimeTable.Data
{
    public class CurriculumModuleCatalog
    {
        public CurriculumModuleCatalog()
        {
            Modules = new HashSet<CurriculumModule>();
        }


        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public string Name { get; set; }

        public virtual ActivityOrganiser Organiser { get; set; }

        public virtual ICollection<CurriculumModule> Modules { get; set; }

    }

    /// <summary>
    /// Das Fach
    /// </summary>
    public class CurriculumModule
    {
        public CurriculumModule ()
        {
            ModuleCourses = new HashSet<ModuleCourse>();
            ModuleExams = new HashSet<ModuleExam>();
            //Groups = new HashSet<CurriculumGroup>();
            Accreditations = new HashSet<ModuleAccreditation>();
        }


        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        /// <summary>
        /// Fachlicher Schlüssel (keine Beschränkung auf Identität)
        /// </summary>
        public string ModuleId { get; set; }

        /// <summary>
        /// Bezeichnung
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Kurzname des Moduls
        /// </summary>
        public string ShortName { get; set; }

        public string Tag { get; set; }

        /// <summary>
        /// Anzahl der Credits für das Modul
        /// </summary>
        //public int ECTS { get; set; }

        /// <summary>
        /// Inhaltsbeschreibung (Elemente, beliebig strukturiert, HTML formatiert)
        /// </summary>
        //public string Description { get; set; }

        //public string PreRequisites { get; set; }

        //public string Competences { get; set; }

        //public string Literature { get; set; }

        /// <summary>
        /// Zugehörige Studiengruppe
        /// Überflüssig!
        /// </summary>
        //public virtual ICollection<CurriculumGroup> Groups { get; set; }


        /// <summary>
        /// Fachverantwortlicher
        /// </summary>
        public virtual OrganiserMember MV { get; set; }


        public virtual CurriculumModuleCatalog Catalog { get; set; }


        /// <summary>
        /// Liste der akkreditierten Module
        /// </summary>
        public virtual ICollection<ModuleCourse> ModuleCourses { get; set; }

        /// <summary>
        /// Liste aller Modulprüfungen
        /// </summary>
        public virtual ICollection<ModuleExam> ModuleExams { get; set; }

        /// <summary>
        /// Alle Akkreditierungen des Moduls
        /// </summary>
       public virtual ICollection<ModuleAccreditation> Accreditations { get; set; }
    }
}
