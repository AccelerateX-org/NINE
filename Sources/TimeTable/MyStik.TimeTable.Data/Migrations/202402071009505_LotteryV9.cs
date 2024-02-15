namespace MyStik.TimeTable.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LotteryV9 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.SeatQuotaFraction",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Percentage = c.Int(nullable: false),
                        Weight = c.Int(nullable: false),
                        Curriculum_Id = c.Guid(),
                        ItemLabelSet_Id = c.Guid(),
                        Quota_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Curriculum", t => t.Curriculum_Id)
                .ForeignKey("dbo.ItemLabelSet", t => t.ItemLabelSet_Id)
                .ForeignKey("dbo.SeatQuota", t => t.Quota_Id)
                .Index(t => t.Curriculum_Id)
                .Index(t => t.ItemLabelSet_Id)
                .Index(t => t.Quota_Id);
            
            AddColumn("dbo.SeatQuota", "Name", c => c.String());
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SeatQuotaFraction", "Quota_Id", "dbo.SeatQuota");
            DropForeignKey("dbo.SeatQuotaFraction", "ItemLabelSet_Id", "dbo.ItemLabelSet");
            DropForeignKey("dbo.SeatQuotaFraction", "Curriculum_Id", "dbo.Curriculum");
            DropIndex("dbo.SeatQuotaFraction", new[] { "Quota_Id" });
            DropIndex("dbo.SeatQuotaFraction", new[] { "ItemLabelSet_Id" });
            DropIndex("dbo.SeatQuotaFraction", new[] { "Curriculum_Id" });
            DropColumn("dbo.SeatQuota", "Name");
            DropTable("dbo.SeatQuotaFraction");
        }
    }
}
