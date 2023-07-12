using System;

namespace MyStik.TimeTable.Contracts
{
    public class OfficeHourCreateRequest
    {
        public Guid SemesterId { get; set; }

        public Guid DozId { get; set; }

        public Guid OrgId { get; set; }

        public DayOfWeek DayOfWeek { get; set; }

        public TimeSpan StartTime { get; set; }

        public TimeSpan EndTime { get; set; }

        public int Capacity { get; set; }

        public int SpareSlots { get; set; }

        public int SlotDuration { get; set; }

        public bool ByAgreement { get; set; }

        public int SubscriptionLimit { get; set; }

        public bool CreateDates { get; set; }

        public string Name { get; set; }

        public string Text { get; set; }

        public int SlotsPerDate { get; set; }

        public int FutureSlots { get; set; }

        public DateTime FirstDate { get; set; }
        public DateTime LastDate { get; set; }
    }
}
