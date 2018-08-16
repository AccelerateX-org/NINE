using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyStik.TimeTable.DataServices.Horst.Data
{
    public class StudyProgramRuleSet
    {
        public int studyProgramId { get; set; }

        public ICollection<StudyProgramStartRule> startRules { get; set; }
        public ICollection<StudyProgramRiseRule> riseRules { get; set; }

        public ICollection<StudyProgramPrequisite> Prequisites { get; set; }

        public StudyProgramRetryDeadlines retryDeadlines { get; set; }
    }

    public class StudyProgramStartRule
    {
        public int inSemester { get; set; }
        public ICollection<int> modules { get; set; }
        public string explanation { get; set; }
    }

    public class StudyProgramRiseRule
    {
        public int forSemester { get; set; }
        public ICollection<int> modules { get; set; }
        public int minEcts { get; set; }
        public string explanation { get; set; }

    }

    public class StudyProgramPrequisite
    {
        public int forModule { get; set; }
        public ICollection<int> modules { get; set; }
    }

    public class StudyProgramRetryDeadlines
    {
        public int first { get; set; }

        public int second { get; set; }

        public int maxSecondFailCount { get; set; }
    }

}
