namespace MyStik.TimeTable.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModuleCatalogV2 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.SubjectAccreditation", "Subject_Id", "dbo.TeachingUnit");
            DropIndex("dbo.SubjectAccreditation", new[] { "Subject_Id" });
            CreateTable(
                "dbo.SubjectAccreditationTeachingUnit",
                c => new
                    {
                        SubjectAccreditation_Id = c.Guid(nullable: false),
                        TeachingUnit_Id = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.SubjectAccreditation_Id, t.TeachingUnit_Id })
                .ForeignKey("dbo.SubjectAccreditation", t => t.SubjectAccreditation_Id, cascadeDelete: true)
                .ForeignKey("dbo.TeachingUnit", t => t.TeachingUnit_Id, cascadeDelete: true)
                .Index(t => t.SubjectAccreditation_Id)
                .Index(t => t.TeachingUnit_Id);
            
            AddColumn("dbo.Activity", "Tag", c => c.String());
            AddColumn("dbo.Activity", "TeachingUnit_Id", c => c.Guid());
            AddColumn("dbo.TeachingUnit", "Tag", c => c.String());
            AddColumn("dbo.TeachingUnit", "Name", c => c.String());
            AddColumn("dbo.TeachingUnit", "Description", c => c.String());
            AddColumn("dbo.SubjectAccreditation", "Tag", c => c.String());
            CreateIndex("dbo.Activity", "TeachingUnit_Id");
            AddForeignKey("dbo.Activity", "TeachingUnit_Id", "dbo.TeachingUnit", "Id");
            DropColumn("dbo.TeachingUnit", "Name2");
            DropColumn("dbo.TeachingUnit", "Description2");
            DropColumn("dbo.SubjectAccreditation", "Subject_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.SubjectAccreditation", "Subject_Id", c => c.Guid());
            AddColumn("dbo.TeachingUnit", "Description2", c => c.String());
            AddColumn("dbo.TeachingUnit", "Name2", c => c.String());
            DropForeignKey("dbo.Activity", "TeachingUnit_Id", "dbo.TeachingUnit");
            DropForeignKey("dbo.SubjectAccreditationTeachingUnit", "TeachingUnit_Id", "dbo.TeachingUnit");
            DropForeignKey("dbo.SubjectAccreditationTeachingUnit", "SubjectAccreditation_Id", "dbo.SubjectAccreditation");
            DropIndex("dbo.SubjectAccreditationTeachingUnit", new[] { "TeachingUnit_Id" });
            DropIndex("dbo.SubjectAccreditationTeachingUnit", new[] { "SubjectAccreditation_Id" });
            DropIndex("dbo.Activity", new[] { "TeachingUnit_Id" });
            DropColumn("dbo.SubjectAccreditation", "Tag");
            DropColumn("dbo.TeachingUnit", "Description");
            DropColumn("dbo.TeachingUnit", "Name");
            DropColumn("dbo.TeachingUnit", "Tag");
            DropColumn("dbo.Activity", "TeachingUnit_Id");
            DropColumn("dbo.Activity", "Tag");
            DropTable("dbo.SubjectAccreditationTeachingUnit");
            CreateIndex("dbo.SubjectAccreditation", "Subject_Id");
            AddForeignKey("dbo.SubjectAccreditation", "Subject_Id", "dbo.TeachingUnit", "Id");
        }
    }
}
