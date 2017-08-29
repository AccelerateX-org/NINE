using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fk11.Model
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
