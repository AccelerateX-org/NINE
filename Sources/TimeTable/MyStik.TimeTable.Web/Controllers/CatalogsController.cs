using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MyStik.TimeTable.Data;
using MyStik.TimeTable.Web.Models;
using Newtonsoft.Json;

namespace MyStik.TimeTable.Web.Controllers
{
    public class CatalogsController : BaseController
    {
        // GET: Catalogs
        public ActionResult Index()
        {
            var org = GetMyOrganisation();

            var model = new OrganiserViewModel
            {
                Organiser = org
            };

            ViewBag.UserRight = GetUserRight(org);
            ViewBag.CurrentSemester = SemesterService.GetSemester(DateTime.Today);

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
                catalog = (CurriculumModuleCatalogImportModel)serializer.Deserialize(file, typeof(CurriculumModuleCatalogImportModel));
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


                var member = org.Members.FirstOrDefault(x => x.ShortName.Equals(catalogModule.responsible));

                if (module == null)
                {
                    module = new CurriculumModule
                    {
                        Name = catalogModule.name,
                        Tag = catalogModule.tag,
                        MV = member,
                        Catalog = cat,
                    };
                }

                // die Fächer
                foreach (var moduleSubject in catalogModule.subjects)
                {
                    var teachingFormat = Db.TeachingFormats.SingleOrDefault(x => x.Tag.Equals(moduleSubject.type));
                    if (teachingFormat == null)
                    {
                        teachingFormat = new TeachingFormat
                        {
                            Tag = moduleSubject.type,
                            CWN = 15,
                        };

                        Db.TeachingFormats.Add(teachingFormat);
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



                Db.CurriculumModules.Add(module);

            }

            Db.SaveChanges();

            return RedirectToAction("Index");
        }



    }
}