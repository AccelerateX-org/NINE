using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using Lib.Net.Http.WebPush;

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
            MyPushSubscription.Instance.subscription = subscription;
            MyPushSubscription.Instance.userid = userid;

            return StatusCode(HttpStatusCode.NoContent);
        }

    }
}
