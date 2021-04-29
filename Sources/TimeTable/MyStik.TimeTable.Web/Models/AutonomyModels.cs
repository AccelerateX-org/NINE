using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MyStik.TimeTable.Data;

namespace MyStik.TimeTable.Web.Models
{
    public class OrgAutonomyModel
    {
        public ActivityOrganiser Organiser { get; set; }

        public Autonomy Autonomy { get; set; }
    }
}