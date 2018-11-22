using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyStik.TimeTable.Web.Models
{
    public class TeachingModuleCreateModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        [AllowHtml]
        public string Description { get; set; }
    }
}