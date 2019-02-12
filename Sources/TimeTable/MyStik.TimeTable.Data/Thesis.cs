using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace MyStik.TimeTable.Data
{
    public class Thesis
    {
        public Thesis()
        {
            Advisors = new HashSet<Advisor>();
            Supervisors = new HashSet<Supervisor>();
        }


        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public string TitleDe { get; set;  }

        public string TitleEn { get; set; }

        /// <summary>
        /// Sperrvermerk - kann nur Studierende setzen
        /// </summary>
        public bool HasLockFlag { get; set; }


        /// <summary>
        /// Aktivität des Betreuers
        /// Veraltet - wird gelöscht
        /// </summary>
        public virtual Supervision Supervision { get; set; }

        /// <summary>
        /// Student
        /// </summary>
        public virtual Student Student { get; set; }

        /// <summary>
        /// Datum der Ausgabe
        /// Wird vom Studierenden am Tag der Anmeldung gesetzt
        /// </summary>
        public DateTime? IssueDate { get; set; }

        /// <summary>
        /// Abgabedatum wird am Tag der Anmeldung gesetzt
        /// </summary>
        public DateTime? ExpirationDate { get; set; }

        /// <summary>
        /// Verlängerungsdatum
        /// </summary>
        public DateTime? RenewalDate { get; set; }

        /// <summary>
        /// Datum der tatsächlichen Abgabe
        /// </summary>
        public DateTime? DeliveryDate { get; set; }

        /// <summary>
        /// Datum der Benotung, d.h. Eintragung der Note durch Sekretariat
        /// bzw. Weiterleitung der Notenmeldung
        /// benötigt Admin-Rechte
        /// </summary>
        public DateTime? GradeDate { get; set; }


        /// <summary>
        /// Wurde abgerechnet
        /// benötigt Admin-Rechte
        /// </summary>
        public bool? IsCleared { get; set; }


        /// <summary>
        /// Datum des Antrags
        /// </summary>
        public DateTime? RequestDate { get; set; }

        /// <summary>
        /// Datum des Ergebnisses des Antrags
        /// </summary>
        public DateTime? ResponseDate { get; set; }

        public virtual OrganiserMember RequestAuthority { get; set; }

        public string RequestMessage { get; set; }

        /// <summary>
        /// Das Ergebnis des Antrags
        /// </summary>
        public bool? IsPassed { get; set; }

        /// <summary>
        /// Das Datum, wenn der Prof die Arbeit annimmt
        /// </summary>
        public DateTime? AcceptanceDate { get; set; }

        /// <summary>
        /// Betreuung akzeptiert
        /// </summary>
        public bool? IsAccepted { get; set; }

        public ICollection<Advisor> Advisors { get; set; }

        public ICollection<Supervisor> Supervisors { get; set; }
    }
}
