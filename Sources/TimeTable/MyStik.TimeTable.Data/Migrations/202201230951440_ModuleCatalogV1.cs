namespace MyStik.TimeTable.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModuleCatalogV1 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.StudentChannel",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Label_Id = c.Guid(),
                        Student_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ItemLabel", t => t.Label_Id)
                .ForeignKey("dbo.Student", t => t.Student_Id)
                .Index(t => t.Label_Id)
                .Index(t => t.Student_Id);
            
            CreateTable(
                "dbo.ItemLabel",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Name = c.String(),
                        Description = c.String(),
                        HtmlColor = c.String(),
                        Organiser_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ActivityOrganiser", t => t.Organiser_Id)
                .Index(t => t.Organiser_Id);
            
            CreateTable(
                "dbo.ItemLabelSet",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
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
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Curriculum", t => t.Curriculum_Id)
                .ForeignKey("dbo.ItemLabel", t => t.Label_Id)
                .Index(t => t.Curriculum_Id)
                .Index(t => t.Label_Id);
            
            CreateTable(
                "dbo.CurriculumSection",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Order = c.Int(nullable: false),
                        Name = c.String(),
                        Curriculum_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Curriculum", t => t.Curriculum_Id)
                .Index(t => t.Curriculum_Id);
            
            CreateTable(
                "dbo.CurriculumSlot",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        POsition = c.Int(nullable: false),
                        Tag = c.String(),
                        ECTS = c.Double(nullable: false),
                        CurriculumSection_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.CurriculumSection", t => t.CurriculumSection_Id)
                .Index(t => t.CurriculumSection_Id);
            
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
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Semester", t => t.Semester_Id)
                .ForeignKey("dbo.TeachingBuildingBlock", t => t.TeachingBuildingBlock_Id)
                .Index(t => t.Semester_Id)
                .Index(t => t.TeachingBuildingBlock_Id);
            
            CreateTable(
                "dbo.ModulePublishing",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Catalog_Id = c.Guid(),
                        Module_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ModuleCatalog", t => t.Catalog_Id)
                .ForeignKey("dbo.TeachingBuildingBlock", t => t.Module_Id)
                .Index(t => t.Catalog_Id)
                .Index(t => t.Module_Id);
            
            CreateTable(
                "dbo.ModuleCatalog",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Name = c.String(),
                        IsPublic = c.Boolean(nullable: false),
                        Owner_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.OrganiserMember", t => t.Owner_Id)
                .Index(t => t.Owner_Id);
            
            CreateTable(
                "dbo.TeachingAssessment",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Name = c.String(),
                        Module_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.TeachingBuildingBlock", t => t.Module_Id)
                .Index(t => t.Module_Id);
            
            CreateTable(
                "dbo.SubjectAccreditation",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Semester_Id = c.Guid(),
                        Slot_Id = c.Guid(),
                        Subject_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Semester", t => t.Semester_Id)
                .ForeignKey("dbo.CurriculumSlot", t => t.Slot_Id)
                .ForeignKey("dbo.TeachingUnit", t => t.Subject_Id)
                .Index(t => t.Semester_Id)
                .Index(t => t.Slot_Id)
                .Index(t => t.Subject_Id);
            
            CreateTable(
                "dbo.ItemLabelSetItemLabel",
                c => new
                    {
                        ItemLabelSet_Id = c.Guid(nullable: false),
                        ItemLabel_Id = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.ItemLabelSet_Id, t.ItemLabel_Id })
                .ForeignKey("dbo.ItemLabelSet", t => t.ItemLabelSet_Id, cascadeDelete: true)
                .ForeignKey("dbo.ItemLabel", t => t.ItemLabel_Id, cascadeDelete: true)
                .Index(t => t.ItemLabelSet_Id)
                .Index(t => t.ItemLabel_Id);
            
            AddColumn("dbo.ExaminationUnit", "Assessment_Id", c => c.Guid());
            CreateIndex("dbo.ExaminationUnit", "Assessment_Id");
            AddForeignKey("dbo.ExaminationUnit", "Assessment_Id", "dbo.TeachingAssessment", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SubjectAccreditation", "Subject_Id", "dbo.TeachingUnit");
            DropForeignKey("dbo.SubjectAccreditation", "Slot_Id", "dbo.CurriculumSlot");
            DropForeignKey("dbo.SubjectAccreditation", "Semester_Id", "dbo.Semester");
            DropForeignKey("dbo.TeachingAssessment", "Module_Id", "dbo.TeachingBuildingBlock");
            DropForeignKey("dbo.ExaminationUnit", "Assessment_Id", "dbo.TeachingAssessment");
            DropForeignKey("dbo.ModulePublishing", "Module_Id", "dbo.TeachingBuildingBlock");
            DropForeignKey("dbo.ModulePublishing", "Catalog_Id", "dbo.ModuleCatalog");
            DropForeignKey("dbo.ModuleCatalog", "Owner_Id", "dbo.OrganiserMember");
            DropForeignKey("dbo.TeachingModule", "TeachingBuildingBlock_Id", "dbo.TeachingBuildingBlock");
            DropForeignKey("dbo.TeachingModule", "Semester_Id", "dbo.Semester");
            DropForeignKey("dbo.CurriculumSlot", "CurriculumSection_Id", "dbo.CurriculumSection");
            DropForeignKey("dbo.CurriculumSection", "Curriculum_Id", "dbo.Curriculum");
            DropForeignKey("dbo.CurriculumScope", "Label_Id", "dbo.ItemLabel");
            DropForeignKey("dbo.CurriculumScope", "Curriculum_Id", "dbo.Curriculum");
            DropForeignKey("dbo.StudentChannel", "Student_Id", "dbo.Student");
            DropForeignKey("dbo.StudentChannel", "Label_Id", "dbo.ItemLabel");
            DropForeignKey("dbo.ItemLabel", "Organiser_Id", "dbo.ActivityOrganiser");
            DropForeignKey("dbo.ItemLabelSetItemLabel", "ItemLabel_Id", "dbo.ItemLabel");
            DropForeignKey("dbo.ItemLabelSetItemLabel", "ItemLabelSet_Id", "dbo.ItemLabelSet");
            DropIndex("dbo.ItemLabelSetItemLabel", new[] { "ItemLabel_Id" });
            DropIndex("dbo.ItemLabelSetItemLabel", new[] { "ItemLabelSet_Id" });
            DropIndex("dbo.SubjectAccreditation", new[] { "Subject_Id" });
            DropIndex("dbo.SubjectAccreditation", new[] { "Slot_Id" });
            DropIndex("dbo.SubjectAccreditation", new[] { "Semester_Id" });
            DropIndex("dbo.ExaminationUnit", new[] { "Assessment_Id" });
            DropIndex("dbo.TeachingAssessment", new[] { "Module_Id" });
            DropIndex("dbo.ModuleCatalog", new[] { "Owner_Id" });
            DropIndex("dbo.ModulePublishing", new[] { "Module_Id" });
            DropIndex("dbo.ModulePublishing", new[] { "Catalog_Id" });
            DropIndex("dbo.TeachingModule", new[] { "TeachingBuildingBlock_Id" });
            DropIndex("dbo.TeachingModule", new[] { "Semester_Id" });
            DropIndex("dbo.CurriculumSlot", new[] { "CurriculumSection_Id" });
            DropIndex("dbo.CurriculumSection", new[] { "Curriculum_Id" });
            DropIndex("dbo.CurriculumScope", new[] { "Label_Id" });
            DropIndex("dbo.CurriculumScope", new[] { "Curriculum_Id" });
            DropIndex("dbo.ItemLabel", new[] { "Organiser_Id" });
            DropIndex("dbo.StudentChannel", new[] { "Student_Id" });
            DropIndex("dbo.StudentChannel", new[] { "Label_Id" });
            DropColumn("dbo.ExaminationUnit", "Assessment_Id");
            DropTable("dbo.ItemLabelSetItemLabel");
            DropTable("dbo.SubjectAccreditation");
            DropTable("dbo.TeachingAssessment");
            DropTable("dbo.ModuleCatalog");
            DropTable("dbo.ModulePublishing");
            DropTable("dbo.TeachingModule");
            DropTable("dbo.CurriculumSlot");
            DropTable("dbo.CurriculumSection");
            DropTable("dbo.CurriculumScope");
            DropTable("dbo.ItemLabelSet");
            DropTable("dbo.ItemLabel");
            DropTable("dbo.StudentChannel");
        }
    }
}
