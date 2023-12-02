using FrontEnd.Models;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;
using static System.Net.WebRequestMethods;

namespace FrontEnd.Services.Address
{
    public class AddressServices : IAddressServices
    {
        private string baseAddress;
        private IHttpContextAccessor _httpContextAccessor;
        private readonly IHttpClientFactory _clientFactory;

        public AddressServices(IHttpContextAccessor httpContextAccessor,IHttpClientFactory clientFactory)
        {
            baseAddress = "http://localhost:5000/api/";
            _httpContextAccessor = httpContextAccessor;
            _clientFactory = clientFactory;
        }

        public async Task<bool> Add(Guid id, AddAddressViewModel model)
        {
            string userId = _httpContextAccessor.HttpContext.Request.Cookies["userId"];
            string token = string.Empty;
            if (_httpContextAccessor.HttpContext.Request.Cookies.TryGetValue("token", out var cookieValue))
            {
                token = cookieValue;
            }
            string endpoint = baseAddress + "Address/Add?userId=" + userId;
            var payload = JsonConvert.SerializeObject(model);
            using(var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                client.BaseAddress = new Uri(endpoint);
                var requestMessage = new HttpRequestMessage(HttpMethod.Post,endpoint);
                requestMessage.Content = new StringContent(payload,Encoding.UTF8,"application/json");
                var responseMessage = await client.SendAsync(requestMessage);
                if(responseMessage.IsSuccessStatusCode)
                {
                    return true;
                }
                return false;
            }
        }

        public void Delete(Guid id)
        {
            string endpoint = baseAddress + "Address/Delete?AddressId=" + Convert.ToString(id);
            string token = string.Empty;
            if (_httpContextAccessor.HttpContext.Request.Cookies.TryGetValue("token", out var cookieValue))
            {
                token = cookieValue;
            }
            var client = _clientFactory.CreateClient();   
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            client.BaseAddress = new Uri(endpoint);
            var requestMessage = new HttpRequestMessage(HttpMethod.Delete, endpoint);
            client.Send(requestMessage);
            
        }

        public async Task<AddressViewModel> GetAddressById(Guid id)
        {
            string endpoint = baseAddress + "Address/GetAddress?id=" + Convert.ToString(id);
            string token = string.Empty;
            if (_httpContextAccessor.HttpContext.Request.Cookies.TryGetValue("token", out var cookieValue))
            {
                token = cookieValue;
            }
            var client = _clientFactory.CreateClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var result = await client.GetAsync(endpoint);
                if (result.IsSuccessStatusCode)
                {
                    var response = await result.Content.ReadAsStringAsync();
                    var model = JsonConvert.DeserializeObject<AddressViewModel>(response);
                    return model;
                }
                return null;
            

        }

        public async Task<List<AddressViewModel>> GetUserAddress()
        {
            try
            {
                string userId = _httpContextAccessor.HttpContext.Request.Cookies["userId"];
                string token = string.Empty;
                if (_httpContextAccessor.HttpContext.Request.Cookies.TryGetValue("token", out var cookieValue))
                {
                    token = cookieValue;
                }
                string endpoint = baseAddress + "Address/Index?userId=" + userId;
                var client = _clientFactory.CreateClient();
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    var result = await client.GetAsync(endpoint);
                    if (result.IsSuccessStatusCode)
                    {
                        var response = await result.Content.ReadAsStringAsync();
                        var model = JsonConvert.DeserializeObject<List<AddressViewModel>>(response);
                        return model;
                    }
                    return null;
                
            }
            catch (Exception ex) { return null; }
        }

        public async Task<bool> Update(AddressViewModel model, Guid id)
        {
            string endpoint = baseAddress + "Address/Update?AddressId=" + Convert.ToString(id);
            string token = string.Empty;
            if (_httpContextAccessor.HttpContext.Request.Cookies.TryGetValue("token", out var cookieValue))
            {
                token = cookieValue;
            }
            var payload = JsonConvert.SerializeObject(model);
            var client = _clientFactory.CreateClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var requestMessage = new HttpRequestMessage(HttpMethod.Put, endpoint);
                requestMessage.Content = new StringContent(payload,Encoding.UTF8,"application/json");
                var response = await client.SendAsync(requestMessage);
                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                return false;
            


        }
    }
}
