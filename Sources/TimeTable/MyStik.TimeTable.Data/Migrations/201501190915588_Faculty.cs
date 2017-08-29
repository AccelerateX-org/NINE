namespace MyStik.TimeTable.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class Faculty : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Activity", "KeyMoodleCourse", c => c.String());
            AddColumn("dbo.Activity", "Title", c => c.String());
            AddColumn("dbo.Curriculum", "Organiser_Id", c => c.Guid());
            AlterColumn("dbo.ActivityOrganiser", "IsStudent", c => c.Boolean(nullable: false));
            AlterColumn("dbo.ActivityOrganiser", "IsFaculty", c => c.Boolean(nullable: false));
            CreateIndex("dbo.Curriculum", "Organiser_Id");
            AddForeignKey("dbo.Curriculum", "Organiser_Id", "dbo.ActivityOrganiser", "Id");

            Sql("UPDATE dbo.ActivityOrganiser SET IsStudent = 'False'");
            Sql("UPDATE dbo.ActivityOrganiser SET IsFaculty = 'True'");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Curriculum", "Organiser_Id", "dbo.ActivityOrganiser");
            DropIndex("dbo.Curriculum", new[] { "Organiser_Id" });
            AlterColumn("dbo.ActivityOrganiser", "IsFaculty", c => c.Boolean());
            AlterColumn("dbo.ActivityOrganiser", "IsStudent", c => c.Boolean());
            DropColumn("dbo.Curriculum", "Organiser_Id");
            DropColumn("dbo.Activity", "Title");
            DropColumn("dbo.Activity", "KeyMoodleCourse");
        }
    }
}
