using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyStik.TimeTable.Data;
using MyStik.TimeTable.DataServices.IO.Horst.Data;

namespace MyStik.TimeTable.DataServices.IO.Horst
{
    public class CritModuleBundle
    {
        public CurriculumCriteria Criteria { get; set; }
        public StudyProgramModule Module { get; set; }
    }

    public class ExportService
    {
        private TimeTableDbContext _db;
        private TimeTable.Data.Curriculum _curr;

        private StudyProgram _prog;

        private SortedList<int, CritModuleBundle> _critList = new SortedList<int, CritModuleBundle>();
        

        public ExportService(TimeTableDbContext db)
        {
            _db = db;
            
        }

        public StudyProgram GetProgram(Guid id, int no)
        {
            _curr = _db.Curricula.SingleOrDefault(x => x.Id == id);

            _prog = new StudyProgram
            {
                id = no,
                name = _curr.Name,
                shortcut = _curr.ShortName,
                modules = new List<StudyProgramModule>(),
                rules = new List<StudyProgramRuleSet>()
            };

            // alle Module bauen
            // Grundidee => Ein kriterium entspricht dem Modul
            // Erstmal ohne Studienrichtung, d.h. immer das erste Paket
            foreach (var pck in _curr.Packages)
            {
                foreach (var req in pck.Options.First().Requirements)
                {
                    // nicht nach Kriterien aufspalten
                    // immer das erste nehmen
                    AddCriteria(req.Criterias.First());
                }
            }
            // spezielle Abfragen
            // ects => summe ects module
            _prog.ects = _prog.modules.Sum(x => x.ects);
            // Studiendauer = höchste semesternummer
            _prog.standardStudyPeriod = _prog.modules.Max(x => x.regularSemester);

            // praktisches Semester => Name einer Anforderung "Praktikum"
            var critPract = _prog.modules.SingleOrDefault(x => x.title.Equals("Praktikum"));
            if (critPract != null)
            {
                _prog.practicalSemester = critPract.regularSemester;
            }

            // Jetzt die Regeln
            // Fakes
            var ruleSet = new StudyProgramRuleSet
            {
                studyProgramId = _prog.id,
                startRules = new List<StudyProgramStartRule>(),
                riseRules = new List<StudyProgramRiseRule>(),
                Prequisites = new List<StudyProgramPrequisite>(),
                retryDeadlines = new StudyProgramRetryDeadlines
                {
                    first = 1,
                    second = 2,
                    maxSecondFailCount = 5
                }
            };

            _prog.rules.Add(ruleSet);

            InitRules();

            return _prog;
        }

        private void AddCriteria(CurriculumCriteria crit)
        {
            var number = _critList.Count + 1;

            var module = new StudyProgramModule
            {
                id = number,
                title = crit.Requirement.Name,
                sws = 0,
                regularSemester = crit.Term,
                coursesBound = true,
                coursesUnrestricted = false,
                gop = false,
                locked = false,
                editable = false,
                exams = new List<StudyProgramExam>()
            };

            _critList[number] = new CritModuleBundle
            {
                Criteria = crit,
                Module = module
            };

            _prog.modules.Add(module);
        }

        private void InitRules()
        {
            
        }
    }
}
