using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Ajax.Utilities;
using MyStik.TimeTable.Data;
using MyStik.TimeTable.Web.Models;
using MyStik.TimeTable.Web.Services;

namespace MyStik.TimeTable.Web.Controllers
{
    public class TeachingController : BaseController
    {
        // GET: Teaching
        public ActionResult Index()
        {
            var user = GetCurrentUser();

            // Die beiden aktuellen Semester holen
            // Ausgehend von heute ist das nächste aktive, falls es bereits irgendwo Semestergruppen gibt
            var semesterToday= SemesterService.GetSemester(DateTime.Today);
            var nextSemester = SemesterService.GetNextSemester(semesterToday);

            var isActive = SemesterService.IsActive(nextSemester);

            var currentSemester = semesterToday;


            var TeachingService = new TeachingService(Db);
            var MemberService = new MemberService(Db, UserManager);
            var userService = new UserInfoService();

            var model = new TeachingOverviewModel();

            model.CurrentSemester = TeachingService.GetActivities(currentSemester, user);
            if (isActive)
            {
                model.PlaningSemester = TeachingService.GetActivities(nextSemester, user);
            }
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



            model.Members = MemberService.GetMemberships(user);

            return View(model);
        }


    }
}