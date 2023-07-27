namespace Libanon_API.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddBookBorrowingsTableToDb : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.BookBorrowings",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        IsAccepted = c.Boolean(nullable: false),
                        IsGiven = c.Boolean(nullable: false),
                        IsReceived = c.Boolean(nullable: false),
                        BorrowerId = c.Int(nullable: false),
                        BookId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Books", t => t.BookId, cascadeDelete: true)
                .ForeignKey("dbo.BookBorrowers", t => t.BorrowerId, cascadeDelete: true)
                .Index(t => t.BorrowerId)
                .Index(t => t.BookId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.BookBorrowings", "BorrowerId", "dbo.BookBorrowers");
            DropForeignKey("dbo.BookBorrowings", "BookId", "dbo.Books");
            DropIndex("dbo.BookBorrowings", new[] { "BookId" });
            DropIndex("dbo.BookBorrowings", new[] { "BorrowerId" });
            DropTable("dbo.BookBorrowings");
        }
    }
}
