using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AutoMapper;
using Libanon_API.Models;
using Libanon_API.Models.DTO;

namespace Libanon_API.Mapping.Resolves
{
    public class OwnerResolver : IValueResolver<BookCreateDTO, Book, BookOwner>
    {
        public BookOwner Resolve(BookCreateDTO source, Book destination, BookOwner destMember, ResolutionContext context)
        {
            return new BookOwner()
            {
                Name = source.OwnerName,
                PhoneNumber = source.OwnerPhone,
                Email = source.OwnerEmail,
            };
        }
    }
}