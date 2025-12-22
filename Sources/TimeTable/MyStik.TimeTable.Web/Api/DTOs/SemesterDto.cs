using System;

namespace MyStik.TimeTable.Web.Api.DTOs
{
    /// <summary>
    /// 
    /// </summary>
    public class SemesterDto
    {
        /// <summary>
        /// 
        /// </summary>
        public string Semester_Id { get; set; }

        public DateTime Begin { get; set; }

        public DateTime End { get; set; }
    }


    /// <summary>
    /// 
    /// </summary>
    public class SemesterStatisticsDto
    {
        public string Name { get; set; }

        public DateTime Begin { get; set; }

        public DateTime End { get; set; }
    }
}