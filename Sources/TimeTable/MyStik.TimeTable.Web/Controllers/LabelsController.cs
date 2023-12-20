using MyStik.TimeTable.Data;
using MyStik.TimeTable.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyStik.TimeTable.Web.Controllers
{
    public class LabelsController : BaseController
    {
        // GET: Labels
        public ActionResult Index(Guid id)
        {
            var org = GetOrganiser(id);

            ViewBag.UserRight = GetUserRight(org);

            return View(org);
        }

        public ActionResult AllLabels()
        {
            var model = Db.ItemLabels.ToList();

            return View(model);
        }


        public ActionResult AddLabel(Guid currId)
        {
            var curr = Db.Curricula.SingleOrDefault(x => x.Id == currId);

            if (curr.LabelSet == null)
            {
                var labelSet = new ItemLabelSet();
                curr.LabelSet = labelSet;
                Db.ItemLabelSets.Add(labelSet);
                Db.SaveChanges();
            }

            var model = new ItemLabelEditModel()
            {
                Curriculum = curr,
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult AddLabel(ItemLabelEditModel model)
        {
            var curr = Db.Curricula.SingleOrDefault(x => x.Id == model.Curriculum.Id);

            var label = new ItemLabel();

            label.Name = model.Name;
            label.Description = model.Description;
            label.HtmlColor = model.HtmlColor;
            label.LabelSets.Add(curr.LabelSet);

            curr.LabelSet.ItemLabels.Add(label);

            Db.SaveChanges();

            return RedirectToAction("Index", new { id = curr.Organiser.Id });
        }



        public ActionResult EditLabel(Guid currId, Guid labelId)
        {
            var curr = Db.Curricula.SingleOrDefault(x => x.Id == currId);
            var label = Db.ItemLabels.SingleOrDefault(x => x.Id == labelId);

            if (curr.LabelSet == null)
            {
                var labelSet = new ItemLabelSet();
                curr.LabelSet = labelSet;
                Db.ItemLabelSets.Add(labelSet);
                Db.SaveChanges();
            }


            var model = new ItemLabelEditModel()
            {
                ItemLabel = label,
                Curriculum = curr,
                Name = label.Name,
                Description = label.Description,
                HtmlColor = label.HtmlColor

            };

            return View(model);
        }

        [HttpPost]
        public ActionResult EditLabel(ItemLabelEditModel model)
        {
            var curr = Db.Curricula.SingleOrDefault(x => x.Id == model.Curriculum.Id);
            var label = Db.ItemLabels.SingleOrDefault(x => x.Id == model.ItemLabel.Id);

            label.Name = model.Name;
            label.Description = model.Description;
            label.HtmlColor = model.HtmlColor;

            Db.SaveChanges();

            return RedirectToAction("Index", new { id = curr.Organiser.Id });
        }

        public ActionResult DeleteLabel(Guid currId, Guid labelId)
        {
            var curr = Db.Curricula.SingleOrDefault(x => x.Id == currId);
            var label = Db.ItemLabels.SingleOrDefault(x => x.Id == labelId);

            Db.ItemLabels.Remove(label);
            Db.SaveChanges();


            return RedirectToAction("Index", new { id = curr.Organiser.Id });
        }

    }
}