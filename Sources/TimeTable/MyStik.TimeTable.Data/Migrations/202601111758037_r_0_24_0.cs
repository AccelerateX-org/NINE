namespace MyStik.TimeTable.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class r_0_24_0 : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.OccurrenceGroupSemesterGroup", newName: "SemesterGroupOccurrenceGroup");
            DropForeignKey("dbo.Committee", "Curriculum_Id", "dbo.Curriculum");
            DropIndex("dbo.Committee", new[] { "Curriculum_Id" });
            DropPrimaryKey("dbo.SemesterGroupOccurrenceGroup");
            AddPrimaryKey("dbo.SemesterGroupOccurrenceGroup", new[] { "SemesterGroup_Id", "OccurrenceGroup_Id" });
            DropColumn("dbo.Committee", "Curriculum_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Committee", "Curriculum_Id", c => c.Guid());
            DropPrimaryKey("dbo.SemesterGroupOccurrenceGroup");
            AddPrimaryKey("dbo.SemesterGroupOccurrenceGroup", new[] { "OccurrenceGroup_Id", "SemesterGroup_Id" });
            CreateIndex("dbo.Committee", "Curriculum_Id");
            AddForeignKey("dbo.Committee", "Curriculum_Id", "dbo.Curriculum", "Id");
            RenameTable(name: "dbo.SemesterGroupOccurrenceGroup", newName: "OccurrenceGroupSemesterGroup");
        }
    }
}
