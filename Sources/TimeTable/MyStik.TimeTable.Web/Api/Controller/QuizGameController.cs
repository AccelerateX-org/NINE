using Microsoft.Ajax.Utilities;
using MyStik.Gym.Data;
using MyStik.TimeTable.Web.Api.DTOs;
using MyStik.TimeTable.Web.Api.Transformers;
using Org.BouncyCastle.Asn1.Ocsp;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace MyStik.TimeTable.Web.Api.Controller
{
    public class QuizGameRequestModel
    {
        public Guid quizid { get; set; }

        public Guid playerid { get; set; }

        public Guid gameId { get; set; }

        public Guid answerId { get; set; }
    }

    public class QuizPlayerModel
    {
        public Guid playerid { get; set; }
    }


    [AllowAnonymous]
    [RoutePrefix("api/v2/quizgames")]
    public class QuizGameController : ApiController
    {
        private readonly GymDbContext _db = new GymDbContext();

        [Route("start")]
        [HttpPost]
        public async Task<QuizGameDto> StartQuizGame(
            [FromBody] QuizGameRequestModel request)
        {
            var quiz = await _db.Quizzes.SingleOrDefaultAsync(x => x.Id == request.quizid);
            // echt suboptimal - das muss später nochmal umgebaut werden
            var player = await _db.Players.SingleOrDefaultAsync(x => x.UserId.Equals(request.playerid.ToString()));

            if (player == null)
            {
                player = new Player()
                {
                    UserId = request.playerid.ToString(),
                };

                _db.Players.Add(player);
                await _db.SaveChangesAsync();
            }


            if (player == null || quiz == null)
                return null;

            var service = new QuizTransformer();
            var game = service.TransformToGame(quiz);

            // Das ist jetzt ein Single Player Game
            // es wird ein neues Spiel gestartet
            var quizGame = new QuizGame
            {
                Quiz = quiz,
                Subscriptions = new List<GameSubscription>()
            };

            var gameSubscription = new GameSubscription
            {
                Game = quizGame,
                Player = player
            };

            quizGame.Subscriptions.Add(gameSubscription);

            _db.QuizGames.Add(quizGame);
            _db.GameSubscriptions.Add(gameSubscription);

            await _db.SaveChangesAsync();

            game.PlayerId = player.Id;
            game.GameId = quizGame.Id;
            game.NumGames = _db.QuizGames.Count(x =>
                x.Quiz.Id == request.quizid &&
                x.Subscriptions.Any(s => s.Player.UserId.Equals(request.playerid.ToString())));

            return game;
        }

        [Route("answer")]
        [HttpPost]
        public async Task<QuizGameActionDto> AnswerQuizGame(
            [FromBody] QuizGameRequestModel request)
        {
            var game = await _db.QuizGames.SingleOrDefaultAsync(x => x.Id == request.gameId);
            var player = await _db.Players.SingleOrDefaultAsync(x => x.UserId.Equals(request.playerid.ToString()));
            var answer = await _db.QuestionAnswers.SingleOrDefaultAsync(x => x.Id == request.answerId);

            if (player == null || game == null || answer == null)
                return null;

            // Single Player Game
            // jeder Spieler hat sein eigenes Spiel und gibt daher nur für sich Antworten
            // spielt der Spieler überhaupt mit?
            var subscription = await _db.GameSubscriptions.FirstOrDefaultAsync(x =>
                x.Game.Id == game.Id &&
                x.Player.Id == player.Id);
            if (subscription == null)
                return null;

            // hat der Spieler in diesem Spiel die Frage schon beantwortet?
            var quizAnswer = await _db.GameAnswers.FirstOrDefaultAsync(x =>
                x.Question.Game.Id == game.Id &&
                x.Player.Id == player.Id &&
                x.Answer.Id == answer.Id);

            // Hat bereits geantwortet - keine weitere Antwort zulassen
            if (quizAnswer != null)
                return null;

            var gameQuestion = game.Questions.FirstOrDefault(x => x.Question.Id == answer.Question.Id);

            if (gameQuestion == null)
            {
                gameQuestion = new GameQuestion
                {
                    Question = answer.Question,
                    Game = game,
                    Opened = DateTime.Now,
                    Answers = new List<GameAnswer>()
                };

                _db.GameQuestions.Add(gameQuestion);
            }

            var gameAnswer = new GameAnswer
            {
                Question = gameQuestion,
                Answer = answer,
                Player = player,
                Answered = DateTime.Now
            };

            _db.GameAnswers.Add(gameAnswer);

            await _db.SaveChangesAsync();

            var model = new QuizGameActionDto();

            return model;
        }

    }
}