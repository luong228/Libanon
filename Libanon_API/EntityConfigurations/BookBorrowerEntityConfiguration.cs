using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;
using Libanon_API.Models;

namespace Libanon_API.EntityConfigurations
{
    public class BookBorrowerEntityConfiguration : EntityTypeConfiguration<BookBorrower>
    {
        public BookBorrowerEntityConfiguration()
        {
            Property(br => br.Email).IsRequired().HasMaxLength(255);
            HasIndex(br => br.Email)
                .IsUnique();

            HasKey(br => br.Id);


            HasMany(br => br.Books)
                .WithOptional(b => b.Borrower)
                .HasForeignKey(b => b.BorrowerId);

            HasMany(br => br.BookBorrowings)
                .WithRequired(brg => brg.Borrower)
                .HasForeignKey(brg => brg.BorrowerId);
        }
    }
}