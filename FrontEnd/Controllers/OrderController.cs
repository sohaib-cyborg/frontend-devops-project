using FrontEnd.Services.Orders;
using Microsoft.AspNetCore.Mvc;

namespace FrontEnd.Controllers
{
    public class OrderController : Controller
    {
        private readonly IOrderServices _orderServices;

        public OrderController(IOrderServices orderServices)
        {
            _orderServices = orderServices;
        }
        public async Task<IActionResult> Index(string OrderId)
        {
            if(OrderId != null)
            {
                var order = await _orderServices.GetOrderById(OrderId);
                if(order != null)
                {
                    return View(order);
                }
                TempData["Failure"] = "Order with Id " + OrderId + " does not exist";
            }
            var model = await _orderServices.GetUserOrders();
            if(model == null)
            {
                return NotFound();
            }
            return View(model);
        }
        public async Task<IActionResult> OrderDetails(Guid id)
        {
            var model = await _orderServices.GetOrderDetails(id);
            if (model == null)
            {
                return NotFound();
            }
            return View(model);
        }
        public async Task<IActionResult> AllOrders(string OrderId)
        {
            if ( OrderId is not null)
            {
                var order = await _orderServices.GetOrderById(OrderId);
                return View("SearchOrder",order);
            }
            var model = await _orderServices.GetAllOrders();
            if (model == null)
            {
                return NotFound();
            }
            return View(model);
        }
    }
}
