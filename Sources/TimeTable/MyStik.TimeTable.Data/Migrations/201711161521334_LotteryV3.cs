namespace MyStik.TimeTable.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class LotteryV3 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.LotteryBudget",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Name = c.String(),
                        Description = c.String(),
                        IsRestricted = c.Boolean(nullable: false),
                        Size = c.Int(nullable: false),
                        Lottery_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Lottery", t => t.Lottery_Id)
                .Index(t => t.Lottery_Id);
            
            CreateTable(
                "dbo.LotteryBet",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Amount = c.Int(nullable: false),
                        IsApproved = c.Boolean(nullable: false),
                        IsConsumed = c.Boolean(nullable: false),
                        Budget_Id = c.Guid(),
                        Subscription_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.LotteryBudget", t => t.Budget_Id)
                .ForeignKey("dbo.Subscription", t => t.Subscription_Id)
                .Index(t => t.Budget_Id)
                .Index(t => t.Subscription_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.LotteryBudget", "Lottery_Id", "dbo.Lottery");
            DropForeignKey("dbo.LotteryBet", "Subscription_Id", "dbo.Subscription");
            DropForeignKey("dbo.LotteryBet", "Budget_Id", "dbo.LotteryBudget");
            DropIndex("dbo.LotteryBet", new[] { "Subscription_Id" });
            DropIndex("dbo.LotteryBet", new[] { "Budget_Id" });
            DropIndex("dbo.LotteryBudget", new[] { "Lottery_Id" });
            DropTable("dbo.LotteryBet");
            DropTable("dbo.LotteryBudget");
        }
    }
}
