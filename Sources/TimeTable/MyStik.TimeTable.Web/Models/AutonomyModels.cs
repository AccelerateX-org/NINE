using System;
using MyStik.TimeTable.Data;

namespace MyStik.TimeTable.Web.Models
{
    public class OrgAutonomyModel
    {
        public ActivityOrganiser Organiser { get; set; }

        public Autonomy Autonomy { get; set; }
    }

    public class CommitteeCreateModel
    {
        public string Name { get; set; }


        public string Description { get; set; }


        public Guid CurriculumId { get; set; }

        public Guid CommitteeId { get; set; }
    }
}