using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyStik.TimeTable.Data
{
    public enum DrawingState
    {
        Confirmed,
        Reserved,
        Waiting,
    }


    public class SubscriptionDrawing
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public virtual OccurrenceSubscription Subscription { get; set; }

        public DateTime DrawingTime { get; set; }

        public DrawingState StateBeforeDrawing { get; set; }

        public int LapCountBeforeDrawing { get; set; }

        public DrawingState StateAfterDrawing { get; set; }

        public int LapCountAfterDrawing { get; set; }

        /// <summary>
        /// In welcher Runde gezogen
        /// -1 wurde nicht gezogen
        /// </summary>
        public int LapCountDrawing { get; set; }

        public string Remark { get; set; }

        public virtual OccurrenceDrawing OccurrenceDrawing { get; set; }

    }
}
