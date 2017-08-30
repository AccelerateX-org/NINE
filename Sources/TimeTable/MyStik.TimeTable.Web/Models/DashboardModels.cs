using System;
using System.Collections.Generic;
using MyStik.TimeTable.Data;

namespace MyStik.TimeTable.Web.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class DashboardViewModel
    {
        /// <summary>
        /// 
        /// </summary>
        public DashboardViewModel()
        {
            MyActivities = new List<ActivitySummary>();
            MyPreviousActivities = new List<ActivitySummary>();
            MySubscriptions = new List<ActivitySubscriptionStateModel>();
            MyCourseSubscriptions = new List<ActivitySubscriptionStateModel>();
        }
        /// <summary>
        /// 
        /// </summary>
        public ApplicationUser User { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Semester Semester { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Semester PreviousSemester { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<ActivitySubscriptionStateModel> MySubscriptions { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        public List<ActivitySubscriptionStateModel> MyCourseSubscriptions { get; private set; }

        #region OrgMember
        /// <summary>
        /// 
        /// </summary>
        public bool IsProf { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public OfficeHour OfficeHour { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public OfficeHourDatePreviewModel NextOfficeHourDate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public OrganiserMember Member { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public ActivityOrganiser Organiser { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<ActivitySummary> MyActivities { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        public List<ActivitySummary> MyPreviousActivities { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        public List<CurriculumModule> MyModules { get; set; }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        public ICollection<ActivityDate> NowPlayingDates { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public ICollection<ActivityDate> UpcomingDates { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public ICollection<OfficeHour> OfficeHours { get; set; }
    }
    /// <summary>
    /// 
    /// </summary>
    public class DashboardStudentViewModel : DashboardViewModel
    {
        /// <summary>
        /// 
        /// </summary>
        public DashboardStudentViewModel() : base()
        {
            CourseDates = new List<UserCourseDatePlanModel>();
        }

        /// <summary>
        /// 
        /// </summary>
        public SemesterGroup SemesterGroup { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public ActivityDate NextCourseDate { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public new ActivityDate NextOfficeHourDate { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<UserCourseDatePlanModel> CourseDates { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime NextSemesterDate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string NextSemesterDateDescription { get; set; }
    }
    /// <summary>
    /// 
    /// </summary>
    public class ActivitySubscriptionStateModel
    {
        /// <summary>
        /// 
        /// </summary>
        public IActivitySummary Activity { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public OccurrenceStateModel State { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Lottery Lottery { get; set; }
    }
}