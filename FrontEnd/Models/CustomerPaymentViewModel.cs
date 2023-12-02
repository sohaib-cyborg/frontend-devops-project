namespace FrontEnd.Models
{
    public class CustomerPaymentViewModel
    {
        public string userId { get; set; }  = string.Empty;
        public string provider { get; set; } = string.Empty; 
        public Guid paymentMethodId { get; set; }
    }
}
