using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyStik.TimeTable.Web.Api.DTOs
{
    /// <summary>
    /// 
    /// </summary>
    public class CourseDto : NamedDto
    {
        /// <summary>
        /// 
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool IsCoterie { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool HasHomeBias { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<CorrelationDto> Correlations { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<CourseDateDto> Dates { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class CorrelationDto : BaseDto
    {
        /// <summary>
        /// 
        /// </summary>
        public string Organiser { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Curriculum { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class CourseDateDto : BaseDto
    {
        /// <summary>
        /// 
        /// </summary>
        public string Begin { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string End { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime From { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime Until { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool IsCanceled { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<RoomDto> Rooms { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<LecturerDto> Lecturer { get; set; }

    }

    /// <summary>
    /// 
    /// </summary>
    public class RoomDto : BaseDto
    {
        /// <summary>
        /// 
        /// </summary>
        public string Number { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Building { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Campus { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class LecturerDto : BaseDto
    {
        /// <summary>
        /// 
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Faculty { get; set; }
    }


    /// <summary>
    /// 
    /// </summary>
    public class CourseSummaryDto : NamedDto
    {
        /// <summary>
        /// 
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool IsCoterie { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <summary>
        /// 
        /// </summary>
        public bool HasHomeBias { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public OrganiserDto Department { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Level { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public double Ects { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public double Sws { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public double UsCredits { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<RoomDto> Locations { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<LecturerDto> Lecturer { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<AppointmentDto> Appointments { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<ModuleDto> Modules { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<CourseDateDto> Dates { get; set; }

    }

    /// <summary>
    /// 
    /// </summary>
    public class AppointmentDto
    {
        /// <summary>
        /// 
        /// </summary>
        public string DayOfWeekName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string TimeBegin { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string TimeEnd { get; set; }
    }
}