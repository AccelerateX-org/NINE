namespace MyStik.TimeTable.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class VirtualRoomsV1 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.VirtualRoomAccess",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        isDefault = c.Boolean(nullable: false),
                        Token = c.String(),
                        Date_Id = c.Guid(),
                        Room_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ActivityDate", t => t.Date_Id)
                .ForeignKey("dbo.VirtualRoom", t => t.Room_Id)
                .Index(t => t.Date_Id)
                .Index(t => t.Room_Id);
            
            CreateTable(
                "dbo.VirtualRoom",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Name = c.String(),
                        Description = c.String(),
                        TokenName = c.String(),
                        Token = c.String(),
                        AccessUrl = c.String(),
                        ParticipientsOnly = c.Boolean(nullable: false),
                        Owner_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.OrganiserMember", t => t.Owner_Id)
                .Index(t => t.Owner_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.VirtualRoom", "Owner_Id", "dbo.OrganiserMember");
            DropForeignKey("dbo.VirtualRoomAccess", "Room_Id", "dbo.VirtualRoom");
            DropForeignKey("dbo.VirtualRoomAccess", "Date_Id", "dbo.ActivityDate");
            DropIndex("dbo.VirtualRoom", new[] { "Owner_Id" });
            DropIndex("dbo.VirtualRoomAccess", new[] { "Room_Id" });
            DropIndex("dbo.VirtualRoomAccess", new[] { "Date_Id" });
            DropTable("dbo.VirtualRoom");
            DropTable("dbo.VirtualRoomAccess");
        }
    }
}
