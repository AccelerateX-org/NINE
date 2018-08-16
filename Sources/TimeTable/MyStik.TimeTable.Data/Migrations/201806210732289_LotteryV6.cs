namespace MyStik.TimeTable.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LotteryV6 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Lottery", "AllowManualSubscription", c => c.Boolean(nullable: false));
            AddColumn("dbo.LotteryGame", "DrawingDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.LotteryGame", "DrawingDate");
            DropColumn("dbo.Lottery", "AllowManualSubscription");
        }
    }
}
