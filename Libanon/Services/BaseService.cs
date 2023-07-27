using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using System.Web;
using Libanon.Models;
using Libanon.Services.IServices;
using Newtonsoft.Json;
using Unity;

namespace Libanon.Services
{
    public class BaseService : IBaseService
    {

        public ApiResponse ApiResponse { get; set; }

        public BaseService()
        {
            ApiResponse = new ApiResponse();
        }
        public async Task<T> SendAsync<T>(ApiRequest request)
        {
            try
            {
                var handler = new HttpClientHandler();
                handler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;
                var _client = new HttpClient(handler);
                _client.BaseAddress = new Uri(request.Url);
                HttpResponseMessage response;

                switch (request.ApiType)
                {
                    case SD.ApiType.GET:
                        response = await _client.GetAsync($"{request.RequestUrl}");
                        break;
                    case SD.ApiType.POST:
                        response = await _client.PostAsJsonAsync($"{request.RequestUrl}", request.Data);
                        break;
                    case SD.ApiType.PUT:
                        response = await _client.PutAsJsonAsync($"{request.RequestUrl}", request.Data);
                        break;
                    case SD.ApiType.DELETE:
                        response = await _client.DeleteAsync($"{request.RequestUrl}");
                        break;
                    default:
                        response = await _client.GetAsync($"{request.RequestUrl}");
                        break;
                }

                var result = await response.Content.ReadAsStringAsync();


                //ApiResponse.Result = JsonConvert.DeserializeObject<object>(result);
                ApiResponse.Result = result;
                ApiResponse.StatusCode = response.StatusCode;
                ApiResponse.IsSuccess = response.IsSuccessStatusCode;

                var res = JsonConvert.SerializeObject(ApiResponse);
                var returnObj = JsonConvert.DeserializeObject<T>(res);
                return returnObj;
            }
            catch (Exception e)
            {

                ApiResponse.StatusCode = HttpStatusCode.BadRequest;
                ApiResponse.Result = e.Message;

                var res = JsonConvert.SerializeObject(ApiResponse);
                var returnObj = JsonConvert.DeserializeObject<T>(res);
                return returnObj;
            }
        }
    }
}