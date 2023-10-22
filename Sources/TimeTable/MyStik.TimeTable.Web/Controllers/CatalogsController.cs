using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using MyStik.TimeTable.Data;
using MyStik.TimeTable.Web.Models;
using Newtonsoft.Json;
using Org.BouncyCastle.Math.EC.Rfc7748;

namespace MyStik.TimeTable.Web.Controllers
{
    public class CatalogsController : BaseController
    {
        // GET: Catalogs
        public ActionResult Index(Guid? id)
        {
            var org = id == null ? GetMyOrganisation() : GetOrganiser(id.Value);

            var model = new OrganiserViewModel
            {
                Organiser = org
            };

            ViewBag.UserRight = GetUserRight(org);
            ViewBag.CurrentSemester = SemesterService.GetSemester(DateTime.Today);
            ViewBag.NextSemester = SemesterService.GetNextSemester(DateTime.Today);

            return View(model);
        }

        public ActionResult Details(Guid id)
        {
            var model = Db.CurriculumModuleCatalogs.FirstOrDefault(x => x.Id == id);

            foreach (var module in model.Modules.ToList())
            {
                if (module.ModuleResponsibilities.Count() == 2)
                {
                    if (module.ModuleResponsibilities.First().Member.Id ==
                        module.ModuleResponsibilities.Last().Member.Id)
                    {
                        var resp = module.ModuleResponsibilities.Last();

                        Db.ModuleResponsibilities.Remove(resp);
                    }
                }

            }

            Db.SaveChanges();


            ViewBag.UserRight = GetUserRight(model.Organiser);
            ViewBag.CurrentSemester = SemesterService.GetSemester(DateTime.Today);
            ViewBag.NextSemester = SemesterService.GetNextSemester(DateTime.Today);

            return View(model);
        }

        public ActionResult AllModules(Guid? id)
        {
            var org = id == null ? GetMyOrganisation() : GetOrganiser(id.Value);

            var model = Db.CurriculumModules.Where(x => x.Catalog.Organiser.Id == org.Id).ToList();

            ViewBag.Organiser = org;
            ViewBag.UserRight = GetUserRight(org);
            ViewBag.CurrentSemester = SemesterService.GetSemester(DateTime.Today);
            ViewBag.NextSemester = SemesterService.GetNextSemester(DateTime.Today);

            return View(model);
        }



        public ActionResult Import(Guid id)
        {
            var org = Db.Organisers.SingleOrDefault(x => x.Id == id);


            var model = new OrganiserImportModel
            {
                Organiser = org
            };


            return View(model);
        }


