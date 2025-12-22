using NodaTime.Calendars;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using System.Web.Http.Results;
using Hangfire;
using MyStik.TimeTable.Web.Api.Contracts;
using Org.BouncyCastle.Asn1.Ocsp;

namespace MyStik.TimeTable.Web.Api.Controller
{
    /*
    [System.Web.Http.RoutePrefix("api/v2/users")]
    public class ScimUsersController : ApiController
    {
        private bool IsValid()
        {
            if (!Request.Headers.Contains("ScimSecret")) return false;
            var token = Request.Headers.GetValues("ScimSecret").FirstOrDefault();

            return !token.Equals("SCIM");
        }

        [System.Web.Http.Route("{id}")]
        public IHttpActionResult GetUsers(string id)
        {
            if (!IsValid()) {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.Forbidden, "No Access"));
            }

            // wenn fillter, dann filltern und Liste
            //
            //if (resourceRequest != null)
            //{
                //var list = new ScimListResourceResponse<ScimUser>();

                //var resp = Request.CreateResponse(HttpStatusCode.OK, list);

              //  return ResponseMessage(resp);
            //}
            //

            // sonst nach user suchen
            if (string.IsNullOrEmpty(id))
            {
                var resp = new ScimErrorResponse(HttpStatusCode.NotFound, "not found");

                var r = Request.CreateResponse(HttpStatusCode.NotFound, resp);

                return ResponseMessage(r);
            }
            var user = new ScimUser
            {
                
            };

            var response = Request.CreateResponse(HttpStatusCode.OK, user);

            //
            //response.Content.Headers.Expires = DateTimeOffset.Now.AddMinutes(10.0);
            //var tag = Guid.NewGuid();
            //response.Headers.ETag = new EntityTagHeaderValue(tag.ToString()); //
            

            return ResponseMessage(response);
        }

    }
    */
}
