using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyStik.TimeTable.Data
{
    /// <summary>
    /// Buchungsbestätigung für eine Raumbelegung
    /// </summary>
    public class BookingConfirmation
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        /// <summary>
        /// Benutzer, der die Buchung bestätigt oder abgelehnt hat
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// Zeitstempel der Bestätigung oder Ablehnung
        /// </summary>
        public DateTime? TimeStamp { get; set; }

        /// <summary>
        /// Default: false
        /// </summary>
        public bool IsConfirmed { get; set; }

        /// <summary>
        /// Die Buchung, die bestätigt oder abgelehnt werden muss
        /// </summary>
        public virtual RoomBooking RoomBooking { get; set; }

        /// <summary>
        /// Die Organisation, die zustimmen muss
        /// </summary>
        public virtual ActivityOrganiser Organiser { get; set; }

    }
}
