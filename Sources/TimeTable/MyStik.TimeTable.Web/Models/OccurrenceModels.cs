using System;
using System.ComponentModel.DataAnnotations;
using System.Text;
using MyStik.TimeTable.Data;

namespace MyStik.TimeTable.Web.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class OccurrenceStateModel
    {
        /// <summary>
        /// 
        /// </summary>
        public Occurrence Occurrence { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public OccurrenceSubscription Subscription { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool HasCapacity { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int CapacityLeft { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool IsFlexible { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public SubscriptionState State { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime SubscriptionStart
        {
            get
            {
                if (Occurrence.FromIsRestricted)
                {
                    if (Occurrence.FromDateTime.HasValue)
                    {
                        return Occurrence.FromDateTime.Value;
                    }
                }

                return GlobalSettings.Now;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string Label
        {
            get
            {
                var sb = new StringBuilder();
                if (Subscription != null)
                {
                    sb.Append("Austragen");
                }
                else
                {
                    sb.Append("Eintragen");
                }
                return sb.ToString();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool HasError { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string ErrorMessage { get; set; }
    }


    /// <summary>
    /// 
    /// </summary>
    public class SubscriptionRuleModel
    {
        /// <summary>
        /// 
        /// </summary>
        public IActivitySummary Summary { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Guid OccurrenceId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Display(Name = "Beschränkung der Anzahl der Teilnehmer")]
        public int CapacityOption { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Display(Name = "Anzahl Plätze")]
        public int Capacity { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Display(Name = "Beginn der Eintragung")]
        public int SubscriptionBeginLimitOption { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Display(Name = "Datum")]
        public string SubscriptionBegin { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Display(Name = "Uhrzeit")]
        public string SubscriptionBeginTime { get; set; }

        /// <summary>
        /// 
        /// </summary>

        [Display(Name = "Ende der Eintragung")]
        public int SubscriptionEndLimitOption { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Display(Name = "Datum")]
        public string SubscriptionEnd { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Display(Name = "Uhrzeit")]
        public string SubscriptionEndTime { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class OccurrenceGroupCapacityModel
    {
        /// <summary>
        /// 
        /// </summary>
        public Guid CourseId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Guid SemesterGroupId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int Capacity { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Curriculum { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Group { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Semester { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Organiser { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class OccurrenceCapacityOption
    {
        /// <summary>
        /// 
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool Selected { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public String Text { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool HasValue { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int Capacity { get; set; }
    }

}