using System;
using MyStik.TimeTable.Data;

namespace MyStik.TimeTable.DataServices.Lottery.Data
{
    public class DrawingLot
    {
        public Course Course { get; set; }

        public OccurrenceSubscription Subscription { get; set; }

        public bool IsValid { get; set; }

        public bool IsTouched { get; set; }

        private string Message { get; set; }

        public int Priority => Subscription.Priority ?? 0;

        public void InitMessage()
        {
            Message = "";
        }

        public void AddMessage(string msg)
        {
            Message += $"<li>{DateTime.Now}: {msg}</li>";
        }

        public void SaveMessage()
        {
            if (string.IsNullOrEmpty(Message))
                Subscription.HostRemark = string.Empty;
            else
                Subscription.HostRemark = $"<ul>{Message}</ul>";
        }
    }
}
