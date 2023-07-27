using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Libanon_API.Utility
{
    public interface IJwtHandler
    {
        string EncodeJwtToken(Claim[] claims);
        IDictionary<string, string> DecodeJwtToken(string  jwtToken);
    }
}
