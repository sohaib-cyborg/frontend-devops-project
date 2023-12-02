namespace FrontEnd.Models
{
    public class AddressPaymentViewModel
    {
        public List<ProductViewModel> Product { get; set; }
        public List<AddressViewModel> Address { get; set; }
        public List<CustomerPaymentViewModel> PaymentMethod { get; set; }
        public Guid? AddressId { get; set; }    
        public Guid? PaymentMethodId { get; set; }
    }
}
