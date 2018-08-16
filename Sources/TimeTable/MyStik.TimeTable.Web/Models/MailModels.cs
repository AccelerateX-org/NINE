using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using MyStik.TimeTable.Data;

namespace MyStik.TimeTable.Web.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class UserMailModel
    {
        private List<CustomMailAttachtmentModel> _attachtments;

        /// <summary>
        /// 
        /// </summary>
        public ApplicationUser User { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string CustomBody { get; set; }

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

        /// <summary>
        /// 
        /// </summary>
        public string CustomSubject { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<CustomMailAttachtmentModel> Attachments
        {
            get
            {
                if (_attachtments == null)
                    _attachtments = new List<CustomMailAttachtmentModel>();
                return _attachtments;
            }

            set { _attachtments = value; }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class ConfirmEmailMailModel : UserMailModel
    {
        /// <summary>
        /// 
        /// </summary>
        public string Token { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class ForgotPasswordMailModel : UserMailModel
    {
        /// <summary>
        /// 
        /// </summary>
        public string Token { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public bool IsNewAccount { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class ResetPasswordMailModel : UserMailModel
    {
        /// <summary>
        /// 
        /// </summary>
        public string Password { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class ActivityMailModel : UserMailModel
    {
        /// <summary>
        /// 
        /// </summary>
        public IActivitySummary Summary { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ApplicationUser SenderUser { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class SubscriptionMailModel : ActivityMailModel
    {
        /// <summary>
        /// 
        /// </summary>
        public OccurrenceSubscription Subscription { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class CustomMailModel
    {
        /// <summary>
        /// 
        /// </summary>
        public CustomMailModel()
        {
            Attachments = new Collection<CustomMailAttachtmentModel>();
            ReceiverUsers = new List<ApplicationUser>();
        }

        /// <summary>
        /// 
        /// </summary>
        public ICollection<CustomMailAttachtmentModel> Attachments { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ApplicationUser SenderUser { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<ApplicationUser> ReceiverUsers { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int ReceiverCount { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool IsImportant { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string ListName { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class CustomMailAttachtmentModel
    {
        /// <summary>
        /// 
        /// </summary>
        public string FileName { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public byte[] Bytes { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class DeleteUserMailModel : UserMailModel
    {
        
    }

    /// <summary>
    /// 
    /// </summary>
    public class GenericMailDeliveryModel
    {
        /// <summary>
        /// 
        /// </summary>
        public GenericMailDeliveryModel()
        {
            Attachments = new Collection<CustomMailAttachtmentModel>();
        }

        /// <summary>
        /// 
        /// </summary>
        public ApplicationUser Sender { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ApplicationUser Receiver { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ICollection<CustomMailAttachtmentModel> Attachments { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string TemplateName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public UserMailModel TemplateContent { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class OrgMemberMailModel : UserMailModel
    {
        /// <summary>
        /// 
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ActivityOrganiser Organiser { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class ThesisRequestMailModel : UserMailModel
    {
        /// <summary>
        /// 
        /// </summary>
        public Supervision Supervision { get; set; }

        public OccurrenceSubscription Request { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ApplicationUser Requester { get; set; }
    }


    /// <summary>
    /// 
    /// </summary>
    public class ThesisRejectMailModel : UserMailModel
    {
        /// <summary>
        /// 
        /// </summary>
        public Exam Thesis { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ApplicationUser Lecturer { get; set; }
    }

    public class MemberMoveDateMailModel : UserMailModel
    {
        public ApplicationUser SourceUser { get; set; }
        public ApplicationUser TargetUser { get; set; }

        public OrganiserMember SourceMember { get; set; }

        public OrganiserMember TargetMember { get; set; }

        public OrganiserMember AdminMember { get; set; }

        public ICollection<ActivityDate> Dates { get; set; }
    }

    public class LotterySelectionMailModel : UserMailModel
    {
        public LotterySelectionMailModel()
        {
            Courses = new List<LotteryOverviewCourseModel>();
        }
        public Lottery Lottery { get; set; }

        public Student Student { get; set; }

        public LotteryGame Game { get; set; }

        public List<LotteryOverviewCourseModel> Courses { get; set; }

        public int ConfirmCount { get; set; }

        public bool AcceptAny { get; set; }

        public DateTime SelectDate { get; set; }
    }
}