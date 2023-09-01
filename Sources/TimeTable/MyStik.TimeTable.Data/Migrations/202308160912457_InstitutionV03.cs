namespace MyStik.TimeTable.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InstitutionV03 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.InstitutionActivityOrganiser", "Institution_Id", "dbo.Institution");
            DropForeignKey("dbo.InstitutionActivityOrganiser", "ActivityOrganiser_Id", "dbo.ActivityOrganiser");
            DropForeignKey("dbo.ActivityOrganiser", "ParentOrganiser_Id", "dbo.ActivityOrganiser");
            DropIndex("dbo.ActivityOrganiser", new[] { "ParentOrganiser_Id" });
            DropIndex("dbo.InstitutionActivityOrganiser", new[] { "Institution_Id" });
            DropIndex("dbo.InstitutionActivityOrganiser", new[] { "ActivityOrganiser_Id" });
            AddColumn("dbo.OrganiserMember", "IsInstitutionAdmin", c => c.Boolean(nullable: false));
            AddColumn("dbo.ActivityOrganiser", "Institution_Id", c => c.Guid());
            AddColumn("dbo.ActivityOrganiser", "LabelSet_Id", c => c.Guid());
            AddColumn("dbo.Institution", "LabelSet_Id", c => c.Guid());
            CreateIndex("dbo.ActivityOrganiser", "Institution_Id");
            CreateIndex("dbo.ActivityOrganiser", "LabelSet_Id");
            CreateIndex("dbo.Institution", "LabelSet_Id");
            AddForeignKey("dbo.Institution", "LabelSet_Id", "dbo.ItemLabelSet", "Id");
            AddForeignKey("dbo.ActivityOrganiser", "Institution_Id", "dbo.Institution", "Id");
            AddForeignKey("dbo.ActivityOrganiser", "LabelSet_Id", "dbo.ItemLabelSet", "Id");
            DropColumn("dbo.ActivityOrganiser", "ParentOrganiser_Id");
            DropTable("dbo.InstitutionActivityOrganiser");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.InstitutionActivityOrganiser",
                c => new
                    {
                        Institution_Id = c.Guid(nullable: false),
                        ActivityOrganiser_Id = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.Institution_Id, t.ActivityOrganiser_Id });
            
            AddColumn("dbo.ActivityOrganiser", "ParentOrganiser_Id", c => c.Guid());
            DropForeignKey("dbo.ActivityOrganiser", "LabelSet_Id", "dbo.ItemLabelSet");
            DropForeignKey("dbo.ActivityOrganiser", "Institution_Id", "dbo.Institution");
            DropForeignKey("dbo.Institution", "LabelSet_Id", "dbo.ItemLabelSet");
            DropIndex("dbo.Institution", new[] { "LabelSet_Id" });
            DropIndex("dbo.ActivityOrganiser", new[] { "LabelSet_Id" });
            DropIndex("dbo.ActivityOrganiser", new[] { "Institution_Id" });
            DropColumn("dbo.Institution", "LabelSet_Id");
            DropColumn("dbo.ActivityOrganiser", "LabelSet_Id");
            DropColumn("dbo.ActivityOrganiser", "Institution_Id");
            DropColumn("dbo.OrganiserMember", "IsInstitutionAdmin");
            CreateIndex("dbo.InstitutionActivityOrganiser", "ActivityOrganiser_Id");
            CreateIndex("dbo.InstitutionActivityOrganiser", "Institution_Id");
            CreateIndex("dbo.ActivityOrganiser", "ParentOrganiser_Id");
            AddForeignKey("dbo.ActivityOrganiser", "ParentOrganiser_Id", "dbo.ActivityOrganiser", "Id");
            AddForeignKey("dbo.InstitutionActivityOrganiser", "ActivityOrganiser_Id", "dbo.ActivityOrganiser", "Id", cascadeDelete: true);
            AddForeignKey("dbo.InstitutionActivityOrganiser", "Institution_Id", "dbo.Institution", "Id", cascadeDelete: true);
        }
    }
}
