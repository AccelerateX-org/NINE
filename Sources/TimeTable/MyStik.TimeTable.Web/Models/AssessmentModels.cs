using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Optimization;
using MyStik.TimeTable.Data;

namespace MyStik.TimeTable.Web.Models
{
    public class AssessmentOverviewModel
    {
        public Curriculum Curriculum { get; set; }

        public ActivityOrganiser Organiser { get; set; }

        public List<Assessment> Assessments { get; set; }

    }

    public class AssessmentCreateModel
    {
        public string CurriculumShortName { get; set; }

        public string SemesterName { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Stage1Name { get; set; }

        public string Stage1Start { get; set; }

        public string Stage1End { get; set; }

        public string Stage2Name { get; set; }

        public string Stage2Start { get; set; }

        public string Stage2End { get; set; }
    }


    public class AssessmentStageCreateModel
    {
        public Guid AssessmentId { get; set; }

        public Guid StageId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Start { get; set; }

        public string End { get; set; }

        public string Publish { get; set; }

        public bool IsAvailable { get; set; }
    }

    public class AddCommitteeMemberModel
    {
        public Assessment Assessment { get; set; }

        public Guid OrganiserId2 { get; set; }
    }

    public class AssessmenSummaryModel
    {
        public Curriculum Curriculum { get; set; }

        public ActivityOrganiser Organiser { get; set; }

        public Assessment Assessment { get; set; }

        public List<AssessmentCandidateViewModel> Candidates { get; set; }
    }


    public class AssessmentCandidateViewModel
    {
        public Candidature Candidature { get; set; }

        public ApplicationUser User { get; set; }

        public ActivityDate Date { get; set; }

    }
}

