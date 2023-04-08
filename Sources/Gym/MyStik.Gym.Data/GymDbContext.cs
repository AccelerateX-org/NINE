using System.Data.Entity;

namespace MyStik.Gym.Data
{
    public class GymDbContext : DbContext
    {
        public DbSet<BinaryStorage> Storages { get; set; }

        public DbSet<Label> Labels { get; set; }

        public DbSet<Author> Authors { get; set; }
        public DbSet<Player> Players { get; set; }


        public DbSet<Question> Questions { get; set; }
        public DbSet<QuestionAnswer> QuestionAnswers { get; set; }

        public DbSet<QuestionLabel> LabelSets { get; set; }

        public DbSet<QuestionSet> QuestionSets { get; set; }

        public DbSet<QuestionMapping> QuestionMappings { get; set; }



        public DbSet<Quiz> Quizzes { get; set; }
        public DbSet<QuizSection> QuizSections { get; set; }
        public DbSet<QuizQuestion> QuizQuestions { get; set; }

        public DbSet<QuizPublishing> QuizPublishings { get; set; }


        public DbSet<QuizGame> QuizGames { get; set; }
        public DbSet<GameSubscription> GameSubscriptions { get; set; }
        public DbSet<GameQuestion> GameQuestions { get; set; }
        public DbSet<GameAnswer> GameAnswers { get; set; }

    }
}
