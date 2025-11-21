using MyStik.TimeTable.Web.Controllers;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;

namespace MyStik.TimeTable.Web.Api.Contracts
{
    public class CourseCohorte
    {
        public string institution { get; set; }
        public string organiser { get; set; }
        public string curriculum { get; set; }
        public string label { get; set; }
    }

    public class CourseQuota
    {
        public int amount { get; set; }

        public List<CourseCohorte> fractions { get; set; }
    }

    public class CourseDateApiModel
    {
        public Guid id { get; set; }

        public DateTime begin { get; set; }

        public DateTime end { get; set; }

        public string title { get; set; }

        public string description { get; set; }

        public bool isCanceled { get; set; }

        public List<string> rooms { get; set; }
        
        public List<string> hosts { get; set; }
    }

    public class CourseSubscriberApiModel
    {
        public string id { get; set; }

        public DateTime subscribed { get; set; }
    }


    public class CourseApiModel
    {
        public Guid id { get; set; }

        public string externalId { get; set; }

        /// <summary>
        /// FK 09
        /// </summary>
        public string organiser { get; set; }

        /// <summary>
        /// SoSe 2026
        /// </summary>
        public string semester { get; set; }

        /// <summary>
        /// no identifier, allows duplicates
        /// </summary>
        public string code { get; set; }

        public string title { get; set; }

        public string description { get; set; }

        public List<CourseCohorte> cohortes { get; set; }

        public List<CourseQuota> quotas { get; set; }

        public List<CourseDateApiModel> dates { get; set; }

        public List<CourseSubscriberApiModel> subscribers { get; set; }
    }

    public class CourseApiResponseModel
    {
        public Guid id { get; set; }
        public string externalId { get; set; }
        public string message { get; set; }
    }
}
