using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyStik.TimeTable.Data;

namespace MyStik.TimeTable.DataServices.Booking.Data
{
    public class BookingTicket
    {
        public Course Course { get; set; }

        public string UserId { get; set; }

        public OccurrenceSubscription Subscription { get; set; }

        public OccurrenceSubscription SucceedingSubscription { get; set; }
    }
}
