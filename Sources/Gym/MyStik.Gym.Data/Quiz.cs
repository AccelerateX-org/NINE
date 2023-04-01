using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MyStik.Gym.Data
{
    public class Quiz
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public virtual Author Author { get; set; }

        public virtual ICollection<QuizSection> Sections { get; set; }

        public virtual ICollection<QuizPublishing> Publishings { get; set; }
    }

    public class QuizSection
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public virtual Quiz Quiz { get; set; }

        public int Position { get; set; }

        public virtual ICollection<QuizQuestion> Questions { get; set; }
    }

    public class QuizQuestion
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public int Position { get; set; }


        public virtual QuizSection QuizSection { get; set; }

        public virtual Question Question { get; set; }

    }


    public class QuizPublishing
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public virtual Quiz Quiz { get; set; }

        public string Description { get; set; }

        /// <summary>
        /// Die Id der LV aus NINE
        /// </summary>
        public Guid CourseId { get; set; }
    }
}
