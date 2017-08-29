using System;
using System.Collections.Generic;
using MyStik.TimeTable.Data;

namespace MyStik.TimeTable.Web.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class WPMSelectModel
    {
        /// <summary>
        /// 
        /// </summary>
        public WPMSelectModel()
        {
            Subscriptions = new List<SubscriptionCourseViewModel>();
        }

        /// <summary>
        /// 
        /// </summary>
        public ApplicationUser User { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<SubscriptionCourseViewModel> Subscriptions { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class WPMSummaryModel
    {
        /// <summary>
        /// 
        /// </summary>
        public WPMSummaryModel()
        {
            PriorityList = new SortedList<int, int>();
        }

        /// <summary>
        /// 
        /// </summary>
        public int WPMTotalCount { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int WPMSubscribedCount { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int SubscriptionCount { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int SubscriberCount { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public SortedList<int, int> PriorityList { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class WPMAdminModel
    {
        /// <summary>
        /// 
        /// </summary>
        public WPMAdminModel()
        {
            Subscriptions = new List<WPMSubscriptionModel>();
            Available = new List<WPMSubscriptionModel>();
        }

        /// <summary>
        /// 
        /// </summary>
        public ApplicationUser User { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<WPMSubscriptionModel> Subscriptions { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public List<WPMSubscriptionModel> Available { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public Curriculum Curriculum { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int Confirmed { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int MaxConfirmed { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Lottery Lottery { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class WPMSubscriptionModel
    {
        /// <summary>
        /// 
        /// </summary>
        public Course Course { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public CourseSummaryModel Summary { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public OccurrenceSubscription Subscription { get; set; }

        /// <summary>
        /// Eintragung gültig? Passt der Studiengang
        /// </summary>
        public bool IsValid { get; set; }

        /// <summary>
        /// Kann der Platz angenommen werden
        /// </summary>
        public bool IsAvailable { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int Capacity { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int Participients { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int Pending { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int Waiting { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int Free { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool IsRestricted { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class WPMListModel
    {
        /// <summary>
        /// 
        /// </summary>
        public WPMListModel()
        {
            WPM = new List<WPMDetailModel>();
            Curricula = new List<Curriculum>();
        }

        /// <summary>
        /// 
        /// </summary>
        public Lottery Lottery { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Semester Semester { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ApplicationUser User { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<WPMDetailModel> WPM { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public List<Curriculum> Curricula { get; private set; }

    }

    /// <summary>
    /// 
    /// </summary>
    public class WPMDetailModel
    {
        /// <summary>
        /// 
        /// </summary>
        public WPMDetailModel()
        {
            Capacites = new Dictionary<Curriculum, CapacityModel>();
        }

        /// <summary>
        /// 
        /// </summary>
        public Course Course { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public CourseSummaryModel Summary { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public OccurrenceStateModel OccurrenceState { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Dictionary<Curriculum, CapacityModel> Capacites { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public int Capacity { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int Participients { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int Pending { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int Waiting { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int Free { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string CapacityState { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string ChancesState { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool Bookable { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class CapacityModel
    {
        /// <summary>
        /// 
        /// </summary>
        public int Capacity { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int Participients { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int Pending { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int Waiting { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int Free { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string CapacityState { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string ChancesState { get; set; }

    }

    /// <summary>
    /// 
    /// </summary>
    public class WpmSubscriptionDetailModel
    {
        /// <summary>
        /// 
        /// </summary>
        public ApplicationUser User { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public SemesterGroup Group { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int Confirmed { get; set;  }

        /// <summary>
        /// 
        /// </summary>
        public int Reservations { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int Waiting { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime FirstAction { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime LastAction { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class WpmSubscriptionMasterModel
    {
        /// <summary>
        /// 
        /// </summary>
        public WpmSubscriptionMasterModel()
        {
            Subscriptions = new Dictionary<string, WpmSubscriptionDetailModel>();
        }

        /// <summary>
        /// 
        /// </summary>
        public Lottery Lottery { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ICollection<WpmSubscriptionDetailModel> SubscriptionList { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Dictionary<string, WpmSubscriptionDetailModel> Subscriptions { get; private set; }

    }

}