using System.Collections.Generic;
using System.Web.Mvc;
using Lib.Net.Http.WebPush;
//using Lib.Net.Http.WebPush;
//using Lib.Net.Http.WebPush.Authentication;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace MyStik.TimeTable.Web.Areas.Admin.Controllers
{
    public class NotificationController : Controller
    {
        // GET: Admin/Notification
        public ActionResult Send()
        {
            var publicKey = "BP2HjQtANkxLEBNq37OAth8Q1Oi59ZcWZO_lKbtRorfg_qY30lSlzMxGHYqH_a4S1p449HLOBy1jM2jy-bliq0o";
            var privateKey = "QJoWkpOGd9H2YFkrU9PeT2jwRM1bU2I4spb3HD2Lgm8";

            /*
            PushServiceClient pushClient = new PushServiceClient();
            pushClient.DefaultAuthentication = new VapidAuthentication(publicKey, privateKey)
            {
                Subject = "https://acceleratex.org"
            };


            PushMessage notification = new AngularPushNotification
            {
                Title = "news from wiQuiz",
                Body = $"eine neues Quiz ist da",
                // Icon = "Assets/icon-96x96.png" - so geht es nicht, andere Art des Pfads
            }.ToPushMessage();

            pushClient.RequestPushMessageDeliveryAsync(MyPushSubscription.Instance.subscription, notification);
            */
            return RedirectToAction("Index", "Dashboard", new {area=""});
        }
    }



    public class AngularPushNotification
    {
        private const string WRAPPER_START = "{\"notification\":";
        private const string WRAPPER_END = "}";

        public class NotificationAction
        {
            public string Action { get; }

            public string Title { get; }

            public NotificationAction(string action, string title)
            {
                Action = action;
                Title = title;
            }
        }

        private static readonly JsonSerializerSettings _jsonSerializerSettings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };

        public string Title { get; set; }

        public string Body { get; set; }

        public string Icon { get; set; }

        public IList<int> Vibrate { get; set; } = new List<int>();

        public IDictionary<string, object> Data { get; set; }

        public IList<NotificationAction> Actions { get; set; } = new List<NotificationAction>();

        
        public PushMessage ToPushMessage(string topic = null, int? timeToLive = null, PushMessageUrgency urgency = PushMessageUrgency.Normal)
        {
            return new PushMessage(WRAPPER_START + JsonConvert.SerializeObject(this, _jsonSerializerSettings) + WRAPPER_END)
            {
                Topic = topic,
                TimeToLive = timeToLive,
                Urgency = urgency
            };
        }
    }

}