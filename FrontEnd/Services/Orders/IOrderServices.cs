using FrontEnd.Models;

namespace FrontEnd.Services.Orders
{
    public interface IOrderServices
    {
        Task<string> CreateOrder(AddressPaymentViewModel model, List<ProductViewModel> data);
        Task<List<CustomerOrderViewModel>> GetAllOrders();
        Task<List<OrderViewModel>> GetOrderById(string result);
        Task<List<OrderDetailsViewModel>> GetOrderDetails(Guid id);
        Task<List<OrderViewModel>> GetUserOrders();
    }
}
