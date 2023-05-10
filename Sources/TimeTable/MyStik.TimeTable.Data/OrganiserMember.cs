using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MyStik.TimeTable.Data
{
    public class OrganiserMember
    {
        public OrganiserMember()
        {
            ModuleResponsibilities = new List<ModuleResponsibility>();
        }


        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        /// <summary>
        /// Fremdschlüssel auf UserDB (Guid)
        /// veraltet
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// Kurzbezeichnung innerhalb der Organisation
        /// Ein User kann also mehrere Kurznamen haben
        /// Kurznamen sollten generell nicht mehr angezeigt werden! dienen nur der internen Identifikation
        /// Können also doppelt sein
        /// Sollten daher auch nicht zwingend sein, z.B. bei der Fachschaft
        /// </summary>
        public string ShortName { get; set; }

        /// <summary>
        /// veraltet
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// veraltet
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// veraltet
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Beschreibung der Rolle(n)
        /// ;-separierte Liste
        /// </summary>
        public string Role { get; set; }

        /// <summary>
        /// Freitext für Beschreibung, z.B. personenbezogene Daten
        /// veraltet
        /// </summary>
        public string Description { get; set; }
        
        /// <summary>
        /// Administrator des Organistors
        /// Vergibt weitere Rechte
        /// </summary>
        public bool IsAdmin { get; set; }


        public bool IsMemberAdmin { get; set; }
        public bool IsRoomAdmin { get; set; }
        public bool IsSemesterAdmin { get; set; }
        public bool IsCurriculumAdmin { get; set; }
        public bool IsCourseAdmin { get; set; }
        public bool IsStudentAdmin { get; set; }
        public bool IsAlumniAdmin { get; set; }
        public bool IsEventAdmin { get; set; }
        public bool IsNewsAdmin { get; set; }
        public bool IsExamAdmin { get; set; }

        /// <summary>
        /// Gäste, LBs etc sind nur "assoziiert"
        /// Profs, MAs können nur in einer Fakultät "daheim" sein
        /// LBs sind immer assoziiert
        /// "importierte" Profs sind ebenfalls assoziiert
        /// myOrg => die Org wo ich daheim bin
        /// </summary>
        public bool IsAssociated { get; set; }

        /// <summary>
        /// Generelle Anzeige des Titles
        /// veraltet
        /// </summary>
        public bool ShowTitle { get; set; }

        /// <summary>
        /// Beschreibung wird bei Suche oder Profilanzeigen eingeblendet
        /// Sichtbar für alle
        /// veraltet
        /// </summary>
        public bool ShowDescription { get; set; }


        /// <summary>
        /// veraltet
        /// </summary>
        public string UrlProfile { get; set; }


        public virtual ActivityOrganiser Organiser { get; set; }

        public virtual ICollection<ActivityDate> Dates { get; set; }

        /// <summary>
        /// Liste aller Modulverantwortlichkeiten
        /// </summary>
        public virtual ICollection<ModuleResponsibility> ModuleResponsibilities { get; set; }

        public virtual ICollection<ActivityOwner> Ownerships { get; set; }

        public virtual ICollection<MemberExport> Exports { get; set; }


        public virtual ICollection<MemberResponsibility> Responsibilities { get; set; }

        public virtual ICollection<MemberSkill> Skills { get; set; }

        public string FullName
        {
            get
            {
                var sb = new StringBuilder();

                sb.Append(Name);

                if (!string.IsNullOrEmpty(FirstName))
                {
                    sb.AppendFormat(", {0} ", FirstName);
                }

                if (!string.IsNullOrEmpty(Title))
                {
                    sb.AppendFormat(" ({0})", Title);
                }

                return sb.ToString();
            }
        }

        public string FullTag => $"{Organiser.Tag}#{ShortName}";

        public string Tag => ShortName;
    }
}
