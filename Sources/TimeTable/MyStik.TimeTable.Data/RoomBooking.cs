using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyStik.TimeTable.Data
{
    /// <summary>
    /// Eine tatsächliche Raumbelegung
    /// Der Zeitraum wird über das ActivityDate festgelegt
    /// Die Belegung erfolgt über die gesamte Zeit des ActivityDate
    /// </summary>
    public class RoomBooking
    {
        public RoomBooking()
        {
            this.Confirmations = new HashSet<BookingConfirmation>();
        }


        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        /// <summary>
        /// Benutzer, der den Raum gebucht hat
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// Zeitstempel der Buchung (nicht der Bestätigung)
        /// </summary>
        public DateTime TimeStamp { get; set; }

        public virtual Room Room { get; set; }

        public virtual ActivityDate Date { get; set; }

        public virtual ICollection<BookingConfirmation> Confirmations { get; set; }
    }
}
