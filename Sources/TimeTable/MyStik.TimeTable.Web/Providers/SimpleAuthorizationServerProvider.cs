using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security.OAuth;
using MyStik.TimeTable.Web.Models;

namespace MyStik.TimeTable.Web.Providers
{
    public class SimpleAuthorizationServerProvider : OAuthAuthorizationServerProvider
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            context.Validated();
        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {

            context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { "*" });


            using (var _repo = new IdentifyConfig.ApplicationUserManager(new UserStore<ApplicationUser>(new ApplicationDbContext())))
            {
                ApplicationUser user = null;
                //Überprüfen ob Mail
                if (context.UserName.Contains("@"))
                {
                    //Suche ob Mail vorhanden
                    var tempUser = await _repo.FindByEmailAsync(context.UserName);

                    if (tempUser != null)
                    {
                        //wenn was gefunden wurde, Überprüfen ob PW stimmt
                        user = await _repo.FindAsync(tempUser.UserName, context.Password);
                    }
                }
                //Übergebener string ist evtl Loginname
                else
                {
                    //Überprüfen ob vorhanden und PW stimmt
                    user = await _repo.FindAsync(context.UserName, context.Password);
                }

                if (user == null)
                {
                    context.SetError("invalid_grant", "The user name or password is incorrect.");
                    return;
                }
            }

            var identity = new ClaimsIdentity(context.Options.AuthenticationType);
            identity.AddClaim(new Claim("sub", context.UserName));
            identity.AddClaim(new Claim("role", "user"));

            context.Validated(identity);

        }
    }
}