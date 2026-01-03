namespace MyStik.TimeTable.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class r_0_23_0 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Curriculum", "StatuteTakeEffect", c => c.DateTime());
            AddColumn("dbo.Degree", "Level", c => c.Int(nullable: false));
            DropColumn("dbo.Degree", "IsUndergraduate");
            DropColumn("dbo.Degree", "IsCertificate");
            DropColumn("dbo.Degree", "IsPhD");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Degree", "IsPhD", c => c.Boolean(nullable: false));
            AddColumn("dbo.Degree", "IsCertificate", c => c.Boolean(nullable: false));
            AddColumn("dbo.Degree", "IsUndergraduate", c => c.Boolean(nullable: false));
            DropColumn("dbo.Degree", "Level");
            DropColumn("dbo.Curriculum", "StatuteTakeEffect");
        }
    }
}
