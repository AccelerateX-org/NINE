using System.Collections.Generic;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using MyStik.TimeTable.Web.Models;

namespace MyStik.TimeTable.Web.Migrations
{
    using System.Data.Entity.Migrations;

    internal sealed class Configuration : DbMigrationsConfiguration<MyStik.TimeTable.Web.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            ContextKey = "MyStik.TimeTable.Web.Models.ApplicationDbContext";
        }

        private UserManager<ApplicationUser> UserManager { get; set; }
        private RoleManager<IdentityRole> RoleManager { get; set; }

        private ApplicationDbContext Db { get; set; }

        protected override void Seed(MyStik.TimeTable.Web.Models.ApplicationDbContext context)
        {
            Db = context;
            UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
            RoleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));

            RoleManager.Create(new IdentityRole("SysAdmin"));

            CreateUser("admin", "", "", new string[] { "SysAdmin" });
        }


        private ApplicationUser CreateUser(string userName, string firstName, string lastName, IEnumerable<string> roles)
        {
            var user = new ApplicationUser
            {
                UserName = userName,
                FirstName = firstName,
                LastName = lastName,
            };
            UserManager.Create(user, "Pas1234?");
            user = UserManager.FindByName(user.UserName);

            foreach (var role in roles)
            {
                UserManager.AddToRole(user.Id, role);
            }

            return user;
        }
    }
}
