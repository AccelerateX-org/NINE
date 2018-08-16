namespace MyStik.TimeTable.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LotteryV4 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Lottery", "MaxSubscription", c => c.Int(nullable: false));
            AddColumn("dbo.Lottery", "IsActive", c => c.Boolean(nullable: false));
            AddColumn("dbo.LotteryBudget", "Fraction", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.LotteryBudget", "Fraction");
            DropColumn("dbo.Lottery", "IsActive");
            DropColumn("dbo.Lottery", "MaxSubscription");
        }
    }
}
