using MyStik.TimeTable.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;

namespace MyStik.TimeTable.Web.Models
{
    public class UserViewModel
    {
        public UserViewModel()
        {
            Messages = new List<string>();
        }

        /// <summary>
        /// Die Guid des Profils als "öffentliche Id"
        /// </summary>
        public string Id { get; set; }
        public string UserName { get; set; }

        public List<string> Messages { get; private set; }


    }

    public class UserAdminViewModel
    {
        public ApplicationUser User { get; set; }

        public bool IsSysAdmin { get; set; }

        public int SubscriptionCount { get; set; }

        public bool IsStudent { get; set; }

        public bool IsAlumni { get; set; }

        public ICollection<OrganiserMember> Members { get; set; }

        public bool MailConfirmed { get; set; }

        public SemesterGroup SemesterGroup { get; set; }
    }

    public class UserStatisticsModel
    {
        public UserStatisticsModel()
        {
            RoleStatistics = new List<RoleStatisticsModel>();
            OrgStatistics = new List<OrgStatisticsModel>();
        }

        public int UserCount { get; set; }

        public int RegisteredToday { get; set; }

        public int ApprovedToday { get; set; }

        public int LogedInToday { get; set; }
        public int WithCurriculum { get; set; }

        public int WithGroup { get; set; }

        public List<RoleStatisticsModel> RoleStatistics { get; private set; }

        public List<OrgStatisticsModel> OrgStatistics { get; private set; }
    }

    public class RoleStatisticsModel
    {
        public string RoleName { get; set; }

        public int UserCount { get; set; }
    }

    public class OrgStatisticsModel
    {
        public string OrgName { get; set; }

        public int UserCount { get; set; }

        public int AdminCount { get; set; }
    }

    public class UserSubscriptionModel
    {
        public OccurrenceSubscription Subscription { get; set; }

        public IActivitySummary Summary { get; set; }
    }

    public class UserCoursePlanViewModel
    {
        public UserCoursePlanViewModel()
        {
            CourseSubscriptions = new List<UserCourseSubscriptionViewModel>();
        }

        public ApplicationUser User { get; set; }

        public List<UserCourseSubscriptionViewModel> CourseSubscriptions { get; private set; }

        public Semester Semester { get; set; }
    }

    public class UserCourseSubscriptionViewModel
    {
        public CourseSummaryModel CourseSummary { get; set; }

        public OccurrenceSubscription Subscription { get; set; }

        public bool IsValid { get; set; }
    }

    public class SubTestModel
    {
        public ApplicationUser User { get; set; }
        public SemesterSubscription Subscription { get; set; }
    }

    public class UserManageViewModel
    {
        public ApplicationUser User { get; set; }

        public string EMail { get; set; }

    }
}