using System;
using System.Collections.Generic;
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
    /// </summary
    [AllowAnonymous]
    public class CurriculaController : BaseController
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Index(Guid? id)
        {
            if (id.HasValue)
            {
                var model = new List<ActivityOrganiser>();
                var org = GetOrganiser(id.Value);
                model.Add(org);
                var userRight = GetUserRight(org);
                ViewBag.UserRight = userRight;
                return View(model);
            }
            else
            {
                var model = Db.Organisers.Where(x => x.Curricula.Any()).OrderBy(g => g.ShortName).ToList();
                var userRight = GetUserRight();
                ViewBag.UserRight = userRight;
                return View(model);
            }
        }



        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Create(Guid id)
        {
            var model = new CurriculaCreateModel();
            var org = GetOrganisation(id);

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

                return RedirectToAction("Details", "Curriculum", new {id = curr.Id});
            }


            return RedirectToAction("Details", "Curriculum");
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

            return RedirectToAction("Index");
        }
    }
}