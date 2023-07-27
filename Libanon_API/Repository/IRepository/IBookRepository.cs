using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Libanon_API.Models;

namespace Libanon_API.Repository.IRepository
{
    public interface IBookRepository : IRepository<Book>
    {
        void Update(Book book);
        void SetBorrow(int id, BookBorrower borrower);
        void ReleaseBorrow(int id);
    }
}
