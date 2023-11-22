using System;
using System.Linq;
using System.Web.Mvc;
using MyStik.TimeTable.Data;

namespace MyStik.TimeTable.Web.Controllers
{
    [AllowAnonymous]
    public class UniversityController : BaseController
    {
        // GET: University
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Campus()
        {
            var user = GetCurrentUser();
            if (user != null)
            {
                var org = Db.Organisers.FirstOrDefault(x => x.Members.Any(m => m.UserId.Equals(user.Id) && m.IsAdmin == true));
                if (org != null)
                {
                    ViewBag.UserRight = GetUserRight(org);
                }
            }

            return View();
        }


        public ActionResult Faculty(Guid id)
        {
            var org = Db.Organisers.SingleOrDefault(x => x.Id == id);
            var user = GetCurrentUser();
            if (user != null)
            {
                ViewBag.UserRight = GetUserRight(org);
            }

            return View(org);
        }

        public ActionResult Faculties()
        {
            var orgs = Db.Organisers
                .OrderBy(x => x.Name)
                .ToList();

            return View(orgs);
        }

        public ActionResult Services()
        {
            var orgs = Db.Organisers
                .OrderBy(x => x.Name)
                .ToList();

            return View(orgs);
        }

        public ActionResult Curricula()
        {
            return View();
        }

        public ActionResult Degree(Guid id)
        {
            var degree = Db.Degrees.SingleOrDefault(x => x.Id == id);
            var model = Db.Curricula.Where(x => x.Degree != null && x.Degree.Id == id).ToList();

            if (Db.Degrees.Count() <= 2)
            {
                // Bachelor of Arts | B. A.
                // Bachelor of Engineering | B. Eng.

                // Master of Science | M. Sc.
                // Master of Engineering | M. Eng
                // Master of Business Administration and Engineering | MBA & Eng
                // Master of Arts in Business Administration | MBA
                // Master of Business Administration | MBA


                var b1 = new Degree { Name = "Bachelor of Arts", ShortName = "B. A.", IsUndergraduate = true, IsCertificate = false, IsPhD = false};
                var b2 = new Degree { Name = "Bachelor of Engineering", ShortName = "B. Eng.", IsUndergraduate = true, IsCertificate = false, IsPhD = false };

                var m1 = new Degree { Name = "Master of Science", ShortName = "M. Sc.", IsUndergraduate = false, IsCertificate = false, IsPhD = false };
                var m2 = new Degree { Name = "Master of Engineering", ShortName = "M. Eng.", IsUndergraduate = false, IsCertificate = false, IsPhD = false };
                var m3 = new Degree { Name = "Master of Business Administration and Engineering", ShortName = "MBA & Eng.", IsUndergraduate = false, IsCertificate = false, IsPhD = false };
                var m4 = new Degree { Name = "Master of Arts in Business Administration", ShortName = "MBA", IsUndergraduate = false, IsCertificate = false, IsPhD = false };
                var m5 = new Degree { Name = "Master of Business Administration", ShortName = "MBA", IsUndergraduate = false, IsCertificate = false, IsPhD = false };


                Db.Degrees.Add(b1);
                Db.Degrees.Add(b2);
                Db.Degrees.Add(m1);
                Db.Degrees.Add(m2);
                Db.Degrees.Add(m3);
                Db.Degrees.Add(m4);
                Db.Degrees.Add(m5);

                Db.SaveChanges();
            }

            ViewBag.Degree = degree;

            return View(model);
        }


        public ActionResult Persons()
        {

            return View();
        }


    }
}