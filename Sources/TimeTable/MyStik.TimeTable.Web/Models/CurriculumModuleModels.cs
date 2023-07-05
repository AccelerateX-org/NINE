using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MyStik.TimeTable.Data;

namespace MyStik.TimeTable.Web.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class CurriculumModuleCreateModel
    {
        public Guid catalogId { get; set; }
        public Guid moduleId { get; set; }

        /// <summary>
        /// 
        /// </summary>

        [Display(Name = "Fachlicher Schlüssel")]
        public string Tag { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Display(Name = "Modulbezeichnung")]
        public string Name { get; set; }

        public string NameEn { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Display(Name = "Stundenplankürzel")]
        public string ShortName { get; set; }


        public string Prequisites { get; set; }

        public string Applicableness { get; set; }

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
        public string tag { get; set; }

        public string version { get; set; }

        public string institution { get; set; }
        public string name { get; set; }

        public int ectsTarget { get; set; }

        public string level { get; set; }


        public ICollection<CurriculumModulePlanSection> sections { get; set; }

        public ICollection<CurriculumModulePlanArea> areas { get; set; }
    }

    public class CurriculumModulePlanArea
    {
        public string tag { get; set; }
        public string name { get; set; }
        public ICollection<CurriculumModulePlanOption> options { get; set; }
    }

    public class CurriculumModulePlanOption
    {
        public string tag { get; set; }

        public ICollection<CurriculumModulePlanSlot> slots { get; set; }
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

        public int semester { get; set; }

        public string tag { get; set; }
        public double ects { get; set; }

        public string name { get; set; }

        public string label { get; set; }
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

        public List<ModuleResponsibleImportModel> responsible { get; set; }

        public List<CurriculumSubjectImportModel> subjects { get; set; }

        public List<CurriculumExamImportModel> exams { get; set; }
    }


    public class CurriculumSubjectImportModel
    {
        public string name { get; set; }

        public string tag { get; set; }

        public string type { get; set; }

        public double sws { get; set; }

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

        public string label { get; set; }

        public string areaslot { get; set; }

        public string sourceslot { get; set; }

        public string targetslot { get; set; }

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

    public class ModuleResponsibleImportModel
       {
        public string tag { get; set; }
    }

    public class CatalogCreateModel
    {
        public Guid catalogId { get; set; }
        public Guid orgId { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }
        public string Tag { get; set; }

        public ActivityOrganiser Organiser { get; set; }
    }
}