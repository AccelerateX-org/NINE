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

        public string Tag { get; set; }

        public string Description { get; set; }

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
            ModuleSubjects = new HashSet<ModuleSubject>();
            ModuleExams = new HashSet<ModuleExam>();
            Accreditations = new HashSet<ModuleAccreditation>();
            Descriptions = new HashSet<ModuleDescription>();
        }


        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        /// <summary>
        /// Fachlicher Schlüssel (keine Beschränkung auf Identität)
        /// deprecrated
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

        /// <summary>
        /// Fachlicher Schlüssel
        /// </summary>
        public string Tag { get; set; }


        /// <summary>
        /// Fachverantwortlicher
        /// </summary>
        public virtual OrganiserMember MV { get; set; }


        public virtual CurriculumModuleCatalog Catalog { get; set; }


        /// <summary>
        /// Liste der Fächer
        /// </summary>
        public virtual ICollection<ModuleSubject> ModuleSubjects { get; set; }

        /// <summary>
        /// Liste aller Modulprüfungen
        /// </summary>
        public virtual ICollection<ModuleExam> ModuleExams { get; set; }

        /// <summary>
        /// Alle Akkreditierungen des Moduls
        /// </summary>
        public virtual ICollection<ModuleAccreditation> Accreditations { get; set; }

        public virtual ICollection<ModuleDescription> Descriptions { get; set; }
    }


    public class ModuleDescription
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }


        public virtual CurriculumModule Module { get; set; }

        public virtual Semester Semester { get; set; }

        public string Description { get; set; }



    }
}
