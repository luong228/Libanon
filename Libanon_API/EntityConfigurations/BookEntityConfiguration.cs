using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;
using Libanon_API.Models;

namespace Libanon_API.EntityConfigurations
{
    public class BookEntityConfiguration : EntityTypeConfiguration<Book>
    {
        public BookEntityConfiguration()
        {
            HasKey(b => b.Id);

            HasMany(b => b.BookBorrowings)
                .WithRequired(br => br.Book)
                .HasForeignKey(br => br.BookId);

        }
    }
}