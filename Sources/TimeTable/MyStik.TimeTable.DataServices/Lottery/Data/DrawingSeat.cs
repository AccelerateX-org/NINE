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