        [HttpPost]
        public ActionResult Import(OrganiserImportModel model)
        {
            string tempFile = Path.GetTempFileName();

            // Speichern der Config-Dateien
            model.AttachmentStructure?.SaveAs(tempFile);

            CurriculumModuleCatalogImportModel catalog = null;

            using (StreamReader file = System.IO.File.OpenText(tempFile))
            {
                var serializer = new JsonSerializer();
                catalog = (CurriculumModuleCatalogImportModel)serializer.Deserialize(file,
                    typeof(CurriculumModuleCatalogImportModel));
            }

            var org = Db.Organisers.SingleOrDefault(x => x.Id == model.Organiser.Id);


            if (catalog == null || org == null)
                return View();

            var cat = org.ModuleCatalogs.FirstOrDefault(x => x.Tag.Equals(catalog.tag));

            if (cat == null)
            {
                cat = new CurriculumModuleCatalog
                {
                    Name = catalog.name,
                    Tag = catalog.tag,
                    Organiser = org
                };

                Db.CurriculumModuleCatalogs.Add(cat);
            }

            // Bis zur Einführung der Labels keine Module doppelt
            foreach (var catalogModule in catalog.modules)
            {
                var module = cat.Modules.FirstOrDefault(x => x.Tag.Equals(catalogModule.tag));


                if (module == null)
                {
                    module = new CurriculumModule
                    {
                        Name = catalogModule.name,
                        Tag = catalogModule.tag,
                        Catalog = cat,
                    };


                    Db.CurriculumModules.Add(module);
                }

                // die Modulverantwortlichen
                foreach (var moduleResp in catalogModule.responsible)
                {
                    var member = org.Members.FirstOrDefault(x => x.ShortName.Equals(moduleResp.tag));

                    if (member != null)
                    {
                        var resp = new ModuleResponsibility { Member = member, Module = module };
                        module.ModuleResponsibilities.Add(resp);
                        Db.ModuleResponsibilities.Add(resp);
                    }
                }


                // die Fächer
                foreach (var moduleSubject in catalogModule.subjects)
                {
                    var teachingFormat = Db.TeachingFormats.FirstOrDefault(x => x.Tag.Equals(moduleSubject.type));
                    if (teachingFormat == null)
                    {
                        teachingFormat = new TeachingFormat
                        {
                            Tag = moduleSubject.type,
                            CWN = 15,
                        };

                        Db.TeachingFormats.Add(teachingFormat);
                        Db.SaveChanges();
                    }

                    var subject = module.ModuleSubjects.FirstOrDefault(x => x.Tag.Equals(moduleSubject.tag));

                    if (subject == null)
                    {
                        subject = new ModuleSubject
                        {
                            Name = moduleSubject.name,
                            Tag = moduleSubject.tag,
                            SWS = moduleSubject.sws,
                            TeachingFormat = teachingFormat,
                            Module = module
                        };

                        Db.ModuleCourses.Add(subject);
                    }
                }

                // die Prüfungsformen
                foreach (var moduleExam in catalogModule.exams)
                {
                    var exam = module.ExaminationOptions.FirstOrDefault(x => x.Name.Equals(moduleExam.name));

                    if (exam == null)
                    {
                        exam = new ExaminationOption
                        {
                            Name = moduleExam.name,
                            Module = module
                        };

                        Db.ExaminationOptions.Add(exam);
                    }

                    foreach (var examFraction in moduleExam.fractions)
                    {
                        var examinationForm =
                            Db.ExaminationForms.FirstOrDefault(x => x.ShortName.Equals(examFraction.tag));

                        if (examinationForm == null)
                        {
                            examinationForm = new ExaminationForm
                            {
                                ShortName = examFraction.tag
                            };

                            Db.ExaminationForms.Add(examinationForm);
                            Db.SaveChanges();
                        }

                        var fraction =
                            exam.Fractions.FirstOrDefault(x => x.Form.ShortName.Equals(examinationForm.ShortName));

                        if (fraction == null)
                        {
                            fraction = new ExaminationFraction
                            {
                                Form = examinationForm,
                                ExaminationOption = exam,
                                MinDuration = examFraction.minDuration,
                                MaxDuration = examFraction.maxDuration,
                                Weight = examFraction.weight
                            };

                            Db.ExaminationFractions.Add(fraction);
                        }

                    }

                }
            }

            Db.SaveChanges();

            return RedirectToAction("Index", new { id = org.Id });
        }

        public FileResult Export(Guid id)
        {
            var org = Db.Organisers.SingleOrDefault(x => x.Id == id);

            var ms = new MemoryStream();
            var writer = new StreamWriter(ms, Encoding.Default);

            writer.Write(
                "Catalog;Tag;Titel");

            writer.Write(Environment.NewLine);

            foreach (var catalog in org.ModuleCatalogs.OrderBy(x => x.Tag))
            {
                foreach (var module in catalog.Modules.OrderBy(x => x.Tag))
                {
                    writer.Write("{0};{1};{2}",
                        catalog.Tag,
                        module.Tag,
                        module.Name);
                    writer.Write(Environment.NewLine);
                }
            }

            writer.Flush();
            writer.Dispose();

            var sb = new StringBuilder();
            sb.Append("Module_");
            sb.Append(org.ShortName);
            sb.Append("_");
            sb.Append(DateTime.Today.ToString("yyyyMMdd"));
            sb.Append(".csv");

            return File(ms.GetBuffer(), "text/csv", sb.ToString());
        }


