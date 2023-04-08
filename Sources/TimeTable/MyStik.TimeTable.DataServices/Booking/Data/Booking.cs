using MyStik.TimeTable.Data;

namespace MyStik.TimeTable.DataServices.Booking.Data
{
    public class Booking
    {
        public OccurrenceSubscription Subscription { get; set; }

        public Student Student { get; set; }
    }
}
