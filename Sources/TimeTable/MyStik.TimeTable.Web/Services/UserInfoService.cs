using System.Linq;
using MyStik.TimeTable.Web.Models;

namespace MyStik.TimeTable.Web.Services
{
    /// <summary>
    /// 
    /// </summary>
    public class UserInfoService
    {
        private ApplicationDbContext _db = new ApplicationDbContext();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public string GetUserName(string userId)
        {
            var user = _db.Users.SingleOrDefault(u => u.Id.Equals(userId));
            if (user != null)
            {
                return string.Format("{0} {1}", user.FirstName, user.LastName);
            }

            return "unbekannt";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public ApplicationUser GetUser(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                return null;

            return _db.Users.SingleOrDefault(u => u.Id.Equals(userId));
        }

        public ApplicationUser GetUserByEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
                return null;

            return _db.Users.SingleOrDefault(x => x.Email.ToLower().Equals(email.ToLower()));
        }
    }
}