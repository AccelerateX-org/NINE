using System.Collections.Generic;
using MyStik.TimeTable.Web.Api.Contracts;

namespace MyStik.TimeTable.Web.Api.Responses
{
    /// <summary>
    /// Respone zur Abfrage der freien Räume
    /// </summary>
    public class FreeRoomResponse
    {
        /// <summary>
        /// Liste der freien Räume, siehe FreeRoomContract
        /// </summary>
        public IEnumerable<FreeRoomContract> Rooms { get; set; }

    }
    /// <summary>
    /// Response zur Abfrage aller Räume
    /// </summary>
    public class AllRoomResponse
    {
        /// <summary>
        /// Liste aller Räume, siehe AllRoomContract
        /// </summary>
        public IEnumerable<AllRoomContract> Rooms { get; set; }
    }
    /// <summary>
    /// Response zur Abfrage der Termine eines Raums
    /// </summary>
    public class RoomDateResponse
    {
        /// <summary>
        /// Liste der Termine eines Raums, siehe RoomDateContract
        /// </summary>
        public IEnumerable<RoomDateContract> RoomDates { get; set; }
    }
    /// <summary>
    /// Response zur Abfrage der freien Räume mit Zeitraum
    /// </summary>
    public class FreeRoomTimespansResponese
    {
        /// <summary>
        /// Liste der zeitraumfreien Räume, siehe FreeRoomTimespansContract
        /// </summary>
        public IEnumerable<FreeRoomTimespansContract> FreeRoomSpans { get; set; }
    }
}
