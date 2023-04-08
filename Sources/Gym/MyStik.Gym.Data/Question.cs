using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyStik.Gym.Data
{
    public class Question
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        /// <summary>
        /// Ein fachlicher Schlüssel der Frage nach freier Vergabe durch den Autor
        /// </summary>
        public string CatalogId { get; set; }

        /// <summary>
        /// Der Name - nur zu Verwaltungszwecken
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Die Fragestellung (wird angezeigt)
        /// </summary>
        public string Problem { get; set; }

        /// <summary>
        /// Der Beschreibungstext (optional)
        /// </summary>
        public string Description { get; set; }

        public virtual Author Author { get; set; }

        public virtual BinaryStorage Image { get; set; } 

        public virtual ICollection<QuestionLabel> Labels { get; set; }

        public virtual ICollection<QuestionAnswer> Answers { get; set; }

        public virtual ICollection<QuestionMapping> Mappings { get; set; }

        public virtual ICollection<QuizQuestion> Quizzes { get; set; }
    }

    public class QuestionAnswer
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public bool IsCorrect { get; set; }

        /// <summary>
        /// Der Name - nur zu Verwaltungszwecken
        /// </summary>
        public string Solution { get; set; }

        /// <summary>
        /// Die Fragestellung (wird angezeigt)
        /// </summary>
        public string Explanaition { get; set; }

        public virtual BinaryStorage Image { get; set; }

        public virtual Question Question { get; set; }
    }

}
