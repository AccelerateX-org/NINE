namespace MyStik.TimeTable.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LotteryV10 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SeatQuota", "Lottery_Id", c => c.Guid());
            CreateIndex("dbo.SeatQuota", "Lottery_Id");
            AddForeignKey("dbo.SeatQuota", "Lottery_Id", "dbo.Lottery", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SeatQuota", "Lottery_Id", "dbo.Lottery");
            DropIndex("dbo.SeatQuota", new[] { "Lottery_Id" });
            DropColumn("dbo.SeatQuota", "Lottery_Id");
        }
    }
}
