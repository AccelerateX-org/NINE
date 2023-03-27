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

        public Semester PrevSemester { get; set; }

        public Semester NextSemester { get; set; }

        public TeachingSemesterSummaryModel CurrentSemester { get; set; }

        public TeachingSemesterSummaryModel PlaningSemester { get; set; }

        public List<OrganiserMember> Members { get; set; }

        public List<ThesisStateModel> ActiveTheses { get; set; }

        public List<CurriculumModule> Modules { get; set; }

    }


    public class TeachingSemesterSummaryModel
    {
        public TeachingSemesterSummaryModel()
        {
            Modules = new List<TeachingModuleSemesterModel>();
            Courses = new List<CourseSummaryModel>();
            OfficeHours = new List<OfficeHourCharacteristicModel>();
        }

        public Semester Semester { get; set; }

        public List<CourseSummaryModel> Courses { get; set;  }

        public List<OfficeHourCharacteristicModel> OfficeHours { get; set;  }

        public List<TeachingModuleSemesterModel> Modules { get; set; }
    }

    public class TeachingModuleSemesterModel
    {
        public TeachingModuleSemesterModel()
        {
            Courses = new List<CourseSummaryModel>();
        }

        public CurriculumModule Module { get; set; }
        
        public List<CourseSummaryModel> Courses { get; set; }
    }
}