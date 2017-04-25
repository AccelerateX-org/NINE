using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WIQuest.Web.Data;

namespace WIQuest.Web.Areas.Quest.Services
{
    public class QuestionService
    {
        private QuestDbContext db = new QuestDbContext();

        public Question GetFirstQuestion()
        {
            if (!db.Categories.Any())
                return null;

            var minCategory = db.Categories.Min(c => c.Reihenfolge);

            var question =
                db.Questions.Where(q => q.Category.Reihenfolge == minCategory)
                    .OrderBy(q => q.Reihenfolge)
                    .FirstOrDefault();

            return question;
        }

        public Question GetNextQuestion(Question currentQuestion)
        {
            // im test nur die ersten 5 Fragen anzeigen
            /*
            if (currentQuestion.Category.Reihenfolge == 1 && currentQuestion.Reihenfolge == 5)
                return null;
             */


            // die nächste Frage innerhalb der aktuelle Kategorie
            var category = currentQuestion.Category;

            // Erste Frage, deren Reihenfolge größer ist, als der aktuelle Wert
            var question =
                db.Questions.Where(q => q.Category.Reihenfolge == category.Reihenfolge &&
                    q.Reihenfolge > currentQuestion.Reihenfolge)
                    .OrderBy(q => q.Reihenfolge)
                    .FirstOrDefault();

            if (question != null)
                return question;

            // nächste Kategorie
            var cat = db.Categories.Where(c => c.Reihenfolge > category.Reihenfolge).OrderBy(c => c.Reihenfolge).FirstOrDefault();
            if (cat == null)
                return null;

            // Erste Frage in der nächsten Kategorie
            question =
                db.Questions.Where(q => q.Category.Reihenfolge == cat.Reihenfolge)
                    .OrderBy(q => q.Reihenfolge)
                    .FirstOrDefault();

            return question;
        }

        public Question GetPreviousQuestion(Question currentQuestion)
        {
            // die vorhergehende Frage innerhalb der aktuelle Kategorie
            var category = currentQuestion.Category;

            var question =
                db.Questions.Where(q => q.Category.Reihenfolge == category.Reihenfolge &&
                    q.Reihenfolge < currentQuestion.Reihenfolge)
                    .OrderByDescending(q => q.Reihenfolge)
                    .FirstOrDefault();

            if (question != null)
                return question;

            // Die Vorgängerkategorie
            var cat = db.Categories.Where(c => c.Reihenfolge < category.Reihenfolge).OrderByDescending(c => c.Reihenfolge).FirstOrDefault();
            if (cat == null)
                return null;

            question =
                db.Questions.Where(q => q.Category.Reihenfolge == cat.Reihenfolge)
                    .OrderByDescending(q => q.Reihenfolge)
                    .FirstOrDefault();

            return question;
        }


        public int GetPercentage(Question question)
        {
            var category = question.Category;

            var numQ = db.Questions.Count(q => q.Category.Reihenfolge == category.Reihenfolge);

            return (question.Reihenfolge*100)/numQ;
        }

        internal Question GetQuestion(Guid questionId)
        {
            return db.Questions.SingleOrDefault(q => q.Id == questionId);
        }

        internal QuestionAnswer GetAnswer(Guid answerId)
        {
            return db.Answers.SingleOrDefault(a => a.Id == answerId);
        }

        internal User GetUser(Guid userId)
        {
            var user = db.Users.SingleOrDefault(u => u.Id == userId);
            return user;
        }

        internal QuestionAnswer GetAnswer(Question question, User user)
        {
            // Existiert für diese Frage bereits ein Logeintrag?
            var log = db.QuestLogs.SingleOrDefault(l => l.User.Id == user.Id &&
                                                        l.Question.Id == question.Id);

            if (log != null)
                return log.Answer;

            return null;
        }


        internal QuestLog WriteLog(Question question, QuestionAnswer answer, User user)
        {
            // Existiert für diese Frage bereits ein Logeintrag?
            var log = db.QuestLogs.SingleOrDefault(l => l.User.Id == user.Id &&
                                                        l.Question.Id == question.Id);

            if (log != null)
            {
                // den Log aktualisieren
                log.Answer = answer;
                log.ViewCount++;
            }
            else
            {
                // einen neuen Logeintrag erstellen
                log = new QuestLog
                {
                    Question = question,
                    Answer = answer,
                    User = user,
                    FirstView = DateTime.Now,
                    ViewCount = 1
                };
                db.QuestLogs.Add(log);
            }

            // Datenbank aktualisieren
            db.SaveChanges();

            return log;
        }
    }
}