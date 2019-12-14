namespace CompileError.DatabaseContext.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LoyalityPointChange : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Customers", "LoyalityPoint", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Customers", "LoyalityPoint", c => c.Single(nullable: false));
        }
    }
}
