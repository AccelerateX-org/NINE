namespace MyStik.TimeTable.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Curiclum_v9 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TeachingUnit", "Name2", c => c.String());
            AddColumn("dbo.TeachingUnit", "Description2", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.TeachingUnit", "Description2");
            DropColumn("dbo.TeachingUnit", "Name2");
        }
    }
}
