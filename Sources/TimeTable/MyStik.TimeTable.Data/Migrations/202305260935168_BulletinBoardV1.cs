namespace MyStik.TimeTable.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class BulletinBoardV1 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.BulletinBoard",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Name = c.String(),
                        Description = c.String(),
                        Autonomy_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Autonomy", t => t.Autonomy_Id)
                .Index(t => t.Autonomy_Id);
            
            CreateTable(
                "dbo.BoardPosting",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Published = c.DateTime(nullable: false),
                        Advertisement_Id = c.Guid(),
                        BulletinBoard_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Advertisement", t => t.Advertisement_Id)
                .ForeignKey("dbo.BulletinBoard", t => t.BulletinBoard_Id)
                .Index(t => t.Advertisement_Id)
                .Index(t => t.BulletinBoard_Id);
            
            AddColumn("dbo.Curriculum", "BulletinBoard_Id", c => c.Guid());
            AddColumn("dbo.OrganiserMember", "BulletinBoard_Id", c => c.Guid());
            AddColumn("dbo.ActivityOrganiser", "BulletinBoard_Id", c => c.Guid());
            CreateIndex("dbo.Curriculum", "BulletinBoard_Id");
            CreateIndex("dbo.OrganiserMember", "BulletinBoard_Id");
            CreateIndex("dbo.ActivityOrganiser", "BulletinBoard_Id");
            AddForeignKey("dbo.OrganiserMember", "BulletinBoard_Id", "dbo.BulletinBoard", "Id");
            AddForeignKey("dbo.ActivityOrganiser", "BulletinBoard_Id", "dbo.BulletinBoard", "Id");
            AddForeignKey("dbo.Curriculum", "BulletinBoard_Id", "dbo.BulletinBoard", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Curriculum", "BulletinBoard_Id", "dbo.BulletinBoard");
            DropForeignKey("dbo.ActivityOrganiser", "BulletinBoard_Id", "dbo.BulletinBoard");
            DropForeignKey("dbo.OrganiserMember", "BulletinBoard_Id", "dbo.BulletinBoard");
            DropForeignKey("dbo.BoardPosting", "BulletinBoard_Id", "dbo.BulletinBoard");
            DropForeignKey("dbo.BoardPosting", "Advertisement_Id", "dbo.Advertisement");
            DropForeignKey("dbo.BulletinBoard", "Autonomy_Id", "dbo.Autonomy");
            DropIndex("dbo.ActivityOrganiser", new[] { "BulletinBoard_Id" });
            DropIndex("dbo.BoardPosting", new[] { "BulletinBoard_Id" });
            DropIndex("dbo.BoardPosting", new[] { "Advertisement_Id" });
            DropIndex("dbo.BulletinBoard", new[] { "Autonomy_Id" });
            DropIndex("dbo.OrganiserMember", new[] { "BulletinBoard_Id" });
            DropIndex("dbo.Curriculum", new[] { "BulletinBoard_Id" });
            DropColumn("dbo.ActivityOrganiser", "BulletinBoard_Id");
            DropColumn("dbo.OrganiserMember", "BulletinBoard_Id");
            DropColumn("dbo.Curriculum", "BulletinBoard_Id");
            DropTable("dbo.BoardPosting");
            DropTable("dbo.BulletinBoard");
        }
    }
}
