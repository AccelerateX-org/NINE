namespace MyStik.TimeTable.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ScriptShop_v01 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ScriptDocument",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        UserId = c.String(),
                        Created = c.DateTime(nullable: false),
                        Title = c.String(),
                        Version = c.String(),
                        Storage_Id = c.Guid(),
                        Course_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.BinaryStorage", t => t.Storage_Id)
                .ForeignKey("dbo.Activity", t => t.Course_Id)
                .Index(t => t.Storage_Id)
                .Index(t => t.Course_Id);
            
            CreateTable(
                "dbo.ScriptOrder",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        OrderBasket_Id = c.Guid(),
                        ScriptDocument_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.OrderBasket", t => t.OrderBasket_Id)
                .ForeignKey("dbo.ScriptDocument", t => t.ScriptDocument_Id)
                .Index(t => t.OrderBasket_Id)
                .Index(t => t.ScriptDocument_Id);
            
            CreateTable(
                "dbo.OrderBasket",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        UserId = c.String(),
                        OrderNumber = c.Int(),
                        OrderPeriod_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.OrderPeriod", t => t.OrderPeriod_Id)
                .Index(t => t.OrderPeriod_Id);
            
            CreateTable(
                "dbo.OrderPeriod",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Description = c.String(),
                        Begin = c.DateTime(nullable: false),
                        End = c.DateTime(nullable: false),
                        LastProcessed = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ScriptPublishing",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Published = c.DateTime(nullable: false),
                        Course_Id = c.Guid(),
                        ScriptDocument_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Activity", t => t.Course_Id)
                .ForeignKey("dbo.ScriptDocument", t => t.ScriptDocument_Id)
                .Index(t => t.Course_Id)
                .Index(t => t.ScriptDocument_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ScriptDocument", "Course_Id", "dbo.Activity");
            DropForeignKey("dbo.ScriptDocument", "Storage_Id", "dbo.BinaryStorage");
            DropForeignKey("dbo.ScriptPublishing", "ScriptDocument_Id", "dbo.ScriptDocument");
            DropForeignKey("dbo.ScriptPublishing", "Course_Id", "dbo.Activity");
            DropForeignKey("dbo.ScriptOrder", "ScriptDocument_Id", "dbo.ScriptDocument");
            DropForeignKey("dbo.ScriptOrder", "OrderBasket_Id", "dbo.OrderBasket");
            DropForeignKey("dbo.OrderBasket", "OrderPeriod_Id", "dbo.OrderPeriod");
            DropIndex("dbo.ScriptPublishing", new[] { "ScriptDocument_Id" });
            DropIndex("dbo.ScriptPublishing", new[] { "Course_Id" });
            DropIndex("dbo.OrderBasket", new[] { "OrderPeriod_Id" });
            DropIndex("dbo.ScriptOrder", new[] { "ScriptDocument_Id" });
            DropIndex("dbo.ScriptOrder", new[] { "OrderBasket_Id" });
            DropIndex("dbo.ScriptDocument", new[] { "Course_Id" });
            DropIndex("dbo.ScriptDocument", new[] { "Storage_Id" });
            DropTable("dbo.ScriptPublishing");
            DropTable("dbo.OrderPeriod");
            DropTable("dbo.OrderBasket");
            DropTable("dbo.ScriptOrder");
            DropTable("dbo.ScriptDocument");
        }
    }
}
