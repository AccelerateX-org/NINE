namespace MyStik.TimeTable.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class ChangeLogs : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ActivityDateChange",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        UserId = c.String(),
                        TimeStamp = c.DateTime(nullable: false),
                        OldBegin = c.DateTime(nullable: false),
                        OldEnd = c.DateTime(nullable: false),
                        NewBegin = c.DateTime(nullable: false),
                        NewEnd = c.DateTime(nullable: false),
                        HasRoomChange = c.Boolean(nullable: false),
                        HasStateChange = c.Boolean(nullable: false),
                        HasTimeChange = c.Boolean(nullable: false),
                        Date_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ActivityDate", t => t.Date_Id)
                .Index(t => t.Date_Id);
            
            CreateTable(
                "dbo.RoomAllocationChange",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        UserId = c.String(),
                        TimeStamp = c.DateTime(nullable: false),
                        Begin = c.DateTime(nullable: false),
                        End = c.DateTime(nullable: false),
                        IsReleased = c.Boolean(nullable: false),
                        ConflictCount = c.Int(nullable: false),
                        DescriptionSource = c.String(),
                        DescriptionConflicts = c.String(),
                        Room_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Room", t => t.Room_Id)
                .Index(t => t.Room_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.RoomAllocationChange", "Room_Id", "dbo.Room");
            DropForeignKey("dbo.ActivityDateChange", "Date_Id", "dbo.ActivityDate");
            DropIndex("dbo.RoomAllocationChange", new[] { "Room_Id" });
            DropIndex("dbo.ActivityDateChange", new[] { "Date_Id" });
            DropTable("dbo.RoomAllocationChange");
            DropTable("dbo.ActivityDateChange");
        }
    }
}
