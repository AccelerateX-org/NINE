using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Numerics;
using System.Web;
using System.Web.Mvc;
using MyStik.Gym.Data;
using MyStik.TimeTable.Web.Areas.Gym.Models;
using Newtonsoft.Json;

namespace MyStik.TimeTable.Web.Areas.Gym.Controllers
{
    public class QuestionsController : GymBaseController
    {
        private GymDbContext db = new GymDbContext();

        // GET: Gym/Questions
        public ActionResult Index()
        {
            return View(db.Questions.ToList());
        }

        // GET: Gym/Questions/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Question question = db.Questions.Find(id);
            if (question == null)
            {
                return HttpNotFound();
            }
            return View(question);
        }

        // GET: Gym/Questions/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Gym/Questions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Title,Problem,Description")] Question question)
        {
            if (ModelState.IsValid)
            {
                var user = GetCurrentUser();

                question.Id = Guid.NewGuid();

                var author = db.Authors.SingleOrDefault(x => x.UserId.Equals(user.Id));
                if (author == null)
                {
                    author = new Author
                    {
                        UserId = user.Id
                    };
                    db.Authors.Add(author);
                }

                question.Author = author;

                db.Questions.Add(question);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(question);
        }

        // GET: Gym/Questions/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Question question = db.Questions.Find(id);
            if (question == null)
            {
                return HttpNotFound();
            }
            return View(question);
        }

        // POST: Gym/Questions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Title,Problem,Description")] Question question)
        {
            if (ModelState.IsValid)
            {
                db.Entry(question).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(question);
        }

        // GET: Gym/Questions/Delete/5
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Question question = db.Questions.Find(id);
            if (question == null)
            {
                return HttpNotFound();
            }
            return View(question);
        }

        // POST: Gym/Questions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            Question question = db.Questions.Find(id);
            db.Questions.Remove(question);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public ActionResult Upload()
        {
            return View();
        }

        [HttpPost]
        public ActionResult UploadFile(HttpPostedFileBase file)
        {
            string tempFile = Path.GetTempFileName();

            // Speichern der Config-Dateien
            file.SaveAs(tempFile);

            QuestionIOModel q = null;

            using (StreamReader f = System.IO.File.OpenText(tempFile))
            {
                var serializer = new JsonSerializer();
                q = (QuestionIOModel)serializer.Deserialize(f, typeof(QuestionIOModel));
            }

            if (q != null)
            {
                var question = new Question
                {
                    CatalogId = q.id,
                    Title = q.title,
                    Problem = q.text,
                    Answers = new List<QuestionAnswer>()
                };
                db.Questions.Add(question);

                foreach (var a in q.answers)
                {
                    var answer = new QuestionAnswer
                    {
                        Solution = a.text,
                        IsCorrect = a.iscorrect,
                        Question = question
                    };
                    db.QuestionAnswers.Add(answer);
                }

                db.SaveChanges();
            }

            return null;
        }

    }
}
