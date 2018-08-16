using System.Collections.Generic;
using MyStik.TimeTable.DataServices.Json.Data;

namespace MyStik.TimeTable.DataServices.Json
{
    public class ImportContext : BaseImportContext
    {
        public ImportContext()
        {
            ValidCourses = new List<ScheduleCourse>();
        }


        public ScheduleModel Model { get; set; }

        public List<ScheduleCourse> ValidCourses { get; private set; }
    }
}
