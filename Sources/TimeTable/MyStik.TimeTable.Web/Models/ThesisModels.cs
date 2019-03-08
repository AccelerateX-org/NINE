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

    public enum RequestState
    {
        None,
        InProgress,
        Accepted,
        Rejected
    }

    public class ThesisStateModel
    {
        public ApplicationUser User { get; set; }

        public Student Student { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Thesis Thesis { get; set; }

        public RequestState ConditionRequest
        {
            get
            {
                // noch keine Arbeit oder noch kein Antrag gestellt
                if (Thesis?.RequestDate == null)
                    return RequestState.None;

                // Antrag gestellt, noch keine Antwort
                if (!Thesis.ResponseDate.HasValue)
                    return RequestState.InProgress;

                // Antrag angenommen
                if (Thesis.IsPassed.HasValue && Thesis.IsPassed.Value)
                    return RequestState.Accepted;

                // Antrag abgelehnt
                return RequestState.Rejected;
            }
        }

        public RequestState SupervisionRequest
        {
            get
            {
                // keine Betreuer
                if (!Thesis.Supervisors.Any())
                {
                    // alle haben abgelehnt => dann gab es schon eine Anfrage
                    if (Thesis.AcceptanceDate.HasValue)
                        return RequestState.Rejected;
                    // noch keinen Antrag gestellt
                    return RequestState.None;
                }

                // Es muss mindestens einer angenommen haben
                if (Thesis.Supervisors.Any(x => x.AcceptanceDate.HasValue))
                    return RequestState.Accepted;

                return RequestState.InProgress;
            }
        }

        public string GetStateMessage(OrganiserMember member)
        {
            var nSupervisors = Thesis.Supervisors.Count;
            var didIAccepted = Thesis.Supervisors.Any(x => x.Member.Id == member.Id && x.AcceptanceDate.HasValue);

            if (nSupervisors == 1)
            {
                if (didIAccepted)
                    return "Arbeit angenommen";

                return "Betreuungsanfrage";
            }

            var nAccepted = Thesis.Supervisors.Count(x => x.AcceptanceDate.HasValue);

            if (nAccepted == 0)
                return $"Betreuungsanfrage bei {nSupervisors} Lehrenden.";

            if (didIAccepted)
            {
                if (nAccepted == 1)
                {
                    return
                        $"Arbeit wurde von mir angenommen. Von {nSupervisors - 1} Lehrenden steht die Antwort noch aus.";
                }

                return $"Arbeit wurde von mir und {nAccepted - 1} Lehrenden angenommen.";
            }

            if (nAccepted > 0)
                return $"Arbeit wurde bereits von {nAccepted} Lehrenden angenommen.";

            return "Betreuungsanfrage";
        }



    }

    public class ThesisSupervisionModel
    {
        public Thesis Thesis { get; set; }

        public Guid OrganiserId { get; set; }
    }

}
