using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static Libanon.Models.SD;

namespace Libanon.Models
{
    public class ApiRequest
    {
        public ApiType ApiType { get; set; } = ApiType.GET;
        public string Url { get; set; }
        public string RequestUrl { get; set; }
        public object Data { get; set; }
    }
}