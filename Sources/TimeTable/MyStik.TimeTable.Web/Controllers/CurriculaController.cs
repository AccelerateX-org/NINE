using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using MyStik.TimeTable.Data;
using MyStik.TimeTable.Web.Models;
using MyStik.TimeTable.Web.Services;

namespace MyStik.TimeTable.Web.Controllers
{
    public class CurriculaController : BaseController
    {
        
        // GET: Curricula
        public ActionResult Index()
        {
            var model = Db.Curricula.ToList();

            SetEditRights();

            return View(model);
        }

        public ActionResult Details(Guid id)
        {
            var curriculum = Db.Curricula.SingleOrDefault(c => c.Id == id);

            SetEditRights();
            
            return View(curriculum);
        }

        public ActionResult CheckGroups()
        {
            var model = Db.SemesterGroups.Where(g => g.CapacityGroup == null || g.CurriculumGroup == null).ToList();

            return View(model);
        }

        public ActionResult GroupDetails(Guid id)
        {
            var group = Db.CurriculumGroups.SingleOrDefault(g => g.Id == id);

            SetEditRights();

            return View(group);
        }


        public ActionResult DeleteCurriculumGroup(Guid id)
        {
            // Die gruppe ermitteln, die gelöscht werden soll
            var group = Db.CurriculumGroups.SingleOrDefault(g => g.Id == id);

            var curr = group.Curriculum;

            // ALTE STRUKTUR
            // alle zugehörigen Semestergruppen löschen 
            // => Die Kurse sind davon nicht betroffen
            //    sie werden u.U. nicht mehr angezeigt / gefunden
            foreach (var semesterGroup in group.SemesterGroups.ToList())
            {
                group.SemesterGroups.Remove(semesterGroup);

                // Alle ggf. vorhandenen Eintragungen (Subscriptions) der
                // Semestergruppe löschen
                foreach (var semSub in semesterGroup.Subscriptions.ToList())
                {
                    semesterGroup.Subscriptions.Remove(semSub);
                    Db.Subscriptions.Remove(semSub);
                }

                Db.SemesterGroups.Remove(semesterGroup);
            }

            // NEUE STRUKTUR
            // alle zugehörigen CapactityGroups löschen
            foreach (var capGroup in group.CapacityGroups.ToList())
            {
                foreach (var semesterGroup in capGroup.SemesterGroups.ToList())
                {
                    group.SemesterGroups.Remove(semesterGroup);

                    // Alle ggf. vorhandenen Eintragungen (Subscriptions) der
                    // Semestergruppe löschen
                    foreach (var semSub in semesterGroup.Subscriptions.ToList())
                    {
                        semesterGroup.Subscriptions.Remove(semSub);
                        Db.Subscriptions.Remove(semSub);
                    }

                    Db.SemesterGroups.Remove(semesterGroup);
                }

                foreach (var groupAlias in capGroup.Aliases.ToList())
                {
                    Db.GroupAliases.Remove(groupAlias);
                }

                Db.CapacityGroups.Remove(capGroup);
            }

            Db.CurriculumGroups.Remove(group);
            Db.SaveChanges();

            // Die neue Seite aufbauen
            // Die Liste des zugehörigen Studiengangs anzeigen
            
            SetEditRights();

            return RedirectToAction("Details", new {id = curr.Id});
        }

        public ActionResult DeleteCapacityGroup(Guid id)
        {
            // Die gruppe ermitteln, die gelöscht werden soll
            var capGroup = Db.CapacityGroups.SingleOrDefault(g => g.Id == id);

            var curr = capGroup.CurriculumGroup.Curriculum;


            // NEUE STRUKTUR
            // alle zugehörigen CapactityGroups löschen
                foreach (var semesterGroup in capGroup.SemesterGroups.ToList())
                {
                    capGroup.SemesterGroups.Remove(semesterGroup);

                    // Alle ggf. vorhandenen Eintragungen (Subscriptions) der
                    // Semestergruppe löschen
                    foreach (var semSub in semesterGroup.Subscriptions.ToList())
                    {
                        semesterGroup.Subscriptions.Remove(semSub);
                        Db.Subscriptions.Remove(semSub);
                    }

                    Db.SemesterGroups.Remove(semesterGroup);
                }

                foreach (var groupAlias in capGroup.Aliases.ToList())
                {
                    Db.GroupAliases.Remove(groupAlias);
                }


            Db.CapacityGroups.Remove(capGroup);
            Db.SaveChanges();

            // Die neue Seite aufbauen
            // Die Liste des zugehörigen Studiengangs anzeigen

            SetEditRights();

            return RedirectToAction("Details", new { id = curr.Id });
        }


        public ActionResult ImportData()
        {
            return View();
        }


