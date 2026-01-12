namespace MyStik.TimeTable.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class r_0_25_0 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CommitteeCompetence",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Autonomy_Id = c.Guid(),
                        Committee_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Autonomy", t => t.Autonomy_Id)
                .ForeignKey("dbo.Committee", t => t.Committee_Id)
                .Index(t => t.Autonomy_Id)
                .Index(t => t.Committee_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CommitteeCompetence", "Committee_Id", "dbo.Committee");
            DropForeignKey("dbo.CommitteeCompetence", "Autonomy_Id", "dbo.Autonomy");
            DropIndex("dbo.CommitteeCompetence", new[] { "Committee_Id" });
            DropIndex("dbo.CommitteeCompetence", new[] { "Autonomy_Id" });
            DropTable("dbo.CommitteeCompetence");
        }
    }
}
