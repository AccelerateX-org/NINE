using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyStik.TimeTable.Data
{
    /// <summary>
    /// Nur Betreuer
    /// Ablehnungen werden nicht gespeichert - Dokumentation nur über die E-Mails
    /// </summary>
    public class Supervisor
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public virtual OrganiserMember Member { get; set; }

        public virtual Thesis Thesis { get; set; }

        /// <summary>
        /// Das Datum, wenn der Prof die Arbeit annimmt
        /// </summary>
        public DateTime? AcceptanceDate { get; set; }

        /// <summary>
        /// Eine bemerkung, auch Erstkorrektor, Zweitkorrektor
        /// </summary>
        public string Remark { get; set; }
    }
}
