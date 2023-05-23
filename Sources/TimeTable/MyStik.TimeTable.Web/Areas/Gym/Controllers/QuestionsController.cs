using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
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
        public ActionResult Create(QuestionCreateModel model)
        {
            if (ModelState.IsValid)
            {
                var user = GetCurrentUser();

                var question = new Question();

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
                question.CatalogId = model.CatalogId; 
                question.Title = model.Title;
                question.Description = model.Description;
                question.Problem = model.Problem;

                if (!string.IsNullOrEmpty(model.AnswerA))
                {
                    var answerA = new QuestionAnswer
                    {
                        IsCorrect = model.IsAnswerACorrect,
                        Solution = model.AnswerA,
                        Question = question
                    };

                    db.QuestionAnswers.Add(answerA);
                }

                if (!string.IsNullOrEmpty(model.AnswerB))
                {
                    var answerB = new QuestionAnswer
                    {
                        IsCorrect = model.IsAnswerBCorrect,
                        Solution = model.AnswerB,
                        Question = question
                    };

                    db.QuestionAnswers.Add(answerB);
                }

                if (!string.IsNullOrEmpty(model.AnswerC))
                {
                    var answerC = new QuestionAnswer
                    {
                        IsCorrect = model.IsAnswerCCorrect,
                        Solution = model.AnswerC,
                        Question = question
                    };

                    db.QuestionAnswers.Add(answerC);
                }

                if (!string.IsNullOrEmpty(model.AnswerD))
                {
                    var answerD = new QuestionAnswer
                    {
                        IsCorrect = model.IsAnswerDCorrect,
                        Solution = model.AnswerD,
                        Question = question
                    };

                    db.QuestionAnswers.Add(answerD);
                }

                db.Questions.Add(question);
                db.SaveChanges();
                return RedirectToAction("Details", new {id = question.Id});
            }

            return View(model);
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

            // den Autor ergänzen
            if (question.Author == null)
            {
                var user = GetCurrentUser();
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
                db.SaveChanges();
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

        public ActionResult Import()
        {
            return View();
        }

        [HttpPost]
        public ActionResult UploadFile(HttpPostedFileBase file)
        {
            string tempFile = Path.GetTempFileName();

            // Speichern der Config-Dateien
            file.SaveAs(tempFile);

            QuestionCatalogIOModel cat = null;

            using (StreamReader f = System.IO.File.OpenText(tempFile))
            {
                var serializer = new JsonSerializer();
                cat = (QuestionCatalogIOModel)serializer.Deserialize(f, typeof(QuestionCatalogIOModel));
            }

            if (cat != null)
            {
                var questionSet = new QuestionSet
                {
                    Title = cat.name,
                    Mappings = new List<QuestionMapping>()
                };

                foreach (var q in cat.questions)
                {
                    var question = new Question
                    {
                        CatalogId =  $"{cat.tag}#{q.tag}",
                        Title = q.tag,
                        Problem = q.problem,
                        Answers = new List<QuestionAnswer>()
                    };
                    db.Questions.Add(question);

                    foreach (var a in q.answers)
                    {
                        var answer = new QuestionAnswer
                        {
                            Solution = a.solution,
                            IsCorrect = a.iscorrect,
                            Question = question
                        };
                        db.QuestionAnswers.Add(answer);
                    }

                    var qMapping = new QuestionMapping
                    {
                        Question = question,
                        QuestionSet = questionSet
                    };

                    db.QuestionMappings.Add(qMapping);
                }

                db.QuestionSets.Add(questionSet);


                db.SaveChanges();
            }

            return null;
        }

        public ActionResult EditAnswer(Guid id)
        {

            return View();
        }
    }
}
