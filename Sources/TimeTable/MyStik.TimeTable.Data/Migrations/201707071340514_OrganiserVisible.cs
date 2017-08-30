namespace MyStik.TimeTable.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class OrganiserVisible : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ActivityOrganiser", "HtmlColor", c => c.String());
            AddColumn("dbo.ActivityOrganiser", "SupportUrl", c => c.String());
            AddColumn("dbo.ActivityOrganiser", "SupportEMail", c => c.String());
            AddColumn("dbo.ActivityOrganiser", "IsVisible", c => c.Boolean(nullable: false));

            Sql("UPDATE dbo.ActivityOrganiser SET IsVisible  = 'False'");
        }
        
        public override void Down()
        {
            DropColumn("dbo.ActivityOrganiser", "IsVisible");
            DropColumn("dbo.ActivityOrganiser", "SupportEMail");
            DropColumn("dbo.ActivityOrganiser", "SupportUrl");
            DropColumn("dbo.ActivityOrganiser", "HtmlColor");
        }
    }
}
