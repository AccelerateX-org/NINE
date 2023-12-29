﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Microsoft.AspNet.SignalR.Hubs;
using MyStik.TimeTable.Data;
using MyStik.TimeTable.Web.Models;

namespace MyStik.TimeTable.Web.Controllers
{
    public class CurriculumModuleController : BaseController
    {
        /// <summary>
        /// Liste aller Module des aktuellen Benutzers
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            var user = GetCurrentUser();

            var model = Db.CurriculumModules.Where(x =>
                x.ModuleResponsibilities.Any(m =>
                    !string.IsNullOrEmpty(m.Member.UserId) && m.Member.UserId.Equals(user.Id))).ToList();

            var semester = SemesterService.GetSemester(DateTime.Today);
            var nextSemester = SemesterService.GetNextSemester(semester);

            ViewBag.Semester = semester;
            ViewBag.NextSemester = nextSemester;

            return View(model);
        }

        public ActionResult Admin(Guid id)
        {
            var model = Db.CurriculumModules.SingleOrDefault(x => x.Id == id);

            ViewBag.UserRight = GetUserRight(model.Catalog.Organiser);

            return View(model);
        }


        public ActionResult Create(Guid catalogId)
        {
            var org = GetMyOrganisation();


            var model = new CurriculumModuleCreateModel();
            model.catalogId = catalogId;
            model.SWS = 4;


            ViewBag.TeachingFormats = Db.TeachingFormats.OrderBy(x => x.Tag).Select(c => new SelectListItem
            {
                Text = c.Tag,
                Value = c.Id.ToString(),
            });


            ViewBag.ExamFormats = Db.ExaminationForms.OrderBy(x => x.ShortName).Select(c => new SelectListItem
            {
                Text = c.ShortName,
                Value = c.Id.ToString(),
            });


            return View(model);
        }


        [HttpPost]
        public ActionResult Create(CurriculumModuleCreateModel model)
        {
            var catalog = Db.CurriculumModuleCatalogs.SingleOrDefault(x => x.Id == model.catalogId);

            var member = GetMyMembership(catalog.Organiser.Id);

            var isDuplicate = catalog.Modules.Any(x => x.Tag.Equals(model.Tag.Trim()));

            if (!isDuplicate)
            {
                var teachingFormat = Db.TeachingFormats.SingleOrDefault(x => x.Id == model.TeachingFormatId);
                var examFormat = Db.ExaminationForms.SingleOrDefault(x => x.Id == model.ExamFormatId);

                var module = new CurriculumModule
                {
                    Name = model.Name,
                    NameEn = model.NameEn,
                    Tag = model.Tag,
                    Applicableness = model.Applicableness,
                    Prerequisites = model.Prequisites,
                    Catalog = catalog,
                    ModuleSubjects = new List<ModuleSubject>(),
                    ExaminationOptions = new List<ExaminationOption>()
                };

                var subject = new ModuleSubject
                {
                    Name = "Lehrveranstaltung",
                    Tag = "LV",
                    TeachingFormat = teachingFormat,
                    SWS = model.SWS,
                    Module = module
                };

                module.ModuleSubjects.Add(subject);
                Db.ModuleCourses.Add(subject);

                var examOption = new ExaminationOption
                {
                    Name = "Option A",
                    Module = module,
                    Fractions = new List<ExaminationFraction>()
                };

                var examFraction = new ExaminationFraction
                {
                    ExaminationOption = examOption,
                    Form = examFormat,
                    Weight = 1.0,
                };

                examOption.Fractions.Add(examFraction);
                Db.ExaminationFractions.Add(examFraction);

                module.ExaminationOptions.Add(examOption);
                Db.ExaminationOptions.Add(examOption);

                var resp = new ModuleResponsibility { Member = member, Module = module };

                module.ModuleResponsibilities.Add(resp);

                Db.ModuleResponsibilities.Add(resp);
                Db.CurriculumModules.Add(module);
                Db.SaveChanges();
            }

            return RedirectToAction("Details", "Catalogs",new {id = model.catalogId});
        }

        public ActionResult EditGeneral(Guid id)
        {
            var module = Db.CurriculumModules.SingleOrDefault(x => x.Id == id);

            var model = new CurriculumModuleCreateModel();
            model.moduleId = module.Id;
            model.catalogId = module.Catalog.Id;
            model.Name = module.Name;
            model.NameEn = module.NameEn;
            model.Tag = module.Tag;
            model.Prequisites = module.Prerequisites;
            model.Applicableness = module.Applicableness;


            ViewBag.UserRight = GetUserRight(module.Catalog.Organiser);

            return View(model);
        }


        [HttpPost]
        public ActionResult EditGeneral(CurriculumModuleCreateModel model)
        {
            var module = Db.CurriculumModules.SingleOrDefault(x => x.Id == model.moduleId);

            var catalog = module.Catalog;
            var isDuplicate = catalog.Modules.Any(x => x.Tag.Equals(model.Tag) && x.Id != model.moduleId);

            if (!isDuplicate)
            {
                module.Tag = model.Tag;
                module.Name = model.Name;
                module.NameEn = model.NameEn;
                module.Applicableness = model.Applicableness;
                module.Prerequisites = model.Prequisites;

                Db.SaveChanges();
            }

            return RedirectToAction("Details", "ModuleDescription", new { id = model.moduleId });
        }

