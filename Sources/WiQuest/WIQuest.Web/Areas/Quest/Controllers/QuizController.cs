using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WIQuest.Web.Areas.Quest.Models;
using WIQuest.Web.Areas.Quest.Services;
using WIQuest.Web.Data;

namespace WIQuest.Web.Areas.Quest.Controllers
{
    public class QuizController : Controller
    {
        // GET: Quest/Quiz
        public ActionResult Index(Guid id)
        {
            var service = new QuestionService();
            var user = service.GetUser(id);

            var model = new QuizQuestionViewModel();

            var firstQuestion = service.GetFirstQuestion();
            if (firstQuestion != null)
            {
                var category = firstQuestion.Category;

                model.User = user;
                model.Question = firstQuestion;
                model.NumQuestions = category.Questions.Count;
                model.IsFirstQuestion = true;
                model.Answer = service.GetAnswer(firstQuestion, user);
            }

            return View(model);
        }


        public PartialViewResult NextQuestion(Guid questionId, Guid userId, Guid? answerId)
        {
            var service = new QuestionService();

            var question = service.GetQuestion(questionId);
            var user  = service.GetUser(userId);

            if (question == null || user == null)
                return PartialView("Error");


            // den Logeintrag erstellen
            var answer = answerId != null ?  service.GetAnswer(answerId.Value) : null;

            // Ergebnis wird nicht weiter verarbeitet
            service.WriteLog(question, answer, user);

            // Nächste Frage in jedem Fall anzeigen

            var model = new QuizQuestionViewModel();

            var nextQuestion = service.GetNextQuestion(question);
            if (nextQuestion != null)
            {
                var category = nextQuestion.Category;

                model.User = user;
                model.Question = nextQuestion;
                model.NumQuestions = category.Questions.Count;
                model.Answer = service.GetAnswer(nextQuestion, user);

                return PartialView("_Question", model);
            }

            // es gibt keine Frage mehr
            return PartialView("_EndOfQuiz", user);
        }

        public PartialViewResult PreviousQuestion(Guid questionId, Guid userId, Guid? answerId)
        {
            var service = new QuestionService();

            var question = service.GetQuestion(questionId);
            var user = service.GetUser(userId);

            if (question == null || user == null)
                return PartialView("Error");

            // Antwort Ermitteln
            var answer = answerId != null ? service.GetAnswer(answerId.Value) : null;
            // Service übernimmt den Update
            service.WriteLog(question, answer, user);


            // Frage davor ermitteln
            var model = new QuizQuestionViewModel();

            var prevQuestion = service.GetPreviousQuestion(question);
            if (prevQuestion != null)
            {
                var category = prevQuestion.Category;

                model.User = user;
                model.Question = prevQuestion;
                model.NumQuestions = category.Questions.Count;
                model.Answer = service.GetAnswer(prevQuestion, user);

                // ist das die erste Frage?
                if (prevQuestion.Category.Reihenfolge == 1 && prevQuestion.Reihenfolge == 1)
                {
                    model.IsFirstQuestion = true;
                }

                return PartialView("_Question", model);
            }

            return PartialView("Error");
        }


    }
}