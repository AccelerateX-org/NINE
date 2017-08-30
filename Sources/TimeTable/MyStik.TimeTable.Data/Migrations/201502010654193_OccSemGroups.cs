namespace MyStik.TimeTable.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class OccSemGroups : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.SemesterGroup", "OccurrenceGroup_Id", "dbo.OccurrenceGroup");
            DropIndex("dbo.SemesterGroup", new[] { "OccurrenceGroup_Id" });
            CreateTable(
                "dbo.OccurrenceGroupSemesterGroup",
                c => new
                    {
                        OccurrenceGroup_Id = c.Guid(nullable: false),
                        SemesterGroup_Id = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.OccurrenceGroup_Id, t.SemesterGroup_Id })
                .ForeignKey("dbo.OccurrenceGroup", t => t.OccurrenceGroup_Id, cascadeDelete: true)
                .ForeignKey("dbo.SemesterGroup", t => t.SemesterGroup_Id, cascadeDelete: true)
                .Index(t => t.OccurrenceGroup_Id)
                .Index(t => t.SemesterGroup_Id);
            
            DropColumn("dbo.SemesterGroup", "OccurrenceGroup_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.SemesterGroup", "OccurrenceGroup_Id", c => c.Guid());
            DropForeignKey("dbo.OccurrenceGroupSemesterGroup", "SemesterGroup_Id", "dbo.SemesterGroup");
            DropForeignKey("dbo.OccurrenceGroupSemesterGroup", "OccurrenceGroup_Id", "dbo.OccurrenceGroup");
            DropIndex("dbo.OccurrenceGroupSemesterGroup", new[] { "SemesterGroup_Id" });
            DropIndex("dbo.OccurrenceGroupSemesterGroup", new[] { "OccurrenceGroup_Id" });
            DropTable("dbo.OccurrenceGroupSemesterGroup");
            CreateIndex("dbo.SemesterGroup", "OccurrenceGroup_Id");
            AddForeignKey("dbo.SemesterGroup", "OccurrenceGroup_Id", "dbo.OccurrenceGroup", "Id");
        }
    }
}
