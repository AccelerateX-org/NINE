using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyStik.TimeTable.Web.Api.DTOs
{
    /// <summary>
    /// 
    /// </summary>
    public class ZpaCourseDto
    {
        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ShortName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Description { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public List<ZpaCourseDateDto> Dates { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class ZpaCourseDateDto
    {
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
        public List<ZpaRoomDto> Rooms { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<ZpaLecturerDto> Lecturer { get; set; }

    }

    /// <summary>
    /// 
    /// </summary>
    public class ZpaRoomDto
    {
        /// <summary>
        /// 
        /// </summary>
        public string Number { get; set; }

    }

    /// <summary>
    /// 
    /// </summary>
    public class ZpaLecturerDto
    {
        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ShortName { get; set; }

    }
}