using System;
using System.Web;
using System.Web.Mvc;

namespace MyStik.TimeTable.Web.Helpers
{
    /// <summary>
    /// 
    /// </summary>
    public class CookieConsentAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// Some government relax their interpretation of the law somewhat:
        /// After the first page with the message, clicking anything other than the cookie refusal link may be interpreted as implicitly allowing cookies. 
        /// </summary>
        public bool ImplicitlyAllowCookies { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public CookieConsentAttribute()
        {
            ImplicitlyAllowCookies = true;
        }

        /// <summary>
        /// 
        /// </summary>
        public const string CONSENT_COOKIE_NAME = "CookieConsentNINE";
        private const string cookieConsentContextName = "cookieConsentInfo";

        private class CookieConsentInfo
        {
            public bool NeedToAskConsent { get; set; }
            public bool HasConsent { get; set; }

            public CookieConsentInfo()
            {
                NeedToAskConsent = true;
                HasConsent = false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filterContext"></param>
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var consentInfo = new CookieConsentInfo();

            var request = filterContext.HttpContext.Request;

            // Check if the user has a consent cookie
            var consentCookie = request.Cookies[CONSENT_COOKIE_NAME];

            if (consentCookie == null)
            {
                // No consent cookie. We first check the Do Not Track header value, this can have the value "0" or "1"
                string dnt = request.Headers.Get("DNT");

                // If we receive a DNT header, we accept its value (0 = give consent, 1 = deny) and do not ask the user anymore...
                if (!String.IsNullOrEmpty(dnt))
                {
                    consentInfo.NeedToAskConsent = false;

                    if (dnt == "0")
                    {
                        consentInfo.HasConsent = true;
                    }
                }
                else
                {
                    if (IsSearchCrawler(request.Headers.Get("User-Agent")))
                    {
                        // don't ask consent from search engines, also don't set cookies
                        consentInfo.NeedToAskConsent = false;
                    }
                    else
                    {
                        // first request on the site and no DNT header (we use session cookie, which is allowed by EU cookie law). 
                        consentCookie = new HttpCookie(CONSENT_COOKIE_NAME);
                        consentCookie.Value = "asked";

                        filterContext.HttpContext.Response.Cookies.Add(consentCookie);
                    }
                }
            }
            else
            {
                // we received a consent cookie
                consentInfo.NeedToAskConsent = false;

                if (ImplicitlyAllowCookies && consentCookie.Value == "asked")
                {
                    // consent is implicitly given & stored
                    consentCookie.Value = "true";
                    consentCookie.Expires = DateTime.UtcNow.AddYears(1);
                    filterContext.HttpContext.Response.Cookies.Set(consentCookie);

                    consentInfo.HasConsent = true;
                }
                else if (consentCookie.Value == "true")
                {
                    consentInfo.HasConsent = true;
                }
                else
                {
                    // assume consent denied
                    consentInfo.HasConsent = false;
                }
            }

            HttpContext.Current.Items[cookieConsentContextName] = consentInfo;

            base.OnActionExecuting(filterContext);
        }

        private bool IsSearchCrawler(string userAgent)
        {
            if (!string.IsNullOrEmpty(userAgent))
            {
                string[] crawlers = new string[]
                {
                    "Baiduspider",
                    "Googlebot",
                    "YandexBot",
                    "YandexImages",
                    "bingbot",
                    "msnbot",
                    "Vagabondo",
                    "SeznamBot",
                    "ia_archiver",
                    "AcoonBot",
                    "Yahoo! Slurp",
                    "AhrefsBot"
                };
                foreach (string crawler in crawlers)
                    if (userAgent.Contains(crawler))
                        return true;
            }
            return false;
        }
    }

    /// <summary>
    /// Helper class for easy/typesafe getting the cookie consent status
    /// </summary>
    public static class CookieConsent
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="response"></param>
        /// <param name="consent"></param>
        public static void SetCookieConsent(HttpResponseBase response, bool consent)
        {
            var consentCookie = new HttpCookie(CookieConsentAttribute.CONSENT_COOKIE_NAME);
            consentCookie.Value = consent ? "true" : "false";
            consentCookie.Expires = DateTime.UtcNow.AddYears(1);
            response.Cookies.Set(consentCookie);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static bool AskCookieConsent(ViewContext context)
        {
            return context.ViewBag.AskCookieConsent ?? false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static bool HasCookieConsent(ViewContext context)
        {
            return context.ViewBag.HasCookieConsent ?? false;
        }
    }
}