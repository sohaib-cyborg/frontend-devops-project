using FrontEnd.Models;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace FrontEnd.Services.Orders
{
    public class OrderServices : IOrderServices
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IHttpClientFactory _clientFactory;
        private string baseAddress;

        public OrderServices(IHttpContextAccessor httpContextAccessor,IHttpClientFactory clientFactory)
        {
            _httpContextAccessor = httpContextAccessor;
            this._clientFactory = clientFactory;
            baseAddress = "http://localhost:5000/api/";
        }
        public async Task<string> CreateOrder(AddressPaymentViewModel model, List<ProductViewModel> data)
        {
            List<OrderItemViewModel> items = data.Select(x => new OrderItemViewModel
            {
                productId = x.productId,
                price = x.price,
                quantity = x.quantity
            }).ToList();
            CreateOrderViewModel order = new CreateOrderViewModel()
            {
                AddressId = model.AddressId,
                PaymentMethodId = model.PaymentMethodId,
                Products = items
            };
            string userId = _httpContextAccessor.HttpContext.Request.Cookies["userId"];
            string endpoint = baseAddress + "Order/Create?userId=" + userId;
            string token = _httpContextAccessor.HttpContext.Request.Cookies["token"];
            var payload = JsonConvert.SerializeObject(order);
            var client = _clientFactory.CreateClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",token);
                client.BaseAddress = new Uri(endpoint);
                var requestMessage = new HttpRequestMessage(HttpMethod.Post, endpoint);
                requestMessage.Content = new StringContent(payload,Encoding.UTF8,"application/json");
                var response = await client.SendAsync(requestMessage);
                if(response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    return result;
                }
                return null;
        }

        public async Task<List<CustomerOrderViewModel>> GetAllOrders()
        {
            string endpoint = baseAddress + "Order/ShowAllOrders";
            string token = _httpContextAccessor.HttpContext.Request.Cookies["token"];
            var client = _clientFactory.CreateClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = await client.GetAsync(endpoint);
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    var model = JsonConvert.DeserializeObject<List<CustomerOrderViewModel>>(result);
                    return model;
                }
                return null;
        }

        public async Task<List<OrderViewModel>> GetOrderById(string result)
        {
            string endpoint = baseAddress + "Order/GetOrder?OrderId=" + result;
            string token = _httpContextAccessor.HttpContext.Request.Cookies["token"];
            var client = _clientFactory.CreateClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = await client.GetAsync(endpoint);
                if(response.IsSuccessStatusCode)
                {
                    var req = await response.Content.ReadAsStringAsync();
                    var model = JsonConvert.DeserializeObject<List<OrderViewModel>>(req);
                    return model;
                }
                return null;
        }

        public async Task<List<OrderDetailsViewModel>> GetOrderDetails(Guid id)
        {
            string endpoint = baseAddress + "Order/OrderDetails?trackingId=" + Convert.ToString(id);
            string token = _httpContextAccessor.HttpContext.Request.Cookies["token"];
            var client = _clientFactory.CreateClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = await client.GetAsync(endpoint);
                if(response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    var list = JsonConvert.DeserializeObject<List<OrderDetailsViewModel>>(result);
                    return list;
                }
                return null;
        }

        public async Task<List<OrderViewModel>> GetUserOrders()
        {
            string userId = _httpContextAccessor.HttpContext.Request.Cookies["userId"];
            string endpoint = baseAddress + "Order/ShowUserOrders?userId=" + userId;
            string token = _httpContextAccessor.HttpContext.Request.Cookies["token"];
            var client = _clientFactory.CreateClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var result = await client.GetAsync(endpoint);
                if(result.IsSuccessStatusCode)
                {
                    var response = await result.Content.ReadAsStringAsync();
                    var list = JsonConvert.DeserializeObject<List<OrderViewModel>>(response);
                    return list;
                }
                return null;
        }
    }
}
