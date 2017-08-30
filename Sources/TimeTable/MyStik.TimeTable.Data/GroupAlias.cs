using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyStik.TimeTable.Data
{
    public class GroupAlias
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public virtual CapacityGroup CapacityGroup { get; set; }

        /// <summary>
        /// Bezeichnung in gpUntis
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Bemerkung für Stundenplanung
        /// </summary>
        public string Remark { get; set; }
    }
}
