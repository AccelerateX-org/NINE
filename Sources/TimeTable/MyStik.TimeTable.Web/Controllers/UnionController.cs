using System;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using MyStik.TimeTable.Data;
using MyStik.TimeTable.Web.Models;

namespace MyStik.TimeTable.Web.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public class UnionController : BaseController
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Details(Guid id)
        {
            var organiser = Db.Organisers.SingleOrDefault(org => org.Id == id);
            if (organiser == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var model = new OrganiserViewModel
            {
                Organiser = organiser,
            };

            var members = organiser.Members.OrderBy(m => m.Name);
            var myUser = UserManager.FindByName(User.Identity.Name);

            foreach (var member in members)
            {
                var itsMe = false;
                if (member.UserId != null && myUser != null)
                    itsMe = member.UserId.Equals(myUser.Id);

                model.Members.Add(new MemberViewModel
                {
                    Member = member,
                    User = member.UserId != null ? UserManager.FindById(member.UserId) : null,
                    ItsMe = itsMe,
                });
            }

            // Benutzerrechte
            ViewBag.UserRight = GetUserRight(User.Identity.Name, organiser.ShortName);

            return View(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult DeleteMember(Guid id)
        {
            var member = Db.Members.SingleOrDefault(m => m.Id == id);

            if (member != null)
            {
                var org = member.Organiser;

                org.Members.Remove(member);
                Db.Members.Remove(member);

                Db.SaveChanges();

                return RedirectToAction("Details", new { id = org.Id });
            }

            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult EditMember(Guid id)
        {
            var member = Db.Members.SingleOrDefault(m => m.Id == id);

            if (member != null)
            {
                var model = new MemberUserViewModel
                {
                    MemberId = id,
                    Role = member.Role,
                    Description = member.Description,
                    UrlProfile = member.UrlProfile,
                    Name = member.Name,
                    ShortName = member.ShortName,
                };

                if (!string.IsNullOrEmpty(member.UserId))
                {
                    var user = UserManager.FindById(member.UserId);
                    if (user != null)
                    {
                        model.UserName = user.UserName;
                    }
                }

                ViewBag.Member = member;

                return View(model);
            }

            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult EditMember(MemberUserViewModel model)
        {
            var member = Db.Members.SingleOrDefault(m => m.Id == model.MemberId);

            if (member != null)
            {
                //member.IsAdmin = model.IsAdmin;
                member.Role = model.Role;

                Db.SaveChanges();
                // Redirect zu den Members
                return RedirectToAction("Details", new { id = member.Organiser.Id });
            }

            return RedirectToAction("Index", "Students");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult GetMember(Guid id)
        {
            var organiser = Db.Organisers.SingleOrDefault(org => org.Id == id);
            if (organiser == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            // Benutzer als neues Mitglied eintragen
            var user = AppUser;
            if (user == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var member = organiser.Members.FirstOrDefault(m => m.UserId.Equals(user.Id));
            if (member != null)
            {
                return RedirectToAction("Details", new { id = id });
            }

            var prospect = new OrganiserMember
            {
                IsAdmin = false,
                Role = "Prospect",
                Name = user.FullName,
                UserId = user.Id
            };

            organiser.Members.Add(prospect);
            Db.SaveChanges();

            return RedirectToAction("Details", new { id = id });
        }
    }
}