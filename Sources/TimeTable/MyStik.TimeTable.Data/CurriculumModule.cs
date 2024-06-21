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
            CatalogResponsibilities = new List<CatalogResponsibility>();
        }


        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Tag { get; set; }

        public string Description { get; set; }

        public virtual ActivityOrganiser Organiser { get; set; }

        public virtual ICollection<CurriculumModule> Modules { get; set; }

        public virtual ICollection<CatalogResponsibility> CatalogResponsibilities { get; set; }

    }

    /// <summary>
    /// Das Fach
    /// </summary>
    public class CurriculumModule
    {
        public CurriculumModule ()
        {
            ModuleSubjects = new HashSet<ModuleSubject>();
            //Accreditations = new HashSet<ModuleAccreditation>();
            Descriptions = new HashSet<ModuleDescription>();
            ExaminationOptions = new List<ExaminationOption>();
            ModuleResponsibilities = new List<ModuleResponsibility>();
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
        /// Bezeichnung Englisch
        /// </summary>
        public string NameEn { get; set; }

        /// <summary>
        /// Kurzname des Moduls
        /// </summary>
        public string ShortName { get; set; }

        /// <summary>
        /// Fachlicher Schlüssel
        /// </summary>
        public string Tag { get; set; }


        /// <summary>
        /// Voraussetzungen
        /// </summary>
        public string Prerequisites { get; set; }

        /// <summary>
        /// Voraussetzungen
        /// </summary>
        public string PrerequisitesEn { get; set; }


        public string Applicableness { get; set; }
        
        public string ApplicablenessEn { get; set; }

        /// <summary>
        /// Hat Kursangebote
        /// </summary>
        public bool HasCourses { get; set; }


        public virtual CurriculumModuleCatalog Catalog { get; set; }

        public virtual ICollection<ModuleResponsibility> ModuleResponsibilities { get; set; }


        /// <summary>
        /// Liste der Fächer
        /// </summary>
        public virtual ICollection<ModuleSubject> ModuleSubjects { get; set; }

        /// <summary>
        /// Die sollten zum Slot verschoben werden
        /// </summary>
        public virtual ICollection<ExaminationOption> ExaminationOptions { get; set; }

        /// <summary>
        /// Alle Akkreditierungen des Moduls
        /// </summary>
        //public virtual ICollection<ModuleAccreditation> Accreditations { get; set; }

        public virtual ICollection<ModuleDescription> Descriptions { get; set; }

        public virtual ICollection<ModuleApplicability> Applicabilities { get; set; }

        public string FullTag
        {
            get
            {
                return $"{Catalog.Organiser.ShortName}#{Catalog.Tag}#{Tag}";
            }
        }
    }

    public class ModuleApplicability
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public virtual CurriculumModule ProvidingModule { get; set; }

        public virtual CurriculumModule ReceivingModule { get; set; }

          /// <summary>
          /// Ausgedrückt in %
          /// </summary>
        public int FitRate { get; set; }

        public string Description { get; set; }

    }



    public class ModuleDescription
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }


        public virtual CurriculumModule Module { get; set; }

        public virtual Semester Semester { get; set; }

        /// <summary>
        /// Komplette Inhaltsangabe inkl. Literatur und allem drum und dran
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Komplette Inhaltsangabe inkl. Literatur und allem drum und dran
        /// </summary>
        public string DescriptionEn { get; set; }


        public virtual ChangeLog ChangeLog { get; set; }
    }

    public class ExaminationDescription
    {
        public ExaminationDescription()
        {
            this.Examiners = new List<Examiner>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }


        //public virtual CurriculumModule Module { get; set; }

        public virtual Semester Semester { get; set; }

        /// <summary>
        /// Das ist die gewählte Prüfungsform
        /// </summary>
        public virtual ExaminationOption ExaminationOption { get; set; }

        /// <summary>
        /// Angabe von Details, wie der Dauer
        /// </summary>
        public int? Duration { get; set; }

        public virtual ChangeLog ChangeLog { get; set; }


        /// <summary>
        /// Beschreibung inkl. der Hilfsmittel
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Rahmenbedingungen
        /// </summary>
        public string Conditions { get; set; }

        /// <summary>
        /// Hilfsmittel
        /// </summary>
        public string Utilities { get; set; }


        /// <summary>
        /// Erstprüfer - es kann nur einen geben
        /// </summary>
        public virtual OrganiserMember FirstExminer { get; set; }
        
        /// <summary>
        /// Zweitprüfer - es kann nur einen geben
        /// </summary>
        public virtual OrganiserMember SecondExaminer { get; set; }

        
        //public virtual ModuleAccreditation Accreditation { get; set; }

        public virtual ICollection<Examiner> Examiners { get; set; }
    }




    /// <summary>
    /// Das Lehrangebot
    /// </summary>
    /*
    public class TeachingDescription
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public int MinCapacity { get; set; }

        public int MaxCapacity { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        ///  required
        /// </summary>
        public virtual Semester Semester { get; set; }

        /// <summary>
        /// optional
        /// </summary>
        public virtual ModuleSubject Subject { get; set; }

        /// <summary>
        /// required
        /// </summary>
        public virtual Course Course { get; set; }

        /// <summary>
        /// optional: für Module, die akkreditiert sind
        /// </summary>
        public virtual ModuleAccreditation Accreditation { get; set; }

        /// <summary>
        /// optional: für Module, die nicht akkreditiert sind
        /// </summary>
        public virtual Curriculum Curriculum { get; set; } 
    }
    */

    public class SubjectTeaching
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }


        /// <summary>
        /// optional
        /// </summary>
        public virtual ModuleSubject Subject { get; set; }

        /// <summary>
        /// required
        /// </summary>
        public virtual Course Course { get; set; }
    }

    /*
    public class SlotExecution
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }


        /// <summary>
        /// optional
        /// </summary>
        public virtual CurriculumSlot Slot { get; set; }

        /// <summary>
        /// required
        /// </summary>
        public virtual Course Course { get; set; }
    }
    */



    public class ModuleResponsibility
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }


        public virtual CurriculumModule Module { get; set; }

        /// <summary>
        /// Fachverantwortlicher
        /// </summary>
        public virtual OrganiserMember Member { get; set; }

        /// <summary>
        /// true: Modulverantwortlich => darf Liste administrieren
        /// false: lehrender => darf anbieten
        /// </summary>
        public bool IsHead { get; set; } = false;

    }

    public class CatalogResponsibility
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }


        public virtual CurriculumModuleCatalog Catalog { get; set; }

        /// <summary>
        /// Fachverantwortlicher
        /// </summary>
        public virtual OrganiserMember Member { get; set; }

        public bool IsHead { get; set; } = false;
    }

}
