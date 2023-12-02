using FrontEnd.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Newtonsoft.Json;
using NuGet.Common;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace FrontEnd.Services.CustomerPayments
{
    public class CustomerPayment : ICustomerPayment
    {
        private string baseAddress;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IHttpClientFactory _clientFactory;

        public CustomerPayment(IHttpContextAccessor httpContextAccessor,IHttpClientFactory clientFactory)
        {
            this._httpContextAccessor = httpContextAccessor;
            this._clientFactory = clientFactory;
            this.baseAddress = "http://localhost:5000/api/";
        }
        public async Task<List<CustomerPaymentViewModel>> GetPayments()
        {
            try
            {
                string userId = _httpContextAccessor.HttpContext.Request.Cookies["userId"];
                string token = string.Empty;
                if (_httpContextAccessor.HttpContext.Request.Cookies.TryGetValue("token", out var cookieValue))
                {
                    token = cookieValue;
                }
                string endpoint = baseAddress + "PaymentMethod/GetUserPaymentMethod?userId=" + userId;
                var client = _clientFactory.CreateClient();
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    var result = await client.GetAsync(endpoint);
                    if (result.IsSuccessStatusCode)
                    {
                        var Response = await result.Content.ReadAsStringAsync();
                        var PaymentMethods = JsonConvert.DeserializeObject<List<CustomerPaymentViewModel>>(Response);
                        return PaymentMethods;
                    }
                    return null;
                
            }catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<bool> AddMethod(AddPaymentMethodViewModel model)
        {
            try
            {
                string userId = _httpContextAccessor.HttpContext.Request.Cookies["userId"];
                string token = string.Empty;
                var payload = JsonConvert.SerializeObject(model);
                if (_httpContextAccessor.HttpContext.Request.Cookies.TryGetValue("token", out var cookieValue))
                {
                    token = cookieValue;
                }
                string endpoint = baseAddress + "PaymentMethod/Create?userId=" + userId;
                var client = _clientFactory.CreateClient();
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    client.BaseAddress = new Uri(endpoint);
                    var requestMessage = new HttpRequestMessage(HttpMethod.Post, endpoint);
                    requestMessage.Content = new StringContent(payload, Encoding.UTF8, "application/json");
                    var responseMessage = await client.SendAsync(requestMessage);
                    if (responseMessage.IsSuccessStatusCode)
                    {
                        return true;
                    }
                    return false;
                
            }catch(Exception ex) {
                return false;
            }
        }

        public async Task<AddPaymentMethodViewModel> GetPaymentById(Guid id)
        {
            string endpoint = baseAddress + "PaymentMethod/GetPaymentMethod?PaymentMethodId=" + Convert.ToString(id);
            string token = string.Empty;
            if (_httpContextAccessor.HttpContext.Request.Cookies.TryGetValue("token", out var cookieValue))
            {
                token = cookieValue;
            }
            var client = _clientFactory.CreateClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var result = await client.GetAsync(endpoint);
                if(result.IsSuccessStatusCode)
                {
                    var response = await result.Content.ReadAsStringAsync();
                    var model = JsonConvert.DeserializeObject<AddPaymentMethodViewModel>(response);
                    return model;
                }
                return null;
                
            

        }

        public async Task<bool> Update(AddPaymentMethodViewModel viewModel, Guid id)
        {
            string endpoint = baseAddress + "PaymentMethod/Update?PaymentMethodId=" + Convert.ToString(id);
            string token = string.Empty;
            if (_httpContextAccessor.HttpContext.Request.Cookies.TryGetValue("token", out var cookieValue))
            {
                token = cookieValue;
            }
            var payload = JsonConvert.SerializeObject(viewModel);
            var client = _clientFactory.CreateClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                client.BaseAddress = new Uri(endpoint);
                var requestMessage = new HttpRequestMessage(HttpMethod.Put, endpoint);
                requestMessage.Content = new StringContent(payload,Encoding.UTF8,"application/json");
                var response = await client.SendAsync(requestMessage);
                if(response.IsSuccessStatusCode)
                {
                    return true;
                }
                return false;

        }

        public async Task<bool> Delete(Guid id)
        {
            string endpoint = baseAddress + "PaymentMethod/Delete?id=" + Convert.ToString(id);
            string token = string.Empty;
            if (_httpContextAccessor.HttpContext.Request.Cookies.TryGetValue("token", out var cookieValue))
            {
                token = cookieValue;
            }
            var client = _clientFactory.CreateClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                client.BaseAddress = new Uri(endpoint);
                var requestMessage = new HttpRequestMessage(HttpMethod.Delete, endpoint);
                var response = await client.SendAsync(requestMessage);
                if(response.IsSuccessStatusCode)
                {
                    return true;
                }
                return false;
        }
    }
}
