using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MyStik.TimeTable.Data;
using MyStik.TimeTable.DataServices;
using MyStik.TimeTable.Web.Models;

namespace MyStik.TimeTable.Web.Controllers
{
    public class UntisController : BaseController
    {
        // GET: Untis
        public ActionResult Index()
        {




            var semester = GetSemester();

            var model = new SemesterImportModel
            {
                Semester = semester,
                SemesterId = semester.Id,
                OrganiserId = GetMyOrganisation().Id,
            };

            /*
                             Existing = Db.Activities.OfType<Course>().Count(c =>
                    c.SemesterGroups.Any(g => g.Semester.Id == semester.Id) &&
                    (!string.IsNullOrEmpty(c.ExternalSource) && c.ExternalSource.Equals("GPUNTIS")))
*/


            // wenn für das Semester ein Verzeichnis angelegt ist
            // dann überprüfen, ob Dateien vorhanden sind
            // Hochladen ohne Einlesen
            // Einlesen üebr Ajax-Aktion => Hub, SugnalR
            /*
            string tempDir = Path.GetTempPath();

            tempDir = Path.Combine(tempDir, semester.Name);

            if (Directory.Exists(tempDir))
            {
                model.FileNames = Directory.EnumerateFiles(tempDir);
            }
            else
            {
                model.Message = string.Format("Bisher keine Daten, Verzeichnis für {0} existiert nicht", semester.Name);
            }
             * */

            ViewBag.Organiser = Db.Organisers.Select(c => new SelectListItem
            {
                Text = c.ShortName,
                Value = c.Id.ToString(),
            });

            ViewBag.Semester = Db.Semesters.OrderByDescending(c => c.StartCourses).Select(c => new SelectListItem
            {
                Text = c.Name,
                Value = c.Id.ToString(),
            });


            return View(model);
        }

        [HttpPost]
        public ActionResult CheckDatabase(SemesterImportModel model)
        {
            model.Existing = Db.Activities.OfType<Course>().Count(c =>
                c.SemesterGroups.Any(g => 
                    g.Semester.Id == model.SemesterId && 
                    g.CurriculumGroup.Curriculum.Organiser.Id == model.OrganiserId
                ) &&
                (!string.IsNullOrEmpty(c.ExternalSource) && c.ExternalSource.Equals("GPUNTIS")));

            model.Semester = Db.Semesters.SingleOrDefault(s => s.Id == model.SemesterId);
            model.Organiser = Db.Organisers.SingleOrDefault(o => o.Id == model.OrganiserId);


            return View(model);
        }

        [HttpPost]
        public ActionResult SelectFiles(SemesterImportModel model)
        {
            return View(model);
        }

        [HttpPost]
        public ActionResult Upload(SemesterImportModel model)
        {
            string tempDir = Path.GetTempPath();

            var semester = new SemesterService().GetSemester(model.SemesterId);
            var org = Db.Organisers.SingleOrDefault(o => o.Id == model.OrganiserId);

            if (semester != null && org != null)
            {
                tempDir = Path.Combine(tempDir, semester.Name);
                tempDir = Path.Combine(tempDir, org.ShortName);

                if (!Directory.Exists(tempDir))
                {
                    Directory.CreateDirectory(tempDir);
                }


                foreach (var attachment in model.Attachments)
                {
                    if (attachment != null)
                    {
                        var fileName = Path.GetFileName(attachment.FileName);

                        var tempFileName = Path.Combine(tempDir, fileName);

                        attachment.SaveAs(tempFileName);
                    }
                }

                model.FileNames = Directory.EnumerateFiles(tempDir);
            }


            return View(model);
        }

    }
}