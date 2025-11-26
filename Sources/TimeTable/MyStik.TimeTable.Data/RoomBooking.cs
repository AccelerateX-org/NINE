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
        /// Deprecated
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// Zeitstempel der Buchung (nicht der Bestätigung)
        /// Deprecated
        /// </summary>
        public DateTime TimeStamp { get; set; }

        public virtual Room Room { get; set; }

        public virtual ActivityDate Date { get; set; }

        public virtual OrganiserMember Booker { get; set; }

        public DateTime BookingDate { get; set; }

        public bool IsCreated { get; set; }

        /// <summary>
        /// Der Confirmer kann auch der Booker selbst sein, wenn die Organisation
        /// als Owner Zugriff auf den Raum hat und "alle internen" buchen dürfen
        /// </summary>
        public virtual OrganiserMember Confirmer { get; set; }

        public DateTime? ConfirmationDate { get; set; }

        public bool? IsConfirmed { get; set; }

        public string ConfirmationComment { get; set; }

        public virtual OrganiserMember Acknowledger { get; set; }

        public DateTime? AcknowledgementDate { get; set; }

        public bool? IsAcknowledged { get; set; }

        public string AcknowledgementComment { get; set; }

        /// <summary>
        /// Deprecated
        /// </summary>
        public virtual ICollection<BookingConfirmation> Confirmations { get; set; }
    }
}
