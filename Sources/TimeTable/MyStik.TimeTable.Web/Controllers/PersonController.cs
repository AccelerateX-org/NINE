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
            var member = GetMyMembership();

            return RedirectToAction("Card", new {memberId = member.Id});
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

        private void FillModel(PersonViewModel model, Semester semester)
        {
            model.Courses = new List<Course>();
            model.OfficeHours = new List<OfficeHour>();
            model.Modules = new List<CurriculumModule>();
            foreach (var member in model.Members)
            {
                model.Courses.AddRange(
                    Db.Activities.OfType<Course>()
                        .Where(x => 
                            x.Semester != null && x.Semester.Id == semester.Id &&
                            x.Dates.Any(d => d.Hosts.Any(h => h.Id == member.Id)))
                        .ToList());

                model.OfficeHours.AddRange(Db.Activities.OfType<OfficeHour>()
                    .Where(x => x.Owners.Any(o => o.Member.Id == member.Id) && x.Semester.Id == semester.Id).ToList());

                model.Modules.AddRange(
                Db.CurriculumModules.Where(x => x.ModuleResponsibilities.Any(r => r.Member.Id == member.Id)).ToList());

            }
        }

        public ActionResult Private(Guid memberId)
        {
            return RedirectToAction("Card", new { memberId = memberId });
        }


        public ActionResult Card(Guid memberId, Guid? semId)
        {
            var semester = semId != null ? SemesterService.GetSemester(semId) : SemesterService.GetSemester(DateTime.Today);
            var nextSemester = SemesterService.GetNextSemester(semester);
            var previousSemester = SemesterService.GetPreviousSemester(semester);


            var member = Db.Members.SingleOrDefault(x => x.Id == memberId);
            var user = GetCurrentUser();

            if (member == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var memberUser = GetUser(member.UserId);

            var model = new PersonViewModel
            {
                Members = memberUser != null ? Db.Members.Where(x => x.UserId.Equals(member.UserId)).ToList() : new List<OrganiserMember>() { member },
            };

            if (!string.IsNullOrEmpty(member.UserId))
            {
                model.User = GetUser(member.UserId);
            }

            FillModel(model, semester);

            ViewBag.IsSelf = user.Id.Equals(member.UserId);
            ViewBag.UserRight = GetUserRight();
            ViewBag.PrevSemester = previousSemester;
            ViewBag.CurrentSemester = semester;
            ViewBag.NextSemester = nextSemester;

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