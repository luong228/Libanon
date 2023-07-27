namespace Libanon_API.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class init : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.BookBorrowers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        PhoneNumber = c.String(),
                        Email = c.String(nullable: false, maxLength: 255),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Email, unique: true);
            
            CreateTable(
                "dbo.Books",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ISBN = c.String(nullable: false),
                        Title = c.String(),
                        Category = c.String(),
                        Description = c.String(),
                        ImageUrl = c.String(),
                        Author = c.String(),
                        IssuedYear = c.String(),
                        IsAvailable = c.Boolean(nullable: false),
                        OwnerId = c.Int(nullable: false),
                        BorrowerId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.BookOwners", t => t.OwnerId, cascadeDelete: true)
                .ForeignKey("dbo.BookBorrowers", t => t.BorrowerId)
                .Index(t => t.OwnerId)
                .Index(t => t.BorrowerId);
            
            CreateTable(
                "dbo.BookOwners",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        PhoneNumber = c.String(),
                        Email = c.String(nullable: false, maxLength: 255),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Email, unique: true);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Books", "BorrowerId", "dbo.BookBorrowers");
            DropForeignKey("dbo.Books", "OwnerId", "dbo.BookOwners");
            DropIndex("dbo.BookOwners", new[] { "Email" });
            DropIndex("dbo.Books", new[] { "BorrowerId" });
            DropIndex("dbo.Books", new[] { "OwnerId" });
            DropIndex("dbo.BookBorrowers", new[] { "Email" });
            DropTable("dbo.BookOwners");
            DropTable("dbo.Books");
            DropTable("dbo.BookBorrowers");
        }
    }
}
