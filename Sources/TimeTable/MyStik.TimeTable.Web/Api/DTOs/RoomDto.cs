using System;
using System.Collections.Generic;

namespace MyStik.TimeTable.Web.Api.DTOs
{
    /// <summary>
    /// 
    /// </summary>
    public class RoomSummaryDto : BaseDto
    {
        /// <summary>
        /// 
        /// </summary>
        public string Number { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; }
    }


    /// <summary>
    /// 
    /// </summary>
    public class RoomLabelDto : BaseDto
    {
        /// <summary>
        /// 
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Der Datentyp
        /// </summary>
        public string ImageFileType { get; set; }

        /// <summary>
        /// Die Bilddaten
        /// </summary>
        public byte[] ImageData { get; set; }
    }

    public class RoomAllocationDto
    {
        public string Number { get; set; } = string.Empty;
        public DateTime From { get; set; }
        public DateTime Until { get; set; }

        public List<RoomEventDto> Events { get; set; }
    }

    public class RoomEventDto
    {
        public string Title { get; set; }

        public DateTime Begin { get; set; }
        public DateTime End { get; set; }
    }
}