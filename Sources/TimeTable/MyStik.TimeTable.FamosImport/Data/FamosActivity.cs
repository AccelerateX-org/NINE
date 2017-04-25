using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyStik.TimeTable.Data;

namespace MyStik.TimeTable.FamosImport.Data
{
    public class FamosActivity
    {
        public FamosActivity()
        {
            Events = new List<FamosEvent>();
        }

        public string EventId { get; set; }

        public string Organiser { get; set; }

        public bool IsValid { get; set; }

        public bool IsTouched { get; set; }

        public Reservation Activity { get; set; }

        public List<FamosEvent> Events { get; set; }
    }
}
