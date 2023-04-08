using System;

namespace MyStik.TimeTable.Web.Api.DTOs
{
    /// <summary>
    /// 
    /// </summary>
    public class SubscriptionDto
    {
        /// <summary>
        /// 
        /// </summary>
        public Guid CourseId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool IsValid { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool OnWaitingList { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Message { get; set; }

        public string Title { get; set; }
    }
}