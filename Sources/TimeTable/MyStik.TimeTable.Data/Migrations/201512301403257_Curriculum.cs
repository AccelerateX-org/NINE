namespace MyStik.TimeTable.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Curriculum : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.SemesterGroupOccurrenceGroup", newName: "OccurrenceGroupSemesterGroup");
            DropForeignKey("dbo.CurriculumGroup", "Course_Id", "dbo.Activity");
            DropForeignKey("dbo.Activity", "CurriculumModule_Id", "dbo.CurriculumModule");
            DropForeignKey("dbo.GroupTemplate", "Alias_Id", "dbo.GroupAlias");
            DropIndex("dbo.Activity", new[] { "CurriculumModule_Id" });
            DropIndex("dbo.CurriculumGroup", new[] { "Course_Id" });
            DropIndex("dbo.GroupTemplate", new[] { "Alias_Id" });
            DropPrimaryKey("dbo.OccurrenceGroupSemesterGroup");
            CreateTable(
                "dbo.CapacityGroup",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Name = c.String(),
                        InWS = c.Boolean(nullable: false),
                        InSS = c.Boolean(nullable: false),
                        CurriculumGroup_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.CurriculumGroup", t => t.CurriculumGroup_Id)
                .Index(t => t.CurriculumGroup_Id);
            
            CreateTable(
                "dbo.Infoscreen",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.InfoAnnouncement",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        ImageURL = c.String(),
                        IsActive = c.Boolean(nullable: false),
                        ShowFrom = c.DateTime(),
                        ShowUntil = c.DateTime(),
                        CreatedAt = c.DateTime(nullable: false),
                        Activity_Id = c.Guid(),
                        Date_Id = c.Guid(),
                        Image_Id = c.Guid(),
                        Infoscreen_Id = c.Guid(),
                        Member_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Activity", t => t.Activity_Id)
                .ForeignKey("dbo.ActivityDate", t => t.Date_Id)
                .ForeignKey("dbo.BinaryStorage", t => t.Image_Id)
                .ForeignKey("dbo.Infoscreen", t => t.Infoscreen_Id)
                .ForeignKey("dbo.OrganiserMember", t => t.Member_Id)
                .Index(t => t.Activity_Id)
                .Index(t => t.Date_Id)
                .Index(t => t.Image_Id)
                .Index(t => t.Infoscreen_Id)
                .Index(t => t.Member_Id);
            
            CreateTable(
                "dbo.InfoText",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Text = c.String(),
                        IsActive = c.Boolean(nullable: false),
                        ShowFrom = c.DateTime(),
                        ShowUntil = c.DateTime(),
                        CreatedAt = c.DateTime(nullable: false),
                        Infoscreen_Id = c.Guid(),
                        Member_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Infoscreen", t => t.Infoscreen_Id)
                .ForeignKey("dbo.OrganiserMember", t => t.Member_Id)
                .Index(t => t.Infoscreen_Id)
                .Index(t => t.Member_Id);
            
            CreateTable(
                "dbo.ModuleCourse",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Name = c.String(),
                        CourseType = c.Int(nullable: false),
                        SWS = c.Int(nullable: false),
                        ExternalId = c.String(),
                        Module_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.CurriculumModule", t => t.Module_Id)
                .Index(t => t.Module_Id);
            
            CreateTable(
                "dbo.ModuleExam",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        ExamType = c.Int(nullable: false),
                        Weight = c.Double(nullable: false),
                        ExternalId = c.String(),
                        Module_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.CurriculumModule", t => t.Module_Id)
                .Index(t => t.Module_Id);
            
            CreateTable(
                "dbo.InfoscreenActivityOrganiser",
                c => new
                    {
                        Infoscreen_Id = c.Guid(nullable: false),
                        ActivityOrganiser_Id = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.Infoscreen_Id, t.ActivityOrganiser_Id })
                .ForeignKey("dbo.Infoscreen", t => t.Infoscreen_Id, cascadeDelete: true)
                .ForeignKey("dbo.ActivityOrganiser", t => t.ActivityOrganiser_Id, cascadeDelete: true)
                .Index(t => t.Infoscreen_Id)
                .Index(t => t.ActivityOrganiser_Id);
            
            CreateTable(
                "dbo.CourseModuleCourse",
                c => new
                    {
                        Course_Id = c.Guid(nullable: false),
                        ModuleCourse_Id = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.Course_Id, t.ModuleCourse_Id })
                .ForeignKey("dbo.Activity", t => t.Course_Id, cascadeDelete: true)
                .ForeignKey("dbo.ModuleCourse", t => t.ModuleCourse_Id, cascadeDelete: true)
                .Index(t => t.Course_Id)
                .Index(t => t.ModuleCourse_Id);
            
            AddColumn("dbo.CurriculumGroup", "IsSubscribable", c => c.Boolean(nullable: false));
            AddColumn("dbo.CurriculumModule", "ModuleId", c => c.String());
            AddColumn("dbo.CurriculumModule", "ECTS", c => c.Int(nullable: false));
            AddColumn("dbo.CurriculumModule", "Description", c => c.String());
            AddColumn("dbo.CurriculumModule", "MV_Id", c => c.Guid());
            AddColumn("dbo.SemesterGroup", "CapacityGroup_Id", c => c.Guid());
            AddColumn("dbo.GroupAlias", "Remark", c => c.String());
            AddColumn("dbo.GroupAlias", "CapacityGroup_Id", c => c.Guid());
            AddPrimaryKey("dbo.OccurrenceGroupSemesterGroup", new[] { "OccurrenceGroup_Id", "SemesterGroup_Id" });
            CreateIndex("dbo.CurriculumModule", "MV_Id");
            CreateIndex("dbo.GroupAlias", "CapacityGroup_Id");
            CreateIndex("dbo.SemesterGroup", "CapacityGroup_Id");
            AddForeignKey("dbo.GroupAlias", "CapacityGroup_Id", "dbo.CapacityGroup", "Id");
            AddForeignKey("dbo.SemesterGroup", "CapacityGroup_Id", "dbo.CapacityGroup", "Id");
            AddForeignKey("dbo.CurriculumModule", "MV_Id", "dbo.OrganiserMember", "Id");
            DropColumn("dbo.Activity", "CurriculumModule_Id");
            DropColumn("dbo.CurriculumGroup", "Course_Id");
            DropTable("dbo.GroupTemplate");


            Sql("UPDATE dbo.CurriculumGroup SET IsSubscribable  = 'True'");
            Sql("UPDATE dbo.CurriculumModule SET ECTS = 0");

        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.GroupTemplate",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        CurriculumGroupName = c.String(),
                        SemesterGroupName = c.String(),
                        Alias_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.CurriculumGroup", "Course_Id", c => c.Guid());
            AddColumn("dbo.Activity", "CurriculumModule_Id", c => c.Guid());
            DropForeignKey("dbo.CurriculumModule", "MV_Id", "dbo.OrganiserMember");
            DropForeignKey("dbo.ModuleExam", "Module_Id", "dbo.CurriculumModule");
            DropForeignKey("dbo.ModuleCourse", "Module_Id", "dbo.CurriculumModule");
            DropForeignKey("dbo.CourseModuleCourse", "ModuleCourse_Id", "dbo.ModuleCourse");
            DropForeignKey("dbo.CourseModuleCourse", "Course_Id", "dbo.Activity");
            DropForeignKey("dbo.InfoscreenActivityOrganiser", "ActivityOrganiser_Id", "dbo.ActivityOrganiser");
            DropForeignKey("dbo.InfoscreenActivityOrganiser", "Infoscreen_Id", "dbo.Infoscreen");
            DropForeignKey("dbo.InfoText", "Member_Id", "dbo.OrganiserMember");
            DropForeignKey("dbo.InfoText", "Infoscreen_Id", "dbo.Infoscreen");
            DropForeignKey("dbo.InfoAnnouncement", "Member_Id", "dbo.OrganiserMember");
            DropForeignKey("dbo.InfoAnnouncement", "Infoscreen_Id", "dbo.Infoscreen");
            DropForeignKey("dbo.InfoAnnouncement", "Image_Id", "dbo.BinaryStorage");
            DropForeignKey("dbo.InfoAnnouncement", "Date_Id", "dbo.ActivityDate");
            DropForeignKey("dbo.InfoAnnouncement", "Activity_Id", "dbo.Activity");
            DropForeignKey("dbo.SemesterGroup", "CapacityGroup_Id", "dbo.CapacityGroup");
            DropForeignKey("dbo.CapacityGroup", "CurriculumGroup_Id", "dbo.CurriculumGroup");
            DropForeignKey("dbo.GroupAlias", "CapacityGroup_Id", "dbo.CapacityGroup");
            DropIndex("dbo.CourseModuleCourse", new[] { "ModuleCourse_Id" });
            DropIndex("dbo.CourseModuleCourse", new[] { "Course_Id" });
            DropIndex("dbo.InfoscreenActivityOrganiser", new[] { "ActivityOrganiser_Id" });
            DropIndex("dbo.InfoscreenActivityOrganiser", new[] { "Infoscreen_Id" });
            DropIndex("dbo.ModuleExam", new[] { "Module_Id" });
            DropIndex("dbo.ModuleCourse", new[] { "Module_Id" });
            DropIndex("dbo.InfoText", new[] { "Member_Id" });
            DropIndex("dbo.InfoText", new[] { "Infoscreen_Id" });
            DropIndex("dbo.InfoAnnouncement", new[] { "Member_Id" });
            DropIndex("dbo.InfoAnnouncement", new[] { "Infoscreen_Id" });
            DropIndex("dbo.InfoAnnouncement", new[] { "Image_Id" });
            DropIndex("dbo.InfoAnnouncement", new[] { "Date_Id" });
            DropIndex("dbo.InfoAnnouncement", new[] { "Activity_Id" });
            DropIndex("dbo.SemesterGroup", new[] { "CapacityGroup_Id" });
            DropIndex("dbo.GroupAlias", new[] { "CapacityGroup_Id" });
            DropIndex("dbo.CapacityGroup", new[] { "CurriculumGroup_Id" });
            DropIndex("dbo.CurriculumModule", new[] { "MV_Id" });
            DropPrimaryKey("dbo.OccurrenceGroupSemesterGroup");
            DropColumn("dbo.GroupAlias", "CapacityGroup_Id");
            DropColumn("dbo.GroupAlias", "Remark");
            DropColumn("dbo.SemesterGroup", "CapacityGroup_Id");
            DropColumn("dbo.CurriculumModule", "MV_Id");
            DropColumn("dbo.CurriculumModule", "Description");
            DropColumn("dbo.CurriculumModule", "ECTS");
            DropColumn("dbo.CurriculumModule", "ModuleId");
            DropColumn("dbo.CurriculumGroup", "IsSubscribable");
            DropTable("dbo.CourseModuleCourse");
            DropTable("dbo.InfoscreenActivityOrganiser");
            DropTable("dbo.ModuleExam");
            DropTable("dbo.ModuleCourse");
            DropTable("dbo.InfoText");
            DropTable("dbo.InfoAnnouncement");
            DropTable("dbo.Infoscreen");
            DropTable("dbo.CapacityGroup");
            AddPrimaryKey("dbo.OccurrenceGroupSemesterGroup", new[] { "SemesterGroup_Id", "OccurrenceGroup_Id" });
            CreateIndex("dbo.GroupTemplate", "Alias_Id");
            CreateIndex("dbo.CurriculumGroup", "Course_Id");
            CreateIndex("dbo.Activity", "CurriculumModule_Id");
            AddForeignKey("dbo.GroupTemplate", "Alias_Id", "dbo.GroupAlias", "Id");
            AddForeignKey("dbo.Activity", "CurriculumModule_Id", "dbo.CurriculumModule", "Id");
            AddForeignKey("dbo.CurriculumGroup", "Course_Id", "dbo.Activity", "Id");
            RenameTable(name: "dbo.OccurrenceGroupSemesterGroup", newName: "SemesterGroupOccurrenceGroup");
        }
    }
}
