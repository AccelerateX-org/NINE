using System;
using MyStik.TimeTable.Data;

namespace MyStik.TimeTable.DataServices.Lottery.Data
{
    public class DrawingMessage
    {
        public DateTime TimeStamp { get; set; }

        public Course Course { get; set; }

        public OccurrenceSubscription Subscription { get; set; }

        public string UserId { get; set; }

        public string Remark { get; set; }
    }
}
