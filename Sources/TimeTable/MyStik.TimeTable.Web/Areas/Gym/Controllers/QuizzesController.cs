using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MyStik.Gym.Data;
using MyStik.TimeTable.Web.Areas.Gym.Models;
using Newtonsoft.Json;
using static System.Collections.Specialized.BitVector32;

namespace MyStik.TimeTable.Web.Areas.Gym.Controllers
{
    public class QuizzesController : Controller
    {
        private GymDbContext db = new GymDbContext();

        // GET: Gym/Quizs
        public ActionResult Index()
        {
            return View(db.Quizzes.ToList());
        }

        // GET: Gym/Quizs/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Quiz quiz = db.Quizzes.Find(id);
            if (quiz == null)
            {
                return HttpNotFound();
            }
            return View(quiz);
        }

        // GET: Gym/Quizs/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Gym/Quizs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Title,Description")] Quiz quiz)
        {
            if (ModelState.IsValid)
            {
                quiz.Id = Guid.NewGuid();
                db.Quizzes.Add(quiz);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(quiz);
        }

        // GET: Gym/Quizs/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Quiz quiz = db.Quizzes.Find(id);
            if (quiz == null)
            {
                return HttpNotFound();
            }
            return View(quiz);
        }

        // POST: Gym/Quizs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Title,Description")] Quiz quiz)
        {
            if (ModelState.IsValid)
            {
                db.Entry(quiz).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(quiz);
        }

        // GET: Gym/Quizs/Delete/5
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Quiz quiz = db.Quizzes.Find(id);
            if (quiz == null)
            {
                return HttpNotFound();
            }
            return View(quiz);
        }

        // POST: Gym/Quizs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            Quiz quiz = db.Quizzes.Find(id);
            db.Quizzes.Remove(quiz);
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

            QuizIOModel q = null;

            using (StreamReader f = System.IO.File.OpenText(tempFile))
            {
                var serializer = new JsonSerializer();
                q = (QuizIOModel)serializer.Deserialize(f, typeof(QuizIOModel));
            }

            if (q != null)
            {
                var quiz = new Quiz
                {
                    Title = q.title,
                    Description = q.description,
                    Sections = new List<QuizSection>()
                };
                db.Quizzes.Add(quiz);

                foreach (var s in q.sections)
                {
                    var section = new QuizSection
                    {
                        Title = s.title,
                        Position = s.position,
                        Quiz = quiz,
                        Questions = new List<QuizQuestion>()
                    };
                    db.QuizSections.Add(section);

                    var i = 0;
                    foreach (var t in s.questions)
                    {
                        var question = db.Questions.FirstOrDefault(x => x.CatalogId.Equals(t.id));

                        if (question != null)
                        {
                            var quizQuestion = new QuizQuestion
                            {
                                Question = question,
                                QuizSection = section,
                                Position = ++i
                            };
                            db.QuizQuestions.Add(quizQuestion);
                        }
                    }
                }

                db.SaveChanges();
            }

            return null;
        }

    }
}
