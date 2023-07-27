namespace Libanon_API.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddISBNColumnToBookRatingTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.BookRatings", "ISBN", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.BookRatings", "ISBN");
        }
    }
}
