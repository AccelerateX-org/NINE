namespace MyStik.Gym.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialSetup : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Authors",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        UserId = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.GameAnswers",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Answer_Id = c.Guid(),
                        Player_Id = c.Guid(),
                        Question_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.QuestionAnswers", t => t.Answer_Id)
                .ForeignKey("dbo.Players", t => t.Player_Id)
                .ForeignKey("dbo.GameQuestions", t => t.Question_Id)
                .Index(t => t.Answer_Id)
                .Index(t => t.Player_Id)
                .Index(t => t.Question_Id);
            
            CreateTable(
                "dbo.QuestionAnswers",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        IsCorrect = c.Boolean(nullable: false),
                        Solution = c.String(),
                        Explanaition = c.String(),
                        Image_Id = c.Guid(),
                        Question_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.BinaryStorages", t => t.Image_Id)
                .ForeignKey("dbo.Questions", t => t.Question_Id)
                .Index(t => t.Image_Id)
                .Index(t => t.Question_Id);
            
            CreateTable(
                "dbo.BinaryStorages",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Name = c.String(),
                        Description = c.String(),
                        Category = c.String(),
                        FileType = c.String(),
                        BinaryData = c.Binary(),
                        AccessCount = c.Int(nullable: false),
                        Created = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Questions",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        CatalogId = c.String(),
                        Title = c.String(),
                        Problem = c.String(),
                        Description = c.String(),
                        Author_Id = c.Guid(),
                        Image_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Authors", t => t.Author_Id)
                .ForeignKey("dbo.BinaryStorages", t => t.Image_Id)
                .Index(t => t.Author_Id)
                .Index(t => t.Image_Id);
            
            CreateTable(
                "dbo.QuestionLabels",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Title = c.String(),
                        Label_Id = c.Guid(),
                        Question_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Labels", t => t.Label_Id)
                .ForeignKey("dbo.Questions", t => t.Question_Id)
                .Index(t => t.Label_Id)
                .Index(t => t.Question_Id);
            
            CreateTable(
                "dbo.Labels",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Title = c.String(),
                        Description = c.String(),
                        BackgroundColor = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.QuestionMappings",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Question_Id = c.Guid(),
                        QuestionSet_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Questions", t => t.Question_Id)
                .ForeignKey("dbo.QuestionSets", t => t.QuestionSet_Id)
                .Index(t => t.Question_Id)
                .Index(t => t.QuestionSet_Id);
            
            CreateTable(
                "dbo.QuestionSets",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Title = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.QuizQuestions",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Position = c.Int(nullable: false),
                        Question_Id = c.Guid(),
                        QuizSection_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Questions", t => t.Question_Id)
                .ForeignKey("dbo.QuizSections", t => t.QuizSection_Id)
                .Index(t => t.Question_Id)
                .Index(t => t.QuizSection_Id);
            
            CreateTable(
                "dbo.QuizSections",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Title = c.String(),
                        Description = c.String(),
                        Position = c.Int(nullable: false),
                        Quiz_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Quizs", t => t.Quiz_Id)
                .Index(t => t.Quiz_Id);
            
            CreateTable(
                "dbo.Quizs",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Title = c.String(),
                        Description = c.String(),
                        Author_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Authors", t => t.Author_Id)
                .Index(t => t.Author_Id);
            
            CreateTable(
                "dbo.QuizPublishings",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Description = c.String(),
                        CourseId = c.Guid(nullable: false),
                        Quiz_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Quizs", t => t.Quiz_Id)
                .Index(t => t.Quiz_Id);
            
            CreateTable(
                "dbo.Players",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        UserId = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.GameQuestions",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Game_Id = c.Guid(),
                        Question_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.QuizGames", t => t.Game_Id)
                .ForeignKey("dbo.Questions", t => t.Question_Id)
                .Index(t => t.Game_Id)
                .Index(t => t.Question_Id);
            
            CreateTable(
                "dbo.QuizGames",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Quiz_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Quizs", t => t.Quiz_Id)
                .Index(t => t.Quiz_Id);
            
            CreateTable(
                "dbo.GameSubscriptions",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Game_Id = c.Guid(),
                        Player_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.QuizGames", t => t.Game_Id)
                .ForeignKey("dbo.Players", t => t.Player_Id)
                .Index(t => t.Game_Id)
                .Index(t => t.Player_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.GameQuestions", "Question_Id", "dbo.Questions");
            DropForeignKey("dbo.GameSubscriptions", "Player_Id", "dbo.Players");
            DropForeignKey("dbo.GameSubscriptions", "Game_Id", "dbo.QuizGames");
            DropForeignKey("dbo.QuizGames", "Quiz_Id", "dbo.Quizs");
            DropForeignKey("dbo.GameQuestions", "Game_Id", "dbo.QuizGames");
            DropForeignKey("dbo.GameAnswers", "Question_Id", "dbo.GameQuestions");
            DropForeignKey("dbo.GameAnswers", "Player_Id", "dbo.Players");
            DropForeignKey("dbo.GameAnswers", "Answer_Id", "dbo.QuestionAnswers");
            DropForeignKey("dbo.QuizSections", "Quiz_Id", "dbo.Quizs");
            DropForeignKey("dbo.QuizPublishings", "Quiz_Id", "dbo.Quizs");
            DropForeignKey("dbo.Quizs", "Author_Id", "dbo.Authors");
            DropForeignKey("dbo.QuizQuestions", "QuizSection_Id", "dbo.QuizSections");
            DropForeignKey("dbo.QuizQuestions", "Question_Id", "dbo.Questions");
            DropForeignKey("dbo.QuestionMappings", "QuestionSet_Id", "dbo.QuestionSets");
            DropForeignKey("dbo.QuestionMappings", "Question_Id", "dbo.Questions");
            DropForeignKey("dbo.QuestionLabels", "Question_Id", "dbo.Questions");
            DropForeignKey("dbo.QuestionLabels", "Label_Id", "dbo.Labels");
            DropForeignKey("dbo.Questions", "Image_Id", "dbo.BinaryStorages");
            DropForeignKey("dbo.Questions", "Author_Id", "dbo.Authors");
            DropForeignKey("dbo.QuestionAnswers", "Question_Id", "dbo.Questions");
            DropForeignKey("dbo.QuestionAnswers", "Image_Id", "dbo.BinaryStorages");
            DropIndex("dbo.GameSubscriptions", new[] { "Player_Id" });
            DropIndex("dbo.GameSubscriptions", new[] { "Game_Id" });
            DropIndex("dbo.QuizGames", new[] { "Quiz_Id" });
            DropIndex("dbo.GameQuestions", new[] { "Question_Id" });
            DropIndex("dbo.GameQuestions", new[] { "Game_Id" });
            DropIndex("dbo.QuizPublishings", new[] { "Quiz_Id" });
            DropIndex("dbo.Quizs", new[] { "Author_Id" });
            DropIndex("dbo.QuizSections", new[] { "Quiz_Id" });
            DropIndex("dbo.QuizQuestions", new[] { "QuizSection_Id" });
            DropIndex("dbo.QuizQuestions", new[] { "Question_Id" });
            DropIndex("dbo.QuestionMappings", new[] { "QuestionSet_Id" });
            DropIndex("dbo.QuestionMappings", new[] { "Question_Id" });
            DropIndex("dbo.QuestionLabels", new[] { "Question_Id" });
            DropIndex("dbo.QuestionLabels", new[] { "Label_Id" });
            DropIndex("dbo.Questions", new[] { "Image_Id" });
            DropIndex("dbo.Questions", new[] { "Author_Id" });
            DropIndex("dbo.QuestionAnswers", new[] { "Question_Id" });
            DropIndex("dbo.QuestionAnswers", new[] { "Image_Id" });
            DropIndex("dbo.GameAnswers", new[] { "Question_Id" });
            DropIndex("dbo.GameAnswers", new[] { "Player_Id" });
            DropIndex("dbo.GameAnswers", new[] { "Answer_Id" });
            DropTable("dbo.GameSubscriptions");
            DropTable("dbo.QuizGames");
            DropTable("dbo.GameQuestions");
            DropTable("dbo.Players");
            DropTable("dbo.QuizPublishings");
            DropTable("dbo.Quizs");
            DropTable("dbo.QuizSections");
            DropTable("dbo.QuizQuestions");
            DropTable("dbo.QuestionSets");
            DropTable("dbo.QuestionMappings");
            DropTable("dbo.Labels");
            DropTable("dbo.QuestionLabels");
            DropTable("dbo.Questions");
            DropTable("dbo.BinaryStorages");
            DropTable("dbo.QuestionAnswers");
            DropTable("dbo.GameAnswers");
            DropTable("dbo.Authors");
        }
    }
}
