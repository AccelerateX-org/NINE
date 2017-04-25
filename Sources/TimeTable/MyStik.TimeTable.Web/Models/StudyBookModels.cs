using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MyStik.TimeTable.Data;

namespace MyStik.TimeTable.Web.Models
{
    public class StudyBookViewModel
    {
        public StudyBookViewModel()
        {
            MySubscriptions = new List<ActivitySubscriptionModel>();
        }

        public List<Course> Courses { get; set; }

        public OfficeHour OfficeHour { get; set; }

        public List<Event> Events { get; set; }

        public Semester Semester { get; set; }

        public List<ActivitySubscriptionModel> MySubscriptions { get; private set; }

    }
}