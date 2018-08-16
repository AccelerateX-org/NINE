using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyStik.TimeTable.DataServices.Cie.Data
{
    public class CieModel
    {
        public CieModel()
        {
            Courses = new List<CieCourse>();
        }

        public List<CieCourse> Courses { get; }

    }
}
