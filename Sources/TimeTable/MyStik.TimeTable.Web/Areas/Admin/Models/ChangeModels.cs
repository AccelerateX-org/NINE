using System;

namespace MyStik.TimeTable.Web.Areas.Admin.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class ChangeSummaryModel
    {
        /// <summary>
        /// 
        /// </summary>
        public DateTime First { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime Last { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int Count { get; set; }
    }
}