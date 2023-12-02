namespace FrontEnd.Models
{
    public class CreateOrderViewModel
    {
        public Guid? AddressId { get; set; }
        public Guid? PaymentMethodId { get; set; }
        public List<OrderItemViewModel> Products { get; set; }
    }
}
