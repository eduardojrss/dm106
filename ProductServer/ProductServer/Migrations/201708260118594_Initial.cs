namespace ProductServer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.OrderItems", "quantidade");
        }
        
        public override void Down()
        {
            AddColumn("dbo.OrderItems", "quantidade", c => c.Int(nullable: false));
        }
    }
}
