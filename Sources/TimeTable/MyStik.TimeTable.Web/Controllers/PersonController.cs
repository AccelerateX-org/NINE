using MyStik.TimeTable.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MyStik.TimeTable.Data;
using Microsoft.Ajax.Utilities;

namespace MyStik.TimeTable.Web.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    
    public class PersonController : BaseController
    {
        /// <summary>
        /// Alle Personen, deren Prfoli öffentlich ist
        /// </summary>
        /// <returns></returns>
        public ActionResult Self()
        {
            var user = GetCurrentUser();

            var model = new PersonViewModel
            {
                User = user,
                Members = Db.Members.Where(x => x.UserId.Equals(user.Id)).ToList()
            };
            
            FillModel(model);

            ViewBag.IsSelf = true;
            ViewBag.UserRight = GetUserRight();

            return View("Profile", model);
        }

        public ActionResult Link()
        {
            var user = GetCurrentUser();

            var model = new PersonViewModel
            {
                User = user,
                Members = Db.Members.Where(x => x.UserId.Equals(user.Id)).ToList()
            };

            ViewBag.IsSelf = true;

            return View(model);
        }

        private void FillModel(PersonViewModel model)
        {
            model.Courses = new List<Course>();
            model.OfficeHours = new List<OfficeHour>();
            model.Modules = new List<CurriculumModule>();
            foreach (var member in model.Members)
            {
                model.Courses.AddRange(
                    Db.Activities.OfType<Course>()
                        .Where(x => x.Dates.Any(d => d.Hosts.Any(h => h.Id == member.Id) && d.End >= DateTime.Today))
                        .ToList());

                model.OfficeHours.AddRange(Db.Activities.OfType<OfficeHour>()
                    .Where(x => x.Owners.Any(o => o.Member.Id == member.Id) && x.Semester.EndCourses >= DateTime.Today).ToList());

                model.Modules.AddRange(
                Db.CurriculumModules.Where(x => x.ModuleResponsibilities.Any(r => r.Member.Id == member.Id)).ToList());

            }
        }

        public ActionResult Private(Guid memberId)
        {
            var member = Db.Members.SingleOrDefault(x => x.Id == memberId);
            var user = GetCurrentUser();

            var memberUser = GetUser(member.UserId);

            if (member == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var model = new PersonViewModel
            {
                Members = memberUser != null ? Db.Members.Where(x => x.UserId.Equals(member.UserId)).ToList() : new List<OrganiserMember>() { member },
            };

            if (!string.IsNullOrEmpty(member.UserId))
            {
                model.User = GetUser(member.UserId);
            }

            FillModel(model);

            ViewBag.IsSelf = user.Id.Equals(member.UserId);
            ViewBag.UserRight = GetUserRight();

            return View("Profile", model);
        }

        [AllowAnonymous]
        public ActionResult Public(string tag)
        {
            var model = new PersonViewModel
            {
            };

            ViewBag.IsSelf = false;
            ViewBag.UserRight = GetUserRight();

            return View("Profile", model);
        }

        public ActionResult GetProfileImage(string id)
        {
            var user = GetUser(id);
            return File(user.BinaryData, user.FileType);
        }

    }
}