﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MyStik.TimeTable.Data
{
    public enum LectureRole
    {
        FullTime,       // Professor
        Visiting,       // Lehrbeauftragter
        Guest,          // Gast
        Tutor,          // Student
    }


    public class OrganiserMember
    {
        public OrganiserMember()
        {
            ModuleResponsibilities = new List<ModuleResponsibility>();
            RoomAccesses = new HashSet<RoomAccess>();
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
        /// Disziplinarische Zuordnung
        /// true: zugeordnet
        /// false: importiert
        /// </summary>
        public bool IsDefault { get; set; }


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
        public bool IsInstitutionAdmin { get; set; }

        /// <summary>
        /// Neue Definition:
        /// true: wird in Listen angezeigt
        /// false: wird in Listen nicht angezeigt
        /// </summary>
        public bool IsAssociated { get; set; }


        public LectureRole LectureRole { get; set; }


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

        public virtual BulletinBoard BulletinBoard { get; set; }


        public virtual ICollection<ActivityDate> Dates { get; set; }

        /// <summary>
        /// Liste aller Modulverantwortlichkeiten
        /// </summary>
        public virtual ICollection<ModuleResponsibility> ModuleResponsibilities { get; set; }

        public virtual ICollection<ActivityOwner> Ownerships { get; set; }

        public virtual ICollection<MemberExport> Exports { get; set; }


        public virtual ICollection<MemberResponsibility> Responsibilities { get; set; }

        public virtual ICollection<MemberSkill> Skills { get; set; }

        public virtual ICollection<VirtualRoom> VirtualRooms { get; set; }

        public virtual ICollection<CatalogResponsibility> CatalogResponsibilities { get; set; }

        public virtual ICollection<CommitteeMember> CommitteeMembershios { get; set; }

        public virtual ICollection<MemberAvailability> Availabilties { get; set; }

        public virtual ICollection<RoomAccess> RoomAccesses { get; set; }


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

    public class MemberAvailability
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public virtual OrganiserMember Member { get; set; }

        public virtual Semester Semester { get; set; }

        public virtual SemesterDate Segment { get; set; }

        public DateTime Begin { get; set; }

        public DateTime End { get; set; }

        /// <summary>
        /// -3 gar nicht
        /// +3 super gut 
        /// </summary>
        public int Priority { get; set; }

        public string Remarks { get; set; }

        /// <summary>
        /// true: Gremientermin
        /// </summary>
        public bool IsAdmin { get; set; }
    }

}
