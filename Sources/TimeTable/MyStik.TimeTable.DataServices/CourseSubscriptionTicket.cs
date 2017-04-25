using MyStik.TimeTable.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyStik.TimeTable.DataServices
{
    public class CourseSubscriptionTicket
    {
        public bool IsValid { get; set; }
        public bool OnWaitingList { get; set; }

        public string Message { get; set; }

        public Course Course { get; set; }


    }
}
