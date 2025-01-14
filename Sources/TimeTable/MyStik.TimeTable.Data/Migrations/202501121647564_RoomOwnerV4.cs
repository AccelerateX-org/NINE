namespace MyStik.TimeTable.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RoomOwnerV4 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Room", "IsBookable", c => c.Boolean());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Room", "IsBookable", c => c.Boolean(nullable: false));
        }
    }
}
