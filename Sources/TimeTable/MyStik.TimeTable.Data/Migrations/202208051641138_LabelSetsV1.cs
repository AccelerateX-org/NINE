namespace MyStik.TimeTable.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LabelSetsV1 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ItemLabel", "Organiser_Id", "dbo.ActivityOrganiser");
            DropIndex("dbo.ItemLabel", new[] { "Organiser_Id" });
            AddColumn("dbo.ModuleAccreditation", "LabelSet_Id", c => c.Guid());
            AddColumn("dbo.Curriculum", "LabelSet_Id", c => c.Guid());
            AddColumn("dbo.Activity", "LabelSet_Id", c => c.Guid());
            AddColumn("dbo.Student", "LabelSet_Id", c => c.Guid());
            CreateIndex("dbo.ModuleAccreditation", "LabelSet_Id");
            CreateIndex("dbo.Curriculum", "LabelSet_Id");
            CreateIndex("dbo.Activity", "LabelSet_Id");
            CreateIndex("dbo.Student", "LabelSet_Id");
            AddForeignKey("dbo.Student", "LabelSet_Id", "dbo.ItemLabelSet", "Id");
            AddForeignKey("dbo.Activity", "LabelSet_Id", "dbo.ItemLabelSet", "Id");
            AddForeignKey("dbo.Curriculum", "LabelSet_Id", "dbo.ItemLabelSet", "Id");
            AddForeignKey("dbo.ModuleAccreditation", "LabelSet_Id", "dbo.ItemLabelSet", "Id");
            DropColumn("dbo.ItemLabel", "Organiser_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ItemLabel", "Organiser_Id", c => c.Guid());
            DropForeignKey("dbo.ModuleAccreditation", "LabelSet_Id", "dbo.ItemLabelSet");
            DropForeignKey("dbo.Curriculum", "LabelSet_Id", "dbo.ItemLabelSet");
            DropForeignKey("dbo.Activity", "LabelSet_Id", "dbo.ItemLabelSet");
            DropForeignKey("dbo.Student", "LabelSet_Id", "dbo.ItemLabelSet");
            DropIndex("dbo.Student", new[] { "LabelSet_Id" });
            DropIndex("dbo.Activity", new[] { "LabelSet_Id" });
            DropIndex("dbo.Curriculum", new[] { "LabelSet_Id" });
            DropIndex("dbo.ModuleAccreditation", new[] { "LabelSet_Id" });
            DropColumn("dbo.Student", "LabelSet_Id");
            DropColumn("dbo.Activity", "LabelSet_Id");
            DropColumn("dbo.Curriculum", "LabelSet_Id");
            DropColumn("dbo.ModuleAccreditation", "LabelSet_Id");
            CreateIndex("dbo.ItemLabel", "Organiser_Id");
            AddForeignKey("dbo.ItemLabel", "Organiser_Id", "dbo.ActivityOrganiser", "Id");
        }
    }
}
