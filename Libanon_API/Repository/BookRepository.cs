using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Libanon_API.Models;
using Libanon_API.Repository.IRepository;

namespace Libanon_API.Repository
{
    public class BookRepository : Repository<Book>, IBookRepository
    {
        private  ApplicationDbContext Db => _db;
        public BookRepository(ApplicationDbContext db) : base(db)
        {
        }

        public void Update(Book book)
        {
            var fromDb = Db.Books.FirstOrDefault(b => b.Id == book.Id);
            if (fromDb == null)
                throw new NullReferenceException();
            fromDb.Author = book.Author;
            fromDb.Title = book.Title;
            fromDb.Description = book.Description;
            fromDb.ISBN = book.ISBN;
            fromDb.ImageUrl = book.ImageUrl;
            fromDb.IssuedYear = book.IssuedYear;
            fromDb.Category = book.Category;
        }

        public void SetBorrow(int id, BookBorrower borrower)
        {
            var fromDb = Db.Books.FirstOrDefault(b => b.Id == id);
            if (fromDb == null)
                throw new NullReferenceException();
            fromDb.Borrower = borrower;
        }

        public void ReleaseBorrow(int id)
        {
            var fromDb = Db.Books.FirstOrDefault(b => b.Id == id);
            if (fromDb == null)
                throw new NullReferenceException();
            fromDb.Borrower = null;
        }
    }
}