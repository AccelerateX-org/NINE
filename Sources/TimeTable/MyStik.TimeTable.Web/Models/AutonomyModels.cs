using System;
using System.Collections.Generic;
using MyStik.TimeTable.Data;

namespace MyStik.TimeTable.Web.Models
{
    public class OrgAutonomyModel
    {
        public ActivityOrganiser Organiser { get; set; }

        public List<Committee> Committees { get; set; }
    }

    public class CommitteeCreateModel
    {
        public string Name { get; set; }


        public string Description { get; set; }


        public Guid CurriculumId { get; set; }

        public Guid CommitteeId { get; set; }
    }
}