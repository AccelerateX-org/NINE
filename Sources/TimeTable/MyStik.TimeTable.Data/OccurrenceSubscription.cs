using System;
using System.Collections.Generic;

namespace MyStik.TimeTable.Data
{
    public class OccurrenceSubscription : Subscription
    {
        public OccurrenceSubscription()
        {
            Bets = new List<LotteryBet>();    
        }

        public int? Priority { get; set; }

        public int? Position { get; set; }

        public string CheckInRemark { get; set; }

        public string CheckOutRemark { get; set; }

        public string PositionRemark { get; set; }

        public string SubscriberRemark { get; set; }

        public string HostRemark { get; set; }

        public bool OnWaitingList { get; set; }

        public int LapCount { get; set; }

        public bool IsConfirmed { get; set; }

        public DateTime? ValdUntil { get; set; }

        public virtual Occurrence Occurrence { get; set; }

        public virtual ICollection<LotteryBet> Bets { get; set; }
    }
}
