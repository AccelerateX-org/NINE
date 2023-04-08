using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyStik.TimeTable.Data
{
    public class VirtualRoom
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string TokenName { get; set; }

        public string Token { get; set; }

        public string AccessUrl { get; set; }

        public bool ParticipientsOnly { get; set; }

        public virtual OrganiserMember Owner { get; set; }

        /// <summary>
        /// Die echten Belegungstermine mit Datum und Uhrzeit
        /// many-to-many
        /// </summary>
        public virtual ICollection<VirtualRoomAccess> Accesses { get; set; }

    }
}
