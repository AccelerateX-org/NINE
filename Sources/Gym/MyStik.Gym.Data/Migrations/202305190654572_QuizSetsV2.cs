namespace MyStik.Gym.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class QuizSetsV2 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.QuestionSetResponsibilities",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Author_Id = c.Guid(),
                        QuestionSet_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Authors", t => t.Author_Id)
                .ForeignKey("dbo.QuestionSets", t => t.QuestionSet_Id)
                .Index(t => t.Author_Id)
                .Index(t => t.QuestionSet_Id);
            
            AddColumn("dbo.GameAnswers", "Answered", c => c.DateTime(nullable: false));
            AddColumn("dbo.QuestionSets", "Tag", c => c.String());
            AddColumn("dbo.GameQuestions", "Opened", c => c.DateTime(nullable: false));
            AddColumn("dbo.GameQuestions", "Closed", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.QuestionSetResponsibilities", "QuestionSet_Id", "dbo.QuestionSets");
            DropForeignKey("dbo.QuestionSetResponsibilities", "Author_Id", "dbo.Authors");
            DropIndex("dbo.QuestionSetResponsibilities", new[] { "QuestionSet_Id" });
            DropIndex("dbo.QuestionSetResponsibilities", new[] { "Author_Id" });
            DropColumn("dbo.GameQuestions", "Closed");
            DropColumn("dbo.GameQuestions", "Opened");
            DropColumn("dbo.QuestionSets", "Tag");
            DropColumn("dbo.GameAnswers", "Answered");
            DropTable("dbo.QuestionSetResponsibilities");
        }
    }
}
