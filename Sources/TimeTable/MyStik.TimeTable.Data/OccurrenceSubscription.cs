using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyStik.TimeTable.Data
{
    public class OccurrenceSubscription : Subscription
    {
        public int? Priority { get; set; }

        public int? Position { get; set; }

        public string PositionRemark { get; set; }

        public string SubscriberRemark { get; set; }

        public string HostRemark { get; set; }

        public bool OnWaitingList { get; set; }

        public int LapCount { get; set; }

        public bool IsConfirmed { get; set; }

        public virtual Occurrence Occurrence { get; set; }

    }
}
