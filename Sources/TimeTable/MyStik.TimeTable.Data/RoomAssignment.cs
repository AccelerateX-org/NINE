using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyStik.TimeTable.Data
{
    /// <summary>
    /// Über die Raumzuweisung wird gesteuert, welche Räume ein Mitglied einer
    /// Organisation belegen darf.
    /// Ein Raum kann von mehreren Organisationen belegbar sein.
    /// Jede Organisation kann festlegen, ob die Belegung von eigenen Mitgliedern
    /// oder externen Mitgliedern jeweils durch die Administration bestätigt werden
    /// muss.
    /// </summary>
    public class RoomAssignment
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        /// <summary>
        /// Veraltet. Wird nicht verwendet
        /// </summary>
        //public bool IsExclusive { get; set; }

        /// <summary>
        /// Mitglieder der Organisation müssen sich Belegungen bestätigen lassen
        /// </summary>
        public bool InternalNeedConfirmation { get; set; }

        /// <summary>
        /// Mitglieder von anderen Organisationen müssen sich Belegungen bestätigen lassen
        /// </summary>
        public bool ExternalNeedConfirmation { get; set; }

        public virtual Room Room { get; set; }

        public virtual ActivityOrganiser Organiser { get; set; }
    }
}
