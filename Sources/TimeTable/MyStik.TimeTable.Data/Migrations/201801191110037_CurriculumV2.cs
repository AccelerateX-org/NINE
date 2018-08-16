namespace MyStik.TimeTable.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CurriculumV2 : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.ActivitySemesterTopic", newName: "SemesterTopicActivity");
            RenameTable(name: "dbo.SemesterGroupActivity", newName: "ActivitySemesterGroup");
            RenameTable(name: "dbo.CurriculumGroupCurriculumModule", newName: "CurriculumModuleCurriculumGroup");
            DropPrimaryKey("dbo.SemesterTopicActivity");
            DropPrimaryKey("dbo.ActivitySemesterGroup");
            DropPrimaryKey("dbo.CurriculumModuleCurriculumGroup");
            CreateTable(
                "dbo.CurriculumPackage",
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
                "dbo.PackageOption",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Name = c.String(),
                        Package_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.CurriculumPackage", t => t.Package_Id)
                .Index(t => t.Package_Id);
            
            CreateTable(
                "dbo.CurriculumVariation",
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
                "dbo.CriteriaSample",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Name = c.String(),
                        IsExcluded = c.Boolean(nullable: false),
                        Criteria_Id = c.Guid(),
                        Variation_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.CurriculumCriteria", t => t.Criteria_Id)
                .ForeignKey("dbo.CurriculumVariation", t => t.Variation_Id)
                .Index(t => t.Criteria_Id)
                .Index(t => t.Variation_Id);
            
            CreateTable(
                "dbo.PackageOptionModuleAccreditation",
                c => new
                    {
                        PackageOption_Id = c.Guid(nullable: false),
                        ModuleAccreditation_Id = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.PackageOption_Id, t.ModuleAccreditation_Id })
                .ForeignKey("dbo.PackageOption", t => t.PackageOption_Id, cascadeDelete: true)
                .ForeignKey("dbo.ModuleAccreditation", t => t.ModuleAccreditation_Id, cascadeDelete: true)
                .Index(t => t.PackageOption_Id)
                .Index(t => t.ModuleAccreditation_Id);
            
            AddColumn("dbo.CurriculumCriteria", "Chapter_Id", c => c.Guid());
            AddPrimaryKey("dbo.SemesterTopicActivity", new[] { "SemesterTopic_Id", "Activity_Id" });
            AddPrimaryKey("dbo.ActivitySemesterGroup", new[] { "Activity_Id", "SemesterGroup_Id" });
            AddPrimaryKey("dbo.CurriculumModuleCurriculumGroup", new[] { "CurriculumModule_Id", "CurriculumGroup_Id" });
            CreateIndex("dbo.CurriculumCriteria", "Chapter_Id");
            AddForeignKey("dbo.CurriculumCriteria", "Chapter_Id", "dbo.CurriculumChapter", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CriteriaSample", "Variation_Id", "dbo.CurriculumVariation");
            DropForeignKey("dbo.CriteriaSample", "Criteria_Id", "dbo.CurriculumCriteria");
            DropForeignKey("dbo.CurriculumVariation", "Curriculum_Id", "dbo.Curriculum");
            DropForeignKey("dbo.PackageOption", "Package_Id", "dbo.CurriculumPackage");
            DropForeignKey("dbo.PackageOptionModuleAccreditation", "ModuleAccreditation_Id", "dbo.ModuleAccreditation");
            DropForeignKey("dbo.PackageOptionModuleAccreditation", "PackageOption_Id", "dbo.PackageOption");
            DropForeignKey("dbo.CurriculumPackage", "Curriculum_Id", "dbo.Curriculum");
            DropForeignKey("dbo.CurriculumCriteria", "Chapter_Id", "dbo.CurriculumChapter");
            DropIndex("dbo.PackageOptionModuleAccreditation", new[] { "ModuleAccreditation_Id" });
            DropIndex("dbo.PackageOptionModuleAccreditation", new[] { "PackageOption_Id" });
            DropIndex("dbo.CriteriaSample", new[] { "Variation_Id" });
            DropIndex("dbo.CriteriaSample", new[] { "Criteria_Id" });
            DropIndex("dbo.CurriculumVariation", new[] { "Curriculum_Id" });
            DropIndex("dbo.PackageOption", new[] { "Package_Id" });
            DropIndex("dbo.CurriculumPackage", new[] { "Curriculum_Id" });
            DropIndex("dbo.CurriculumCriteria", new[] { "Chapter_Id" });
            DropPrimaryKey("dbo.CurriculumModuleCurriculumGroup");
            DropPrimaryKey("dbo.ActivitySemesterGroup");
            DropPrimaryKey("dbo.SemesterTopicActivity");
            DropColumn("dbo.CurriculumCriteria", "Chapter_Id");
            DropTable("dbo.PackageOptionModuleAccreditation");
            DropTable("dbo.CriteriaSample");
            DropTable("dbo.CurriculumVariation");
            DropTable("dbo.PackageOption");
            DropTable("dbo.CurriculumPackage");
            AddPrimaryKey("dbo.CurriculumModuleCurriculumGroup", new[] { "CurriculumGroup_Id", "CurriculumModule_Id" });
            AddPrimaryKey("dbo.ActivitySemesterGroup", new[] { "SemesterGroup_Id", "Activity_Id" });
            AddPrimaryKey("dbo.SemesterTopicActivity", new[] { "Activity_Id", "SemesterTopic_Id" });
            RenameTable(name: "dbo.CurriculumModuleCurriculumGroup", newName: "CurriculumGroupCurriculumModule");
            RenameTable(name: "dbo.ActivitySemesterGroup", newName: "SemesterGroupActivity");
            RenameTable(name: "dbo.SemesterTopicActivity", newName: "ActivitySemesterTopic");
        }
    }
}
