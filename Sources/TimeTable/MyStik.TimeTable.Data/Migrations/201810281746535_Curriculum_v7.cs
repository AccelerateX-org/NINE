namespace MyStik.TimeTable.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Curriculum_v7 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CertificateSubject",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Name = c.String(),
                        Term = c.Int(nullable: false),
                        Ects = c.Double(nullable: false),
                        USCredits = c.Double(nullable: false),
                        CertificateModule_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.CertificateModule", t => t.CertificateModule_Id)
                .Index(t => t.CertificateModule_Id);
            
            CreateTable(
                "dbo.CertificateModule",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Name = c.String(),
                        Weight = c.Double(nullable: false),
                        Curriculum_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Curriculum", t => t.Curriculum_Id)
                .Index(t => t.Curriculum_Id);
            
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
                "dbo.ExaminationUnit",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Duration = c.Int(),
                        Form_Id = c.Guid(),
                        Module_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ExaminationForm", t => t.Form_Id)
                .ForeignKey("dbo.TeachingBuildingBlock", t => t.Module_Id)
                .Index(t => t.Form_Id)
                .Index(t => t.Module_Id);
            
            CreateTable(
                "dbo.ExaminationForm",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Name = c.String(),
                        ShortName = c.String(),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Lecturer",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        IsAdmin = c.Boolean(nullable: false),
                        Member_Id = c.Guid(),
                        Module_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.OrganiserMember", t => t.Member_Id)
                .ForeignKey("dbo.TeachingBuildingBlock", t => t.Module_Id)
                .Index(t => t.Member_Id)
                .Index(t => t.Module_Id);
            
            CreateTable(
                "dbo.TeachingUnit",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Form_Id = c.Guid(),
                        Module_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.TeachingForm", t => t.Form_Id)
                .ForeignKey("dbo.TeachingBuildingBlock", t => t.Module_Id)
                .Index(t => t.Form_Id)
                .Index(t => t.Module_Id);
            
            CreateTable(
                "dbo.TeachingForm",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Name = c.String(),
                        ShortName = c.String(),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.ModuleAccreditation", "ShortName", c => c.String());
            AddColumn("dbo.ModuleAccreditation", "Number", c => c.String());
            AddColumn("dbo.ModuleAccreditation", "CertificateSubject_Id", c => c.Guid());
            AddColumn("dbo.ModuleAccreditation", "TeachingBuildingBlock_Id", c => c.Guid());
            CreateIndex("dbo.ModuleAccreditation", "CertificateSubject_Id");
            CreateIndex("dbo.ModuleAccreditation", "TeachingBuildingBlock_Id");
            AddForeignKey("dbo.ModuleAccreditation", "CertificateSubject_Id", "dbo.CertificateSubject", "Id");
            AddForeignKey("dbo.ModuleAccreditation", "TeachingBuildingBlock_Id", "dbo.TeachingBuildingBlock", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TeachingUnit", "Module_Id", "dbo.TeachingBuildingBlock");
            DropForeignKey("dbo.TeachingUnit", "Form_Id", "dbo.TeachingForm");
            DropForeignKey("dbo.Lecturer", "Module_Id", "dbo.TeachingBuildingBlock");
            DropForeignKey("dbo.Lecturer", "Member_Id", "dbo.OrganiserMember");
            DropForeignKey("dbo.ExaminationUnit", "Module_Id", "dbo.TeachingBuildingBlock");
            DropForeignKey("dbo.ExaminationUnit", "Form_Id", "dbo.ExaminationForm");
            DropForeignKey("dbo.ModuleAccreditation", "TeachingBuildingBlock_Id", "dbo.TeachingBuildingBlock");
            DropForeignKey("dbo.ModuleAccreditation", "CertificateSubject_Id", "dbo.CertificateSubject");
            DropForeignKey("dbo.CertificateSubject", "CertificateModule_Id", "dbo.CertificateModule");
            DropForeignKey("dbo.CertificateModule", "Curriculum_Id", "dbo.Curriculum");
            DropIndex("dbo.TeachingUnit", new[] { "Module_Id" });
            DropIndex("dbo.TeachingUnit", new[] { "Form_Id" });
            DropIndex("dbo.Lecturer", new[] { "Module_Id" });
            DropIndex("dbo.Lecturer", new[] { "Member_Id" });
            DropIndex("dbo.ExaminationUnit", new[] { "Module_Id" });
            DropIndex("dbo.ExaminationUnit", new[] { "Form_Id" });
            DropIndex("dbo.CertificateModule", new[] { "Curriculum_Id" });
            DropIndex("dbo.CertificateSubject", new[] { "CertificateModule_Id" });
            DropIndex("dbo.ModuleAccreditation", new[] { "TeachingBuildingBlock_Id" });
            DropIndex("dbo.ModuleAccreditation", new[] { "CertificateSubject_Id" });
            DropColumn("dbo.ModuleAccreditation", "TeachingBuildingBlock_Id");
            DropColumn("dbo.ModuleAccreditation", "CertificateSubject_Id");
            DropColumn("dbo.ModuleAccreditation", "Number");
            DropColumn("dbo.ModuleAccreditation", "ShortName");
            DropTable("dbo.TeachingForm");
            DropTable("dbo.TeachingUnit");
            DropTable("dbo.Lecturer");
            DropTable("dbo.ExaminationForm");
            DropTable("dbo.ExaminationUnit");
            DropTable("dbo.TeachingBuildingBlock");
            DropTable("dbo.CertificateModule");
            DropTable("dbo.CertificateSubject");
        }
    }
}
