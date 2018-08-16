using MyStik.TimeTable.Data;
using MyStik.TimeTable.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace MyStik.TimeTable.Web.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public class CoursePlanController : BaseController
    {



        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            // Liste aller meiner CoursePlan
            var model = Db.CoursePlans.ToList();
            return View(model);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Create()
        {
            // liefert das leere Formular
            return View();
        }
    
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Schwerpunkt"></param>
        /// <param name="Sprachkonzept"></param>
        /// <param name="semester"></param>
        /// <param name="Fachsprache"></param>
        /// <param name="nameofplan"></param>
        /// <param name="description"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Create(string Schwerpunkt, string Sprachkonzept, string semester, string Fachsprache, string nameofplan, string description)
        {
            // hier kommt das Formular an

            // TODO
            // jetzt muss hier der persönliche Modulplan aufgestellt werden

            var myPlan = new CoursePlan();
            myPlan.UserId = GetCurrentUser().Id;

            myPlan.Name = nameofplan;
            myPlan.Description = description;



            var wi = Db.Curricula.SingleOrDefault(x => x.ShortName.Equals("WI"));

            var richtung = wi.CurriculumGroups.Where(x => 
                x.Name.Equals("1") || x.Name.Equals("2") ||
                x.Name.Contains(Schwerpunkt));


            /*
            // Hier jetzt die Module
            // wir legen uns eine neue leere Liste an
            var modulListe = new List<CurriculumModule>();

            var gsSem1 = Db.CurriculumModules.Where(m => m.Groups.Any(g => g.Name.Equals("1") && g.Curriculum.ShortName.Equals("WI"))).ToList();
            modulListe.AddRange(gsSem1);

            var gsSem2 = Db.CurriculumModules.Where(m => m.Groups.Any(g => g.Name.Equals("2") && g.Curriculum.ShortName.Equals("WI"))).ToList();
            modulListe.AddRange(gsSem2);

            if (Schwerpunkt.Equals("TEC"))
            {
                // nimm alle Module aus den TEC Gruppen
                var alleTECModule = Db.CurriculumModules.Where(m => m.Groups.Any(g => g.Name.Contains("TEC") && g.Curriculum.ShortName.Equals("WI"))).ToList();

                // pro Modul muss ich jetzt noch das Mapping aufbauen
                modulListe.AddRange(alleTECModule);
            }
            else if (Schwerpunkt.Equals("INF"))
            {
                var alleINFModule = Db.CurriculumModules.Where(m => m.Groups.Any(g => g.Name.Contains("INF") && g.Curriculum.ShortName.Equals("WI"))).ToList();
                modulListe.AddRange(alleINFModule);
            }
            else if (Schwerpunkt.Equals("BIO"))
            {
                var alleBIOModule = Db.CurriculumModules.Where(m => m.Groups.Any(g => g.Name.Contains("BIO") && g.Curriculum.ShortName.Equals("WI"))).ToList();
                modulListe.AddRange(alleBIOModule);
            }
            else
            {

            }

            // unser moduleListe enthält jetzt alle Module 
            // - des Grundstudiums
            // - der jeweiligen Studienrichtung



            // jetzt muss man die "falschen" Sprachen entfernen


            if (Sprachkonzept.Equals("Konzept1"))
            {
                var sprachModule = new List<CurriculumModule>();
                if (Fachsprache.Contains("EN"))
                {
                    var sprachModuleFR = modulListe.Where(m => m.Name.Contains("Französisch")).ToList();
                    sprachModule.AddRange(sprachModuleFR);
                }
                else if (Fachsprache.Contains("FR"))
                {
                    var sprachModuleEN = modulListe.Where(m => m.Name.Contains("Englisch")).ToList();
                    sprachModule.AddRange(sprachModuleEN);
                }

                // pro Modul muss ich jetzt noch das Mapping aufbauen
                foreach (var module in sprachModule)
                {
                    modulListe.Remove(module);
                }

            }
            else if (Sprachkonzept.Equals("Konzept2"))
            {
                var sprachModule2 = new List<CurriculumModule>();
                if (Fachsprache.Contains("EN"))
                {
                    var sprachmoduleFR2 = modulListe.Where(m => m.Name.Contains("Französisch 2") || m.Name.Contains("Französisch 3")).ToList();
                    sprachModule2.AddRange(sprachmoduleFR2);
                }
                else
                {
                    var sprachmoduleEN2 = modulListe.Where(m => m.Name.Contains("Englisch 2") || m.Name.Contains("Englisch 3")).ToList();
                    sprachModule2.AddRange(sprachmoduleEN2);
                }
                foreach (var module in sprachModule2)
                {
                    modulListe.Remove(module);
                }

                var wpm = modulListe.SingleOrDefault(m => m.Name.Equals("Fachwissenschaftliches Wahlpflichtmodul III"));
                if (wpm != null)
                {
                    modulListe.Remove(wpm);
                }
            }
            else if (Sprachkonzept.Equals("Konzept3"))
            {
                var sprachModul3 = new List<CurriculumModule>();
                var sprachModul3b = modulListe.Where(m => m.Name.Contains("Englisch 3") || m.Name.Contains("Französisch 3")).ToList();
                sprachModul3.AddRange(sprachModul3b);
                foreach (var module in sprachModul3)
                {
                    modulListe.Remove(module);
                }
                var wpm = modulListe.SingleOrDefault(m => m.Name.Equals("Fachwissenschaftliches Wahlpflichtmodul III"));
                if (wpm != null)
                {
                    modulListe.Remove(wpm);
                }
            }
            else
            {
            }
            */

            // jetzt ist unser Modulliste komplett
            // jetzt noch die Mappings aufbauen

            // willkürliche Annahme: jeder fängt in diesem Semester
            var firstSemester = SemesterService.GetSemester(DateTime.Today);

            foreach (var curriculumGroup in richtung)
            {
                // was kennen wir von unserem Modul?
                // willkürliche Annahme: das Semester steht in der Gruppe drin
                // Annahme 2: Es existiert immer mindestens 1 Gruppe!
                var words = curriculumGroup.Name.Split(' ');

                // Annahme: im ersten Wort steckt die Semesternummer
                var semNr = int.Parse(words[0]);

                var moduleSemester = GetSemester(firstSemester, semNr - 1);

                if (!myPlan.Semester.Contains(moduleSemester))
                {
                    myPlan.Semester.Add(moduleSemester);
                }

                /*
                foreach (var accreditation in curriculumGroup.Accreditations)
                {

                    var mapping = new ModuleMapping
                    {
                        Module = accreditation.Module,
                        Semester = moduleSemester,
                        CurriculumSemester = moduleSemester
                    };

                    myPlan.ModuleMappings.Add(mapping);

                }
                */
            }

            // den neuen Plan zu DB hinzufügen
            Db.CoursePlans.Add(myPlan);
            Db.SaveChanges();

            return RedirectToAction("Details", new {id=myPlan.Id});
        }

        private Semester GetSemester(Semester startSemester, int offset)
        {
            if (offset == 0)
            {
                return Db.Semesters.SingleOrDefault(s => s.Id == startSemester.Id);
            }

            // von allen zukünftigen Semestern
            // ordne sie nach Vorlesungsbeginn
            // nimm die Anzahl, die der offset angibt
            // das letzte Element in der Liste ist das gesuchte.
            // das muss ich in zwei Schritten machen
            // 1. geht es überhaupt
            // 2. wenn ja, dann mach
            // var sem = Db.Semesters.Where(s => s.StartCourses > startSemester.StartCourses).OrderBy(s => s.StartCourses).Take(offset).LastOrDefault();

            // was ist das Problem?
            // wenn es nicht genug zukünftige Semester gibt!
            // oben ist die Annahme verbaut, dass die Datenbank mit unendlich vielen Semestern gefüllt ist.
            var futureSemesterList = Db.Semesters.Where(s => s.StartCourses > startSemester.StartCourses).OrderBy(s => s.StartCourses).ToList();
            if (futureSemesterList.Count < offset)
            {
                // es gibt zu wenige Semester
                // wir müssen hier die fehlenden Semester erzeugen
                Semester sem = null;
                for (int i = futureSemesterList.Count + 1; i <= offset; i++)
                {
                    // Semester erzeugen

                    //was wissen wir
                    // SS16, WS16
                    // nimm 3. und 4. Zeichen => rechne daraus das Jahr
                    // ermittle Typ SS / WS
                    // es geht auch anders
                    // WS fangen immer im Oktober am
                    // SS fangen immer im März an

                    // wir müssen berechnen
                    // den Namen WS/SS und das Jahr
                    // den Vorlesungsbeginn (15.03. / 01.10.)
                    var name = "";
                    var year = 0;
                    DateTime startDate;
                    if (startSemester.StartCourses.Month == 3)
                    {
                        // Sommersemester
                        // dann sind die geraden offsets bzw. i sind dann auch wieder SS
                        // die ungeraden i sind WS
                        if (i % 2 == 0)
                        {
                            // gerade
                            year = startSemester.StartCourses.Year + i / 2;
                            name = string.Format("SS{0}", year - 2000);
                            startDate = new DateTime(year, 3, 15);
                        }
                        else
                        {
                            // ungerade
                            year = startSemester.StartCourses.Year + (i - 1) / 2;
                            name = string.Format("WS{0}", year - 2000);
                            startDate = new DateTime(year, 10, 1);
                        }
                    }
                    else
                    {
                        // Wintersemester
                        if (i % 2 == 0)
                        {
                            // gerade
                            year = startSemester.StartCourses.Year + i / 2;
                            name = string.Format("WS{0}", year - 2000);
                            startDate = new DateTime(year, 10, 1);
                        }
                        else
                        {
                            // ungerade
                            year = startSemester.StartCourses.Year + (i + 1) / 2;
                            name = string.Format("SS{0}", year - 2000);
                            startDate = new DateTime(year, 3, 15);
                        }
                    }

                    sem = new Semester
                    {
                        Name = name,
                        StartCourses = startDate,
                        EndCourses = startDate.AddDays(112)
                    };

                    Db.Semesters.Add(sem);
                    Db.SaveChanges();
                }

                // das zuletzt erzeugte Semester ist das gesuchte
                return sem;
            }
            else
            {
                return futureSemesterList.Take(offset).LastOrDefault();

            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Details(Guid? id)
        {
            if (!id.HasValue)
                return DummyDetails();

            // es gibt einen Plan
            var myPlan = Db.CoursePlans.SingleOrDefault(x => x.Id == id.Value);

            var model = new CoursePlanPlanningViewModel();
            model.CoursePlan = myPlan;

            List<Semester> semesterList = null;

            if (myPlan.Semester.Any())
            {
                semesterList = myPlan.Semester.ToList();
            }
            else 
            {
                semesterList = (from x in myPlan.ModuleMappings
                                    select
                                            x.CurriculumSemester
                                        ).Distinct().ToList();

                // hier müsste man noch die Semesterliste aufbauen und gleich wieder speichern
                foreach (var sem in semesterList)
                {
                    myPlan.Semester.Add(sem);
                    Db.SaveChanges();
                }
            }



            var orderedSemesterList = semesterList.OrderBy(x => x.StartCourses);

            foreach (var sem in orderedSemesterList)
            {
                var semModel = new CoursePlanSemesterViewModel();
                semModel.Semester = sem;

                var nextSem = orderedSemesterList.FirstOrDefault(x => x.StartCourses > sem.StartCourses);
                semModel.NextSemester = nextSem != null ? nextSem : null;

                var prevSem = orderedSemesterList.LastOrDefault(x => x.StartCourses < sem.StartCourses);
                semModel.PrevSemester = prevSem != null ? prevSem : null;


                // Liste der ModuleMapings aus Plan lesen
                // Module nach individuellem Plan
                var modules = myPlan.ModuleMappings.Where(x => x.Semester.Id == sem.Id);
                semModel.Modules.AddRange(modules);

                // Module nach SPO Zuordnung
                modules = myPlan.ModuleMappings.Where(x => x.CurriculumSemester.Id == sem.Id);
                semModel.CurriculumModules.AddRange(modules);

                model.SemesterModules.Add(semModel);


            }



            return View("Details3", model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="planId"></param>
        /// <param name="mappingId"></param>
        /// <param name="targetSemId"></param>
        /// <returns></returns>
        public ActionResult MoveMapping(Guid planId, Guid mappingId, Guid targetSemId)
        {
            var plan = Db.CoursePlans.SingleOrDefault(x => x.Id == planId);

            var mapping = plan.ModuleMappings.SingleOrDefault(x => x.Id == mappingId);

            var targetSemester = Db.Semesters.SingleOrDefault(x => x.Id == targetSemId);

            mapping.Semester = targetSemester;
            Db.SaveChanges();


            return RedirectToAction("Details", new { id = planId });
        }


        private ActionResult DummyDetails()
        {
            // hier kann man schon den Plan holen
            /*
             var myPlan = Db.CoursePlans.First();

             var sem1 = Db.Semesters.Single();
             myPlan.ModuleMappings.Where(m => m.Semester.Id == sem1.Id);

             var sem2 = Db.Semesters.Single();
             myPlan.ModuleMappings.Where(m => m.Semester.Id == sem2.Id);


             // TODO: persönlichen Studienplan erstellen
             // TODO: geht nicht in einem Schritt
             // TODO: Erster Schritt: Module des Studiengangs WI hier auslesen und aufbereiten
             */


            //var modulListe = new List<CurriculumModule>();
            // Liste aller Module im 1. Semester des Studiengangs WI
            // Alle Module, die einer Curriculumsgruppe mit dem Namen "1" und dem Curriculum mit Kurzname "WI" zugeordnet sind
            var modulListeSem1 = Db.CurriculumModules.Where(m =>
                m.Groups.Any(g =>
                    g.Name.Equals("1") && g.Curriculum.ShortName.Equals("WI")))
                .ToList();

            var modulListeSem2 = Db.CurriculumModules.Where(m =>
                m.Groups.Any(g =>
                    g.Name.Equals("2") && g.Curriculum.ShortName.Equals("WI")))
                .ToList();

            var modulListeSem3 = Db.CurriculumModules.Where(m =>
                m.Groups.Any(g =>
                    g.Name.Equals("3 INF") && g.Curriculum.ShortName.Equals("WI")))
                .ToList();

            var modulListeSem4 = Db.CurriculumModules.Where(m =>
                m.Groups.Any(g =>
                    g.Name.Equals("4 INF") && g.Curriculum.ShortName.Equals("WI")))
                .ToList();

            var modulListeSem5 = Db.CurriculumModules.Where(m =>
                m.Groups.Any(g =>
                    g.Name.Equals("5 INF") && g.Curriculum.ShortName.Equals("WI")))
                .ToList();

            var modulListeSem6 = Db.CurriculumModules.Where(m =>
                m.Groups.Any(g =>
                    g.Name.Equals("6 INF") && g.Curriculum.ShortName.Equals("WI")))
                .ToList();

            var modulListeSem7 = Db.CurriculumModules.Where(m =>
                m.Groups.Any(g =>
                    g.Name.Equals("7 INF") && g.Curriculum.ShortName.Equals("WI")))
                .ToList();


            // Übungen: andere Abfragen
            // alle Module, die mit M anfangen
            // alle Module, die weniger als 5 ECTS haben
            // alle Module, die den Wortteil "technik" beinhalten

            // Lösung?
            var Aufgabe1 = Db.CurriculumModules.Where(m =>
                m.Groups.Any(g =>
                    g.Name.StartsWith("M")))
                .ToList();

            var Aufgabe2 = Db.CurriculumModules.Where(m => m.ECTS < 5);

            var Aufgabe3 = Db.CurriculumModules.Where(m =>
                m.Groups.Any(g =>
                    g.Name.Contains("technik")));


            var model = new CoursePlanViewModel();
            model.Semester1 = modulListeSem1;
            model.Semester2 = modulListeSem2;
            model.Semester3 = modulListeSem3;
            model.Semester4 = modulListeSem4;
            model.Semester5 = modulListeSem5;
            model.Semester6 = modulListeSem6;
            model.Semester7 = modulListeSem7;


            // Übergabe des mit Daten gefüllten Models an den View
            return View(model);

        
        }
        /*
        public ActionResult Delete(Guid planId, Guid mappingId)
        {

            var plan = Db.CoursePlans.Single(x => x.Id == planId);
            Db.CoursePlans.Remove(plan);
            Db.SaveChanges();

            return View("Index");

        }
        */
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Delete(Guid id)
        {
            // liefert das leere Formular
            var myPlan = Db.CoursePlans.SingleOrDefault(x => x.Id == id);

            return RedirectToAction("Index");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Change(Guid id)
        {
            // liefert das leere Formular
            var myPlan = Db.CoursePlans.SingleOrDefault(x => x.Id == id);

            return View("Change", myPlan);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Change(CoursePlan model)
           {
            var myPlan = Db.CoursePlans.SingleOrDefault(x => x.Id == model.Id);

            if (myPlan != null)
            {
                myPlan.Name = model.Name;
                myPlan.Description = model.Description;
                myPlan.IsFavorit = model.IsFavorit;
                
                Db.SaveChanges();
            }

               return RedirectToAction ("Index");
            }

   
   
        /// <summary>
        /// 
        /// </summary>
        /// <param name="planId"></param>
        /// <returns></returns>
        public ActionResult AddSemester(Guid planId)
        {
            var plan = Db.CoursePlans.Single(x => x.Id == planId);

            // letztes Semester des plans ermitteln
            var lastSemester = plan.Semester.OrderBy(s => s.EndCourses).Last();

            var nextSemester = GetSemester(lastSemester, 1);

            plan.Semester.Add(nextSemester);
            
            Db.SaveChanges();


            return RedirectToAction("Details", new { id = planId });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Overview()
        {
            return View("Overview");
        }
    }

}

