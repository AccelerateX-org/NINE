using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using fk11.Model;

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
