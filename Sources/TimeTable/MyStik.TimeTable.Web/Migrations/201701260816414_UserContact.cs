namespace MyStik.TimeTable.Web.Migrations
{
    using System.Data.Entity.Migrations;
    
    /// <summary>
    /// 
    /// </summary>
    public partial class UserContact : DbMigration
    {
        /// <summary>
        /// 
        /// </summary>
        public override void Up()
        {
            CreateTable(
                "dbo.UserContacts",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        EMail = c.String(),
                        IsValidated = c.Boolean(nullable: false),
                        LastErrorMessage = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        /// <summary>
        /// 
        /// </summary>
        public override void Down()
        {
            DropTable("dbo.UserContacts");
        }
    }
}
