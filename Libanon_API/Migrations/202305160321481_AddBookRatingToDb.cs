namespace Libanon_API.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddBookRatingToDb : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.BookRatings",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Point = c.Int(nullable: false),
                        BookID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Books", t => t.BookID, cascadeDelete: true)
                .Index(t => t.BookID);
            
            AddColumn("dbo.Books", "TotalRating", c => c.Single(nullable: false));
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.BookRatings", "BookID", "dbo.Books");
            DropIndex("dbo.BookRatings", new[] { "BookID" });
            DropColumn("dbo.Books", "TotalRating");
            DropTable("dbo.BookRatings");
        }
    }
}
