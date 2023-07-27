using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Libanon_API.Models
{
    public class BookRating
    {
        public int Id { get; set; }
        public int Point { get; set; }
        [ForeignKey("Book")]
        public int BookID { get; set; }
        public string ISBN { get; set; }
        public Book Book { get; set; }
    }
}