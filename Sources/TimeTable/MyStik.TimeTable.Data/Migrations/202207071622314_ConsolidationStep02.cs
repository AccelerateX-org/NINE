namespace MyStik.TimeTable.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ConsolidationStep02 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.CurriculumCriteriaCurriculumGroup", "CurriculumCriteria_Id", "dbo.CurriculumCriteria");
            DropForeignKey("dbo.CurriculumCriteriaCurriculumGroup", "CurriculumGroup_Id", "dbo.CurriculumGroup");
            DropForeignKey("dbo.CurriculumCriteria", "Requirement_Id", "dbo.CurriculumRequirement");
            DropForeignKey("dbo.CurriculumRequirement", "LecturerInCharge_Id", "dbo.OrganiserMember");
            DropForeignKey("dbo.CourseModuleNexus", "Requirement_Id", "dbo.CurriculumRequirement");
            DropForeignKey("dbo.CurriculumPackage", "Curriculum_Id", "dbo.Curriculum");
            DropForeignKey("dbo.PackageOption", "Package_Id", "dbo.CurriculumPackage");
            DropForeignKey("dbo.CurriculumRequirement", "Option_Id", "dbo.PackageOption");
            DropForeignKey("dbo.CriteriaRule", "Criteria_Id", "dbo.CurriculumCriteria");
            DropForeignKey("dbo.CriteriaRule", "Group_Id", "dbo.CurriculumGroup");
            DropForeignKey("dbo.CriteriaSample", "Criteria_Id", "dbo.CurriculumCriteria");
            DropForeignKey("dbo.CurriculumVariation", "Curriculum_Id", "dbo.Curriculum");
            DropForeignKey("dbo.CriteriaSample", "Variation_Id", "dbo.CurriculumVariation");
            DropIndex("dbo.CurriculumCriteria", new[] { "Requirement_Id" });
            DropIndex("dbo.CurriculumRequirement", new[] { "LecturerInCharge_Id" });
            DropIndex("dbo.CurriculumRequirement", new[] { "Option_Id" });
            DropIndex("dbo.CourseModuleNexus", new[] { "Requirement_Id" });
            DropIndex("dbo.PackageOption", new[] { "Package_Id" });
            DropIndex("dbo.CurriculumPackage", new[] { "Curriculum_Id" });
            DropIndex("dbo.CriteriaRule", new[] { "Criteria_Id" });
            DropIndex("dbo.CriteriaRule", new[] { "Group_Id" });
            DropIndex("dbo.CriteriaSample", new[] { "Criteria_Id" });
            DropIndex("dbo.CriteriaSample", new[] { "Variation_Id" });
            DropIndex("dbo.CurriculumVariation", new[] { "Curriculum_Id" });
            DropIndex("dbo.CurriculumCriteriaCurriculumGroup", new[] { "CurriculumCriteria_Id" });
            DropIndex("dbo.CurriculumCriteriaCurriculumGroup", new[] { "CurriculumGroup_Id" });
            AddColumn("dbo.CurriculumGroup", "CurriculumCriteria_Id", c => c.Guid());
            CreateIndex("dbo.CurriculumGroup", "CurriculumCriteria_Id");
            AddForeignKey("dbo.CurriculumGroup", "CurriculumCriteria_Id", "dbo.CurriculumCriteria", "Id");
            DropColumn("dbo.CurriculumCriteria", "Requirement_Id");
            DropColumn("dbo.CourseModuleNexus", "Requirement_Id");
            DropTable("dbo.CurriculumRequirement");
            DropTable("dbo.PackageOption");
            DropTable("dbo.CurriculumPackage");
            DropTable("dbo.CriteriaRule");
            DropTable("dbo.CriteriaSample");
            DropTable("dbo.CurriculumVariation");
            DropTable("dbo.CurriculumCriteriaCurriculumGroup");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.CurriculumCriteriaCurriculumGroup",
                c => new
                    {
                        CurriculumCriteria_Id = c.Guid(nullable: false),
                        CurriculumGroup_Id = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.CurriculumCriteria_Id, t.CurriculumGroup_Id });
            
            CreateTable(
                "dbo.CurriculumVariation",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Name = c.String(),
                        Curriculum_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id);
            
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
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.CriteriaRule",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        AtEarliest = c.Boolean(),
                        AtLatest = c.Boolean(),
                        Criteria_Id = c.Guid(),
                        Group_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.CurriculumPackage",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Name = c.String(),
                        Curriculum_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.PackageOption",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Name = c.String(),
                        Package_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.CurriculumRequirement",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Name = c.String(),
                        ShortName = c.String(),
                        CatalogId = c.String(),
                        ECTS = c.Double(nullable: false),
                        USCredits = c.Double(nullable: false),
                        SWS = c.Double(nullable: false),
                        LecturerInCharge_Id = c.Guid(),
                        Option_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.CourseModuleNexus", "Requirement_Id", c => c.Guid());
            AddColumn("dbo.CurriculumCriteria", "Requirement_Id", c => c.Guid());
            DropForeignKey("dbo.CurriculumGroup", "CurriculumCriteria_Id", "dbo.CurriculumCriteria");
            DropIndex("dbo.CurriculumGroup", new[] { "CurriculumCriteria_Id" });
            DropColumn("dbo.CurriculumGroup", "CurriculumCriteria_Id");
            CreateIndex("dbo.CurriculumCriteriaCurriculumGroup", "CurriculumGroup_Id");
            CreateIndex("dbo.CurriculumCriteriaCurriculumGroup", "CurriculumCriteria_Id");
            CreateIndex("dbo.CurriculumVariation", "Curriculum_Id");
            CreateIndex("dbo.CriteriaSample", "Variation_Id");
            CreateIndex("dbo.CriteriaSample", "Criteria_Id");
            CreateIndex("dbo.CriteriaRule", "Group_Id");
            CreateIndex("dbo.CriteriaRule", "Criteria_Id");
            CreateIndex("dbo.CurriculumPackage", "Curriculum_Id");
            CreateIndex("dbo.PackageOption", "Package_Id");
            CreateIndex("dbo.CourseModuleNexus", "Requirement_Id");
            CreateIndex("dbo.CurriculumRequirement", "Option_Id");
            CreateIndex("dbo.CurriculumRequirement", "LecturerInCharge_Id");
            CreateIndex("dbo.CurriculumCriteria", "Requirement_Id");
            AddForeignKey("dbo.CriteriaSample", "Variation_Id", "dbo.CurriculumVariation", "Id");
            AddForeignKey("dbo.CurriculumVariation", "Curriculum_Id", "dbo.Curriculum", "Id");
            AddForeignKey("dbo.CriteriaSample", "Criteria_Id", "dbo.CurriculumCriteria", "Id");
            AddForeignKey("dbo.CriteriaRule", "Group_Id", "dbo.CurriculumGroup", "Id");
            AddForeignKey("dbo.CriteriaRule", "Criteria_Id", "dbo.CurriculumCriteria", "Id");
            AddForeignKey("dbo.CurriculumRequirement", "Option_Id", "dbo.PackageOption", "Id");
            AddForeignKey("dbo.PackageOption", "Package_Id", "dbo.CurriculumPackage", "Id");
            AddForeignKey("dbo.CurriculumPackage", "Curriculum_Id", "dbo.Curriculum", "Id");
            AddForeignKey("dbo.CourseModuleNexus", "Requirement_Id", "dbo.CurriculumRequirement", "Id");
            AddForeignKey("dbo.CurriculumRequirement", "LecturerInCharge_Id", "dbo.OrganiserMember", "Id");
            AddForeignKey("dbo.CurriculumCriteria", "Requirement_Id", "dbo.CurriculumRequirement", "Id");
            AddForeignKey("dbo.CurriculumCriteriaCurriculumGroup", "CurriculumGroup_Id", "dbo.CurriculumGroup", "Id", cascadeDelete: true);
            AddForeignKey("dbo.CurriculumCriteriaCurriculumGroup", "CurriculumCriteria_Id", "dbo.CurriculumCriteria", "Id", cascadeDelete: true);
        }
    }
}
