using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MyStik.TimeTable.Data;
using MyStik.TimeTable.Web.Models;
using MyStik.TimeTable.Web.Services;

namespace MyStik.TimeTable.Web.Controllers
{
    public class ActivityDateController : BaseController
    {

        public ActionResult Details(Guid id)
        {
            var org = GetMyOrganisation();
            var date = Db.ActivityDates.SingleOrDefault(x => x.Id == id);
            var course = date.Activity as Course;

            var courseService = new CourseService(Db);

            var model = new CourseDateInfoModel
            {
                Course = course,
                Summary = courseService.GetCourseSummary(course),
                Organiser = org
            };


            var userRight = GetUserRight(User.Identity.Name, model.Course);
            ViewBag.UserRight = userRight;

            return View(model);
        }
    }
}