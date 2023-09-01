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

                var publicKey = "BOfD3Lh4iV2e1t9PAv58O_sszZTAU72v1oPM7bIs6r1vA2eoIsu6zX5qxk--Zf8_G1dxKPkxckXcGKPWZOlODOQ";
                var privateKey = "VRtnmeVDP3r1LjNS-2lhbgHDRznduYSNKFe0myIjeXc";


                PushServiceClient pushClient = new PushServiceClient();
                pushClient.DefaultAuthentication = new VapidAuthentication(publicKey, privateKey)
                {
                    Subject = "mailto:nine@hm.edu"
                };

                var notification = new AngularPushNotification
                {
                    Title = "news from NINE",
                    Body = $"from nine to fillter",
                    // Icon = "Assets/icon-96x96.png" - so geht es nicht, andere Art des Pfads
                    Data = new Dictionary<string, object>(),
                    Icon = "img.png"
                };

                notification.Data["url"] = "https://nine.hm.edu";
                notification.Data["room"] = "R 2.089";
                notification.Data["lecturer"] = "Hinz";

                var pushNMessage = notification.ToPushMessage();


                pushClient.RequestPushMessageDeliveryAsync(subscription, pushNMessage);

            }


            return RedirectToAction("Index", "Home");
        }
    }
}