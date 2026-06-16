using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyStik.TimeTable.DataServices.IO.Contracts
{
    public class CohortContextApiContract
    {
        public string Institution { get; set; }
        public string Organiser { get; set; }
        public string Program { get; set; }

        public string Label { get; set; }
    }

    public class CohortEntityApiContract
    {
        public Guid Id { get; set; }
        public string Key { get; set; }
        public CohortContextApiContract Context { get; set; }
    }

    public class CohortScheduleApiContract
    {

    }
}
