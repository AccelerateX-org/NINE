﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyStik.TimeTable.Web.Areas.Gym.Controllers
{
    public class HomeController : Controller
    {
        // GET: Gym/Home
        public ActionResult Index()
        {
            return View();
        }
    }
}