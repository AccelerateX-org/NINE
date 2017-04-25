using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using MyStik.TimeTable.Data;

namespace MyStik.TimeTable.Web.Models
{
    public class OccurrenceMailingModel
    {
        public Guid OccurrenceId { get; set; }

        public IActivitySummary Summary { get; set; }

        public string Name { get; set; }

        public ICollection<HttpPostedFileBase> Attachments { get; set; }

        [Required]
        [Display(Name = "Betreff")]
        public string Subject { get; set; }

        [Required]
        [Display(Name = "Nachricht")]
        [AllowHtml]
        public string Body { get; set; }

        public int ReceiverCount { get; set; }


        public string Room { get; set; }


        public DateTime From { get; set; }

        public DateTime Until { get; set; }

        [Display(Name = "Wichtige Nachricht für das Studium, geht an alle Studierenden, unabhängig von den Einstellungen im Benutzerprofil")]
        public bool IsImportant { get; set; }

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
    }

    public class MailDeliverSummaryReportModel
    {
        public MailDeliverSummaryReportModel()
        {
            Deliveries = new List<MailDeliveryReportModel>();
        }


        public List<MailDeliveryReportModel> Deliveries { get; private set; }
    }

    public class MailDeliveryReportModel
    {
        public ApplicationUser User { get; set; }

        public bool DeliverySuccessful { get; set; }

        
        public string ErrorMessage { get; set; }
    }

    public class ContactMailModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "E-Mail")]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Betreff")]
        public string Subject { get; set; }

        [Required]
        [Display(Name = "Nachricht")]
        public string Body { get; set; }
    }
}