using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fk11.Model
{
    public class ScheduleCourse
    {
        public ScheduleCourse()
        {
            Groups = new List<ScheduleGroup>();
            Dates = new List<ScheduleDate>();
        }

        public int CourseId { get; set; }

        public string Name { get; set; }

        public string ShortName { get; set; }

        public string Description { get; set; }

        public int SeatRestriction { get; set; }


        public List<ScheduleGroup> Groups { get; private set; }

        public List<ScheduleDate> Dates { get; private set; }
    }
}
