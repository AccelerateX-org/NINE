using MyStik.TimeTable.Data;
using MyStik.TimeTable.DataServices.IO.Csv.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyStik.TimeTable.DataServices.IO.CSV2
{
    public class ImportItem
    {
        public string Token { get; set; }

        public Guid? ObjectId { get; set; }
    }

    public class ImportDate
    {
        public DateTime Begin { get; set; }
        public DateTime End { get; set; }
    }

    public class ImportCourseId
    {
        public string Faculty { get; set; }
        public string Semester { get; set; }
        public string Id { get; set; }

        public string CourseName { get; set; }

        public bool IsValid { get; set; }

        public Guid? CourseId { get; set; }
        public Guid? OrgId { get; set; }
        public Guid? SemId { get; set; }

        public List<string> Errors { get; set; } = new List<string>();
    }

    public class ImportCourseDataSet
    {
        public string Segment { get; set; }
        public DateTime? PeriodBegin { get; set; }
        public DateTime? PeriodEnd { get; set; }
        public List<DayOfWeek> DaysOfWeek { get; set; } = new List<DayOfWeek>();
        public int Frequency { get; set; }
        public DateTime? DateBegin { get; set; }
        public DateTime? DateEnd { get; set; }

        public List<ImportItem> Lecturers { get; set; } = new List<ImportItem>();
        public List<ImportItem> Rooms { get; set; } = new List<ImportItem>();
        public List<ImportItem> Cohorts { get; set; } = new List<ImportItem>();

        public int? Capacity { get; set; }

        public List<ImportItem> Modules { get; set; } = new List<ImportItem>();

        public List<ImportDate> Dates { get; set; } = new List<ImportDate>();
    }
}
