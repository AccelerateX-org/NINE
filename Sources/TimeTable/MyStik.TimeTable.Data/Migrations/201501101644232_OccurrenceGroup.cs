namespace MyStik.TimeTable.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class OccurrenceGroup : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.OccurrenceGroup",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Capacity = c.Int(nullable: false),
                        FitToCurriculumOnly = c.Boolean(nullable: false),
                        Occurrence_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Occurrence", t => t.Occurrence_Id)
                .Index(t => t.Occurrence_Id);
            
            AddColumn("dbo.Occurrence", "LotteryEnabled", c => c.Boolean(nullable: false));
            AddColumn("dbo.Subscription", "OnWaitingList", c => c.Boolean());
            AddColumn("dbo.Subscription", "LapCount", c => c.Int());
            AddColumn("dbo.Subscription", "IsConfirmed", c => c.Boolean());
            AddColumn("dbo.SemesterGroup", "OccurrenceGroup_Id", c => c.Guid());
            CreateIndex("dbo.SemesterGroup", "OccurrenceGroup_Id");
            AddForeignKey("dbo.SemesterGroup", "OccurrenceGroup_Id", "dbo.OccurrenceGroup", "Id");

            Sql("UPDATE dbo.Occurrence SET LotteryEnabled = 'False'");
            Sql("UPDATE dbo.Subscription SET OnWaitingList = 'False'");
            Sql("UPDATE dbo.Subscription SET LapCount = 0");
            Sql("UPDATE dbo.Subscription SET IsConfirmed = 'True'");

        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SemesterGroup", "OccurrenceGroup_Id", "dbo.OccurrenceGroup");
            DropForeignKey("dbo.OccurrenceGroup", "Occurrence_Id", "dbo.Occurrence");
            DropIndex("dbo.SemesterGroup", new[] { "OccurrenceGroup_Id" });
            DropIndex("dbo.OccurrenceGroup", new[] { "Occurrence_Id" });
            DropColumn("dbo.SemesterGroup", "OccurrenceGroup_Id");
            DropColumn("dbo.Subscription", "IsConfirmed");
            DropColumn("dbo.Subscription", "LapCount");
            DropColumn("dbo.Subscription", "OnWaitingList");
            DropColumn("dbo.Occurrence", "LotteryEnabled");
            DropTable("dbo.OccurrenceGroup");
        }
    }
}
