namespace MyStik.TimeTable.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModuleDescV3 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ModuleApplicability",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        FitRate = c.Int(nullable: false),
                        Description = c.String(),
                        ProvidingModule_Id = c.Guid(),
                        ReceivingModule_Id = c.Guid(),
                        CurriculumModule_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.CurriculumModule", t => t.ProvidingModule_Id)
                .ForeignKey("dbo.CurriculumModule", t => t.ReceivingModule_Id)
                .ForeignKey("dbo.CurriculumModule", t => t.CurriculumModule_Id)
                .Index(t => t.ProvidingModule_Id)
                .Index(t => t.ReceivingModule_Id)
                .Index(t => t.CurriculumModule_Id);
            
            CreateTable(
                "dbo.Examiner",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        IsFirstExaminer = c.Boolean(),
                        IsSecondExaminer = c.Boolean(),
                        IsStaff = c.Boolean(),
                        Description = c.String(),
                        Examination_Id = c.Guid(),
                        Member_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ExaminationDescription", t => t.Examination_Id)
                .ForeignKey("dbo.OrganiserMember", t => t.Member_Id)
                .Index(t => t.Examination_Id)
                .Index(t => t.Member_Id);
            
            AddColumn("dbo.CurriculumModule", "Prerequisites", c => c.String());
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Examiner", "Member_Id", "dbo.OrganiserMember");
            DropForeignKey("dbo.Examiner", "Examination_Id", "dbo.ExaminationDescription");
            DropForeignKey("dbo.ModuleApplicability", "CurriculumModule_Id", "dbo.CurriculumModule");
            DropForeignKey("dbo.ModuleApplicability", "ReceivingModule_Id", "dbo.CurriculumModule");
            DropForeignKey("dbo.ModuleApplicability", "ProvidingModule_Id", "dbo.CurriculumModule");
            DropIndex("dbo.Examiner", new[] { "Member_Id" });
            DropIndex("dbo.Examiner", new[] { "Examination_Id" });
            DropIndex("dbo.ModuleApplicability", new[] { "CurriculumModule_Id" });
            DropIndex("dbo.ModuleApplicability", new[] { "ReceivingModule_Id" });
            DropIndex("dbo.ModuleApplicability", new[] { "ProvidingModule_Id" });
            DropColumn("dbo.CurriculumModule", "Prerequisites");
            DropTable("dbo.Examiner");
            DropTable("dbo.ModuleApplicability");
        }
    }
}
