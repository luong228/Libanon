using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using Libanon.Models;
using Libanon.Models.DTO;
using Libanon.Services.IServices;

namespace Libanon.Services
{
    public class BookService : BaseService, IBookService
    {
        private string libanonUrl;

        public BookService()
        {
            this.libanonUrl = ConfigurationManager.AppSettings["ServiceUrl"];
        }
        public Task<T> GetAllAsync<T>()
        {
            return SendAsync<T>(new ApiRequest()
            {
                ApiType = SD.ApiType.GET,
                Url = libanonUrl,
                RequestUrl = "/api/book"
            });
        }

        public Task<T> GetAsync<T>(int id)
        {
            return SendAsync<T>(new ApiRequest()
            {
                ApiType = SD.ApiType.GET,
                Url = libanonUrl,
                RequestUrl = $"/api/book/{id}"
            });
        }

        public Task<T> CreateAsync<T>(BookCreateDTO createDTO)
        {
            return SendAsync<T>(new ApiRequest()
            {
                ApiType = SD.ApiType.POST,
                Url = libanonUrl,
                RequestUrl = "/api/book/",
                Data = createDTO
            });
        }

        public Task<T> UpdateAsync<T>(BookUpdateDTO updateDTO)
        {
            return SendAsync<T>(new ApiRequest()
            {
                ApiType = SD.ApiType.PUT,
                Url = libanonUrl,
                RequestUrl = $"/api/book/{updateDTO.Id}",
                Data = updateDTO
            });
        }

        public Task<T> DeleteAsync<T>(int id)
        {
            return SendAsync<T>(new ApiRequest()
            {
                ApiType = SD.ApiType.DELETE,
                Url = libanonUrl,
                RequestUrl = $"/api/book/delete/{id}"
            });
        }

        public Task<T> RatingAsync<T>(int id, int rating)
        {
            return SendAsync<T>(new ApiRequest()
            {
                ApiType = SD.ApiType.POST,
                Url = libanonUrl,
                RequestUrl = $"/api/book/rating/{id}",
                Data = new RatingDTO
                {
                    BookId = id,
                    Point = rating
                },
            });
        }

        public Task<T> BorrowAsync<T>(int id, BookBorrowerCreateDTO createDTO)
        {
            return SendAsync<T>(new ApiRequest()
            {
                ApiType = SD.ApiType.POST,
                Url = libanonUrl,
                RequestUrl = $"api/book/borrow/{id}",
                Data =createDTO
            });
        }

        public Task<T> ReturnAsync<T>(int id, BookBorrowerUpdateDTO updateDTO)
        {
            return SendAsync<T>(new ApiRequest()
            {
                ApiType = SD.ApiType.POST,
                Url = libanonUrl,
                RequestUrl = $"api/book/return/{id}",
                Data = updateDTO
            });
        }

        public Task<T> VerifyOtpAsync<T>(OtpCodeDTO otpDTO)
        {
            return SendAsync<T>(new ApiRequest()
            {
                ApiType = SD.ApiType.POST,
                Url = libanonUrl,
                RequestUrl = $"api/book/ConfirmUpdate/{otpDTO.OtpCode}",
                Data = otpDTO
            });
        }
    }
}