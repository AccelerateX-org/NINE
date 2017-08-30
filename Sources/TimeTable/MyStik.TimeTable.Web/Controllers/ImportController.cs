using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MyStik.TimeTable.Data;
using MyStik.TimeTable.DataServices.Json;
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
        public ActionResult Index()
        {
            var organiser = GetMyOrganisation();
            if (organiser == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var model = new OrganiserViewModel
            {
                Organiser = organiser,
                Semester = GetSemester()
            };

            ViewBag.UserRight = GetUserRight();

            return View(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Select()
        {
            var org = GetMyOrganisation();
            var semester = GetSemester();

            var model = new SemesterImportModel
            {
                Semester = semester,
                Organiser = org,
                SemesterId = semester.Id,
                OrganiserId = org.Id,
                Existing = GetCourseCount(org.Id, semester.Id)
            };


            ViewBag.Semester = Db.Semesters.OrderByDescending(c => c.StartCourses).Select(c => new SelectListItem
            {
                Text = c.Name,
                Value = c.Id.ToString(),
            });

            return View(model);
        }

        private int GetCourseCount(Guid orgId, Guid semId)
        {
            var nInSemester = Db.Activities.OfType<Course>().Count(c =>
                c.Organiser.Id == orgId &&
                c.SemesterGroups.Any(g =>
                    g.Semester.Id == semId &&
                    g.CapacityGroup.CurriculumGroup.Curriculum.Organiser.Id == orgId
                ) &&
                (!string.IsNullOrEmpty(c.ExternalSource) && c.ExternalSource.Equals("JSON")));

            var nNoGroup = Db.Activities.OfType<Course>().Count(c =>
                c.Organiser.Id == orgId &&
                !string.IsNullOrEmpty(c.ExternalSource) &&
                c.ExternalSource.Equals("JSON") &&
                !c.SemesterGroups.Any());

            return nInSemester + nNoGroup;
        }

        private string GetTempPath(Guid orgId, Guid semId)
        {
            string tempDir = Path.GetTempPath();

            var semester = Db.Semesters.SingleOrDefault(x => x.Id == semId);
            var org = Db.Organisers.SingleOrDefault(x => x.Id == orgId);

            if (semester != null && org != null)
            {
                tempDir = Path.Combine(tempDir, semester.Name);
                tempDir = Path.Combine(tempDir, org.ShortName);

                if (!Directory.Exists(tempDir))
                {
                    Directory.CreateDirectory(tempDir);
                }
            }

            return tempDir;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SelectFiles(SemesterImportModel model)
        {
            // Die Dateien löschen
            string tempDir = GetTempPath(model.OrganiserId, model.SemesterId);

            Directory.Delete(tempDir, true);

            return View(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Upload(SemesterImportModel model)
        {
            string tempDir = GetTempPath(model.OrganiserId, model.SemesterId);

            // Speichern der Config-Dateien
            if (model.AttachmentDays != null)
            {
                model.AttachmentDays.SaveAs(Path.Combine(tempDir, "import.json"));
            }

            // Dateien prüfen
            var reader = new FileReader();
            
            reader.ReadFiles(tempDir);

            var importer = new SemesterImport(reader.Context, model.SemesterId, model.OrganiserId);

            // Die Fakultät muss existieren
            importer.CheckFaculty();

            // Räume sollten existieren => Warnung
            // Zuordnungen zu Räumen sollten existieren => Warnung
            importer.CheckRooms();

            // Dozenten sollten existieren => Warnung
            importer.CheckLecturers();

            reader.Context.AddErrorMessage("Zusammenfassung", string.Format("Von {0} Kursen werden {1} importiert",
                reader.Context.Model.Courses.Count, reader.Context.ValidCourses.Count), false);

            model.Context = reader.Context;

            model.Organiser = GetMyOrganisation();
            model.Semester = GetSemester(model.SemesterId);


            return View(model);
        }


    }
}