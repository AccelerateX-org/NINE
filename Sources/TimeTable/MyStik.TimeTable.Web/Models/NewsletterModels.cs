using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Ajax.Utilities;
using MyStik.TimeTable.Data;

namespace MyStik.TimeTable.Web.Models
{
    public class NewsletterViewModel
    {
        public Newsletter Newsletter { get; set; }

        public OccurrenceStateModel State { get; set; }

        public bool IsAdmin { get; set; }

        public bool IsMember { get; set; }
    }

    public class NewsletterTestModel
    {
        public int UserCount { get; set; }

        public Guid NewsletterId { get; set; }
    }

    public class NewsletterCharacteristicModel
    {
        public NewsletterCharacteristicModel()
        {
            Member = new List<CourseMemberModel>();
        }

        public Newsletter Newsletter { get; set; }

        public List<CourseMemberModel> Member { get; set; }
    }
}