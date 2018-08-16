namespace MyStik.TimeTable.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MemberV3 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.MemberResponsibility",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Description = c.String(),
                        Member_Id = c.Guid(),
                        Tag_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.OrganiserMember", t => t.Member_Id)
                .ForeignKey("dbo.Tag", t => t.Tag_Id)
                .Index(t => t.Member_Id)
                .Index(t => t.Tag_Id);
            
            CreateTable(
                "dbo.Tag",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.MemberSkill",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Description = c.String(),
                        Member_Id = c.Guid(),
                        Tag_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.OrganiserMember", t => t.Member_Id)
                .ForeignKey("dbo.Tag", t => t.Tag_Id)
                .Index(t => t.Member_Id)
                .Index(t => t.Tag_Id);
            
            CreateTable(
                "dbo.AdvertisementInfo",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Description = c.String(),
                        Advertisement_Id = c.Guid(),
                        Tag_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Advertisement", t => t.Advertisement_Id)
                .ForeignKey("dbo.Tag", t => t.Tag_Id)
                .Index(t => t.Advertisement_Id)
                .Index(t => t.Tag_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AdvertisementInfo", "Tag_Id", "dbo.Tag");
            DropForeignKey("dbo.AdvertisementInfo", "Advertisement_Id", "dbo.Advertisement");
            DropForeignKey("dbo.MemberSkill", "Tag_Id", "dbo.Tag");
            DropForeignKey("dbo.MemberSkill", "Member_Id", "dbo.OrganiserMember");
            DropForeignKey("dbo.MemberResponsibility", "Tag_Id", "dbo.Tag");
            DropForeignKey("dbo.MemberResponsibility", "Member_Id", "dbo.OrganiserMember");
            DropIndex("dbo.AdvertisementInfo", new[] { "Tag_Id" });
            DropIndex("dbo.AdvertisementInfo", new[] { "Advertisement_Id" });
            DropIndex("dbo.MemberSkill", new[] { "Tag_Id" });
            DropIndex("dbo.MemberSkill", new[] { "Member_Id" });
            DropIndex("dbo.MemberResponsibility", new[] { "Tag_Id" });
            DropIndex("dbo.MemberResponsibility", new[] { "Member_Id" });
            DropTable("dbo.AdvertisementInfo");
            DropTable("dbo.MemberSkill");
            DropTable("dbo.Tag");
            DropTable("dbo.MemberResponsibility");
        }
    }
}
