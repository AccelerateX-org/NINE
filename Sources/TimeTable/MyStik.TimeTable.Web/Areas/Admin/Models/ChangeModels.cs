using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyStik.TimeTable.Web.Areas.Admin.Models
{
    public class ChangeSummaryModel
    {
        public DateTime First { get; set; }
        public DateTime Last { get; set; }

        public int Count { get; set; }
    }
}