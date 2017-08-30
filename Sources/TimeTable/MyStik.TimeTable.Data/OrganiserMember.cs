using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyStik.TimeTable.Data
{
    public class OrganiserMember
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        /// <summary>
        /// Fremdschlüssel auf UserDB (Guid)
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

        public string Name { get; set; }

        /// <summary>
        /// Beschreibung der Rolle(n)
        /// ;-separierte Liste
        /// </summary>
        public string Role { get; set; }

        /// <summary>
        /// Freitext für Beschreibung, z.B. personenbezogene Daten
        /// </summary>
        public string Description { get; set; }
        
        /// <summary>
        /// Administrator des Organistors
        /// Darf alle Activities, Dates und Slots ändern
        /// veraltet - soll raus
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

        /// <summary>
        /// Gäste, LBs etc sind nur "assoziiert"
        /// </summary>
        public bool IsAssociated { get; set; }


        public string UrlProfile { get; set; }

        public virtual ActivityOrganiser Organiser { get; set; }

        public virtual ICollection<ActivityDate> Dates { get; set; }

        /// <summary>
        /// Liste aller Modulverantwortlichkeiten
        /// </summary>
        public virtual ICollection<CurriculumModule> Modules { get; set; }

        public virtual ICollection<ActivityOwner> Ownerships { get; set; }
    }
}
