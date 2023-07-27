using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Web;
using Libanon_API.Models;
using Libanon_API.Repository.IRepository;

namespace Libanon_API.Repository
{
    public class BookBorrowerRepository : Repository<BookBorrower>, IBookBorrowerRepository
    {
        public ApplicationDbContext Db => _db;
        //private readonly ApplicationDbContext _db;
        public BookBorrowerRepository(ApplicationDbContext db) : base(db)
        {
            //_db = db;
        }

        public void Update(BookBorrower bookBorrower)
        {
            var fromDb = Db.BookBorrowers.FirstOrDefault(o => o.Id == bookBorrower.Id);
            if (fromDb == null)
                throw new NullReferenceException();

            fromDb.Name = bookBorrower.Name;
            fromDb.PhoneNumber = bookBorrower.PhoneNumber;
            fromDb.Email = bookBorrower.Email;
        }


    }
}