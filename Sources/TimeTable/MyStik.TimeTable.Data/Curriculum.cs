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
            Packages = new HashSet<CurriculumPackage>();
            Sections = new HashSet<CurriculumSection>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string ShortName { get; set; }

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
        /// Gültig ab Semester
        /// </summary>
        public virtual Semester ValidSince { get; set; }

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
        /// Studiengruppen - integraler Bestandteil
        /// </summary>
        public virtual ICollection<CurriculumGroup> CurriculumGroups { get; set; }

        /// <summary>
        /// Werden diese noch benötigt?
        /// </summary>
        // public virtual ICollection<GroupAlias> GroupAliases { get; set; }

        /// <summary>
        /// Inhaltliche Struktur
        /// veraltet
        /// </summary>
        public virtual ICollection<CurriculumChapter> Chapters { get; set; }


        /// <summary>
        /// Pakete
        /// Pflicht, WPM, Absarbeit
        /// veraltet
        /// </summary>
        public virtual ICollection<CurriculumPackage> Packages { get; set; }

        /// <summary>
        /// Die Module nach SPO
        /// Falls nötig nur die abstrakten Bezeichnungen
        /// </summary>
        // public virtual ICollection<CertificateModule> Modules { get; set; }


        public virtual ICollection<CurriculumSection> Sections { get; set; }

        public virtual ICollection<CurriculumScope> Scopes { get; set; }
    }
}
