namespace MyStik.TimeTable.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Contact_v1 : DbMigration
    {
        public override void Up()
        {
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
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ModuleExam", t => t.Exam_Id)
                .ForeignKey("dbo.Student", t => t.Examinee_Id)
                .Index(t => t.Exam_Id)
                .Index(t => t.Examinee_Id);
            
            CreateTable(
                "dbo.Student",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Number = c.String(),
                        UserId = c.String(),
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
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.StudentExam", t => t.Exam_Id)
                .ForeignKey("dbo.OrganiserMember", t => t.Member_Id)
                .Index(t => t.Exam_Id)
                .Index(t => t.Member_Id);
            
            CreateTable(
                "dbo.ExamPaper",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Document_Id = c.Guid(),
                        Exam_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.BinaryStorage", t => t.Document_Id)
                .ForeignKey("dbo.StudentExam", t => t.Exam_Id)
                .Index(t => t.Document_Id)
                .Index(t => t.Exam_Id);
            
            CreateTable(
                "dbo.AdvertisementRole",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        RoleName = c.String(),
                        Advertisement_Id = c.Guid(),
                        Contact_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Advertisement", t => t.Advertisement_Id)
                .ForeignKey("dbo.PersonalContact", t => t.Contact_Id)
                .Index(t => t.Advertisement_Id)
                .Index(t => t.Contact_Id);
            
            CreateTable(
                "dbo.Advertisement",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Type = c.Int(nullable: false),
                        Title = c.String(),
                        Description = c.String(),
                        Created = c.DateTime(nullable: false),
                        VisibleUntil = c.DateTime(nullable: false),
                        Attachment_Id = c.Guid(),
                        Owner_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.BinaryStorage", t => t.Attachment_Id)
                .ForeignKey("dbo.OrganiserMember", t => t.Owner_Id)
                .Index(t => t.Attachment_Id)
                .Index(t => t.Owner_Id);
            
            CreateTable(
                "dbo.PersonalContact",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        UserId = c.String(),
                        Department = c.String(),
                        Created = c.DateTime(nullable: false),
                        Corporate_Id = c.Guid(),
                        Owner_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.CorporateContact", t => t.Corporate_Id)
                .ForeignKey("dbo.OrganiserMember", t => t.Owner_Id)
                .Index(t => t.Corporate_Id)
                .Index(t => t.Owner_Id);
            
            CreateTable(
                "dbo.CorporateContact",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Name = c.String(),
                        Url = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.OrganiserMember", "ModuleExam_Id", c => c.Guid());
            AddColumn("dbo.ActivityOrganiser", "ParentOrganiser_Id", c => c.Guid());
            AddColumn("dbo.ModuleExam", "Duration", c => c.Time(precision: 7));
            CreateIndex("dbo.OrganiserMember", "ModuleExam_Id");
            CreateIndex("dbo.ActivityOrganiser", "ParentOrganiser_Id");
            AddForeignKey("dbo.ActivityOrganiser", "ParentOrganiser_Id", "dbo.ActivityOrganiser", "Id");
            AddForeignKey("dbo.OrganiserMember", "ModuleExam_Id", "dbo.ModuleExam", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AdvertisementRole", "Contact_Id", "dbo.PersonalContact");
            DropForeignKey("dbo.PersonalContact", "Owner_Id", "dbo.OrganiserMember");
            DropForeignKey("dbo.PersonalContact", "Corporate_Id", "dbo.CorporateContact");
            DropForeignKey("dbo.AdvertisementRole", "Advertisement_Id", "dbo.Advertisement");
            DropForeignKey("dbo.Advertisement", "Owner_Id", "dbo.OrganiserMember");
            DropForeignKey("dbo.Advertisement", "Attachment_Id", "dbo.BinaryStorage");
            DropForeignKey("dbo.ExamPaper", "Exam_Id", "dbo.StudentExam");
            DropForeignKey("dbo.ExamPaper", "Document_Id", "dbo.BinaryStorage");
            DropForeignKey("dbo.Examiner", "Member_Id", "dbo.OrganiserMember");
            DropForeignKey("dbo.Examiner", "Exam_Id", "dbo.StudentExam");
            DropForeignKey("dbo.StudentExam", "Examinee_Id", "dbo.Student");
            DropForeignKey("dbo.StudentExam", "Exam_Id", "dbo.ModuleExam");
            DropForeignKey("dbo.OrganiserMember", "ModuleExam_Id", "dbo.ModuleExam");
            DropForeignKey("dbo.ActivityOrganiser", "ParentOrganiser_Id", "dbo.ActivityOrganiser");
            DropIndex("dbo.PersonalContact", new[] { "Owner_Id" });
            DropIndex("dbo.PersonalContact", new[] { "Corporate_Id" });
            DropIndex("dbo.Advertisement", new[] { "Owner_Id" });
            DropIndex("dbo.Advertisement", new[] { "Attachment_Id" });
            DropIndex("dbo.AdvertisementRole", new[] { "Contact_Id" });
            DropIndex("dbo.AdvertisementRole", new[] { "Advertisement_Id" });
            DropIndex("dbo.ExamPaper", new[] { "Exam_Id" });
            DropIndex("dbo.ExamPaper", new[] { "Document_Id" });
            DropIndex("dbo.Examiner", new[] { "Member_Id" });
            DropIndex("dbo.Examiner", new[] { "Exam_Id" });
            DropIndex("dbo.StudentExam", new[] { "Examinee_Id" });
            DropIndex("dbo.StudentExam", new[] { "Exam_Id" });
            DropIndex("dbo.ActivityOrganiser", new[] { "ParentOrganiser_Id" });
            DropIndex("dbo.OrganiserMember", new[] { "ModuleExam_Id" });
            DropColumn("dbo.ModuleExam", "Duration");
            DropColumn("dbo.ActivityOrganiser", "ParentOrganiser_Id");
            DropColumn("dbo.OrganiserMember", "ModuleExam_Id");
            DropTable("dbo.CorporateContact");
            DropTable("dbo.PersonalContact");
            DropTable("dbo.Advertisement");
            DropTable("dbo.AdvertisementRole");
            DropTable("dbo.ExamPaper");
            DropTable("dbo.Examiner");
            DropTable("dbo.Student");
            DropTable("dbo.StudentExam");
        }
    }
}
