using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Libanon_API.Models.DTO
{
    public class RatingCreateDTO
    {
        public int BookId { get; set; }
        public int Point
        {
            get;
            set;
        }
    }
}