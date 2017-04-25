using MyStik.TimeTable.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace MyStik.TimeTable.Web.Models
{
    public class AlumniStatisticModel
    {
        public Curriculum Curriculum { get; set; }
        public int CurrentStudentCount { get; set; }
        public int AlumniCount { get; set; }
    }

    public class AlumniCreateFromFileModel
    {
        public HttpPostedFileBase File { get; set; }
        public DateTime AlumniSince { get; set; }
    }
    public class AlumniFeedbackViewModel
    {
        private List<ApplicationUser> _success = new List<ApplicationUser>();
        private List<ApplicationUser> _error = new List<ApplicationUser>();
        private List<ApplicationUser> _suggestion = new List<ApplicationUser>();
        public DateTime AlumniSince { get; set; }

        public virtual List<ApplicationUser> success 
        {
            get { return _success; }
            set { _success = value; } 
        }
        public virtual List<ApplicationUser> error 
        {
            get { return _error; }
            set { _error = value; } 
        }
        public virtual List<ApplicationUser> suggestion 
        {
            get { return _suggestion; }
            set { _suggestion = value; }
        }

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

    public class AlumniMailModel
    {
        [Display(Name="An")]
        public List<string> mailTo { get; set; }
        [Display(Name="Betreff")]
        public string subject { get; set; }
        [Display(Name="Nachricht")]
        public string message { get; set; }
        public Exception sendException { get; set; }
    }

    public class TakeSuggestionModel
    {
        public string[] suggestedIDs { get; set; }
        public DateTime AlumniSince { get; set; }
    }
}