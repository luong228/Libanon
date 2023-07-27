using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Design;
using System.Linq;
using System.Web;

namespace Libanon_API.Models
{
    public class Book
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string ISBN { get; set; }
        public string Title { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public string Author { get; set; }
        public string IssuedYear { get; set; }
        public bool IsAvailable { get; set; } = false;

        public double AverageRating { get; set; } = 0;
        // Navigation
        [Required]
        [ForeignKey("Owner")]
        public int OwnerId { get; set; }
        public virtual BookOwner Owner { get; set; }
        [ForeignKey("Borrower")]
        public int? BorrowerId { get; set; }

        public virtual BookBorrower Borrower { get; set; } = null;
        public virtual ICollection<BookBorrowing> BookBorrowings { get; set; } = null;
        public virtual ICollection<BookRating> BookRatings { get; set; }

    }
}