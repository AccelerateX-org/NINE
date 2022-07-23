using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MyStik.TimeTable.Data;

namespace MyStik.TimeTable.Web.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class CurriculumModuleCreateModel
    {
        public Guid CurriculumId { get; set; }

        /// <summary>
        /// 
        /// </summary>

        [Display(Name = "Fachlicher Schlüssel")]
        public string ModuleId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Display(Name = "Modulbezeichnung")]
        public string Name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Display(Name = "Stundenplankürzel")]
        public string ShortName { get; set; }


        public int LectureType { get; set; }

        public bool HasPractice { get; set; }
        public bool HasTutorium { get; set; }
        public bool HasLaboratory { get; set; }
    }


    public class ModuleSemesterCoursesModel
    {
        public ModuleSemesterCoursesModel()
        {
            Courses = new List<ModuleSemesterCourseModel>();
            Participants = new List<ModuleParticipantModel>();
        }


        public CurriculumModule Module { get; set; }

        public Semester Semester { get; set; }


        public List<ModuleSemesterCourseModel> Courses { get; private set; }

        public List<ModuleParticipantModel> Participants { get; private set; }
    }

    public class ModuleSemesterCourseModel
    {
        public ModuleSubject ModuleSubject { get; set; }

        public CourseSummaryModel CourseSummary { get; set; }

    }


    public class ModuleParticipantModel
    {
        public ModuleParticipantModel()
        {
            Courses = new List<ModuleParticipantSubscriptionModel>();
        }

        public string UserId { get; set; }

        public ApplicationUser User { get; set; }

        public Student Student { get; set; }

        public List<ModuleParticipantSubscriptionModel> Courses { get; private set; }
    }

    public class ModuleParticipantSubscriptionModel
    {
        public Course Course { get; set; }

        public OccurrenceSubscription Subscription { get; set; }

    }


    public class CurriculumModulePlan
    {
        public string faculty { get; set; }
        public string name { get; set; }

        public ICollection<CurriculumModulePlanSection> sections { get; set; }
    }

    public class CurriculumModulePlanSection
    {
        public int order { get; set; }

        public string name { get; set; }

        public ICollection<CurriculumModulePlanSlot> slots { get; set; }
    }

    public class CurriculumModulePlanSlot
    {
        public int position { get; set; }

        public string tag { get; set; }
        public int ects { get; set; }

        public string name { get; set; }
    }


    public class CurriculumModuleCatalogImportModel
    {
        public string name { get; set; }

        public string tag { get; set; }

        public List<CurriculumModuleImportModel> modules { get; set; }
    }

    public class CurriculumModuleImportModel
    {
        public string name { get; set; }

        public string tag { get; set; }

        public string responsible { get; set; }

        public List<CurriculumSubjectImportModel> subjects { get; set; }

        public List<CurriculumExamImportModel> exams { get; set; }
    }


    public class CurriculumSubjectImportModel
    {
        public string name { get; set; }

        public string tag { get; set; }

        public string type { get; set; }

        public int sws { get; set; }

    }


    public class AccreditationImportModel
    {
        public string curriculum { get; set; }

        public string version { get; set; }

        public List<ModuleAccreditationImportModel> accreditions { get; set; }
    }

    public class ModuleAccreditationImportModel
    {
        public string slot { get; set; }

        public string module { get; set; }
    }



    public class OpportunityImportModel
    {
        public string curriculum { get; set; }

        public string version { get; set; }

        public string semester { get; set; }

        public List<CourseOpportunityImportModel> opportunities { get; set; }
    }

    public class CourseOpportunityImportModel
    {
        public string subject { get; set; }

        public string course { get; set; }
    }

    public class CurriculumExamImportModel
    {
        public string name { get; set; }

        public List<CurriculumExamFractionImportModel> fractions { get; set; }
    }

    public class CurriculumExamFractionImportModel
    {
        public string tag { get; set; }
        public double weight { get; set; }
        public int minDuration { get; set; }
        public int maxDuration { get; set; }
        public double betterment { get; set; }
    }

}