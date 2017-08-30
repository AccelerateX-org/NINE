using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MyStik.TimeTable.Data;
using MyStik.TimeTable.Web.Models;

namespace MyStik.TimeTable.Web.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public class ModuleController : BaseController
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            var model = new List<ModuleCatalogViewModel>();

            var modules = Db.CurriculumModules.ToList();

            foreach (var module in modules)
            {
                var subModel = new ModuleCatalogViewModel();
                subModel.Module = module;
                model.Add(subModel);
            }

            return View(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Details(Guid id)
        {
            var model = Db.CurriculumModules.SingleOrDefault(x => x.Id == id);
            ViewBag.Member = GetMyMembership();
            return View(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Create(CurriculumModule model)
        {
            var member = GetMyMembership();

            model.MV = member;
            Db.CurriculumModules.Add(model);
            Db.SaveChanges();

            return RedirectToAction("Details", new {id = model.Id});
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult LinkCourses()
        {
            // Alle Kurse suchen
            var modules = Db.CurriculumModules.ToList();

            foreach (var module in modules)
            {
                foreach (var moduleCourse in module.ModuleCourses)
                {
                    // Holzhammer, müsste feiner gehen
                    // Die ExternalId ist die Untis Nummer
                    // Das Fachkürzel wird in der Kurzbezeichnung gespeichert
                    var courseList = Db.Activities.OfType<Course>()
                        .Where(c => !string.IsNullOrEmpty(c.ShortName) && c.ShortName.Equals(moduleCourse.ExternalId)).ToList();

                    foreach (var course in courseList)
                    {
                        // TODO: hier noch die richtige Zugehröigkeit kontrollieren
                        moduleCourse.Courses.Add(course);
                    }
                }
                Db.SaveChanges();
            }


            return RedirectToAction("Index");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Accredition()
        {
            // für jedes Modul
            // Kriterium im Studiengang anlegen
            // Alle Gruppen transportieren => mit Regel
            // Jeweils eine Akkreditierunganlegen

            var modules = Db.CurriculumModules.ToList();

            foreach (var module in modules)
            {
                foreach (var @group in module.Groups)
                {
                    var curriculum = group.Curriculum;

                    var criteria = curriculum.Criterias.FirstOrDefault(c => c.Name.Equals(module.Name));

                    if (criteria == null)
                    {
                        // nur Kriterien anlegen, die es noch nicht gibt
                        criteria = new CurriculumCriteria
                        {
                            Curriculum = curriculum,
                            Name = module.Name,
                        };

                        Db.Criterias.Add(criteria);
                        Db.SaveChanges();
                    }

                    // nach der Gruppe suchen
                    var ruleForGroup = criteria.Rules.FirstOrDefault(g => g.Group.Id == @group.Id);
                    if (ruleForGroup == null)
                    {
                        ruleForGroup = new CriteriaRule
                        {
                            Group = @group,
                            Criteria = criteria
                        };

                        Db.Rules.Add(ruleForGroup);
                        Db.SaveChanges();
                    }

                    // Akkreditierung - auch für jede Gruppe!
                    var accredForModule = criteria.Accreditations.FirstOrDefault(k => k.Module.Id == module.Id);

                    if (accredForModule == null)
                    {
                        accredForModule = new ModuleAccreditation
                        {
                            Module = module,
                            Criteria = criteria,
                        };
                        Db.Accreditations.Add(accredForModule);
                        Db.SaveChanges();
                    }
                }
            }

            return RedirectToAction("Index");
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult ClearCatalog()
        {
            var allModules = Db.CurriculumModules.ToList();

            foreach (var curriculumModule in allModules)
            {
                var allCourses = curriculumModule.ModuleCourses.ToList();

                foreach (var moduleCourse in allCourses)
                {
                    Db.ModuleCourses.Remove(moduleCourse);
                }

                Db.CurriculumModules.Remove(curriculumModule);
            }

            Db.SaveChanges();

            return RedirectToAction("Index");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult CreateExam(Guid id)
        {
            var model = Db.CurriculumModules.SingleOrDefault(x => x.Id == id);

            return View(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="examtype"></param>
        /// <returns></returns>
        public ActionResult CreateExamByType(Guid id, string examtype)
        {
            var model = Db.CurriculumModules.SingleOrDefault(x => x.Id == id);
            var member = GetMyMembership();

            var exam = new ModuleExam();
            exam.Examiners.Add(member);

            if (examtype.Equals("SP"))
            {
                exam.ExamType = ExamType.SP;    
            }
            else if (examtype.Equals("MP"))
            {
                exam.ExamType = ExamType.MP;
            }
            else
            {
                exam.ExamType = ExamType.PA;
            }

            model.ModuleExams.Add(exam);

            Db.ModuleExams.Add(exam);
            Db.SaveChanges();

            return RedirectToAction("Details", new {id = id});
        }
    }
}