using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Libanon_API.Models;

namespace Libanon_API.Repository.IRepository
{
    public interface IBookOwnerRepository : IRepository<BookOwner>
    {
        void Update(BookOwner bookOwner);
        int GetOwnerId(string email);

    }
}
