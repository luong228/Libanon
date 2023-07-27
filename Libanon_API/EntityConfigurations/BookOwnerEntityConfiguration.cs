using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;
using Libanon_API.Models;

namespace Libanon_API.EntityConfigurations
{
    public class BookOwnerEntityConfiguration : EntityTypeConfiguration<BookOwner>
    {
        public BookOwnerEntityConfiguration()
        {
            Property(o => o.Email).IsRequired().HasMaxLength(255);
            HasIndex(o => o.Email)
                .IsUnique();
            HasKey(o => o.Id);


            HasMany(o => o.Books)
                .WithRequired(b => b.Owner)
                .HasForeignKey(b => b.OwnerId);
        }
    }
}