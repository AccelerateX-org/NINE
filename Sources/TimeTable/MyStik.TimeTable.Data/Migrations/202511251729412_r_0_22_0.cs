namespace MyStik.TimeTable.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class r_0_22_0 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RoomBooking", "BookingDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.RoomBooking", "IsCreated", c => c.Boolean(nullable: false));
            AddColumn("dbo.RoomBooking", "ConfirmationDate", c => c.DateTime());
            AddColumn("dbo.RoomBooking", "IsConfirmed", c => c.Boolean());
            AddColumn("dbo.RoomBooking", "ConfirmationComment", c => c.String());
            AddColumn("dbo.RoomBooking", "AcknowledgementDate", c => c.DateTime());
            AddColumn("dbo.RoomBooking", "IsAcknowledged", c => c.Boolean());
            AddColumn("dbo.RoomBooking", "AcknowledgementComment", c => c.String());
            AddColumn("dbo.RoomBooking", "Acknowledger_Id", c => c.Guid());
            AddColumn("dbo.RoomBooking", "Booker_Id", c => c.Guid());
            AddColumn("dbo.RoomBooking", "Confirmer_Id", c => c.Guid());
            CreateIndex("dbo.RoomBooking", "Acknowledger_Id");
            CreateIndex("dbo.RoomBooking", "Booker_Id");
            CreateIndex("dbo.RoomBooking", "Confirmer_Id");
            AddForeignKey("dbo.RoomBooking", "Acknowledger_Id", "dbo.OrganiserMember", "Id");
            AddForeignKey("dbo.RoomBooking", "Booker_Id", "dbo.OrganiserMember", "Id");
            AddForeignKey("dbo.RoomBooking", "Confirmer_Id", "dbo.OrganiserMember", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.RoomBooking", "Confirmer_Id", "dbo.OrganiserMember");
            DropForeignKey("dbo.RoomBooking", "Booker_Id", "dbo.OrganiserMember");
            DropForeignKey("dbo.RoomBooking", "Acknowledger_Id", "dbo.OrganiserMember");
            DropIndex("dbo.RoomBooking", new[] { "Confirmer_Id" });
            DropIndex("dbo.RoomBooking", new[] { "Booker_Id" });
            DropIndex("dbo.RoomBooking", new[] { "Acknowledger_Id" });
            DropColumn("dbo.RoomBooking", "Confirmer_Id");
            DropColumn("dbo.RoomBooking", "Booker_Id");
            DropColumn("dbo.RoomBooking", "Acknowledger_Id");
            DropColumn("dbo.RoomBooking", "AcknowledgementComment");
            DropColumn("dbo.RoomBooking", "IsAcknowledged");
            DropColumn("dbo.RoomBooking", "AcknowledgementDate");
            DropColumn("dbo.RoomBooking", "ConfirmationComment");
            DropColumn("dbo.RoomBooking", "IsConfirmed");
            DropColumn("dbo.RoomBooking", "ConfirmationDate");
            DropColumn("dbo.RoomBooking", "IsCreated");
            DropColumn("dbo.RoomBooking", "BookingDate");
        }
    }
}
