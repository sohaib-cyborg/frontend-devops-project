using FrontEnd.Models;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace FrontEnd.Services.Products
{
    public class ProductServices : IProductServices
    {
        private IHttpContextAccessor _httpContextAccessor;
        private readonly IWebHostEnvironment env;
        private readonly IHttpClientFactory _clientFactory;
        private string baseAddress;
        public ProductServices(IHttpContextAccessor httpContextAccessor,IWebHostEnvironment env,IHttpClientFactory clientFactory)
        {
            this._httpContextAccessor = httpContextAccessor;
            this.env = env;
            this._clientFactory = clientFactory;
            this.baseAddress = "http://localhost:5000/api/";

        }

        

        public async Task<ProductViewModel> FindById(Guid id)
        {
            string endpoint = baseAddress + "Product/Product?ProductId=" + Convert.ToString(id);
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
                    var model = JsonConvert.DeserializeObject<ProductViewModel>(response);
                    return model;
                }
                return null;

        }
        private string SaveImage(IFormFile image)
        {
            var wwwPath = this.env.WebRootPath;
            var path = Path.Combine(wwwPath, "Uploads");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            var ext = Path.GetExtension(image.FileName);
            var allowedExtensions = new string[] { ".jpg", ".png", ".jpeg" };
            if (!allowedExtensions.Contains(ext))
            {
                string msg = string.Format("Only {0} extensions are allowed", string.Join(",", allowedExtensions));
            }
            string uniqueString = Guid.NewGuid().ToString();
            var newFileName = uniqueString + ext;
            var fileWithPath = Path.Combine(path, newFileName);
            var stream = new FileStream(fileWithPath, FileMode.Create);
            image.CopyTo(stream);
            stream.Close();
            return newFileName;
        }
        private void DeleteImage(string img)
        {
            if (img != null)
            {
                var wwwPath = this.env.WebRootPath;
                var path = Path.Combine(wwwPath, "Uploads\\", img);
                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                }
            }
        }

        public async Task<List<ProductViewModel>> GetProducts()
        {
            string endpoint = baseAddress + "Product/Products";
            var client = _clientFactory.CreateClient();
                var result = await client.GetAsync(endpoint);
                if (result.IsSuccessStatusCode)
                {
                    var Response = await result.Content.ReadAsStringAsync();
                    var list = JsonConvert.DeserializeObject<List<ProductViewModel>>(Response);
                    return list;
                }
                return null;
        }

        public async Task<bool> Add(AddProductViewModel model)
        {
            ProductModel product = new ProductModel()
            {
                name = model.name,
                description = model.description,
                category = model.category,
                price = model.price,
                quantity = model.quantity,
            };
            if(model.image != null)
            {
                product.image = SaveImage(model.image);
            }
            string endpoint = baseAddress + "Product/Create";
            var payload = JsonConvert.SerializeObject(product);
            string token = _httpContextAccessor.HttpContext.Request.Cookies["token"];
            var client = _clientFactory.CreateClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var requestMessage = new HttpRequestMessage(HttpMethod.Post, endpoint);
                requestMessage.Content = new StringContent(payload,Encoding.UTF8,"application/json");
                var response = await client.SendAsync(requestMessage);
                if(response.IsSuccessStatusCode)
                {
                    return true;
                }
                return false;
            
        }

        public async Task<bool> Update(Guid id, AddProductViewModel model)
        {
            var prod = await FindById(id);
            ProductViewModel req = new ProductViewModel()
            {
                productId = id,
                name = model.name,
                description = model.description,
                category = model.category,
                price = model.price,
                quantity = model.quantity,
            };
            if(model.image != null)
            {
                DeleteImage(prod.image);
                req.image = SaveImage(model.image);
            }
            string endpoint = baseAddress + "Product/Update";
            string token = _httpContextAccessor.HttpContext.Request.Cookies["token"];
            var payload = JsonConvert.SerializeObject(req);
            var client = _clientFactory.CreateClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var requestMessage = new HttpRequestMessage(HttpMethod.Put,endpoint);
                requestMessage.Content = new StringContent(payload,Encoding.UTF8,"application/json");
                var response = await client.SendAsync(requestMessage);
                if(response.IsSuccessStatusCode)
                {
                    return true;
                }
                return false;
        }

        public void Delete(Guid id)
        {
            string endpoint = baseAddress + "Product/Delete?productId=" + Convert.ToString(id);
            string token = _httpContextAccessor.HttpContext.Request.Cookies["token"];
            var client = _clientFactory.CreateClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var requestMessage = new HttpRequestMessage(HttpMethod.Delete,endpoint);
                var response = client.Send(requestMessage);
                if(response.IsSuccessStatusCode)
                {
                    return;
                }
        }
    }
}
