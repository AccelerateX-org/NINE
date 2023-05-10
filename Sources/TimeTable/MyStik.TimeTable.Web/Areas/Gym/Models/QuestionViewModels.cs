using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MyStik.TimeTable.Web.Areas.Gym.Models
{
    public class QuestionCreateModel
    {
        [Required]
        public string CatalogId { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Problem { get; set; }

        [Required]
        public string AnswerA { get; set; }

        public bool IsAnswerACorrect { get; set; }

        [Required]
        public string AnswerB { get; set; }

        public bool IsAnswerBCorrect { get; set; }

        public string AnswerC { get; set; }

        public bool IsAnswerCCorrect { get; set; }

        public string AnswerD { get; set; }

        public bool IsAnswerDCorrect { get; set; }

    }
}