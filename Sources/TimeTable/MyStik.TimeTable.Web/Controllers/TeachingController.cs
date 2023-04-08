using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MyStik.TimeTable.Data;
using MyStik.TimeTable.Web.Models;
using MyStik.TimeTable.Web.Services;

namespace MyStik.TimeTable.Web.Controllers
{
    public class TeachingController : BaseController
    {
        // GET: Teaching
        public ActionResult Index(Guid? id)
        {
            var user = GetCurrentUser();
            var members = MemberService.GetFacultyMemberships(user.Id);
            if (!members.Any())
                return RedirectToAction("Apply");

            Semester currentSemester = null;

            if (id == null)
            {
                // das aktuelle Semester bestimmen es gilt das neues aller Semester in alle
                // Fakultäen
                var semesterToday = SemesterService.GetSemester(DateTime.Today);
                var mySemester = semesterToday;
                foreach (var organiserMember in members)
                {
                    var latestSemester = SemesterService.GetLatestSemester(organiserMember.Organiser);

                    if (latestSemester.StartCourses > semesterToday.StartCourses)
                    {
                        mySemester = latestSemester;
                    }
                }

                // gegencheck: wenn ich noch termine im aktuellen Semester habe, dann nimm das
                if (mySemester != semesterToday)
                {

                }


                currentSemester = mySemester;
            }
            else
            {
                currentSemester = SemesterService.GetSemester(id);
            }

            var TeachingService = new TeachingService(Db);
            var userService = new UserInfoService();

            var model = new TeachingOverviewModel();

            model.CurrentSemester = TeachingService.GetActivities(currentSemester, user, members);
            model.PrevSemester = SemesterService.GetPreviousSemester(currentSemester);
            model.NextSemester = SemesterService.GetNextSemester(currentSemester);
            model.Members = members.ToList();


            // Abschlussarbeiten
            model.ActiveTheses = new List<ThesisStateModel>();

            var theses = TeachingService.GetActiveTheses(user);

            foreach (var thesis in theses)
            {
                var tm = new ThesisStateModel
                {
                    Thesis = thesis,
                    Student = thesis.Student,
                    User = userService.GetUser(thesis.Student.UserId)
                };

                model.ActiveTheses.Add(tm);
            }

            model.Modules = Db.CurriculumModules
                .Where(x => x.ModuleResponsibilities.Any(m =>
                    !string.IsNullOrEmpty(m.Member.UserId) && m.Member.UserId.Equals(user.Id)))
                .ToList();

            return View(model);
        }

        public ActionResult Apply()
        {
            return View();
        }
    }
}


