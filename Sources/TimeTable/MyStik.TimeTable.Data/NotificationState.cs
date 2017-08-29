using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyStik.TimeTable.Data
{
    public class NotificationState
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        /// <summary>
        /// Fremdschlüssel in UserDB
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// Für den Benutzer neue Nachricht
        /// </summary>
        public bool IsNew { get; set; }

        /// <summary>
        /// Datum, zu dem die Notification gelesen wurde
        /// </summary>
        public DateTime? ReadingDate { get; set; }

        public virtual ActivityDateChange ActivityDateChange { get; set; }

    }
}
