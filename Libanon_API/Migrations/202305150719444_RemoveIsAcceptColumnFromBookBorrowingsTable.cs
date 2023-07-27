namespace Libanon_API.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveIsAcceptColumnFromBookBorrowingsTable : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.BookBorrowings", "IsAccepted");
        }
        
        public override void Down()
        {
            AddColumn("dbo.BookBorrowings", "IsAccepted", c => c.Boolean(nullable: false));
        }
    }
}
