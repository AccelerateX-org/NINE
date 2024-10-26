using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using MyStik.TimeTable.Data;
using MyStik.TimeTable.DataServices;
using MyStik.TimeTable.DataServices.IO.GpUntis.Data;
using MyStik.TimeTable.Web.Api.DTOs;
using MyStik.TimeTable.Web.Services;
using NodaTime;

namespace MyStik.TimeTable.Web.Api.Controller
{
    public class RoomSearchRequest
    {
        public string number { get; set; }

        public List<OrganiserRequest> organisers { get; set; }

        public RoomStateRequest state { get; set; }
    }

    public class OrganiserRequest
    {
        public string name { get; set; }
    }

    public class RoomStateRequest
    {
        public DateTime? from { get; set; }

        public DateTime? until { get; set; }

        public bool? isAvailable { get; set; }
    }


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
        //[System.Web.Http.HttpGet]
        public IQueryable<RoomSummaryDto> GetRooms()
        {
            var result = new List<RoomSummaryDto>();

            var rooms = Db.Rooms.ToList();

            foreach (var room in rooms)
            {
                var r = new RoomSummaryDto
                {
                    Id = room.Id,
                    Number = room.Number, 
                    Name = room.Name,
                    Description = room.Description,
                    Capactiy = room.Capacity,
                    Assignees = new List<OrganiserDto>()
                };

                foreach (var roomAssignment in room.Assignments)
                {
                    r.Assignees.Add(new OrganiserDto
                    {
                        Id = roomAssignment.Organiser.Id,
                        Name = roomAssignment.Organiser.Name,
                        ShortName = roomAssignment.Organiser.ShortName,
                        Color = roomAssignment.Organiser.HtmlColor
                    });
                }

                result.Add(r);
            }

            return result.AsQueryable();
        }

        [System.Web.Http.Route("search")]
        //[System.Web.Http.HttpGet]
        public IQueryable<RoomSummaryDto> GetRoomsByNumber(string searchPattern)
        {
            var result = new List<RoomSummaryDto>();

            var rooms = Db.Rooms.Where(x => x.Number.ToLower().StartsWith(searchPattern.ToLower())).ToList();

            foreach (var room in rooms)
            {
                var r = new RoomSummaryDto
                {
                    Id = room.Id,
                    Number = room.Number,
                    Name = room.Name,
                    Description = room.Description,
                    Capactiy = room.Capacity,
                    Assignees = new List<OrganiserDto>()
                };

                foreach (var roomAssignment in room.Assignments)
                {
                    r.Assignees.Add(new OrganiserDto
                    {
                        Id = roomAssignment.Organiser.Id,
                        Name = roomAssignment.Organiser.Name,
                        ShortName = roomAssignment.Organiser.ShortName,
                        Color = roomAssignment.Organiser.HtmlColor
                    });
                }

                result.Add(r);
            }

            return result.AsQueryable();
        }



        /*
        /// <summary>
        /// Suche nach Räumen
        /// </summary>
        /// <param name="number">Nummer des Raumes</param>
        /// <returns></returns>
        [System.Web.Http.Route("search")]
        [System.Web.Mvc.HttpPost]
        public IQueryable<OccupancyDto> Search([FromBody] RoomSearchRequest request)
        {
            // ohne state => alle suchen
            var from = DateTime.Now;
            var until = from.AddMinutes(30);
            bool? isAvailable = null;

            if (request.state != null)
            {
                isAvailable = request.state.isAvailable;
                if (request.state.from != null)
                    from = request.state.from.Value;
                if (request.state.until != null)
                    until = request.state.until.Value;
            }




            var rooms = Db.Rooms.Where(r => r.Number.StartsWith(request.number)) .ToList();

            var result = new List<OccupancyDto>();

            foreach (var room in rooms)
            {
                var occDto = new OccupancyDto();
                occDto.Room.Number = room.Number;
                occDto.Room.Name = room.Name;
                occDto.Room.Description = room.Description;
                if (room.Number.StartsWith("K") || room.Number.StartsWith("L"))
                {
                    occDto.Room.Campus = "Pasing";
                }
                else if (room.Number.StartsWith("F"))
                {
                    occDto.Room.Campus = "Karlstrasse";
                }
                else
                {
                    occDto.Room.Campus = "Lothstrasse";
                }


                // aktuelle Belegung
                var currentDates = room.Dates
                    .Where(d =>
                            (d.End > @from && d.End <= until) || // Veranstaltung endet im Zeitraum
                            (d.Begin >= @from && d.Begin < until) || // Veranstaltung beginnt im Zeitraum
                            (d.Begin <= @from && d.End >= until) // Veranstaltung zieht sich über gesamten Zeitraum
                    )
                    .OrderBy(x => x.Begin)
                    .ToList();

                bool include = true;
                if (isAvailable.HasValue)
                {
                    if ((currentDates.Any() && isAvailable.Value) || // es werden freie gesucht, aber sind Belegungen vorhanden
                        (!currentDates.Any() && !isAvailable.Value)
                        )   
                    {
                        include = false;
                    }
                }

                if (include)
                {
                    // aktuelle Belegung aufbauen
                    foreach (var currentDate in currentDates)
                    {
                        var resDto = new ReservationDto
                        {
                            Id = currentDate.Id,
                            Begin = currentDate.Begin,
                            End = currentDate.End,
                            Name = currentDate.Activity.Name,
                            ShortName = currentDate.Activity.ShortName
                        };


                        occDto.Current.Add(resDto);
                    }

                    // nächste Belegung
                    var nextDate = room.Dates.OrderBy(x => x.Begin).FirstOrDefault(x => x.Begin > from);
                    if (nextDate != null)
                    {
                        var resDto = new ReservationDto
                        {
                            Id = nextDate.Id,
                            Begin = nextDate.Begin,
                            End = nextDate.End,
                            Name = nextDate.Activity.Name,
                            ShortName = nextDate.Activity.ShortName
                        };


                        occDto.Next.Add(resDto);
                    }

                    result.Add(occDto);
                }
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
        */

