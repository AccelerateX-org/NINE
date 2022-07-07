namespace MyStik.TimeTable.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ConsolidationStep01 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.SubjectAccreditationTeachingUnit", "SubjectAccreditation_Id", "dbo.SubjectAccreditation");
            DropForeignKey("dbo.SubjectAccreditationTeachingUnit", "TeachingUnit_Id", "dbo.TeachingUnit");
            DropForeignKey("dbo.TeachingUnit", "Form_Id", "dbo.TeachingForm");
            DropForeignKey("dbo.Lecturer", "Module_Id", "dbo.TeachingBuildingBlock");
            DropForeignKey("dbo.TeachingModule", "Semester_Id", "dbo.Semester");
            DropForeignKey("dbo.TeachingModule", "TeachingBuildingBlock_Id", "dbo.TeachingBuildingBlock");
            DropForeignKey("dbo.ModulePublishing", "Module_Id", "dbo.TeachingBuildingBlock");
            DropForeignKey("dbo.ExaminationUnit", "Assessment_Id", "dbo.TeachingAssessment");
            DropForeignKey("dbo.ExaminationUnit", "Module_Id", "dbo.TeachingBuildingBlock");
            DropForeignKey("dbo.TeachingAssessment", "Module_Id", "dbo.TeachingBuildingBlock");
            DropForeignKey("dbo.TeachingUnit", "Module_Id", "dbo.TeachingBuildingBlock");
            DropForeignKey("dbo.Activity", "TeachingUnit_Id", "dbo.TeachingUnit");
            DropForeignKey("dbo.ModuleAccreditation", "TeachingBuildingBlock_Id", "dbo.TeachingBuildingBlock");
            DropIndex("dbo.ModuleAccreditation", new[] { "TeachingBuildingBlock_Id" });
            DropIndex("dbo.Activity", new[] { "TeachingUnit_Id" });
            DropIndex("dbo.TeachingUnit", new[] { "Form_Id" });
            DropIndex("dbo.TeachingUnit", new[] { "Module_Id" });
            DropIndex("dbo.Lecturer", new[] { "Module_Id" });
            DropIndex("dbo.TeachingModule", new[] { "Semester_Id" });
            DropIndex("dbo.TeachingModule", new[] { "TeachingBuildingBlock_Id" });
            DropIndex("dbo.ModulePublishing", new[] { "Module_Id" });
            DropIndex("dbo.TeachingAssessment", new[] { "Module_Id" });
            DropIndex("dbo.ExaminationUnit", new[] { "Assessment_Id" });
            DropIndex("dbo.ExaminationUnit", new[] { "Module_Id" });
            DropIndex("dbo.SubjectAccreditationTeachingUnit", new[] { "SubjectAccreditation_Id" });
            DropIndex("dbo.SubjectAccreditationTeachingUnit", new[] { "TeachingUnit_Id" });
            DropColumn("dbo.ModuleAccreditation", "TeachingBuildingBlock_Id");
            DropColumn("dbo.Activity", "TeachingUnit_Id");
            DropColumn("dbo.Lecturer", "Module_Id");
            DropColumn("dbo.ModulePublishing", "Module_Id");
            DropColumn("dbo.ExaminationUnit", "Assessment_Id");
            DropColumn("dbo.ExaminationUnit", "Module_Id");
            DropTable("dbo.TeachingUnit");
            DropTable("dbo.TeachingForm");
            DropTable("dbo.TeachingBuildingBlock");
            DropTable("dbo.TeachingModule");
            DropTable("dbo.TeachingAssessment");
            DropTable("dbo.SubjectAccreditationTeachingUnit");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.SubjectAccreditationTeachingUnit",
                c => new
                    {
                        SubjectAccreditation_Id = c.Guid(nullable: false),
                        TeachingUnit_Id = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.SubjectAccreditation_Id, t.TeachingUnit_Id });
            
            CreateTable(
                "dbo.TeachingAssessment",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Name = c.String(),
                        Module_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.TeachingModule",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Name = c.String(),
                        Description = c.String(),
                        ECTS = c.Int(nullable: false),
                        Semester_Id = c.Guid(),
                        TeachingBuildingBlock_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.TeachingBuildingBlock",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Name = c.String(),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.TeachingForm",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Name = c.String(),
                        ShortName = c.String(),
                        Description = c.String(),
                        Capacity = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.TeachingUnit",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Tag = c.String(),
                        Name = c.String(),
                        Description = c.String(),
                        SWS = c.Double(nullable: false),
                        Form_Id = c.Guid(),
                        Module_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.ExaminationUnit", "Module_Id", c => c.Guid());
            AddColumn("dbo.ExaminationUnit", "Assessment_Id", c => c.Guid());
            AddColumn("dbo.ModulePublishing", "Module_Id", c => c.Guid());
            AddColumn("dbo.Lecturer", "Module_Id", c => c.Guid());
            AddColumn("dbo.Activity", "TeachingUnit_Id", c => c.Guid());
            AddColumn("dbo.ModuleAccreditation", "TeachingBuildingBlock_Id", c => c.Guid());
            CreateIndex("dbo.SubjectAccreditationTeachingUnit", "TeachingUnit_Id");
            CreateIndex("dbo.SubjectAccreditationTeachingUnit", "SubjectAccreditation_Id");
            CreateIndex("dbo.ExaminationUnit", "Module_Id");
            CreateIndex("dbo.ExaminationUnit", "Assessment_Id");
            CreateIndex("dbo.TeachingAssessment", "Module_Id");
            CreateIndex("dbo.ModulePublishing", "Module_Id");
            CreateIndex("dbo.TeachingModule", "TeachingBuildingBlock_Id");
            CreateIndex("dbo.TeachingModule", "Semester_Id");
            CreateIndex("dbo.Lecturer", "Module_Id");
            CreateIndex("dbo.TeachingUnit", "Module_Id");
            CreateIndex("dbo.TeachingUnit", "Form_Id");
            CreateIndex("dbo.Activity", "TeachingUnit_Id");
            CreateIndex("dbo.ModuleAccreditation", "TeachingBuildingBlock_Id");
            AddForeignKey("dbo.ModuleAccreditation", "TeachingBuildingBlock_Id", "dbo.TeachingBuildingBlock", "Id");
            AddForeignKey("dbo.Activity", "TeachingUnit_Id", "dbo.TeachingUnit", "Id");
            AddForeignKey("dbo.TeachingUnit", "Module_Id", "dbo.TeachingBuildingBlock", "Id");
            AddForeignKey("dbo.TeachingAssessment", "Module_Id", "dbo.TeachingBuildingBlock", "Id");
            AddForeignKey("dbo.ExaminationUnit", "Module_Id", "dbo.TeachingBuildingBlock", "Id");
            AddForeignKey("dbo.ExaminationUnit", "Assessment_Id", "dbo.TeachingAssessment", "Id");
            AddForeignKey("dbo.ModulePublishing", "Module_Id", "dbo.TeachingBuildingBlock", "Id");
            AddForeignKey("dbo.TeachingModule", "TeachingBuildingBlock_Id", "dbo.TeachingBuildingBlock", "Id");
            AddForeignKey("dbo.TeachingModule", "Semester_Id", "dbo.Semester", "Id");
            AddForeignKey("dbo.Lecturer", "Module_Id", "dbo.TeachingBuildingBlock", "Id");
            AddForeignKey("dbo.TeachingUnit", "Form_Id", "dbo.TeachingForm", "Id");
            AddForeignKey("dbo.SubjectAccreditationTeachingUnit", "TeachingUnit_Id", "dbo.TeachingUnit", "Id", cascadeDelete: true);
            AddForeignKey("dbo.SubjectAccreditationTeachingUnit", "SubjectAccreditation_Id", "dbo.SubjectAccreditation", "Id", cascadeDelete: true);
        }
    }
}
