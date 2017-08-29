using System.Collections.Generic;
using MyStik.TimeTable.Data;

namespace MyStik.TimeTable.Web.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class StudyBookViewModel
    {
        /// <summary>
        /// 
        /// </summary>
        public StudyBookViewModel()
        {
            MySubscriptions = new List<ActivitySubscriptionModel>();
        }

        /// <summary>
        /// 
        /// </summary>
        public List<Course> Courses { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public OfficeHour OfficeHour { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<Event> Events { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Semester Semester { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<ActivitySubscriptionModel> MySubscriptions { get; private set; }

    }
}