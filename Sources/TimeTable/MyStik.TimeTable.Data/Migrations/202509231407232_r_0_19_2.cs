namespace MyStik.TimeTable.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class r_0_19_2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Infoscreen", "Description", c => c.String());
            AddColumn("dbo.Infoscreen", "PublicTransporrtInfo", c => c.String());
            AddColumn("dbo.Infoscreen", "PublicTransporrtUrl", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Infoscreen", "PublicTransporrtUrl");
            DropColumn("dbo.Infoscreen", "PublicTransporrtInfo");
            DropColumn("dbo.Infoscreen", "Description");
        }
    }
}
