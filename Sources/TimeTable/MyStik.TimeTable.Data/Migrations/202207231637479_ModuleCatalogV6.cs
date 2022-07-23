namespace MyStik.TimeTable.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModuleCatalogV6 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.OrganiserMember", "ModuleExam_Id", "dbo.ModuleExam");
            DropForeignKey("dbo.ExamModuleExam", "Exam_Id", "dbo.Activity");
            DropForeignKey("dbo.ExamModuleExam", "ModuleExam_Id", "dbo.ModuleExam");
            DropForeignKey("dbo.StudentExam", "Exam_Id", "dbo.Activity");
            DropForeignKey("dbo.StudentExam", "Examinee_Id", "dbo.Student");
            DropForeignKey("dbo.Examiner", "Exam_Id", "dbo.StudentExam");
            DropForeignKey("dbo.Examiner", "Member_Id", "dbo.OrganiserMember");
            DropForeignKey("dbo.ExamPaper", "Document_Id", "dbo.BinaryStorage");
            DropForeignKey("dbo.ExamPaper", "Exam_Id", "dbo.StudentExam");
            DropForeignKey("dbo.ModuleExam", "Module_Id", "dbo.CurriculumModule");
            DropIndex("dbo.OrganiserMember", new[] { "ModuleExam_Id" });
            DropIndex("dbo.ModuleExam", new[] { "Module_Id" });
            DropIndex("dbo.StudentExam", new[] { "Exam_Id" });
            DropIndex("dbo.StudentExam", new[] { "Examinee_Id" });
            DropIndex("dbo.Examiner", new[] { "Exam_Id" });
            DropIndex("dbo.Examiner", new[] { "Member_Id" });
            DropIndex("dbo.ExamPaper", new[] { "Document_Id" });
            DropIndex("dbo.ExamPaper", new[] { "Exam_Id" });
            DropIndex("dbo.ExamModuleExam", new[] { "Exam_Id" });
            DropIndex("dbo.ExamModuleExam", new[] { "ModuleExam_Id" });
            CreateTable(
                "dbo.ExaminationOption",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Name = c.String(),
                        Module_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.CurriculumModule", t => t.Module_Id)
                .Index(t => t.Module_Id);
            
            CreateTable(
                "dbo.ExaminationFraction",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        MinDuration = c.Int(),
                        MaxDuration = c.Int(),
                        Weight = c.Double(nullable: false),
                        ExaminationOption_Id = c.Guid(),
                        Form_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ExaminationOption", t => t.ExaminationOption_Id)
                .ForeignKey("dbo.ExaminationForm", t => t.Form_Id)
                .Index(t => t.ExaminationOption_Id)
                .Index(t => t.Form_Id);
            
            AddColumn("dbo.ExaminationUnit", "ModuleDescription_Id", c => c.Guid());
            CreateIndex("dbo.ExaminationUnit", "ModuleDescription_Id");
            AddForeignKey("dbo.ExaminationUnit", "ModuleDescription_Id", "dbo.ModuleDescription", "Id");
            DropColumn("dbo.OrganiserMember", "ModuleExam_Id");
            DropTable("dbo.ModuleExam");
            DropTable("dbo.StudentExam");
            DropTable("dbo.Examiner");
            DropTable("dbo.ExamPaper");
            DropTable("dbo.ExamModuleExam");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.ExamModuleExam",
                c => new
                    {
                        Exam_Id = c.Guid(nullable: false),
                        ModuleExam_Id = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.Exam_Id, t.ModuleExam_Id });
            
            CreateTable(
                "dbo.ExamPaper",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Title = c.String(),
                        Description = c.String(),
                        Version = c.String(),
                        CreateDate = c.DateTime(nullable: false),
                        LastChangeDate = c.DateTime(),
                        LastChangeUserId = c.String(),
                        ReleaseDate = c.DateTime(),
                        Document_Id = c.Guid(),
                        Exam_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Examiner",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Role = c.String(),
                        Exam_Id = c.Guid(),
                        Member_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.StudentExam",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        State = c.Int(nullable: false),
                        Registered = c.DateTime(nullable: false),
                        Start = c.DateTime(),
                        End = c.DateTime(),
                        Exam_Id = c.Guid(),
                        Examinee_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ModuleExam",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        ExamType = c.Int(nullable: false),
                        Weight = c.Double(nullable: false),
                        ExternalId = c.String(),
                        Duration = c.Time(precision: 7),
                        Module_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.OrganiserMember", "ModuleExam_Id", c => c.Guid());
            DropForeignKey("dbo.ExaminationOption", "Module_Id", "dbo.CurriculumModule");
            DropForeignKey("dbo.ExaminationFraction", "Form_Id", "dbo.ExaminationForm");
            DropForeignKey("dbo.ExaminationFraction", "ExaminationOption_Id", "dbo.ExaminationOption");
            DropForeignKey("dbo.ExaminationUnit", "ModuleDescription_Id", "dbo.ModuleDescription");
            DropIndex("dbo.ExaminationFraction", new[] { "Form_Id" });
            DropIndex("dbo.ExaminationFraction", new[] { "ExaminationOption_Id" });
            DropIndex("dbo.ExaminationOption", new[] { "Module_Id" });
            DropIndex("dbo.ExaminationUnit", new[] { "ModuleDescription_Id" });
            DropColumn("dbo.ExaminationUnit", "ModuleDescription_Id");
            DropTable("dbo.ExaminationFraction");
            DropTable("dbo.ExaminationOption");
            CreateIndex("dbo.ExamModuleExam", "ModuleExam_Id");
            CreateIndex("dbo.ExamModuleExam", "Exam_Id");
            CreateIndex("dbo.ExamPaper", "Exam_Id");
            CreateIndex("dbo.ExamPaper", "Document_Id");
            CreateIndex("dbo.Examiner", "Member_Id");
            CreateIndex("dbo.Examiner", "Exam_Id");
            CreateIndex("dbo.StudentExam", "Examinee_Id");
            CreateIndex("dbo.StudentExam", "Exam_Id");
            CreateIndex("dbo.ModuleExam", "Module_Id");
            CreateIndex("dbo.OrganiserMember", "ModuleExam_Id");
            AddForeignKey("dbo.ModuleExam", "Module_Id", "dbo.CurriculumModule", "Id");
            AddForeignKey("dbo.ExamPaper", "Exam_Id", "dbo.StudentExam", "Id");
            AddForeignKey("dbo.ExamPaper", "Document_Id", "dbo.BinaryStorage", "Id");
            AddForeignKey("dbo.Examiner", "Member_Id", "dbo.OrganiserMember", "Id");
            AddForeignKey("dbo.Examiner", "Exam_Id", "dbo.StudentExam", "Id");
            AddForeignKey("dbo.StudentExam", "Examinee_Id", "dbo.Student", "Id");
            AddForeignKey("dbo.StudentExam", "Exam_Id", "dbo.Activity", "Id");
            AddForeignKey("dbo.ExamModuleExam", "ModuleExam_Id", "dbo.ModuleExam", "Id", cascadeDelete: true);
            AddForeignKey("dbo.ExamModuleExam", "Exam_Id", "dbo.Activity", "Id", cascadeDelete: true);
            AddForeignKey("dbo.OrganiserMember", "ModuleExam_Id", "dbo.ModuleExam", "Id");
        }
    }
}
