using System;
using System.Configuration;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using log4net;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Owin;
using Microsoft.Owin.Host.SystemWeb;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using Microsoft.Owin.Security.OpenIdConnect;
using MyStik.TimeTable.Web.Models;
using MyStik.TimeTable.Web.Providers;
using MyStik.TimeTable.Web.Utils;
using Newtonsoft.Json;
using Owin;
using MyStik.TimeTable.Web.Helpers;
using MyStik.TimeTable.Data.Migrations;

namespace MyStik.TimeTable.Web
{
    /// <summary>
    /// 
    /// </summary>
    public partial class Startup
    {
        /// <summary>
        /// For more information on configuring authentication, please visit http://go.microsoft.com/fwlink/?LinkId=301864
        /// </summary>
        /// <param name="app"></param>
        public void ConfigureAuth(IAppBuilder app)
        {
            // Configure the db context, user manager and signin manager to use a single instance per request
            app.CreatePerOwinContext(ApplicationDbContext.Create);
            app.CreatePerOwinContext<IdentifyConfig.ApplicationUserManager>(IdentifyConfig.ApplicationUserManager.Create);
            app.CreatePerOwinContext<IdentifyConfig.ApplicationSignInManager>(IdentifyConfig.ApplicationSignInManager.Create);

            // Enable the application to use a cookie to store information for the signed in user
            // and to use a cookie to temporarily store information about a user logging in with a third party login provider
            // Configure the sign in cookie
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Account/Login"),
                Provider = new CookieAuthenticationProvider
                {
                    // Enables the application to validate the security stamp when the user logs in.
                    // This is a security feature which is used when you change a password or add an external login to your account.  
                    OnValidateIdentity = SecurityStampValidator.OnValidateIdentity<IdentifyConfig.ApplicationUserManager, ApplicationUser>(
                        validateInterval: TimeSpan.FromMinutes(30),
                        regenerateIdentity: (manager, user) => user.GenerateUserIdentityAsync(manager))
                }
            });
            // Use a cookie to temporarily store information about a user logging in with a third party login provider
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            // Enables the application to temporarily store user information when they are verifying the second factor in the two-factor authentication process.
            app.UseTwoFactorSignInCookie(DefaultAuthenticationTypes.TwoFactorCookie, TimeSpan.FromMinutes(5));

