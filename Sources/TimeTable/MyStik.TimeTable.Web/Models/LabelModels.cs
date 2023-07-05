using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MyStik.TimeTable.Data;

namespace MyStik.TimeTable.Web.Models
{
    public class ItemLabelEditModel
    {
        public Curriculum Curriculum { get; set; }

        public ItemLabel ItemLabel { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string HtmlColor { get; set; }
    }
}