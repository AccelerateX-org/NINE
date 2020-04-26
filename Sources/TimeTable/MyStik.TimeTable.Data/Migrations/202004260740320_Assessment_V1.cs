namespace MyStik.TimeTable.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Assessment_V1 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Assessment",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Name = c.String(),
                        Description = c.String(),
                        IsAvailable = c.Boolean(nullable: false),
                        Committee_Id = c.Guid(),
                        Curriculum_Id = c.Guid(),
                        Semester_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Committee", t => t.Committee_Id)
                .ForeignKey("dbo.Curriculum", t => t.Curriculum_Id)
                .ForeignKey("dbo.Semester", t => t.Semester_Id)
                .Index(t => t.Committee_Id)
                .Index(t => t.Curriculum_Id)
                .Index(t => t.Semester_Id);
            
            CreateTable(
                "dbo.Candidature",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        UserId = c.String(),
                        Joined = c.DateTime(nullable: false),
                        Characteristics = c.String(),
                        Motivation = c.String(),
                        IsAccepted = c.Boolean(),
                        Feedback = c.String(),
                        Assessment_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Assessment", t => t.Assessment_Id)
                .Index(t => t.Assessment_Id);
            
            CreateTable(
                "dbo.CandidatureStage",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        IsAccepted = c.Boolean(),
                        Candidature_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Candidature", t => t.Candidature_Id)
                .Index(t => t.Candidature_Id);
            
            CreateTable(
                "dbo.CandidatureStageMaterial",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Stage_Id = c.Guid(),
                        Storage_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.CandidatureStage", t => t.Stage_Id)
                .ForeignKey("dbo.BinaryStorage", t => t.Storage_Id)
                .Index(t => t.Stage_Id)
                .Index(t => t.Storage_Id);
            
            CreateTable(
                "dbo.Committee",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Name = c.String(),
                        Description = c.String(),
                        Curriculum_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Curriculum", t => t.Curriculum_Id)
                .Index(t => t.Curriculum_Id);
            
            CreateTable(
                "dbo.CommitteeMember",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        From = c.DateTime(),
                        Until = c.DateTime(),
                        HasChair = c.Boolean(nullable: false),
                        Member_Id = c.Guid(),
                        Student_Id = c.Guid(),
                        Committee_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.OrganiserMember", t => t.Member_Id)
                .ForeignKey("dbo.Student", t => t.Student_Id)
                .ForeignKey("dbo.Committee", t => t.Committee_Id)
                .Index(t => t.Member_Id)
                .Index(t => t.Student_Id)
                .Index(t => t.Committee_Id);
            
            CreateTable(
                "dbo.AssessmentStage",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Name = c.String(),
                        Description = c.String(),
                        IsAvailable = c.Boolean(nullable: false),
                        OpeningDateTime = c.DateTime(),
                        ClosingDateTime = c.DateTime(),
                        ReportingDateTime = c.DateTime(),
                        Assessment_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Assessment", t => t.Assessment_Id)
                .Index(t => t.Assessment_Id);
            
            CreateTable(
                "dbo.AssessmentStageMaterial",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Stage_Id = c.Guid(),
                        Storage_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AssessmentStage", t => t.Stage_Id)
                .ForeignKey("dbo.BinaryStorage", t => t.Storage_Id)
                .Index(t => t.Stage_Id)
                .Index(t => t.Storage_Id);
            
            AddColumn("dbo.BinaryStorage", "Created", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AssessmentStageMaterial", "Storage_Id", "dbo.BinaryStorage");
            DropForeignKey("dbo.AssessmentStageMaterial", "Stage_Id", "dbo.AssessmentStage");
            DropForeignKey("dbo.AssessmentStage", "Assessment_Id", "dbo.Assessment");
            DropForeignKey("dbo.Assessment", "Semester_Id", "dbo.Semester");
            DropForeignKey("dbo.Assessment", "Curriculum_Id", "dbo.Curriculum");
            DropForeignKey("dbo.Assessment", "Committee_Id", "dbo.Committee");
            DropForeignKey("dbo.CommitteeMember", "Committee_Id", "dbo.Committee");
            DropForeignKey("dbo.CommitteeMember", "Student_Id", "dbo.Student");
            DropForeignKey("dbo.CommitteeMember", "Member_Id", "dbo.OrganiserMember");
            DropForeignKey("dbo.Committee", "Curriculum_Id", "dbo.Curriculum");
            DropForeignKey("dbo.CandidatureStageMaterial", "Storage_Id", "dbo.BinaryStorage");
            DropForeignKey("dbo.CandidatureStageMaterial", "Stage_Id", "dbo.CandidatureStage");
            DropForeignKey("dbo.CandidatureStage", "Candidature_Id", "dbo.Candidature");
            DropForeignKey("dbo.Candidature", "Assessment_Id", "dbo.Assessment");
            DropIndex("dbo.AssessmentStageMaterial", new[] { "Storage_Id" });
            DropIndex("dbo.AssessmentStageMaterial", new[] { "Stage_Id" });
            DropIndex("dbo.AssessmentStage", new[] { "Assessment_Id" });
            DropIndex("dbo.CommitteeMember", new[] { "Committee_Id" });
            DropIndex("dbo.CommitteeMember", new[] { "Student_Id" });
            DropIndex("dbo.CommitteeMember", new[] { "Member_Id" });
            DropIndex("dbo.Committee", new[] { "Curriculum_Id" });
            DropIndex("dbo.CandidatureStageMaterial", new[] { "Storage_Id" });
            DropIndex("dbo.CandidatureStageMaterial", new[] { "Stage_Id" });
            DropIndex("dbo.CandidatureStage", new[] { "Candidature_Id" });
            DropIndex("dbo.Candidature", new[] { "Assessment_Id" });
            DropIndex("dbo.Assessment", new[] { "Semester_Id" });
            DropIndex("dbo.Assessment", new[] { "Curriculum_Id" });
            DropIndex("dbo.Assessment", new[] { "Committee_Id" });
            DropColumn("dbo.BinaryStorage", "Created");
            DropTable("dbo.AssessmentStageMaterial");
            DropTable("dbo.AssessmentStage");
            DropTable("dbo.CommitteeMember");
            DropTable("dbo.Committee");
            DropTable("dbo.CandidatureStageMaterial");
            DropTable("dbo.CandidatureStage");
            DropTable("dbo.Candidature");
            DropTable("dbo.Assessment");
        }
    }
}
