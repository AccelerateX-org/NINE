using System.Collections.Generic;

namespace MyStik.TimeTable.DataServices.IO.Cie.Data
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
