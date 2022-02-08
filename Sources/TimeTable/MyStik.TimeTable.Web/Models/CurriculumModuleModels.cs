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
        public ModuleCourse ModuleCourse { get; set; }

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
    }



}