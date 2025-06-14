﻿using MyStik.TimeTable.Data;
using MyStik.TimeTable.Web.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyStik.TimeTable.Web.Controllers
{
    public class LabelsController : BaseController
    {
        // GET: Labels
        public ActionResult Index(Guid? id)
        {
            if (id == null)
            {
                var model = Db.ItemLabels.ToList();
                return View("AllLabels", model);
            }
            
            var org = GetOrganiser(id.Value);
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



        public ActionResult EditLabel(Guid labelId)
        {
            var label = Db.ItemLabels.SingleOrDefault(x => x.Id == labelId);


            var model = new ItemLabelEditModel()
            {
                ItemLabel = label,
                Name = label.Name,
                Description = label.Description,
                HtmlColor = label.HtmlColor
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult EditLabel(ItemLabelEditModel model)
        {
            var label = Db.ItemLabels.SingleOrDefault(x => x.Id == model.ItemLabel.Id);

            label.Name = model.Name;
            label.Description = model.Description;
            label.HtmlColor = model.HtmlColor;

            Db.SaveChanges();

            return RedirectToAction("Details", new { id = label.Id });
        }

        public ActionResult DeleteLabel(Guid labelId)
        {
            var label = Db.ItemLabels.SingleOrDefault(x => x.Id == labelId);

            return View(label);
        }


        public ActionResult DeleteLabelConfirm(Guid labelId)
        {
            var label = Db.ItemLabels.SingleOrDefault(x => x.Id == labelId);

            var institutions = Db.Institutions
                .Where(x => x.LabelSet != null && x.LabelSet.ItemLabels.Any(y => y.Id == label.Id))
                .ToList();

            var orgs = Db.Organisers
                .Where(x => x.LabelSet != null && x.LabelSet.ItemLabels.Any(y => y.Id == label.Id)).Include(activityOrganiser => activityOrganiser.Institution)
                .ToList();


            var currs = Db.Curricula
                .Where(x => x.LabelSet != null && x.LabelSet.ItemLabels.Any(y => y.Id == label.Id)).Include(curriculum => curriculum.Organiser)
                .ToList();

            Db.ItemLabels.Remove(label);
            Db.SaveChanges();

            if (currs.Any())
            {
                var org = currs.First().Organiser;
                return RedirectToAction("Index", new { id = org.Id });
            }

            return RedirectToAction("Index");
        }


        public ActionResult MergeLabel(Guid currId, Guid labelId)
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
        public ActionResult MergeLabel(ItemLabelEditModel model)
        {
            var curr = Db.Curricula.Include(curriculum => curriculum.Organiser).Include(curriculum1 =>
                curriculum1.LabelSet.ItemLabels).SingleOrDefault(x => x.Id == model.Curriculum.Id);

            var sourceLabel = curr.LabelSet.ItemLabels.FirstOrDefault(x => x.Id == model.ItemLabel.Id);
            var targetLabel = curr.LabelSet.ItemLabels.FirstOrDefault(x => x.Name.Equals(model.Name));

            if (sourceLabel != null && targetLabel != null)
            {
                var labelSets = Db.ItemLabelSets.Where(x => x.ItemLabels.Any(l => l.Id == sourceLabel.Id)).ToList();
                foreach (var labelSet in labelSets.Where(labelSet => labelSet.Id != curr.LabelSet.Id))
                {
                    labelSet.ItemLabels.Remove(sourceLabel);
                    labelSet.ItemLabels.Add(targetLabel);
                }

                Db.SaveChanges();
            }

            return RedirectToAction("Index", new { id = curr.Organiser.Id });
        }

        public ActionResult Details(Guid id)
        {
            var label = Db.ItemLabels.SingleOrDefault(x => x.Id == id);
            if (label == null)
            {
                return HttpNotFound();
            }

            ViewBag.UserRight = GetUserRight();

            return View(label);
        }
    }
}