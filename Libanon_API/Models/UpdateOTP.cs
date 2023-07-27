using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Org.BouncyCastle.Bcpg;

namespace Libanon_API.Models
{
    public class UpdateOtp
    {
        public string OtpCode { get; set; }
        public int BookId { get; set; }
        public string ISBN { get; set; }
        public string Title { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public string Author { get; set; }
        public string IssuedYear { get; set; }
    }
}