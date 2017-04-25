namespace MyStik.TimeTable.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SemesterDate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OrganiserMember", "IsAssociated", c => c.Boolean(nullable: false));
            AddColumn("dbo.SemesterDate", "IsInternal", c => c.Boolean());
            AddColumn("dbo.SemesterDate", "Organiser_Id", c => c.Guid());
            CreateIndex("dbo.SemesterDate", "Organiser_Id");
            AddForeignKey("dbo.SemesterDate", "Organiser_Id", "dbo.ActivityOrganiser", "Id");

            Sql("UPDATE dbo.OrganiserMember SET IsAssociated  = 'False'");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SemesterDate", "Organiser_Id", "dbo.ActivityOrganiser");
            DropIndex("dbo.SemesterDate", new[] { "Organiser_Id" });
            DropColumn("dbo.SemesterDate", "Organiser_Id");
            DropColumn("dbo.SemesterDate", "IsInternal");
            DropColumn("dbo.OrganiserMember", "IsAssociated");
        }
    }
}
