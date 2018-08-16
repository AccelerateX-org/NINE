using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyStik.TimeTable.Data
{
    public class RoomEquipment
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public int Amount { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public bool IsImmobile { get; set; }

        public virtual Room Room { get; set; }
    }
}
