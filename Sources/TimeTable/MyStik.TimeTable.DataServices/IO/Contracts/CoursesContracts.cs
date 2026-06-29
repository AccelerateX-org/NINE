using System;
using System.Collections.Generic;

namespace MyStik.TimeTable.DataServices.IO.Contracts
{
    #region Course
    public class CourseApiContract
    {
        public string CourseId { get; set; }

        public string ExternalId { get; set; }

        /// <summary>
        /// FK 09
        /// </summary>
        public string InstitutionId { get; set; }

        /// <summary>
        /// FK 09
        /// </summary>
        public string OrganiserId { get; set; }

        /// <summary>
        /// SoSe 2026
        /// </summary>
        public string SemesterId { get; set; }

        /// <summary>
        /// no identifier, allows duplicates
        /// </summary>
        public string Code { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public List<CourseApiCohortContract> Cohorts { get; set; }

        public List<CourseApiQuotaContract> Quotas { get; set; }

        public List<CourseApiSequenceContract> Sequences { get; set; }

        public List<CourseApiTeachingContract> Teachings { get; set; }
        public List<CourseApiDateContract> Dates { get; set; }

        public List<CourseApiSubscriptionContract> Subscriptions { get; set; }
    }



    public class CourseApiCohortContract
    {
        public string InstitutionId { get; set; }
        public string OrganiserId { get; set; }
        public string CurriculumId { get; set; }

        /// <summary>
        /// format: 2026-10-01
        /// </summary>
        public DateTime? CurriculumDate { get; set; }

        public string CurriculumAlias { get; set; }
        
        public string Label { get; set; }

        public override string ToString()
        {
            if (string.IsNullOrEmpty(CurriculumAlias))
            {
                return $"{InstitutionId}|{OrganiserId}|{CurriculumId}|{CurriculumDate}|{Label}";
            }
            return $"{InstitutionId}|{OrganiserId}|{CurriculumAlias}|{Label}";
        }
    }

    public class CourseApiQuotaContract
    {
        public string Title { get; set; }

        public int Capacity { get; set; }

        public List<CourseApiCohortContract> Cohorts { get; set; }
    }

    public class CourseApiTeachingContract
    {
        public string InstitutionId { get; set; }
        public string OrganiserId { get; set; }
        public string CatalogId { get; set; }
        public string ModuleId { get; set; }
        public string SubjectId { get; set; }
    }

    public class CourseApiSequenceContract
    {
        public DateTime FirstBegin { get; set; }

        public DateTime LastEnd { get; set; }

        /// <summary>
        /// Alle sieben Tage oder 14 Tage oder töglich (Block)
        /// </summary>
        public int Frequency { get; set; }

        /// <summary>
        /// Vorlesung oder Übung - bekommt jedes Date
        /// </summary>
        public string Title { get; set; }

        public List<string> RoomIds { get; set; }

        public List<string> LecturerIds { get; set; }
    }

    // Als Teil des Kurses
    public class CourseApiSubscriptionContract
    {
        public string UserId { get; set; }

        public string MatriculationNumber { get; set; }

        public DateTime SubscriptionDate { get; set; }

        public bool OnWaitingList { get; set; }
    }

    public class CourseApiDateContract
    {
        public Guid Id { get; set; }

        public DateTime Begin { get; set; }

        public DateTime End { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public List<string> Rooms { get; set; }

        public List<string> Hosts { get; set; }
    }

    #endregion





    public class CourseApiResponseModel
    {
        public Guid CourseId { get; set; }
        public string Message { get; set; }

        public List<string> Warnings { get; set; }
    }

    public class CourseDateApiResponseModel
    {
        public Guid CourseId { get; set; }
        public Guid DateId { get; set; }

        public string Message { get; set; }
    }



    public class CourseSubscriptionCreateApiModel
    {
        public string UserId { get; set; }

        public string MatriculationNumber { get; set; }
    }

    // Antwort
    public class CourseSubscriptionApiModel
    {
        public Guid CourseId { get; set; }

        public string UserId { get; set; }

        public string MatriculationNumber { get; set; }

        public DateTime SubscriptionDate { get; set; }
    }



    public class CourseDateApiModel
    {
        public Guid Id { get; set; }

        public DateTime Begin { get; set; }

        public DateTime End { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public bool IsCanceled { get; set; }

        public List<string> Rooms { get; set; }

        public List<string> Hosts { get; set; }
    }

}
