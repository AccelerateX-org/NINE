using System;
using System.Linq;
using System.Web.Mvc;
using MyStik.TimeTable.Web.Models;

namespace MyStik.TimeTable.Web.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public class SemesterController : BaseController
    {
        //
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            ViewBag.SemesterList = Db.Semesters.OrderByDescending(s => s.StartCourses).Select(f => new SelectListItem
            {
                Text = f.Name,
                Value = f.Name,
            });

            var org = GetMyOrganisation();


            ViewBag.UserRight = GetUserRight(org);

            var model = new SemesterViewModel
            {
                Semester = SemesterService.GetSemester(DateTime.Today)
            };

            return View(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="semGroupId"></param>
        /// <returns></returns>
        [HttpPost]
        public PartialViewResult DateList(string semGroupId)
        {
            var model = Db.Semesters.SingleOrDefault(s => s.Name.Equals(semGroupId));

            if (model == null)
            {
                model = Db.Semesters.FirstOrDefault();
            }

            ViewBag.UserRight = GetUserRight();

            return PartialView("_DateList", model);
        }

        public ActionResult Group(Guid id)
        {
            var semGroup = Db.SemesterGroups.SingleOrDefault(x => x.Id == id);

            return RedirectToAction("CapacityGroup", "Planer",
                new {semId = semGroup.Semester.Id, groupId = semGroup.CapacityGroup.Id});
        }
    }
}