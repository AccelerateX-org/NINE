using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using MyStik.TimeTable.Data;
using MyStik.TimeTable.Web.Controllers;
using MyStik.TimeTable.Web.Models;

namespace MyStik.TimeTable.Web.Areas.Admin.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public class RepairController : BaseController
    {
        // GET: Admin/Repair
        /// <summary>
        /// 
        /// </summary>
        public ActionResult ChangeDoz()
        {
            var model = new SemesterImportModel();
            model.OrganiserId = Guid.Empty;
            model.SemesterId = Guid.Empty;
            model.FormatId = "CSV";

            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        [HttpPost]
        public ActionResult ChangeDoz(SemesterImportModel model)
        {
            string tempDir = Path.GetTempPath();
            string tempFile = Path.GetTempFileName();

            // Speichern der Config-Dateien
            model.AttachmentDays?.SaveAs(tempFile);


            var lines = System.IO.File.ReadAllLines(tempFile, Encoding.Default);

            var org = Db.Organisers.SingleOrDefault(x => x.ShortName.Equals("FK 11"));



            var i = 0;
            foreach (var line in lines)
            {
                if (i > 0)
                {
                    var words = line.Split(';');
                    var newId = words[0].Trim();
                    var oldId = words[1].Trim();

                    var member = org.Members.SingleOrDefault(x => x.ShortName.Equals(oldId));
                    if (member != null)
                    {
                        member.ShortName = newId;
                    }
                }


                i++;
            }

            Db.SaveChanges();


            return RedirectToAction("Index", "Dashboard");
        }


        /// <summary>
        /// 
        /// </summary>
        public ActionResult ChangeGroups()
        {
            var sem = SemesterService.GetSemester(DateTime.Today);
            var org = Db.Organisers.SingleOrDefault(x => x.ShortName.Equals("FK 11"));
            var curr = org.Curricula.SingleOrDefault(x => x.ShortName.Equals("BASA Präsenz"));

            for (var i = 1; i <= 7; i++)
            {
                var sGroupVZ = $"{i} VZ";

                var currGroupVZ = curr.CurriculumGroups.SingleOrDefault(x => x.Name.Equals(sGroupVZ));

                var semGroup = Db.SemesterGroups.SingleOrDefault(x =>
                    x.Semester.Id == sem.Id && x.CapacityGroup.CurriculumGroup.Id == currGroupVZ.Id);

                if (semGroup == null)
                {
                    semGroup = new SemesterGroup
                    {
                        Semester = sem,
                        CapacityGroup = currGroupVZ.CapacityGroups.FirstOrDefault()
                    };

                    Db.SemesterGroups.Add(semGroup);
                }

                currGroupVZ.Name = $"{i}";
            }

            Db.SaveChanges();

            return RedirectToAction("Index", "Dashboard");
        }
            }
        }