            // Enables the application to remember the second login verification factor such as phone or email.
            // Once you check this option, your second step of verification during the login process will be remembered on the device where you logged in from.
            // This is similar to the RememberMe option when you log in.
            app.UseTwoFactorRememberBrowserCookie(DefaultAuthenticationTypes.TwoFactorRememberBrowserCookie);
        }



        private void ConfigureOIDC(IAppBuilder app)
        {
            var OidcCaption = ConfigurationManager.AppSettings["oidc:Caption"];
            var OidcAuthority = ConfigurationManager.AppSettings["oidc:Authority"];
            var OidcRedirectUrl = ConfigurationManager.AppSettings["oidc:RedirectUrl"];
            var OidcClientId = ConfigurationManager.AppSettings["oidc:ClientId"];
            var OidcClientSecret = ConfigurationManager.AppSettings["oidc:ClientSecret"];
            var OidcLogoutUrl = ConfigurationManager.AppSettings["oidc:LogoutUrl"];


            var oidcOptions = new OpenIdConnectAuthenticationOptions
            {
                AuthenticationType = OidcAuthority,
                Caption = OidcCaption,
                Authority = OidcAuthority,
                ClientId = OidcClientId,
                ClientSecret = OidcClientSecret,
                PostLogoutRedirectUri = OidcLogoutUrl,   //"https://localhost:44300/",
                RedirectUri = OidcRedirectUrl,
                ResponseType = OpenIdConnectResponseType.Code,
                //Scope = $"{OpenIdConnectScope.Phone} {OpenIdConnectScope.OpenIdProfile}",
                Scope = "openid profile email",
                UsePkce = false,
                RedeemCode = true,
                SaveTokens = true,
                Notifications = new OpenIdConnectAuthenticationNotifications
                {
                    MessageReceived = async n =>
                    {
                        var logger = LogManager.GetLogger("OIDC");
                        logger.Debug("in MessageReceived");
                        logger.DebugFormat("obje: {0}", n);
                    },
                    AuthorizationCodeReceived = async n =>
                    {
                        var logger = LogManager.GetLogger("OIDC");
                        logger.Debug("in AuthorizationCodeReceived");
                        logger.DebugFormat("obje: {0}", n);
                    },
                    TokenResponseReceived = async n =>
                    {
                        var logger = LogManager.GetLogger("OIDC");
                        logger.Debug("in TokenResponseReceived");
                        logger.DebugFormat("obje: {0}", n);
                    },
                    SecurityTokenReceived = async n =>
                    {
                        var logger = LogManager.GetLogger("OIDC");
                        logger.Debug("in SecurityTokenReceived");
                        logger.DebugFormat("obje: {0}", n);
                    },
                    AuthenticationFailed = async n =>
                    {
                        var logger = LogManager.GetLogger("OIDC");
                        logger.Debug("in AuthenticationFailed");
                        logger.DebugFormat("obje: {0}", n);
                    },
                    RedirectToIdentityProvider = async n =>
                    {
                        var logger = LogManager.GetLogger("OIDC");
                        logger.Debug("in RedirectToIdentityProvider");
                        logger.DebugFormat("obje: {0}", n);
                    },
                    SecurityTokenValidated = async n =>
                    {
                        var logger = LogManager.GetLogger("OIDC");
                        logger.Debug("in SecurityTokenValidated");
                        logger.DebugFormat("obje: {0}", n);

                        n.AuthenticationTicket.Identity.AddClaim(new Claim("id_token", n.ProtocolMessage.IdToken));
                        n.AuthenticationTicket.Identity.AddClaim(new Claim("access_token", n.ProtocolMessage.AccessToken));
                    },
                }
            };

            app.UseOpenIdConnectAuthentication(oidcOptions);


        }

        private void ConfigureAuth0(IAppBuilder app)
        {
            var auth0Domain = ConfigurationManager.AppSettings["auth0:Domain"];
            var auth0ClientId = ConfigurationManager.AppSettings["auth0:ClientId"];
            var auth0RedirectUri = ConfigurationManager.AppSettings["auth0:RedirectUri"];
            var auth0PostLogoutRedirectUri = ConfigurationManager.AppSettings["auth0:PostLogoutRedirectUri"];

            // Configure Auth0 authentication
            app.UseOpenIdConnectAuthentication(new OpenIdConnectAuthenticationOptions
            {
                AuthenticationType = "Auth0",

                Authority = $"https://{auth0Domain}",

                ClientId = auth0ClientId,

                RedirectUri = auth0RedirectUri,
                PostLogoutRedirectUri = auth0PostLogoutRedirectUri,

                Scope = "openid profile email",

                TokenValidationParameters = new TokenValidationParameters
                {
                    NameClaimType = "name"
                },

                // More information on why the CookieManager needs to be set can be found here: 
                // https://docs.microsoft.com/en-us/aspnet/samesite/owin-samesite
                CookieManager = new SameSiteCookieManager(new SystemWebCookieManager()),

                Notifications = new OpenIdConnectAuthenticationNotifications
                {
                    RedirectToIdentityProvider = notification =>
                    {
                        if (notification.ProtocolMessage.RequestType == OpenIdConnectRequestType.Logout)
                        {
                            var logoutUri = $"https://{auth0Domain}/v2/logout?client_id={auth0ClientId}";

                            var postLogoutUri = notification.ProtocolMessage.PostLogoutRedirectUri;
                            if (!string.IsNullOrEmpty(postLogoutUri))
                            {
                                if (postLogoutUri.StartsWith("/"))
                                {
                                    // transform to absolute
                                    var request = notification.Request;
                                    postLogoutUri = request.Scheme + "://" + request.Host + request.PathBase + postLogoutUri;
                                }
                                logoutUri += $"&returnTo={Uri.EscapeDataString(postLogoutUri)}";
                            }

                            notification.Response.Redirect(logoutUri);
                            notification.HandleResponse();
                        }
                        return Task.FromResult(0);
                    }
                }
            });

        }

        private void ConfigureSSO(IAppBuilder app)
        {
            var ssoDomain = ConfigurationManager.AppSettings["sso:Domain"];
            var ssoClientId = ConfigurationManager.AppSettings["sso:ClientId"];
            var ssoClientSecret = ConfigurationManager.AppSettings["sso:ClientSecret"];
            var ssoRedirectUri = ConfigurationManager.AppSettings["sso:RedirectUri"];
            var ssoPostLogoutRedirectUri = ConfigurationManager.AppSettings["sso:PostLogoutRedirectUri"];

            var json = "";
            using (WebClient wc = new WebClient())
            {
                json = wc.DownloadString("https://sso.hm.edu/.well-known/openid-configuration");
            }



            // Configure Auth0 authentication
            app.UseOpenIdConnectAuthentication(new OpenIdConnectAuthenticationOptions
            {
                AuthenticationType = "SSO",

                Authority = $"https://{ssoDomain}",

                ClientId = ssoClientId,
                ClientSecret = ssoClientSecret,

                RedirectUri = ssoRedirectUri,

                ResponseType = OpenIdConnectResponseType.Code,
                ResponseMode = OpenIdConnectResponseMode.Query,
                //RedeemCode = true,
                //SaveTokens = true,
                //CallbackPath = new PathString("/callback"),
                

                PostLogoutRedirectUri = ssoPostLogoutRedirectUri,

                //Configuration = new OpenIdConnectConfiguration(json),

                Scope = "openid profile email",

                TokenValidationParameters = new TokenValidationParameters
                {
                    NameClaimType = "name",
                    ValidateIssuer = false
                },

                RequireHttpsMetadata = false,
                SignInAsAuthenticationType = "Cookies",

                // More information on why the CookieManager needs to be set can be found here: 
                // https://docs.microsoft.com/en-us/aspnet/samesite/owin-samesite
                //CookieManager = new SameSiteCookieManager(new SystemWebCookieManager()),

                Notifications = new OpenIdConnectAuthenticationNotifications
                {
                    RedirectToIdentityProvider = notification =>
                    {
                        /*
                        if (notification.ProtocolMessage.RequestType == OpenIdConnectRequestType.Logout)
                        {
                            var logoutUri = $"https://{ssoDomain}/v2/logout?client_id={ssoClientId}";

                            var postLogoutUri = notification.ProtocolMessage.PostLogoutRedirectUri;
                            if (!string.IsNullOrEmpty(postLogoutUri))
                            {
                                if (postLogoutUri.StartsWith("/"))
                                {
                                    // transform to absolute
                                    var request = notification.Request;
                                    postLogoutUri = request.Scheme + "://" + request.Host + request.PathBase + postLogoutUri;
                                }
                                logoutUri += $"&returnTo={Uri.EscapeDataString(postLogoutUri)}";
                            }

                            notification.Response.Redirect(logoutUri);
                            notification.HandleResponse();
                        }
                        */
                        return Task.FromResult(0);
                    },
                    AuthorizationCodeReceived = notification =>
                    {
                        return Task.FromResult(0);
                    },
                    MessageReceived = notification =>
                    {
                        return Task.FromResult(0);
                    },
                    AuthenticationFailed = notification =>
                    {
                        return Task.FromResult(0);
                    },
                    SecurityTokenReceived = notification =>
                    {
                        return Task.FromResult(0);
                    },
                    SecurityTokenValidated = notification =>
                    {
                        return Task.FromResult(0);
                    },
                    TokenResponseReceived = notification =>
                    {
                        return Task.FromResult(0);
                    },
                }
            });

        }



        /// <summary>
        /// 
        /// </summary>
        public void ConfigureOAuth(IAppBuilder app)
        {
            // mit eigenem Provider
            /*
            var myProvider = new AuthProvider();
            var options = new OAuthAuthorizationServerOptions
            {
                AllowInsecureHttp = true,
                TokenEndpointPath = new PathString("/api/v2/token"),
                AccessTokenExpireTimeSpan = TimeSpan.FromDays(1),
                Provider = myProvider
            };
            */

            var options = new OAuthAuthorizationServerOptions()
            {
                AllowInsecureHttp = true,
                TokenEndpointPath = new PathString("/token"),
                AccessTokenExpireTimeSpan = TimeSpan.FromDays(1),
                Provider = new SimpleAuthorizationServerProvider()
            };
            // Token Generation
            app.UseOAuthAuthorizationServer(options);
            app.UseOAuthBearerAuthentication(new OAuthBearerAuthenticationOptions());

        }


    }
}