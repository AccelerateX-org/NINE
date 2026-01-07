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
        public string institution_id { get; set; }
        public string organiser_id { get; set; }
        public string curriculum_id { get; set; }
        public string curriculum_alias { get; set; }
        public string label { get; set; }
    }

    public class CourseQuota
    {
        public int amount { get; set; }

        public List<CourseCohorte> cohortes { get; set; }
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

    public class CourseDateSequenceCreateApiModel
    {
        public DateTime first_begin { get; set; }

        public DateTime last_end { get; set; }

        /// <summary>
        /// Alle sieben Tage oder 14 Tage oder töglich (Block)
        /// </summary>
        public int frequency { get; set; }

        /// <summary>
        /// Vorlesung oder Übung - bekommt jedes Date
        /// </summary>
        public string title { get; set; }

        public List<string> room_ids { get; set; }

        public List<string> lecturer_ids { get; set; }
    }

    public class CourseDateCreateApiModel
    {
        public DateTime begin { get; set; }

        public DateTime end { get; set; }

        /// <summary>
        /// Vorlesung oder Übung - bekommt jedes Date
        /// </summary>
        public string title { get; set; }

        public string description { get; set; }

        public List<string> room_ids { get; set; }

        public List<string> lecturer_ids { get; set; }
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


    public class CourseCreateApiModel
    {
        public string external_id { get; set; }

        /// <summary>
        /// FK 09
        /// </summary>
        public string organiser_id { get; set; }

        /// <summary>
        /// SoSe 2026
        /// </summary>
        public string semester_id { get; set; }

        /// <summary>
        /// no identifier, allows duplicates
        /// </summary>
        public string code { get; set; }

        public string title { get; set; }

        public string description { get; set; }

        public List<CourseCohorte> cohortes { get; set; }

        public List<CourseQuota> quotas { get; set; }

        public List<CourseDateSequenceCreateApiModel> sequences { get; set; }
    }

    public class CourseApiResponseModel
    {
        public Guid course_id { get; set; }
        public string message { get; set; }
    }

    public class CourseDateApiResponseModel
    {
        public Guid course_id { get; set; }
        public Guid date_id { get; set; }

        public string message { get; set; }
    }

    // Als Teil des Kurses
    public class CourseSubscriberApiModel
    {
        public string user_id { get; set; }

        public string matriculation_number { get; set; }

        public DateTime subscrition_date { get; set; }

        public bool on_waiting_list { get; set; }
    }


    public class CourseSubscriptionCreateApiModel
    {
        public string user_id { get; set; }

        public string matriculation_number { get; set; }
    }

    // Antwort
    public class CourseSubscriptionApiModel
    {
        public Guid course_id { get; set; }

        public string user_id { get; set; }

        public string matriculation_number { get; set; }

        public DateTime subscription_date { get; set; }
    }

}
