namespace MyStik.TimeTable.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ConsolidationStep04 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.CapacityCourse", "Course_Id", "dbo.ModuleCourse");
            DropForeignKey("dbo.CourseModuleNexus", "Course_Id", "dbo.Activity");
            DropForeignKey("dbo.CourseModuleNexus", "ModuleCourse_Id", "dbo.ModuleCourse");
            DropForeignKey("dbo.Curriculum", "ValidSince_Id", "dbo.Semester");
            DropForeignKey("dbo.CurriculumAccreditation", "Curriculum_Id", "dbo.Curriculum");
            DropForeignKey("dbo.CurriculumAccreditation", "Program_Id", "dbo.CurriculumProgram");
            DropForeignKey("dbo.CurriculumProgram", "Autonomy_Id", "dbo.Autonomy");
            DropForeignKey("dbo.CurriculumScope", "Curriculum_Id", "dbo.Curriculum");
            DropForeignKey("dbo.CurriculumScope", "Label_Id", "dbo.ItemLabel");
            DropForeignKey("dbo.Lecturer", "Member_Id", "dbo.OrganiserMember");
            DropForeignKey("dbo.ModuleCatalog", "Owner_Id", "dbo.OrganiserMember");
            DropForeignKey("dbo.ModulePublishing", "Catalog_Id", "dbo.ModuleCatalog");
            DropIndex("dbo.Curriculum", new[] { "ValidSince_Id" });
            DropIndex("dbo.CapacityCourse", new[] { "Course_Id" });
            DropIndex("dbo.CourseModuleNexus", new[] { "Course_Id" });
            DropIndex("dbo.CourseModuleNexus", new[] { "ModuleCourse_Id" });
            DropIndex("dbo.CurriculumAccreditation", new[] { "Curriculum_Id" });
            DropIndex("dbo.CurriculumAccreditation", new[] { "Program_Id" });
            DropIndex("dbo.CurriculumProgram", new[] { "Autonomy_Id" });
            DropIndex("dbo.CurriculumScope", new[] { "Curriculum_Id" });
            DropIndex("dbo.CurriculumScope", new[] { "Label_Id" });
            DropIndex("dbo.Lecturer", new[] { "Member_Id" });
            DropIndex("dbo.ModuleCatalog", new[] { "Owner_Id" });
            DropIndex("dbo.ModulePublishing", new[] { "Catalog_Id" });
            CreateTable(
                "dbo.CurriculumOpportunity",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        IsPublished = c.Boolean(nullable: false),
                        Curriculum_Id = c.Guid(),
                        Semester_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Curriculum", t => t.Curriculum_Id)
                .ForeignKey("dbo.Semester", t => t.Semester_Id)
                .Index(t => t.Curriculum_Id)
                .Index(t => t.Semester_Id);
            
            AddColumn("dbo.Curriculum", "Tag", c => c.String());
            AddColumn("dbo.Curriculum", "EctsTarget", c => c.Double(nullable: false));
            AddColumn("dbo.CommitteeMember", "UserId", c => c.String());
            DropColumn("dbo.Curriculum", "ValidSince_Id");
            DropTable("dbo.CapacityCourse");
            DropTable("dbo.CourseModuleNexus");
            DropTable("dbo.CurriculumAccreditation");
            DropTable("dbo.CurriculumProgram");
            DropTable("dbo.CurriculumScope");
            DropTable("dbo.Lecturer");
            DropTable("dbo.ModuleCatalog");
            DropTable("dbo.ModulePublishing");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.ModulePublishing",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Catalog_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ModuleCatalog",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Name = c.String(),
                        IsPublic = c.Boolean(nullable: false),
                        Owner_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Lecturer",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        IsAdmin = c.Boolean(nullable: false),
                        Member_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.CurriculumScope",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        IsRequired = c.Boolean(nullable: false),
                        EarliestSection = c.Int(nullable: false),
                        LatestSection = c.Int(nullable: false),
                        Curriculum_Id = c.Guid(),
                        Label_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.CurriculumProgram",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Name = c.String(),
                        Autonomy_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.CurriculumAccreditation",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Curriculum_Id = c.Guid(),
                        Program_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.CourseModuleNexus",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Course_Id = c.Guid(),
                        ModuleCourse_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.CapacityCourse",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        ShortName = c.String(),
                        Course_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.Curriculum", "ValidSince_Id", c => c.Guid());
            DropForeignKey("dbo.CurriculumOpportunity", "Semester_Id", "dbo.Semester");
            DropForeignKey("dbo.CurriculumOpportunity", "Curriculum_Id", "dbo.Curriculum");
            DropIndex("dbo.CurriculumOpportunity", new[] { "Semester_Id" });
            DropIndex("dbo.CurriculumOpportunity", new[] { "Curriculum_Id" });
            DropColumn("dbo.CommitteeMember", "UserId");
            DropColumn("dbo.Curriculum", "EctsTarget");
            DropColumn("dbo.Curriculum", "Tag");
            DropTable("dbo.CurriculumOpportunity");
            CreateIndex("dbo.ModulePublishing", "Catalog_Id");
            CreateIndex("dbo.ModuleCatalog", "Owner_Id");
            CreateIndex("dbo.Lecturer", "Member_Id");
            CreateIndex("dbo.CurriculumScope", "Label_Id");
            CreateIndex("dbo.CurriculumScope", "Curriculum_Id");
            CreateIndex("dbo.CurriculumProgram", "Autonomy_Id");
            CreateIndex("dbo.CurriculumAccreditation", "Program_Id");
            CreateIndex("dbo.CurriculumAccreditation", "Curriculum_Id");
            CreateIndex("dbo.CourseModuleNexus", "ModuleCourse_Id");
            CreateIndex("dbo.CourseModuleNexus", "Course_Id");
            CreateIndex("dbo.CapacityCourse", "Course_Id");
            CreateIndex("dbo.Curriculum", "ValidSince_Id");
            AddForeignKey("dbo.ModulePublishing", "Catalog_Id", "dbo.ModuleCatalog", "Id");
            AddForeignKey("dbo.ModuleCatalog", "Owner_Id", "dbo.OrganiserMember", "Id");
            AddForeignKey("dbo.Lecturer", "Member_Id", "dbo.OrganiserMember", "Id");
            AddForeignKey("dbo.CurriculumScope", "Label_Id", "dbo.ItemLabel", "Id");
            AddForeignKey("dbo.CurriculumScope", "Curriculum_Id", "dbo.Curriculum", "Id");
            AddForeignKey("dbo.CurriculumProgram", "Autonomy_Id", "dbo.Autonomy", "Id");
            AddForeignKey("dbo.CurriculumAccreditation", "Program_Id", "dbo.CurriculumProgram", "Id");
            AddForeignKey("dbo.CurriculumAccreditation", "Curriculum_Id", "dbo.Curriculum", "Id");
            AddForeignKey("dbo.Curriculum", "ValidSince_Id", "dbo.Semester", "Id");
            AddForeignKey("dbo.CourseModuleNexus", "ModuleCourse_Id", "dbo.ModuleCourse", "Id");
            AddForeignKey("dbo.CourseModuleNexus", "Course_Id", "dbo.Activity", "Id");
            AddForeignKey("dbo.CapacityCourse", "Course_Id", "dbo.ModuleCourse", "Id");
        }
    }
}
