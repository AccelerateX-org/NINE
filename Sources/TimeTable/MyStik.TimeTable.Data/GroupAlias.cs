using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
