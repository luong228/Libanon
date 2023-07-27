using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;
using Libanon_API.Models;

namespace Libanon_API.EntityConfigurations
{
    public class BookRatingEntityConfiguration : EntityTypeConfiguration<BookRating>
    {
        public BookRatingEntityConfiguration()
        {
            HasKey(r => r.Id);

            HasRequired(r => r.Book)
                .WithMany(b => b.BookRatings)
                .HasForeignKey(r => r.BookID);
        }
    }
}