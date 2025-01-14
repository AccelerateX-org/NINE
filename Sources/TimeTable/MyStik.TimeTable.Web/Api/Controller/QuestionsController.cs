using MyStik.Gym.Data;
using MyStik.TimeTable.Web.Api.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.Mvc;
using MyStik.TimeTable.Web.Api.Contracts;
using MyStik.TimeTable.Web.Models;
using MyStik.TimeTable.Web.Api.Transformers;
using MyStik.TimeTable.Web.Areas.Gym.Models;

namespace MyStik.TimeTable.Web.Api.Controller
{
    [System.Web.Http.AllowAnonymous]
    [System.Web.Http.RoutePrefix("api/v2/questions")]

    public class QuestionsController : ApiController
    {
        private readonly GymDbContext _gymDb = new GymDbContext();
        private readonly ApplicationDbContext _userDb = new ApplicationDbContext();

        [System.Web.Http.Route("list")]
        [System.Web.Http.HttpGet]
        public IQueryable<QuizQuestionDto> List()
        {
            var result = new List<QuizQuestionDto>();

            var questions = _gymDb.Questions.ToList();

            var transformer = new QuizTransformer();

            foreach (var question in questions)
            {
                var dto = transformer.TransformQuestion(question);

                result.Add(dto);
            }


            return result.AsQueryable();
        }

        [System.Web.Http.Route("create")]
        [System.Web.Http.HttpPost]
        public QuizQuestionDto Create([FromBody] GymCreateApiQuestionModel request)
        {

            var user = _userDb.Users.SingleOrDefault(x => x.Id.Equals(request.userId));
            if (user == null)
            {
                return null;
            }

            var author = _gymDb.Authors.SingleOrDefault(x => x.UserId.Equals(request.userId));
            if (author == null)
            {
                author = new Author
                {
                    UserId = request.userId
                };
                _gymDb.Authors.Add(author);
            }


            var q = new Question
            {
                Author = author,
                Problem = request.title,
                Answers = new List<QuestionAnswer>()
            };
            _gymDb.Questions.Add(q);

            foreach (var answer in request.answers)
            {
                var a = new QuestionAnswer
                {
                    Solution = answer.text,
                    IsCorrect = answer.isCorrect
                };
                q.Answers.Add(a);
                _gymDb.QuestionAnswers.Add(a);
            }

            _gymDb.SaveChanges();

            var transformer = new QuizTransformer();

            var result = transformer.TransformQuestion(q);

            return result;
        }

    }
}
