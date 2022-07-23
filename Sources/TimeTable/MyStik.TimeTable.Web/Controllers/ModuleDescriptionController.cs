using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using MyStik.TimeTable.Data;

namespace MyStik.TimeTable.Web.Controllers
{
    public class ModuleDescriptionController : BaseController
    {
        public ActionResult Details(Guid id)
        {

            var module = Db.CurriculumModules.SingleOrDefault(x => x.Id == id);
            var semester = GetSemesterFromFilter();

            var desc = module.Descriptions.FirstOrDefault(x => x.Semester.Id == semester.Id);



            // Default => lege eine Beschreibung an
            // TODO: automatisch auf dem Vorsemester, falls vorhanden

            if (desc == null)
            {
                desc = new ModuleDescription
                {
                    Description = "",
                    Module = module,
                    Semester = semester
                };

                Db.ModuleDescriptions.Add(desc);
                Db.SaveChanges();
            }

            ViewBag.UserRight = GetUserRight(GetMyOrganisation());


            return View(desc);
        }

        public ActionResult Edit(Guid id)
        {
            var desc = Db.ModuleDescriptions.SingleOrDefault(x => x.Id == id);

            var model = new ModuleDescriptionEditModel()
            {
                ModuleDescription = desc,
                DescriptionText = desc.Description
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(ModuleDescriptionEditModel model)
        {
            var desc = Db.ModuleDescriptions.SingleOrDefault(x => x.Id == model.ModuleDescription.Id);

            desc.Description = model.DescriptionText;
            Db.SaveChanges();

            return RedirectToAction("Details", new {id = model.ModuleDescription.Id});
        }


        public PartialViewResult ChangeSemester(Guid semId)
        {
            var semester = SemesterService.GetSemester(semId);
            Session["SemesterId"] = semId.ToString();

            return null;
        }


    }

        public class ModuleDescriptionEditModel
    {
        public ModuleDescription ModuleDescription { get; set; }

        [AllowHtml]
        public string DescriptionText { get; set; }
    }


}
