using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyStik.TimeTable.Data
{
    /// <summary>
    /// Grundsätzliche Idee: jeder darf nur das eigene ändern
    /// Wenn jemand Rechte hat etwas von jemand anderem zu ändern, dann wird das immer eine neue Version
    /// Das Datum lastEdited wird für die Sortierung verwendet
    /// Das Datum created ist nur nachrichtlich
    /// </summary>
    public class ChangeLog
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        /// <summary>
        /// Der die Änderung gemacht hat
        /// </summary>
        public string UserIdAmendment { get; set; }

        /// <summary>
        /// Benutzer, der Freigabe erteilt hat
        /// </summary>
        public string UserIdApproval { get; set; }

        /// <summary>
        /// Datum angelegt
        /// </summary>
        public DateTime Created { get; set; }

        /// <summary>
        /// Datum zuletzt geändert
        /// </summary>
        public DateTime LastEdited { get; set; }

        /// <summary>
        /// Datum der Freigabe => Verabschiedung
        /// </summary>
        public DateTime? Approved { get; set; }

        /// <summary>
        /// Anzeigen / Verbergen => analogie moodle
        /// </summary>
        public bool IsVisible { get; set; }


    }
}
