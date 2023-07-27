using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;
using Libanon_API.Models;

namespace Libanon_API.EntityConfigurations
{
    public class BookBorrowingEntityConfiguration : EntityTypeConfiguration<BookBorrowing>
    {
        public BookBorrowingEntityConfiguration()
        {
            HasKey(br => br.Id);

        }
    }
}