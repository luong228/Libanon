using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Libanon.Models.DTO;

namespace Libanon.Services.IServices
{
    public interface IBookService
    {
        Task<T> GetAllAsync<T>();
        Task<T> GetAsync<T>(int id);
        Task<T> CreateAsync<T>(BookCreateDTO createDTO);
        Task<T> UpdateAsync<T>(BookUpdateDTO updateDTO);
        Task<T> DeleteAsync<T>(int id);
        Task<T> RatingAsync<T>(int id, int rating);
        Task<T> BorrowAsync<T>(int id, BookBorrowerCreateDTO createDTO);
        Task<T> ReturnAsync<T>(int id, BookBorrowerUpdateDTO updateDTO);
        Task<T> VerifyOtpAsync<T>(OtpCodeDTO otpDTO);
    }
}
