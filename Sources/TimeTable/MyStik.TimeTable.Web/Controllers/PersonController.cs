using Microsoft.Ajax.Utilities;
using MyStik.TimeTable.Data;
using MyStik.TimeTable.Data.Migrations;
using MyStik.TimeTable.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace MyStik.TimeTable.Web.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    
    public class PersonController : BaseController
    {
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

        public ActionResult Card(Guid? memberId, Guid? semId)
        {
            var semester = semId != null ? SemesterService.GetSemester(semId) : SemesterService.GetSemester(DateTime.Today);
            var nextSemester = SemesterService.GetNextSemester(semester);
            var previousSemester = SemesterService.GetPreviousSemester(semester);

            var user = GetCurrentUser();
            var userId = user.Id;

            OrganiserMember member = null;
            if (memberId != null)
            {
                member = Db.Members.SingleOrDefault(x => x.Id == memberId);
                if (member == null)
                {
                    return RedirectToAction("Index", "Home");
                }
                userId = member.UserId;
            }

            var members = Db.Members.Where(x => x.UserId.Equals(userId)).ToList();
            if (!members.Any())
            {
                return RedirectToAction("Index", "Home");    
            }

            var model = new PersonViewModel
            {
                User = GetUser(userId),
                Members = members,
            };

            FillModel(model, semester);

            ViewBag.IsSelf = user.Id.Equals(userId);
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