        public ActionResult EditResponsibilities(Guid id)
        {
            var module = Db.CurriculumModules.SingleOrDefault(x => x.Id == id);

            return View(module);
        }

        [HttpPost]
        public ActionResult SaveResponsibilities(Guid moduleId, ICollection<Guid> DozIds)
        {
            var module = Db.CurriculumModules.SingleOrDefault(x => x.Id == moduleId);

            var resp2delete = new List<ModuleResponsibility>();
            foreach (var responsibility in module.ModuleResponsibilities)
            {
                if (!DozIds.Contains(responsibility.Member.Id))
                {
                    resp2delete.Add(responsibility);
                }
            }

            foreach (var responsibility in resp2delete)
            {
                module.ModuleResponsibilities.Remove(responsibility);
                Db.ModuleResponsibilities.Remove(responsibility);
            }

            var doz2create = new List<Guid>();
            foreach (var dozId in DozIds)
            {
                var isHere = module.ModuleResponsibilities.Any(x => x.Member.Id == dozId);

                if (!isHere)
                {
                    doz2create.Add(dozId);
                }
            }

            foreach (var dozId in doz2create)
            {
                var member = Db.Members.SingleOrDefault(x => x.Id == dozId);
                var resp = new ModuleResponsibility
                {
                    Module = module,
                    Member = member
                };
                Db.ModuleResponsibilities.Add(resp);
            }

            Db.SaveChanges();

            return null;
        }




        public ActionResult Delete(Guid id)
        {
            var model = Db.CurriculumModules.SingleOrDefault(x => x.Id == id);
            var catalog = model.Catalog;

            /*
            if (model.Accreditations.Any())
            {
                // so lange es noch Akkreditierungen hat die Struktur nicht löschen

                if (model.Descriptions.Any() || model.Accreditations.Any(x => x.ExaminationDescriptions.Any()))
                {
                    // so lange es nich Hostorie gibt zuerst diese löschen
                    foreach (var description in model.Descriptions.ToList())
                    {
                        if (description.ChangeLog != null)
                            Db.ChangeLogs.Remove(description.ChangeLog);
                        Db.ModuleDescriptions.Remove(description);
                    }

                    foreach (var accreditation in model.Accreditations.ToList())
                    {
                        foreach (var examinationDescription in accreditation.ExaminationDescriptions.ToList())
                        {
                            if (examinationDescription.ChangeLog != null)
                                Db.ChangeLogs.Remove(examinationDescription.ChangeLog);

                            Db.ExaminationDescriptions.Remove(examinationDescription);
                        }

                        foreach (var teachingDescription in accreditation.TeachingDescriptions.ToList())
                        {
                            Db.TeachingDescriptions.Remove(teachingDescription);
                        }
                    }

                    Db.SaveChanges();
                    return RedirectToAction("Admin", new { id = id });
                }

                foreach (var accreditation in model.Accreditations.ToList())
                {
                    Db.Accreditations.Remove(accreditation);
                }

                Db.SaveChanges();
                return RedirectToAction("Admin", new { id = id });
            }
            */

            // so lange es nich Hostorie gibt zuerst diese löschen
            foreach (var description in model.Descriptions.ToList())
            {
                if (description.ChangeLog != null)
                    Db.ChangeLogs.Remove(description.ChangeLog);
                Db.ModuleDescriptions.Remove(description);
            }

            /*
            foreach (var subject in model.ModuleSubjects.ToList())
            {
                foreach (var opportunity in subject.Opportunities.ToList())
                {
                    Db.SubjectOpportunities.Remove(opportunity);
                }
                Db.ModuleCourses.Remove(subject);
            }
            */

            foreach (var option in model.ExaminationOptions.ToList())
            {
                foreach (var fraction in option.Fractions.ToList())
                {
                    Db.ExaminationFractions.Remove(fraction);
                }

                Db.ExaminationOptions.Remove(option);
            }

            foreach (var moduleResponsibility in model.ModuleResponsibilities.ToList())
            {
                Db.ModuleResponsibilities.Remove(moduleResponsibility);
            }


            foreach (var moduleCourse in model.ModuleSubjects.ToList())
            {
                Db.ModuleCourses.Remove(moduleCourse);
            }


            var mappings = Db.ModuleMappings.Where(x => x.Module.Id == model.Id).ToList();
            foreach (var mapping in mappings)
            {
                mapping.Module = null;
            }

            Db.CurriculumModules.Remove(model);
            Db.SaveChanges();

            if (catalog != null)
            {
                return RedirectToAction("Details", "Catalogs", new {id=catalog.Id});
            }

            return RedirectToAction("Index", "Catalogs");
        }

    }
}