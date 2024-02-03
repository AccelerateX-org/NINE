namespace MyStik.TimeTable.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AlumniV2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Alumnus", "Gender", c => c.String());
            AddColumn("dbo.Alumnus", "Faculty", c => c.String());
            AddColumn("dbo.Alumnus", "Created", c => c.DateTime());
            AddColumn("dbo.Alumnus", "Organiser_Id", c => c.Guid());
            CreateIndex("dbo.Alumnus", "Organiser_Id");
            AddForeignKey("dbo.Alumnus", "Organiser_Id", "dbo.ActivityOrganiser", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Alumnus", "Organiser_Id", "dbo.ActivityOrganiser");
            DropIndex("dbo.Alumnus", new[] { "Organiser_Id" });
            DropColumn("dbo.Alumnus", "Organiser_Id");
            DropColumn("dbo.Alumnus", "Created");
            DropColumn("dbo.Alumnus", "Faculty");
            DropColumn("dbo.Alumnus", "Gender");
        }
    }
}
