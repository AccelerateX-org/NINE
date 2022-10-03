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
                foreach (var moduleCourse in module.ModuleSubjects)
                {
                    // Holzhammer, müsste feiner gehen
                    // Die ExternalId ist die Untis Nummer
                    // Das Fachkürzel wird in der Kurzbezeichnung gespeichert
                    var courseList = Db.Activities.OfType<Course>()
                        .Where(c => !string.IsNullOrEmpty(c.ShortName) && c.ShortName.Equals(moduleCourse.ExternalId)).ToList();

                    foreach (var course in courseList)
                    {
                        // TODO: hier noch die richtige Zugehröigkeit kontrollieren
                        //moduleCourse.Courses.Add(course);
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
            /*
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
                            //Curriculum = curriculum,
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
            */
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
                var allCourses = curriculumModule.ModuleSubjects.ToList();

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

            /*
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
            */
            Db.SaveChanges();

            return RedirectToAction("Details", new {id = id});
        }


        public ActionResult CreateFromCourse(Guid courseId, Guid currId)
        {
            var course = Db.Activities.OfType<Course>().SingleOrDefault(x => x.Id == courseId);
            var curr = Db.Curricula.SingleOrDefault(x => x.Id == currId);

            // jetzt die Default Packages


            var model = new ModuleCreateViewModel
            {
                Course = course,
                Curriculum = curr,
                MV = GetMyMembership(),
                Name = course.Name,
                ShortName = course.ShortName,
                Description = course.Description
            };



            return View(model);
        }
        /*
        [HttpPost]
        public ActionResult CreateFromCourse(ModuleCreateViewModel model)
        {
            var course = Db.Activities.OfType<Course>().SingleOrDefault(x => x.Id == model.Course.Id);
            var curr = Db.Curricula.SingleOrDefault(x => x.Id == model.Curriculum.Id);

            var pck = curr.Packages.SingleOrDefault(x => x.Name.Equals("Gesamt"));
            if (pck == null)
            {
                pck = new CurriculumPackage
                {
                    Curriculum = curr,
                    Name = "Gesamt",
                };

                Db.CurriculumPackages.Add(pck);
                Db.SaveChanges();
            }

            var option = pck.Options.SingleOrDefault(x => x.Name.Equals("Standard"));
            if (option == null)
            {
                option = new PackageOption
                {
                    Package = pck,
                    Name = "Standard"
                };

                Db.PackageOptions.Add(option);
                Db.SaveChanges();
            }


            // Doppelte ausschließen
            var isExisting = option.Requirements.Any(x =>
                x.Name.Equals(model.Name) || x.ShortName.Equals(model.ShortName) ||
                x.CatalogId.Equals(model.CatalogId));

            if (isExisting)
            {
                ModelState.AddModelError("", "Modul existiert bereits");
                
                // Modell wieder vervollständigen
                model.Course = course;
                model.Curriculum = curr;
                model.MV = GetMyMembership();

                return View(model);
            }


            var module = new CurriculumRequirement
            {
                Name = model.Name,
                ShortName = model.ShortName,
                CatalogId = model.CatalogId,
                ECTS = model.Ects,
                SWS = model.Sws,
                USCredits = model.UsCredits,
                LecturerInCharge = GetMyMembership(),
                Option = option,
            };

            Db.Requirements.Add(module);

            var nexus = new CourseModuleNexus
            {
                Course = course,
                Requirement = module
            };

            Db.CourseNexus.Add(nexus);

            Db.SaveChanges();


            return RedirectToAction("Admin", "Course", new {id = course.Id});
        }
        */

        /*
        [HttpPost]
        public PartialViewResult OptionList(Guid pckId)
        {
            var model = Db.CurriculumPackages.SingleOrDefault(x => x.Id == pckId);
            return PartialView("_OptionList", model);
        }

        [HttpPost]
        public PartialViewResult ModuleList(Guid optionId)
        {
            var model = Db.PackageOptions.SingleOrDefault(x => x.Id == optionId);
            return PartialView("_ModuleList", model);
        }


        [HttpPost]
        public PartialViewResult ModuleSummary(Guid moduleId)
        {
            var model = Db.Requirements.SingleOrDefault(x => x.Id == moduleId);
            return PartialView("_ModuleSummary", model);
        }
        */

        /*
        public ActionResult SelectForCourse(Guid courseId, Guid currId)
        {
            var course = Db.Activities.OfType<Course>().SingleOrDefault(x => x.Id == courseId);
            var curr = Db.Curricula.SingleOrDefault(x => x.Id == currId);

            ViewBag.Packages = curr.Packages.OrderBy(x => x.Name).Select(c => new SelectListItem
            {
                Text = c.Name,
                Value = c.Id.ToString(),
            });
            var pck = curr.Packages.FirstOrDefault();


            ViewBag.Options = pck.Options.OrderBy(x => x.Name).Select(c => new SelectListItem
            {
                Text = c.Name,
                Value = c.Id.ToString(),
            });

            var option = pck.Options.FirstOrDefault();


            ViewBag.Modules = option.Requirements.OrderBy(x => x.Name).Select(c => new SelectListItem
            {
                Text = c.Name,
                Value = c.Id.ToString(),
            });

            var module = option.Requirements.FirstOrDefault();

            var model = new ModuleSelectViewModel
            {
                Curriculum = curr,
                Course = course,
                PackageId = pck.Id,
                OptionId = option.Id,
                ModuleId = module.Id
            };

            return View(model);
        }
        */

        /*
        [HttpPost]
        public JsonResult SelectModuleForCourse(Guid courseId, Guid moduleId)
        {
            var course = Db.Activities.OfType<Course>().SingleOrDefault(x => x.Id == courseId);
            var module = Db.Requirements.SingleOrDefault(x => x.Id == moduleId);

            if (module != null)
            {
                if (string.IsNullOrEmpty(module.ShortName))
                {
                    module.ShortName = course.ShortName;
                }

                if (module.LecturerInCharge == null)
                {
                    module.LecturerInCharge = GetMyMembership();
                }


                var nexus = new CourseModuleNexus
                {
                    Course = course,
                    Requirement = module
                };

                Db.CourseNexus.Add(nexus);

                Db.SaveChanges();

            }




            return Json(new { result = "Redirect", url = Url.Action("Admin", "Course", new { id = course.Id}) });
        }
        */
    }
}