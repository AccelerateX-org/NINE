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
    public class ImportJsonController : BaseController
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

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Select(Guid orgId, Guid semId)
        {
            var org = GetOrganiser(orgId);
            var semester = SemesterService.GetSemester(semId);

            var model = new SemesterImportModel
            {
                Semester = semester,
                Organiser = org,
                SemesterId = semester.Id,
                OrganiserId = org.Id,
                Existing = GetCourseCount(org.Id, semester.Id, "JSON"),
                FormatId = "JSON"
            };


            ViewBag.Semester = Db.Semesters.OrderByDescending(c => c.StartCourses).Select(c => new SelectListItem
            {
                Text = c.Name,
                Value = c.Id.ToString(),
            });

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

            var org = GetMyOrganisation();
            model.Organiser = org;
            model.Semester = SemesterService.GetSemester(model.SemesterId);

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

            BaseImportContext ctx = null;
            if (model.FormatId.Equals("CIE"))
            {
                // Dateien prüfen
                var reader = new DataServices.IO.Cie.FileReader();
                reader.ReadFiles(tempDir);
                var importer = new DataServices.IO.Cie.SemesterImport(reader.Context, model.SemesterId);

                // Die Fakultät muss existieren
                importer.CheckFaculty();

                // Räume sollten existieren => Warnung
                // Zuordnungen zu Räumen sollten existieren => Warnung
                importer.CheckRooms();

                // Dozenten sollten existieren => Warnung
                importer.CheckLecturers();

                reader.Context.AddErrorMessage("Zusammenfassung", string.Format("Von {0} Kursen werden {1} importiert",
                    reader.Context.Model.Courses.Count, reader.Context.ValidCourses.Count), false);

                ctx = reader.Context;
            }
            else
            {
                // Dateien prüfen
                var reader = new DataServices.IO.Json.FileReader();
                reader.ReadFiles(tempDir);
                var importer = new DataServices.IO.Json.SemesterImport(reader.Context, model.SemesterId, model.OrganiserId);

                // Die Fakultät muss existieren
                importer.CheckFaculty();

                // Räume sollten existieren => Warnung
                // Zuordnungen zu Räumen sollten existieren => Warnung
                importer.CheckRooms();

                // Dozenten sollten existieren => Warnung
                importer.CheckLecturers();

                reader.Context.AddErrorMessage("Zusammenfassung", string.Format("Von {0} Kursen werden {1} importiert",
                    reader.Context.Model.Courses.Count, reader.Context.ValidCourses.Count), false);

                ctx = reader.Context;
            }

            model.Context = ctx;
            model.Organiser = GetMyOrganisation();
            model.Semester = SemesterService.GetSemester(model.SemesterId);


            return View(model);
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

        public ActionResult ChangeDozIds()
        {

            var org = GetMyOrganisation();
            var semester = SemesterService.GetSemester(DateTime.Today);

            var model = new SemesterImportModel
            {
                Semester = semester,
                Organiser = org,
                SemesterId = semester.Id,
                OrganiserId = org.Id,
                Existing = 0,
                FormatId = "dozId"
            };


            return View(model);
        }

        [HttpPost]
        public ActionResult ChangeDozIds(SemesterImportModel model)
        {
            // den IMport mit Check durchführen
            string tempDir = GetTempPath(model.OrganiserId, model.SemesterId);

            // Speichern der Config-Dateien
            if (model.AttachmentDays != null)
            {
                model.AttachmentDays.SaveAs(Path.Combine(tempDir, "dozId.csv"));
            }

            var org = GetOrganiser(model.OrganiserId);

            var importService = new MemberUpdateService(org.ShortName);

            importService.ReadFile(tempDir, "dozId.csv");

            model.Context = importService.Context;


            return View("DozIdReport", model); 
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