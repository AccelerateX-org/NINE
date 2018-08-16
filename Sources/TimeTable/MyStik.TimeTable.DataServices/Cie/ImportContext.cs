using System.Collections.Generic;
using MyStik.TimeTable.DataServices.Cie.Data;

namespace MyStik.TimeTable.DataServices.Cie
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
