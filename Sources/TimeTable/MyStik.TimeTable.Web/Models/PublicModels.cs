using MyStik.TimeTable.Data;
using System.Collections.Generic;

namespace MyStik.TimeTable.Web.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class RoomReservationViewModel
    {
        /// <summary>
        /// 
        /// </summary>
        public Room Room { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ICollection<ActivityDate> CurrentDates { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ActivityDate NextDate { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ActivityDate CurrentDate { get; set; }
    }
}