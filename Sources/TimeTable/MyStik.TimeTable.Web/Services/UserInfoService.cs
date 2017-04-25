using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MyStik.TimeTable.Web.Models;

namespace MyStik.TimeTable.Web.Services
{
    public class UserInfoService
    {
        private ApplicationDbContext _db = new ApplicationDbContext();

        public string GetUserName(string userId)
        {
            var user = _db.Users.SingleOrDefault(u => u.Id.Equals(userId));
            if (user != null)
            {
                return string.Format("{0} {1}", user.FirstName, user.LastName);
            }

            return "unbekannt";
        }
    }
}