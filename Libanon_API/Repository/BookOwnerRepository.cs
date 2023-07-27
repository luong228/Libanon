using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Libanon_API.Models;
using Libanon_API.Repository.IRepository;

namespace Libanon_API.Repository
{
    public class BookOwnerRepository : Repository<BookOwner>, IBookOwnerRepository
    {
        public ApplicationDbContext Db => _db;
        public BookOwnerRepository(ApplicationDbContext db) : base(db)
        {
        }

        public void Update(BookOwner bookOwner)
        {
            var fromDb = Db.BookOwners.FirstOrDefault(o => o.Id == bookOwner.Id);
            if (fromDb == null)
                throw new NullReferenceException();

            fromDb.Name = bookOwner.Name;
            fromDb.PhoneNumber = bookOwner.PhoneNumber;
            fromDb.Email = bookOwner.Email;
        }

        public int GetOwnerId(string email)
        {
            if(email == null) throw new NullReferenceException();
            var owner = _db.BookOwners.FirstOrDefault(o => o.Email == email);
            return owner?.Id ?? 0;
        }
        public int GetBorrowerId(string email)
        {
            if (email == null) throw new NullReferenceException();
            var borrower = _db.BookBorrowers.FirstOrDefault(br => br.Email == email);
            return borrower?.Id ?? 0;
        }
    }
}