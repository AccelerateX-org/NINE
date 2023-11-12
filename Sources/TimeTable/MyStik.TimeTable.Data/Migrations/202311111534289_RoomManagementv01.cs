namespace MyStik.TimeTable.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RoomManagementv01 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.RoomLayout",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Name = c.String(),
                        Room_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Room", t => t.Room_Id)
                .Index(t => t.Room_Id);
            
            CreateTable(
                "dbo.RoomDesk",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Name = c.String(),
                        Layout_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.RoomLayout", t => t.Layout_Id)
                .Index(t => t.Layout_Id);
            
            CreateTable(
                "dbo.RoomSeat",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Name = c.String(),
                        Layout_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.RoomDesk", t => t.Layout_Id)
                .Index(t => t.Layout_Id);
            
            CreateTable(
                "dbo.RoomSeatBooking",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Seat_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.RoomSeat", t => t.Seat_Id)
                .Index(t => t.Seat_Id);
            
            CreateTable(
                "dbo.ActivityPublication",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Activity_Id = c.Guid(),
                        PublishingChannel_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Activity", t => t.Activity_Id)
                .ForeignKey("dbo.PublishingChannel", t => t.PublishingChannel_Id)
                .Index(t => t.Activity_Id)
                .Index(t => t.PublishingChannel_Id);
            
            CreateTable(
                "dbo.PublishingChannel",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Semester = c.Int(nullable: false),
                        Curriculum_Id = c.Guid(),
                        LabelSet_Id = c.Guid(),
                        Organiser_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Curriculum", t => t.Curriculum_Id)
                .ForeignKey("dbo.ItemLabelSet", t => t.LabelSet_Id)
                .ForeignKey("dbo.ActivityOrganiser", t => t.Organiser_Id)
                .Index(t => t.Curriculum_Id)
                .Index(t => t.LabelSet_Id)
                .Index(t => t.Organiser_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ActivityPublication", "PublishingChannel_Id", "dbo.PublishingChannel");
            DropForeignKey("dbo.PublishingChannel", "Organiser_Id", "dbo.ActivityOrganiser");
            DropForeignKey("dbo.PublishingChannel", "LabelSet_Id", "dbo.ItemLabelSet");
            DropForeignKey("dbo.PublishingChannel", "Curriculum_Id", "dbo.Curriculum");
            DropForeignKey("dbo.ActivityPublication", "Activity_Id", "dbo.Activity");
            DropForeignKey("dbo.RoomLayout", "Room_Id", "dbo.Room");
            DropForeignKey("dbo.RoomSeat", "Layout_Id", "dbo.RoomDesk");
            DropForeignKey("dbo.RoomSeatBooking", "Seat_Id", "dbo.RoomSeat");
            DropForeignKey("dbo.RoomDesk", "Layout_Id", "dbo.RoomLayout");
            DropIndex("dbo.PublishingChannel", new[] { "Organiser_Id" });
            DropIndex("dbo.PublishingChannel", new[] { "LabelSet_Id" });
            DropIndex("dbo.PublishingChannel", new[] { "Curriculum_Id" });
            DropIndex("dbo.ActivityPublication", new[] { "PublishingChannel_Id" });
            DropIndex("dbo.ActivityPublication", new[] { "Activity_Id" });
            DropIndex("dbo.RoomSeatBooking", new[] { "Seat_Id" });
            DropIndex("dbo.RoomSeat", new[] { "Layout_Id" });
            DropIndex("dbo.RoomDesk", new[] { "Layout_Id" });
            DropIndex("dbo.RoomLayout", new[] { "Room_Id" });
            DropTable("dbo.PublishingChannel");
            DropTable("dbo.ActivityPublication");
            DropTable("dbo.RoomSeatBooking");
            DropTable("dbo.RoomSeat");
            DropTable("dbo.RoomDesk");
            DropTable("dbo.RoomLayout");
        }
    }
}
