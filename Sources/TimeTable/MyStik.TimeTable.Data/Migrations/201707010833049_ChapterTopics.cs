namespace MyStik.TimeTable.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChapterTopics : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.CurriculumModuleCurriculumGroup", newName: "CurriculumGroupCurriculumModule");
            RenameTable(name: "dbo.ActivitySemesterGroup", newName: "SemesterGroupActivity");
            DropPrimaryKey("dbo.CurriculumGroupCurriculumModule");
            DropPrimaryKey("dbo.SemesterGroupActivity");
            CreateTable(
                "dbo.CurriculumChapter",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Name = c.String(),
                        Curriculum_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Curriculum", t => t.Curriculum_Id)
                .Index(t => t.Curriculum_Id);
            
            CreateTable(
                "dbo.CurriculumTopic",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Name = c.String(),
                        Chapter_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.CurriculumChapter", t => t.Chapter_Id)
                .Index(t => t.Chapter_Id);
            
            CreateTable(
                "dbo.SemesterTopic",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Semester_Id = c.Guid(),
                        Topic_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Semester", t => t.Semester_Id)
                .ForeignKey("dbo.CurriculumTopic", t => t.Topic_Id)
                .Index(t => t.Semester_Id)
                .Index(t => t.Topic_Id);
            
            CreateTable(
                "dbo.ActivitySemesterTopic",
                c => new
                    {
                        Activity_Id = c.Guid(nullable: false),
                        SemesterTopic_Id = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.Activity_Id, t.SemesterTopic_Id })
                .ForeignKey("dbo.Activity", t => t.Activity_Id, cascadeDelete: true)
                .ForeignKey("dbo.SemesterTopic", t => t.SemesterTopic_Id, cascadeDelete: true)
                .Index(t => t.Activity_Id)
                .Index(t => t.SemesterTopic_Id);
            
            AddPrimaryKey("dbo.CurriculumGroupCurriculumModule", new[] { "CurriculumGroup_Id", "CurriculumModule_Id" });
            AddPrimaryKey("dbo.SemesterGroupActivity", new[] { "SemesterGroup_Id", "Activity_Id" });
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SemesterTopic", "Topic_Id", "dbo.CurriculumTopic");
            DropForeignKey("dbo.SemesterTopic", "Semester_Id", "dbo.Semester");
            DropForeignKey("dbo.ActivitySemesterTopic", "SemesterTopic_Id", "dbo.SemesterTopic");
            DropForeignKey("dbo.ActivitySemesterTopic", "Activity_Id", "dbo.Activity");
            DropForeignKey("dbo.CurriculumTopic", "Chapter_Id", "dbo.CurriculumChapter");
            DropForeignKey("dbo.CurriculumChapter", "Curriculum_Id", "dbo.Curriculum");
            DropIndex("dbo.ActivitySemesterTopic", new[] { "SemesterTopic_Id" });
            DropIndex("dbo.ActivitySemesterTopic", new[] { "Activity_Id" });
            DropIndex("dbo.SemesterTopic", new[] { "Topic_Id" });
            DropIndex("dbo.SemesterTopic", new[] { "Semester_Id" });
            DropIndex("dbo.CurriculumTopic", new[] { "Chapter_Id" });
            DropIndex("dbo.CurriculumChapter", new[] { "Curriculum_Id" });
            DropPrimaryKey("dbo.SemesterGroupActivity");
            DropPrimaryKey("dbo.CurriculumGroupCurriculumModule");
            DropTable("dbo.ActivitySemesterTopic");
            DropTable("dbo.SemesterTopic");
            DropTable("dbo.CurriculumTopic");
            DropTable("dbo.CurriculumChapter");
            AddPrimaryKey("dbo.SemesterGroupActivity", new[] { "Activity_Id", "SemesterGroup_Id" });
            AddPrimaryKey("dbo.CurriculumGroupCurriculumModule", new[] { "CurriculumModule_Id", "CurriculumGroup_Id" });
            RenameTable(name: "dbo.SemesterGroupActivity", newName: "ActivitySemesterGroup");
            RenameTable(name: "dbo.CurriculumGroupCurriculumModule", newName: "CurriculumModuleCurriculumGroup");
        }
    }
}
