using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MyStik.TimeTable.Data;

namespace MyStik.TimeTable.Web.Models
{
    public class MoveModuleModel
    {
        public ActivityOrganiser Organiser { get; set; }
        public CurriculumModuleCatalog Catalog { get; set; }

        public List<ActivityOrganiser> Organises { get; set; }

    }

    public class CatalogPlanModel
    {
        public Semester Semester { get; set; }

        public ActivityOrganiser Organiser { get; set; }

        public CurriculumModuleCatalog Catalog { get; set; }

        public Curriculum Curriculum { get; set; }
    }
}