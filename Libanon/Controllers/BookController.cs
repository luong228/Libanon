using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Results;
using System.Web.Mvc;
using Libanon.Models;
using Libanon.Models.DTO;
using Libanon.Services.IServices;
using Newtonsoft.Json;

namespace Libanon.Controllers
{
    public class BookController : Controller
    {
        private readonly IBookService _bookService;

        public BookController(IBookService bookService)
        {
            _bookService = bookService;
        }
        public async Task<ActionResult> Index()
        {
            var books = new List<BookDTO>();
            var apiRes = await _bookService.GetAllAsync<ApiResponse>();
            if (apiRes != null && apiRes.IsSuccess)
            {
                books = JsonConvert.DeserializeObject<List<BookDTO>>(apiRes.Result);
            }
            return View(books);
        }
        public async Task<ActionResult> Create()
        {
            return View();
        }

        [System.Web.Mvc.HttpPost]
        public async Task<ActionResult> Create([Bind(Exclude = "Id")] BookCreateDTO  createDTO)
        {
            if (!ModelState.IsValid)
                return View(createDTO);
            var apiRes = await _bookService.CreateAsync<ApiResponse>(createDTO);
            if (apiRes != null && apiRes.IsSuccess)
            {
                HttpContext.Session.Add("success", "Book created request send your email");
                return RedirectToAction("Index");
            }
            return View(createDTO);
            
        }
        public async Task<ActionResult> Details(int id)
        {
            var apiRes = await _bookService.GetAsync<ApiResponse>(id);
            if (apiRes != null && apiRes.IsSuccess)
            {
                var book = JsonConvert.DeserializeObject<BookDTO>(apiRes.Result);
                return View(book);
            }
            return HttpNotFound();
        }

        [System.Web.Http.HttpPost]
        public async Task<ActionResult> Borrow(int id, BookBorrowerCreateDTO createDTO)
        {
            if (!ModelState.IsValid)
            {
                HttpContext.Session.Add("error", "Borrow book failed");
                return Json("error");

            }
            var apiRes = await _bookService.BorrowAsync<ApiResponse>(id, createDTO);
            if (apiRes != null && apiRes.IsSuccess)
            {
                HttpContext.Session.Add("success", "Borrow book request sent successfully");
                return Json("hehe");
            }
            HttpContext.Session.Add("error", "Borrow book failed");
            return Json("hehe");
        }
        [System.Web.Http.HttpPost]
        public async Task<ActionResult> Rating([FromBody]RatingViewModel model)
        {
            var apiRes = await _bookService.RatingAsync<ApiResponse>(model.BookId, model.Rating);
            if (apiRes != null && apiRes.IsSuccess)
            {
                HttpContext.Session.Add("success", "Rating book successfully");
                return Json("hehe");
            }
            HttpContext.Session.Add("Error", "Rating book failed");
            return Json("hehe");
        }
        [System.Web.Http.HttpPost]
        public async Task<ActionResult> Return(int id, [FromBody] BookBorrowerUpdateDTO updateDTO)
        {
            var apiRes = await _bookService.ReturnAsync<ApiResponse>(id, updateDTO);
            if (apiRes != null && apiRes.IsSuccess)
            {
                HttpContext.Session.Add("success", "Return Request send your email successfully");
                return Json("hehe");
            }
            HttpContext.Session.Add("Error", "Return  book failed");
            return Json("hehe");
        }
        public async Task<ActionResult> Edit(int id)
        {
            var apiRes = await _bookService.GetAsync<ApiResponse>(id);
            if (apiRes != null && apiRes.IsSuccess)
            {
                var book = JsonConvert.DeserializeObject<BookUpdateDTO>(apiRes.Result);
                return View(book);
            }
            return HttpNotFound();
        }

        [System.Web.Mvc.HttpPost]
        public async Task<ActionResult> Edit(BookUpdateDTO updateDTO)
        {
            if (!ModelState.IsValid)
                return View(updateDTO);
            var apiRes = await _bookService.UpdateAsync<ApiResponse>(updateDTO);
            if (apiRes != null && apiRes.IsSuccess)
            {
                HttpContext.Session.Add("success", "Please input OTP to confirm");
                return RedirectToAction("VerifyOtp");
            }
            HttpContext.Session.Add("error", "An error occurred when editing your book");
            return View(updateDTO);
           
        }
        public async Task<ActionResult> Delete()
        {
            return View();
        }
        public async Task<ActionResult> VerifyOtp()
        {
            return View();
        }
        [System.Web.Mvc.HttpPost]
        public async Task<ActionResult> VerifyOtp(OtpCodeDTO otpDTO)
        {
            if (!ModelState.IsValid)
                return View(otpDTO);

            var apiRes = await _bookService.VerifyOtpAsync<ApiResponse>(otpDTO);
            if (apiRes != null && apiRes.IsSuccess)
            {
                HttpContext.Session.Add("success", "Your book updated successfully");
                return RedirectToAction("Index");
            }
            HttpContext.Session.Add("error", "Invalid OTP");
            return View(otpDTO);
        }

        [System.Web.Mvc.HttpGet]
        public async Task<ActionResult> Delete(int id)
        {
            var apiRes = await _bookService.DeleteAsync<ApiResponse>(id);
            if (apiRes != null && apiRes.IsSuccess)
            {
                HttpContext.Session.Add("success", "Deleted book request sent successfully");
                return RedirectToAction("Index");
            }
            HttpContext.Session.Add("error", "Error occurred when deleting");
            return RedirectToAction("Index");
        }

    }
}