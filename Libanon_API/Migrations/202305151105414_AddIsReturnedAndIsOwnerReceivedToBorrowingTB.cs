namespace Libanon_API.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddIsReturnedAndIsOwnerReceivedToBorrowingTB : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.BookBorrowings", "IsReturned", c => c.Boolean(nullable: false));
            AddColumn("dbo.BookBorrowings", "IsOwnerReceived", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.BookBorrowings", "IsOwnerReceived");
            DropColumn("dbo.BookBorrowings", "IsReturned");
        }
    }
}
