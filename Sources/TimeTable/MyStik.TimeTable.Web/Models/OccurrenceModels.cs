using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Web;
using MyStik.TimeTable.Data;

namespace MyStik.TimeTable.Web.Models
{
    public class OccurrenceStateModel
    {
        public Occurrence Occurrence { get; set; }

        public OccurrenceSubscription Subscription { get; set; }

        public bool HasCapacity { get; set; }

        public int CapacityLeft { get; set; }

        public bool IsFlexible { get; set; }

        public SubscriptionState State { get; set; }

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


        public bool HasError { get; set; }

        public string ErrorMessage { get; set; }
    }


    public class SubscriptionRuleModel
    {
        public IActivitySummary Summary { get; set; }

        public Guid OccurrenceId { get; set; }

        [Display(Name = "Beschränkung der Anzahl der Teilnehmer")]
        public int CapacityOption { get; set; }

        [Display(Name = "Anzahl Plätze")]
        public int Capacity { get; set; }

        [Display(Name = "Beginn der Eintragung")]
        public int SubscriptionBeginLimitOption { get; set; }

        [Display(Name = "Datum")]
        public string SubscriptionBegin { get; set; }

        [Display(Name = "Uhrzeit")]
        public string SubscriptionBeginTime { get; set; }


        [Display(Name = "Ende der Eintragung")]
        public int SubscriptionEndLimitOption { get; set; }

        [Display(Name = "Datum")]
        public string SubscriptionEnd { get; set; }

        [Display(Name = "Uhrzeit")]
        public string SubscriptionEndTime { get; set; }
    }

    public class OccurrenceGroupCapacityModel
    {
        public Guid CourseId { get; set; }

        public Guid SemesterGroupId { get; set; }

        public int Capacity { get; set; }

        public string Curriculum { get; set; }

        public string Group { get; set; }

        public string Semester { get; set; }

        public string Organiser { get; set; }
    }

    public class OccurrenceCapacityOption
    {
        public int Id { get; set; }

        public bool Selected { get; set; }

        public String Text { get; set; }

        public bool HasValue { get; set; }

        public int Capacity { get; set; }
    }

}