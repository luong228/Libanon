using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Libanon_API.Models
{
    public class BookBorrowing
    {
        public int Id { get; set; }
        public bool IsGiven { get; set; } = false;
        public bool IsReceived { get; set; } = false;
        public bool IsReturned { get; set; } = false;
        public bool IsOwnerReceived { get; set; } = false;
        [ForeignKey("Borrower")]
        public int BorrowerId { get; set; }
        public virtual BookBorrower Borrower { get; set; }
        [ForeignKey("Book")]
        public int BookId { get; set; }
        public virtual Book Book { get; set; }

    }
}