using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MyStik.TimeTable.Data;

namespace MyStik.TimeTable.Web.Models
{
    public class SupervisionCreateModel
    {
        public Supervision Supervision { get; set; }

        public string Title { get; set; }

        [AllowHtml]
        public string Description { get; set; }
    }


    public class SupervisionOverviewModel
    {
        public SupervisionOverviewModel()
        {
            Supervisions = new Dictionary<OrganiserMember, List<Supervision>>();
        }

        public ActivityOrganiser Organiser { get; set; }

        public Dictionary<OrganiserMember, List<Supervision>> Supervisions { get; }

    }

    public class SupervisionRequestModel
    {
        public OrganiserMember Lecturer { get; set; }

        public OccurrenceSubscription Subscription { get; set; }

        public Supervision Supervision { get; set; }

        [AllowHtml]
        public string Description { get; set; }
    }


    public class SupervisionAcceptModel
    {
        public SupervisionAcceptModel()
        {
            AlternativeRequests = new List<SupervisionRequestModel>();
            Request = new SupervisionRequestModel();
        }

        public ApplicationUser User { get; set; }

        public Curriculum Curriculum { get; set; }

        public SupervisionRequestModel Request { get; set; }

        public List<SupervisionRequestModel> AlternativeRequests { get; set; }
    }

}