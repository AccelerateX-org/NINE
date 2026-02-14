namespace MyStik.TimeTable.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class r_0_28_0 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ActivitySubscriptionLog",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        OccurrenceId = c.Guid(nullable: false),
                        SubsscriberUserId = c.String(),
                        SubscriptionTimeStamp = c.DateTime(nullable: false),
                        ActorUserId = c.String(),
                        LotteryId = c.Guid(),
                        Timestamp = c.DateTime(nullable: false),
                        Action = c.Int(nullable: false),
                        State = c.Int(nullable: false),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.Room", "ServiceUrl", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Room", "ServiceUrl");
            DropTable("dbo.ActivitySubscriptionLog");
        }
    }
}
