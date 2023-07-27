using System;
using System.Collections.Generic;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Security.Claims;
using System.Text;
using System.Web;
using System.Web.Http;
using AutoMapper;
using Libanon_API.Models;
using Libanon_API.Models.DTO;
using Libanon_API.Repository.IRepository;
using Libanon_API.Utility;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Org.BouncyCastle.Ocsp;
using static System.Int32;

namespace Libanon_API.Controllers
{
    public class BookController : ApiController
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailSender _emailSender;
        private readonly IJwtHandler _jwtHandler;

        public BookController(IUnitOfWork unitOfWork, IMapper mapper, IEmailSender emailSender, IJwtHandler jwtHandler)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _emailSender = emailSender;
            _jwtHandler = jwtHandler;
        }

        #region CRUD
        [Route("api/book/{id}")]
        public IHttpActionResult Get(int id)
        {
            try
            {
                if (id <= 0)
                    return NotFound();

                var book = _unitOfWork.BookRepository.FirstOrDefault(b => b.Id == id);

                if (book == null)
                    return NotFound();

                var bookDTO = _mapper.Map<BookDTO>(book);
                return Ok(bookDTO);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        public IHttpActionResult GetAll()
        {   
            try
            {
                var books = _unitOfWork.BookRepository.GetAll();

                var booksDTO = _mapper.Map<List<BookDTO>>(books);
                return Ok(booksDTO);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        public IHttpActionResult Post([FromBody] BookCreateDTO createDTO)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest();

                var ownerEmail = createDTO.OwnerEmail;
                var ownerId = _unitOfWork.OwnerRepository.GetOwnerId(createDTO.OwnerEmail);
                var bookModel = _mapper.Map<Book>(createDTO);
                if (ownerId != 0)
                {
                    bookModel.OwnerId = ownerId;
                    bookModel.Owner = null;
                }
                //Encode JWT Token

                _unitOfWork.BookRepository.Add(bookModel);
                _unitOfWork.Save();
                var claims = new[]
                {
                    new Claim("bookId", bookModel.Id.ToString())
                };
                var tokenString = _jwtHandler.EncodeJwtToken(claims);
                var urlToConfirm = HttpContext.Current.Request.Url.Authority + "/api/book/confirm/";


                _emailSender.SendEmailAsync(
                    ownerEmail,
                    "Confirm New Book - Libanon",
                    "<p>Please click the button below to confirm your book</p>" +
                    $"<ul><li>{bookModel.Id}</li><li>{bookModel.ISBN}</li><li>{bookModel.Title}</li></ul>" +
                    $"<form action=\"https://{urlToConfirm}\" method=\"POST\">\r\n  " +
                    $"<input type=\"hidden\" id=\"jwtToken\" name=\"jwtToken\" value=\"{tokenString}\" />" +
                    $"\r\n  <button type=\"submit\">Confirm</button>\r\n</form>"
                );
                return Ok("Book created successfully");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [Route("api/book/confirm/")]
        // Add Confirm
        public IHttpActionResult PostConfirmBook([FromBody] JObject data)
        {
            var jsonData = _jwtHandler.DecodeJwtToken(data["jwtToken"].ToString());
            var idInt32 = Convert.ToInt32(jsonData["bookId"]);
            if (idInt32 <= 0)
                return NotFound();
            var book = _unitOfWork.BookRepository.FirstOrDefault(b => b.Id == idInt32);
            if (book == null)
                return NotFound();
            book.IsAvailable = true;
            _unitOfWork.Save();

            return Ok("Book Create successfully");
        }
        [Route("api/book/{id}")]
        public IHttpActionResult PutBook(int id, [FromBody] BookUpdateDTO updateDTO)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);
                if (id != updateDTO.Id)
                    return BadRequest("Id does not match");

                var bookModel = _mapper.Map<Book>(updateDTO);
                var rnd = new Random();
                var updateOtp = _mapper.Map<UpdateOtp>(bookModel);
                updateOtp.OtpCode = (rnd.Next(1000000)).ToString();

                _unitOfWork.UpdateOtpRepository.Add(updateOtp);
                //Send Email
                var bookFromDb = _unitOfWork.BookRepository.FirstOrDefault(b => b.Id == id);
                if (bookFromDb == null)
                    return NotFound();

                //Generate Jwt Token
                var claims = new[]
                {
                    new Claim("bookId", bookFromDb.Id.ToString()),
                    new Claim("otpCode", updateOtp.OtpCode)
                };
                var tokenString = _jwtHandler.EncodeJwtToken(claims);
                var ownerEmail = bookFromDb.Owner.Email;
                var urlToConfirm = HttpContext.Current.Request.Url.Authority + "/api/book/ConfirmUpdate/";

                var htmlToSend = $"<p>Your OTP to update your book is <b>{updateOtp.OtpCode}</b> or click the button below to confirm update your book</p>" +
                                 $"<ul><li>{bookFromDb.Id}</li><li>{bookFromDb.ISBN}</li><li>{bookFromDb.Title}</li></ul>" +
                                 $"<form action=\"https://{urlToConfirm}\" method=\"POST\">\r\n  " +
                                 $"<input type=\"hidden\" id=\"jwtToken\" name=\"jwtToken\" value=\"{tokenString}\" />" +
                                 $"\r\n  <button type=\"submit\">Confirm Update</button>\r\n</form>";
                _emailSender.SendEmailAsync(ownerEmail, "Update Book - Libanon", htmlToSend);
                _unitOfWork.Save();
                return Ok("Book update successfully - pls confirm your email");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        
        //Update confirm via OTP
        [Route("api/book/ConfirmUpdate/{otp}")]
        public IHttpActionResult PostUpdateOtp(string otp, [FromBody] JObject data)
        {
            try
            {
                var otpBody = data.ContainsKey("OtpCode") ? data["OtpCode"].ToString() : "";

                if (otp != otpBody)
                    return BadRequest();


                var otpUpdateFromDb = _unitOfWork.UpdateOtpRepository.FirstOrDefault(u => u.OtpCode == otpBody);
                if (otpUpdateFromDb == null)
                    return BadRequest("Otp Code is not valid");

                var bookFromDb = _mapper.Map<Book>(otpUpdateFromDb);

                _unitOfWork.BookRepository.Update(bookFromDb);
                _unitOfWork.UpdateOtpRepository.Remove(otpUpdateFromDb);
                _unitOfWork.Save();
                return Ok("Your book update successfully");


            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        //Update confirm
        [Route("api/book/ConfirmUpdate/")]
        public IHttpActionResult PostConfirmUpdate([FromBody] JObject data)
        {
            try
            {
                var jsonData = _jwtHandler.DecodeJwtToken(data["jwtToken"].ToString());
                var idInt32 = Convert.ToInt32(jsonData["bookId"]);
                if (idInt32 <= 0)
                    return NotFound();

                var otpCode = jsonData.ContainsKey("otpCode") ? jsonData["otpCode"] : "";

                var otpFromDb = _unitOfWork.UpdateOtpRepository.FirstOrDefault(u => u.OtpCode == otpCode);

                if (otpFromDb == null)
                    return BadRequest("Otp Code is not valid");

                var updatedBook = _mapper.Map<Book>(otpFromDb);
                _unitOfWork.BookRepository.Update(updatedBook);
                _unitOfWork.UpdateOtpRepository.Remove(otpFromDb);
                _unitOfWork.Save();
                return Ok("Book update successfully");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [HttpPost]
        [Route("api/book/delete/confirm")]
        public IHttpActionResult DeleteConfirm([FromBody] JObject data)
        {
            var jsonData = _jwtHandler.DecodeJwtToken(data["jwtToken"].ToString());
            var bookId = Convert.ToInt32(jsonData.ContainsKey("bookId") ? (jsonData["bookId"]) : "0");
            if (bookId == 0)
                return BadRequest("Book Id invalid");
            var bookFromDb = _unitOfWork.BookRepository.FirstOrDefault(b => b.Id == bookId);
            if (bookFromDb == null)
                return BadRequest("Book id is not exists");

            _unitOfWork.BookRepository.Remove(bookFromDb);
            _unitOfWork.Save();
            return Ok("Book deleted successfully");

        }
        [HttpDelete]
        [Route("api/book/delete/{bookId}")]
        public IHttpActionResult Delete(int bookId)
        {
            if(bookId <= 0) return BadRequest();
            var bookFromDb = _unitOfWork.BookRepository.FirstOrDefault(b => b.Id == bookId);
            if(bookFromDb == null) return BadRequest();

            var claims = new Claim[]
            {
                new Claim("bookId", bookId.ToString())
            };
            var tokenString = _jwtHandler.EncodeJwtToken(claims);
            var ownerEmail = bookFromDb.Owner.Email;
            var urlToConfirm = HttpContext.Current.Request.Url.Authority + "/api/book/delete/confirm";

            var htmlToSend = $"<p>Please click the button below to confirm deleting your book</p>" +
                             $"<ul><li>{bookFromDb.Id}</li><li>{bookFromDb.ISBN}</li><li>{bookFromDb.Title}</li></ul>" +
                             $"<form action=\"https://{urlToConfirm}\" method=\"POST\">\r\n  " +
                             $"<input type=\"hidden\" id=\"jwtToken\" name=\"jwtToken\" value=\"{tokenString}\" />" +
                             $"\r\n  <button type=\"submit\">Confirm Delete</button>\r\n</form>";
            _emailSender.SendEmailAsync(ownerEmail, "Delete Book - Libanon", htmlToSend);
            return Ok("Book deleted request sent successfully");
        }
        #endregion

        #region Borrow
        [Route("api/book/borrow/{bookId}")]
        //Borrower send
        public IHttpActionResult PostBorrowBook(int bookId, [FromBody] BookBorrowerCreateDTO createDTO)
        {
            try
            {
                // Check
                if (!ModelState.IsValid || bookId <= 0)
                    return BadRequest();

                var bookFromDb = _unitOfWork.BookRepository.FirstOrDefault(b => b.Id == bookId);

                if (bookFromDb == null || bookFromDb.BorrowerId != null)
                    return BadRequest("Book does not exist or book was borrowed");

                //Add Or Update borrower
                var borrowerDb = _unitOfWork.BorrowerRepository.FirstOrDefault(br => br.Email == createDTO.Email);

                var borrowerModel = _mapper.Map<BookBorrower>(createDTO);
                if (borrowerDb == null)
                {
                    _unitOfWork.BorrowerRepository.Add(borrowerModel);
                }
                else
                {
                    borrowerModel.Id = borrowerDb.Id;
                    _unitOfWork.BorrowerRepository.Update(borrowerModel);
                }
                _unitOfWork.Save();

                var urlToConfirm = HttpContext.Current.Request.Url.Authority + $"/api/book/ConfirmBorrow/?bookId={bookId}&borrowerId={borrowerModel.Id}";


                _emailSender.SendEmailAsync(
                    createDTO.Email,
                    "Confirm Borrow Book - Libanon",
                    "<p>Please click the button below to confirm borrow a book</p>" +
                    $"<ul><li>{bookFromDb.Id}</li><li>{bookFromDb.ISBN}</li><li>{bookFromDb.Title}</li></ul>" +
                    $"<a href=\"https://{urlToConfirm}\"><button>Confirm</button></a>");
                return Ok("Pls confirm borrow a book from email");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        //Confirm Deliver - Receive
        [Route("api/book/ConfirmDeliver")]
        public IHttpActionResult PostConfirmDeliver([FromBody] JObject data)
        {
            var jsonData = _jwtHandler.DecodeJwtToken(data["jwtToken"].ToString());
            var isDelivered = Convert.ToBoolean(jsonData.ContainsKey("isDelivered") ? jsonData["isDelivered"] : "false");
            var isReceived = Convert.ToBoolean(jsonData.ContainsKey("isReceived") ? jsonData["isReceived"] : "false");
            var borrowingId = Convert.ToInt32(jsonData.ContainsKey("borrowingId") ? jsonData["borrowingId"] : "0");
            var bookId = Convert.ToInt32(jsonData.ContainsKey("bookId") ? jsonData["bookId"] : "0");

            if (isReceived)
                _unitOfWork.BorrowingRepository.SetReceived(borrowingId);
            if (isDelivered)
            {
                var bookFromDb = _unitOfWork.BookRepository.FirstOrDefault(b => b.Id == bookId);
                var emailToSend = "<p>Your book borrowed request has been cancelled because the owner lent someone</p>" +
                                  $"<ul><li>{bookFromDb.Id}</li><li>{bookFromDb.ISBN}</li><li>{bookFromDb.Title}</li></ul>";
                                  _unitOfWork.BorrowingRepository.SetDelivered(borrowingId);

                var borrowings = _unitOfWork.BorrowingRepository.GetAll(br => br.Id != borrowingId);
                foreach (var item in borrowings)
                {
                    //Get borrower to send email
                    var borrowerDb = _unitOfWork.BorrowerRepository.FirstOrDefault(br => br.Id == item.BorrowerId);

                    if (borrowerDb != null)
                        _emailSender.SendEmailAsync(borrowerDb.Email, "Someone lent your book borrowed request",
                            emailToSend);

                    if(item.BookId == bookId)
                        _unitOfWork.BorrowingRepository.Remove(item);
                }
            }
            //Save
            _unitOfWork.Save();
            var isBorrowed = _unitOfWork.BorrowingRepository.IsBorrowed(borrowingId);

            if (isBorrowed)
            {
                var bookFromDb = _unitOfWork.BookRepository.FirstOrDefault(b => b.Id == bookId);
                bookFromDb.Borrower = _unitOfWork.BorrowingRepository.FirstOrDefault(br => br.Id == borrowingId).Borrower;
                _unitOfWork.Save();

            }
            return Ok("Book borrow successfully");
        }
        [Route("api/book/ConfirmBorrow/")]
        //Owner Accept or Reject
        public IHttpActionResult PostConfirmBorrow([FromBody] JObject data)
        {
            var jsonData = _jwtHandler.DecodeJwtToken(data["jwtToken"].ToString());
            var bookId = Convert.ToInt32(jsonData["bookId"]);
            var borrowerId = Convert.ToInt32(jsonData["borrowerId"]);
            var isAccepted = Convert.ToBoolean(jsonData["isAccepted"]);

            var bookFromDb = _unitOfWork.BookRepository.FirstOrDefault(b => b.Id == bookId);
            var borrowerFromDb = _unitOfWork.BorrowerRepository.FirstOrDefault(br => br.Id == borrowerId);
            //Reject
            if (!isAccepted)
            {
                _emailSender.SendEmailAsync(
                    borrowerFromDb.Email,
                    "Rejected your borrowing - Libanon",
                    $"<p>Your borrowing request has been rejected by owner {bookFromDb.Owner.Email}</ p > " +
                    $"<ul><li>{bookFromDb.Id}</li><li>{bookFromDb.ISBN}</li><li>{bookFromDb.Title}</li></ul>"
                   
                );
                _emailSender.SendEmailAsync(
                    bookFromDb.Owner.Email,
                    "Rejected your borrowing - Libanon",
                    $"<p>You has been rejected {borrowerFromDb.Email} borrowing request for your book</ p > " +
                    $"<ul><li>{bookFromDb.Id}</li><li>{bookFromDb.ISBN}</li><li>{bookFromDb.Title}</li></ul>"
                );
                return Ok("Reject borrow");
            }
            //Accept
            var bookBorrowing = new BookBorrowing
            {
                BorrowerId = borrowerId,
                BookId = bookId
            };
            _unitOfWork.BorrowingRepository.Add(bookBorrowing);
            _unitOfWork.Save();

            var urlToConfirm = HttpContext.Current.Request.Url.Authority + "/api/book/ConfirmDeliver/";

            var ownerClaims = new[]
            {
                new Claim("borrowingId", bookBorrowing.Id.ToString()),
                new Claim("bookId", bookFromDb.Id.ToString()),
                new Claim("isDelivered", "true")
            };
            var borrowerClaims = new[]
            {
                new Claim("borrowingId", bookBorrowing.Id.ToString()),
                new Claim("bookId", bookFromDb.Id.ToString()),
                new Claim("isReceived", "true")
            };

            var ownerToken = _jwtHandler.EncodeJwtToken(ownerClaims);
            var borrowerToken = _jwtHandler.EncodeJwtToken(borrowerClaims);

            _emailSender.SendEmailAsync(
                bookFromDb.Owner.Email,
                "Confirm Deliver Book - Libanon",
                $"<p>Please click the button below to confirm you has delivered your book to {borrowerFromDb.Email}</ p > "
                +
                $"<ul><li>{bookFromDb.Id}</li><li>{bookFromDb.ISBN}</li><li>{bookFromDb.Title}</li></ul>" +
                $"<form action=\"https://{urlToConfirm}\" method=\"POST\">\r\n  " +
                $"<input type=\"hidden\" id=\"jwtToken\" name=\"jwtToken\" value=\"{ownerToken}\" />" +
                $"\r\n  <button type=\"submit\">Confirm</button>\r\n</form>"
                + $"<form action=\"https://{urlToConfirm}\" method=\"POST\">\r\n "
            );
            _emailSender.SendEmailAsync(
                borrowerFromDb.Email,
                "Confirm Receive Book - Libanon",
                $"<p>Please click the button below to confirm you has received book from {bookFromDb.Owner.Email}</ p > "
                +
                $"<ul><li>{bookFromDb.Id}</li><li>{bookFromDb.ISBN}</li><li>{bookFromDb.Title}</li></ul>" +
                $"<form action=\"https://{urlToConfirm}\" method=\"POST\">\r\n  " +
                $"<input type=\"hidden\" id=\"jwtToken\" name=\"jwtToken\" value=\"{borrowerToken}\" />" +
                $"\r\n  <button type=\"submit\">Confirm</button>\r\n</form>"
                + $"<form action=\"https://{urlToConfirm}\" method=\"POST\">\r\n "
            );
            return Ok("Accept borrow");
        }
        [Route("api/book/ConfirmBorrow/")]
        //Borrower Confirm
        public IHttpActionResult GetConfirmBorrow(string bookId, string borrowerId)
        {
            try
            {
                var borrowerIdInt = Convert.ToInt32(borrowerId);
                var bookIdInt = Convert.ToInt32(bookId);
                var bookFromDb = _unitOfWork.BookRepository.FirstOrDefault(b => b.Id == bookIdInt);
                if (bookFromDb == null)
                    return BadRequest();

                var urlToConfirm = HttpContext.Current.Request.Url.Authority + "/api/book/ConfirmBorrow/";
                var acceptedClaims = new[]
                {
                    new Claim("bookId", bookId),
                    new Claim("borrowerId", borrowerId),
                    new Claim("isAccepted", "true")
                };
                var rejectedClaims = new[]
                {
                    new Claim("bookId", bookId),
                    new Claim("borrowerId", borrowerId),
                    new Claim("isAccepted", "false")
                };
                var acceptedTokenString = _jwtHandler.EncodeJwtToken(acceptedClaims);
                var rejectedTokenString = _jwtHandler.EncodeJwtToken(rejectedClaims);
                var borrowerDb = _unitOfWork.BorrowerRepository.FirstOrDefault(br => br.Id == borrowerIdInt);

                _emailSender.SendEmailAsync(
                    bookFromDb.Owner.Email,
                    "Confirm Borrow Book - Libanon",
                    $"<p>Please click the button below to let your book borrowed by email {borrowerDb.Email}</ p > "
                    +
                    $"<ul><li>{bookFromDb.Id}</li><li>{bookFromDb.ISBN}</li><li>{bookFromDb.Title}</li></ul>" +
                    $"<form action=\"https://{urlToConfirm}\" method=\"POST\">\r\n  " +
                    $"<input type=\"hidden\" id=\"jwtToken\" name=\"jwtToken\" value=\"{acceptedTokenString}\" />" +
                    $"\r\n  <button type=\"submit\">Accept</button>\r\n</form>"
                    + $"<form action=\"https://{urlToConfirm}\" method=\"POST\">\r\n  " +
                    $"<input type=\"hidden\" id=\"jwtToken\" name=\"jwtToken\" value=\"{rejectedTokenString}\" />" +
                    $"\r\n  <button type=\"submit\">Reject</button>\r\n</form>"
                );
                return Ok("Confirm borrow a book");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        #endregion

        #region Return Book

        [Route("api/book/return/confirm")]
        public IHttpActionResult PostConfirmReturn([FromBody] JObject data)
        {
            try
            {
                var jsonData = _jwtHandler.DecodeJwtToken(data["jwtToken"].ToString());
                var isReturned = Convert.ToBoolean(jsonData.ContainsKey("isReturned") ? jsonData["isReturned"] : "false");
                var isReceived = Convert.ToBoolean(jsonData.ContainsKey("isReceived") ? jsonData["isReceived"] : "false");
                var borrowingId = Convert.ToInt32(jsonData.ContainsKey("borrowingId") ? jsonData["borrowingId"] : "0");
                var bookId = Convert.ToInt32(jsonData.ContainsKey("bookId") ? jsonData["bookId"] : "0");

                var borrowingDb = _unitOfWork.BorrowingRepository.FirstOrDefault(brg => brg.Id == borrowingId);
                if (isReturned)
                {
                    borrowingDb.IsReturned = true;
                }
                if (isReceived)
                {
                    borrowingDb.IsOwnerReceived = true;
                }
                _unitOfWork.Save();
                if (borrowingDb.IsReturned && borrowingDb.IsOwnerReceived)
                {
                    var bookFromDb = _unitOfWork.BookRepository.FirstOrDefault(b => b.Id == bookId);

                    if (bookFromDb == null)
                        return BadRequest();

                    bookFromDb.BorrowerId = null;
                    _unitOfWork.BorrowingRepository.Remove(borrowingDb);
                    _unitOfWork.Save();
                }
                return Ok("Return book successfully");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [Route("api/book/return/confirm")]
        public IHttpActionResult GetConfirmReturn(string bookId, string borrowingId)
        {
            try
            {
                var bookIdInt32 = Convert.ToInt32(bookId);
                var borrowingIdInt32 = Convert.ToInt32(borrowingId);

                var bookFromDb = _unitOfWork.BookRepository.FirstOrDefault(b => b.Id == bookIdInt32);
                if (bookFromDb == null)
                    return BadRequest();

                var borrowerClaims = new[]
                {
                    new Claim("bookId", bookId),
                    new Claim("borrowingId", borrowingId),
                    new Claim("isReturned", "true")
                };
                var ownerClaims = new[]
                {
                    new Claim("bookId", bookId),
                    new Claim("borrowingId", borrowingId),
                    new Claim("isReceived", "true")
                };
                var borrowerTokenString = _jwtHandler.EncodeJwtToken(borrowerClaims);
                var ownerTokenString = _jwtHandler.EncodeJwtToken(ownerClaims);
                var urlToConfirm = HttpContext.Current.Request.Url.Authority + "/api/book/return/confirm";

                _emailSender.SendEmailAsync(
                    bookFromDb.Owner.Email,
                    "Confirm Receive Borrowed Book - Libanon",
                    $"<p>Please click the button below to confirm you received your book from email {bookFromDb.Borrower.Email}</ p > "
                    +
                    $"<ul><li>{bookFromDb.Id}</li><li>{bookFromDb.ISBN}</li><li>{bookFromDb.Title}</li></ul>" +
                    $"<form action=\"https://{urlToConfirm}\" method=\"POST\">\r\n  " +
                    $"<input type=\"hidden\" id=\"jwtToken\" name=\"jwtToken\" value=\"{ownerTokenString}\" />" +
                    "\r\n  <button type=\"submit\">I received my book</button>\r\n</form>"
                );
                _emailSender.SendEmailAsync(
                    bookFromDb.Borrower.Email,
                    "Confirm Return Book - Libanon",
                    $"<p>Please click the button below to confirm you return a book to owner  {bookFromDb.Owner.Email}</ p > "
                    +
                    $"<ul><li>{bookFromDb.Id}</li><li>{bookFromDb.ISBN}</li><li>{bookFromDb.Title}</li></ul>" +
                    $"<form action=\"https://{urlToConfirm}\" method=\"POST\">\r\n  " +
                    $"<input type=\"hidden\" id=\"jwtToken\" name=\"jwtToken\" value=\"{borrowerTokenString}\" />" +
                    "\r\n  <button type=\"submit\">I returned a book</button>\r\n</form>"
                );
                return Ok("Please confirm return and receive book");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [Route("api/book/return/{bookId}")]
        //Borrower Return
        public IHttpActionResult PostReturnBook(int bookId, [FromBody] BookBorrowerUpdateDTO updateDTO)
        {
            try
            {
                // Check
                if (!ModelState.IsValid || bookId <= 0)
                    return BadRequest(ModelState);

                var bookFromDb = _unitOfWork.BookRepository.FirstOrDefault(b => b.Id == bookId);

                if (bookFromDb == null || bookFromDb.BorrowerId == null)
                    return BadRequest("Book does not exist or book was NOT borrowed");

                //Update borrower

                var borrowerModel = _unitOfWork.BorrowerRepository.FirstOrDefault(br => br.Email == updateDTO.Email);

                if (borrowerModel == null)
                    return BadRequest("Email borrower does not exist");

                _unitOfWork.BorrowerRepository.Update(borrowerModel);
                _unitOfWork.Save();
                //Send Email
                var borrowingDb = _unitOfWork.BorrowingRepository.FirstOrDefault(brg =>
                    brg.BookId == bookId && brg.BorrowerId == bookFromDb.BorrowerId);

                if (borrowingDb == null)
                    return BadRequest("Conflict borrowing");

                var urlToConfirm = HttpContext.Current.Request.Url.Authority + $"/api/book/return/confirm/?bookId={bookId}&borrowingId={borrowingDb.Id}";

                _emailSender.SendEmailAsync(
                    updateDTO.Email,
                    "Confirm Return Book - Libanon",
                    $"<p>Please click the button below to confirm return a book to {bookFromDb.Owner.Email} </p>" +
                    $"<ul><li>{bookFromDb.Id}</li><li>{bookFromDb.ISBN}</li><li>{bookFromDb.Title}</li></ul>" +
                    $"<a href=\"https://{urlToConfirm}\"><button>Return</button></a>");
                return Ok("Return request book created successfully");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        #endregion
        [Route("api/book/rating/{bookId}")]
        public IHttpActionResult PostRating(int bookId, [FromBody] RatingCreateDTO ratingCreateDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);
                if (ratingCreateDto.Point < 1 || ratingCreateDto.Point > 5)
                    return BadRequest("Rating point is invalid");
                if (bookId != ratingCreateDto.BookId)
                    return BadRequest();

                var bookFromDb = _unitOfWork.BookRepository.FirstOrDefault(b => b.Id == ratingCreateDto.BookId);

                if (bookFromDb == null)
                    return BadRequest();

                var bookRating = new BookRating
                {
                    BookID = ratingCreateDto.BookId,
                    Point = ratingCreateDto.Point,
                    ISBN = bookFromDb.ISBN
                };
               

                

                _unitOfWork.BookRatingRepository.Add(bookRating);
                _unitOfWork.Save();

                var listBookRatings = _unitOfWork.BookRatingRepository.GetAll(r => r.ISBN == bookFromDb.ISBN).ToList();

                var averageRating = listBookRatings.Average(item => item.Point);
                var bookLists = _unitOfWork.BookRepository.GetAll(r => r.ISBN == bookFromDb.ISBN).ToList();
                foreach (var item in bookLists)
                {
                    item.AverageRating = averageRating;
                }

                _unitOfWork.Save();
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
       
        //Test API
        //[Route("api/book/gettest")]
        //public IHttpActionResult GetTest()
        //{
        //    var book7 = _unitOfWork.BookRepository.FirstOrDefault(b => b.Id == 7);
        //    if (book7 == null)
        //        return NotFound();
        //    book7.BorrowerId = null;
        //    _unitOfWork.Save();
        //    return Ok(); 
        //}
    }
}