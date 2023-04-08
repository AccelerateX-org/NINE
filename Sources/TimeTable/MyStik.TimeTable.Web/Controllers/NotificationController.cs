using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
//using Lib.Net.Http.WebPush;
//using Lib.Net.Http.WebPush.Authentication;
using MyStik.TimeTable.Web.Areas.Admin.Controllers;
using MyStik.TimeTable.Web.Models;

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
                //var subscription = JsonConvert.DeserializeObject<PushSubscription>(device.DeviceName);

                /*
                    https://tools.reactpwa.com/vapid
                    {
                      "subject": "mailto:nine@hm.edu",
                      "publicKey": "BPI8YpVBlwF62LYEapQb6zEj8i75ZYPHp3ugnYn0Sc8GBBX0s-pZEL-POjEAbzeIBzMQHx1bcq1yhY982hMm7oA",
                      "privateKey": "yMXePWjZMvx_xwZczffh6nG1j-E6oW0fcUVHOYeXR0c"
                    }
                 */


                var publicKey = "BPI8YpVBlwF62LYEapQb6zEj8i75ZYPHp3ugnYn0Sc8GBBX0s-pZEL-POjEAbzeIBzMQHx1bcq1yhY982hMm7oA";
                var privateKey = "yMXePWjZMvx_xwZczffh6nG1j-E6oW0fcUVHOYeXR0c";

                /*
                PushServiceClient pushClient = new PushServiceClient();
                pushClient.DefaultAuthentication = new VapidAuthentication(publicKey, privateKey)
                {
                    Subject = "mailto:nine@hm.edu"
                };
                */

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

                //var pushNMessage = notification.ToPushMessage();


                //pushClient.RequestPushMessageDeliveryAsync(subscription, pushNMessage);




            }


            return RedirectToAction("Index", "Home");
        }
    }
}