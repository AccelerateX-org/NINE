using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web.Mvc;
using Lib.Net.Http.WebPush;
using Lib.Net.Http.WebPush.Authentication;
using MyStik.TimeTable.Web.Areas.Admin.Controllers;
using MyStik.TimeTable.Web.Models;
using Newtonsoft.Json;

namespace MyStik.TimeTable.Web.Controllers
{
    public class NotificationModel
    {
        public string Title { get; set; }
        public string Message { get; set; }
        public string Url { get; set; }
        public string Author { get; set; }
        public string Category { get; set; }
        public string Room { get; set; }
        public string Lecturer { get; set; }

        public string originDate { get; set; }

        public string originTime { get; set; }

        public string newDate { get; set; }

        public string newTime { get; set; }
        
        public string changeDate { get; set; }

        public string changeTime { get; set; }
    }

    public class NotificationController : BaseController
    {
        public ActionResult PersonalMessage()
        {
            var culture = Thread.CurrentThread.CurrentUICulture;
            ViewBag.Culture = culture;

            var model = new NotificationModel()
            {
                originDate = DateTime.Now.ToShortDateString(),
                newDate = DateTime.Now.ToShortDateString(),
                changeDate = DateTime.Now.ToShortDateString(),
                originTime = DateTime.Now.ToShortTimeString(),
                newTime = DateTime.Now.ToShortTimeString(),
                changeTime = DateTime.Now.ToShortTimeString(),
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult PersonalMessage(NotificationModel model)
        {
            var user = GetCurrentUser();

            var devices = user.Devices.Where(x => x.Platform == DevicePlatform.PWA).ToList();

            foreach (var device in devices)
            {
                var subscription = JsonConvert.DeserializeObject<PushSubscription>(device.DeviceName);

                var publicKey = "BOfD3Lh4iV2e1t9PAv58O_sszZTAU72v1oPM7bIs6r1vA2eoIsu6zX5qxk--Zf8_G1dxKPkxckXcGKPWZOlODOQ";
                var privateKey = "VRtnmeVDP3r1LjNS-2lhbgHDRznduYSNKFe0myIjeXc";


                PushServiceClient pushClient = new PushServiceClient();
                pushClient.DefaultAuthentication = new VapidAuthentication(publicKey, privateKey)
                {
                    Subject = "mailto:nine@hm.edu"
                };

                var notification = new AngularPushNotification
                {
                    Title = model.Title, //"news from NINE",
                    Body = model.Message, // $"from nine to fillter",
                    Data = new Dictionary<string, object>(),
                    Icon = "img.png"
                };

                notification.Data["url"] = model.Url; //"https://nine.hm.edu";
                notification.Data["room"] = model.Room; //"R 2.089";
                notification.Data["lecturer"] = model.Lecturer; //"Hinz";
                notification.Data["author"] = model.Author; //"Hinz";
                notification.Data["category"] = model.Category; //"Hinz";

                var oDay = DateTime.Parse(model.originDate);
                var oTime = DateTime.Parse(model.originTime);
                var oDate = oDay.Add(oTime.TimeOfDay);
                notification.Data["dateOrigin"] = oDate;

                var nDay = DateTime.Parse(model.newDate);
                var nTime = DateTime.Parse(model.newTime);
                var nDate = nDay.Add(nTime.TimeOfDay);
                notification.Data["dateChanged"] = nDate;

                var cDay = DateTime.Parse(model.changeDate);
                var cTime = DateTime.Parse(model.changeTime);
                var cDate = cDay.Add(cTime.TimeOfDay);
                notification.Data["dateSend"] = cDate;



                var pushNMessage = notification.ToPushMessage();


                pushClient.RequestPushMessageDeliveryAsync(subscription, pushNMessage);

            }


            return RedirectToAction("Index", "Home");
        }
    }
}