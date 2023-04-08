using System.Collections.Generic;
using System.Linq;
using MyStik.TimeTable.Data;
using MyStik.TimeTable.DataServices.Booking.Data;

namespace MyStik.TimeTable.DataServices.Lottery.Data
{
    public class DrawingLotPot
    {
        public DrawingLotPot()
        {
            Lots = new List<DrawingLot>();
        }

        public Course Course { get; set; }

        public string Name
        {
            get { return BookingList.Name; }
        }

        public int Capacity
        {
            get { return BookingList.Capacity; }
        }

        public BookingList BookingList { get; set; }


        public List<DrawingLot> Lots { get; private set; }

        /// <summary>
        /// Berücksichtigt den aktuellen Zustand aller Subscriptions dieses Losttopfs
        /// </summary>
        public int SeatsAvailable => Capacity - BookingList.Participients.Count;

        public double BookingRank
        {
            get
            {
                if (SeatsAvailable == 0)
                    return 0;

                var nPrio1 = Lots.Count(x => x.Priority == 1);

                return nPrio1 / (double) SeatsAvailable;
            }
        }

        /* Überflüssig
        public int RemainingSeats
        {
            get
            {
                return SeatsAvailable - Lots.Count(x => !x.Subscription.OnWaitingList);
            }
        }
        */

        public double OccupancyRate => BookingList.Participients.Count / (double) Capacity;
    }


}
