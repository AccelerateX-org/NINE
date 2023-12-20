namespace MyStik.TimeTable.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RoomManagementV02 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.InfoAnnouncement", "Activity_Id", "dbo.Activity");
            DropForeignKey("dbo.InfoAnnouncement", "Date_Id", "dbo.ActivityDate");
            DropForeignKey("dbo.InfoAnnouncement", "Image_Id", "dbo.BinaryStorage");
            DropForeignKey("dbo.InfoAnnouncement", "Infoscreen_Id", "dbo.Infoscreen");
            DropForeignKey("dbo.InfoAnnouncement", "Member_Id", "dbo.OrganiserMember");
            DropForeignKey("dbo.InfoText", "Infoscreen_Id", "dbo.Infoscreen");
            DropForeignKey("dbo.InfoText", "Member_Id", "dbo.OrganiserMember");
            DropIndex("dbo.InfoAnnouncement", new[] { "Activity_Id" });
            DropIndex("dbo.InfoAnnouncement", new[] { "Date_Id" });
            DropIndex("dbo.InfoAnnouncement", new[] { "Image_Id" });
            DropIndex("dbo.InfoAnnouncement", new[] { "Infoscreen_Id" });
            DropIndex("dbo.InfoAnnouncement", new[] { "Member_Id" });
            DropIndex("dbo.InfoText", new[] { "Infoscreen_Id" });
            DropIndex("dbo.InfoText", new[] { "Member_Id" });
            CreateTable(
                "dbo.InfoscreenPage",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Name = c.String(),
                        Type = c.Int(nullable: false),
                        Image_Id = c.Guid(),
                        Infoscreen_Id = c.Guid(),
                        RoomAllocation_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.BinaryStorage", t => t.Image_Id)
                .ForeignKey("dbo.Infoscreen", t => t.Infoscreen_Id)
                .ForeignKey("dbo.RoomAllocation", t => t.RoomAllocation_Id)
                .Index(t => t.Image_Id)
                .Index(t => t.Infoscreen_Id)
                .Index(t => t.RoomAllocation_Id);
            
            CreateTable(
                "dbo.RoomAllocation",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Capacity = c.Int(nullable: false),
                        Group_Id = c.Guid(),
                        Room_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.RoomAllocationGroup", t => t.Group_Id)
                .ForeignKey("dbo.Room", t => t.Room_Id)
                .Index(t => t.Group_Id)
                .Index(t => t.Room_Id);
            
            CreateTable(
                "dbo.RoomAllocationGroup",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Name = c.String(),
                        Description = c.String(),
                        ShortName = c.String(),
                        Organiser_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ActivityOrganiser", t => t.Organiser_Id)
                .Index(t => t.Organiser_Id);
            
            AddColumn("dbo.OrganiserMember", "LectureRole", c => c.Int(nullable: false));
            AddColumn("dbo.Infoscreen", "Tag", c => c.String());
            DropTable("dbo.InfoAnnouncement");
            DropTable("dbo.InfoText");
        }
        
        public override void Down()
        {
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
                .PrimaryKey(t => t.Id);
            
            DropForeignKey("dbo.InfoscreenPage", "RoomAllocation_Id", "dbo.RoomAllocation");
            DropForeignKey("dbo.RoomAllocation", "Room_Id", "dbo.Room");
            DropForeignKey("dbo.RoomAllocation", "Group_Id", "dbo.RoomAllocationGroup");
            DropForeignKey("dbo.RoomAllocationGroup", "Organiser_Id", "dbo.ActivityOrganiser");
            DropForeignKey("dbo.InfoscreenPage", "Infoscreen_Id", "dbo.Infoscreen");
            DropForeignKey("dbo.InfoscreenPage", "Image_Id", "dbo.BinaryStorage");
            DropIndex("dbo.RoomAllocationGroup", new[] { "Organiser_Id" });
            DropIndex("dbo.RoomAllocation", new[] { "Room_Id" });
            DropIndex("dbo.RoomAllocation", new[] { "Group_Id" });
            DropIndex("dbo.InfoscreenPage", new[] { "RoomAllocation_Id" });
            DropIndex("dbo.InfoscreenPage", new[] { "Infoscreen_Id" });
            DropIndex("dbo.InfoscreenPage", new[] { "Image_Id" });
            DropColumn("dbo.Infoscreen", "Tag");
            DropColumn("dbo.OrganiserMember", "LectureRole");
            DropTable("dbo.RoomAllocationGroup");
            DropTable("dbo.RoomAllocation");
            DropTable("dbo.InfoscreenPage");
            CreateIndex("dbo.InfoText", "Member_Id");
            CreateIndex("dbo.InfoText", "Infoscreen_Id");
            CreateIndex("dbo.InfoAnnouncement", "Member_Id");
            CreateIndex("dbo.InfoAnnouncement", "Infoscreen_Id");
            CreateIndex("dbo.InfoAnnouncement", "Image_Id");
            CreateIndex("dbo.InfoAnnouncement", "Date_Id");
            CreateIndex("dbo.InfoAnnouncement", "Activity_Id");
            AddForeignKey("dbo.InfoText", "Member_Id", "dbo.OrganiserMember", "Id");
            AddForeignKey("dbo.InfoText", "Infoscreen_Id", "dbo.Infoscreen", "Id");
            AddForeignKey("dbo.InfoAnnouncement", "Member_Id", "dbo.OrganiserMember", "Id");
            AddForeignKey("dbo.InfoAnnouncement", "Infoscreen_Id", "dbo.Infoscreen", "Id");
            AddForeignKey("dbo.InfoAnnouncement", "Image_Id", "dbo.BinaryStorage", "Id");
            AddForeignKey("dbo.InfoAnnouncement", "Date_Id", "dbo.ActivityDate", "Id");
            AddForeignKey("dbo.InfoAnnouncement", "Activity_Id", "dbo.Activity", "Id");
        }
    }
}
