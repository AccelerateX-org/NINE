namespace MyStik.TimeTable.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LotteryV7 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.LotteryBundle",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Name = c.String(),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.Lottery", "IsScheduled", c => c.Boolean(nullable: false));
            AddColumn("dbo.Lottery", "UseLapCount", c => c.Boolean(nullable: false));
            AddColumn("dbo.Lottery", "LotteryBundle_Id", c => c.Guid());
            CreateIndex("dbo.Lottery", "LotteryBundle_Id");
            AddForeignKey("dbo.Lottery", "LotteryBundle_Id", "dbo.LotteryBundle", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Lottery", "LotteryBundle_Id", "dbo.LotteryBundle");
            DropIndex("dbo.Lottery", new[] { "LotteryBundle_Id" });
            DropColumn("dbo.Lottery", "LotteryBundle_Id");
            DropColumn("dbo.Lottery", "UseLapCount");
            DropColumn("dbo.Lottery", "IsScheduled");
            DropTable("dbo.LotteryBundle");
        }
    }
}
