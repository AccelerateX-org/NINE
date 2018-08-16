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
        /// </summary>
        public virtual Supervision Supervision { get; set; }

        /// <summary>
        /// Student
        /// </summary>
        public virtual Student Student { get; set; }

        /// <summary>
        /// Datum der Ausgabe
        /// </summary>
        public DateTime IssueDate { get; set; }

        /// <summary>
        /// Abgabedatum
        /// </summary>
        public DateTime ExpirationDate { get; set; }

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
    }
}
