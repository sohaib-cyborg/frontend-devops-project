using FrontEnd.Models;
using Newtonsoft.Json;
using NuGet.Common;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.IdentityModel.Tokens.Jwt;

namespace FrontEnd.Services
{
    public class AuthServices : IAuthServices
    {
        private string baseAddress;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IHttpClientFactory _clientFactory;

        public AuthServices(IHttpContextAccessor httpContextAccessor,IHttpClientFactory clientFactory)
        {
            this._httpContextAccessor = httpContextAccessor;
            this._clientFactory = clientFactory;
            this.baseAddress = "http://localhost:5000/api/";
        }
        public async Task<bool> Login(LoginRequestViewModel model)
        {
            try
            {
                string endpoint = baseAddress + "Auth/Login";
                string role = string.Empty;
                AuthResponseViewModel authViewModel = null;
                var payload = JsonConvert.SerializeObject(model);
                var httpContext = _httpContextAccessor.HttpContext;
                var client = _clientFactory.CreateClient();
                    client.BaseAddress = new Uri(endpoint);
                    var requestMessage = new HttpRequestMessage(HttpMethod.Post, endpoint);
                    requestMessage.Content = new StringContent(payload, Encoding.UTF8, "application/json");
                    var responseMessage = await client.SendAsync(requestMessage);
                    if (responseMessage.IsSuccessStatusCode)
                    {
                        var Response = await responseMessage.Content.ReadAsStringAsync();
                        authViewModel = JsonConvert.DeserializeObject<AuthResponseViewModel>(Response);
                        var handler = new JwtSecurityTokenHandler();
                        var decoded = handler.ReadJwtToken(authViewModel.token);
                        foreach(var items in decoded.Claims)
                        {
                            if(items.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role")
                            {
                                role = items.Value;
                            }
                        }
                        httpContext.Response.Cookies.Append("role", role, new CookieOptions
                        {
                            Expires = DateTime.Now.AddMinutes(60),
                            HttpOnly = true,
                        });
                        httpContext.Response.Cookies.Append("token", authViewModel.token, new CookieOptions
                        {
                            Expires = DateTime.Now.AddMinutes(60),
                            HttpOnly = true,
                        });
                        httpContext.Response.Cookies.Append("loggedIn", "true", new CookieOptions
                        {
                            Expires = DateTime.Now.AddMinutes(60),
                            HttpOnly = true,
                        });
                        httpContext.Response.Cookies.Append("userId", authViewModel.userId, new CookieOptions
                        {
                            Expires = DateTime.Now.AddMinutes(60),
                            HttpOnly = true,
                        });
                        return true;
                    }
                    return false;
            }
            catch(Exception ex) {
                return false;
            }
        }

        public async Task<UserViewModel> Details()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            string token = string.Empty;
            string userId =string.Empty;
            // Ensure that a token cookie with the correct name exists
            if (httpContext.Request.Cookies.TryGetValue("token", out var cookieValue))
            {
                token = cookieValue;
            }
            if (httpContext.Request.Cookies.TryGetValue("userId", out var cookieValue1))
            {
                userId = cookieValue1;
            }
            string endpoint = baseAddress + "Customer/GetCustomer?UserId=" + userId;
            var client = _clientFactory.CreateClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var result = await client.GetAsync(endpoint);
                if (result.IsSuccessStatusCode)
                {
                    var Response = await result.Content.ReadAsStringAsync();
                    var userViewModel = JsonConvert.DeserializeObject<UserViewModel>(Response);
                    return userViewModel;
                }
                return null;
        }

        public void Logout() { 
            var httpContext = _httpContextAccessor.HttpContext;
        foreach (var cookie in httpContext.Request.Cookies.Keys)
            {
                httpContext.Response.Cookies.Delete(cookie);
            }
        }

        public async Task<bool> Update(UserViewModel model)
        {
            string userId = _httpContextAccessor.HttpContext.Request.Cookies["userId"];
            string token = _httpContextAccessor.HttpContext.Request.Cookies["token"];
            var payload = JsonConvert.SerializeObject(model);
            if(model.userId == userId)
            {
                string endpoint = baseAddress + "Customer/Update?UserId=" + userId;
                var client = _clientFactory.CreateClient();
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    var requestMessage = new HttpRequestMessage(HttpMethod.Put, endpoint);
                    requestMessage.Content = new StringContent(payload, Encoding.UTF8, "application/json");
                    var responseMessage = await client.SendAsync(requestMessage);
                    if (responseMessage.IsSuccessStatusCode)
                    {
                        return true;
                    }
                    return false;
            }
            return false;
        }

        public async Task<List<UserViewModel>> GetCustomers()
        {
            string endpoint = baseAddress + "Customer/Index";
            string token = _httpContextAccessor.HttpContext.Request.Cookies["token"];
            var client = _clientFactory.CreateClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = await client.GetAsync(endpoint);
                if(response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    var list = JsonConvert.DeserializeObject<List<UserViewModel>>(result);
                    return list;
                }
                return null;
        }

        public async Task<bool> create(RegistrationViewModel model)
        {
            string endpoint = baseAddress + "Auth/Register";
            var payload = JsonConvert.SerializeObject(model);
            var client = _clientFactory.CreateClient();
                var requestMessage = new HttpRequestMessage(HttpMethod.Post, endpoint);
                requestMessage.Content = new StringContent(payload, Encoding.UTF8, "application/json");
                var responseMessage = await client.SendAsync(requestMessage);
                if (responseMessage.IsSuccessStatusCode)
                {
                    return true;
                }
                return false;
        }
    }
}
