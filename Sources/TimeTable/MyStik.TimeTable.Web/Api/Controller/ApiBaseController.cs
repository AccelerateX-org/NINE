using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using MyStik.TimeTable.Data;

namespace MyStik.TimeTable.Web.Api.Controller
{
    public class ApiBaseController : ApiController
    {
        protected readonly TimeTableDbContext Db = new TimeTableDbContext();

    }
}
