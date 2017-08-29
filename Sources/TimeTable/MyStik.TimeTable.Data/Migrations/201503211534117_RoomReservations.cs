namespace MyStik.TimeTable.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class RoomReservations : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.RoomBooking",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        UserId = c.String(),
                        TimeStamp = c.DateTime(nullable: false),
                        Date_Id = c.Guid(),
                        Room_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ActivityDate", t => t.Date_Id)
                .ForeignKey("dbo.Room", t => t.Room_Id)
                .Index(t => t.Date_Id)
                .Index(t => t.Room_Id);
            
            CreateTable(
                "dbo.BookingConfirmation",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        UserId = c.String(),
                        TimeStamp = c.DateTime(),
                        IsConfirmed = c.Boolean(nullable: false),
                        Organiser_Id = c.Guid(),
                        RoomBooking_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ActivityOrganiser", t => t.Organiser_Id)
                .ForeignKey("dbo.RoomBooking", t => t.RoomBooking_Id)
                .Index(t => t.Organiser_Id)
                .Index(t => t.RoomBooking_Id);
            
            AddColumn("dbo.RoomAssignment", "InternalNeedConfirmation", c => c.Boolean(nullable: false));
            AddColumn("dbo.RoomAssignment", "ExternalNeedConfirmation", c => c.Boolean(nullable: false));
            DropColumn("dbo.RoomAssignment", "IsExclusive");
        }
        
        public override void Down()
        {
            AddColumn("dbo.RoomAssignment", "IsExclusive", c => c.Boolean(nullable: false));
            DropForeignKey("dbo.RoomBooking", "Room_Id", "dbo.Room");
            DropForeignKey("dbo.RoomBooking", "Date_Id", "dbo.ActivityDate");
            DropForeignKey("dbo.BookingConfirmation", "RoomBooking_Id", "dbo.RoomBooking");
            DropForeignKey("dbo.BookingConfirmation", "Organiser_Id", "dbo.ActivityOrganiser");
            DropIndex("dbo.BookingConfirmation", new[] { "RoomBooking_Id" });
            DropIndex("dbo.BookingConfirmation", new[] { "Organiser_Id" });
            DropIndex("dbo.RoomBooking", new[] { "Room_Id" });
            DropIndex("dbo.RoomBooking", new[] { "Date_Id" });
            DropColumn("dbo.RoomAssignment", "ExternalNeedConfirmation");
            DropColumn("dbo.RoomAssignment", "InternalNeedConfirmation");
            DropTable("dbo.BookingConfirmation");
            DropTable("dbo.RoomBooking");
        }
    }
}
