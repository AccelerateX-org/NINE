using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MyStik.TimeTable.Data;
using MyStik.TimeTable.Web.Models;

namespace MyStik.TimeTable.Web.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public class AdminDashboardController : BaseController
    {
        DateTime today = GlobalSettings.Today;
        DateTime tomorrow = GlobalSettings.Today.AddDays(1);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            var model = GetCoursesToday(false);
            GetBadges();
            return View(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>

        public ActionResult ActiveCoursesToday()
        {
            var model = GetCoursesToday(false);
            GetBadges();
            return View(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult CanceledCoursesToday()
        {
            var model = GetCoursesToday(true);
            GetBadges();
            return View(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult ActiveOfficeHoursToday()
        {
            var model = GetOfficeHoursToday(false);
            GetBadges();
            return View(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult CanceledOfficeHoursToday()
        {
            var model = GetOfficeHoursToday(true);
            GetBadges();
            return View(model);
        }
        /// <summary>
        /// 
        /// </summary>
        private void GetBadges()
        {
            var datesToday = Db.ActivityDates.Where(d => 
                (d.Begin >= today && d.End <= tomorrow)
                ).ToList();

            var coursesActive = 0;
            var coursesCanceled = 0;
            var officeHoursActive = 0;
            var officeHoursCanceled = 0;


            foreach (var date in datesToday)
            {
                if (date.Activity is Course)
                {
                    if (date.Occurrence.IsCanceled)
                    {
                        coursesCanceled++;
                    }
                    else
                    {
                        coursesActive++;
                    }
                }
                else if (date.Activity is OfficeHour)
                {
                    if (date.Occurrence.IsCanceled)
                    {
                        officeHoursCanceled++;
                    }
                    else
                    {
                        officeHoursActive++;
                    }
                }
            }

            ViewBag.CoursesCanceled = coursesCanceled;
            ViewBag.CoursesActive = coursesActive;
            ViewBag.OfficeHourCanceled = officeHoursCanceled;
            ViewBag.OfficeHourActive = officeHoursActive;

        }

        private List<CourseDateInfoModel> GetCoursesToday(bool isCanceled)
        {

            var datesToday = Db.ActivityDates.Where(d =>
                (d.Begin >= today && d.End <= tomorrow)
                ).ToList();

            var list = new List<CourseDateInfoModel>();

            foreach (var date in datesToday)
            {
                if (date.Activity is Course && date.Occurrence.IsCanceled == isCanceled)
                {
                    list.Add(
                        new CourseDateInfoModel()
                        {
                            Course = date.Activity as Course,
                            Date = date,
                        });
                }
            }

            list = list.OrderBy(m => m.Date.Begin).ToList();

            return list;
        }

        private List<OfficeHourDateViewModel> GetOfficeHoursToday(bool isCanceled)
        {

            var datesToday = Db.ActivityDates.Where(d =>
                (d.Begin >= today && d.End <= tomorrow)
                ).ToList();

            var list = new List<OfficeHourDateViewModel>();

            foreach (var date in datesToday)
            {
                if (date.Activity is OfficeHour && date.Occurrence.IsCanceled == isCanceled)
                {
                    list.Add(
                        new OfficeHourDateViewModel()
                        {
                            OfficeHour = date.Activity as OfficeHour,
                            Date = date,
                        });
                }
            }

            list = list.OrderBy(m => m.Date.Begin).ToList();

            return list;
        }

    }
}