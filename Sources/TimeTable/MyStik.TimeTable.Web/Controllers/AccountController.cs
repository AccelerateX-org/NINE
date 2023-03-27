using System;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using log4net;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using MyStik.TimeTable.Data;
using MyStik.TimeTable.Web.Helpers;
using MyStik.TimeTable.Web.Models;

namespace MyStik.TimeTable.Web.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Authorize]
    //[CookieConsent]
    public class AccountController : BaseController
    {
        private IdentifyConfig.ApplicationSignInManager _signInManager;
        private ILog logger = LogManager.GetLogger("Account");

        /// <summary>
        /// 
        /// </summary>
        public AccountController()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userManager"></param>
        /// <param name="signInManager"></param>
        public AccountController(IdentifyConfig.ApplicationUserManager userManager, IdentifyConfig.ApplicationSignInManager signInManager)
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
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (string.IsNullOrEmpty(model.Email) && string.IsNullOrEmpty(model.Password))
                return RedirectToAction("Login");

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Standard: 1 E-Mail Adresse = Benutzername
            var user = await UserManager.FindByEmailAsync(model.Email);
            // Abwärtskompatibel: Benutzername
            if (user == null)
            {
                user = await UserManager.FindByNameAsync(model.Email);
            }
            // Ausnahme: In Kontakten suchen => bisher noch nicht


            if (user == null)
            {
                ModelState.AddModelError("", "Falscher Benutzername oder Passwort.");
                logger.WarnFormat("Invalid login attempt: {0}", model.Email);
                return View(model);
            }

            // Require the user to have a confirmed email before they can log on.
            if (user.Registered > new DateTime(2015, 7, 26))
            {
                if (!await UserManager.IsEmailConfirmedAsync(user.Id))
                {
                    var model2 = new CheckEMailViewModel();

                    model2.EMail = user.Email;
                    model2.UserId = user.Id;

                    return View("CheckEMail", model2);
                }
            }
            else
            {
                // Reparatur alter Konten
                // 
                if (!user.EmailConfirmed)
                {
                    user.EmailConfirmed = true;
                    user.ExpiryDate = null;
                    user.Remark = String.Empty;
                    user.Group = String.Empty;
                    user.Curriculum = String.Empty;
                    user.Email = string.IsNullOrEmpty(user.Email) ? String.Empty : user.Email.ToLower();
                    await UserManager.UpdateAsync(user);
                }
            }

            // Version 0.4.29; Gäste gibt es nicht mehr
            if (user.MemberState == MemberState.Guest)
            {
                user.MemberState = MemberState.Student;
                await UserManager.UpdateAsync(user);
            }

            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, change to shouldLockout: true
            var result = await SignInManager.PasswordSignInAsync(user.UserName, model.Password, model.RememberMe, shouldLockout: false);
            switch (result)
            {
                case SignInStatus.Success:
                    await UserManager.StoreLogInAsync(user.Id);
                    var sem = SemesterService.GetSemester(DateTime.Today);

                    if (sem != null)
                    {
                        Session["SemesterName"] = sem.Name;
                        Session["SemesterId"] = sem.Id;
                    }

                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Falscher Benutzername oder Passwort.");
                    return View(model);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="returnUrl"></param>
        /// <param name="rememberMe"></param>
        /// <returns></returns>
        [AllowAnonymous]
        public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
        {
            // Require that the user has already logged in via username/password or external login
            if (!await SignInManager.HasBeenVerifiedAsync())
            {
                return View("Error");
            }
            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // The following code protects for brute force attacks against the two factor codes. 
            // If a user enters incorrect codes for a specified amount of time then the user account 
            // will be locked out for a specified amount of time. 
            // You can configure the account lockout settings in IdentityConfig
            var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent: model.RememberMe, rememberBrowser: model.RememberBrowser);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(model.ReturnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid code.");
                    return View(model);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var now = DateTime.Now;
                var user = new ApplicationUser
                {
                    UserName = model.Email, 
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Registered = now,
                    // bis Version 0.4.28 MemberState = model.Email.ToLower().EndsWith("@hm.edu") ? MemberState.Student : MemberState.Guest,
                    MemberState = MemberState.Student,  // ab version 0.4.29
                    Remark = "Abschluss Registrierung steht aus",
                    ExpiryDate = DateTime.Today.AddDays(14),
                    Submitted = now
                };
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    // nicht automatisch anmelden, sondern auf Nachrichtenseite weiterleiten
                    //await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

                    // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                    // Send an email with this link
                    string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    var mailModel = new ConfirmEmailMailModel
                    {
                        User = user,
                        Token = code,
                    };

                    try
                    {
                        new MailController().VerfiyEMail(mailModel).Deliver();

                        var modelSuccess = new RegisterSuccessViewModel
                        {
                            Email = model.Email,
                            ExpiryDate = user.ExpiryDate.Value,
                        };

                        // Bestätigungs E-Mail senden und Bestätigungsseite senden
                        return RedirectToAction("RegisterSuccess", modelSuccess);

                    }
                    catch (SmtpFailedRecipientException exSmtp)
                    {
                        ModelState.AddModelError("Email", "Fehler bei Zustellung: " + exSmtp.Message);
                        return View(model);
                    }


                    //return RedirectToAction("Index", "Home");
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [AllowAnonymous]
        public ActionResult RegisterSuccess(RegisterSuccessViewModel model)
        {
            return View(model);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            var model = new VerifyEMailModel();
            if (userId == null || code == null)
            {
                model.Message = "Ungültiger Link";
                return View("VerifyEMailError", model);
            }
            var result = await UserManager.ConfirmEmailAsync(userId, code);

            if (result.Succeeded)
            {
                // Expiry Date entfernen, Bemerkung entfernen
                var user = await UserManager.FindByIdAsync(userId);
                if (user != null)
                {
                    user.ExpiryDate = null;
                    user.Remark = string.Empty;
                    user.Submitted = null;
                    await UserManager.UpdateAsync(user);
                }

                return View("VerifyEMailSuccess", model);

            }
            else
            {
                model.Message = "";
                foreach (var error in result.Errors)
                {
                    model.Message += error + ", ";
                }

                return View("VerifyEMailError", model);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [AllowAnonymous]
        public async Task<ActionResult> ChangeEmail(string userId)
        {
            var model = new VerifyEMailModel();
            if (userId == null)
            {
                model.Message = "Ungültiger Link";
                return View("ChangeEMailError", model);
            }

            // Expiry Date entfernen, Bemerkung entfernen
            var user = await UserManager.FindByIdAsync(userId);
            if (user == null)
            {
                model.Message = "Ungültiger Link";
                return View("ChangeEMailError", model);
            }

            user.UserName = user.Remark;
            user.Email = user.Remark;
            user.ExpiryDate = null;
            user.Remark = string.Empty;
            user.Submitted = null;
            await UserManager.UpdateAsync(user);

            return View("ChangeEMailSuccess", model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="code"></param>
        /// <returns></returns>

        [AllowAnonymous]
        public async Task<ActionResult> ConfirmHmEmail(string userId, string code)
        {
            var model = new VerifyEMailModel();
            if (userId == null || code == null)
            {
                model.Message = "Ungültiger Link";
                return View("VerifyEMailError", model);
            }

            var user = await UserManager.FindByIdAsync(userId);
            if (user != null)
            {
                user.MemberState = MemberState.Student;
                await UserManager.UpdateAsync(user);

                return View("VerifyEMailSuccess", model);
            }

            model.Message = "Unbekannter Benutzer";
            return View("VerifyEMailError", model);
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult> CheckEMail(CheckEMailViewModel model)
        {
            var user = UserManager.FindById(model.UserId);
            if (user != null)
            {
                // der Benutzer hat seine E-Mail Adresse nicht verändert
                if (user.Email.ToLower().Equals(model.EMail.ToLower()))
                {
                    // es passiert nix

                }
                else
                {
                    // der Benutzer hat seine E-Mail Adresse verändert
                    // prüfen, ob es diese Adresse bereits gibt
                    var user2 = UserManager.FindByEmail(model.EMail);
                    if (user2 != null)
                    {
                        ModelState.AddModelError("EMail", "Es existiert bereits ein Benutzerkonto für diese E-Mail Adresse");
                        return View(model);
                    }
                    // neue E-Mail Adresse setzen
                    UserManager.SetEmail(model.UserId, model.EMail);
                }


                // Jetzt eine neue Mail senden
                string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);

                var mailModel = new ConfirmEmailMailModel
                {
                    User = user,
                    Token = code
                };

                new MailController().VerfiyEMail(mailModel).Deliver();

                return RedirectToAction("RegisterSuccess", model);
            }
            else
            {
                ModelState.AddModelError("EMail", "Das Benutzerkonto existiert nicht!");
                return View(model);
            }
        }



        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                // ACHTUNG:
                // die alten Benutzerkonten haben einen eigenen Benutzernamen
                // bei den neuen Benutzerkonten sind Benutzername und Email in sync!
                // d.h. wenn der Benutzer seine E-Mail Adresse ändert, so wird auch sein
                // Benutzername geändert!
                // wenn jetzt der Benutzer noch seinen Benutzernamen weiss, dann auch das prüfen
                var user = await UserManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    user = await UserManager.FindByNameAsync(model.Email);
                }

                if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return View("ForgotPasswordConfirmation");
                }

                // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                // Send an email with this link
                string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                
                //var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);		
                //await UserManager.SendEmailAsync(user.Id, "Reset Password", "Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a>");

                var mailModel = new ForgotPasswordMailModel
                {
                    User = user,
                    Token = code,
                };

                new MailController().ForgotPasswordMail(mailModel).Deliver();
                
                return RedirectToAction("ForgotPasswordConfirmation", "Account");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        [AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            return code == null ? View("Error") : View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            // hier ist nur die E-Mail adresse zulässig, an die die mail gesendet wurde
            var user = await UserManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            AddErrors(result);
            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult DeleteAccount()
        {
            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DeleteAccount(DeleteUserModel model)
        {

            var user = UserManager.FindByName(User.Identity.Name);

            if (user != null)
            {
                // Devices löschen!
                var _db = new ApplicationDbContext();
                var devices = _db.Devices.Where(d => d.User.Id.Equals(user.Id)).ToList();
                foreach (var userDevice in devices)
                {
                    _db.Devices.Remove(userDevice);
                }
                _db.SaveChanges();

                UserManager.Delete(user);

                // Mail senden
                var mailModel = new DeleteUserMailModel { User = user };
                new MailController().DeleteUserMail(mailModel).Deliver();

                // Abmelden und byebye anzeigen
                AuthenticationManager.SignOut();

                return RedirectToAction("ByeBye");
            }

            throw new Exception("Kein gültiger Benutzer");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        public ActionResult ByeBye()
        {
            return View();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="returnUrl"></param>
        /// <param name="rememberMe"></param>
        /// <returns></returns>
        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
        {
            var userId = await SignInManager.GetVerifiedUserIdAsync();
            if (userId == null)
            {
                return View("Error");
            }
            var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendCode(SendCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            // Generate the token and send it
            if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
            {
                return View("Error");
            }
            return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // Sign in the user with this external login provider if the user already has a login
            var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
                case SignInStatus.Failure:
                default:
                    // If the user does not have an account, then prompt the user to create an account
                    ViewBag.ReturnUrl = returnUrl;
                    ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                    return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Manage");
            }

            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut();
            return RedirectToAction("Index", "Home");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
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


        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }
}