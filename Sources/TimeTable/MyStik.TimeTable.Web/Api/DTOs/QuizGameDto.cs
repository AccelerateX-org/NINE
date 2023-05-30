using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyStik.TimeTable.Web.Api.DTOs
{
    public class QuizGameDto
    {
        public Guid QuizId { get; set; }

        public Guid GameId { get; set; }

        public Guid PlayerId { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public int NumQuestions { get; set; }

        public int NumGames { get; set; }


        public ICollection<QuizGameSectionDto> Sections { get; set; }



    }

    public class QuizGameSectionDto
    {
        public string Title { get; set; }

        public string Description { get; set; }

        public List<QuizGameQuestionDto> Questions { get; set; }
    }

    public class QuizGameQuestionDto
    {
        public Guid QuestionId { get; set; }

        public string QuestionText { get; set; }

        public string DescriptionText { get; set; }

        public List<QuizGameAnswerDto> Answers { get; set; }
    }

    public class QuizGameAnswerDto
    {
        public Guid AnswerId { get; set; }

        public string AnswerText { get; set; }

        public string Explanation { get; set; }

        public string Helptext { get; set; }

        public bool IsCorrect { get; set; }

        public bool IsSelected { get; set; }

    }

    public class QuizGameActionDto
    {

    }
}