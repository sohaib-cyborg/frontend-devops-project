using FrontEnd.Models;
using FrontEnd.Services.Address;
using FrontEnd.Services.CustomerPayments;
using FrontEnd.Services.Orders;
using FrontEnd.Services.Products;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace FrontEnd.Controllers
{
    public class CartController : Controller
    {
        private readonly IProductServices _ProductServices;
        private readonly IOrderServices _OrderServices;
        private readonly IAddressServices _AddressServices;
        private readonly ICustomerPayment _CustomerPaymentServices;
        public CartController(IProductServices productServices,IOrderServices orderServices,IAddressServices addressServices,ICustomerPayment customerPaymentServices)
        {
            _ProductServices = productServices;
            _OrderServices = orderServices;
            _AddressServices = addressServices;
            _CustomerPaymentServices= customerPaymentServices;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var data = GetOrderItemsFromSession();
            return View(await data);
        }

        private async Task<List<ProductViewModel>> GetOrderItemsFromSession()
        {
            await HttpContext.Session.LoadAsync();
            var sessionString = HttpContext.Session.GetString("cart");
            if (sessionString != null)
            {
                return JsonSerializer.Deserialize<List<ProductViewModel>>(sessionString);
            }
            return Enumerable.Empty<ProductViewModel>().ToList();
        }
        public async Task<IActionResult> AddToCart(Guid id)
        {
            var data = await GetOrderItemsFromSession();

            var product = await _ProductServices.FindById(id);

            if (product is not null)
            {
                product.quantity = 1;
                data.Add(product);

                HttpContext.Session.SetString("cart", JsonSerializer.Serialize(data));

                TempData["Success"] = "Product Added Successfully!";
          
                return RedirectToAction("Index","Product");
            }

            return NotFound();
        }
        public async Task<IActionResult> AddressPayment()
        {
            var data =  await GetOrderItemsFromSession();
            if (data.Count == 0)
            {
                TempData["Failure"] = "Please add items to cart before checking out!";
                return RedirectToAction("Index", "Cart");
            }
            double total = 0;
            foreach(var item in data)
            {
                total += item.quantity*item.price;
            }
            var add = _AddressServices.GetUserAddress();
            var pm = _CustomerPaymentServices.GetPayments();
            AddressPaymentViewModel model = new AddressPaymentViewModel
            {
                Product = data,
                Address =await add,
                PaymentMethod =await pm
            };
            ViewBag.Total = total;
            return await Task.Run(()=>View("AddressPayment",model));
        }
        
        public async Task<IActionResult> CheckOut(AddressPaymentViewModel model)
        {
            var data = await GetOrderItemsFromSession();
            var orderId = await _OrderServices.CreateOrder(model, data);
            if (orderId != null)
            {
                HttpContext.Session.Clear();
                TempData["Success"] = "Order Placed Successfully! Order Id : " + orderId;
                return RedirectToAction("Index", "Order");
            }
            TempData["Failure"] = "Unable To Place Order!";
            return RedirectToAction("Index", "Home");
        }
        public async Task<IActionResult> Increase(Guid id,string UserId)
        {
            var data = await GetOrderItemsFromSession();
            var product = data.Find(x => x.productId == id);
            if(product is not null)
            {
                product.quantity += 1;
                HttpContext.Session.SetString("cart", JsonSerializer.Serialize(data));
                var RouteValue = new
                {
                    Id = UserId
                };
                return RedirectToAction("Index", "Cart", RouteValue);
            }
            return NotFound();

        }
        public async Task<IActionResult> Decrease(Guid id, string UserId)
        {
            var data = await GetOrderItemsFromSession();
            var product = data.Find(x => x.productId == id);
            if (product is not null)
            {
                product.quantity -= 1;
                if(product.quantity == 0)
                {
                    data.Remove(product);
                }
                HttpContext.Session.SetString("cart", JsonSerializer.Serialize(data));
                var RouteValue = new
                {
                    Id = UserId
                };
                return RedirectToAction("Index", "Cart", RouteValue);
            }
            return NotFound();

        }
        public async Task<IActionResult> Remove(Guid id, string UserId)
        {
            var data = await GetOrderItemsFromSession();
            var product = data.Find(x=>x.productId==id);

            if (product is not null)
            {
                data.Remove(product);
                Console.WriteLine(data);
                HttpContext.Session.SetString("cart", JsonSerializer.Serialize(data));

                TempData["Success"] = "Product Removed Successfully!";
                var RouteValue = new
                {
                    Id = UserId
                };
                return RedirectToAction("Index", "Cart", RouteValue);
            }

            return NotFound();
        }

    }
}
