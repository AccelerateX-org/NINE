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
        public ActionResult Details(Guid? modId, Guid? semId, Guid? slotId, Guid? id)
        {
            ModuleDescription desc = null;
            Semester semester = null;
            CurriculumModule module = null;

            if (id.HasValue)
            {
                desc = Db.ModuleDescriptions.SingleOrDefault(x => x.Id == id);
                semester = desc.Semester;
                module = desc.Module;

                slotId = desc.Module.Accreditations.First().Slot.Id;
            }
            else
            {
                module = Db.CurriculumModules.SingleOrDefault(x => x.Id == modId);
                semester = Db.Semesters.SingleOrDefault(x => x.Id == semId);

                desc = module.Descriptions.FirstOrDefault(x => x.Semester.Id == semId);

                if (!slotId.HasValue)
                {
                    slotId = desc.Module.Accreditations.First().Slot.Id;
                }
            }



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


            ViewBag.BackNavigation_Action = "SlotDetails";
            ViewBag.BackNavigation_Controller = "Curriculum";
            ViewBag.BackNavigation_Id = slotId;


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
    }

        public class ModuleDescriptionEditModel
    {
        public ModuleDescription ModuleDescription { get; set; }

        [AllowHtml]
        public string DescriptionText { get; set; }
    }
}
