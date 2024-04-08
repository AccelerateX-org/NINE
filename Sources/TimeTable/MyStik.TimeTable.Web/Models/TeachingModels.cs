using System;
using System.Collections.Generic;
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

        public List<ActivityOrganiser> Organisers { get; set; }

        public Student Student { get; set; }

        public Thesis Thesis { get; set; }

    }


    public class TeachingSemesterSummaryModel
    {
        public TeachingSemesterSummaryModel()
        {
            SubscribedCourses = new List<CourseSummaryModel>();
            OfferedCourses = new List<CourseSummaryModel>();
            OfferedOfficeHours = new List<OfficeHourCharacteristicModel>();
        }

        public Semester Semester { get; set; }

        public List<CourseSummaryModel> OfferedCourses { get; set;  }
        public List<CourseSummaryModel> SubscribedCourses { get; set; }

        public List<OfficeHourCharacteristicModel> OfferedOfficeHours { get; set;  }



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