using System;
using System.Collections.Generic;
using System.Linq;
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
            MySubscriptions = new List<ActivitySubscriptionStateModel>();
            MyCourseSubscriptions = new List<ActivitySubscriptionStateModel>();
            ThisSemesterActivities = new SemesterActivityModel();
            NextSemesterActivities = new SemesterActivityModel();
            PreviousSemesterActivities = new SemesterActivityModel();
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
        public Semester NextSemester { get; set; }

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
        public ICollection<ActivityDate> NowPlayingDates { get; set; }

        public SemesterActivityModel ThisSemesterActivities { get; private set; }

        public SemesterActivityModel NextSemesterActivities { get; private set; }

        public SemesterActivityModel PreviousSemesterActivities { get; private set; }

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
            Requests = new List<SupervisionRequestModel>();
            Theses = new List<ThesisDetailModel>();
            TodaysActivities = new List<AgendaActivityViewModel>();
            Lotteries = new List<Lottery>();
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
        public ActivityDate NextOfficeHourDate { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ActivitySlot NextOfficeHourSlot { get; set; }

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


        public SemesterGroup NextSemesterGroup { get; set; }

        public ICollection<ActivityOrganiser> ActiveOrgsSemester { get; set; }

        public ICollection<ActivityOrganiser> ActiveOrgsNextSemester { get; set; }

        public ICollection<SupervisionRequestModel> Requests { get; set; }

        public ICollection<ThesisDetailModel> Theses { get; set; }

        public ICollection<AgendaActivityViewModel> TodaysActivities { get; set; }

        public  ICollection<CourseSummaryModel> Courses { get; set; }

        public Student Student { get; set; }

        public ICollection<Lottery> Lotteries { get; set; }

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

    public class SemesterActivityModel
    {
        public SemesterActivityModel()
        {
            MyCourses = new List<ActivitySummary>();
            MyEvents = new List<ActivitySummary>();
            MyReservations = new List<ActivitySummary>();
            MyExams = new List<ActivitySummary>();
        }

        public Semester Semester { get; set; }

        public ActivityOrganiser Organiser { get; set; }

        public List<ActivitySummary> MyCourses { get; private set; }

        public List<ActivitySummary> MyEvents { get; private set; }

        public List<ActivitySummary> MyReservations { get; private set; }

        public List<ActivitySummary> MyExams { get; private set; }

        public OfficeHour MyOfficeHour { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public OfficeHourDatePreviewModel NextOfficeHourDate { get; set; }
    }
}