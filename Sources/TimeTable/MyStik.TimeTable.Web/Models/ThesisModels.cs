using MyStik.TimeTable.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace MyStik.TimeTable.Web.Models
{
    public class ThesisOverviewModel
    {
        public ThesisOverviewModel()
        {
            Thesis = new List<ThesisOfferViewModel>();
        }

        public Semester Semester { get; set; }

        public ActivityOrganiser Organiser { get; set; }

        public List<ThesisOfferViewModel> Thesis { get; private set; }
    }


    public class ThesisOfferViewModel
    {
        public Exam Thesis { get; set; }

        public OrganiserMember Lecturer { get; set; }

        public Curriculum Curriculum { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class ThesisAdminModel
    {
        public ThesisAdminModel()
        {
            Exams = new List<ThesisExamModel>();
            Requests = new List<ThesisRequestModel>();
        }

        /// <summary>
        /// 
        /// </summary>
        public Exam Thesis { get; set; }

        public List<ThesisRequestModel> Requests { get; private set; }

        public List<ThesisExamModel> Exams { get; private set; }
    }

    public class ThesisRequestModel
    {
        public OccurrenceSubscription Subscription { get; set; }

        public ApplicationUser User { get; set; }
    }


    public class ThesisExamModel
    {
        public StudentExam Exam { get; set; }

        public OccurrenceSubscription Subscription { get; set; }

        public ApplicationUser User { get; set; }
    }


    /// <summary>
    /// 
    /// </summary>
    public class ThesisDetailModel
    {
        public ApplicationUser User { get; set; }

        public Student Student { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Thesis Thesis { get; set; }

        public OrganiserMember Lecturer { get; set; }

        public OccurrenceSubscription Subscription { get; set; }
    }



    public class ThesisEditModel
    {
        /// <summary>
        /// 
        /// </summary>
        public Thesis Thesis { get; set; }

        public string TitleDe { get; set; }

        public string TitleEn { get; set; }
    }


    /// <summary>
    /// 
    /// </summary>
    public class ThesisCreateModel
    {
        /// <summary>
        /// 
        /// </summary>
        [Required]
        public string Title { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Lecturer { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Start { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public string End { get; set;  }

    }

    /// <summary>
    /// 
    /// </summary>
    public class ThesisExamCreateModel
    {
        /// <summary>
        /// 
        /// </summary>
        public Guid ExamId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string StudentUserId { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Guid FirstExaminerId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Guid SecondExaminerId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Start { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string End { get; set; }
    }

    public class ThesisRejectModel
    {
        public Exam Thesis { get; set; }

        public OccurrenceSubscription Subscription { get; set; }

        public ApplicationUser User { get; set; }

    }

    public class ThesisAcceptModel
    {
        public Supervision Supervision { get; set; }

        public OccurrenceSubscription Subscription { get; set; }

        public Curriculum Curriculum { get; set; }

        public OrganiserMember Lecturer { get; set; }

        public ApplicationUser User { get; set; }


        [Display(Name = "Titel")]
        public string Title { get; set; }

        public int Period { get; set; }

        public DateTime IssueDate { get; set; }

        public DateTime DeliveryDate { get; set; }
    }

    public class ThesisSemesterSummaryModel
    {
        public Semester Semester { get; set; }
        
        public List<Supervision> Supervisions { get; set; }

        public List<Thesis> Theses { get; set; }
    }

    public class ThesisIssueModel
    {
        public string UserName { get; set; }

        public string TitleGer { get; set; }

        public string TitleEng { get; set; }

        public string IssueDate { get; set; }
    }


    public enum ThesisStage
    {
        Unknown,
        InRequest,
        InSearch,
        InProgress,
        InExamination
    }

    public class ThesisStateModel
    {
        public ApplicationUser User { get; set; }

        public Student Student { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Thesis Thesis { get; set; }


        public ThesisStage Stage
        {
            get
            {
                // Antrag gestellt ODERR
                // Antrag wurde abgelehnt
                if (!Thesis.ResponseDate.HasValue || 
                    (Thesis.IsPassed.HasValue && !Thesis.IsPassed.Value) ||
                    (Thesis.IsPassed.HasValue && Thesis.IsPassed.Value && !Thesis.Supervisors.Any()))
                    return ThesisStage.InRequest;

                if (!Thesis.IsAccepted.HasValue)
                    return ThesisStage.InSearch;

                return ThesisStage.Unknown;
            }

        }

        public string StageText
        {
            get
            {
                switch (Stage)
                {
                    case ThesisStage.InRequest:
                        return "Prüfung der Voraussetzungen";

                    case ThesisStage.InSearch:
                        return "Suche nach Betreunden";
                }

                return "Unbekannt";
            }
        }

        public string StateText
        {
            get
            {
                switch (Stage)
                {
                    case ThesisStage.InRequest:
                        if (Thesis.ResponseDate.HasValue)
                            return Thesis.IsPassed.Value ? "Voraussetzungen erfüllt" : "Voraussetzungen nicht erfüllt";
                        return "angefragt";

                    case ThesisStage.InSearch:
                        return "angefragt";
                }

                return "unbekannt";
            }
        }


        public DateTime? LastActionDate
        {
            get
            {
                switch (Stage)
                {
                    case ThesisStage.InRequest:
                        if (Thesis.ResponseDate.HasValue)
                            return Thesis.ResponseDate;
                        return Thesis.RequestDate;

                    case ThesisStage.InSearch:
                        return Thesis.ResponseDate;
                }

                return null;
            }
        }

        public string LastActionUser
        {
            get
            {
                switch (Stage)
                {
                    case ThesisStage.InRequest:
                        if (Thesis.ResponseDate.HasValue)
                            return Thesis.RequestAuthority.FullName;
                        return "Studierende(r)";

                    case ThesisStage.InSearch:
                        return "Studierende(r)";
                }

                return "unbekannt";
            }
        }




    }

}
