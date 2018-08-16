namespace MyStik.TimeTable.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CurriculumV4 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ModuleAccreditation", "IsMandatory", c => c.Boolean(nullable: false));
            AddColumn("dbo.CurriculumCriteria", "PackageOption_Id", c => c.Guid());
            AddColumn("dbo.CurriculumCriteria", "Group_Id", c => c.Guid());
            AddColumn("dbo.Curriculum", "IsPublished", c => c.Boolean(nullable: false));
            AddColumn("dbo.Curriculum", "IsDeprecated", c => c.Boolean(nullable: false));
            AddColumn("dbo.Curriculum", "Version", c => c.String());
            AddColumn("dbo.Curriculum", "ValidSince_Id", c => c.Guid());
            AddColumn("dbo.OrganiserMember", "FirstName", c => c.String());
            AddColumn("dbo.OrganiserMember", "Title", c => c.String());
            AddColumn("dbo.OrganiserMember", "ShowTitle", c => c.Boolean(nullable: false));
            AddColumn("dbo.OrganiserMember", "ShowDescription", c => c.Boolean(nullable: false));
            CreateIndex("dbo.CurriculumCriteria", "PackageOption_Id");
            CreateIndex("dbo.CurriculumCriteria", "Group_Id");
            CreateIndex("dbo.Curriculum", "ValidSince_Id");
            AddForeignKey("dbo.CurriculumCriteria", "PackageOption_Id", "dbo.PackageOption", "Id");
            AddForeignKey("dbo.Curriculum", "ValidSince_Id", "dbo.Semester", "Id");
            AddForeignKey("dbo.CurriculumCriteria", "Group_Id", "dbo.CurriculumGroup", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CurriculumCriteria", "Group_Id", "dbo.CurriculumGroup");
            DropForeignKey("dbo.Curriculum", "ValidSince_Id", "dbo.Semester");
            DropForeignKey("dbo.CurriculumCriteria", "PackageOption_Id", "dbo.PackageOption");
            DropIndex("dbo.Curriculum", new[] { "ValidSince_Id" });
            DropIndex("dbo.CurriculumCriteria", new[] { "Group_Id" });
            DropIndex("dbo.CurriculumCriteria", new[] { "PackageOption_Id" });
            DropColumn("dbo.OrganiserMember", "ShowDescription");
            DropColumn("dbo.OrganiserMember", "ShowTitle");
            DropColumn("dbo.OrganiserMember", "Title");
            DropColumn("dbo.OrganiserMember", "FirstName");
            DropColumn("dbo.Curriculum", "ValidSince_Id");
            DropColumn("dbo.Curriculum", "Version");
            DropColumn("dbo.Curriculum", "IsDeprecated");
            DropColumn("dbo.Curriculum", "IsPublished");
            DropColumn("dbo.CurriculumCriteria", "Group_Id");
            DropColumn("dbo.CurriculumCriteria", "PackageOption_Id");
            DropColumn("dbo.ModuleAccreditation", "IsMandatory");
        }
    }
}
