using System.Collections.Generic;
using MyStik.TimeTable.DataServices.IO.Cie.Data;

namespace MyStik.TimeTable.DataServices.IO.Cie
{
    public class ImportContext : BaseImportContext
    {
        public ImportContext()
        {
            ValidCourses = new List<CieCourse>();
        }


        public CieModel Model { get; set; }

        public List<CieCourse> ValidCourses { get; private set; }
    }
}
