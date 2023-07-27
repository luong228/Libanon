using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Libanon.Models;

namespace Libanon.Services.IServices
{
    public interface IBaseService
    {
        Task<T> SendAsync<T>(ApiRequest request);
    }
}
