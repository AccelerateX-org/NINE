using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MyStik.TimeTable.Data;

namespace MyStik.TimeTable.Web.Models
{
    public class StudyPlanViewModel
    {
        public DateTime TimeStamp { get; set; }

        public string Remark { get; set; }

        public Curriculum Curriculum { get; set; }

        public Semester Semester { get; set; }

        public List<CurriculumModule> Modules { get; set;}
    }
}