using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using MyStik.TimeTable.Data;
using MyStik.TimeTable.DataServices;
using MyStik.TimeTable.Web.Controllers;
using MyStik.TimeTable.Web.Models;

namespace MyStik.TimeTable.Web.Areas.Admin.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public class TestController : BaseController
    {

        /// <summary>
        /// 
        /// </summary>
        public ActionResult Index()
        {
            var userDb = new ApplicationDbContext();

            var users = userDb.Users.Where(x => x.UserName.StartsWith("stud")).ToList();

            var model = new List<UserAdminViewModel>();

            foreach (var user in users)
            {
                var stud = new UserAdminViewModel();
                stud.User = user;
                stud.Student = Db.Students.Where(x => x.UserId.Equals(user.Id)).OrderByDescending(x => x.Created)
                    .FirstOrDefault();

                model.Add(stud);
            }


            return View(model);
        }


        /// <summary>
        /// 
        /// </summary>

        public ActionResult CreateTestUser()
        {
            var org = Db.Organisers.SingleOrDefault(x => x.ShortName.Equals("FK 09"));
            var semester = SemesterService.GetSemester(DateTime.Today);

            var curricula = new List<Curriculum>();

            curricula.Add(org.Curricula.SingleOrDefault(x => x.ShortName.Equals("WI")));
            curricula.Add(org.Curricula.SingleOrDefault(x => x.ShortName.Equals("LM")));
            curricula.Add(org.Curricula.SingleOrDefault(x => x.ShortName.Equals("AU")));

            for (var i = 1; i <= 100; i++)
            {
                var name = $"stud{i:000}";
                var user = CreateUser(name, name, "student", MemberState.Student);

                var student = Db.Students.Where(x => x.UserId.Equals(user.Id)).OrderByDescending(x => x.Created)
                    .FirstOrDefault();

                if (student == null)
                {
                    curricula.Shuffle();

                    student = new Student();
                    student.Curriculum = curricula.First();
                    student.UserId = user.Id;
                    student.FirstSemester = semester;
                    student.Created = DateTime.Now;

                    Db.Students.Add(student);
                }

            }

            Db.SaveChanges();


            return RedirectToAction("Index");


        }


        /// <summary>
        /// 
        /// </summary>
        private ApplicationUser CreateUser(string userName, string firstName, string lastName, MemberState state)
        {
            var user = UserManager.FindByName(userName);
            if (user != null)
                return user;

            user = new ApplicationUser
            {
                Email = $"test.{userName}@acceleratex.org",
                UserName = userName,
                FirstName = firstName,
                LastName = lastName,
                MemberState = state
            };

            var result = UserManager.Create(user, "Pas1234?");
            if (result == null) throw new ArgumentNullException("result");

            user = UserManager.FindByName(userName);

            return user;
        }

    }
}