using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyStik.TimeTable.Data
{
    public class Subscription
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        /// <summary>
        /// Fremdschlüssel in UserDB
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// Zeitstempel der Eintragung
        /// wird in Ticks gemessen (100 nanosekunden)
        /// </summary>
        public DateTime TimeStamp { get; set; }

        /// <summary>
        /// Schalter, ob Erhalt von E-Mails über Verteiler gewünscht wird
        /// </summary>
        public bool LikesEMails { get; set; }

        /// <summary>
        /// Schalter, ob Informationen per Notifications gewünscht wird
        /// </summary>
        public bool LikesNotifications { get; set; }

    }
}
