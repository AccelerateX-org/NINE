using System;
using System.Collections.Generic;
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
        /// Dieser Raum gehört der Organisation
        /// Ein Raum kann auch mehreren organisationen gehören
        /// Diese müssten im Zweifel alle einer Buchungsanfrage zustimmen
        /// </summary>
        public bool IsOwner { get; set; }

        /// <summary>
        /// interne Raum Admins dürfen diesen Raum belegen
        /// false: interne brauchen keine Buchungsanfrage
        /// true: für interne wird eine Buchungsanfrage erstellt
        /// </summary>
        public bool InternalNeedConfirmation { get; set; }

        /// <summary>
        /// externe Raum Admins dürfen diesen Raum belegen
        /// false: externe brauchen keine Buchungsanfrage
        /// true: externe brauchen eine Buchungsanfrage
        /// </summary>
        public bool ExternalNeedConfirmation { get; set; }

        public virtual Room Room { get; set; }

        public virtual ActivityOrganiser Organiser { get; set; }
    }

    public class RoomAllocationGroup
    {
        public RoomAllocationGroup()
        {
            RoomAllocations = new HashSet<RoomAllocation>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public virtual ActivityOrganiser Organiser { get; set; }

        public string Name { get; set; }

        public virtual string Description { get; set; } = string.Empty;

        public virtual string ShortName { get; set; }

        public virtual ICollection<RoomAllocation> RoomAllocations { get; set; }

    }

    public class RoomAllocation
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        /// <summary>
        /// Platzhalter, falls nur Teile des Raums belegt werden dürfen
        /// </summary>
        public int Capacity { get; set; }

        public virtual Room Room { get; set; }

        public virtual RoomAllocationGroup Group { get; set; }
    }
}
