using Microsoft.AspNet.Identity;
using MyStik.TimeTable.Web.Controllers;
using MyStik.TimeTable.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Hangfire.Logging.LogProviders;
using log4net;
using Microsoft.AspNet.Identity.EntityFramework;

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
            // Läuft jede Stunde und prüft, ob Accounts gelöscht werden müssen
            // Idee: Lösche nur einen Anteil der Accounts, damit die DB nicht überlastet wird
            // bei 24h Laufzeit sind das dann alle Accounts => 1/24 pro Stunde = ca. 4%
            // Obergrenze bei 1% der gesamten Accounts
            // 1. Lösche alle abgelaufenen Konten
            // 2. Sende Warnungen an Konten, die in 14 Tagen ablaufen


            // Zeitgrenzen
            var today = DateTime.Today;
            var loginBorder = DateTime.Today.AddDays(-365);

            // Anteil der zu überprüfenden Konten
            var percAccount = 0.01; // 1%
            var nMaxAccounts = (int)(_db.Users.Count() * percAccount);

            var percAccountFraction = 0.08; // 8%

            // 1. Lösche alle abgelaufenen Konten

            var nExpired = _db.Users.Count(x => x.ExpiryDate.HasValue && x.ExpiryDate.Value < today);
            var nExpiredFraction = (int)(nExpired * percAccountFraction);

            // wenn es nur ganz wenige sind, dann die gleich nehmen.
            if (nExpired > 0 && nExpiredFraction == 0)
            {
                nExpiredFraction = nExpired;
            }

            var nToCheck = Math.Min(nMaxAccounts, nExpiredFraction);

            Logger.InfoFormat("Calculating size for deleting: {0} total fraction, {1} expired, {2} expired fraction, {3} final", nMaxAccounts, nExpired, nExpiredFraction, nToCheck);


            var accountsToDelete = _db.Users
                .Where(x => x.ExpiryDate.HasValue && x.ExpiryDate.Value < today)
                .OrderBy(x => x.ExpiryDate.Value)
                .Take(nToCheck)
                .Select(x => x.Id)
                .ToList();

            Logger.InfoFormat("Will check {0} account for deleting.", accountsToDelete.Count);
            foreach (var account in accountsToDelete)
            {
                DeleteUserAccount(account);
            }

            // 2. Sende Warnungen aus, an alle inaktiven, die noch nicht gewarnt wurden

            var nInActive = _db.Users.Count(x => x.LastLogin.HasValue && x.LastLogin.Value < loginBorder && x.ExpiryDate == null);
            var nWarningFraction = (int)(nInActive * percAccountFraction);
            var nToWarn = Math.Min(nMaxAccounts, nWarningFraction);

            Logger.InfoFormat("Calculating size for warning: {0} total fraction, {1} inactive, {2} inactive fraction, {3} final", nMaxAccounts, nInActive, nWarningFraction, nToWarn);

            var accountsToCheck = _db.Users
                .Where(x => x.LastLogin.HasValue && x.LastLogin.Value < loginBorder && x.ExpiryDate == null)
                .Take(nToWarn)
                .Select(x => x.Id).ToList();


            Logger.InfoFormat("Will check {0} account for warning.", accountsToCheck.Count);
            foreach (var account in accountsToCheck)
            {
                WarnUserAccount(account);
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

            // nur Accounts löschen, die expired sind
            // das ist hier die doppelte Sicherheit
            if (user.ExpiryDate.HasValue)
            {
                if (user.ExpiryDate.Value < DateTime.Today)
                {
                    try
                    {
                        var devices = _db.Devices.Where(d => d.User.Id.Equals(user.Id)).ToList();
                        foreach (var userDevice in devices)
                        {
                            _db.Devices.Remove(userDevice);
                        }

                        var userManager = new IdentifyConfig.ApplicationUserManager(new UserStore<ApplicationUser>(new ApplicationDbContext()));

                        var userClaims = userManager.GetClaims(user.Id);
                        foreach (var userClaim in userClaims)
                        {
                            userManager.RemoveClaimAsync(user.Id, userClaim);
                        }

                        _db.Users.Remove(user);

                        _db.SaveChanges();
                    }
                    catch (Exception e)
                    {
                        Logger.ErrorFormat("Account with id {0} could not delete: {1}", accountId, e.ToString());

                        var email = new AccountDeleteErrorEmail()
                        {
                            User = user,
                            To = "olav.hinz@hm.edu",
                            Message = e.ToString()
                        };

                        SendMail(email);

                        return;
                    }

                    var emailDelete = new AccountDeleteExecutionEmail()
                    {
                        User = user,
                        To = user.Email,
                    };

                    SendMail(emailDelete);

                    // Löschen weil abgelaufen
                    Logger.InfoFormat("Account with id {0} deleted", accountId);
                }
                else
                {
                    Logger.InfoFormat("Account with id {0} pending re-login", accountId);
                }
            }
            else
            {
            }

        }


        private void WarnUserAccount(string accountId)
        {
            // es wird nur noch der User gelöscht, alle Infos bleiben erhalten
            var user = _db.Users.SingleOrDefault(x => x.Id.Equals(accountId));
            if (user == null)
            {
                Logger.ErrorFormat("Account with id {0} not present - could not delete", accountId);
                return;
            }

            // zuerst die Warnung aussprechen
            if (user.ExpiryDate.HasValue)
            {
            }
            else
            {
                user.ExpiryDate = DateTime.Today.AddDays(14);
                // Warnung versenden

                var emailDelete = new AccountDeleteWarningEmail()
                {
                    User = user,
                    To = user.Email,
                };

                var success = SendMail(emailDelete);

                if (!success)
                {
                    user.ExpiryDate = DateTime.Today;
                }
                
                Logger.InfoFormat("Account with id {0} warned. Account will be deleted at {1}", accountId, user.ExpiryDate.Value.ToShortDateString());

                _db.SaveChanges();
            }

        }
    }
}