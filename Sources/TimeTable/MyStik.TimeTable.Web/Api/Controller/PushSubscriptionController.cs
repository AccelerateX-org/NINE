using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using Lib.Net.Http.WebPush;
using MyStik.TimeTable.Web.Models;
using Newtonsoft.Json;

namespace MyStik.TimeTable.Web.Api.Controller
{
    public sealed class MyPushSubscription
    {
        public PushSubscription subscription { get; set; }

        public string userid { get; set; }

        static readonly MyPushSubscription _instance = new MyPushSubscription();

        public static MyPushSubscription Instance
        {
            get { return _instance; }
        }

        private MyPushSubscription()
        {

        }
    }


    [RoutePrefix("api/v2/push")]

    public class PushSubscriptionController : ApiBaseController
    {
        [Route("{getkey}")]
        public HttpResponseMessage GetPublicKey()
        {
            string result = "BP2HjQtANkxLEBNq37OAth8Q1Oi59ZcWZO_lKbtRorfg_qY30lSlzMxGHYqH_a4S1p449HLOBy1jM2jy-bliq0o";
            var resp = new HttpResponseMessage(HttpStatusCode.OK);
            resp.Content = new StringContent(result, System.Text.Encoding.UTF8, "text/plain");
            return resp;
        }

        [Route("{subscribe}")]
        [ResponseType(typeof(void))]
        [System.Web.Http.HttpPost]
        public IHttpActionResult Subscribe([FromBody] PushSubscription subscription, string userid)
        {
            var deviceId = subscription.Endpoint;
            var deviceDesc = JsonConvert.SerializeObject(subscription);

            var userDb = new ApplicationDbContext();

            var currentUser = userDb.Users.SingleOrDefault(u => u.Id.Equals(userid));
            if (currentUser == null)
                return StatusCode(HttpStatusCode.NoContent);

            // Überprüfen, ob der Token bereits in der DB hinterlegt ist            
            var existingUser = userDb.Users.FirstOrDefault(x => x.Devices.Any(y => y.DeviceId.Equals(deviceId)));

            if (existingUser != null)
            {

                // Falls der Token schon hinterlegt ist, werden die UserIds verglichen
                // Wenn die UserId nicht mit der neuen übereinstimmt wird der token bei der neuen UserId hinterlegt und bei der alten gelöscht

                if (!existingUser.Id.Equals(currentUser.Id))
                {

                    // das Device, dass ich entfernen will
                    var deviceToDelete = existingUser.Devices.FirstOrDefault(x => x.DeviceId.Equals(deviceId));
                    // Token beim alten User entfernen
                    existingUser.Devices.Remove(deviceToDelete);

                    // Token beim neuen User hinzufügen
                    var device = new UserDevice()
                    {
                        DeviceId = deviceId,
                        DeviceName = deviceDesc,
                        Platform = DevicePlatform.PWA,
                        Registered = DateTime.Now,
                    };
                    currentUser.Devices.Add(device);
                    userDb.Devices.Add(device);
                }
            }
            else
            {
                // Token beim neuen User hinzufügen
                var device = new UserDevice()
                {
                    DeviceId = deviceId,
                    DeviceName = deviceDesc,
                    Platform = DevicePlatform.PWA,
                    Registered = DateTime.Now,
                };
                currentUser.Devices.Add(device);
                userDb.Devices.Add(device);
            }
            userDb.SaveChanges();


            return StatusCode(HttpStatusCode.NoContent);
        }

    }
}
