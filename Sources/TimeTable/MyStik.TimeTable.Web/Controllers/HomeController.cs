using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using MyStik.TimeTable.Web.Models;
using MyStik.TimeTable.Web.Services;

namespace MyStik.TimeTable.Web.Controllers
{
    //[RequireHttps]
    public class HomeController : BaseController
    {
        public ActionResult Index()
        {
            return RedirectToAction("Index", "Dashboard");
        }


        [AllowAnonymous]
        public ActionResult Contact()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Contact(ContactMailModel model)
        {
            SmtpClient smtp = new SmtpClient();

            smtp.Send(model.Email, "hinz@hm.edu", "[fillter] " + model.Subject, model.Body);

            return View("ThankYou");
        }

        [AllowAnonymous]
        public ActionResult Missing()
        {
            return View();
        }
        
        [AllowAnonymous]
        public ActionResult Imprint()
        {
            return View();
        }


        [AllowAnonymous]
        public ActionResult TestLab()
        {
            return View();
        }
    }
}