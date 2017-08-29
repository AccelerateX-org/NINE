using System.Linq;
using System.Web.Mvc;
using MyStik.TimeTable.Web.Controllers;

namespace MyStik.TimeTable.Web.Areas.Admin.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public class ModuleController : BaseController
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult DeleteCatalog()
        {
            // zuerst alle Kurspläne löschen
            var allPlans = Db.CoursePlans.ToList();

            foreach (var plan in allPlans)
            {
                var allMappings = plan.ModuleMappings.ToList();
                foreach (var mapping in allMappings)
                {
                    var allTrials = mapping.Trials.ToList();
                    foreach (var trial in allTrials)
                    {
                        Db.ModuleTrials.Remove(trial);
                    }
                    Db.ModuleMappings.Remove(mapping);
                }

                var allSemester = plan.Semester.ToList();
                foreach (var semester in allSemester)
                {
                    plan.Semester.Remove(semester);
                }

                Db.CoursePlans.Remove(plan);
            }
            Db.SaveChanges();

            var allModules = Db.CurriculumModules.ToList();

            foreach (var module in allModules)
            {
                var courses = module.ModuleCourses.ToList();
                foreach (var moduleCourse in courses)
                {
                    var c2 = moduleCourse.Courses.ToList();
                    foreach (var course in c2)
                    {
                        Db.Activities.Remove(course);
                    }

                    Db.ModuleCourses.Remove(moduleCourse);
                }

                Db.CurriculumModules.Remove(module);

            }

            Db.SaveChanges();


            return RedirectToAction("Index", "Home");
        }
    }
}