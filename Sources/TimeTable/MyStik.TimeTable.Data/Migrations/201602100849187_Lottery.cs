namespace MyStik.TimeTable.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Lottery : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Lottery",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Name = c.String(),
                        JobId = c.String(),
                        Description = c.String(),
                        MaxConfirm = c.Int(nullable: false),
                        FirstDrawing = c.DateTime(nullable: false),
                        LastDrawing = c.DateTime(nullable: false),
                        DrawingTime = c.Time(nullable: false, precision: 7),
                        DrawingFrequency = c.Int(nullable: false),
                        Owner_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.OrganiserMember", t => t.Owner_Id)
                .Index(t => t.Owner_Id);
            
            CreateTable(
                "dbo.LotteryDrawing",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Start = c.DateTime(nullable: false),
                        End = c.DateTime(nullable: false),
                        Message = c.String(),
                        JobId = c.String(),
                        Lottery_Id = c.Guid(),
                        Report_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Lottery", t => t.Lottery_Id)
                .ForeignKey("dbo.BinaryStorage", t => t.Report_Id)
                .Index(t => t.Lottery_Id)
                .Index(t => t.Report_Id);
            
            CreateTable(
                "dbo.LotteryOccurrence",
                c => new
                    {
                        Lottery_Id = c.Guid(nullable: false),
                        Occurrence_Id = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.Lottery_Id, t.Occurrence_Id })
                .ForeignKey("dbo.Lottery", t => t.Lottery_Id, cascadeDelete: true)
                .ForeignKey("dbo.Occurrence", t => t.Occurrence_Id, cascadeDelete: true)
                .Index(t => t.Lottery_Id)
                .Index(t => t.Occurrence_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Lottery", "Owner_Id", "dbo.OrganiserMember");
            DropForeignKey("dbo.LotteryOccurrence", "Occurrence_Id", "dbo.Occurrence");
            DropForeignKey("dbo.LotteryOccurrence", "Lottery_Id", "dbo.Lottery");
            DropForeignKey("dbo.LotteryDrawing", "Report_Id", "dbo.BinaryStorage");
            DropForeignKey("dbo.LotteryDrawing", "Lottery_Id", "dbo.Lottery");
            DropIndex("dbo.LotteryOccurrence", new[] { "Occurrence_Id" });
            DropIndex("dbo.LotteryOccurrence", new[] { "Lottery_Id" });
            DropIndex("dbo.LotteryDrawing", new[] { "Report_Id" });
            DropIndex("dbo.LotteryDrawing", new[] { "Lottery_Id" });
            DropIndex("dbo.Lottery", new[] { "Owner_Id" });
            DropTable("dbo.LotteryOccurrence");
            DropTable("dbo.LotteryDrawing");
            DropTable("dbo.Lottery");
        }
    }
}
