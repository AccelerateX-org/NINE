using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyStik.TimeTable.Web.Models
{
    public class ActivityAnalysisModel
    {
        public int CourseCount { get; set; }
        public int CourseDateCount { get; set; }


        public int OfficeHourCount { get; set; }
        public int OfficeHourDateCount { get; set; }
        public int OfficeHourSlotCount { get; set; }

        public int EventCount { get; set; }
        public int EventDateCount { get; set; }


        public int NewsletterCount { get; set; }


        public int OccurrenceCount { get; set; }

        public int SubscriptionCount { get; set; }

        public int DanglingSubscriptionCount { get; set; }
    }
}