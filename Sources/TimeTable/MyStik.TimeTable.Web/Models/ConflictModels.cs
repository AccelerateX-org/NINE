using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MyStik.TimeTable.Data;
using MyStik.TimeTable.DataServices;

namespace MyStik.TimeTable.Web.Models
{
    public class RoomConflictModel
    {
        public Course Course { get; set; }

        public ActivityDate Date { get; set; }

        public List<Room> AvailableRooms { get; set; }

        public List<RoomAlternativeModel> Alternatives { get; set; }
    }

    public class RoomAlternativeModel
    {
        public ActivityDate Date { get; set; }

        public List<Room> AvailableRooms { get; set; }
    }

    public class DateAlternativeModel
    {
        public Course Course { get; set; }

        public DatePeriod DatePeriod { get; set; }

        public List<Room> Rooms { get; set; }
    }
}