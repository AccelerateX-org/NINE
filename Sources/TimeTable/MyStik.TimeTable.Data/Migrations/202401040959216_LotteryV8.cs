namespace MyStik.TimeTable.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LotteryV8 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.SeatQuota",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        MinCapacity = c.Int(nullable: false),
                        MaxCapacity = c.Int(nullable: false),
                        Description = c.String(),
                        Curriculum_Id = c.Guid(),
                        ItemLabelSet_Id = c.Guid(),
                        Occurrence_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Curriculum", t => t.Curriculum_Id)
                .ForeignKey("dbo.ItemLabelSet", t => t.ItemLabelSet_Id)
                .ForeignKey("dbo.Occurrence", t => t.Occurrence_Id)
                .Index(t => t.Curriculum_Id)
                .Index(t => t.ItemLabelSet_Id)
                .Index(t => t.Occurrence_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SeatQuota", "Occurrence_Id", "dbo.Occurrence");
            DropForeignKey("dbo.SeatQuota", "ItemLabelSet_Id", "dbo.ItemLabelSet");
            DropForeignKey("dbo.SeatQuota", "Curriculum_Id", "dbo.Curriculum");
            DropIndex("dbo.SeatQuota", new[] { "Occurrence_Id" });
            DropIndex("dbo.SeatQuota", new[] { "ItemLabelSet_Id" });
            DropIndex("dbo.SeatQuota", new[] { "Curriculum_Id" });
            DropTable("dbo.SeatQuota");
        }
    }
}
