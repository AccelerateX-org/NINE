using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Web;
using MyStik.TimeTable.Data;

namespace MyStik.TimeTable.Web.Models
{
    public class WPMSelectModel
    {
        public WPMSelectModel()
        {
            Subscriptions = new List<SubscriptionCourseViewModel>();
        }


        public ApplicationUser User { get; set; }

        public List<SubscriptionCourseViewModel> Subscriptions { get; set; }
    }

    public class WPMSummaryModel
    {
        public WPMSummaryModel()
        {
            PriorityList = new SortedList<int, int>();
        }

        public int WPMTotalCount { get; set; }
        public int WPMSubscribedCount { get; set; }

        public int SubscriptionCount { get; set; }

        public int SubscriberCount { get; set; }

        public SortedList<int, int> PriorityList { get; set; }
    }

    public class WPMAdminModel
    {
        public WPMAdminModel()
        {
            Subscriptions = new List<WPMSubscriptionModel>();
        }

        public ApplicationUser User { get; set; }

        public List<WPMSubscriptionModel> Subscriptions { get; private set; }

        public Curriculum Curriculum { get; set; }

        public int Confirmed { get; set; }

        public int MaxConfirmed { get; set; }

        public Lottery Lottery { get; set; }
    }

    public class WPMSubscriptionModel
    {
        public Course Course { get; set; }

        public CourseSummaryModel Summary { get; set; }

        public OccurrenceSubscription Subscription { get; set; }

        /// <summary>
        /// Eintragung gültig? Passt der Studiengang
        /// </summary>
        public bool IsValid { get; set; }

        /// <summary>
        /// Kann der Platz angenommen werden
        /// </summary>
        public bool IsAvailable { get; set; }

        public int Capacity { get; set; }

        public int Participients { get; set; }

        public int Pending { get; set; }

        public int Waiting { get; set; }

        public int Free { get; set; }


        public bool IsRestricted { get; set; }
    }

    public class WPMListModel
    {
        public WPMListModel()
        {
            WPM = new List<WPMDetailModel>();
            Curricula = new List<Curriculum>();
        }

        public Lottery Lottery { get; set; }

        public Semester Semester { get; set; }

        public ApplicationUser User { get; set; }

        public List<WPMDetailModel> WPM { get; private set; }

        public List<Curriculum> Curricula { get; private set; }

    }

    public class WPMDetailModel
    {
        public WPMDetailModel()
        {
            Capacites = new Dictionary<Curriculum, CapacityModel>();
        }

        public Course Course { get; set; }

        public CourseSummaryModel Summary { get; set; }

        public OccurrenceStateModel OccurrenceState { get; set; }

        public Dictionary<Curriculum, CapacityModel> Capacites { get; private set; }

        public int Capacity { get; set; }

        public int Participients { get; set; }

        public int Pending { get; set; }

        public int Waiting { get; set; }

        public int Free { get; set; }

        public string CapacityState { get; set; }
        public string ChancesState { get; set; }

        public bool Bookable { get; set; }
    }

    public class CapacityModel
    {
        public int Capacity { get; set; }

        public int Participients { get; set; }

        public int Pending { get; set; }

        public int Waiting { get; set; }

        public int Free { get; set; }

        public string CapacityState { get; set; }
        public string ChancesState { get; set; }

    }

    public class WpmSubscriptionDetailModel
    {
        public ApplicationUser User { get; set; }

        public SemesterGroup Group { get; set; }

        public int Confirmed { get; set;  }

        public int Reservations { get; set; }

        public int Waiting { get; set; }

        public DateTime FirstAction { get; set; }

        public DateTime LastAction { get; set; }
    }

    public class WpmSubscriptionMasterModel
    {
        public WpmSubscriptionMasterModel()
        {
            Subscriptions = new Dictionary<string, WpmSubscriptionDetailModel>();
        }

        public Lottery Lottery { get; set; }

        public ICollection<WpmSubscriptionDetailModel> SubscriptionList { get; set; }

        public Dictionary<string, WpmSubscriptionDetailModel> Subscriptions { get; private set; }

    }

}