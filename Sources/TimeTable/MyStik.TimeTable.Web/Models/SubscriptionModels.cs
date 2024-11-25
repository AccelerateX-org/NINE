using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using MyStik.TimeTable.Data;

namespace MyStik.TimeTable.Web.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class SubscriptionViewModel
    {
        /// <summary>
        /// 
        /// </summary>
        public Guid OccurrenceId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Summary { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string EMail { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Display(Name="E-Mail Adresse speichern")]
        public bool SaveEMail { get; set; }


    }

    /// <summary>
    /// 
    /// </summary>
    public class UserSubscriptionViewModel
    {
        /// <summary>
        /// 
        /// </summary>
        public UserSubscriptionViewModel()
        {
            Subscriptions = new List<UserSubscriptionModel>();
            WPMSubscriptions = new Collection<UserSubscriptionModel>();
        }

        /// <summary>
        /// 
        /// </summary>
        public ICollection<UserSubscriptionModel> Subscriptions { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ICollection<UserSubscriptionModel> WPMSubscriptions { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ICollection<SemesterSubscription> Semester { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class SubscriptionCourseViewModel
    {
        /// <summary>
        /// 
        /// </summary>
        public Guid SubscriptionId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int Priority { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Course Course { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int Capacity { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int SubscriptionTotalCount { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int SubscriptionPrio1Count { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int SubscriptionPrio2Count { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime SubscriptionDateTime { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class SubscriptionDetailViewModel
    {
        /// <summary>
        /// 
        /// </summary>
        public OccurrenceSubscription Subscription { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ApplicationUser User { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public SemesterGroup SemesterGroup { get; set; }
    
    }

    /// <summary>
    /// 
    /// </summary>
    public class SemesterSubscriptionViewModel
    {
        /// <summary>
        /// 
        /// </summary>
        [Display(Name = "Semester")]
        public string Semester { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public SemesterGroup Group { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool HasData { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Display(Name = "Fakultät")]
        public String Faculty { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Display(Name = "Studienprogramm")]
        public string Curriculum { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Display(Name = "Semestergruppe")]
        public string CurrGroup { get; set; }

    }

    /// <summary>
    /// 
    /// </summary>
    public class SemesterSubscriptionOverviewModel
    {
        /// <summary>
        /// 
        /// </summary>
        public Semester Semester { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public SemesterGroup Group { get; set; }
        
    }


    public class CurriculumSubscriptionViewModel
    {
        /// <summary>
        /// 
        /// </summary>
        [Display(Name = "Semester Studienbeginn")]
        public Guid SemId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Display(Name = "Fakultät")]
        public Guid OrgId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Display(Name = "Studienprogramm")]
        public Guid CurrId { get; set; }


        [Display(Name = "Vollzeit")]
        public bool IsFullTime { get; set; }


        [Display(Name = "Teilzeit")]
        public bool IsPartTime { get; set; }

        
        public bool IsIncomer { get; set; }


        [Display(Name = "Duales Studium (Angabe frewillig)")]
        public bool IsDual { get; set; }

        [Display(Name = "Matrikelnummer")]
        public string Number { get; set; }


        public ActivityOrganiser Organiser { get; set; }

        public Curriculum Curriculum { get; set; }

        public Semester Semester { get; set; }

        public ApplicationUser User { get; set; }

        public Student Student { get; set; }
    }

}