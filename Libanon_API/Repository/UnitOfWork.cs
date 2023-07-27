using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Libanon_API.Repository.IRepository;

namespace Libanon_API.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _db;

        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;
            BookRepository = new BookRepository(_db);
            BorrowerRepository = new BookBorrowerRepository(_db);
            OwnerRepository = new BookOwnerRepository(_db);
            UpdateOtpRepository = new UpdateOtpRepository(_db);
            BorrowingRepository = new BookBorrowingRepository(_db);
            BookRatingRepository = new BookRatingRepository(_db);

        }
        public IBookRepository BookRepository { get; }
        public IBookBorrowerRepository BorrowerRepository { get; }
        public IBookOwnerRepository OwnerRepository { get; }
        public IUpdateOtpRepository UpdateOtpRepository { get; }
        public IBookBorrowingRepository BorrowingRepository { get; }
        public IBookRatingRepository BookRatingRepository { get; }

        public void Save()
        {
           _db.SaveChanges();
        }
    }
}