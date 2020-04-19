using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MyStik.TimeTable.Data;

namespace MyStik.TimeTable.Web.Models
{
    public class TeachingModuleCreateModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        [AllowHtml]
        public string Description { get; set; }
    }

    public class TeachingOverviewModel
    {
        public ApplicationUser User { get; set; }

        public TeachingSemesterSummaryModel CurrentSemester { get; set; }

        public TeachingSemesterSummaryModel PlaningSemester { get; set; }

        public List<OrganiserMember> Members { get; set; }

        public List<ThesisStateModel> ActiveTheses { get; set; }

        public List<CurriculumModule> Modules { get; set; }
    }


    public class TeachingSemesterSummaryModel
    {
        public Semester Semester { get; set; }

        public List<Course> Courses { get; set;  }

        public List<OfficeHour> OfficeHours { get; set;  }

    }
}