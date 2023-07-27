namespace Libanon_API.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUpdateOtpTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.UpdateOtps",
                c => new
                    {
                        OtpCode = c.String(nullable: false, maxLength: 255),
                        BookId = c.Int(nullable: false),
                        ISBN = c.String(),
                        Title = c.String(),
                        Category = c.String(),
                        Description = c.String(),
                        ImageUrl = c.String(),
                        Author = c.String(),
                        IssuedYear = c.String(),
                    })
                .PrimaryKey(t => t.OtpCode);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.UpdateOtps");
        }
    }
}
