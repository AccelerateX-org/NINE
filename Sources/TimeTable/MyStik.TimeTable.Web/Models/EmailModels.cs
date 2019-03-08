using System.Collections.Generic;
using MyStik.TimeTable.Data;
using MyStik.TimeTable.DataServices.Lottery.Data;
using Postal;

namespace MyStik.TimeTable.Web.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class BaseEmail : Email
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="registration"></param>
        public BaseEmail(string registration) : base(registration)
        {
            
        }

        /// <summary>
        /// 
        /// </summary>
        public string From { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string To { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Subject { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class InactivityReportEmail : BaseEmail
    {
        /// <summary>
        /// 
        /// </summary>
        public InactivityReportEmail() : base("InactiveReport")
        {
            
        }

        /// <summary>
        /// 
        /// </summary>
        public int Count { get; set; }
    }

    public class LotteryDrawingReportEmail : BaseEmail
    {
        /// <summary>
        /// 
        /// </summary>
        public LotteryDrawingReportEmail(string mailTemplate) : base(mailTemplate)
        {
            Courses = new List<LotteryDrawingCourseReport>();
        }

        public ApplicationUser User { get; set; }

        public LotteryDrawing Drawing { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<LotteryDrawingCourseReport> Courses { get; private set; }
    }

    public class LotteryDrawingCourseReport
    {

        public Course Course { get; set; }

        public OccurrenceSubscription Subscription { get; set; }

        public int Rank
        {
            get
            {
                if (Subscription == null)
                    return 0;

                if (Subscription.OnWaitingList)
                    return 1;

                if (!Subscription.IsConfirmed)
                    return 2;

                return 3;
            }
        }

        public string State
        {
            get
            {
                if (Subscription == null)
                    return "nicht eingetragen";

                if (Subscription.OnWaitingList)
                    return "Warteliste";

                if (!Subscription.IsConfirmed)
                    return "Reservierung";

                return "Teilnehmer";
            }
        }


    }



    /// <summary>
    /// 
    /// </summary>
    public class CustomBodyEmail : BaseEmail
    {
        /// <summary>
        /// 
        /// </summary>
        public CustomBodyEmail() : base("CustomBodyMail")
        {

        }

        /// <summary>
        /// 
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool IsImportant { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool IsDistributionList { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string ListName { get; set; }
    }

    public class LotteryDrawingStudentEmail : BaseEmail
    {
        /// <summary>
        /// 
        /// </summary>
        public LotteryDrawingStudentEmail(string mailTemplate) : base(mailTemplate)
        {
            
        }

        public ApplicationUser User { get; set; }

        public DrawingGame Game { get; set; }

        public LotteryDrawing Drawing { get; set; }

        public OrganiserMember Member { get; set; }

        public Course Course { get; set; }

        public OccurrenceSubscription Subscription { get; set; }

        public Lottery Lottery { get; set; }

    }

    public class SubscriptionEmail : BaseEmail
    {
        public SubscriptionEmail(string registration) : base(registration)
        {
        }

        public Course Course { get; set; }

        public OccurrenceSubscription Subscription { get; set; }

        public ApplicationUser Actor { get; set; }

        public ApplicationUser Student { get; set; }

    }

    public class ThesisEmail : BaseEmail
    {
        public ThesisEmail(string registration) : base(registration)
        {
        }

        public ThesisStateModel Thesis { get; set; }

        public OrganiserMember Member { get; set; }

        public ApplicationUser User { get; set; }

        public string Body { get; set; }
    }

}