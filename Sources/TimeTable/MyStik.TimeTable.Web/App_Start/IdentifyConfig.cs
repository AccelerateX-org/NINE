using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using MyStik.TimeTable.Web.Models;

namespace MyStik.TimeTable.Web
{
    /// <summary>
    /// 
    /// </summary>
    public class IdentifyConfig
    {
        /// <summary>
        /// 
        /// </summary>
        public class EmailService : IIdentityMessageService
        {
            /// <summary>
            /// 
            /// </summary>
            /// <param name="message"></param>
            /// <returns></returns>
            public Task SendAsync(IdentityMessage message)
            {
                return Task.FromResult(0);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public class SmsService : IIdentityMessageService
        {
            /// <summary>
            /// 
            /// </summary>
            /// <param name="message"></param>
            /// <returns></returns>
            public Task SendAsync(IdentityMessage message)
            {
                // Plug in your SMS service here to send a text message.
                return Task.FromResult(0);
            }
        }

        /// <summary>
        /// Configure the application user manager used in this application. UserManager is defined in ASP.NET Identity and is used by the application.
        /// </summary>
        public class ApplicationUserManager : UserManager<ApplicationUser>
        {
            /// <summary>
            /// 
            /// </summary>
            /// <param name="store"></param>
            public ApplicationUserManager(IUserStore<ApplicationUser> store)
                : base(store)
            {
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="options"></param>
            /// <param name="context"></param>
            /// <returns></returns>
            public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options,
                IOwinContext context)
            {
                var manager =
                    new ApplicationUserManager(new UserStore<ApplicationUser>(context.Get<ApplicationDbContext>()));
                // Configure validation logic for usernames
                manager.UserValidator = new UserValidator<ApplicationUser>(manager)
                {
                    AllowOnlyAlphanumericUserNames = false,
                    RequireUniqueEmail = true
                };

                // Configure validation logic for passwords
                manager.PasswordValidator = new PasswordValidator
                {
                    RequiredLength = 6,
                    RequireNonLetterOrDigit = true,
                    RequireDigit = true,
                    RequireLowercase = true,
                    RequireUppercase = true,
                };

                // Configure user lockout defaults
                manager.UserLockoutEnabledByDefault = true;
                manager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
                manager.MaxFailedAccessAttemptsBeforeLockout = 5;

                // Register two factor authentication providers. This application uses Phone and Emails as a step of receiving a code for verifying the user
                // You can write your own provider and plug it in here.
                manager.RegisterTwoFactorProvider("Phone Code", new PhoneNumberTokenProvider<ApplicationUser>
                {
                    MessageFormat = "Your security code is {0}"
                });
                manager.RegisterTwoFactorProvider("Email Code", new EmailTokenProvider<ApplicationUser>
                {
                    Subject = "Security Code",
                    BodyFormat = "Your security code is {0}"
                });
                manager.EmailService = new EmailService();
                manager.SmsService = new SmsService();
                var dataProtectionProvider = options.DataProtectionProvider;
                if (dataProtectionProvider != null)
                {
                    manager.UserTokenProvider =
                        new DataProtectorTokenProvider<ApplicationUser>(dataProtectionProvider.Create("ASP.NET Identity"));
                }
                return manager;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="userId"></param>
            /// <returns></returns>
            public async Task<DateTime> StoreLogInAsync(string userId)
            {
                var user = await FindByIdAsync(userId);
                var now = GlobalSettings.Now;
                user.LastLogin = now;

                // in jedem Fall wieder auf aktiv für E-Mail setzen
                user.IsApproved = true;
                
                // Falls ein Ablaufdatum vorhanden
                // => zurücksetzen
                if (user.ExpiryDate.HasValue)
                    user.ExpiryDate = null;

                // Falls eine Vorwarnung vorhanden
                // => zurücksetzen
                if (user.Approved.HasValue)
                    user.Approved = null;
                
                await this.Store.UpdateAsync(user);
                
                return now;
            }
        }

        /// <summary>
        /// Configure the application sign-in manager which is used in this application.
        /// </summary>
        public class ApplicationSignInManager : SignInManager<ApplicationUser, string>
        {
            /// <summary>
            /// 
            /// </summary>
            /// <param name="userManager"></param>
            /// <param name="authenticationManager"></param>
            public ApplicationSignInManager(ApplicationUserManager userManager,
                IAuthenticationManager authenticationManager)
                : base(userManager, authenticationManager)
            {
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="user"></param>
            /// <returns></returns>
            public override Task<ClaimsIdentity> CreateUserIdentityAsync(ApplicationUser user)
            {
                return user.GenerateUserIdentityAsync((ApplicationUserManager) UserManager);
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="options"></param>
            /// <param name="context"></param>
            /// <returns></returns>
            public static ApplicationSignInManager Create(IdentityFactoryOptions<ApplicationSignInManager> options,
                IOwinContext context)
            {
                return new ApplicationSignInManager(context.GetUserManager<ApplicationUserManager>(),
                    context.Authentication);
            }
        }
    }

}