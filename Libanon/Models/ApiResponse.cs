using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;

namespace Libanon.Models
{
    public class ApiResponse
    {
        public HttpStatusCode StatusCode { get; set; }
        public bool IsSuccess { get; set; }
        public string Result { get; set; }
    }
}