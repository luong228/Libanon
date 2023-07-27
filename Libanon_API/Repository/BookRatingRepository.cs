using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Libanon_API.Models;
using Libanon_API.Repository.IRepository;

namespace Libanon_API.Repository
{
    public class BookRatingRepository : Repository<BookRating>, IBookRatingRepository
    {
        private ApplicationDbContext Db => _db;

        public BookRatingRepository(ApplicationDbContext db) : base(db)
        {
        }

        public void Update()
        {
            throw new NotImplementedException();
        }
    }
}