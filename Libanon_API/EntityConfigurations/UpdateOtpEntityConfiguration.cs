using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;
using Libanon_API.Models;

namespace Libanon_API.EntityConfigurations
{
    public class UpdateOtpEntityConfiguration : EntityTypeConfiguration<UpdateOtp>
    {
        public UpdateOtpEntityConfiguration()
        {
            HasKey(o => o.OtpCode);

            Property(o => o.OtpCode).IsRequired().HasMaxLength(255);

        }
    }
}