using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyStik.TimeTable.Data;

namespace MyStik.TimeTable.DataServices.Lottery.Data
{
    public class DrawingSeat
    {
        public Course Course { get; set; }

        public OccurrenceSubscription Subscription { get; set; }

        public int Priority => Subscription.Priority ?? 0;

    }
}
