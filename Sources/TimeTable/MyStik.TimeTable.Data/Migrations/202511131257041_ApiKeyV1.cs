namespace MyStik.TimeTable.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ApiKeyV1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ActivityDate", "IsOnline", c => c.Boolean());
            AddColumn("dbo.ActivityDate", "IsAway", c => c.Boolean());
            AddColumn("dbo.OrganiserMember", "ApiKey", c => c.String());
            AddColumn("dbo.OrganiserMember", "ApiKeyValidUntil", c => c.DateTime());
            AddColumn("dbo.ModuleSubject", "NameEn", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ModuleSubject", "NameEn");
            DropColumn("dbo.OrganiserMember", "ApiKeyValidUntil");
            DropColumn("dbo.OrganiserMember", "ApiKey");
            DropColumn("dbo.ActivityDate", "IsAway");
            DropColumn("dbo.ActivityDate", "IsOnline");
        }
    }
}
