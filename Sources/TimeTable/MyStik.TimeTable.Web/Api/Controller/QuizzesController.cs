using MyStik.Gym.Data;
using MyStik.TimeTable.Web.Api.DTOs;
using MyStik.TimeTable.Web.Api.Transformers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace MyStik.TimeTable.Web.Api.Controller
{
    [AllowAnonymous]
    [RoutePrefix("api/v2/quizzes")]
    public class QuizzesController : ApiController
    {
        private readonly GymDbContext _db = new GymDbContext();

        [Route("{quizId}/summary")]
        public QuizDto GetQuizSummary(Guid quizId)
        {
            var quiz = _db.Quizzes.SingleOrDefault(x => x.Id == quizId);

            var transformer = new QuizTransformer();

            if (quiz != null)
            {
                return transformer.GetStructure(quiz);
            }
            else
            {
                return transformer.GetEmptyQuiz();
            }
        }

        [Route("{quizId}/content")]
        public QuizDto GetQuizContent(Guid quizId)
        {
            var quiz = _db.Quizzes.SingleOrDefault(x => x.Id == quizId);

            var transformer = new QuizTransformer();

            if (quiz != null)
            {
                return transformer.GetStructure(quiz, true);
            }
            else
            {
                return transformer.GetEmptyQuiz();
            }
        }

        [Route("recommended/{userId}")]
        public IQueryable<QuizDto> GetRecommendedQuizzes(Guid userId)
        {
            var quizzes = _db.Quizzes.ToList();

            var transformer = new QuizTransformer();

            var result = new List<QuizDto>();
            
            foreach (var quiz in quizzes)
            {
                result.Add(transformer.GetStructure(quiz));
            }

            return result.AsQueryable();
        }
    }
}
