namespace MyStik.TimeTable.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MiscV2 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.RoomEquipment",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Amount = c.Int(nullable: false),
                        Title = c.String(),
                        Description = c.String(),
                        IsImmobile = c.Boolean(nullable: false),
                        Room_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Room", t => t.Room_Id)
                .Index(t => t.Room_Id);
            
            AddColumn("dbo.BinaryStorage", "AccessCount", c => c.Int(nullable: false));
            AddColumn("dbo.BinaryStorage", "Room_Id", c => c.Guid());
            AddColumn("dbo.Room", "IsForLearning", c => c.Boolean(nullable: false));
            AddColumn("dbo.Room", "HasAccessControl", c => c.Boolean(nullable: false));
            AddColumn("dbo.Occurrence", "HasHomeBias", c => c.Boolean(nullable: false));
            AddColumn("dbo.Occurrence", "IsCoterie", c => c.Boolean(nullable: false));
            AddColumn("dbo.Lottery", "MaxExceptionConfirm", c => c.Int(nullable: false));
            AddColumn("dbo.Lottery", "ExceptionRemark", c => c.String());
            AddColumn("dbo.Lottery", "MinSubscription", c => c.Int(nullable: false));
            CreateIndex("dbo.BinaryStorage", "Room_Id");
            AddForeignKey("dbo.BinaryStorage", "Room_Id", "dbo.Room", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.BinaryStorage", "Room_Id", "dbo.Room");
            DropForeignKey("dbo.RoomEquipment", "Room_Id", "dbo.Room");
            DropIndex("dbo.RoomEquipment", new[] { "Room_Id" });
            DropIndex("dbo.BinaryStorage", new[] { "Room_Id" });
            DropColumn("dbo.Lottery", "MinSubscription");
            DropColumn("dbo.Lottery", "ExceptionRemark");
            DropColumn("dbo.Lottery", "MaxExceptionConfirm");
            DropColumn("dbo.Occurrence", "IsCoterie");
            DropColumn("dbo.Occurrence", "HasHomeBias");
            DropColumn("dbo.Room", "HasAccessControl");
            DropColumn("dbo.Room", "IsForLearning");
            DropColumn("dbo.BinaryStorage", "Room_Id");
            DropColumn("dbo.BinaryStorage", "AccessCount");
            DropTable("dbo.RoomEquipment");
        }
    }
}
