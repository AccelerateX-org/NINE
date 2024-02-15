using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MyStik.TimeTable.Data;

namespace MyStik.TimeTable.Web.Controllers
{
    public class LabelSetController : BaseController
    {
        // GET: LabelSet
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public PartialViewResult GetLabelList(Guid setId)
        {
            var labelSet = Db.ItemLabelSets.SingleOrDefault(x => x.Id == setId);

            var labels = labelSet.ItemLabels.ToList();

            var model = labels
                .OrderBy(g => g.Name)
                .ToList();

            ViewBag.ListName = setId.ToString();

            return PartialView("_LabelListGroup", model);
        }


        [HttpPost]
        public PartialViewResult GetLabelListCurriculum(Guid currId)
        {
            var curr = Db.Curricula.SingleOrDefault(x => x.Id == currId);

            if (curr != null)
            {
                ViewBag.ListName = currId.ToString();
                return PartialView("_LabelListGroupCurriculum", curr);
            }

            var inst = Db.Institutions.SingleOrDefault(x => x.Id == currId);
            if (inst != null)
            {
                ViewBag.ListName = inst.Id.ToString();
                return PartialView("_LabelListGroupInstitution", inst);
            }

            return null;
        }


        [HttpPost]
        public PartialViewResult GetLabelSelectListCurriculum(Guid currId)
        {
            var curr = Db.Curricula.SingleOrDefault(x => x.Id == currId);

            if (curr != null)
            {
                ViewBag.ListName = currId.ToString();
                return PartialView("_LabelSelectListCurriculum", curr);
            }

            var inst = Db.Institutions.SingleOrDefault(x => x.Id == currId);
            if (inst != null)
            {
                ViewBag.ListName = inst.Id.ToString();
                return PartialView("_LabelSelectListInstitution", inst);
            }

            return null;

        }

    }
}