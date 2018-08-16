namespace MyStik.TimeTable.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CurriculumV6 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.CourseModuleCourse", "Course_Id", "dbo.Activity");
            DropForeignKey("dbo.CourseModuleCourse", "ModuleCourse_Id", "dbo.ModuleCourse");
            DropIndex("dbo.CourseModuleCourse", new[] { "Course_Id" });
            DropIndex("dbo.CourseModuleCourse", new[] { "ModuleCourse_Id" });
            CreateTable(
                "dbo.CourseModuleNexus",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Course_Id = c.Guid(),
                        ModuleCourse_Id = c.Guid(),
                        Requirement_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Activity", t => t.Course_Id)
                .ForeignKey("dbo.ModuleCourse", t => t.ModuleCourse_Id)
                .ForeignKey("dbo.CurriculumRequirement", t => t.Requirement_Id)
                .Index(t => t.Course_Id)
                .Index(t => t.ModuleCourse_Id)
                .Index(t => t.Requirement_Id);
            
            AddColumn("dbo.CurriculumModule", "PreRequisites", c => c.String());
            AddColumn("dbo.CurriculumModule", "Competences", c => c.String());
            AddColumn("dbo.CurriculumModule", "Literature", c => c.String());
            AddColumn("dbo.Lottery", "IsActiveFrom", c => c.DateTime());
            AddColumn("dbo.Lottery", "IsActiveUntil", c => c.DateTime());
            AddColumn("dbo.Lottery", "IsFixed", c => c.Boolean(nullable: false));
            AddColumn("dbo.CurriculumRequirement", "CatalogId", c => c.String());
            AddColumn("dbo.CurriculumRequirement", "USCredits", c => c.Double(nullable: false));
            AddColumn("dbo.CurriculumRequirement", "SWS", c => c.Double(nullable: false));
            AddColumn("dbo.CurriculumRequirement", "LecturerInCharge_Id", c => c.Guid());
            AlterColumn("dbo.CurriculumRequirement", "ECTS", c => c.Double(nullable: false));
            CreateIndex("dbo.CurriculumRequirement", "LecturerInCharge_Id");
            AddForeignKey("dbo.CurriculumRequirement", "LecturerInCharge_Id", "dbo.OrganiserMember", "Id");
            DropColumn("dbo.CurriculumModule", "Lecturer");
            DropColumn("dbo.CurriculumModule", "Language");
            DropColumn("dbo.CurriculumModule", "SWS");
            DropColumn("dbo.CurriculumModule", "Work");
            DropColumn("dbo.CurriculumModule", "Requirements");
            DropColumn("dbo.CurriculumModule", "Skills");
            DropColumn("dbo.CurriculumModule", "Topic");
            DropColumn("dbo.CurriculumModule", "Leistung");
            DropColumn("dbo.CurriculumModule", "Books");
            DropTable("dbo.CourseModuleCourse");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.CourseModuleCourse",
                c => new
                    {
                        Course_Id = c.Guid(nullable: false),
                        ModuleCourse_Id = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.Course_Id, t.ModuleCourse_Id });
            
            AddColumn("dbo.CurriculumModule", "Books", c => c.Int(nullable: false));
            AddColumn("dbo.CurriculumModule", "Leistung", c => c.Int(nullable: false));
            AddColumn("dbo.CurriculumModule", "Topic", c => c.Int(nullable: false));
            AddColumn("dbo.CurriculumModule", "Skills", c => c.Int(nullable: false));
            AddColumn("dbo.CurriculumModule", "Requirements", c => c.Int(nullable: false));
            AddColumn("dbo.CurriculumModule", "Work", c => c.Int(nullable: false));
            AddColumn("dbo.CurriculumModule", "SWS", c => c.Int(nullable: false));
            AddColumn("dbo.CurriculumModule", "Language", c => c.Int(nullable: false));
            AddColumn("dbo.CurriculumModule", "Lecturer", c => c.Int(nullable: false));
            DropForeignKey("dbo.CourseModuleNexus", "Requirement_Id", "dbo.CurriculumRequirement");
            DropForeignKey("dbo.CurriculumRequirement", "LecturerInCharge_Id", "dbo.OrganiserMember");
            DropForeignKey("dbo.CourseModuleNexus", "ModuleCourse_Id", "dbo.ModuleCourse");
            DropForeignKey("dbo.CourseModuleNexus", "Course_Id", "dbo.Activity");
            DropIndex("dbo.CurriculumRequirement", new[] { "LecturerInCharge_Id" });
            DropIndex("dbo.CourseModuleNexus", new[] { "Requirement_Id" });
            DropIndex("dbo.CourseModuleNexus", new[] { "ModuleCourse_Id" });
            DropIndex("dbo.CourseModuleNexus", new[] { "Course_Id" });
            AlterColumn("dbo.CurriculumRequirement", "ECTS", c => c.Int(nullable: false));
            DropColumn("dbo.CurriculumRequirement", "LecturerInCharge_Id");
            DropColumn("dbo.CurriculumRequirement", "SWS");
            DropColumn("dbo.CurriculumRequirement", "USCredits");
            DropColumn("dbo.CurriculumRequirement", "CatalogId");
            DropColumn("dbo.Lottery", "IsFixed");
            DropColumn("dbo.Lottery", "IsActiveUntil");
            DropColumn("dbo.Lottery", "IsActiveFrom");
            DropColumn("dbo.CurriculumModule", "Literature");
            DropColumn("dbo.CurriculumModule", "Competences");
            DropColumn("dbo.CurriculumModule", "PreRequisites");
            DropTable("dbo.CourseModuleNexus");
            CreateIndex("dbo.CourseModuleCourse", "ModuleCourse_Id");
            CreateIndex("dbo.CourseModuleCourse", "Course_Id");
            AddForeignKey("dbo.CourseModuleCourse", "ModuleCourse_Id", "dbo.ModuleCourse", "Id", cascadeDelete: true);
            AddForeignKey("dbo.CourseModuleCourse", "Course_Id", "dbo.Activity", "Id", cascadeDelete: true);
        }
    }
}
