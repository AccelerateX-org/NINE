namespace MyStik.TimeTable.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AdvertisementV3 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Thesis", "HasLockFlag", c => c.Boolean(nullable: false));
            AddColumn("dbo.Advertisement", "ForInternship", c => c.Boolean(nullable: false));
            AddColumn("dbo.Advertisement", "ForThesis", c => c.Boolean(nullable: false));
            AddColumn("dbo.Advertisement", "ForStayAbroad", c => c.Boolean(nullable: false));
            AddColumn("dbo.Advertisement", "ForAdvancement", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Advertisement", "ForAdvancement");
            DropColumn("dbo.Advertisement", "ForStayAbroad");
            DropColumn("dbo.Advertisement", "ForThesis");
            DropColumn("dbo.Advertisement", "ForInternship");
            DropColumn("dbo.Thesis", "HasLockFlag");
        }
    }
}
