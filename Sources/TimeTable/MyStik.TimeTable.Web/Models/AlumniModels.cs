using MyStik.TimeTable.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace MyStik.TimeTable.Web.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class AlumniStatisticModel
    {
        /// <summary>
        /// 
        /// </summary>
        public Curriculum Curriculum { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int CurrentStudentCount { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int AlumniCount { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class AlumniCreateFromFileModel
    {
        /// <summary>
        /// 
        /// </summary>
        public HttpPostedFileBase File { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime AlumniSince { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class AlumniFeedbackViewModel
    {
        private List<ApplicationUser> _success = new List<ApplicationUser>();
        private List<ApplicationUser> _error = new List<ApplicationUser>();
        private List<ApplicationUser> _suggestion = new List<ApplicationUser>();

        /// <summary>
        /// 
        /// </summary>
        public DateTime AlumniSince { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual List<ApplicationUser> success 
        {
            get { return _success; }
            set { _success = value; } 
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual List<ApplicationUser> error 
        {
            get { return _error; }
            set { _error = value; } 
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual List<ApplicationUser> suggestion 
        {
            get { return _suggestion; }
            set { _suggestion = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public string panelClass
        {
            get
            {
                if (success.Count > 0 && error.Count > 0)
                {
                    return "warning";
                }
                else if (success.Count > 0 && error.Count == 0)
                {
                    return "success";
                }
                return "danger";
            }

            set
            {
                panelClass = "default";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string panelHeading
        {
            get
            {
                if (success.Count > 0 && error.Count > 0)
                {
                    return "Vorsicht!";
                }
                else if (success.Count > 0 && error.Count == 0)
                {
                    return "Yippie!";
                }
                return "Oh Nein!";
            }
            set
            {
                panelHeading = "Feedback";
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class AlumniMailModel
    {
        /// <summary>
        /// 
        /// </summary>
        [Display(Name="An")]
        public List<string> mailTo { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Display(Name="Betreff")]
        public string subject { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Display(Name="Nachricht")]
        public string message { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Exception sendException { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class TakeSuggestionModel
    {
        /// <summary>
        /// 
        /// </summary>
        public string[] suggestedIDs { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime AlumniSince { get; set; }
    }
}