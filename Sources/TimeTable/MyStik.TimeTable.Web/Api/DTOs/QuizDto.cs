﻿using System;
using System.Collections.Generic;

namespace MyStik.TimeTable.Web.Api.DTOs
{
    public class QuizDto
    {
        public Guid id { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public List<QuizSectionDto> sections { get; set; }

        public int NumQuestions { get; set; }

        public int NumGames { get; set; }
    }

    public class QuizSectionDto
    {
        public string title { get; set; }
        public string description { get; set; }
        public int position { get; set; }
        public List<QuizQuestionDto> questions { get; set; }
    }

    public class QuizQuestionDto
    {
        public Guid id { get; set; }

        public int position { get; set; }

        public string title { get; set; }

        public string description { get; set; }

        public List<QuizQuestionAnswerDto> answers { get; set; }
    }

    public class QuizQuestionAnswerDto
    {
        public Guid id { get; set; }

        public string answerText { get; set; }

        public bool isCorrect { get; set; }
    }
}