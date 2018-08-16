namespace MyStik.TimeTable.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LotteryV5 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.LotteryGame",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        UserId = c.String(),
                        Created = c.DateTime(nullable: false),
                        LastChange = c.DateTime(nullable: false),
                        AcceptDefault = c.Boolean(nullable: false),
                        CoursesWanted = c.Int(nullable: false),
                        Lottery_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Lottery", t => t.Lottery_Id)
                .Index(t => t.Lottery_Id);
            
            AddColumn("dbo.Lottery", "LoINeeded", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.LotteryGame", "Lottery_Id", "dbo.Lottery");
            DropIndex("dbo.LotteryGame", new[] { "Lottery_Id" });
            DropColumn("dbo.Lottery", "LoINeeded");
            DropTable("dbo.LotteryGame");
        }
    }
}
