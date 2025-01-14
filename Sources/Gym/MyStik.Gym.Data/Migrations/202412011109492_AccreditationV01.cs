namespace MyStik.Gym.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AccreditationV01 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.QuestionAccreditations",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        LearningModule_Id = c.Guid(),
                        Question_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.LearningModules", t => t.LearningModule_Id)
                .ForeignKey("dbo.Questions", t => t.Question_Id)
                .Index(t => t.LearningModule_Id)
                .Index(t => t.Question_Id);
            
            CreateTable(
                "dbo.LearningModules",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        ModuleTag = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.QuestionAccreditations", "Question_Id", "dbo.Questions");
            DropForeignKey("dbo.QuestionAccreditations", "LearningModule_Id", "dbo.LearningModules");
            DropIndex("dbo.QuestionAccreditations", new[] { "Question_Id" });
            DropIndex("dbo.QuestionAccreditations", new[] { "LearningModule_Id" });
            DropTable("dbo.LearningModules");
            DropTable("dbo.QuestionAccreditations");
        }
    }
}
