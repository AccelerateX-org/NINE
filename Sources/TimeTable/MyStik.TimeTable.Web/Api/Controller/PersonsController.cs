using MyStik.TimeTable.Data;
using MyStik.TimeTable.DataServices;
using MyStik.TimeTable.Web.Api.Contracts;
using MyStik.TimeTable.Web.Api.DTOs;
using MyStik.TimeTable.Web.Api.Responses;
using MyStik.TimeTable.Web.Api.Services;
using MyStik.TimeTable.Web.Models;
using MyStik.TimeTable.Web.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;

namespace MyStik.TimeTable.Web.Api.Controller
{
    public class LecturerSearchRequest
    {
        public string Organiser { get; set; }

        public string Committee { get; set; }

        public string Name { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    /*
    [System.Web.Http.RoutePrefix("api/v2/persons")]
    public class PersonsController : ApiBaseController
    {
        [HttpGet]
        [System.Web.Http.Route("")]
        [ResponseType(typeof(List<LecturerDto>))]

        public IHttpActionResult GetPersons(string pattern = "")
        {
            var response = new List<LecturerDto>();
            return Ok(response);
        }
    }
    */
}
