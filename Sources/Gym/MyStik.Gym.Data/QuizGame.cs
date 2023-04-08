using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyStik.Gym.Data
{
    public class QuizGame
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public virtual ICollection<GameSubscription> Subscriptions { get; set; }

        public virtual Quiz Quiz { get; set; }

        public virtual ICollection<GameQuestion> Questions { get; set; }
    }

    public class GameSubscription
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public virtual QuizGame Game { get; set; }


        public virtual Player Player { get; set; }
    }

    public class GameQuestion
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public virtual QuizGame Game { get; set; }

        public virtual Question Question { get; set; }

        public virtual ICollection<GameAnswer> Answers { get; set; }

   }

    public class GameAnswer
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public virtual GameQuestion Question { get; set; }


        public virtual QuestionAnswer Answer { get; set; }

        public virtual Player Player { get; set; }

    }

}
