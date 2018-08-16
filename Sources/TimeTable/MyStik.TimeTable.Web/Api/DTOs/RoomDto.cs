using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

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


}