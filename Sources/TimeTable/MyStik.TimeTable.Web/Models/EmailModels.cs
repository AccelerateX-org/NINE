using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Postal;

namespace MyStik.TimeTable.Web.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class BaseEmail : Email
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="registration"></param>
        public BaseEmail(string registration) : base(registration)
        {
            
        }

        /// <summary>
        /// 
        /// </summary>
        public string From { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string To { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Subject { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class InactivityReportEmail : BaseEmail
    {
        /// <summary>
        /// 
        /// </summary>
        public InactivityReportEmail() : base("InactiveReport")
        {
            
        }

        /// <summary>
        /// 
        /// </summary>
        public int Count { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class CustomBodyEmail : BaseEmail
    {
        /// <summary>
        /// 
        /// </summary>
        public CustomBodyEmail() : base("CustomBodyMail")
        {

        }

        /// <summary>
        /// 
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool IsImportant { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool IsDistributionList { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string ListName { get; set; }
    }

}