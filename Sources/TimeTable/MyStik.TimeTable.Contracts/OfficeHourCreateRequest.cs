using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyStik.TimeTable.Contracts
{
    public class OfficeHourCreateRequest
    {
        public string SemesterId { get; set; }

        public string DozId { get; set; }

        public string OrgId { get; set; }

        public DayOfWeek DayOfWeek { get; set; }

        public TimeSpan StartTime { get; set; }

        public TimeSpan EndTime { get; set; }

        public int Capacity { get; set; }

        public int SpareSlots { get; set; }

        public int SlotDuration { get; set; }

        public bool ByAgreement { get; set; }

        public int SubscriptionLimit { get; set; }

        public bool CreateDates { get; set; }
    }
}
