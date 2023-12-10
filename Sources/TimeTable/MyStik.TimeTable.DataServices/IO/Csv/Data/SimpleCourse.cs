using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyStik.TimeTable.Data;

namespace MyStik.TimeTable.DataServices.IO.Csv.Data
{
    public class SimpleCourse
    {
        public string CourseId { get; set; }
        public string SubjectId { get; set; }
        public string Title { get; set; }
        public DayOfWeek? DayOfWeek { get; set; }
        public DateTime? Begin { get; set; }
        public DateTime? End { get; set; }
        public string Lecturer { get; set; }
        public string Room { get; set; }
        public string LabelLevel { get; set; }
        public string Label { get; set; }

    }
}
