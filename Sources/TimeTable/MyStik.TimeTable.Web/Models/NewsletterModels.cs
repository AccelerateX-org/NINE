using System;
using System.Collections.Generic;
using MyStik.TimeTable.Data;

namespace MyStik.TimeTable.Web.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class NewsletterViewModel
    {
        /// <summary>
        /// 
        /// </summary>
        public Newsletter Newsletter { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public OccurrenceStateModel State { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool IsAdmin { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool IsMember { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class NewsletterTestModel
    {
        /// <summary>
        /// 
        /// </summary>
        public int UserCount { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Guid NewsletterId { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class NewsletterCharacteristicModel
    {
        /// <summary>
        /// 
        /// </summary>
        public NewsletterCharacteristicModel()
        {
            Member = new List<CourseMemberModel>();
        }

        /// <summary>
        /// 
        /// </summary>
        public Newsletter Newsletter { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<CourseMemberModel> Member { get; set; }
    }
}