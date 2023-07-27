using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Libanon_API.Models.DTO
{
    public class BookBorrowingCreateDTO
    {
        public bool IsGiven { get; set; }
        public bool IsReceived { get; set; }
        public int BorrowerId { get; set; }
        public int BookId { get; set; }
    }
}