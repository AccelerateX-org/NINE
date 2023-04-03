using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyStik.TimeTable.Data
{
    /// <summary>
    /// Studiengang
    /// </summary>
    public class Curriculum
    {
        public Curriculum()
        {
            CurriculumGroups = new HashSet<CurriculumGroup>();
            Chapters = new HashSet<CurriculumChapter>();
            Sections = new HashSet<CurriculumSection>();
            Opportunities = new HashSet<CurriculumOpportunity>();
            Areas = new List<CurriculumArea>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public string Name { get; set; }

        /// <summary>
        /// der gebräuchliche Kurzname
        /// </summary>
        public string ShortName { get; set; }

        /// <summary>
        /// der technische Kurzname
        /// </summary>
        public string Tag { get; set; }

        /// <summary>
        /// Als Teilzeitmodell einsetzbar
        /// Zusatzoption
        /// </summary>
        public bool AsPartTime { get; set; }

        /// <summary>
        /// Weiterbildungsangebot
        /// ja oder nein, aber nicht beides
        /// </summary>
        public bool IsQualification { get; set; }

        /// <summary>
        /// Duales Studium
        /// Unterkategorien (werden aktuell nicht unterschieden)
        /// Verbundstidum
        /// Vertiefte Praxis
        /// Zusatzoption
        /// </summary>
        public bool AsDual { get; set; }

        /// <summary>
        /// Großer Schalter: alles was nicht published ist bekommen nur die Admins zu sehen
        /// </summary>
        public bool IsPublished { get; set; }

        /// <summary>
        /// Gilt nicht mehr, z.B. bei Erstsemestern
        /// </summary>
        public bool IsDeprecated { get; set; }

        /// <summary>
        /// Eine Idee der Versionierung
        /// z.B. für die interne Revision "beta"
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// geforderte ECTS
        /// </summary>
        public double EctsTarget { get; set; }

        public int ThesisDuration { get; set; }

        /// <summary>
        /// Gültig ab Semester
        /// </summary>
        // public virtual Semester ValidSince { get; set; }

        /// <summary>
        /// Der Abschluss
        /// da gibt es immer nur einen
        /// </summary>
        public virtual Degree  Degree { get; set; }

        /// <summary>
        /// Die Selbstverwaltung
        /// </summary>
        public virtual Autonomy Autonomy { get; set; }

        /// <summary>
        /// Studiengangleiter
        /// </summary>
        public virtual ActivityOrganiser Organiser { get; set; }

        /// <summary>
        /// Der Labelset definiert das, was es für den Studiengang gibt
        /// </summary>
        public virtual ItemLabelSet LabelSet { get; set; }

        /// <summary>
        /// Studiengruppen - integraler Bestandteil
        /// </summary>
        public virtual ICollection<CurriculumGroup> CurriculumGroups { get; set; }


        /// <summary>
        /// Inhaltliche Struktur
        /// veraltet
        /// </summary>
        public virtual ICollection<CurriculumChapter> Chapters { get; set; }



        public virtual ICollection<CurriculumSection> Sections { get; set; }

        public virtual ICollection<CurriculumOpportunity> Opportunities { get; set; }

        public virtual ICollection<CurriculumArea> Areas { get; set; }

    }

    public class CurriculumOpportunity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }


        public virtual Curriculum Curriculum { get; set; }

        public virtual Semester Semester { get; set; }

        public bool IsPublished { get; set; }
    }

    public class CurriculumArea
    {
        public CurriculumArea()
        {
            Options = new List<AreaOption>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public string Name { get; set; }

        /// <summary>
        /// der technische Kurzname
        /// </summary>
        public string Tag { get; set; }

        /// <summary>
        /// Beschreibung
        /// </summary>
        public string Description { get; set; }

        public virtual Curriculum Curriculum { get; set; }

        public virtual ICollection<AreaOption> Options { get; set; }
    }

    public class AreaOption
    {
        public AreaOption()
        {
            Slots = new List<CurriculumSlot>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public string Name { get; set; }

        /// <summary>
        /// der technische Kurzname
        /// </summary>
        public string Tag { get; set; }

        /// <summary>
        /// Beschreibung
        /// </summary>
        public string Description { get; set; }

        public virtual CurriculumArea Area { get; set; }

        public virtual ICollection<CurriculumSlot> Slots { get; set; }
    }
}
