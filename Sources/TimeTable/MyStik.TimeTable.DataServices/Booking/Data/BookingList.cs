using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyStik.TimeTable.Data;

namespace MyStik.TimeTable.DataServices.Booking.Data
{
    public class BookingList
    {
        private string _name;


        public BookingList(string name)
        {
            _name = name;
            Curricula = new List<Curriculum>();
            Bookings = new List<Booking>();
        }

        public bool IsMisc { get; set; }

        public Occurrence Occurrence { get; set; }

        public OccurrenceGroup Group { get; set; }

        public List<Curriculum> Curricula { get; }

        public List<Booking> Bookings { get; }

        public int Capacity
        {
            get
            {
                if (Group == null)
                {
                    return Occurrence.Capacity < 0 ? int.MaxValue : Occurrence.Capacity;
                }

                return Group.Capacity < 0 ? int.MaxValue : Group.Capacity;
            }
        }

        public int FreeSeats
        {
            get { return Capacity != int.MaxValue ? Capacity - Participients.Count : int.MaxValue; }
        }


        public List<Booking> Participients
        {
            get { return Bookings.Where(x => !x.Subscription.OnWaitingList).ToList(); }
        }

        public List<Booking> WaitingList
        {
            get { return Bookings.Where(x => x.Subscription.OnWaitingList).ToList(); }
        }

        public int GetPosition(OccurrenceSubscription subscription)
        {
            var orderedWaiting = WaitingList.OrderBy(x => x.Subscription.TimeStamp).ToList();
                
           var booking = orderedWaiting.FirstOrDefault(x => x.Subscription.Id == subscription.Id);
            if (booking != null)
            {
                return orderedWaiting.IndexOf(booking) + 1;
            }

            return -1;
        }

        public Booking GetSucceedingBooking()
        {
            return WaitingList.OrderBy(x => x.Subscription.TimeStamp).FirstOrDefault();

        }

        public string Name
        {
            get
            {
                if (!Curricula.Any())
                {
                    return _name;
                }

                var sb = new StringBuilder();
                sb.Append("Studiengänge: ");
                foreach (var curriculum in Curricula)
                {
                    sb.Append(curriculum.ShortName);
                    if (curriculum != Curricula.Last())
                    {
                        sb.Append(", ");
                    }
                }

                return sb.ToString();
            }
        }
    }
}
