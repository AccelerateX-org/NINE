using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using MyStik.TimeTable.Data;
using MyStik.TimeTable.DataServices;
using MyStik.TimeTable.Web.Models;
using MyStik.TimeTable.Web.Services;

namespace MyStik.TimeTable.Web.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public class OrganiserMembersController : BaseController
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            var organiser = GetMyOrganisation();
            if (organiser == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var semester = GetSemester();
            
            var model = new OrganiserViewModel
            {
                Organiser = organiser,
                Semester = semester
            };

            var memberPage = organiser.Members.OrderBy(m => m.Name);

            var myUser = UserManager.FindByName(User.Identity.Name);
         
            var vorSemester = new SemesterService().GetSemester(semester, 1);

            var courseService = new CourseService(UserManager);

            foreach (var member in memberPage)
            {
                var itsMe = false;
                if (member.UserId != null && myUser != null)
                    itsMe = member.UserId.Equals(myUser.Id);

                model.Members.Add(new MemberViewModel
                {
                    Member = member,
                    //User = member.UserId != null ? UserManager.FindById(member.UserId) : null,
                    ItsMe = itsMe,
                    //IsActive = courseService.IsActive(member, semester),
                    //WasActiveLastSemester = courseService.IsActive(member, vorSemester),
                });
            }

            // Benutzerrechte
            ViewBag.UserRight = GetUserRight(User.Identity.Name, organiser.ShortName);
            ViewBag.ThisSemester = semester.Name;
            ViewBag.LastSemester = vorSemester.Name;


            return View(model);

        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Active()
        {
            var organiser = GetMyOrganisation();
            if (organiser == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var semester = GetSemester();

            var model = new OrganiserViewModel
            {
                Organiser = organiser,
                Semester = semester
            };

            var memberPage = organiser.Members.OrderBy(m => m.Name);

            var myUser = UserManager.FindByName(User.Identity.Name);

            var vorSemester = new SemesterService().GetSemester(semester, 1);

            var courseService = new CourseService(UserManager);

            foreach (var member in memberPage)
            {
                var itsMe = false;
                if (member.UserId != null && myUser != null)
                    itsMe = member.UserId.Equals(myUser.Id);

                model.Members.Add(new MemberViewModel
                {
                    Member = member,
                    //User = member.UserId != null ? UserManager.FindById(member.UserId) : null,
                    ItsMe = itsMe,
                    IsActive = courseService.IsActive(member, semester),
                    WasActiveLastSemester = courseService.IsActive(member, vorSemester),
                });
            }

            // Benutzerrechte
            ViewBag.UserRight = GetUserRight(User.Identity.Name, organiser.ShortName);
            ViewBag.ThisSemester = semester.Name;
            ViewBag.LastSemester = vorSemester.Name;


            return View(model);

        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Today()
        {
            var organiser = GetMyOrganisation();
            if (organiser == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var semester = GetSemester();

            var model = new OrganiserViewModel
            {
                Organiser = organiser,
                Semester = semester
            };

            var day = DateTime.Today;
            var nextDay = day.AddDays(1);

            var memberPage = organiser.Members.Where(x => x.Dates.Any(d => d.Begin >= day && d.End < nextDay)).OrderBy(m => m.Name);

            var vorSemester = new SemesterService().GetSemester(semester, 1);


            foreach (var member in memberPage)
            {
                model.Members.Add(new MemberViewModel
                {
                    Member = member,
                    ItsMe = false,
                    IsActive = true,
                });
            }

            // Benutzerrechte
            ViewBag.UserRight = GetUserRight(User.Identity.Name, organiser.ShortName);
            ViewBag.ThisSemester = semester.Name;
            ViewBag.LastSemester = vorSemester.Name;


            return View(model);

        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OrganiserMember organiserMember = Db.Members.Find(id);
            if (organiserMember == null)
            {
                return HttpNotFound();
            }
            return View(organiserMember);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="organiserMember"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,UserId,ShortName,Name,Role,Description,IsAdmin,UrlProfile")] OrganiserMember organiserMember)
        {
            if (ModelState.IsValid)
            {
                organiserMember.Id = Guid.NewGuid();
                Db.Members.Add(organiserMember);
                Db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(organiserMember);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OrganiserMember organiserMember = Db.Members.Find(id);
            if (organiserMember == null)
            {
                return HttpNotFound();
            }
            return View(organiserMember);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="organiserMember"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,UserId,ShortName,Name,Role,Description,IsAdmin,UrlProfile")] OrganiserMember organiserMember)
        {
            if (ModelState.IsValid)
            {
                Db.Entry(organiserMember).State = EntityState.Modified;
                Db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(organiserMember);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OrganiserMember organiserMember = Db.Members.Find(id);
            if (organiserMember == null)
            {
                return HttpNotFound();
            }
            return View(organiserMember);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            OrganiserMember organiserMember = Db.Members.Find(id);
            Db.Members.Remove(organiserMember);
            Db.SaveChanges();
            return RedirectToAction("Index");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
