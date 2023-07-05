using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web.Mvc;
using MyStik.TimeTable.Data;
using MyStik.TimeTable.DataServices.IO.GpUntis;
using MyStik.TimeTable.Web.Models;

namespace MyStik.TimeTable.Web.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public class UntisController : BaseController
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Index(Guid id)
        {
            var org = GetMyOrganisation();
            var semester = SemesterService.GetSemester(id);

            var model = new SemesterImportModel
            {
                Semester = semester,
                FirstDate = semester.StartCourses.ToShortDateString(),
                LastDate = semester.EndCourses.ToShortDateString(),
                Organiser = org,
                SemesterId = semester.Id,
                OrganiserId = org.Id,
                Existing = GetCourseCount(org.Id, semester.Id)
            };

            /*
            ViewBag.Semester = Db.Semesters
                .Where(x => x.EndCourses >= DateTime.Today && x.Groups.Any())
                .OrderBy(s => s.StartCourses)
                .Select(c => new SelectListItem
                {
                    Text = c.Name,
                    Value = c.Id.ToString(),
                });
            */

            var culture = Thread.CurrentThread.CurrentUICulture;
            ViewBag.Culture = culture;


            return View(model);
        }

        [HttpPost]
        public JsonResult GetCourseStat(Guid orgId, Guid semId)
        {
            var n = GetCourseCount(orgId, semId);

            return Json(new { nCourses = n });
        }

        private int GetCourseCount(Guid orgId, Guid semId)
        {
            var nInSemester = Db.Activities.OfType<Course>().Count(c =>
                c.Organiser.Id == orgId &&
                c.SemesterGroups.Any(g => 
                    g.Semester.Id == semId && 
                    g.CapacityGroup.CurriculumGroup.Curriculum.Organiser.Id == orgId
                ) &&
                (!string.IsNullOrEmpty(c.ExternalSource) && c.ExternalSource.Equals("GPUNTIS")));

            var nNoGroup = Db.Activities.OfType<Course>().Count(c =>
                c.Organiser.Id == orgId &&
                !string.IsNullOrEmpty(c.ExternalSource) && 
                c.ExternalSource.Equals("GPUNTIS") &&
                !c.SemesterGroups.Any());

            return nInSemester; // + nNoGroup;
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

            model.BeginImport = DateTime.Parse(model.FirstDate);
            model.EndImport = DateTime.Parse(model.LastDate);

            model.FirstDateYYYYMMDD = model.BeginImport.ToString("yyyyMMdd");
            model.LastDateYYYYMMDD = model.EndImport.ToString("yyyyMMdd");

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

            // Speichern der Untis Dateien
            foreach (var attachment in model.Attachments)
            {
                if (attachment != null)
                {
                    var fileName = Path.GetFileName(attachment.FileName);

                    var tempFileName = Path.Combine(tempDir, fileName);

                    attachment.SaveAs(tempFileName);
                }
            }

            // Speichern der Config-Dateien
            model.AttachmentDays?.SaveAs(Path.Combine(tempDir, "configDays.txt"));
            model.AttachmentHours?.SaveAs(Path.Combine(tempDir, "configHours.txt"));
            model.AttachmentGroups?.SaveAs(Path.Combine(tempDir, "configGroups.txt"));

            // Dateien prüfen
            var reader = new FileReader();
            reader.ReadFiles(tempDir);

            var importer = new SemesterImport(reader.Context, model.SemesterId, model.OrganiserId);

            // Gruppen müssen existieren! => Fehler, wenn nicht
            importer.CheckGroups();

            // Räume sollten existieren => Warnung
            // Zuordnungen zu Räumen sollten existieren => Warnung
            importer.CheckRooms();

            // Dozenten sollten existieren => Warnung
            importer.CheckLecturers();

            importer.CheckCourses();

            importer.CheckRestriktions();

            reader.Context.AddErrorMessage("Zusammenfassung", string.Format("Von {0} Kursen werden {1} importiert",
                reader.Context.Kurse.Count, reader.Context.Kurse.Count(x => x.IsValid)), false);


            model.Context = reader.Context;
            model.Organiser = GetMyOrganisation();
            model.Semester = SemesterService.GetSemester(model.SemesterId);


            return View(model);
        }

    }
}