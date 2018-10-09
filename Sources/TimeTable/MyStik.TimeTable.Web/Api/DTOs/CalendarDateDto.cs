using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

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
}