using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MyStik.TimeTable.Data;
using MyStik.TimeTable.Web.Models;

namespace MyStik.TimeTable.Web.Controllers
{
    public class TeachingController : BaseController
    {
        // GET: Teaching
        public ActionResult Index()
        {
            var member = GetMyMembership();

            var model = Db.TeachingBuildingBlocks.Where(x => x.Lecturers.Any(m => m.Member.Id == member.Id)).ToList();

            return View(model);
        }

        public ActionResult CreateModule()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreateModule(TeachingModuleCreateModel model)
        {
            var member = GetMyMembership();

            var teachingModule = new TeachingBuildingBlock
            {
                Name = model.Name,
                Description = model.Description
            };

            var lecturer = new Lecturer
            {
                IsAdmin = true,
                Member = member
            };

            teachingModule.Lecturers.Add(lecturer);

            Db.Lecturers.Add(lecturer);
            Db.TeachingBuildingBlocks.Add(teachingModule);
            Db.SaveChanges();

            return RedirectToAction("Index");
        }

        public ActionResult ModuleDetails(Guid id)
        {
            var model = Db.TeachingBuildingBlocks.SingleOrDefault(x => x.Id == id);

            return View(model);
        }

    }
}