using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MyStik.TimeTable.Data;

namespace MyStik.TimeTable.Web.Models
{
    public class AssessmentOverviewModel
    {
        public Curriculum Curriculum { get; set; }

        public ActivityOrganiser Organiser { get; set; }

        public List<Assessment> Assessments { get; set; }
    }

    public class AssessmentCreateModel
    {
        public string CurriculumShortName { get; set; }

        public string SemesterName { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Stage1Name { get; set; }

        public string Stage1Start { get; set; }

        public string Stage1End { get; set; }

        public string Stage2Name { get; set; }

        public string Stage2Start { get; set; }

        public string Stage2End { get; set; }
    }
}