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
    public class RepairController : BaseController
    {
        // GET: Admin/Repair
        public ActionResult ChangeDoz()
        {
            var model = new SemesterImportModel();
            model.OrganiserId = Guid.Empty;
            model.SemesterId = Guid.Empty;
            model.FormatId = "CSV";

            return View();
        }

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


        /*
        public ActionResult ChangeGroups()
        {
            var org = Db.Organisers.SingleOrDefault(x => x.ShortName.Equals("FK 11"));
            var curr = org.Curricula.SingleOrDefault(x => x.ShortName.Equals("BASA Präsenz"));

            for (var i = 1; i <=7; i ++)
            {
                var sGroupVZ = $"{i} VZ";
                var sGroupTZ = $"{i} TZ";

                var currGroupVZ = curr.CurriculumGroups.SingleOrDefault(x => x.Name.Equals(sGroupVZ));
                var capGroupVZ = currGroupVZ.CapacityGroups.FirstOrDefault();


                var currGroupTZ = curr.CurriculumGroups.SingleOrDefault(x => x.Name.Equals(sGroupTZ));
                if (currGroupTZ != null)
                {
                    // alle SemesterSubscriptions umhängen
                    var subscriptions = Db.Subscriptions.OfType<SemesterSubscription>()
                        .Where(x => x.SemesterGroup.CapacityGroup.CurriculumGroup.Id == currGroupTZ.Id).ToList();

                    foreach (var subscription in subscriptions)
                    {
                        if (capGroupVZ != null)
                        {
                            subscription.SemesterGroup.CapacityGroup = capGroupVZ;
                        }
                    }

                    // alle Semestergruppen löschen
                    var semGroupsTZ = Db.SemesterGroups.Where(x => x.CapacityGroup.CurriculumGroup.Id == currGroupTZ.Id)
                        .ToList();

                    foreach (var semesterGroup in semGroupsTZ)
                    {
                        Db.SemesterGroups.Remove(semesterGroup);
                    }

                    // jetzt die CurrGrupp löschen
                    foreach (var capacityGroup in currGroupTZ.CapacityGroups.ToList())
                    {
                        Db.CapacityGroups.Remove(capacityGroup);
                    }
                    Db.CurriculumGroups.Remove(currGroupTZ);
                }
            }

            var currGroupVZ7 = curr.CurriculumGroups.SingleOrDefault(x => x.Name.Equals("7 VZ"));

            for (var i = 8; i <= 14; i++)
            {
                var sGroupTZ = $"{i} TZ";

                var currGroupTZ = curr.CurriculumGroups.SingleOrDefault(x => x.Name.Equals(sGroupTZ));
                if (currGroupTZ != null)
                {
                    // alle SemesterSubscriptions umhängen
                    var subscriptions = Db.Subscriptions.OfType<SemesterSubscription>()
                        .Where(x => x.SemesterGroup.CapacityGroup.CurriculumGroup.Id == currGroupTZ.Id).ToList();

                    foreach (var subscription in subscriptions)
                    {
                        if (currGroupVZ7 != null)
                        {
                            subscription.SemesterGroup.CapacityGroup = currGroupVZ7.CapacityGroups.First();
                        }
                    }

                    // alle Semestergruppen löschen
                    var semGroupsTZ = Db.SemesterGroups.Where(x => x.CapacityGroup.CurriculumGroup.Id == currGroupTZ.Id)
                        .ToList();

                    foreach (var semesterGroup in semGroupsTZ)
                    {
                        Db.SemesterGroups.Remove(semesterGroup);
                    }

                    // jetzt die CurrGrupp löschen
                    foreach (var capacityGroup in currGroupTZ.CapacityGroups.ToList())
                    {
                        Db.CapacityGroups.Remove(capacityGroup);
                    }
                    Db.CurriculumGroups.Remove(currGroupTZ);
                }
            }

            Db.SaveChanges();

            return RedirectToAction("Index", "Dashboard");
        }
        */
            }
        }