using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Ical.Net.DataTypes;
using MyStik.TimeTable.Data;
using MyStik.TimeTable.DataServices;
using MyStik.TimeTable.DataServices.IO;
using MyStik.TimeTable.Web.Models;

namespace MyStik.TimeTable.Web.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public class ImportController : BaseController
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Index(Guid orgId, Guid semId)
        {
            var organiser = GetOrganiser(orgId);
            var semester = SemesterService.GetSemester(semId);
            if (organiser == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var model = new OrganiserViewModel
            {
                Organiser = organiser,
                Semester = semester
            };

            ViewBag.UserRight = GetUserRight(organiser);

            return View(model);
        }


        private int GetCourseCount(Guid orgId, Guid semId, string formatId)
        {
            if (string.IsNullOrEmpty(formatId))
            {
                return Db.Activities.OfType<Course>().Count(x => x.Organiser.Id == orgId && x.Semester.Id == semId && !string.IsNullOrEmpty(x.ExternalSource));
            }

            return Db.Activities.OfType<Course>().Count(x => x.Organiser.Id == orgId && x.Semester.Id == semId && x.ExternalSource.Equals(formatId));
        }

        public ActionResult Reports(Guid orgId, Guid semId)
        {
            var path = Path.GetTempPath();

            var info = Directory.GetFiles(path, "Import*");

            var model = new List<ImportReportViewModel>();

            foreach (var s in info)
            {
                var fileName = Path.GetFileName(s);
                model.Add(new ImportReportViewModel{Name=fileName});
            }


            return View(model);
        }

        public ActionResult GetReport(string id)
        {
            var path = Path.GetTempPath();

            var file = Path.Combine(path, id);

            return File(file, "text/html");
        }


        public ActionResult Delete(Guid orgId, Guid semId)
        {
            var org = GetOrganiser(orgId);
            var semester = SemesterService.GetSemester(semId);

            var model = new SemesterImportModel
            {
                Semester = semester,
                Organiser = org,
                SemesterId = semester.Id,
                OrganiserId = org.Id,
                Existing = GetCourseCount(org.Id, semester.Id, ""),
                FormatId = "JSON"
            };

            ViewBag.UserRight = GetUserRight(org);

            return View(model);
        }


    }
}