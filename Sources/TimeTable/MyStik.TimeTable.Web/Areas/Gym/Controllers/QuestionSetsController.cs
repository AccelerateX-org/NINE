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

namespace MyStik.TimeTable.Web.Areas.Gym.Controllers
{
    public class QuestionSetsController : GymBaseController
    {
        private GymDbContext db = new GymDbContext();

        // GET: Gym/QuestionSets
        public ActionResult Index()
        {
            return View(db.QuestionSets.ToList());
        }

        // GET: Gym/QuestionSets/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            QuestionSet questionSet = db.QuestionSets.Find(id);
            if (questionSet == null)
            {
                return HttpNotFound();
            }
            return View(questionSet);
        }

        // GET: Gym/QuestionSets/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Gym/QuestionSets/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Tag,Title")] QuestionSet questionSet)
        {
            if (ModelState.IsValid)
            {
                questionSet.Id = Guid.NewGuid();
                db.QuestionSets.Add(questionSet);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(questionSet);
        }

        // GET: Gym/QuestionSets/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            QuestionSet questionSet = db.QuestionSets.Find(id);
            if (questionSet == null)
            {
                return HttpNotFound();
            }
            return View(questionSet);
        }

        // POST: Gym/QuestionSets/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Tag,Title")] QuestionSet questionSet)
        {
            if (ModelState.IsValid)
            {
                db.Entry(questionSet).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(questionSet);
        }

        // GET: Gym/QuestionSets/Delete/5
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            QuestionSet questionSet = db.QuestionSets.Find(id);
            if (questionSet == null)
            {
                return HttpNotFound();
            }
            return View(questionSet);
        }

        // POST: Gym/QuestionSets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            QuestionSet questionSet = db.QuestionSets.Find(id);
            db.QuestionSets.Remove(questionSet);
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

                var questionSet = db.QuestionSets.FirstOrDefault(x => x.Tag.ToUpper().Equals(cat.tag.ToUpper()));

                if (questionSet == null)
                {
                    questionSet = new QuestionSet
                    {
                        Tag = cat.tag,
                        Title = cat.name,
                        // Description wird bisher nicht übernommen
                        Mappings = new List<QuestionMapping>(),
                        SetResponsibilities = new List<QuestionSetResponsibility>()
                    };

                    var resp = new QuestionSetResponsibility
                    {
                        Author = author,
                        QuestionSet = questionSet
                    };

                    db.QuestionSetResponsibilities.Add(resp);
                    db.QuestionSets.Add(questionSet);
                }
                else
                {
                    // Autor muss zu den Verantwortlichen gehören
                    var isResponsible = questionSet.SetResponsibilities.Any(x => x.Author.Id == author.Id);
                    if (!isResponsible) { return null; }
                }

                foreach (var q in cat.questions)
                {
                    var question = new Question
                    {
                        CatalogId = $"{cat.tag}#{q.tag}",
                        Title = q.tag,
                        Problem = q.problem,
                        Answers = new List<QuestionAnswer>(),
                        Author = author
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


                db.SaveChanges();
            }

            return null;
        }

    }
}
