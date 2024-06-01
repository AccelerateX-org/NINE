namespace MyStik.TimeTable.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RoomOwnerV2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RoomAssignment", "IsOwner", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.RoomAssignment", "IsOwner");
        }
    }
}