        [HttpPost]
        public ActionResult ImportData(HttpPostedFileBase importFile)
        {
            if (importFile != null)
            {
                var bytes = new byte[importFile.ContentLength];
                importFile.InputStream.Read(bytes, 0, importFile.ContentLength);

                var stream = new System.IO.MemoryStream(bytes);
                var reader = new System.IO.StreamReader(stream, Encoding.Default);
                var text = reader.ReadToEnd();

                string[] lines = text.Split('\n');


                var i = 0;
                foreach (var line in lines)
                {
                    if (i > 0)
                    {
                        string newline = line.Trim();

                        if (!string.IsNullOrEmpty(newline))
                        {
                            string[] words = newline.Split(';');

                            var fk = words[0]; // Kurzname Fakultät
                            var cu = words[1]; // Kurzname Studiengang
                            var sg = words[2]; // Name Studiengruppe
                            var cg = words[3]; // Name Kapazitätsgruppe - kann leer sein
                            var ws = words[4]; // Kapazitätsgruppe wird im WS angeboten
                            var ss = words[5]; // Kapazitätsgruppe wird im SS angeboten
                            var untis = words[6]; // Kürzel gpUntis

                            // Vorbedingung
                            // Fakultät und Studiengang müssen existieren
                            var faculty = Db.Organisers.SingleOrDefault(f => f.ShortName.Equals(fk));
                            if (faculty != null)
                            {
                                var curriculum = faculty.Curricula.SingleOrDefault(c => c.ShortName.Equals(cu));

                                if (curriculum != null)
                                {
                                    var studyGroup = curriculum.CurriculumGroups.SingleOrDefault(g => g.Name.Equals(sg));

                                    // Neue Studiengruppen werden automatisch angelegt
                                    if (studyGroup == null)
                                    {
                                        studyGroup = new CurriculumGroup
                                        {
                                            Name = sg,
                                            Curriculum = curriculum,
                                            IsSubscribable = false
                                        };
                                        Db.CurriculumGroups.Add(studyGroup);
                                        Db.SaveChanges();
                                    }

                                    var capacityGroup = string.IsNullOrEmpty(cg) ?
                                        studyGroup.CapacityGroups.SingleOrDefault(g => string.IsNullOrEmpty(g.Name)) :
                                        studyGroup.CapacityGroups.SingleOrDefault(g => g.Name.Equals(cg));

                                    // Neue Kapazitätsgruppen werden automatisch angelegt
                                    if (capacityGroup == null)
                                    {
                                        capacityGroup = new CapacityGroup
                                        {
                                            CurriculumGroup = studyGroup,
                                            Name = cg
                                        };
                                        Db.CapacityGroups.Add(capacityGroup);
                                        Db.SaveChanges();
                                    }

                                    // ggf. bereits vorhandene Eigenschaften überschreiben
                                    capacityGroup.InWS = string.Equals(ws, "WS");
                                    capacityGroup.InSS = string.Equals(ss, "SS");
                                    Db.SaveChanges();

                                    var alias = capacityGroup.Aliases.SingleOrDefault(a => a.Name.Equals(untis));

                                    if (alias == null)
                                    {
                                        alias = new GroupAlias
                                        {
                                            CapacityGroup = capacityGroup,
                                            Name = untis
                                        };
                                        Db.GroupAliases.Add(alias);
                                        Db.SaveChanges();
                                    }
                                }
                            }
                        }
                    }
                    i++;
                }
            }

            return RedirectToAction("Index");
        }

        public ActionResult CorrectGroups()
        {
            var currService = new CurriculumService();

            currService.SetSubscriptionState("WI", "WPM", false);
            currService.SetSubscriptionState("LM", "WPM", false);
            currService.SetSubscriptionState("AU", "WPM", false);
            currService.SetSubscriptionState("WIM", "WPM", false);
            currService.SetSubscriptionState("MBA", "WPM", false);

            currService.DeleteAlias("AU", "WPM", "", "WPM LM");
            currService.DeleteAlias("WIM", "3", "G2", "WI M2 G2");
            currService.DeleteAlias("WIM", "3", "G2", "WI M2 G3");

            currService.MoveWI();
            currService.MoveAU();
            currService.MoveLM();
            currService.MoveWIM();
            currService.MoveMBA();
            currService.MoveMisc();

            return RedirectToAction("Index");
        }


        public ActionResult MoveGroup(string srcGroupMove, string trgGroupMove, Guid id)
        {
            var currService = new CurriculumService();

            currService.MoveGroup(srcGroupMove, trgGroupMove);

            return RedirectToAction("Details", new { id = id });
        }

        public ActionResult MergeGroup(string srcGroup, string trgGroup, Guid id)
        {
            var currService = new CurriculumService();

            currService.MergeGroup(srcGroup, trgGroup);

            return RedirectToAction("Details", new {id = id});
        }

        public ActionResult DeleteGroup2(string delGroup, Guid id)
        {
            var currService = new CurriculumService();

            currService.DeleteGroup(delGroup);

            return RedirectToAction("Details", new { id = id });
        }


        public ActionResult RemoveDangelingGroups()
        {
             var model = Db.SemesterGroups.Where(g => g.CapacityGroup == null || g.CurriculumGroup == null).ToList();

            foreach (var semesterGroup in model)
            {
                Db.SemesterGroups.Remove(semesterGroup);
            }

            Db.SaveChanges();

            return RedirectToAction("Index");
        }


        private void SetEditRights()
        {
            ViewBag.HasEditRights = User.IsInRole("SysAdmin");
            ViewBag.UserRight = GetUserRight();
        }
    }
}