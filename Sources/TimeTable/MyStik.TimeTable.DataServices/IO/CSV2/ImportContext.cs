using System.Collections.Generic;
using MyStik.TimeTable.DataServices.IO.Csv.Data;

namespace MyStik.TimeTable.DataServices.IO.CSV2
{
    public class ImportContext : BaseImportContext
    {
        public Dictionary<ImportCourseId, List<ImportCourseDataSet>> Courses { get; set; } = new Dictionary<ImportCourseId, List<ImportCourseDataSet>>();

    }
}
