using PdfSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using MyStik.TimeTable.Web.Utils;
using TheArtOfDev.HtmlRenderer.PdfSharp;
using MyStik.TimeTable.Web.Models;
using MyStik.TimeTable.Data;
using System.Web.Mvc;
using ImageMagick.ImageOptimizers;
using Microsoft.Ajax.Utilities;

namespace MyStik.TimeTable.Web.Jobs
{
    public class StudyPlanPrintJobDescription
    {
        public Guid CurriculumId { get; set; }

        public Guid SemesterId { get; set; }

        public Guid MemberId { get; set; }

        public string Remark { get; set; }
    }

    public class StudyPlanPrintJob
    {
        public void Print(StudyPlanPrintJobDescription jobDescription)
        {
            /*
            var db = new TimeTableDbContext();

            var curr = db.Curricula.SingleOrDefault(x => x.Id == jobDescription.CurriculumId);
            var semester = db.Semesters.SingleOrDefault(x => x.Id == jobDescription.SemesterId);
            var member = db.Members.SingleOrDefault(x => x.Id == jobDescription.MemberId);

            var subjects = db.ModuleCourses.Where(x =>
                x.SubjectAccreditations.Any(c =>
                    c.Slot != null &&
                    c.Slot.AreaOption != null &&
                    c.Slot.AreaOption.Area.Curriculum.Id == curr.Id)).ToList();

            var modules = subjects.Select(x => x.Module).Distinct().ToList();

            var model = new StudyPlanViewModel
            {
                Curriculum = curr,
                Semester = semester,
                Modules = modules,
            };


            var viewName = "Views\\Shared\\_StudyPlanPrintOut.cshtml";

            var html = FakeViewExtensions.RenderPartialToString(viewName, model);

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

            db.Storages.Add(storage);

            var adv = new Advertisement
            {
                Title = "Studienplan für Semester",
                Description = "Details zum Studienplan",
                Owner = member,
                Created = DateTime.Now,
                VisibleUntil = semester.EndCourses,
                Attachment = storage,
            };

            db.Advertisements.Add(adv);

            var positing = new BoardPosting
            {
                Advertisement = adv,
                BulletinBoard = curr.BulletinBoard,
                Published = DateTime.Now
            };
            
            db.BoardPosts.Add(positing);

            db.SaveChanges();

            */

            return;
        }
    }
}