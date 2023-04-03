namespace MyStik.TimeTable.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModuleDescV2 : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.ItemLabelSetItemLabel", newName: "ItemLabelItemLabelSet");
            DropPrimaryKey("dbo.ItemLabelItemLabelSet");
            CreateTable(
                "dbo.CurriculumArea",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Name = c.String(),
                        Tag = c.String(),
                        Description = c.String(),
                        Curriculum_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Curriculum", t => t.Curriculum_Id)
                .Index(t => t.Curriculum_Id);
            
            CreateTable(
                "dbo.AreaOption",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Name = c.String(),
                        Tag = c.String(),
                        Description = c.String(),
                        Area_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.CurriculumArea", t => t.Area_Id)
                .Index(t => t.Area_Id);
            
            CreateTable(
                "dbo.CatalogResponsibility",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Catalog_Id = c.Guid(),
                        Member_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.CurriculumModuleCatalog", t => t.Catalog_Id)
                .ForeignKey("dbo.OrganiserMember", t => t.Member_Id)
                .Index(t => t.Catalog_Id)
                .Index(t => t.Member_Id);
            
            CreateTable(
                "dbo.ExaminationDescription",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Duration = c.Int(),
                        Description = c.String(),
                        Conditions = c.String(),
                        Utilities = c.String(),
                        Accreditation_Id = c.Guid(),
                        ChangeLog_Id = c.Guid(),
                        ExaminationOption_Id = c.Guid(),
                        FirstExminer_Id = c.Guid(),
                        SecondExaminer_Id = c.Guid(),
                        Semester_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ModuleAccreditation", t => t.Accreditation_Id)
                .ForeignKey("dbo.ChangeLog", t => t.ChangeLog_Id)
                .ForeignKey("dbo.ExaminationOption", t => t.ExaminationOption_Id)
                .ForeignKey("dbo.OrganiserMember", t => t.FirstExminer_Id)
                .ForeignKey("dbo.OrganiserMember", t => t.SecondExaminer_Id)
                .ForeignKey("dbo.Semester", t => t.Semester_Id)
                .Index(t => t.Accreditation_Id)
                .Index(t => t.ChangeLog_Id)
                .Index(t => t.ExaminationOption_Id)
                .Index(t => t.FirstExminer_Id)
                .Index(t => t.SecondExaminer_Id)
                .Index(t => t.Semester_Id);
            
            CreateTable(
                "dbo.TeachingDescription",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Accreditation_Id = c.Guid(),
                        Course_Id = c.Guid(),
                        Semester_Id = c.Guid(),
                        Subject_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ModuleAccreditation", t => t.Accreditation_Id)
                .ForeignKey("dbo.Activity", t => t.Course_Id)
                .ForeignKey("dbo.Semester", t => t.Semester_Id)
                .ForeignKey("dbo.ModuleSubject", t => t.Subject_Id)
                .Index(t => t.Accreditation_Id)
                .Index(t => t.Course_Id)
                .Index(t => t.Semester_Id)
                .Index(t => t.Subject_Id);
            
            AddColumn("dbo.CurriculumSlot", "Semester", c => c.Int(nullable: false));
            AddColumn("dbo.CurriculumSlot", "AreaOption_Id", c => c.Guid());
            AddPrimaryKey("dbo.ItemLabelItemLabelSet", new[] { "ItemLabel_Id", "ItemLabelSet_Id" });
            CreateIndex("dbo.CurriculumSlot", "AreaOption_Id");
            AddForeignKey("dbo.CurriculumSlot", "AreaOption_Id", "dbo.AreaOption", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TeachingDescription", "Subject_Id", "dbo.ModuleSubject");
            DropForeignKey("dbo.TeachingDescription", "Semester_Id", "dbo.Semester");
            DropForeignKey("dbo.TeachingDescription", "Course_Id", "dbo.Activity");
            DropForeignKey("dbo.TeachingDescription", "Accreditation_Id", "dbo.ModuleAccreditation");
            DropForeignKey("dbo.ExaminationDescription", "Semester_Id", "dbo.Semester");
            DropForeignKey("dbo.ExaminationDescription", "SecondExaminer_Id", "dbo.OrganiserMember");
            DropForeignKey("dbo.ExaminationDescription", "FirstExminer_Id", "dbo.OrganiserMember");
            DropForeignKey("dbo.ExaminationDescription", "ExaminationOption_Id", "dbo.ExaminationOption");
            DropForeignKey("dbo.ExaminationDescription", "ChangeLog_Id", "dbo.ChangeLog");
            DropForeignKey("dbo.ExaminationDescription", "Accreditation_Id", "dbo.ModuleAccreditation");
            DropForeignKey("dbo.CatalogResponsibility", "Member_Id", "dbo.OrganiserMember");
            DropForeignKey("dbo.CatalogResponsibility", "Catalog_Id", "dbo.CurriculumModuleCatalog");
            DropForeignKey("dbo.CurriculumSlot", "AreaOption_Id", "dbo.AreaOption");
            DropForeignKey("dbo.AreaOption", "Area_Id", "dbo.CurriculumArea");
            DropForeignKey("dbo.CurriculumArea", "Curriculum_Id", "dbo.Curriculum");
            DropIndex("dbo.TeachingDescription", new[] { "Subject_Id" });
            DropIndex("dbo.TeachingDescription", new[] { "Semester_Id" });
            DropIndex("dbo.TeachingDescription", new[] { "Course_Id" });
            DropIndex("dbo.TeachingDescription", new[] { "Accreditation_Id" });
            DropIndex("dbo.ExaminationDescription", new[] { "Semester_Id" });
            DropIndex("dbo.ExaminationDescription", new[] { "SecondExaminer_Id" });
            DropIndex("dbo.ExaminationDescription", new[] { "FirstExminer_Id" });
            DropIndex("dbo.ExaminationDescription", new[] { "ExaminationOption_Id" });
            DropIndex("dbo.ExaminationDescription", new[] { "ChangeLog_Id" });
            DropIndex("dbo.ExaminationDescription", new[] { "Accreditation_Id" });
            DropIndex("dbo.CatalogResponsibility", new[] { "Member_Id" });
            DropIndex("dbo.CatalogResponsibility", new[] { "Catalog_Id" });
            DropIndex("dbo.CurriculumSlot", new[] { "AreaOption_Id" });
            DropIndex("dbo.AreaOption", new[] { "Area_Id" });
            DropIndex("dbo.CurriculumArea", new[] { "Curriculum_Id" });
            DropPrimaryKey("dbo.ItemLabelItemLabelSet");
            DropColumn("dbo.CurriculumSlot", "AreaOption_Id");
            DropColumn("dbo.CurriculumSlot", "Semester");
            DropTable("dbo.TeachingDescription");
            DropTable("dbo.ExaminationDescription");
            DropTable("dbo.CatalogResponsibility");
            DropTable("dbo.AreaOption");
            DropTable("dbo.CurriculumArea");
            AddPrimaryKey("dbo.ItemLabelItemLabelSet", new[] { "ItemLabelSet_Id", "ItemLabel_Id" });
            RenameTable(name: "dbo.ItemLabelItemLabelSet", newName: "ItemLabelSetItemLabel");
        }
    }
}
