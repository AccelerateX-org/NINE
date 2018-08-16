using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using MyStik.TimeTable.Web.Api.DTOs;

namespace MyStik.TimeTable.Web.Api.Controller
{
    /// <summary>
    /// Anfrage von Rauminformationen
    /// </summary>
    [System.Web.Http.RoutePrefix("api/v2/rooms")]
    public class RoomsController : ApiBaseController
    {
        /// <summary>
        /// 
        /// </summary>
        [System.Web.Http.Route("")]
        public IQueryable<RoomSummaryDto> GetRooms()
        {
            var rooms = Db.Rooms.ToList();

            var result = new List<RoomSummaryDto>();

            foreach (var room in rooms)
            {
                var r = new RoomSummaryDto();

                r.Number = room.Number;
                r.Name = room.Name;

                result.Add(r);
            }

            return result.AsQueryable();
        }

        /// <summary>
        /// 
        /// </summary>

        [System.Web.Http.Route("{number}/label")]
        public IQueryable<RoomLabelDto> GetRoomLabel(string number, string mode)
        {
            var result = new List<RoomLabelDto>();

            var label = new RoomLabelDto();

            var room = Db.Rooms.FirstOrDefault(x => x.Number.ToLower().Equals(number.ToLower()));

            if (room != null)
            {
                label.Title = $"Belegungsplan für Raum {number}";

                MemoryStream ms = null;
                HttpContext context = HttpContext.Current;

                ms = CopyFileToMemory(context.Server.MapPath("/content/images/room_ok.png"));

                label.ImageData = ms.ToArray();

            }
            else
            {
                label.Title = $"Raum {number} nicht gefunden";

                MemoryStream ms = null;
                HttpContext context = HttpContext.Current;

                ms = CopyFileToMemory(context.Server.MapPath("/content/images/room_error.png"));

                label.ImageData = ms.ToArray();
            }


            result.Add(label);

            return result.AsQueryable();
        }


        /// <summary>
        /// 
        /// </summary>
        [System.Web.Http.Route("{number}/display")]
        public FileResult GetRoomDisplay(string number, string mode)
        {
            var result = new List<RoomLabelDto>();

            var label = new RoomLabelDto();

            var room = Db.Rooms.FirstOrDefault(x => x.Number.ToLower().Equals(number.ToLower()));

            if (room != null)
            {
                label.Title = $"Belegungsplan für Raum {number}";

                MemoryStream ms = null;
                HttpContext context = HttpContext.Current;

                ms = CopyFileToMemory(context.Server.MapPath("/content/images/room_ok.png"));

                label.ImageData = ms.ToArray();

            }
            else
            {
                label.Title = $"Raum {number} nicht gefunden";

                MemoryStream ms = null;
                HttpContext context = HttpContext.Current;

                ms = CopyFileToMemory(context.Server.MapPath("/content/images/room_error.png"));

                label.ImageData = ms.ToArray();
            }

            return new FileContentResult(label.ImageData, "image/png");
        }


        /// <summary>
        /// 
        /// </summary>
        [System.Web.Http.Route("{number}/image")]
        public HttpResponseMessage GetRoomImage(string number, string mode)
        {
            var result = new List<RoomLabelDto>();

            var label = new RoomLabelDto();

            var room = Db.Rooms.FirstOrDefault(x => x.Number.ToLower().Equals(number.ToLower()));

            MemoryStream ms = null;

            if (room != null)
            {
                label.Title = $"Belegungsplan für Raum {number}";

                HttpContext context = HttpContext.Current;

                ms = CopyFileToMemory(context.Server.MapPath("/content/images/room_ok.png"));

                label.ImageData = ms.ToArray();

            }
            else
            {
                label.Title = $"Raum {number} nicht gefunden";

                HttpContext context = HttpContext.Current;

                ms = CopyFileToMemory(context.Server.MapPath("/content/images/room_error.png"));

                label.ImageData = ms.ToArray();
            }

            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new StreamContent(ms);
            response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/png");

            return response;
        }



        private MemoryStream CopyFileToMemory(string path)
        {
            MemoryStream ms = new MemoryStream();
            FileStream fs = new FileStream(path, FileMode.Open);
            fs.Position = 0;
            fs.CopyTo(ms);
            fs.Close();
            fs.Dispose();
            return ms;
        }

    }
}
