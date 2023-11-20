using System.Collections.Generic;
using System.Linq;
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
    }

    public class NotificationController : BaseController
    {
        public ActionResult PersonalMessage()
        {
            return View();
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

                var pushNMessage = notification.ToPushMessage();


                pushClient.RequestPushMessageDeliveryAsync(subscription, pushNMessage);

            }


            return RedirectToAction("Index", "Home");
        }
    }
}