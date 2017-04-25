using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyStik.TimeTable.Data;

namespace MyStik.TimeTable.FamosImport.Data
{
    public class FamosEvent
    {
        public string EventId { get; set; }

        public DateTime Begin { get; set; }

        public DateTime End { get; set; }

        public string RoomNumber { get; set; }

        public string Description { get; set; }

        public string Organiser { get; set; }
    }
}
