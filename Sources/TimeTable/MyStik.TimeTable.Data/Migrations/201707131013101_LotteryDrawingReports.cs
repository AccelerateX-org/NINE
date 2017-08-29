namespace MyStik.TimeTable.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LotteryDrawingReports : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.OccurrenceDrawing",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Start = c.DateTime(nullable: false),
                        End = c.DateTime(nullable: false),
                        LotteryDrawing_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.LotteryDrawing", t => t.LotteryDrawing_Id)
                .Index(t => t.LotteryDrawing_Id);
            
            CreateTable(
                "dbo.SubscriptionDrawing",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        DrawingTime = c.DateTime(nullable: false),
                        StateBeforeDrawing = c.Int(nullable: false),
                        LapCountBeforeDrawing = c.Int(nullable: false),
                        StateAfterDrawing = c.Int(nullable: false),
                        LapCountAfterDrawing = c.Int(nullable: false),
                        LapCountDrawing = c.Int(nullable: false),
                        Remark = c.String(),
                        OccurrenceDrawing_Id = c.Guid(),
                        Subscription_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.OccurrenceDrawing", t => t.OccurrenceDrawing_Id)
                .ForeignKey("dbo.Subscription", t => t.Subscription_Id)
                .Index(t => t.OccurrenceDrawing_Id)
                .Index(t => t.Subscription_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SubscriptionDrawing", "Subscription_Id", "dbo.Subscription");
            DropForeignKey("dbo.SubscriptionDrawing", "OccurrenceDrawing_Id", "dbo.OccurrenceDrawing");
            DropForeignKey("dbo.OccurrenceDrawing", "LotteryDrawing_Id", "dbo.LotteryDrawing");
            DropIndex("dbo.SubscriptionDrawing", new[] { "Subscription_Id" });
            DropIndex("dbo.SubscriptionDrawing", new[] { "OccurrenceDrawing_Id" });
            DropIndex("dbo.OccurrenceDrawing", new[] { "LotteryDrawing_Id" });
            DropTable("dbo.SubscriptionDrawing");
            DropTable("dbo.OccurrenceDrawing");
        }
    }
}
