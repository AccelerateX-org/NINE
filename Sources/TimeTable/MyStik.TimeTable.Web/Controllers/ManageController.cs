using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using MyStik.TimeTable.Web.Models;
using Newtonsoft.Json.Linq;
using Swashbuckle.Swagger;
using System.Security.Claims;
using static MyStik.TimeTable.DataServices.BaseImportContext;

namespace MyStik.TimeTable.Web.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Authorize]
    public class ManageController : Controller
    {
        private IdentifyConfig.ApplicationSignInManager _signInManager;
        private IdentifyConfig.ApplicationUserManager _userManager;

        /// <summary>
        /// 
        /// </summary>
        public ManageController()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userManager"></param>
        /// <param name="signInManager"></param>
        public ManageController(IdentifyConfig.ApplicationUserManager userManager, IdentifyConfig.ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        /// <summary>
        /// 
        /// </summary>
        public IdentifyConfig.ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<IdentifyConfig.ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public IdentifyConfig.ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<IdentifyConfig.ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task<ActionResult> Index(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "Your password has been changed."
                : message == ManageMessageId.SetPasswordSuccess ? "Your password has been set."
                : message == ManageMessageId.SetTwoFactorSuccess ? "Your two-factor authentication provider has been set."
                : message == ManageMessageId.Error ? "An error has occurred."
                : message == ManageMessageId.AddPhoneSuccess ? "Your phone number was added."
                : message == ManageMessageId.RemovePhoneSuccess ? "Your phone number was removed."
                : "";

            var userId = User.Identity.GetUserId();
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());

            var userLogins = await UserManager.GetLoginsAsync(User.Identity.GetUserId());
            var otherLogins = AuthenticationManager.GetExternalAuthenticationTypes().Where(auth => userLogins.All(ul => auth.AuthenticationType != ul.LoginProvider)).ToList();
            ViewBag.ShowRemoveButton = user.PasswordHash != null || userLogins.Count > 1;
            ViewBag.CalendarToken = user.Id;
            ViewBag.HasImage = false;
            if (user.BinaryData != null && user.BinaryData.Length > 0)
                ViewBag.HasImage = true;

            var model = new IndexViewModel
            {
                HasPassword = HasPassword(),
                PhoneNumber = await UserManager.GetPhoneNumberAsync(userId),
                TwoFactor = await UserManager.GetTwoFactorEnabledAsync(userId),
                Logins = await UserManager.GetLoginsAsync(userId),
                BrowserRemembered = await AuthenticationManager.TwoFactorBrowserRememberedAsync(userId),
                CurrentLogins = userLogins,
                OtherLogins = otherLogins,
                User = user
            };
            return View(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="loginProvider"></param>
        /// <param name="providerKey"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RemoveLogin(string loginProvider, string providerKey)
        {
            ManageMessageId? message;
            var result = await UserManager.RemoveLoginAsync(User.Identity.GetUserId(), new UserLoginInfo(loginProvider, providerKey));
            if (result.Succeeded)
            {
                var claims = await UserManager.GetClaimsAsync(User.Identity.GetUserId());
                foreach (var claim in claims)
                {
                    result = await UserManager.RemoveClaimAsync(User.Identity.GetUserId(), claim);
                }

                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                if (user != null)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                }
                message = ManageMessageId.RemoveLoginSuccess;
            }
            else
            {
                message = ManageMessageId.Error;
            }

            var model = new ExternalLoginStatus
            {
                IsConnect = false,
                Errors = new List<string>(result.Errors)
            };
            return RedirectToAction("Index");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult AddPhoneNumber()
        {
            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddPhoneNumber(AddPhoneNumberViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            // Generate the token and send it
            var code = await UserManager.GenerateChangePhoneNumberTokenAsync(User.Identity.GetUserId(), model.Number);
            if (UserManager.SmsService != null)
            {
                var message = new IdentityMessage
                {
                    Destination = model.Number,
                    Body = "Your security code is: " + code
                };
                await UserManager.SmsService.SendAsync(message);
            }
            return RedirectToAction("VerifyPhoneNumber", new { PhoneNumber = model.Number });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EnableTwoFactorAuthentication()
        {
            await UserManager.SetTwoFactorEnabledAsync(User.Identity.GetUserId(), true);
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user != null)
            {
                await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
            }
            return RedirectToAction("Index", "Manage");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DisableTwoFactorAuthentication()
        {
            await UserManager.SetTwoFactorEnabledAsync(User.Identity.GetUserId(), false);
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user != null)
            {
                await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
            }
            return RedirectToAction("Index", "Manage");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <returns></returns>
        public async Task<ActionResult> VerifyPhoneNumber(string phoneNumber)
        {
            var code = await UserManager.GenerateChangePhoneNumberTokenAsync(User.Identity.GetUserId(), phoneNumber);
            // Send an SMS through the SMS provider to verify the phone number
            return phoneNumber == null ? View("Error") : View(new VerifyPhoneNumberViewModel { PhoneNumber = phoneNumber });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyPhoneNumber(VerifyPhoneNumberViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var result = await UserManager.ChangePhoneNumberAsync(User.Identity.GetUserId(), model.PhoneNumber, model.Code);
            if (result.Succeeded)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                if (user != null)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                }
                return RedirectToAction("Index", new { Message = ManageMessageId.AddPhoneSuccess });
            }
            // If we got this far, something failed, redisplay form
            ModelState.AddModelError("", "Failed to verify phone");
            return View(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> RemovePhoneNumber()
        {
            var result = await UserManager.SetPhoneNumberAsync(User.Identity.GetUserId(), null);
            if (!result.Succeeded)
            {
                return RedirectToAction("Index", new { Message = ManageMessageId.Error });
            }
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user != null)
            {
                await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
            }
            return RedirectToAction("Index", new { Message = ManageMessageId.RemovePhoneSuccess });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult ChangePassword()
        {
            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
            if (result.Succeeded)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                if (user != null)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                }
                return RedirectToAction("Index", new { Message = ManageMessageId.ChangePasswordSuccess });
            }
            AddErrors(result);
            return View(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult ChangeEMail()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());

            ViewBag.User = user;

            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ChangeEMail(ChangeEMailModel model)
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            ViewBag.User = user;

            if (user.Email.ToLower().Equals(model.EMail.ToLower()))
            {
                ModelState.AddModelError("EMail", "Das ist Ihre bisherige Adresse");
                return View(model);
            }

            var existingUser = UserManager.FindByEmail(model.EMail);
            if (existingUser != null)
            {
                ModelState.AddModelError("EMail", "Ungültige E-Mail Adresse");
                return View(model);
            }


            user.Remark = model.EMail;
            UserManager.Update(user);

            var mailModel = new ConfirmEmailMailModel
            {
                User = user,
                Token = "",
            };

            try
            {
                new MailController().ChangeEMail(mailModel).Deliver();
            }
            catch (SmtpFailedRecipientException exSmtp)
            {
                ModelState.AddModelError("EMail", "Fehler bei Zustellung: " + exSmtp.Message);

                user.Remark = string.Empty;
                UserManager.Update(user);

                return View(model);
            }


            return RedirectToAction("Index");
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult SetPassword()
        {
            return View();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SetPassword(SetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await UserManager.AddPasswordAsync(User.Identity.GetUserId(), model.NewPassword);
                if (result.Succeeded)
                {
                    var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                    if (user != null)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                    }
                    return RedirectToAction("Index", new { Message = ManageMessageId.SetPasswordSuccess });
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public async Task<ActionResult> ManageLogins(ExternalLoginStatus status)
        {
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user == null)
            {
                return View("Error");
            }


            return View();

            /*
            var userLogins = await UserManager.GetLoginsAsync(User.Identity.GetUserId());
            var otherLogins = AuthenticationManager.GetExternalAuthenticationTypes().Where(auth => userLogins.All(ul => auth.AuthenticationType != ul.LoginProvider)).ToList();
            ViewBag.ShowRemoveButton = user.PasswordHash != null || userLogins.Count > 1;

            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync(XsrfKey, User.Identity.GetUserId());
            ViewBag.LoginInfo = loginInfo;

            if (loginInfo != null)
            {

                var claims = loginInfo.ExternalIdentity.Claims.ToList();
                ViewBag.Claims = claims;

                var accessToken = claims.Find(c => c.Type == "access_token");

                var client = new HttpClient()
                {
                    BaseAddress = new Uri("https://sso.hm.edu/"),
                };

                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken.Value);

                var response = await client.GetAsync("idp/profile/oidc/userinfo");

                ViewBag.UserName = "unknown";
                ViewBag.Email = "unknown";

                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    ViewBag.UserData = data;

                    if (!String.IsNullOrWhiteSpace(data))
                    {
                        dynamic obj = JObject.Parse(data);

                        string userName = obj["eduPersonPrincipalName"];
                        string email = obj["eduPersonPrincipalName"];

                        ViewBag.UserName = userName;
                        ViewBag.Email = email;

                    }
                }
                else
                {
                    ViewBag.UserData = await response.Content.ReadAsStringAsync();
                }
            }


            return View(new ManageLoginsViewModel
            {
                CurrentLogins = userLogins,
                OtherLogins = otherLogins
            });
            */
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LinkLogin(string provider)
        {
            // Request a redirect to the external login provider to link a login for the current user
            return new AccountController.ChallengeResult(provider, Url.Action("LinkLoginCallback", "Manage"), User.Identity.GetUserId());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> LinkLoginCallback()
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync(XsrfKey, User.Identity.GetUserId());
            if (loginInfo == null)
            {
                var model = new ExternalLoginStatus
                {
                    IsConnect = true,
                    Errors = new List<string> { "Keine Benutzerinfos verfügbar" }
                };
                
                return RedirectToAction("Index");
            }
            var result = await UserManager.AddLoginAsync(User.Identity.GetUserId(), loginInfo.Login);

            if (loginInfo.ExternalIdentity.Claims != null)
            {
                var claims = loginInfo.ExternalIdentity.Claims.ToList();
                var accessToken = claims.Find(c => c.Type == "access_token");

                // jetzt den eduPersonPrincipalName als claim speichern
                // für spätere Zweck, falls mal SCIM kommt oder so

                var client = new HttpClient()
                {
                    BaseAddress = new Uri("https://sso.hm.edu/"),
                };

                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken.Value);

                var response = await client.GetAsync("idp/profile/oidc/userinfo");
                dynamic userInfo = null;

                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();

                    if (!String.IsNullOrWhiteSpace(data))
                    {
                        userInfo = JObject.Parse(data);
                    }
                }

                if (userInfo != null)
                {
                    var userId = User.Identity.GetUserId();
                    var userClaims = await UserManager.GetClaimsAsync(userId);
                    foreach (var property in userInfo.Properties())
                    {
                        var userClaim = userClaims.FirstOrDefault(c => c.Type == property.Name);
                        if (userClaim != null)
                        {
                            await UserManager.RemoveClaimAsync(userId, userClaim);
                        }

                        var infoClaim = new Claim(property.Name, property.Value.ToString());
                        await UserManager.AddClaimAsync(userId, infoClaim);
                    }

                }
            }

            var model2 = new ExternalLoginStatus
            {
                IsConnect = true,
                Errors = new List<string>(result.Errors)
            };

            // original
            return RedirectToAction("Index");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && _userManager != null)
            {
                _userManager.Dispose();
                _userManager = null;
            }

            base.Dispose(disposing);
        }

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private bool HasPassword()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (user != null)
            {
                return user.PasswordHash != null;
            }
            return false;
        }

        private bool HasPhoneNumber()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (user != null)
            {
                return user.PhoneNumber != null;
            }
            return false;
        }

        public async Task<ActionResult> GetProfileImage()
        {
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            return File(user.BinaryData, user.FileType);
        }


        public ActionResult DeleteProfileImage()
        {
            var _db = new ApplicationDbContext();

            var user = _db.Users.SingleOrDefault(u => u.UserName.Equals(User.Identity.Name));

            if (user != null)
            {
                user.BinaryData = null;
                user.FileType = string.Empty;

                _db.SaveChanges();
            }

            return RedirectToAction("Index");
        }


        /// <summary>
        /// 
        /// </summary>
        public enum ManageMessageId
        {
            /// <summary>
            /// 
            /// </summary>
            AddPhoneSuccess,
            /// <summary>
            /// 
            /// </summary>
            ChangePasswordSuccess,
            /// <summary>
            /// 
            /// </summary>
            SetTwoFactorSuccess,
            /// <summary>
            /// 
            /// </summary>
            SetPasswordSuccess,
            /// <summary>
            /// 
            /// </summary>
            RemoveLoginSuccess,
            /// <summary>
            /// 
            /// </summary>
            RemovePhoneSuccess,
            /// <summary>
            /// 
            /// </summary>
            Error
        }

        #endregion
    }

    public class ExternalLoginStatus
    {
        public bool IsConnect { get; set; }
        public List<string> Errors { get; set; }
    }
}