namespace MyStik.TimeTable.Web.Migrations
{
    using System.Data.Entity.Migrations;
    
    /// <summary>
    /// 
    /// </summary>
    public partial class UserProfile : DbMigration
    {
        /// <summary>
        /// 
        /// </summary>
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "IsAnonymous", c => c.Boolean());
            AddColumn("dbo.AspNetUsers", "LikeEMails", c => c.Boolean());
            AddColumn("dbo.AspNetUsers", "MemberState", c => c.Int());

            Sql("UPDATE dbo.AspNetUsers SET IsAnonymous = 'True'");
            Sql("UPDATE dbo.AspNetUsers SET LikeEMails = 'False'");
            Sql("UPDATE dbo.AspNetUsers SET MemberState = 1");

        }
        
        /// <summary>
        /// 
        /// </summary>
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "MemberState");
            DropColumn("dbo.AspNetUsers", "LikeEMails");
            DropColumn("dbo.AspNetUsers", "IsAnonymous");
        }
    }
}
