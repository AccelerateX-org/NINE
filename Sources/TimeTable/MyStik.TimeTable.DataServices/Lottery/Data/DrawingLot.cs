using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyStik.TimeTable.Data;

namespace MyStik.TimeTable.DataServices.Lottery.Data
{
    public class DrawingLot
    {
        public Course Course { get; set; }

        public OccurrenceSubscription Subscription { get; set; }

        public bool IsValid { get; set; }

        public bool IsTouched { get; set; }

        public string Message { get; set; }

        public int Priority => Subscription.Priority ?? 0;
    }
}
