using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyStik.TimeTable.Data;

namespace MyStik.TimeTable.DataServices.Drawing.Data
{
    public class DrawingLotPot
    {
        public DrawingLotPot()
        {
            SeatsTaken = new List<OccurrenceSubscription>();
            Lots = new List<DrawingLot>();
        }

        public Course Course { get; set; }

        public string Name { get; set; }

        public int Capacity { get; set; }

        public List<OccurrenceSubscription> SeatsTaken { get; private set; }


        public List<DrawingLot> Lots { get; private set; }


        public int SeatsAvailable => Capacity - SeatsTaken.Count;

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

        public int RemainingSeats
        {
            get
            {
                return SeatsAvailable - Lots.Count(x => !x.Subscription.OnWaitingList);
            }
        }

        public double OccupancyRate => SeatsTaken.Count / (double) Capacity;
    }


}
