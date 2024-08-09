using Microsoft.AspNet.Identity;
using MyStik.TimeTable.Web.Controllers;
using MyStik.TimeTable.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Hangfire.Logging.LogProviders;
using log4net;

namespace MyStik.TimeTable.Web.Jobs
{
    public class AccountJob : BaseJob
    {
        private ApplicationDbContext _db = new ApplicationDbContext();

        public AccountJob()
        {
            Logger = LogManager.GetLogger("Account");
        }

        public void CheckUserAccounts()
        {
            var today = DateTime.Today;
            // Anteil der zu überprüfenden Konten
            var percAccount = 0.01; // 1%

            var nMaxAccounts = (int)(_db.Users.Count() * percAccount);

            var nExpired = _db.Users.Count(x => x.ExpiryDate != null && x.ExpiryDate < today);

            var nToCheck = Math.Min(nMaxAccounts, nExpired);

            var accountsToDelete = _db.Users
                .Where(x => x.ExpiryDate != null && x.ExpiryDate < today)
                .OrderBy(x => x.ExpiryDate)
                .Take(nToCheck)
                .Select(x => x.Id).ToList();

            foreach (var account in accountsToDelete)
            {
                DeleteUserAccount(account);
            }

            return;
        }

        private void DeleteUserAccount(string accountId)
        {
            // es wird nur noch der User gelöscht, alle Infos bleiben erhalten
            var user = _db.Users.SingleOrDefault(x => x.Id.Equals(accountId));
            if (user == null)
            {
                Logger.ErrorFormat("Account with id {0} not present - could not delete", accountId);
                return;
            }

            try
            {
                // Devices löschen!
                var devices = _db.Devices.Where(d => d.User.Id.Equals(user.Id)).ToList();
                foreach (var userDevice in devices)
                {
                    _db.Devices.Remove(userDevice);
                }

                _db.Users.Remove(user);

                _db.SaveChanges();
                Logger.InfoFormat("Account with id {0} deleted", accountId);
            }
            catch (Exception e)
            {
                Logger.ErrorFormat("Account with id {0} could not delete: {1}", accountId, e.ToString());
                return;
            }

            try
            {
                if (user.LastLogin == null)
                {
                    // war noe angemeldet => keine Mail
                    Logger.InfoFormat("Account with id {0} never logged in - no mail sent", accountId);
                }
                else
                {
                    if (user.ExpiryDate == null)
                    {
                        // Einfach so gelöscht
                        var emailDelete = new AccountDeleteEmail()
                        {
                            User = user,
                            To = user.Email,
                            IsExpired = false
                        };

                        SendMail(emailDelete);

                        Logger.InfoFormat("Account with id {0} deleted - mail sent to {1}", accountId, user.Email);
                    }
                    else
                    {
                        // gelöscht, weil abgelaufen
                        var emailDelete = new AccountDeleteEmail()
                        {
                            User = user,
                            To = user.Email,
                            IsExpired = true
                        };

                        SendMail(emailDelete);

                        Logger.InfoFormat("Account with id {0} deleted because expired - mail sent to {1}", accountId, user.Email);
                    }
                }

            }
            catch (Exception ex)
            {
                var msg = ex.Message;

                if (ex.InnerException != null)
                {
                    msg += " - ";
                    msg += ex.InnerException.Message;
                    if (ex.InnerException.InnerException != null)
                    {
                        msg += " - ";
                        msg += ex.InnerException.InnerException.Message;
                    }
                }

                Logger.WarnFormat("Account with id {0} deleted - mail error to {1} {2}", accountId, user.Email, msg);
            }
        }
    }
}