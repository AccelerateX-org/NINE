using Hangfire;
using MyStik.TimeTable.Data;
using MyStik.TimeTable.Web.Helpers;
using MyStik.TimeTable.Web.Jobs;
using MyStik.TimeTable.Web.Models;
using PdfSharp;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TheArtOfDev.HtmlRenderer.PdfSharp;

namespace MyStik.TimeTable.Web.Controllers
{
    public class StudyPlanPublishingModel
    {
        public Curriculum Curriculum { get; set; } 
        public Semester Semester { get; set; }
        public string Description { get; set; }

        public bool RollForward { get; set; }
    }

    public class StudyPlanController : BaseController
    {
        // GET: StudyPlan
        public ActionResult Details(Guid currId, Guid semId)
        {
            var modules = Db.CurriculumModules.Where(x =>
                x.Accreditations.Any(c =>
                    c.Slot != null &&
                    c.Slot.AreaOption != null &&
                    c.Slot.AreaOption.Area.Curriculum.Id == currId)).ToList();

            var semester = Db.Semesters.SingleOrDefault(x => x.Id == semId);
            var curriculum = Db.Curricula.SingleOrDefault(x => x.Id == currId);

            var model = new StudyPlanViewModel
            {
                Curriculum = curriculum,
                Semester = semester,
                Modules = modules
            };

            // hier muss überprüft werden, ob der aktuelle Benutzer
            // der Fakultät des Studiengangs angehört oder nicht
            ViewBag.UserRight = GetUserRight(model.Curriculum.Organiser);


            return View(model);
        }

        public ActionResult Publish(Guid currId, Guid semId)
        {
            var model = new StudyPlanPublishingModel
            {
                Curriculum = Db.Curricula.SingleOrDefault(x => x.Id == currId),
                Semester =Db.Semesters.SingleOrDefault(y => y.Id == semId)
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult Publish(StudyPlanPublishingModel model)
        {
            var job = new StudyPlanPrintJobDescription
            {
                CurriculumId = model.Curriculum.Id,
                SemesterId = model.Semester.Id,
                MemberId = GetMyMembership().Id
            };

            bool useBackground = false;

            if (useBackground)
            {
                BackgroundJob.Enqueue<StudyPlanPrintJob>(x => x.Print(job));
            }
            else
            {
                var curr = Db.Curricula.SingleOrDefault(x => x.Id == job.CurriculumId);
                var semester = Db.Semesters.SingleOrDefault(x => x.Id == job.SemesterId);
                var member = Db.Members.SingleOrDefault(x => x.Id == job.MemberId);

                var modules = Db.CurriculumModules.Where(x =>
                    x.Accreditations.Any(c =>
                        c.Slot != null &&
                        c.Slot.AreaOption != null &&
                        c.Slot.AreaOption.Area.Curriculum.Id == curr.Id)).ToList();


                var printModel = new ModuleSemesterView
                {
                    Curriculum = curr,
                    Semester = semester,
                    Modules = modules,
                };


                var viewName = "_StudyPlanPrintOutForeground";

                var html = this.RenderViewToString(viewName, printModel);

                var pdf = PdfGenerator.GeneratePdf(html, PageSize.A4);

                var storage = new BinaryStorage
                {
                    Category = "Studienplan",
                    FileType = "application/pdf",
                    Name = "Studienplan.pdf",
                    Created = DateTime.Now,
                    Description = "Automatisch erzeugt",
                };

                using (var stream = new MemoryStream())
                {
                    pdf.Save(stream, false);
                    stream.Position = 0;
                    storage.BinaryData = stream.GetBuffer();
                }

                Db.Storages.Add(storage);

                var adv = new Advertisement
                {
                    Title = "Studienplan für Semester",
                    Description = "Details zum Studienplan",
                    Owner = member,
                    Created = DateTime.Now,
                    VisibleUntil = semester.EndCourses,
                    Attachment = storage,
                };

                Db.Advertisements.Add(adv);

                var positing = new BoardPosting
                {
                    Advertisement = adv,
                    BulletinBoard = curr.BulletinBoard,
                    Published = DateTime.Now
                };

                Db.BoardPosts.Add(positing);

                Db.SaveChanges();
            }


            return RedirectToAction("Details", "Curriculum", new { id = model.Curriculum.Id });
            //return RedirectToAction("Details", new {currId = model.Curriculum.Id, semId = model.Semester.Id});
        }

    }
}