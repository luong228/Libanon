using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Libanon_API.Models.DTO
{
    public class BookBorrowingDTO
    {
        public int Id { get; set; }
        public bool IsGiven { get; set; }
        public bool IsReceived { get; set; }
        public virtual BookBorrowerDTO Borrower { get; set; }
        public virtual BookDTO Book { get; set; }
    }
}