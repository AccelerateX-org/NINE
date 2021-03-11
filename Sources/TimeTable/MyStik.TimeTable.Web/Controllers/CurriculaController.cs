using System;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using MyStik.TimeTable.Data;
using MyStik.TimeTable.Web.Models;
using MyStik.TimeTable.Web.Services;

namespace MyStik.TimeTable.Web.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public class CurriculaController : BaseController
    {
        public ActionResult Disclaimer()
        {
            return View();
        }

        
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            var model = Db.Organisers.Where(x => x.Curricula.Any()).OrderBy(g => g.ShortName).ToList();
            ViewBag.UserRight = GetUserRight();
            return View(model);
        }

        public ActionResult Organiser(Guid id)
        {
            var org = GetOrganiser(id);

            var model = new OrganiserViewModel
            {
                Organiser = org
            };

            ViewBag.UserRights = GetUserRight(org);

            return View(model);
        }



        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Create()
        {
            var model = new CurriculaCreateModel();
            var org = GetMyOrganisation();

            ViewBag.Organiser = org;

            model.OrganiserId = org.Id;
            return View(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Create(CurriculaCreateModel model)
        {
            var org = Db.Organisers.SingleOrDefault(o => o.Id == model.OrganiserId);

            if (org != null)
            {
                var curr = new Curriculum
                {
                    Name = model.Name,
                    ShortName = model.ShortName,
                    Organiser = org
                };
                Db.Curricula.Add(curr);
                Db.SaveChanges();

                return RedirectToAction("Index", "Curriculum", new {id = curr.Id});
            }


            return RedirectToAction("Index", "Curriculum");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult GroupList(Guid id)
        {
            var curriculum = Db.Curricula.SingleOrDefault(c => c.Id == id);

            SetEditRights();

            return View(curriculum);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult CreateGroup(Guid id)
        {
            var curriculum = Db.Curricula.SingleOrDefault(c => c.Id == id);

            var model = new CurriculumGroupCreateModel
            {
                Curriculum = curriculum,
                CapacityGroupCount = 1
            };

            return View(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult CreateGroup(CurriculumGroupCreateModel model)
        {
            var curriculum = Db.Curricula.SingleOrDefault(c => c.Id == model.Curriculum.Id);

            var group = new CurriculumGroup
            {
                Curriculum = curriculum,
                IsSubscribable = model.IsSubscribable,
                Name = model.Name
            };

            Db.CurriculumGroups.Add(group);

            if (model.CapacityGroupCount > 1)
            {
                for (int i = 1; i <= model.CapacityGroupCount; i++)
                {
                    Char c = (Char) ((65) + (i - 1));

                    var capGroup = new CapacityGroup
                    {
                        CurriculumGroup = group,
                        InSS = true,
                        InWS = true,
                        Name = c.ToString()
                    };

                    group.CapacityGroups.Add(capGroup);
                }

            }
            else
            {
                var capGroup = new CapacityGroup
                {
                    CurriculumGroup = group,
                    InSS = true,
                    InWS = true
                };

                group.CapacityGroups.Add(capGroup);
            }


            Db.SaveChanges();

            return RedirectToAction("Structure", "Curriculum", new {id = model.Curriculum.Id});
        }




        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult ModuleCatalog(Guid id)
        {
            var curriculum = Db.Curricula.SingleOrDefault(c => c.Id == id);

            SetEditRights();

            return View(curriculum);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Details(Guid id)
        {
            var curriculum = Db.Curricula.SingleOrDefault(c => c.Id == id);

            if (curriculum.ShortName.Equals("WI"))
            {
                var g = curriculum.CurriculumGroups.SingleOrDefault(x => x.Name.Equals("2"));
                if (g != null)
                {
                    var g2 = g.CapacityGroups.SingleOrDefault(x => x.Name.Equals("C"));
                    var g3 = g2.Aliases.SingleOrDefault(x => x.Name.Equals("2C-IMT"));
                    if (g3 == null)
                    {
                        g3 = new GroupAlias
                        {
                            CapacityGroup = g2,
                            Name = "2C-IMT"
                        };

                        Db.GroupAliases.Add(g3);
                        Db.SaveChanges();
                    }
                }
            }
            
            SetEditRights();
            
            return View(curriculum);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult CheckGroups()
        {
            var model = Db.SemesterGroups.Where(g => g.CapacityGroup == null || g.CurriculumGroup == null).ToList();

            return View(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult GroupDetails(Guid id)
        {
            var group = Db.CurriculumGroups.SingleOrDefault(g => g.Id == id);

            /*
             * OHI 20160429: wird nicht mehr benötigt
            if (group.Name.Equals("3 INF"))
            {
                var cm = new CurriculumModule()
                {
                    Id = Guid.NewGuid(),
                    ModuleId = "SE1",
                    Description = "Soft Ing",
                    Name = "SE1",
                    ShortName = "SE1"
                };
                group.Modules.Add(cm);
            }
            */


            SetEditRights();

            return View(group);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult ModuleDetails(Guid id)
        {
            // Daten "erzeugen", z.B. durch Abfragen einer DB

            // Daten an den View übergeben

            // An Statt "Daten" verwenden wir den Begriff "Model"

            // Aufbau des Models
            // zuerst "Dummydaten"
            // später: hier kommt eine Datenbankabfrage rein
            var daten = new ModuleViewModel();
            daten.Name = "Software Engineering 1";
            daten.Description = "Agile Produktentwicklung";
            daten.Shortcut = "SE1";
            daten.MV = "Olav Hinz";
            daten.Dozent = "Olav Hinz";
            daten.Language = "deutsch";
            daten.Assignment = "Bachelor WI";
            daten.SWS = "Seminaristischer Unterricht, 4 SWS";
            daten.Work = "Präsenzzeit: 60 Stunden";
            daten.Credits = "5 ECTS";
            daten.Requirements = "Module des 1. und 2. Semesters";
            daten.Skills = "Viele Sachen";
            daten.Topic = "";
            daten.Leistungen = "Projektarbeit";
            daten.Books = "Methoden des Software Engineerings";
            // Übergabe des Models an den View
            return View(daten);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult EditCurriculumGroup(Guid id)
        {
            var currGroup = Db.CurriculumGroups.SingleOrDefault(g => g.Id == id);

            if (currGroup != null)
            {
                var model = new CurriculumGroupEditModel
                {
                    CurriculumGroup = currGroup,
                    Name = currGroup.Name,
                    IsSubscribable = currGroup.IsSubscribable
                };
                return View(model);
            }

            return RedirectToAction("Index");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult EditCurriculumGroup(CurriculumGroupEditModel model)
        {
            var currGroup = Db.CurriculumGroups.SingleOrDefault(g => g.Id == model.CurriculumGroup.Id);

            if (currGroup != null)
            {
                currGroup.Name = model.Name;
                currGroup.IsSubscribable = model.IsSubscribable;

                Db.SaveChanges();
            }

            return RedirectToAction("GroupList", new {id = currGroup.Curriculum.Id});
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult DeleteCurriculumGroup(Guid id)
        {
            // Die gruppe ermitteln, die gelöscht werden soll
            var group = Db.CurriculumGroups.SingleOrDefault(g => g.Id == id);

            var curr = group.Curriculum;

            // ALTE STRUKTUR
            // alle zugehörigen Semestergruppen löschen 
            // => Die Kurse sind davon nicht betroffen
            //    sie werden u.U. nicht mehr angezeigt / gefunden
            /*
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
            */

            // NEUE STRUKTUR
            // alle zugehörigen CapactityGroups löschen
            foreach (var capGroup in group.CapacityGroups.ToList())
            {
                foreach (var semesterGroup in capGroup.SemesterGroups.ToList())
                {
                    //group.SemesterGroups.Remove(semesterGroup);

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

            return RedirectToAction("GroupList", new {id = curr.Id});
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult CreateCapacityGroup(Guid id)
        {
            var currGroup = Db.CurriculumGroups.SingleOrDefault(g => g.Id == id);

            if (currGroup != null)
            {
                var model = new CapacityGroupCreateModel
                {
                    CurriculumGroup = currGroup,
                    InWS = true,
                    InSS = true,
                };
                return View(model);
            }

            return RedirectToAction("Index");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult CreateCapacityGroup(CapacityGroupCreateModel model)
        {
            var currGroup = Db.CurriculumGroups.SingleOrDefault(g => g.Id == model.CurriculumGroup.Id);

            if (currGroup != null)
            {
                var capGroup = new CapacityGroup
                {
                    CurriculumGroup = currGroup,
                    Name = model.Name,
                    InSS = model.InSS,
                    InWS = model.InWS
                };
                Db.CapacityGroups.Add(capGroup);

                if (!string.IsNullOrEmpty(model.AliasList))
                {
                    var elems = model.AliasList.Split(',');
                    foreach (var elem in elems)
                    {
                        var groupAlias = new GroupAlias
                        {
                            CapacityGroup = capGroup,
                            Name = elem
                        };

                        Db.GroupAliases.Add(groupAlias);
                    }
                }

                Db.SaveChanges();
            }

            return RedirectToAction("GroupList", new { id = currGroup.Curriculum.Id });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult EditCapacityGroup(Guid id)
        {
            var capGroup = Db.CapacityGroups.SingleOrDefault(g => g.Id == id);

            if (capGroup != null)
            {
                var sb = new StringBuilder();
                foreach (var alias in capGroup.Aliases)
                {
                    if (alias == capGroup.Aliases.First())
                    {
                        sb.Append(alias.Name);
                    }
                    else
                    {
                        sb.AppendFormat(",{0}", alias.Name);
                    }
                }


                var model = new CapacityGroupEditModel
                {
                    CapacityGroup = capGroup,
                    Name = capGroup.Name,
                    InWS = capGroup.InWS,
                    InSS = capGroup.InSS,
                    AliasList = sb.ToString()
                };
                return View(model);
            }

            return RedirectToAction("Index");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult EditCapacityGroup(CapacityGroupEditModel model)
        {
            var currGroup = Db.CapacityGroups.SingleOrDefault(g => g.Id == model.CapacityGroup.Id);

            if (currGroup != null)
            {
                currGroup.Name = model.Name;
                currGroup.InSS = model.InSS;
                currGroup.InWS = model.InWS;

                currGroup.Aliases.Clear();
                if (!string.IsNullOrEmpty(model.AliasList))
                {
                    var elems = model.AliasList.Split(',');
                    foreach (var elem in elems)
                    {
                        var groupAlias = new GroupAlias
                        {
                            CapacityGroup = currGroup,
                            Name = elem
                        };

                        Db.GroupAliases.Add(groupAlias);
                    }
                }

                Db.SaveChanges();
            }

            return RedirectToAction("GroupList", new { id = currGroup.CurriculumGroup.Curriculum.Id });
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
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

            return RedirectToAction("GroupList", new { id = curr.Id });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult ImportData()
        {
            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="importFile"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="srcGroupMove"></param>
        /// <param name="trgGroupMove"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult MoveGroup(string srcGroupMove, string trgGroupMove, Guid id)
        {
            var currService = new CurriculumService();

            currService.MoveGroup(srcGroupMove, trgGroupMove);

            return RedirectToAction("Details", new { id = id });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="srcGroup"></param>
        /// <param name="trgGroup"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult MergeGroup(string srcGroup, string trgGroup, Guid id)
        {
            var currService = new CurriculumService();

            currService.MergeGroup(srcGroup, trgGroup);

            return RedirectToAction("Details", new {id = id});
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="delGroup"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult DeleteGroup2(string delGroup, Guid id)
        {
            var currService = new CurriculumService();

            currService.DeleteGroup(delGroup);

            return RedirectToAction("Details", new { id = id });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Erzeugt das leere Formular
        /// </summary>
        /// <returns></returns>
        public ActionResult CreateModule()
        {
            return View();
        }

        /// <summary>
        /// verarbeitet das ausgefüllte Formular
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult CreateModule(CurriculumModuleCreateModel model)
        {
            return RedirectToAction("Index");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult EditModule()
        {
            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult LinkCourses(Guid id)
        {
            

            return RedirectToAction("Details", new {id = id});
        }

        /// <summary>
        /// verarbeitet das ausgefüllte Formular
        /// </summary>
        /// <param name="module"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult EditModule(CurriculumModule module)
        {
            return RedirectToAction("Index");
        }

        private void SetEditRights()
        {
            var org = GetMyOrganisation();
            var userRight = GetUserRight(org);
            ViewBag.HasEditRights = User.IsInRole("SysAdmin") || userRight.IsCurriculumAdmin;
            ViewBag.UserRight = userRight;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="currId"></param>
        /// <param name="critId"></param>
        /// <returns></returns>
        public ActionResult SelectModule(Guid currId, Guid critId)
        {
            var curr = Db.Curricula.SingleOrDefault(x => x.Id == currId);
            //var crit = curr.Criterias.SingleOrDefault(x => x.Id == critId);
            var allModules = Db.CurriculumModules.ToList();

            var model = new SelectModuleViewModel
            {
                Curriculum = curr,
                //Criteria = crit,
                Modules = allModules
            };

            return View(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="moduleId"></param>
        /// <param name="critId"></param>
        /// <returns></returns>
        public ActionResult AccreditateModule(Guid moduleId, Guid critId)
        {
            var crit = Db.Criterias.SingleOrDefault(x => x.Id == critId);
            var module = Db.CurriculumModules.SingleOrDefault(x => x.Id == moduleId);

            var accr = new ModuleAccreditation
            {
                Criteria = crit,
                Module = module
            };

            Db.Accreditations.Add(accr);
            Db.SaveChanges();


            return RedirectToAction("Details", new {id = crit.Id});
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Criteria(Guid id)
        {
            var model = Db.Criterias.SingleOrDefault(x => x.Id == id);

            return View(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="accrId"></param>
        /// <returns></returns>
        public ActionResult LinkModule(Guid accrId)
        {
            var model = Db.Accreditations.SingleOrDefault(x => x.Id == accrId);

            return View(model);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="accrId"></param>
        /// <param name="groupId"></param>
        /// <returns></returns>
        public ActionResult LinkGroup(Guid accrId, Guid groupId)
        {
            var accr = Db.Accreditations.SingleOrDefault(x => x.Id == accrId);
            var group = Db.CurriculumGroups.SingleOrDefault(x => x.Id == groupId);


            return RedirectToAction("Details", new {id = group.Curriculum.Id});
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Delete(Guid id)
        {
            var curriculum = Db.Curricula.SingleOrDefault(x => x.Id == id);
            if (curriculum == null)
                return RedirectToAction("Index");

            var service = new CurriculumService();
            service.DeleteCurriculum(id);


            return RedirectToAction("Curricula", "Organiser");
        }
    }
}