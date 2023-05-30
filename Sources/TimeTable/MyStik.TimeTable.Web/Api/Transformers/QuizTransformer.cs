using MyStik.Gym.Data;
using MyStik.TimeTable.Web.Api.DTOs;
using System.Collections.Generic;

namespace MyStik.TimeTable.Web.Api.Transformers
{
    public class QuizTransformer
    {
        internal QuizDto GetEmptyQuiz()
        {
            var dto = new QuizDto();
            return dto;
        }

        internal QuizDto GetStructure(Quiz quiz, bool withContent = false)
        {
            var dto = new QuizDto
            {
                id = quiz.Id,
                title = quiz.Title,
                description = quiz.Description,
                sections = new List<QuizSectionDto>(),
                NumQuestions = 0,
                NumGames = 0
            };

            foreach (var section in quiz.Sections)
            {
                var dto_section = new QuizSectionDto
                {
                    title = section.Title,
                    description = section.Description,
                    position = section.Position,
                    questions = new List<QuizQuestionDto>()
                };

                dto.sections.Add(dto_section);

                foreach (var question in section.Questions)
                {
                    var dto_question = new QuizQuestionDto
                    {
                        position = question.Position,
                        answers = new List<QuizQuestionAnswerDto>()
                    };

                    dto_section.questions.Add(dto_question);
                    dto.NumQuestions++;

                    if (withContent)
                    {
                        dto_question.title = question.Question.Problem;
                        dto_question.id = question.Question.Id;

                        foreach (var answer in question.Question.Answers)
                        {
                            var dto_answer = new QuizQuestionAnswerDto
                            {
                                id = answer.Id,
                                answerText = answer.Solution,
                                isCorrect = answer.IsCorrect
                            };

                            dto_question.answers.Add(dto_answer);
                        }
                    }
                }
            }

            return dto;
        }

        public QuizGameDto TransformToGame(Quiz quiz)
        {
            var dtoGame = new QuizGameDto
            {
                QuizId = quiz.Id,
                Title = quiz.Title,
                Description = quiz.Description,
                Sections = new List<QuizGameSectionDto>(),
                NumQuestions = 0
            };

            foreach (var quizSection in quiz.Sections)
            {
                var dtoSection = new QuizGameSectionDto
                {
                    Title = quizSection.Title,
                    Description = quizSection.Description,
                    Questions = new List<QuizGameQuestionDto>()
                };

                dtoGame.Sections.Add(dtoSection);

                foreach (var quizQuestion in quizSection.Questions)
                {
                    var dtoQuestion = new QuizGameQuestionDto
                    {
                        QuestionId = quizQuestion.Question.Id,
                        QuestionText = quizQuestion.Question.Problem,
                        DescriptionText = quizQuestion.Question.Description,
                        Answers = new List<QuizGameAnswerDto>()
                    };

                    dtoSection.Questions.Add(dtoQuestion);
                    dtoGame.NumQuestions++;

                    foreach (var questionAnswer in quizQuestion.Question.Answers)
                    {
                        var dtoAnswer = new QuizGameAnswerDto
                        {
                            AnswerId = questionAnswer.Id,
                            AnswerText = questionAnswer.Solution,
                            IsCorrect = questionAnswer.IsCorrect,
                            Explanation = questionAnswer.Explanaition
                        };

                        dtoQuestion.Answers.Add(dtoAnswer);
                    }
                }
            }


            return dtoGame;
        }

    }
}