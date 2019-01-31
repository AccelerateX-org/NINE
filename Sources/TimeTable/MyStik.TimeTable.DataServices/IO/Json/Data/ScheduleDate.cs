using System;
using System.Collections.Generic;

namespace MyStik.TimeTable.DataServices.IO.Json.Data
{
    public class ScheduleDate
    {
        public ScheduleDate()
        {
            Lecturers = new List<DateLecturer>();
            Rooms = new List<DateRoom>();
        }

        public DateTime Begin { get; set; }

        public DateTime End { get; set; }

        public List<DateLecturer> Lecturers { get; private set; }

        public List<DateRoom> Rooms { get; private set; }
    }
}
