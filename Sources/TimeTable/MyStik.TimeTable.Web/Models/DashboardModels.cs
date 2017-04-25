using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MyStik.TimeTable.Data;

namespace MyStik.TimeTable.Web.Models
{
    public class DashboardViewModel
    {
        public DashboardViewModel()
        {
            MyActivities = new List<ActivitySummary>();
            MySubscriptions = new List<ActivitySubscriptionStateModel>();
            MyCourseSubscriptions = new List<ActivitySubscriptionStateModel>();
        }

        public ApplicationUser User { get; set; }

        public Semester Semester { get; set; }

        public List<ActivitySubscriptionStateModel> MySubscriptions { get; private set; }

        public List<ActivitySubscriptionStateModel> MyCourseSubscriptions { get; private set; }

        #region OrgMember

        public bool IsProf { get; set; }
        
        public OfficeHour OfficeHour { get; set; }

        public OfficeHourDatePreviewModel NextOfficeHourDate { get; set; }

        public OrganiserMember Member { get; set; }

        public ActivityOrganiser Organiser { get; set; }

        public List<ActivitySummary> MyActivities { get; private set; }

        #endregion


        public List<ActivityDate> CurrentDates { get; set; }

        public List<OrganiserMember> ActiveMembers { get; set; }


        public ICollection<RoomInfoModel> AvailableRooms { get; set; }

        public List<ActivityDate> CanceledDates { get; set; }



        #region Students
        public List<SemesterGroup> SemesterGroups { get; set; }

        public string Curriculum { get; set; }

        public string CurrGroup { get; set; }

        public ICollection<OfficeHourDateViewModel> OfficeHours { get; set; }
        #endregion
    }

    public class ActivitySubscriptionStateModel
    {
        public IActivitySummary Activity { get; set; }
        public OccurrenceStateModel State { get; set; }

        public Lottery Lottery { get; set; }
    }
}