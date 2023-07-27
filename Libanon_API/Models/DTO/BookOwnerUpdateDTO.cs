using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Libanon_API.Models.DTO
{
    public class BookOwnerUpdateDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
    }
}