namespace MyStik.TimeTable.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MiscMarch2024 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Activity", "Comment", c => c.String());
            AddColumn("dbo.Activity", "Preference", c => c.Int());
            AddColumn("dbo.ActivityDate", "IsVirtual", c => c.Boolean());
            AddColumn("dbo.ActivityDate", "IsRealWorld", c => c.Boolean());
            AddColumn("dbo.Subscription", "CheckInRemark", c => c.String());
            AddColumn("dbo.Subscription", "CheckOutRemark", c => c.String());
            AddColumn("dbo.Subscription", "ValdUntil", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Subscription", "ValdUntil");
            DropColumn("dbo.Subscription", "CheckOutRemark");
            DropColumn("dbo.Subscription", "CheckInRemark");
            DropColumn("dbo.ActivityDate", "IsRealWorld");
            DropColumn("dbo.ActivityDate", "IsVirtual");
            DropColumn("dbo.Activity", "Preference");
            DropColumn("dbo.Activity", "Comment");
        }
    }
}
