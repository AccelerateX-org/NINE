using System;
using MyStik.TimeTable.Data;
using System.Collections.Generic;

namespace MyStik.TimeTable.Web.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class UserViewModel
    {
        /// <summary>
        /// 
        /// </summary>
        public UserViewModel()
        {
            Messages = new List<string>();
        }

        /// <summary>
        /// Die Guid des Profils als "öffentliche Id"
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<string> Messages { get; private set; }


    }

    /// <summary>
    /// 
    /// </summary>
    public class UserAdminViewModel
    {
        /// <summary>
        /// 
        /// </summary>
        public ApplicationUser User { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool IsSysAdmin { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int SubscriptionCount { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool IsStudent { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool IsAlumni { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ICollection<OrganiserMember> Members { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool MailConfirmed { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public SemesterGroup SemesterGroup { get; set; }

        public ICollection<Student> Students { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class UserStatisticsModel
    {
        /// <summary>
        /// 
        /// </summary>
        public UserStatisticsModel()
        {
            RoleStatistics = new List<RoleStatisticsModel>();
            OrgStatistics = new List<OrgStatisticsModel>();
        }

        /// <summary>
        /// 
        /// </summary>
        public int UserCount { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int RegisteredToday { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int ApprovedToday { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int LogedInToday { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int WithCurriculum { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int WithGroup { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<RoleStatisticsModel> RoleStatistics { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public List<OrgStatisticsModel> OrgStatistics { get; private set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class RoleStatisticsModel
    {
        /// <summary>
        /// 
        /// </summary>
        public string RoleName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int UserCount { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class OrgStatisticsModel
    {
        /// <summary>
        /// 
        /// </summary>
        public string OrgName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int UserCount { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int AdminCount { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class UserSubscriptionModel
    {
        /// <summary>
        /// 
        /// </summary>
        public OccurrenceSubscription Subscription { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public IActivitySummary Summary { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class UserCoursePlanViewModel
    {
        /// <summary>
        /// 
        /// </summary>
        public UserCoursePlanViewModel()
        {
            CourseSubscriptions = new List<UserCourseSubscriptionViewModel>();
            CourseDates = new List<UserCourseDatePlanModel>();
        }

        /// <summary>
        /// 
        /// </summary>
        public ApplicationUser User { get; set; }

        public Student Student { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<UserCourseSubscriptionViewModel> CourseSubscriptions { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public Semester Semester { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<UserCourseDatePlanModel> CourseDates { get; private set; }

        public OccurrenceSubscription Subscription { get; set; }

        public Course Course { get; set; }

        public CourseSummaryModel Summary { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class UserCourseDatePlanModel
    {
        /// <summary>
        /// 
        /// </summary>
        public UserCourseDatePlanModel()
        {
            Dates = new List<ActivityDate>();
        }

        /// <summary>
        /// 
        /// </summary>
        public DateTime Day { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<ActivityDate> Dates { get; private set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class UserCourseSubscriptionViewModel
    {
        /// <summary>
        /// 
        /// </summary>
        public CourseSummaryModel CourseSummary { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public OccurrenceSubscription Subscription { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool IsValid { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class SubTestModel
    {
        /// <summary>
        /// 
        /// </summary>
        public ApplicationUser User { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public SemesterSubscription Subscription { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class UserManageViewModel
    {
        /// <summary>
        /// 
        /// </summary>
        public ApplicationUser User { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string EMail { get; set; }

    }

    public class UserMemberModel
    {
        public UserMemberModel()
        {
            Members = new List<OrganiserMember>();
        }

        public ApplicationUser User { get; set; }

        public ICollection<OrganiserMember> Members { get; set; }
    }
}