using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using MyStik.TimeTable.Data;
using MyStik.TimeTable.Web.Api.DTOs;
using MyStik.TimeTable.Web.Models;

namespace MyStik.TimeTable.Web.Api.Controller
{
    /// <summary>
    /// SuperHero
    /// </summary>
    [RoutePrefix("api/routingMatters")]
    public class SuperHeroController : ApiController
    {
        [Route("magic")]
        public IHttpActionResult Index(SendCodeViewModel sendView)
        {
            return Ok();
        }
        [Route("magic2")]
        public IHttpActionResult About(SendCodeViewModel sendView)
        {

            return Ok("Your application description page.");
        }
        [Route("magic3")]
        public IHttpActionResult Contact(SendCodeViewModel sendView)
        {
            return Ok("Your contact page.");
        }
    }
}
