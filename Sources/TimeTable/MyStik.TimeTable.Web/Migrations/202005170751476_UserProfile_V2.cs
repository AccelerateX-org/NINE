namespace MyStik.TimeTable.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UserProfile_V2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "Title", c => c.String());
            AddColumn("dbo.AspNetUsers", "FileType", c => c.String());
            AddColumn("dbo.AspNetUsers", "BinaryData", c => c.Binary());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "BinaryData");
            DropColumn("dbo.AspNetUsers", "FileType");
            DropColumn("dbo.AspNetUsers", "Title");
        }
    }
}
