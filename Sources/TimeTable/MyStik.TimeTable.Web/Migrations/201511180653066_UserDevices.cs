namespace MyStik.TimeTable.Web.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class UserDevices : DbMigration
    {
        /// <summary>
        /// 
        /// </summary>
        public override void Up()
        {
            CreateTable(
                "dbo.UserDevices",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        DeviceId = c.String(),
                        DeviceName = c.String(),
                        Platform = c.Int(nullable: false),
                        Registered = c.DateTime(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        User_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.User_Id)
                .Index(t => t.User_Id);
            
        }
        
        /// <summary>
        /// 
        /// </summary>
        public override void Down()
        {
            DropForeignKey("dbo.UserDevices", "User_Id", "dbo.AspNetUsers");
            DropIndex("dbo.UserDevices", new[] { "User_Id" });
            DropTable("dbo.UserDevices");
        }
    }
}