        public ActionResult DeleteAll(Guid id)
        {
            var org = GetMyOrganisation();

            var org2 = GetOrganiser(id);

            var userRight = GetUserRight(org);

            if (org.Id != org2.Id || !userRight.IsCurriculumAdmin)
            {
                return RedirectToAction("Index");
            }

            foreach (var catalog in org.ModuleCatalogs.ToList())
            {
                _DeleteCatalog(catalog.Id);
            }


            return RedirectToAction("Index");
        }

        public ActionResult DeleteCatalog(Guid id)
        {
            _DeleteCatalog(id);
            return RedirectToAction("Index");
        }


        private void _DeleteCatalog(Guid catId)
        {
            var catalog = Db.CurriculumModuleCatalogs.SingleOrDefault(x => x.Id == catId);
            if (catalog == null)
                return;

            if (catalog.Modules.Any())
                return;

            foreach (var resp in catalog.CatalogResponsibilities.ToList())
            {
                Db.CatalogResponsibilities.Remove(resp);
            }

            Db.CurriculumModuleCatalogs.Remove(catalog);

            Db.SaveChanges();
        }

        public ActionResult CreateCatalog(Guid? id)
        {
            var org = id == null ? GetMyOrganisation() : GetOrganiser(id.Value);

            var model = new CatalogCreateModel
            {
                orgId = org.Id,

                Organiser = org
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult CreateCatalog(CatalogCreateModel model)
        {
            var org = GetOrganiser(model.orgId);

            var isDuplicate = org.ModuleCatalogs.Any(x => x.Tag.ToUpper().Equals(model.Tag.ToUpper()));

            if (!isDuplicate)
            {
                var cat = new CurriculumModuleCatalog
                {
                    Organiser = org,
                    Tag = model.Tag,
                    Name = model.Name,
                    Description = model.Description
                };

                Db.CurriculumModuleCatalogs.Add(cat);
                Db.SaveChanges();
            }

            return RedirectToAction("Index", new { id = org.Id });
        }

        public ActionResult EditGeneral(Guid id)
        {
            var catalog = Db.CurriculumModuleCatalogs.SingleOrDefault(x => x.Id == id);

            var model = new CurriculumModuleCreateModel();
            model.catalogId = catalog.Id;
            model.Name = catalog.Name;
            model.Tag = catalog.Tag;

            return View(model);
        }


        [HttpPost]
        public ActionResult EditGeneral(CurriculumModuleCreateModel model)
        {
            var catalog = Db.CurriculumModuleCatalogs.SingleOrDefault(x => x.Id == model.catalogId);

            var org = catalog.Organiser;
            var isDuplicate = org.ModuleCatalogs.Any(x => x.Tag.Equals(model.Tag) && x.Id != model.catalogId);

            if (!isDuplicate)
            {
                catalog.Tag = model.Tag;
                catalog.Name = model.Name;
                Db.SaveChanges();
            }

            return RedirectToAction("Details", new { id = model.catalogId });
        }

        public ActionResult EditResponsibilities(Guid id)
        {
            var catalog = Db.CurriculumModuleCatalogs.SingleOrDefault(x => x.Id == id);

            return View(catalog);
        }

        [HttpPost]
        public ActionResult SaveResponsibilities(Guid catalogId, ICollection<Guid> DozIds)
        {
            var catalog = Db.CurriculumModuleCatalogs.SingleOrDefault(x => x.Id == catalogId);

            var resp2delete = new List<CatalogResponsibility>();
            foreach (var responsibility in catalog.CatalogResponsibilities)
            {
                if (!DozIds.Contains(responsibility.Member.Id))
                {
                    resp2delete.Add(responsibility);
                }
            }

            foreach (var responsibility in resp2delete)
            {
                catalog.CatalogResponsibilities.Remove(responsibility);
                Db.CatalogResponsibilities.Remove(responsibility);
            }

            var doz2create = new List<Guid>();
            foreach (var dozId in DozIds)
            {
                var isHere = catalog.CatalogResponsibilities.Any(x => x.Member.Id == dozId);

                if (!isHere)
                {
                    doz2create.Add(dozId);
                }
            }

            foreach (var dozId in doz2create)
            {
                var member = Db.Members.SingleOrDefault(x => x.Id == dozId);
                var resp = new CatalogResponsibility
                {
                    Catalog = catalog,
                    Member = member
                };
                Db.CatalogResponsibilities.Add(resp);
            }

            Db.SaveChanges();

            return null;
        }

        public ActionResult MoveModules(Guid? id)
        {
            // Liste der Organisationen, in der der user aktiv ist

            // Liste der Kataloge, zu dnen Zugang besteht
            // weil Admin
            // weil Katalogverantwortlicher

            var user = GetCurrentUser();
            var org = GetMyOrganisation();

            var model = new MoveModuleModel
            {
                Organiser = org,
                Organises = new List<ActivityOrganiser>()
            };

            model.Organises.Add(org);

            return View(model);
        }

        [HttpPost]
        public PartialViewResult GetCatalogs(Guid orgId)
        {
            var org = Db.Organisers.SingleOrDefault(x => x.Id == orgId);

            var currs = org.ModuleCatalogs.ToList();

            var model = currs
                .OrderBy(g => g.Tag)
                .ToList();

            return PartialView("_CatalogSelectList", model);
        }

        [HttpPost]
        public PartialViewResult GetModules(Guid catId, string side)
        {
            var catalog = Db.CurriculumModuleCatalogs.SingleOrDefault(x => x.Id == catId);

            var model = catalog.Modules
                .OrderBy(g => g.Tag)
                .ToList();

            ViewBag.ListName = side + "ModuleList";

            return PartialView("_ModuleListGroup", model);
        }

        [HttpPost]
        public ActionResult MoveModulesSave(Guid catId, Guid[] moduleIds)
        {
            var catalog = Db.CurriculumModuleCatalogs.SingleOrDefault(x => x.Id == catId);

            foreach (var moduleId in moduleIds)
            {
                var module = Db.CurriculumModules.SingleOrDefault(x => x.Id == moduleId);

                if (catalog == null || module == null) continue;

                if (catalog.Modules.All(x => x.Id != module.Id))
                {
                    module.Catalog = catalog;
                }
            }

            Db.SaveChanges();

            return null;
        }


        public ActionResult PlanCatalog(Guid id)
        {
            var catalog = Db.CurriculumModuleCatalogs.SingleOrDefault(x => x.Id == id);

            var model = new CatalogPlanModel();

            model.Semester = SemesterService.GetSemester(DateTime.Today);
            model.Organiser = catalog.Organiser;
            model.Catalog = catalog;

            var currList = new List<Curriculum>();
            foreach (var module in catalog.Modules)
            {
                var currs = module.Accreditations.Select(x => x.Slot.AreaOption.Area.Curriculum).Distinct().ToList();

                foreach (var curr in currs)
                {
                    if (!currList.Contains(curr))
                    {
                        currList.Add(curr);
                    }
                }

            }

            model.Curriculum = currList.FirstOrDefault();

            var semesterList = new List<Semester>();
            semesterList.Add(SemesterService.GetNextSemester(model.Semester));
            semesterList.Add(model.Semester);

            ViewBag.Semester = semesterList;
            ViewBag.Curricula = currList;

            return View(model);
        }

        public ActionResult PlanCourse(Guid id)
        {
            var catalog = Db.CurriculumModuleCatalogs.SingleOrDefault(x => x.Id == id);

            var model = new CatalogPlanModel();

            model.Semester = SemesterService.GetSemester(DateTime.Today);
            model.Organiser = catalog.Organiser;
            model.Catalog = catalog;

            var currList = new List<Curriculum>();
            foreach (var module in catalog.Modules)
            {
                var currs = module.Accreditations.Select(x => x.Slot.AreaOption.Area.Curriculum).Distinct().ToList();

                foreach (var curr in currs)
                {
                    if (!currList.Contains(curr))
                    {
                        currList.Add(curr);
                    }
                }

            }

            model.Curriculum = currList.FirstOrDefault();

            var semesterList = new List<Semester>();
            semesterList.Add(SemesterService.GetNextSemester(model.Semester));
            semesterList.Add(model.Semester);

            ViewBag.Semester = semesterList;
            ViewBag.Curricula = currList;
            ViewBag.Organisers = Db.Organisers.Where(x => x.ModuleCatalogs.Any()).ToList();

            return View(model);
        }

    }

}
