using System;
using System.IO;
using System.Linq;
using System.Text;
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
        public ActionResult MigrateCurricula()
        {

            return View();
        }


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

        public ActionResult CreateCie()
        {
            createCurricula("CIE-B", "CIE-Bachelor");
            createCurricula("CIE-M", "CIE-Master");

            return RedirectToAction("Index", "Dashboard");
        }


        public ActionResult CreateCieGroups()
        {
            var semester = Db.Semesters.SingleOrDefault(x => x.Name.Equals("WiSe 2018"));

            // Alle Curricula durchgehen
            foreach (var curriculum in Db.Curricula.Where(x => x.ShortName.Equals("CIE-B") || x.ShortName.Equals("CIE-M")).ToList())
            {
                foreach (var curriculumGroup in curriculum.CurriculumGroups.ToList())
                {
                    foreach (var capacityGroup in curriculumGroup.CapacityGroups.ToList())
                    {
                        var exist = semester.Groups.Any(g => g.CapacityGroup.Id == capacityGroup.Id);

                        if (!exist)
                        {
                            var semGroup = new SemesterGroup
                            {
                                CapacityGroup = capacityGroup,
                                CurriculumGroup = capacityGroup.CurriculumGroup,        // nur noch aus Gründen der Sicherheit
                                Semester = semester
                            };

                            semester.Groups.Add(semGroup);
                            Db.SemesterGroups.Add(semGroup);
                        }
                    }
                }
            }

            Db.SaveChanges();

            return RedirectToAction("Index", "Dashboard");
        }



        private void createCurricula(string shortName, string name)
        { 
            var orgs = Db.Organisers.Where(x => x.ShortName.StartsWith("FK")).ToList();

            foreach (var org in orgs)
            {
                var cieB = org.Curricula.SingleOrDefault(x => x.ShortName.Equals(shortName));

                if (cieB == null)
                {
                    cieB = new Curriculum
                    {
                        Name = name,
                        ShortName = shortName,
                        Organiser = org
                    };

                    Db.Curricula.Add(cieB);
                }

                var firstGroup = cieB.CurriculumGroups.FirstOrDefault();
                if (firstGroup != null)
                {
                    firstGroup.Name = string.Empty;
                }
                else
                {
                    firstGroup = new CurriculumGroup
                    {
                        Curriculum = cieB,
                        Name = string.Empty,
                        IsSubscribable = true
                    };

                    Db.CurriculumGroups.Add(firstGroup);
                }

                var firstCapGroup = firstGroup.CapacityGroups.FirstOrDefault();

                if (firstCapGroup == null)
                { 
                    firstCapGroup = new  CapacityGroup
                    {
                        CurriculumGroup = firstGroup,
                        Name = string.Empty,
                        InSS = true,
                        InWS = true
                    };

                    Db.CapacityGroups.Add(firstCapGroup);
                }

            }



            Db.SaveChanges();
        }

        public ActionResult CreateExamForms()
        {
            AddExamForm("schrP", "schriftliche Prüfung", "§ 21 ASPO");
            AddExamForm("mdlP", "mündliche Prüfung", "§ 22 ASPO");
            AddExamForm("Präs", "Präsentation", "§ 23 ASPO");
            AddExamForm("ModA", "Modularbeit", "§ 24 ASPO");
            AddExamForm("praP", "Praktische Prüfung", "§ 25 ASPO");


            return RedirectToAction("Index", "Dashboard");
        }

        private void AddExamForm(string shortName, string name, string description)
        {
            var form = Db.ExaminationForms.SingleOrDefault(x => x.ShortName.Equals(shortName));

            if (form == null)
            {
                form = new ExaminationForm
                {
                    Name = name,
                    ShortName = shortName,
                    Description = description
                };
                Db.ExaminationForms.Add(form);
            }
            else
            {
                form.Name = name;
                form.Description = description;
            }

        }

    }
}
