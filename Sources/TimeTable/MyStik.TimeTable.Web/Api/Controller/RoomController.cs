using System;
using MyStik.TimeTable.Web.Api.Responses;
using MyStik.TimeTable.Web.Api.Services;

namespace MyStik.TimeTable.Web.Api.Controller
{


    /// <summary>
    /// 
    /// </summary>
    public class RoomController : ApiBaseController
    {


        /// <summary>
        /// Gibt alle freien Räume eines Tages ab dem Abfragezeitpunkt wieder,die mindestens 30min frei sind
        /// </summary>
        /// <returns>Liste der freien Räume</returns>
        public FreeRoomResponse GetFreeRoomsNow(string buildingId="R")
        {
            //Initialisieren des RoomService, damit dieser genutzt werden kann
            var roomService = new RoomInfoService();
            //Abfrage der freien Räume durch den RoomInfoService und Zwischenspeicherung desen Responeses als Liste roomList
            var code = string.IsNullOrEmpty(buildingId) ? "R" : buildingId;
            var roomList = roomService.GetFreeRoomsNow(code);
            //Erstellen eines "Response" mit Hilfe der roomList
            var response = new FreeRoomResponse
            {
                Rooms = roomList,
            };
            //Rückgabe des Response
            return response;
        }

        /// <summary>
        /// Abfrage aller Räume die in einen festgelegeten Zeitraum frei sind,d.h. sie sollen von "from" bis "until" frei sein!
        /// </summary>
        /// <param name="FromDay">Startdatum des Zeitraums im Format dd.MM.yyyy</param>
        /// <param name="UntilDay">Enddatum des Zeitraums im Format dd.MM.yyyy</param>
        /// <param name="FromTime">Startzeitpunkt am Startdatum des Zeitraums im Format hh:mm</param>
        /// <param name="UntilTime">Endzeitpunkt am Enddatum im Format hh:mm</param>
        /// <returns>Liste aller Räume, die den gewünschten Zeitraum frei sind</returns>
        public FreeRoomResponse GetFreeRooms(string FromDay, string UntilDay, string FromTime, string UntilTime)
        {

            var from = DateTime.Parse(FromDay).Add(TimeSpan.Parse(FromTime));

            var until = DateTime.Parse(UntilDay).Add(TimeSpan.Parse(UntilTime));
            
            var roomService = new RoomInfoService();

            var roomList = roomService.GetFreeRooms(from, until);

            var response = new FreeRoomResponse
            {
                Rooms = roomList,
            };

            return response;
        }

        /// <summary>
        /// Abfrage aller freien Räume (min X-min(span)frei) für eine Woche(ein tag der Woche dann mo-fr freie räume mit min 30min)
        /// </summary>
        /// <param name="Span">Die Zeitspanne, die der Raum mindestens frei sein soll</param>
        /// <param name="OneDateOfWeek">irgendein Tag der gewünschten Woche</param>
        /// <returns>Liste der Räume für alle Tage einer Woche, die den Zeitraum frei sind</returns>
        public FreeRoomTimespansResponese GetFreeRoomsSpanOfWeek(string Span, string OneDateOfWeek)
        {
            var span = TimeSpan.Parse(Span);
            var DateOfWeek = DateTime.Parse(OneDateOfWeek);

            var roomService = new RoomInfoService();

            var roomList = roomService.GetFreeRoomTimesOfWeek(span, DateOfWeek);

            var response = new FreeRoomTimespansResponese
            {
                FreeRoomSpans = roomList,
            };

            return response;
        }


        /// <summary>
        /// Abfrage aller Räume die an einem gewählten Tag eine gewählte TimeSpan frei sind
        /// </summary>
        /// <param name="Span">Die Zeitspanne, die der Raum mindestens frei sein soll</param>
        /// <param name="Day">Der gewünschte Tag im Format dd.MM.yyyy</param>
        /// <returns>Liste der Räume, die am übergeben Tag mindestens Span frei sind</returns>
        public FreeRoomTimespansResponese GetFreeRoomsDaySpan (string Span, string Day)
        {
            var span=TimeSpan.Parse(Span);
            var day=DateTime.Parse(Day);

            var roomService = new RoomInfoService();

            var roomList = roomService.GetFreeRoomTimespan(span, day);

            var response = new FreeRoomTimespansResponese
            {
                FreeRoomSpans = roomList,
            };

            return response;
        }

        /// <summary>
        /// Abfrage aller verfügbaren Räume
        /// </summary>
        /// <returns>Liste aller Räume</returns>
        public AllRoomResponse GetAllRooms()
        {
            var roomService = new RoomInfoService();

            var roomList = roomService.GetAllRooms();

            var response = new AllRoomResponse
            {
                Rooms = roomList,
            };

            return response;
        }

        /// <summary>
        /// Abfrage aller Räume die mit einem "StartsWith"-string beginnen
        /// </summary>
        /// <param name="StartsWith">String mit dem der Raum beginnen soll, d.h. z.B. R für alle R1.021 usw.</param>
        /// <returns>Liste aller Räume die mit dem übergebenen String anfangen</returns>
        public AllRoomResponse GetRoomsStartWith(string StartsWith)
        {
            var roomService = new RoomInfoService();

            var roomList = roomService.GetRoomsStartWith(StartsWith);

            var response = new AllRoomResponse
            {
                Rooms = roomList,
            };

            return response;
        }

        /// <summary>
        /// Abfrage aller Termine, die in einem Raum stattfinden
        /// </summary>
        /// <param name="RoomId">Id des Raums</param>
        /// <returns>Liste aller Termine, die im gewählten Raum stattfinden</returns>
        public RoomDateResponse GetRoomActivities(string RoomId)
        {
            var roomService = new RoomInfoService();

            var roomDateList = roomService.GetRoomDates(RoomId);

            var response = new RoomDateResponse
            {
                RoomDates = roomDateList,
            };

            return response;
        }
    }


    //evtl. spätere Erweiterungen

    //alle Räume einer Fakultät
    //    public IEnumerable<RoomInfo> GetAllFacRooms(Guid FacID)
    //    {
    //        return null;
    //    }


    //alle freien Räume einer Fakultät in Zeitraum
    //    public IEnumerable<FreeRoomInfo> GetAllFreeFacRooms(Guid FacID, DateTime Start, DateTime End)
    //    {
    //        return null;
    //    }


}

