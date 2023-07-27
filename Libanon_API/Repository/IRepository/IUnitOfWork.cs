using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libanon_API.Repository.IRepository
{
    public interface IUnitOfWork
    {
        IBookRepository BookRepository { get; }
        IBookBorrowerRepository BorrowerRepository { get; }
        IBookOwnerRepository OwnerRepository { get; }
        IUpdateOtpRepository UpdateOtpRepository { get; }
        IBookBorrowingRepository BorrowingRepository { get; }
        IBookRatingRepository BookRatingRepository { get; }
        void Save();
    }
}
