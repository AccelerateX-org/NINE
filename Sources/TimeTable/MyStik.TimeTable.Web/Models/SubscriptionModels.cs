using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using MyStik.TimeTable.Data;

namespace MyStik.TimeTable.Web.Models
{
    public class SubscriptionViewModel
    {
        public Guid OccurrenceId { get; set; }

        public string Name { get; set; }

        public string Summary { get; set; }

        public string EMail { get; set; }

        [Display(Name="E-Mail Adresse speichern")]
        public bool SaveEMail { get; set; }


    }


    public class UserSubscriptionViewModel
    {
        public UserSubscriptionViewModel()
        {
            Subscriptions = new List<UserSubscriptionModel>();
            WPMSubscriptions = new Collection<UserSubscriptionModel>();
        }


        public ICollection<UserSubscriptionModel> Subscriptions { get; set; }

        public ICollection<UserSubscriptionModel> WPMSubscriptions { get; set; }

        public ICollection<SemesterSubscription> Semester { get; set; }
    }


    public class SubscriptionCourseViewModel
    {
        public Guid SubscriptionId { get; set; }

        public int Priority { get; set; }

        public Course Course { get; set; }

        public int Capacity { get; set; }

        public int SubscriptionTotalCount { get; set; }

        public int SubscriptionPrio1Count { get; set; }

        public int SubscriptionPrio2Count { get; set; }

        public DateTime SubscriptionDateTime { get; set; }
    }

    public class SubscriptionDetailViewModel
    {
        public OccurrenceSubscription Subscription { get; set; }

        public ApplicationUser User { get; set; }

        public SemesterGroup SemesterGroup { get; set; }
    
    }
}