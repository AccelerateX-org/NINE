﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyStik.TimeTable.Web.Controllers
{
    [AllowAnonymous]
    public class ApiTestController : Controller
    {
        // GET: ApiTest
        public ActionResult Index()
        {
            return View();
        }
    }
}