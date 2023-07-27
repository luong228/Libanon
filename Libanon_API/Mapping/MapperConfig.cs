using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AutoMapper;
using Libanon_API.Mapping.Resolves;
using Libanon_API.Models;
using Libanon_API.Models.DTO;
using Microsoft.Ajax.Utilities;

namespace Libanon_API
{
    public class MapperConfig : Profile
    {
        public MapperConfig()
        {
            CreateMap<Book, BookCreateDTO>().ReverseMap().ForMember(dest => dest.Owner, opt => opt.MapFrom<OwnerResolver>());
            CreateMap<Book, BookUpdateDTO>().ReverseMap();
            CreateMap<Book, BookDTO>().ReverseMap();

            CreateMap<BookOwner, BookOwnerDTO>().ReverseMap();
            CreateMap<BookOwner, BookOwnerUpdateDTO>().ReverseMap();
            CreateMap<BookOwner, BookOwnerCreateDTO>().ReverseMap();

            CreateMap<BookBorrower, BookBorrowerDTO>().ReverseMap();
            CreateMap<BookBorrower, BookBorrowerCreateDTO>().ReverseMap();
            CreateMap<BookBorrower, BookBorrowerUpdateDTO>().ReverseMap();

            CreateMap<Book, UpdateOtp>().ForMember(src => src.BookId, otp => otp.MapFrom(dest => dest.Id));
            CreateMap<UpdateOtp, Book>().ForMember(src => src.Id, otp => otp.MapFrom(dest => dest.BookId));
        }
    }
}