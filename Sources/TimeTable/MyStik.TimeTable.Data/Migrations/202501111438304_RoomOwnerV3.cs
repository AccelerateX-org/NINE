namespace MyStik.TimeTable.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RoomOwnerV3 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.RoomAccess",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        IsOwner = c.Boolean(nullable: false),
                        MayBook = c.Boolean(nullable: false),
                        Member_Id = c.Guid(),
                        Room_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.OrganiserMember", t => t.Member_Id)
                .ForeignKey("dbo.Room", t => t.Room_Id)
                .Index(t => t.Member_Id)
                .Index(t => t.Room_Id);
            
            AddColumn("dbo.Activity", "IsOpenDoor", c => c.Boolean(defaultValue:false));
            AddColumn("dbo.Room", "IsBookable", c => c.Boolean(nullable: false, defaultValue:true));
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.RoomAccess", "Room_Id", "dbo.Room");
            DropForeignKey("dbo.RoomAccess", "Member_Id", "dbo.OrganiserMember");
            DropIndex("dbo.RoomAccess", new[] { "Room_Id" });
            DropIndex("dbo.RoomAccess", new[] { "Member_Id" });
            DropColumn("dbo.Room", "IsBookable");
            DropColumn("dbo.Activity", "IsOpenDoor");
            DropTable("dbo.RoomAccess");
        }
    }
}
