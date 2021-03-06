﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using MyStik.TimeTable.Web.Api.DTOs;
using MyStik.TimeTable.Web.Services;
using RazorEngine.Compilation.ImpromptuInterface;

namespace MyStik.TimeTable.Web.Api.Controller
{
    public class OccupancyRequestModel
    {
        public bool IsAvailable { get; set; }

        public string OrgName { get; set; }
    }

    [System.Web.Http.RoutePrefix("api/v2/occupancies")]
    public class OccupanciesController : ApiBaseController
    {
        /// <summary>
        /// 
        /// </summary>
        [System.Web.Http.Route("org")]
        [HttpPost]
        public IQueryable<OccupancyDto> ByOrg([FromBody]OccupancyRequestModel model)
        {
            var list = new List<OccupancyDto>();

            var from = DateTime.Now;
            var until = from.AddMinutes(1);

            var org = Db.Organisers.SingleOrDefault(x => x.ShortName.Equals(model.OrgName));
            if (org == null)
                return list.AsQueryable();

            //Initialisierung des RoomService von NINE
            var roomService = new RoomService();

            //Abfrage der freien Räume durch den RoomInfoService und Zwischenspeicherung desen Responeses als Liste roomList
            var roomList = roomService.GetFreeRooms(org.Id, false, from, until);


            foreach (var room in roomList)
            {
                var occDto = new OccupancyDto();
                occDto.Room.Number = room.Number;
                occDto.Room.Description = room.Description;

                // nächste Belegung
                var nextDate = room.Dates.OrderBy(x => x.Begin).FirstOrDefault(x => x.Begin > from);
                if (nextDate != null)
                {
                    var resDto = new ReservationDto
                    {
                        Begin = nextDate.Begin,
                        End = nextDate.End,
                        Name = nextDate.Activity.Name,
                        ShortName = nextDate.Activity.ShortName
                    };


                    occDto.Next.Add(resDto);
                }

                list.Add(occDto);
            }

            return list.AsQueryable();
        }
    }
}
