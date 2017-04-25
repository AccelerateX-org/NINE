namespace MyStik.TimeTable.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Notifications : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.NotificationState",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        UserId = c.String(),
                        IsNew = c.Boolean(nullable: false),
                        ReadingDate = c.DateTime(),
                        ActivityDateChange_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ActivityDateChange", t => t.ActivityDateChange_Id)
                .Index(t => t.ActivityDateChange_Id);
            
            AddColumn("dbo.ActivityDateChange", "NotificationContent", c => c.String());
            AddColumn("dbo.ActivityDateChange", "IsNotificationGenerated", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.NotificationState", "ActivityDateChange_Id", "dbo.ActivityDateChange");
            DropIndex("dbo.NotificationState", new[] { "ActivityDateChange_Id" });
            DropColumn("dbo.ActivityDateChange", "IsNotificationGenerated");
            DropColumn("dbo.ActivityDateChange", "NotificationContent");
            DropTable("dbo.NotificationState");
        }
    }
}
