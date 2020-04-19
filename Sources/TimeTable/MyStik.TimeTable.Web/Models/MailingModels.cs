using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Web.Mvc;

namespace MyStik.TimeTable.Web.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class OccurrenceMailingModel
    {
        /// <summary>
        /// 
        /// </summary>
        public Guid OccurrenceId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public IActivitySummary Summary { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ICollection<HttpPostedFileBase> Attachments { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Required]
        [Display(Name = "Betreff")]
        public string Subject { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Required]
        [Display(Name = "Nachricht")]
        [AllowHtml]
        public string Body { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int ReceiverCount { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Room { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime From { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime Until { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Display(Name = "Wichtige Nachricht für das Studium, geht an alle Studierenden, unabhängig von den Einstellungen im Benutzerprofil")]
        public bool IsImportant { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string ListName { get; set; }

        /// <summary>
        /// Name der Mailvorlage, z.B. falls es mal verschiedene Inhaltsvorlagen gibt
        /// </summary>
        public string TemplateName { get; set; }

        /// <summary>
        /// Sprache des verwendeten Kürzels
        /// de-DE, etc. nach Standard
        /// </summary>
        [Display(Name = "Sprache der Vorlage")]
        public string TemplateLanguage { get; set; }

        /// <summary>
        /// true: wenn es ein Gruppenverteiler ist
        /// </summary>
        public bool IsDistributionList { get; set; }


        public ICollection<SemesterGroupViewModel> GroupList { get; set; }

        public string GroupIds { get; set; }

        public ICollection<string> GroupIdList { get; set; }

        [Display(Name = "Semester Studienbeginn")]
        public Guid SemId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Display(Name = "Studienprogramm")]
        public Guid CurrId { get; set; }

        public Guid ModuleId { get; set; }

    }

    /// <summary>
    /// 
    /// </summary>
    public class MailDeliverSummaryReportModel
    {
        /// <summary>
        /// 
        /// </summary>
        public MailDeliverSummaryReportModel()
        {
            Deliveries = new List<MailDeliveryReportModel>();
        }

        /// <summary>
        /// 
        /// </summary>
        public List<MailDeliveryReportModel> Deliveries { get; private set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class MailDeliveryReportModel
    {
        /// <summary>
        /// 
        /// </summary>
        public ApplicationUser User { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool DeliverySuccessful { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string ErrorMessage { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class ContactMailModel
    {
        /// <summary>
        /// 
        /// </summary>
        [Required]
        [EmailAddress]
        [Display(Name = "Email", ResourceType =typeof(Resources))]
        public string Email { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Required]
        [Display(Name = "ContactMailSubject", ResourceType =typeof(Resources))]
        public string Subject { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Required]
        [Display(Name = "ContactMailBody", ResourceType =typeof(Resources))]
        public string Body { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class MailJobModel
    {
        /// <summary>
        /// 
        /// </summary>
        public MailJobModel()
        {
            Files = new List<CustomMailAttachtmentModel>();
        }

        /// <summary>
        /// 
        /// </summary>
        public string SenderId { get; set; }

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
        public List<CustomMailAttachtmentModel> Files { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public Guid OrgId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Guid SemesterId { get; set; }

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
}