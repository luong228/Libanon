using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Libanon.Models.DTO
{
    public class RatingDTO
    {
        public int Id { get; set; }
        public int BookId { get; set; }
        public int Point
        {
            get;
            set;
        }
    }
}