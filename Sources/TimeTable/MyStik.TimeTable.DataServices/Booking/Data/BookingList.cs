using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public bool IsLost { get; set; }

        public Occurrence Occurrence { get; set; }

        public SeatQuota SeatQuota { get; set; }

        public List<Booking> Bookings { get; }

        /// <summary>
        /// deprecated
        /// </summary>
        //public OccurrenceGroup Group { get; set; }

        /// <summary>
        /// deprecated
        /// </summary>
        public List<Curriculum> Curricula { get; }


        public int Capacity
        {
            get
            {
                if (SeatQuota != null)
                {
                    return SeatQuota.MaxCapacity;
                }

                return int.MaxValue;
            }
        }

        public int FreeSeats
        {
            get
            {
                if (SeatQuota != null)
                {
                    return Capacity != int.MaxValue ? Capacity - Participients.Count : int.MaxValue;
                }

                return int.MaxValue;
            }
        }


        public List<Booking> Participients
        {
            get { return Bookings.Where(x => !x.Subscription.OnWaitingList).ToList(); }
        }

        public List<Booking> WaitingList
        {
            get { return Bookings.Where(x => x.Subscription.OnWaitingList).ToList(); }
        }

        public List<Booking> WaitingListWithPrio(int p)
        {
            return WaitingList.Where(x => x.Subscription.Priority == p).ToList();
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
            if (IsMisc)
                return null;

            return WaitingList.OrderBy(x => x.Subscription.TimeStamp).FirstOrDefault();

        }

        public string Name
        {
            get
            {
                if (SeatQuota != null)
                {
                    return SeatQuota.Summary;
                }

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
