using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

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

        
    }
}