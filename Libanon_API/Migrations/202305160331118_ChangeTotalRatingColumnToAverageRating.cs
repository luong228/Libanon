namespace Libanon_API.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangeTotalRatingColumnToAverageRating : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Books", "AverageRating", c => c.Double(nullable: false));
            DropColumn("dbo.Books", "TotalRating");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Books", "TotalRating", c => c.Single(nullable: false));
            DropColumn("dbo.Books", "AverageRating");
        }
    }
}
