using Microsoft.Owin.Security.OAuth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace MyStik.TimeTable.Web.Helpers
{
    public class AuthProvider : OAuthAuthorizationServerProvider
    {
        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            string clientId;
            string clientSecret;

            //if (context.TryGetBasicCredentials(out clientId, out clientSecret)) 
            if (context.TryGetFormCredentials(out clientId, out clientSecret))
            {
                    // validate the client Id and secret against database or from configuration file.
                    context.Validated();
            }
            else
            {
                context.SetError("invalid_client", "Client credentials could not be retrieved from the Authorization header");
                context.Rejected();
            }

            return Task.CompletedTask;
        }

        public override Task GrantAuthorizationCode(OAuthGrantAuthorizationCodeContext context)
        {
            var isValid = context.Request.Headers["token"] = "Hallo";

            context.Validated();
            return Task.CompletedTask;
            //return base.GrantAuthorizationCode(context);
        }
    }
}