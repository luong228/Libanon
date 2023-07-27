using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using Libanon_API.EntityConfigurations;
using Libanon_API.Models;

namespace Libanon_API
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext() : base("name=DefaultConnection")
        {
        }
        public DbSet<Book> Books { get; set; }
        public DbSet<BookOwner> BookOwners { get; set; }
        public DbSet<BookBorrower> BookBorrowers { get; set; }
        public DbSet<UpdateOtp> UpdateOtps { get; set; }
        public DbSet<BookBorrowing> BookBorrowings { get; set; }
        public DbSet<BookRating> BookRatings { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new BookOwnerEntityConfiguration());
            modelBuilder.Configurations.Add(new BookBorrowerEntityConfiguration());
            modelBuilder.Configurations.Add(new BookEntityConfiguration());
            modelBuilder.Configurations.Add(new UpdateOtpEntityConfiguration());
            modelBuilder.Configurations.Add(new BookBorrowingEntityConfiguration());
            modelBuilder.Configurations.Add(new BookRatingEntityConfiguration());
        }
    }
}