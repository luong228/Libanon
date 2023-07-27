﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Libanon.Models.DTO
{
    public class BookUpdateDTO
    {
        public int Id { get; set; }
        public string ISBN { get; set; }
        public string Title { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public string Author { get; set; }
        public string IssuedYear { get; set; }
    }
}