using MyStik.TimeTable.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyStik.TimeTable.Web.Models
{
    public class RoomReservationViewModel
    {
        public Room Room { get; set; }

        public ICollection<ActivityDate> CurrentDates { get; set; }

        public string Message { get; set; }

        public ActivityDate NextDate { get; set; }
        public ActivityDate CurrentDate { get; set; }
    }
}