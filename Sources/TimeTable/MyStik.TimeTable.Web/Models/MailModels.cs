using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using MyStik.TimeTable.Data;

namespace MyStik.TimeTable.Web.Models
{
    public class UserMailModel
    {
        private List<CustomMailAttachtmentModel> _attachtments;

        public ApplicationUser User { get; set; }

        public string CustomBody { get; set; }

        public bool IsImportant { get; set; }

        public bool IsDistributionList { get; set; }

        public string ListName { get; set; }

        public string CustomSubject { get; set; }

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

    public class ConfirmEmailMailModel : UserMailModel
    {
        public string Token { get; set; }
    }

    public class ForgotPasswordMailModel : UserMailModel
    {
        public string Token { get; set; }
        
        public bool IsNewAccount { get; set; }
    }


    public class ResetPasswordMailModel : UserMailModel
    {
        public string Password { get; set; }
    }

    public class ActivityMailModel : UserMailModel
    {
        public IActivitySummary Summary { get; set; }

        public ApplicationUser SenderUser { get; set; }
    }

    public class SubscriptionMailModel : ActivityMailModel
    {

        public OccurrenceSubscription Subscription { get; set; }
    }

    public class CustomMailModel
    {
        public CustomMailModel()
        {
            Attachments = new Collection<CustomMailAttachtmentModel>();
            ReceiverUsers = new List<ApplicationUser>();
        }

        public ICollection<CustomMailAttachtmentModel> Attachments { get; set; }

        public string Subject { get; set; }

        public string Body { get; set; }

        public ApplicationUser SenderUser { get; set; }

        public List<ApplicationUser> ReceiverUsers { get; set; }

        public int ReceiverCount { get; set; }

        public bool IsImportant { get; set; }

        public string ListName { get; set; }
    }

    public class CustomMailAttachtmentModel
    {
        public string FileName { get; set; }
        
        public byte[] Bytes { get; set; }
    }

    public class DeleteUserMailModel : UserMailModel
    {
        
    }

    public class GenericMailDeliveryModel
    {
        public GenericMailDeliveryModel()
        {
            Attachments = new Collection<CustomMailAttachtmentModel>();
        }


        public ApplicationUser Sender { get; set; }
        public ApplicationUser Receiver { get; set; }

        public string Subject { get; set; }

        public ICollection<CustomMailAttachtmentModel> Attachments { get; set; }

        public string TemplateName { get; set; }

        public UserMailModel TemplateContent { get; set; }
    }
}