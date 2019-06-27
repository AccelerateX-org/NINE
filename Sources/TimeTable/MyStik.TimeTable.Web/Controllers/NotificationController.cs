using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Lib.Net.Http.WebPush;
using Lib.Net.Http.WebPush.Authentication;
using MyStik.TimeTable.Web.Api.Controller;
using MyStik.TimeTable.Web.Areas.Admin.Controllers;
using MyStik.TimeTable.Web.Models;
using Newtonsoft.Json;

namespace MyStik.TimeTable.Web.Controllers
{
    public class NotificationController : BaseController
    {
        // GET: Notification
        public ActionResult SendMeANotofication()
        {
            var user = GetCurrentUser();

            var device = user.Devices.FirstOrDefault(x => x.Platform == DevicePlatform.PWA);

            if (device != null)
            {
                var subscription = JsonConvert.DeserializeObject<PushSubscription>(device.DeviceName);


                var publicKey = "BP2HjQtANkxLEBNq37OAth8Q1Oi59ZcWZO_lKbtRorfg_qY30lSlzMxGHYqH_a4S1p449HLOBy1jM2jy-bliq0o";
                var privateKey = "QJoWkpOGd9H2YFkrU9PeT2jwRM1bU2I4spb3HD2Lgm8";


                PushServiceClient pushClient = new PushServiceClient();
                pushClient.DefaultAuthentication = new VapidAuthentication(publicKey, privateKey)
                {
                    Subject = "https://acceleratex.org"
                };


                PushMessage notification = new AngularPushNotification
                {
                    Title = "news from NINE",
                    Body = $"from nine to fillter",
                    // Icon = "Assets/icon-96x96.png" - so geht es nicht, andere Art des Pfads
                }.ToPushMessage();

                
                pushClient.RequestPushMessageDeliveryAsync(subscription, notification);




            }


            return RedirectToAction("Index", "Home");
        }
    }
}