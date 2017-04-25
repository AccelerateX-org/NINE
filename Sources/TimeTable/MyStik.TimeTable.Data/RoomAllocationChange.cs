using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyStik.TimeTable.Data
{
    public class RoomAllocationChange
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        /// <summary>
        /// Referenz auf Benutzer, der die Änderung durchgeführt hat
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// Zeitstempel der Änderung
        /// </summary>
        public DateTime TimeStamp { get; set; }

        /// <summary>
        /// Raum, der von der Änderung betroffen ist
        /// </summary>
        public virtual Room Room { get; set; }

        /// <summary>
        /// Beginn des Zeitraums
        /// </summary>
        public DateTime Begin { get; set; }

        /// <summary>
        /// Ende des Zeitraums
        /// </summary>
        public DateTime End { get; set; }

        /// <summary>
        /// True: Freigabe des Raums
        /// False: Belegung des Raums
        /// </summary>
        public bool IsReleased { get; set; }


        /// <summary>
        /// Anzahl der Konflikt nach der Änderung
        /// </summary>
        public int ConflictCount { get; set; }

        /// <summary>
        /// Beschreibung des Grunds der Veränderung
        /// </summary>
        public string DescriptionSource { get; set; }

        /// <summary>
        /// Beschreibung der Konflikte
        /// </summary>
        public string DescriptionConflicts { get; set; }
    }
}
