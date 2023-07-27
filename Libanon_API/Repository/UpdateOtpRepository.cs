using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Libanon_API.Models;
using Libanon_API.Repository.IRepository;

namespace Libanon_API.Repository
{
    public class UpdateOtpRepository : Repository<UpdateOtp>, IUpdateOtpRepository
    {
        private ApplicationDbContext Db => _db;

        public UpdateOtpRepository(ApplicationDbContext db) : base(db)
        {
        }

        public void UpdateOtp()
        {
            throw new NotImplementedException();
        }
    }
}