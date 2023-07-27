using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Libanon_API.Models
{
    public class BookBorrower
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        [Required]
        public string Email { get; set; }
        public virtual ICollection<Book> Books { get; set; }
        public virtual ICollection<BookBorrowing> BookBorrowings { get; set; } = null;
    }
}