        /// <summary>
        /// Abfrage der Belegungen eines Raums
        /// </summary>
        /// <param name="number">Raumnummer</param>
        /// <param name="start">Beginn des Zeitraums. Format "dd-MM-yyyy"</param>
        /// <param name="end">Ende des Zeitraums. Format "dd-MM-yyyy"</param>
        /// <returns></returns>
        [System.Web.Http.Route("{number}/allocation/{start}/{end}")]
        public RoomAllocationDto GetRoomAllocation(string number, string start, string end)
        {
            var semester = new SemesterService().GetSemester(DateTime.Today);

            var startDate = string.IsNullOrEmpty(start) ? semester != null ? semester.StartCourses : DateTime.Today : DateTime.ParseExact(start, "yyyy-MM-dd", null);
            var endDate = string.IsNullOrEmpty(start) ? semester != null ? semester.EndCourses : DateTime.Today : DateTime.ParseExact(end, "yyyy-MM-dd", null);


            var result = new RoomAllocationDto
            {
                Number = number,
                Events = new List<RoomEventDto>(),
                From = startDate,
                Until = endDate.AddDays(1)
            };


            var room = Db.Rooms.FirstOrDefault(x => x.Number.ToLower().Equals(number.ToLower()));

            if (room != null)
            {
                var dates = room.Dates.Where(x => x.Begin >= result.From && x.End <= result.Until).OrderBy(x => x.Begin);

                foreach (var date in dates)
                {
                    var dto = new RoomEventDto
                    {
                        Title = date.Activity.Name,
                        Begin = date.Begin,
                        End = date.End
                    };

                    result.Events.Add(dto);
                }
            }

            return result;
        }


        /* so nicht
        [System.Web.Http.Route("famos/state")]
        //[System.Web.Http.HttpGet]
        public HttpResponseMessage GetFamosState(string searchPattern)
        {
            var now = DateTime.Now;

            var semester = new SemesterService(Db).GetSemester(now);

            var rooms = string.IsNullOrEmpty(searchPattern) ?
                Db.Rooms.ToList() :
                Db.Rooms.Where(x => x.Number.ToLower().StartsWith(searchPattern.ToLower())).ToList();

            var roomsWithDate = rooms.Where(x => x.Dates.Any(d => d.Begin <= now && now <= d.End)).ToList();

            var ms = new MemoryStream();
            var writer = new StreamWriter(ms, Encoding.Default);

            writer.Write(
                "ID;Semester;Vorlesungstitel;Langtitel;Bemerkung;Dozent;Fakultaet;Start;Ende;Serientyp;Intervall;Serienendtyp;Wiederholung;Teilnehmer;Raum");


            writer.Write(Environment.NewLine);

            foreach (var room in roomsWithDate)
            {
                var dates = room.Dates.Where(d => d.Begin <= now && now <= d.End).ToList();

                foreach (var date in dates)
                {
                    var id = room.Id.ToString();
                    var sem = semester.Name;
                    var comment = string.Empty;
                    var org = date.Activity.Organiser != null ? date.Activity.Organiser.ShortName : string.Empty;

                    var lec = date.Hosts.FirstOrDefault() != null ? date.Hosts.FirstOrDefault()?.Name : string.Empty;

                    var type = 2;
                    var interval = 1;
                    var typeEnd = 2;
                    var frequency = 1;
                    var part = date.Occurrence != null ? date.Occurrence.Subscriptions.Count : 0;



                    writer.Write("{0};{1};{2};{3};{4};{5};{6};{7};{8};{9};{10};{11};{12};{13};{14}",
                        id, sem, 
                        date.Activity.Name, date.Activity.Name, comment, lec, org,
                        date.Begin.ToString("dd.MM.yyyy HH:mm"), date.End.ToString("dd.MM.yyyy HH:mm"),
                        type, interval, typeEnd, frequency, part,
                        room.Number
                        );
                    writer.Write(Environment.NewLine);
                }

            }

            writer.Flush();
            writer.Dispose();

            //ms.Position = 0;

            var sb = new StringBuilder();
            sb.Append("Raumbelegung");
            sb.Append("_");
            sb.Append(now.ToString("yyyyMMdd"));
            sb.Append(".csv");

            

            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new StreamContent(ms);
            response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/csv");
            response.Content.Headers.ContentDisposition.FileName = sb.ToString();

            return response;

        }
        */




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
