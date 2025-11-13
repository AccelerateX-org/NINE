using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;

namespace MyStik.TimeTable.Web.Api.Contracts
{
    public class CourseSignature
    {
        /// <summary>
        /// HM
        /// </summary>
        public string institution { get; set; }

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
    }

    public class CourseCohorte
    {
        public string institution { get; set; }
        public string organiser { get; set; }
        public string curriculum { get; set; }
        public string label { get; set; }
    }


    public class CourseCreateRequest
    {
        public Guid id { get; set; }

        public string title { get; set; }

        public string externalId { get; set; }

        public string description { get; set; }

        public CourseSignature signature { get; set; }

        public List<CourseCohorte> cohortes { get; set; }
    }

    public class CourseCreateResponse
    {
    }

}
