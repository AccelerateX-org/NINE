using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyStik.TimeTable.DataServices.Horst.Data
{
    public class StudyProgramModule
    {
        public int id { get; set; }
        public string title { get; set; }
        public int ects { get; set; }
        public  int sws { get; set; }
        public int regularSemester { get; set; }
        public bool coursesBound { get; set; }
        public bool coursesUnrestricted { get; set; }
        public bool gop { get; set; }
        public bool locked { get; set; }
        public bool editable { get; set; }

        public ICollection<StudyProgramExam> exams { get; set; }


        public string state { get; set; }
        public string stateClasses { get; set; }
        public bool courseWarning { get; set; }
        public string height { get; set; }
    }
}
