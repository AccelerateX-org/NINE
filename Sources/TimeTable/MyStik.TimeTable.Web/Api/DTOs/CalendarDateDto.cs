using System;

namespace MyStik.TimeTable.Web.Api.DTOs
{
    public enum CalendarDateType
    {
        Course,
        Event,
        OfficeHour
    }

    public class CalendarDateDto : NamedDto
    {
        public SubscriptionDto Subscription { get; set; }
        
        public CourseDateDto Date { get; set; }

        public CalendarDateType Type { get; set; }
    }


    public class ActiveEvent
    {
        public string course { get; set; }
        public DateTime starttime { get; set; }
        public DateTime endtime { get; set; }
        public string teacher { get; set; }
        public string room { get; set; }
        public string virtual_room { get; set; }
        public string description { get; set; }
        public string moodle { get; set; }
        public string moodle_key { get; set; }
        public string special { get; set; }
    }
}