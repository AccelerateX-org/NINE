namespace MyStik.TimeTable.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ScriptShop_v04 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ScriptOrder", "OrderedAt", c => c.DateTime(nullable: false));
            AddColumn("dbo.OrderBasket", "Delivered", c => c.Boolean());
        }
        
        public override void Down()
        {
            DropColumn("dbo.OrderBasket", "Delivered");
            DropColumn("dbo.ScriptOrder", "OrderedAt");
        }
    }
}
