using System;
using System.Collections.Generic;

namespace MyStik.TimeTable.Web.Api.DTOs
{
    /// <summary>
    /// 
    /// </summary>
    public class OccupancyDto : BaseDto
    {
        public OccupancyDto()
        {
            Room = new RoomDto();
            Current = new List<ReservationDto>();
            Next = new List<ReservationDto>();
        }

        /// <summary>
        /// 
        /// </summary>
        public RoomDto Room { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<ReservationDto> Current { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<ReservationDto> Next { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class ReservationDto : NamedDto
    {
        /// <summary>
        /// 
        /// </summary>
        public DateTime Begin { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime End { get; set; }
    }

}