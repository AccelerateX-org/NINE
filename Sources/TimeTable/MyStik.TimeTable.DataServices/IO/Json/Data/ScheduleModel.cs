using System.Collections.Generic;

namespace MyStik.TimeTable.DataServices.IO.Json.Data
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
