using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Libanon_API.Models;
using Libanon_API.Repository.IRepository;

namespace Libanon_API.Repository
{
    public class BookBorrowingRepository : Repository<BookBorrowing>, IBookBorrowingRepository
    {
        public ApplicationDbContext Db => _db;
        public BookBorrowingRepository(ApplicationDbContext db) : base(db)
        {
        }

        public void Update()
        {
            throw new NotImplementedException();
        }

        public void SetReceived(int id)
        {
            var entity = Db.BookBorrowings.FirstOrDefault(br => br.Id == id);
            if (entity != null) entity.IsReceived = true;
        }

        public void SetDelivered(int id)
        {
            var entity = Db.BookBorrowings.FirstOrDefault(br => br.Id == id);
            if (entity != null) entity.IsGiven = true;
        }

        public bool IsBorrowed(int id)
        {
            var entity = Db.BookBorrowings.FirstOrDefault(br => br.Id == id);
            if (entity != null)
            {
                return entity.IsGiven && entity.IsReceived;
            }

            return false;

        }
    }
}