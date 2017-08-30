using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace fk11.Model
{
    public class ScheduleModel
    {
        public ScheduleModel()
        {
            Courses = new List<ScheduleCourse>();
            Lotteries = new List<CourseLottery>();
        }

        public List<ScheduleCourse> Courses { get; }

        public List<CourseLottery> Lotteries { get; }
    }
}
