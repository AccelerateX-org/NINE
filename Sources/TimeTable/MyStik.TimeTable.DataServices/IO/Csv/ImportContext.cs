using System.Collections.Generic;
using MyStik.TimeTable.DataServices.IO.Csv.Data;

namespace MyStik.TimeTable.DataServices.IO.Csv
{
    public class ImportContext : BaseImportContext
    {
        public ImportContext()
        {
            ValidCourses = new Dictionary<string, List<SimpleCourse>>();
        }


        public Dictionary<string, List<SimpleCourse>> ValidCourses { get; set; }

    }
}
