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
            Previous = new List<ReservationDto>();
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
        
        public List<ReservationDto> Previous { get; set; }
    }

    public class FreeRoomDto
    {
        public string RoomNumber { get; set; }

        public string RoomName { get; set; }

        public int Capacity { get; set; }

        public DateTime FreeFrom { get; set; }

        public DateTime FreeTo { get; set; }

        public string NextEventName { get; set; }
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