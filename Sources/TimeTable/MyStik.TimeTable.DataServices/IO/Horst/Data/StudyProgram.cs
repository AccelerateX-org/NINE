using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyStik.TimeTable.DataServices.IO.Horst.Data
{
    public class StudyProgram
    {
        public int id { get; set; }

        public string name { get; set; }

        public string shortcut { get; set; }

        public int ects { get; set; }

        public int standardStudyPeriod { get; set; }

        public int practicalSemester { get; set; }

        public ICollection<StudyProgramModule> modules { get; set; }

        public ICollection<StudyProgramRuleSet> rules { get; set; }
    }
}
